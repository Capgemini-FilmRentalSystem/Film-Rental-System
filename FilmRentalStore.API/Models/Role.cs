using System;
using System.Collections.Generic;

namespace FilmRentalStore.API.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleTitle { get; set; } = null!;

    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
