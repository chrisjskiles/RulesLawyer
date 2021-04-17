using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace RulesLawyer.Models
{
    public partial class PF2RulesContext : DbContext
    {
        public PF2RulesContext()
        {
        }

        public PF2RulesContext(DbContextOptions<PF2RulesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Action> Actions { get; set; }
        public virtual DbSet<ActionSkill> ActionSkills { get; set; }
        public virtual DbSet<ActionTrait> ActionTraits { get; set; }
        public virtual DbSet<Condition> Conditions { get; set; }
        public virtual DbSet<CreatureIdentification> CreatureIdentifications { get; set; }
        public virtual DbSet<Skill> Skills { get; set; }
        public virtual DbSet<Trait> Traits { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(@$"DataSource={Path.Join(Directory.GetCurrentDirectory(), "PF2Rules.db")}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<ActionSkill>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.SkillId });

                entity.Property(e => e.ActionId).HasColumnName("ActionID");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");
            });

            modelBuilder.Entity<ActionTrait>(entity =>
            {
                entity.HasKey(e => new { e.ActionId, e.TraitId });

                entity.Property(e => e.ActionId).HasColumnName("ActionID");

                entity.Property(e => e.TraitId).HasColumnName("TraitID");
            });

            modelBuilder.Entity<Condition>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<CreatureIdentification>(entity =>
            {
                entity.HasKey(e => new { e.SkillId, e.CreatureTraitId });

                entity.ToTable("CreatureIdentification");

                entity.Property(e => e.SkillId).HasColumnName("SkillID");

                entity.Property(e => e.CreatureTraitId).HasColumnName("CreatureTraitID");
            });

            modelBuilder.Entity<Skill>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            modelBuilder.Entity<Trait>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
