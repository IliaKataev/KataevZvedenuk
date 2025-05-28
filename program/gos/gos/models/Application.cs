using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace gos.models;

public partial class Application
{
    [Column("id")]
    public int Id { get; set; }
    [Column("user_id")]
    public int UserId { get; set; }
    [Column("service_id")]
    public int ServiceId { get; set; }
    [Column("result")]
    public string? Result { get; set; }
    [Column("creation_date")]
    public DateTime CreationDate { get; set; }
    [Column("closure_date")]
    public DateTime? ClosureDate { get; set; }
    [Column("deadline")]
    public DateTime Deadline { get; set; }

    public virtual Service Service { get; set; } = null!;

    public virtual User User { get; set; } = null!;
    [Column("status")]
    public ApplicationStatus Status { get; set; }
}
