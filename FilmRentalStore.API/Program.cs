using FilmRentalStore.API.Data;
using FilmRentalStore.API.Filters;
using FilmRentalStore.API.Mappings;
using FilmRentalStore.API.Middleware;
using FilmRentalStore.API.Repositories.Implementations;
using FilmRentalStore.API.Repositories.Interfaces;
using FilmRentalStore.API.Services.Implementations;
using FilmRentalStore.API.Services.Interfaces;
using FilmRentalStore.API.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();

        // DbContext
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        // Repository Dependency Injection
        builder.Services.AddScoped<IStaffRepository, StaffRepository>();
        builder.Services.AddScoped<IStoreRepository, StoreRepository>();
        builder.Services.AddScoped<IActorRepository, ActorRepository>();
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<ILanguageRepository, LanguageRepository>();
        builder.Services.AddScoped<IFilmRepository, FilmRepository>();
        builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
        builder.Services.AddScoped<IRentalRepository, RentalRepository>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
        builder.Services.AddScoped<IAddressRepository, AddressRepository>();
        builder.Services.AddScoped<ICountryRepository, CountryRepository>();
        builder.Services.AddScoped<ICityRepository, CityRepository>();

        // Service Dependency Injection
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddScoped<IAddressService, AddressService>();
        builder.Services.AddScoped<IStaffService, StaffService>();
        builder.Services.AddScoped<IStoreService, StoreService>();
        builder.Services.AddScoped<IActorService, ActorService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<ILanguageService, LanguageService>();
        builder.Services.AddScoped<IFilmService, FilmService>();
        builder.Services.AddScoped<IInventoryService, InventoryService>();
        builder.Services.AddScoped<IRentalService, RentalService>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<ICustomerService, CustomerService>();

        // AutoMapper
        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<CommonMappingProfile>();
            cfg.AddProfile<StaffMappingProfile>();
            cfg.AddProfile<StoreMappingProfile>();
            cfg.AddProfile<ActorMappingProfile>();
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<LanguageMappingProfile>();
            cfg.AddProfile<FilmMappingProfile>();
            cfg.AddProfile<InventoryMappingProfile>();
            cfg.AddProfile<RentalMappingProfile>();
            cfg.AddProfile<PaymentMappingProfile>();
            cfg.AddProfile<CustomerMappingProfile>();
        });

        // FluentValidation
        builder.Services.AddValidatorsFromAssemblyContaining<StaffCreateRequestDtoValidator>();
        builder.Services.AddScoped<ValidationFilter>();

        builder.Services.AddControllers(options =>
        {
            options.Filters.AddService<ValidationFilter>();
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                    RoleClaimType = ClaimTypes.Role
                };
            });

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.OperationFilter<AllowAnonymousOperationFilter>();

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
