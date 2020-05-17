using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ProjectManagementSystem.DataAccess.Models
{
    public partial class ProjectManagementSystemContext : DbContext
    {
        public ProjectManagementSystemContext()
        {
        }

        public ProjectManagementSystemContext(DbContextOptions<ProjectManagementSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<Task> Task { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FinishDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Project_ParentId");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.FinishDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.StartDate).HasColumnType("datetime");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Task_ParentId");

                entity.HasOne(d => d.Project)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.ProjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Task_ProjectId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
