using Microsoft.EntityFrameworkCore;
using ProcessTracker.Core.Entities;

namespace ProcessTracker.Infrastructure.Data
{
    public class ProcessTrackerContext : DbContext
    {
        public ProcessTrackerContext(DbContextOptions<ProcessTrackerContext> options)
            : base(options)
        { }
        public DbSet<Example> Examples { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Example>()
                .Property(e => e.Name)
                .HasColumnType("varchar(512)");
        }
    }
}
