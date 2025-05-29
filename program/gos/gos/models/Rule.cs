using System;
using System.Collections.Generic;

namespace gos.models;

public partial class Rule
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public string Value { get; set; } = null!;

    public string ComparisonOperator { get; set; } = null!;

    public int NeededTypeId { get; set; }

    public int? DeadlineDays { get; set; } 

    public virtual ParameterType NeededType { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
