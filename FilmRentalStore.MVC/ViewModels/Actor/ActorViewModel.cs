using System.ComponentModel.DataAnnotations;

namespace FilmRentalStore.MVC.ViewModels.Actor
{
    public class ActorViewModel
    {
        
            public int ActorId { get; set; }

            [Required]
            [StringLength(45)]
            public string FirstName { get; set; } = string.Empty;

            [Required]
            [StringLength(45)]
            public string LastName { get; set; } = string.Empty;
        }
    }
