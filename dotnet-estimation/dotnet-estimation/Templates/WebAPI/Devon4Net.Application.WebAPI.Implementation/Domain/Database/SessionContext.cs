using Devon4Net.Application.WebAPI.Implementation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devon4Net.Application.WebAPI.Implementation.Domain.Database
{
    /// <summary>
    /// Session database context definition
    /// </summary>
    public class SessionContext : DbContext
    {
        /// <summary>
        /// Session context definition
        /// </summary>
        /// <param name="options"></param>
        public SessionContext(DbContextOptions<SessionContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Dbset
        /// </summary>
        public virtual DbSet<Session> Session { get; set; }

        /// <summary>
        /// Model rules definition
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>(entity =>
            {
                entity.Property(e => e.ExpiresAt)
                    .IsRequired();

                entity.Property(e => e.Tasks);
                entity.Property(e => e.Users);
            });
        }
    }
}