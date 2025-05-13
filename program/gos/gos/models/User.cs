using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace gos.models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Parameter> Parameters { get; set; } = new List<Parameter>();
    public UserRole Role { get; set; }

    public override string? ToString()
    {
        return FullName + Login + Role.GetType();
    }
}
