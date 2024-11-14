using Microsoft.EntityFrameworkCore;
using PassIn.Communication.Requests;
using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Events.Register;
public class RegisterEventUseCase
{
    public ResponseEventJson Execute(RequestEventJson request)
    {
        Validate(request);

        var dbContext = new PassInDbContext();

        var entity = new Infrastructure.Entities.Event
        {   
            Title = request.Title,
            Details = request.Details,
            Maximum_Attendees = request.MaximumAttendees,
            Slug = request.Title.ToLower().Replace(" ", "-"),
        };

        dbContext.Events.Add(entity);
        dbContext.SaveChanges();

        return new ResponseEventJson
        {
            Id = entity.Id,
            Title = entity.Title,
            Details = entity.Details,
            MaximumAttendees = entity.Maximum_Attendees,
            AttendeesAmount = entity.Attendees.Count()
        };
    }

    public void Validate(RequestEventJson request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ErrorOnValidationException("O título é inválido.");
        }

        if (request.MaximumAttendees <= 0)
        {
            throw new ErrorOnValidationException("Número máximo de participantes inválido.");
        }

        if (string.IsNullOrWhiteSpace(request.Details))
        {
            throw new ErrorOnValidationException("Detalhes é inválido.");
        }
    }
}