using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }
        public DbSet<ToDoList.Models.Tag> Tags { get; set; }
        public DbSet<ToDoList.Models.Note> Notes { get; set; }
        public DbSet<ToDoList.Models.Reminder> Reminders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Note>()
                .HasMany(n => n.Tags)
                .WithMany(t => t.Notes);

            modelBuilder.Entity<Reminder>()
                .HasMany(r => r.Tags)
                .WithMany(t => t.Reminders);
        }
    }
}