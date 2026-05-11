using System.Net;

namespace FilmRentalStore.API.Exceptions
{
    public abstract class ApiException : Exception
    {
        public int StatusCode { get; }

        protected ApiException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = (int)statusCode;
        }
    }
}