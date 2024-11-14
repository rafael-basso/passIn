using PassIn.Communication.Responses;
using PassIn.Exceptions;
using PassIn.Infrastructure;

namespace PassIn.Application.UseCases.Checkins.DoCheckin;

public class DoAttendeeCheckinUseCase
{
    private readonly PassInDbContext _dbContext;

    public DoAttendeeCheckinUseCase()
    {
        _dbContext = new PassInDbContext();
    }

    public ResponseRegisterJson Execute(Guid attendeeId)
    {
        Validate(attendeeId);
        string attendeeName = GetNameById(attendeeId);

        var entity = new Infrastructure.Entities.CheckIn
        {
            Attendee_Id = attendeeId,
            Created_at = DateTime.UtcNow
        };

        _dbContext.CheckIns.Add(entity);
        _dbContext.SaveChanges();

        return new ResponseRegisterJson
        {
            Id = entity.Id,
            Mensagem = "Check-in para " + attendeeName + " realizado com sucesso."
        };
    }

    private void Validate(Guid attendeeId)
    {
        var existAttendee = _dbContext.Attendees.Any(attendee => attendee.Id == attendeeId);

        if (existAttendee == false)
        {
            throw new NotFoundException("Participante com este ID não foi encontrado.");
        }

        var existCheckin = _dbContext.CheckIns.Any(ch => ch.Attendee_Id == attendeeId);

        if (existCheckin)
        {
            throw new ConflictException("Participante não pode fazer check-in no mesmo evento.");
        }
    }

    private string GetNameById(Guid attendeeId)
    {
        var name = _dbContext.Attendees.Where(a => a.Id == attendeeId).Select(a => a.Name).FirstOrDefault();

        return name ?? string.Empty;
    }

}