using Coravel;
using Spark.Library.Database;
using Spark.Library.Logging;
using Spark.Library.Mail;
using dotnetbase.Application.Database;
using dotnetbase.Application.Events.Listeners;
using dotnetbase.Application.Services.Auth;
using dotnetbase.Application.Tasks;
using System.Reflection;
using Microsoft.Extensions.DependencyModel;
using Scrutor;
using dotnetbase.Application.Services;
using Microsoft.AspNetCore.Identity;
using dotnetbase.Application.Models;
using Microsoft.EntityFrameworkCore;


namespace dotnetbase.Application.Startup
{
    public static class AppServiceRegistration
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddJwt(config);
            services.AddSwagger();
            services.AddCors(options =>
            options.AddDefaultPolicy(builder =>
             builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()));



            services.AddHttpContextAccessor();
            services.AddSingleton<IUriService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor?.HttpContext?.Request;
                var uri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
                return new UriService(uri);
            });

            // services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<DatabaseContext>();

            // Add ASP.NET Core Identity support
            services.AddIdentity<ApplicationUser, Role>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
            }).AddEntityFrameworkStores<DatabaseContext>().AddDefaultTokenProviders();



            services.AddControllers();
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddCustomServices();

            services.AddDatabase<DatabaseContext>(config);
            services.AddHealthChecks().AddDbContextCheck<DatabaseContext>();
            services.AddHealthChecksUI().AddInMemoryStorage();
            services.AddLogger(config);
            services.AddTaskServices();
            services.AddScheduler();
            services.AddQueue();
            services.AddEventServices();
            services.AddEvents();
            services.AddMailer(config);


            return services;
        }

        private static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            // add custom services
            services.AddScoped<UsersService>();
            services.AddScoped<RolesService>();
            services.AddScoped<AuthService>();
            services.AddScoped<YaboUtilsService>();
            services.AddScoped<AuthorizePaymentService>();
            services.AddScoped<CodeGenService>();

            services.Scan(selector => selector
            .FromCallingAssembly()
            .AddClasses(publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .WithScopedLifetime());

            return services;
        }

        private static IServiceCollection AddEventServices(this IServiceCollection services)
        {
            // add custom events here
            services.AddTransient<EmailNewUser>();
            services.AddTransient<EmailCodeConfirmation>();
            services.AddTransient<EmailForgottenPassword>();
            services.AddTransient<EmailOrderAssigned>();
            services.AddTransient<EmailOrderMessage>();
            return services;
        }

        private static IServiceCollection AddTaskServices(this IServiceCollection services)
        {
            // add custom background tasks here
            services.AddTransient<ExampleTask>();
            services.AddTransient<OrderAssignmentTask>();
            services.AddTransient<OrderRefundTask>();
            services.AddTransient<SubscriptionUpdateTask>();
            return services;
        }
    }
}
