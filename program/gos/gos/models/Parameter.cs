using System;
using System.Collections.Generic;

namespace gos.models;

public partial class Parameter
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TypeId { get; set; }

    public string Value { get; set; } = null!;

    public virtual ParameterType Type { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
