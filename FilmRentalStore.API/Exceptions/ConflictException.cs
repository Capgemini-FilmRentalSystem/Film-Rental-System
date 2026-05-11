using System.Net;

namespace FilmRentalStore.API.Exceptions
{
    public class ConflictException : ApiException
    {
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict) { }
    }
}