using System;
using System.Collections.Generic;

namespace gos.models;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly ActivationDate { get; set; }

    public DateOnly? DeactivationDate { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Rule> Rules { get; set; } = new List<Rule>();
}
