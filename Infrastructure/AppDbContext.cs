using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using Domian.Entites;

namespace Infrastructure
{
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options)
            {
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Answer>()
                .HasOne(q => q.question)
                .WithMany(a => a.answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.NoAction);

                modelBuilder.Entity<Answer>()
                .HasOne(a => a.response)
                .WithMany(r => r.answers)
                .HasForeignKey(a => a.ResponseId)
                .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Answer>()
               .HasOne(a => a.SelectedOption)
               .WithMany(o => o.Answers)
               .HasForeignKey(a => a.SelectedOptionId)
               .OnDelete(DeleteBehavior.Restrict);
               modelBuilder.Entity<Response>()
               .HasOne(r => r.users)
               .WithMany(u => u.Responses)
               .HasForeignKey(r => r.UserId)
               .OnDelete(DeleteBehavior.Restrict);



        }
        public DbSet<Survey> surveys { get; set; }

            public DbSet<Question> questions { get; set; }

            public DbSet<QuestionOption> questionOptions { get; set; }

            public DbSet<Answer> answers { get; set; }

            public DbSet<Response> responses { get; set; }

            public DbSet<User> users { get; set; }


        }
    }
