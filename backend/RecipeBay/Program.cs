using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using RecipeBay.Data;
using Npgsql;
using RecipeBay.Services;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureServices(builder);
ConfigureDatabase(builder);

var app = builder.Build();

ApplyMigrations(app);
ConfigureMiddleware(app);

app.Run();

// Method Definitions

static void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY") ?? string.Empty);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });
}

static void ConfigureServices(WebApplicationBuilder builder)
{
    builder.Services.AddControllers();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

static void ConfigureDatabase(WebApplicationBuilder builder)
{
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(
        builder.Configuration.GetConnectionString("DefaultConnection")
    );
    dataSourceBuilder.EnableDynamicJson();
    var dataSource = dataSourceBuilder.Build();

    builder.Services.AddDbContext<RecipeBayContext>(options =>
        options.UseNpgsql(dataSource)
    );
}

static void ApplyMigrations(WebApplication app)
{
    var retry = 10;
    while (retry > 0)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RecipeBayContext>();
            db.Database.Migrate();
            break;
        }
        catch
        {
            retry--;
            Thread.Sleep(1000);
        }
    }
}

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.MapControllers();
}