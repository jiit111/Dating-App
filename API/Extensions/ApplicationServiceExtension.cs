using API.Data;
using API.Helper;
using API.Interface;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,IConfiguration config)
        {
          services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
          services.AddScoped<ITokenService,TokenService>();
          services.AddScoped<IPhotoService,PhotoService>();
          services.AddScoped<IUserRepository,UserRepository>();
          services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
            });
            return services;
        }
    }
}