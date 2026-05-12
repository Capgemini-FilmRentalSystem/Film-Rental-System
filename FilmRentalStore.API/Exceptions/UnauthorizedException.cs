using System.Net;

namespace FilmRentalStore.API.Exceptions
{
    public class UnauthorizedException : ApiException
    {
        public UnauthorizedException(string message = "Unauthorized")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}
