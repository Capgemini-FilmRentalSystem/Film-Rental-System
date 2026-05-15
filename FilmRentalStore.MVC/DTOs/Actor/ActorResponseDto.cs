namespace FilmRentalStore.MVC.DTOs.Actor
{
    public class ActorResponseDto
    {
        public int ActorId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime LastUpdate { get; set; }
        public string FullName => $"{FirstName} {LastName}";
    }
}
