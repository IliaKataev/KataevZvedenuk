using gos.repositories;
using gos.services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using gos.controllers;
using gos.models;

namespace gos.app
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(configuration)
                .AddDbContext<AppDbContext>(options =>
                    options.UseNpgsql(connectionString))
                .AddSingleton<AuthSession>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IApplicationRepository, ApplicationRepository>()
                .AddScoped<IServiceRepository, ServiceRepository>()
                .AddScoped<IParameterRepository, ParameterRepository>()
                .AddScoped<IParameterTypeRepository, ParameterTypeRepository>()
                .AddScoped<IRuleRepository, RuleRepository>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IApplicationService, ApplicationService>()
                .AddScoped<IAuthService, AuthService>()
                .AddScoped<IAdminService, AdminService>()
                .AddScoped<ICitizenService, CitizenService>()
                .AddScoped<LoginForm>()
                .AddScoped<AdminForm>()
                .AddScoped<CitizenForm>()
                .AddScoped<CivilServantForm>()
                .AddScoped<AuthController>()
                .AddScoped<CitizenController>()
                .BuildServiceProvider();

            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(serviceProvider.GetRequiredService<LoginForm>());
        }
    }
}
