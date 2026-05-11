using System.Net;

namespace FilmRentalStore.API.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound) { }

        public NotFoundException(string entity, object key)
            : base($"{entity} with id '{key}' was not found.", HttpStatusCode.NotFound) { }
    }
}