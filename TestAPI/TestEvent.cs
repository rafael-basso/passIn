using PassIn.Application.UseCases.Events.Register;
using PassIn.Communication.Requests;
using PassIn.Exceptions;

namespace TestAPI
{
    [TestClass]
    public class TestEvent
    {
        [TestMethod]
        public void EventRequest_Test()
        {
            //var mockRequest = new RequestEventJson();

            var mockRequest = new RequestEventJson()
            {
                Details = "teste detalhes",
                MaximumAttendees = 2,
                Title = "teste titulo"
            };

            var validate = new RegisterEventUseCase();
            var message = "";

            try
            {
                validate.Validate(mockRequest);

                message = "Success";                

            }
            catch (ErrorOnValidationException ex)
            {
                message = ex.Message;
            }

            Assert.AreEqual("Success", message);
        }
    }
}