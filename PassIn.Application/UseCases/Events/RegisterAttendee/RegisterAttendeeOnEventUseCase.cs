using System.Net.Mail;
using Microsoft.Extensions.Options;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;
using PassIn.Infrastructure.Entities;

namespace PassIn.Application.UseCases.Events.RegisterAttendee;

public class RegisterAttendeeOnEventUseCase
{
    private readonly PassInDbContext _dbContext;

    public RegisterAttendeeOnEventUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisterJson Execute(Guid eventId, RequestRegisterEventJson request)
    {
        Validate(eventId, request);

        var entity = new Infrastructure.Entities.Attendee
        {   
            Email = request.Email,
            Name = request.Name,
            Event_Id = eventId,
            Created_At = DateTime.UtcNow,
        };

        _dbContext.Attendees.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisterJson
        {
            Id = entity.Id,
            Mensagem = $"Participante {request.Name} registrado no evento com sucesso!"
        };
    }

    private void Validate(Guid eventId, RequestRegisterEventJson request)
    {
        var eventEntity = _dbContext.Events.Find(eventId);
        if (eventEntity is null)
            throw new NotFoundException("Evento com este ID não existe.");

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ErrorOnValidationException("O nome é inválido.");
        }

        var emailIsValid = EmailIsValid(request.Email);
        if (emailIsValid == false)
        {
            throw new ErrorOnValidationException("O email é inválido.");
        }

        var attendeeAlreadyRegistered = _dbContext
            .Attendees
            .Any(attendee => attendee.Email.Equals(request.Email) && attendee.Event_Id == eventId);
        
        if (attendeeAlreadyRegistered)
        {
            throw new ConflictException("Participante não pode registrar no mesmo evento duas vezes.");
        }

        var attendeesForEvent =  _dbContext.Attendees.Count(attendee => attendee.Event_Id == eventId);
        if (attendeesForEvent == eventEntity.Maximum_Attendees)
        {
            throw new ErrorOnValidationException($"Não há mais espaço para este evento. Limite {eventEntity.Maximum_Attendees} de participantes.");
        }
    }

    private bool EmailIsValid(string email)
    {
        try
        {
            new MailAddress(email);

            return true;
        }
        catch
        {
            return false;
        }
    }
}