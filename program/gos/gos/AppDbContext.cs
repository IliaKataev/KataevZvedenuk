using System;
using System.Collections.Generic;
using gos.models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace gos;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<gos.models.Application> Applications { get; set; }
    public virtual DbSet<Parameter> Parameters { get; set; }
    public virtual DbSet<ParameterType> ParameterTypes { get; set; }
    public virtual DbSet<Rule> Rules { get; set; }
    public virtual DbSet<Service> Services { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            //ьУДАЛИТЬ: устаревшая ручная настройка enum
            // NpgsqlConnection.GlobalTypeMapper.EnableUnmappedTypes();
            // NpgsqlConnection.GlobalTypeMapper.MapEnum<UserRole>("public.user_role");

            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=gosuslugi;Username=postgres;Password=1111");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion<int>();
        });

        modelBuilder.Entity<gos.models.Application>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("applications_pkey");
            entity.ToTable("applications");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClosureDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("closure_date");
            entity.Property(e => e.CreationDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("creation_date");
            entity.Property(e => e.Deadline).HasColumnName("deadline")
            .HasColumnType("timestamp without time zone");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Service).WithMany(p => p.Applications)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("applications_service_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Applications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("applications_user_id_fkey");
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<int>();

        });

        modelBuilder.Entity<Parameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parameters_pkey");

            entity.ToTable("parameters");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TypeId).HasColumnName("type_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .HasColumnName("value");

            entity.HasOne(d => d.Type).WithMany(p => p.Parameters)
                .HasForeignKey(d => d.TypeId)
                .HasConstraintName("parameters_type_id_fkey");

            entity.HasOne(d => d.User).WithMany(p => p.Parameters)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("parameters_user_id_fkey");
        });

        modelBuilder.Entity<ParameterType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("parameter_types_pkey");

            entity.ToTable("parameter_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Rule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rules_pkey");

            entity.ToTable("rules");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComparisonOperator)
                .HasMaxLength(255)
                .HasColumnName("comparison_operator");
            entity.Property(e => e.NeededTypeId).HasColumnName("needed_type_id");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .HasColumnName("value");
            entity.Property(e => e.DeadlineDays)
                .HasColumnName("deadline_days"); // <= добавлено

            entity.HasOne(d => d.NeededType).WithMany(p => p.Rules)
                .HasForeignKey(d => d.NeededTypeId)
                .HasConstraintName("rules_needed_type_id_fkey");

            entity.HasOne(d => d.Service).WithMany(p => p.Rules)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("rules_service_id_fkey");
        });


        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("services_pkey");

            entity.ToTable("services");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActivationDate).HasColumnName("activation_date");
            entity.Property(e => e.DeactivationDate).HasColumnName("deactivation_date");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
