using dotnetbase.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace dotnetbase.Application.Database
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext() : base()
        {
        }


        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        { }

        public virtual DbSet<ApplicationUser> ApplicationUsers { set; get; }
        public virtual DbSet<Role> Roles { set; get; }
        public virtual DbSet<UserRole> UserRoles { get; set; }


        public virtual DbSet<Country> Countries { get; set; }


        public virtual DbSet<IdType> IdTypes { get; set; }


        public virtual DbSet<AuditLog> AuditLogs { get; set; } = default!;





        public virtual DbSet<City> Cities { get; set; }






        public virtual DbSet<ContactUs> ContactUs { get; set; }


        public virtual DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
        public virtual DbSet<UserDeviceToken> UserDeviceTokens { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<CodeGenerator> CodeGenerators { get; set; }



        public virtual DbSet<Faq> Faqs { get; set; }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var modifiedEntities = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                || e.State == EntityState.Modified
                || e.State == EntityState.Deleted)
                .ToList();

            foreach (var modifiedEntity in modifiedEntities)
            {
                var auditLog = new AuditLog
                {
                    EntityName = modifiedEntity.Entity.GetType().Name,
                    Action = modifiedEntity.State.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Changes = GetChanges(modifiedEntity),
                    CreatedAt = DateTime.UtcNow,

                };

                AuditLogs.Add(auditLog);
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        private static string GetChanges(EntityEntry entity)
        {
            var changes = new StringBuilder();

            foreach (var property in entity.OriginalValues.Properties)
            {
                var originalValue = entity.OriginalValues[property];
                var currentValue = entity.CurrentValues[property];

                if (!Equals(originalValue, currentValue))
                {
                    changes.AppendLine($"{property.Name}: From '{originalValue}' to '{currentValue}'");
                }
            }

            return changes.ToString();
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            // it should be placed here, otherwise it will rewrite the following settings!
            base.OnModelCreating(builder);

            // Custom application mappings
            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(450).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Password).IsRequired();

            });

            builder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.RoleId);
                entity.Property(e => e.UserId);
                entity.Property(e => e.RoleId);
                entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId);
                entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId);
            });

            builder.Entity<Role>().HasData(
                new Role { Name = CustomRoles.User, NormalizedName = CustomRoles.User.ToUpper() },
                new Role { Name = CustomRoles.Admin, NormalizedName = CustomRoles.Admin.ToUpper() }
            );

            builder.Entity<IdType>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();

                entity.Property(e => e.Status).HasDefaultValue(true);

            });





            builder.Entity<Country>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Status).HasDefaultValue(true);

            });

            builder.Entity<City>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();

                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Status).HasDefaultValue(true);
                entity.Property(e => e.CountryId);
                entity.HasOne(d => d.Country).WithMany(p => p.Cities).HasForeignKey(d => d.CountryId);

            });




            builder.Entity<ContactUs>();






            builder.Entity<UserVerificationCode>(entity =>
           {
               entity.HasKey(e => e.Id);
               entity.HasIndex(e => e.Email).IsUnique();

           });

            builder.Entity<UserDeviceToken>(entity =>
         {
             entity.HasKey(e => e.Id);
             entity.HasIndex(e => e.Email).IsUnique();

         });

            builder.Entity<Faq>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Question).IsUnique();
            });



        }

    }
}
