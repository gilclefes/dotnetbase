using dotnetbase.Application.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using dotnetbase.Application.Models.AuthorizeNet;

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
        public virtual DbSet<Charge> Charges { get; set; }
        public virtual DbSet<ChargeCategory> ChargeCategories { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Country> Countries { get; set; }

        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<IdType> IdTypes { get; set; }
        public virtual DbSet<ItemType> ItemTypes { get; set; }
        public virtual DbSet<LaundryItem> LaundryItems { get; set; }
        public virtual DbSet<Period> Periods { get; set; }
        public virtual DbSet<Price> Prices { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServicePeriod> ServicePeriods { get; set; }

        public virtual DbSet<ServiceCharge> ServiceCharges { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategories { get; set; }
        public virtual DbSet<RegStatus> RegStatuses { get; set; }
        public virtual DbSet<UnitType> UnitTypes { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<AuditLog> AuditLogs { get; set; } = default!;

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Models.ServiceProvider> ServiceProviders { get; set; }

        public virtual DbSet<ClientAddress> ClientAddress { get; set; }
        public virtual DbSet<PartnerAddress> PartnerAddresses { get; set; }
        public virtual DbSet<ServiceProviderAddress> ServiceProviderAddresses { get; set; }

        public virtual DbSet<PartnerGeoLocation> PartnerGeoLocations { get; set; }
        public virtual DbSet<ClientGeoLocation> ClientGeoLocations { get; set; }
        public virtual DbSet<ServiceProviderGeoLocation> ServiceProviderGeoLocations { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderAssignment> OrderAssignments { get; set; }
        public virtual DbSet<OrderCharge> OrderCharges { get; set; }

        public virtual DbSet<OrderLocation> OrderLocations { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderMessage> OrderMessages { get; set; }
        public virtual DbSet<OrderPayment> OrderPayments { get; set; }

        public virtual DbSet<OrderRecon> OrderRecons { get; set; }

        public virtual DbSet<OrderStatusUpdate> OrderStatusUpdates { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<ClientSubscription> ClientSubscriptions { get; set; }

        public virtual DbSet<ClientSubscriptionDetail> ClientSubscriptionDetails { get; set; }
        public virtual DbSet<ClientSubscriptionDetailPayment> ClientSubscriptionDetailPayments { get; set; }

        public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }
        public virtual DbSet<SubscriptionPlanCharge> SubscriptionPlanCharges { get; set; }
        public virtual DbSet<SubscriptionPlanChargeExemption> SubscriptionPlanChargeExemptions { get; set; }
        public virtual DbSet<SubscriptionPlanPrice> SubscriptionPlanPrices { get; set; }

        public virtual DbSet<SubscriptionPlanBenefit> SubscriptionPlanBenefits { get; set; }
        public virtual DbSet<SubscriptionPlanService> SubscriptionPlanServices { get; set; }
        public virtual DbSet<OperatingCity> OperatingCities { get; set; }
        public virtual DbSet<CityTax> CityTaxes { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; } = default!;

        public virtual DbSet<ContactUs> ContactUs { get; set; }
        public virtual DbSet<PromoCode> PromoCodes { get; set; }
        public virtual DbSet<OrderPromoCode> OrderPromoCodes { get; set; }

        public virtual DbSet<Detergent> Detergents { get; set; }
        public virtual DbSet<OrderDetergent> OrderDetergents { get; set; }

        public virtual DbSet<ServiceDetergent> ServiceDetergents { get; set; }
        public virtual DbSet<OrderRating> OrderRatings { get; set; }

        public virtual DbSet<UserVerificationCode> UserVerificationCodes { get; set; }
        public virtual DbSet<UserDeviceToken> UserDeviceTokens { get; set; }

        public virtual DbSet<AuthorizeNetCustomerProfile> AuthorizeNetCustomerProfiles { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<CodeGenerator> CodeGenerators { get; set; }

        public virtual DbSet<OrderRefund> OrderRefunds { get; set; }
        public virtual DbSet<OrderRefundDetail> OrderRefundDetails { get; set; }

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

            builder.Entity<RegStatus>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();

                entity.Property(e => e.Status).HasDefaultValue(true);

            });

            builder.Entity<OrderStatus>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Status).HasDefaultValue(true);

            });

            builder.Entity<Currency>(entity =>
             {
                 entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                 entity.Property(e => e.Code).HasMaxLength(50);
                 entity.HasIndex(e => e.Name).IsUnique();
                 entity.HasIndex(e => e.Code).IsUnique();
                 entity.Property(e => e.Symbol).HasMaxLength(5);
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

            builder.Entity<ServiceCategory>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50);
                entity.Property(e => e.Logo).HasMaxLength(450);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Status).HasDefaultValue(true);
                entity.Property(e => e.Description).HasMaxLength(500);

            });

            builder.Entity<ServiceType>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
                entity.HasIndex(e => e.Code).IsUnique();
                entity.Property(e => e.Status).HasDefaultValue(true);
                entity.Property(e => e.Description).HasMaxLength(500);

            });

            builder.Entity<Service>(entity =>
           {
               entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
               entity.Property(e => e.Code).HasMaxLength(50);
               entity.HasIndex(e => e.Name).IsUnique();
               entity.HasIndex(e => e.Code).IsUnique();
               entity.Property(e => e.Status).HasDefaultValue(true);
               entity.Property(e => e.Description).HasMaxLength(500);
               entity.Property(e => e.CategoryId);
               entity.Property(e => e.TypeId);
               entity.HasOne(d => d.ServiceCategory).WithMany(p => p.Services).HasForeignKey(d => d.CategoryId);
               entity.HasOne(d => d.ServiceType).WithMany(p => p.Services).HasForeignKey(d => d.TypeId);

           });

            builder.Entity<Period>(entity =>
             {
                 entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                 entity.Property(e => e.Code).HasMaxLength(50);
                 entity.HasIndex(e => e.Name).IsUnique();
                 entity.HasIndex(e => e.Code).IsUnique();
                 entity.Property(e => e.Description).HasMaxLength(500);
                 entity.Property(e => e.Status).HasDefaultValue(true);
                 entity.Property(e => e.NoOfDays).IsRequired();

             });

            builder.Entity<ItemType>(entity =>
          {
              entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
              entity.Property(e => e.Code).HasMaxLength(50);
              entity.Property(e => e.Logo).HasMaxLength(450);
              entity.HasIndex(e => e.Name).IsUnique();
              entity.HasIndex(e => e.Code).IsUnique();
              entity.Property(e => e.Status).HasDefaultValue(true);
              entity.Property(e => e.Description).HasMaxLength(500);

          });

            builder.Entity<LaundryItem>(entity =>
              {
                  entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                  entity.Property(e => e.Code).HasMaxLength(50);
                  entity.HasIndex(e => e.Name).IsUnique();
                  entity.HasIndex(e => e.Code).IsUnique();
                  entity.Property(e => e.Status).HasDefaultValue(true);
                  entity.Property(e => e.Description).HasMaxLength(500);
                  entity.Property(e => e.ItemTypeId);

                  entity.HasOne(d => d.ItemType).WithMany(p => p.Items).HasForeignKey(d => d.ItemTypeId);


              });

            builder.Entity<UnitType>(entity =>
             {
                 entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                 entity.Property(e => e.Code).HasMaxLength(50);
                 entity.HasIndex(e => e.Name).IsUnique();
                 entity.HasIndex(e => e.Code).IsUnique();
                 entity.Property(e => e.Status).HasDefaultValue(true);
                 entity.Property(e => e.Description).HasMaxLength(500);

             });

            builder.Entity<ChargeCategory>(entity =>
          {
              entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
              entity.Property(e => e.Code).HasMaxLength(50);
              entity.Property(e => e.Logo).HasMaxLength(450);
              entity.HasIndex(e => e.Name).IsUnique();
              entity.HasIndex(e => e.Code).IsUnique();
              entity.Property(e => e.Status).HasDefaultValue(true);
              entity.Property(e => e.Description).HasMaxLength(500);

          });

            builder.Entity<Charge>(entity =>
                {
                    entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                    entity.Property(e => e.Code).HasMaxLength(50);
                    entity.HasIndex(e => e.Name).IsUnique();
                    entity.HasIndex(e => e.Code).IsUnique();
                    entity.Property(e => e.Status).HasDefaultValue(true);
                    entity.Property(e => e.Description).HasMaxLength(500);
                    entity.Property(e => e.CategoryId);
                    entity.HasOne(d => d.ChargeCategory).WithMany(p => p.Charges).HasForeignKey(d => d.CategoryId);


                });

            builder.Entity<ServicePeriod>(entity =>
          {

              entity.HasIndex(e => e.ServiceId);
              entity.HasIndex(e => e.PeriodId);

              entity.HasOne(d => d.Service).WithMany(p => p.ServicePeriods).HasForeignKey(d => d.ServiceId);
              entity.HasOne(d => d.Period).WithMany(p => p.ServicePeriods).HasForeignKey(d => d.PeriodId);
          });

            builder.Entity<ServiceCharge>(entity =>
         {

             entity.HasIndex(e => e.ServiceId);
             entity.HasIndex(e => e.ChargeId);

             entity.HasOne(d => d.Service).WithMany(p => p.ServiceCharges).HasForeignKey(d => d.ServiceId);
             entity.HasOne(d => d.Charge).WithMany(p => p.ServiceCharges).HasForeignKey(d => d.ChargeId);
         });

            builder.Entity<Price>(entity =>
          {
              entity.HasIndex(e => e.ServiceId);
              entity.HasIndex(e => e.PeriodId);
              entity.HasIndex(e => e.ItemId);
              entity.HasIndex(e => e.UnitTypeId);
              entity.Property(e => e.ServiceId);
              entity.Property(e => e.PeriodId);
              entity.Property(e => e.ItemId);
              entity.Property(e => e.UnitTypeId);

              entity.Property(e => e.Status).HasDefaultValue(true);
              entity.Property(e => e.Amount).IsRequired();
              entity.HasOne(d => d.Service).WithMany(p => p.Prices).HasForeignKey(d => d.ServiceId);
              entity.HasOne(d => d.Period).WithMany(p => p.Prices).HasForeignKey(d => d.PeriodId);
              entity.HasOne(d => d.Item).WithMany(p => p.Prices).HasForeignKey(d => d.ItemId);
              entity.HasOne(d => d.UnitType).WithMany(p => p.Prices).HasForeignKey(d => d.UnitTypeId);
          });

            builder.Entity<Client>(entity =>
                 {
                     entity.Property(e => e.FirstName).HasMaxLength(450).IsRequired();
                     entity.Property(e => e.LastName).HasMaxLength(450).IsRequired();
                     entity.HasIndex(e => new { e.IdTypeId, e.IdNumber, }).IsUnique();
                     entity.HasIndex(e => e.RegStatusId);
                     entity.HasOne(d => d.RegStatus).WithMany(p => p.Clients).HasForeignKey(d => d.RegStatusId);
                     entity.HasOne(d => d.IdType).WithMany(p => p.Clients).HasForeignKey(d => d.IdTypeId);

                 });


            builder.Entity<Models.ServiceProvider>(entity =>
                 {
                     entity.Property(e => e.FirstName).HasMaxLength(450).IsRequired();
                     entity.Property(e => e.LastName).HasMaxLength(450).IsRequired();
                     entity.HasIndex(e => new { e.IdTypeId, e.IdNumber, }).IsUnique();
                     entity.HasIndex(e => e.RegStatusId);
                     entity.HasOne(d => d.RegStatus).WithMany(p => p.ServiceProviders).HasForeignKey(d => d.RegStatusId);
                     entity.HasOne(d => d.IdType).WithMany(p => p.ServiceProviders).HasForeignKey(d => d.IdTypeId);

                 });

            builder.Entity<Partner>(entity =>
            {
                entity.Property(e => e.CompanyName).HasMaxLength(450).IsRequired();
                entity.HasIndex(e => new { e.IdTypeId, e.IdNumber, }).IsUnique();
                entity.HasIndex(e => e.RegStatusId);
                entity.HasOne(d => d.RegStatus).WithMany(p => p.Partners).HasForeignKey(d => d.RegStatusId);
                entity.HasOne(d => d.IdType).WithMany(p => p.Partners).HasForeignKey(d => d.IdTypeId);

            });

            builder.Entity<ClientAddress>(entity =>
          {
              entity.HasIndex(e => e.ClientId);
              entity.HasOne(d => d.Client).WithMany(p => p.ClientAddresses).HasForeignKey(d => d.ClientId);

          });

            builder.Entity<ServiceProviderAddress>(entity =>
            {
                entity.HasIndex(e => e.ServiceProviderId);

                entity.HasOne(d => d.ServiceProvider).WithMany(p => p.ServiceProviderAddresses).HasForeignKey(d => d.ServiceProviderId);

            });

            builder.Entity<PartnerAddress>(entity =>
          {
              entity.HasIndex(e => e.PartnerId);
              entity.HasOne(d => d.Partner).WithMany(p => p.PartnerAddresses).HasForeignKey(d => d.PartnerId);

          });

            builder.Entity<ClientGeoLocation>(entity =>
         {
             entity.HasIndex(e => e.ClientId);
             entity.HasOne(d => d.Client).WithMany(p => p.ClientGeoLocations).HasForeignKey(d => d.ClientId);

         });

            builder.Entity<PartnerGeoLocation>(entity =>
         {
             entity.HasIndex(e => e.PartnerId);
             entity.HasOne(d => d.Partner).WithMany(p => p.PartnerGeoLocations).HasForeignKey(d => d.PartnerId);

         });

            builder.Entity<ServiceProviderGeoLocation>(entity =>
          {
              entity.HasIndex(e => e.ServiceProviderId);
              entity.HasOne(d => d.ServiceProvider).WithMany(p => p.ServiceProviderGeoLocations).HasForeignKey(d => d.ServiceProviderId);

          });

            builder.Entity<Order>(entity =>
           {
               entity.HasIndex(e => e.OrderStatusId);
               entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders).HasForeignKey(d => d.OrderStatusId);
           });

            builder.Entity<OrderAssignment>(entity =>
          {
              entity.HasIndex(e => e.OrderId);
              entity.HasOne(d => d.Order).WithMany(p => p.OrderAssigments).HasForeignKey(d => d.OrderId);
          });

            builder.Entity<OrderCharge>(entity =>
          {
              entity.HasIndex(e => e.OrderId);
              entity.HasOne(d => d.Order).WithMany(p => p.OrderCharges).HasForeignKey(d => d.OrderId);
          });

            builder.Entity<OrderItem>(entity =>
           {
               entity.HasIndex(e => e.OrderId);
               entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasForeignKey(d => d.OrderId);
           });

            builder.Entity<OrderMessage>(entity =>
          {
              entity.HasIndex(e => e.OrderId);
              entity.HasOne(d => d.Order).WithMany(p => p.OrderMessages).HasForeignKey(d => d.OrderId);
          });

            builder.Entity<OrderRecon>(entity =>
       {
           entity.HasIndex(e => e.OrderId);
           entity.HasOne(d => d.Order).WithMany(p => p.OrderRecons).HasForeignKey(d => d.OrderId);
       });

            builder.Entity<OrderStatusUpdate>(entity =>
      {
          entity.HasIndex(e => e.OrderId);
          entity.HasOne(d => d.Order).WithMany(p => p.OrderStatusUpdates).HasForeignKey(d => d.OrderId);
      });


            builder.Entity<OrderLocation>(entity =>
          {
              entity.HasIndex(e => e.OrderId);
              entity.HasOne(d => d.Order).WithMany(p => p.OrderLocations).HasForeignKey(d => d.OrderId);
          });

            builder.Entity<OrderPayment>(entity =>
          {
              entity.HasIndex(e => e.OrderId);
              entity.HasOne(d => d.Order).WithMany(p => p.OrderPayments).HasForeignKey(d => d.OrderId);
              entity.HasOne(d => d.Payment).WithMany(p => p.OrderPayments).HasForeignKey(d => d.PaymentId);
          });

            builder.Entity<Payment>(entity =>
          {
              entity.HasIndex(e => e.RefNumber).IsUnique();
              entity.HasIndex(e => e.CurrencyId);
              entity.HasOne(d => d.Currency).WithMany(p => p.Payments).HasForeignKey(d => d.CurrencyId);
          });

            builder.Entity<ClientSubscription>(entity =>
            {

                entity.HasIndex(e => e.SubscriptionId);
                entity.Property(e => e.Status).HasDefaultValue(false);
                entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.ClientSubscriptions).HasForeignKey(d => d.SubscriptionId);
            });

            builder.Entity<ClientSubscriptionDetail>(entity =>
              {
                  entity.HasIndex(e => e.ClientSubscriptionId);
                  entity.HasIndex(e => e.SubscriptionPlanPriceId);
                  entity.HasOne(d => d.ClientSubscription).WithMany(p => p.ClientSubscriptionDetails).HasForeignKey(d => d.ClientSubscriptionId);
                  entity.HasOne(d => d.SubscriptionPlanPrice).WithMany(p => p.ClientSubscriptionDetails).HasForeignKey(d => d.SubscriptionPlanPriceId);
              });

            builder.Entity<ClientSubscriptionDetailPayment>(entity =>
             {
                 entity.HasIndex(e => e.PaymentId);
                 entity.HasIndex(e => e.ClientSubscriptionDetailId);
                 entity.HasOne(d => d.ClientSubscriptionDetail).WithMany(p => p.ClientSubscriptionDetailPayments).HasForeignKey(d => d.ClientSubscriptionDetailId);
                 entity.HasOne(d => d.Payment).WithMany(p => p.ClientSubscriptionDetailPayments).HasForeignKey(d => d.PaymentId);

             });

            builder.Entity<SubscriptionPlan>(entity =>
            {

                entity.HasIndex(e => e.OrderPeriodId);
                entity.Property(e => e.Status).HasDefaultValue(false);
                entity.HasOne(d => d.Period).WithMany(p => p.SubscriptionPlans).HasForeignKey(d => d.OrderPeriodId);
            });

            builder.Entity<SubscriptionPlanCharge>(entity =>
            {
                entity.HasIndex(e => e.SubscriptionId);
                entity.HasIndex(e => e.ChargeId);
                entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.SubscriptionPlanCharges).HasForeignKey(d => d.SubscriptionId);
                entity.HasOne(d => d.Charge).WithMany(p => p.SubscriptionPlanCharges).HasForeignKey(d => d.ChargeId);
            });

            builder.Entity<SubscriptionPlanChargeExemption>(entity =>
          {
              entity.HasIndex(e => e.SubscriptionId);
              entity.HasIndex(e => e.ChargeId);
              entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.SubscriptionPlanChargeExemptions).HasForeignKey(d => d.SubscriptionId);
              entity.HasOne(d => d.Charge).WithMany(p => p.SubscriptionPlanChargeExemptions).HasForeignKey(d => d.ChargeId);
          });

            builder.Entity<SubscriptionPlanPrice>(entity =>
           {
               entity.HasIndex(e => e.SubscriptionId);
               entity.HasIndex(e => e.PeriodId);
               entity.HasIndex(e => e.CurrencyId);
               entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.SubscriptionPlanPrices).HasForeignKey(d => d.SubscriptionId);
               entity.HasOne(d => d.Currency).WithMany(p => p.SubscriptionPlanPrices).HasForeignKey(d => d.CurrencyId);
               entity.HasOne(d => d.Period).WithMany(p => p.SubscriptionPlanPrices).HasForeignKey(d => d.PeriodId);
           });

            builder.Entity<SubscriptionPlanService>(entity =>
             {
                 entity.HasIndex(e => e.SubscriptionId);
                 entity.HasIndex(e => e.ServiceId);
                 entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.SubscriptionPlanServices).HasForeignKey(d => d.SubscriptionId);
                 entity.HasOne(d => d.Service).WithMany(p => p.SubscriptionPlanServices).HasForeignKey(d => d.ServiceId);
             });

            builder.Entity<SubscriptionPlanBenefit>(entity =>
           {
               entity.HasIndex(e => e.SubscriptionId);
               entity.HasIndex(e => e.Benefit);
               entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.SubscriptionPlanBenefits).HasForeignKey(d => d.SubscriptionId);
           });

            builder.Entity<OperatingCity>(entity =>
          {
              entity.HasIndex(e => e.CityId);
              entity.HasOne(d => d.City).WithMany(p => p.OperatingCities).HasForeignKey(d => d.CityId);
          });


            builder.Entity<CityTax>(entity =>
          {
              entity.HasIndex(e => e.CityId);
              entity.HasOne(d => d.City).WithMany(p => p.CityTaxes).HasForeignKey(d => d.CityId);
          });

            builder.Entity<ContactUs>();

            builder.Entity<PromoCode>(entity =>
        {
            entity.HasIndex(e => e.CodeValue);
        });

            builder.Entity<OrderPromoCode>(entity =>
                {
                    entity.HasIndex(e => e.OrderId);
                    entity.HasIndex(e => e.PromoId);
                    entity.HasOne(d => d.Order).WithMany(p => p.OrderPromoCodes).HasForeignKey(d => d.OrderId);
                    entity.HasOne(d => d.PromoCode).WithMany(p => p.OrderPromoCodes).HasForeignKey(d => d.PromoId);
                });

            builder.Entity<Detergent>(entity =>
                {
                    entity.HasIndex(e => e.Name).IsUnique();
                });

            builder.Entity<ServiceDetergent>(entity =>
           {
               entity.HasIndex(e => e.ServiceId);
               entity.HasIndex(e => e.DetergentId);
               entity.HasOne(d => d.Service).WithMany(p => p.ServiceDetergents).HasForeignKey(d => d.ServiceId);
               entity.HasOne(d => d.Detergent).WithMany(p => p.ServiceDetergents).HasForeignKey(d => d.DetergentId);
           });

            builder.Entity<OrderDetergent>(entity =>
          {
              entity.HasIndex(e => e.ServiceId);
              entity.HasIndex(e => e.DetergentId);
              entity.HasIndex(e => e.OrderId);
              entity.HasOne(d => d.Service).WithMany(p => p.OrderDetergents).HasForeignKey(d => d.ServiceId);
              entity.HasOne(d => d.Detergent).WithMany(p => p.OrderDetergents).HasForeignKey(d => d.DetergentId);
              entity.HasOne(d => d.Order).WithMany(p => p.OrderDetergents).HasForeignKey(d => d.OrderId);
          });

            builder.Entity<OrderRating>(entity =>
          {
              entity.HasIndex(e => e.OrderId);

              entity.HasOne(d => d.Order).WithMany(p => p.OrderRatings).HasForeignKey(d => d.OrderId);
          });

            builder.Entity<OrderRefund>(entity =>
         {
             entity.HasIndex(e => e.OrderId);
             entity.HasOne(d => d.Client).WithMany(p => p.OrderRefunds).HasForeignKey(d => d.ClientId);
             entity.HasOne(d => d.Order).WithMany(p => p.OrderRefunds).HasForeignKey(d => d.OrderId);
         });


            builder.Entity<OrderRefundDetail>(entity =>
         {
             entity.HasIndex(e => e.OrderRefundId);

             entity.HasOne(d => d.OrderRefund).WithMany(p => p.OrderRefundDetails).HasForeignKey(d => d.OrderRefundId);
         });

            builder.Entity<AuthorizeNetCustomerProfile>(entity =>
            {
                entity.HasKey(e => e.ClientId);
                entity.HasIndex(e => e.Email).IsUnique();

            });

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
