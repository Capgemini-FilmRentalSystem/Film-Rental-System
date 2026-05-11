using System.Net;

namespace FilmRentalStore.API.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest) { }
    }
}