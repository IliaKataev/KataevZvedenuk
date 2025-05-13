using System;
using System.Collections.Generic;

namespace gos.models;

public partial class ParameterType
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Parameter> Parameters { get; set; } = new List<Parameter>();

    public virtual ICollection<Rule> Rules { get; set; } = new List<Rule>();
}
