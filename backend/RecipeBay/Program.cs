using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RecipeBay.Data;
using Npgsql;
using RecipeBay.Services;

const bool UseLocalEnvironment = false;
if(UseLocalEnvironment)
    DotNetEnv.Env.Load("../../.env");

const bool SeedIngredientsOnStartup = false;
const bool ApplyMigrationsOnStartup = false;

var builder = WebApplication.CreateBuilder(args);

ConfigureAuthentication(builder);
ConfigureServices(builder);

ConfigureDatabase(builder);

AddCORS(builder);

var app = builder.Build();
app.UseCors("AllowAngular");

if(ApplyMigrationsOnStartup)
    ApplyMigrations(app);
if(SeedIngredientsOnStartup)
    SeedDatabase(app);
ConfigureMiddleware(app);

app.Run();

// Method Definitions

static void AddCORS(WebApplicationBuilder builder)
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAngular",
            policy => policy
                .WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod());
    });
}

static void ConfigureAuthentication(WebApplicationBuilder builder)
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");


    var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException("JWT_KEY environment variable is not set. Go to .env and set at least 32-char long key."));

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
        options.MapInboundClaims = false; // To prevent claim type mapping ("sub" to ClaimTypes.NameIdentifier)
    });
}



static void ConfigureServices(WebApplicationBuilder builder)
{

    builder.Services.AddControllers();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddEndpointsApiExplorer();

    // https://stackoverflow.com/questions/43447688/setting-up-swagger-asp-net-core-using-the-authorization-headers-bearer
    builder.Services.AddSwaggerGen(c =>
    {
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter 'Bearer <token>'"
        });

        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
}

static void ConfigureDatabase(WebApplicationBuilder builder)
{
    var dataSourceBuilder = new NpgsqlDataSourceBuilder( 
        UseLocalEnvironment ? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") :
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
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<RecipeBayContext>();
        db.Database.Migrate();
    }
    catch
    {
        Console.WriteLine("DB is up to date with latest migration.");
    }
}

static void SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RecipeBayContext>();
    string filePath = Path.Combine("Data", "PossibleIngredientsSingular.txt");
    IngredientSeeder.SeedIngredients(db, filePath);
}

static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();
    app.MapControllers();
}