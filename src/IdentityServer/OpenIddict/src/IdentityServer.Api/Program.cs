using IdentityServer.Domain.Entities;
using IdentityServer.Domain.Contracts;
using IdentityServer.Infrastructure.Data;
using IdentityServer.Infrastructure.Services;
using IdentityServer.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    // Register OpenIddict entity sets
    options.UseOpenIddict<Guid>();
});

// Configure ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure OpenIddict
builder.Services.AddOpenIddict()

    // Register the OpenIddict core components
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>()
            .ReplaceDefaultEntities<Guid>();
    })

    // Register the OpenIddict server components
    .AddServer(options =>
    {
        // Enable authorization and token endpoints (OpenIddict 7.x simplified API)
        options.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token");

        // Enable authorization code, refresh token, and client credentials flows
        options.AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            .AllowClientCredentialsFlow()
            .AllowPasswordFlow();

        // Accept anonymous clients (public clients)
        options.AcceptAnonymousClients();

        // Register signing and encryption credentials
        options.AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();

        // Register ASP.NET Core host
        options.UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableStatusCodePagesIntegration()
            .DisableTransportSecurityRequirement(); // Allow HTTP for development
    })

    // Register the OpenIddict validation components
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });

// Configure authentication with OpenIddict
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

// Add permission-based authorization
builder.Services.AddPermissionAuthorization();

// Configure CORS for Web Admin UI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AdminUI", policy =>
    {
        policy.WithOrigins(
                "https://localhost:5003", // Web project HTTPS
                "http://localhost:5002"   // Web project HTTP
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add controllers and Razor Pages
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Register Mediator (source generator will wire handlers automatically)
// Use Scoped lifetime for handlers to match scoped dependencies (like IIdentityService)
builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});

// Register IdentityService implementation
builder.Services.AddScoped<IIdentityService, IdentityService>();

// Register Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();

// Register Session Service
builder.Services.AddScoped<ISessionService, SessionService>();

// Register HttpClient for internal API calls
builder.Services.AddHttpClient();

// Add API explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Identity Server API", Version = "v1" });

    // Add JWT Bearer authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

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

// Log cookies for debugging
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("=== Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    logger.LogInformation("Cookies: {Cookies}", string.Join(", ", context.Request.Cookies.Select(c => $"{c.Key}={c.Value.Substring(0, Math.Min(20, c.Value.Length))}...")));
    logger.LogInformation("User authenticated: {IsAuth}, Identity: {Identity}", context.User?.Identity?.IsAuthenticated, context.User?.Identity?.Name);

    await next();

    logger.LogInformation("Response: {StatusCode}, User after auth: {IsAuth}", context.Response.StatusCode, context.User?.Identity?.IsAuthenticated);
});

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable CORS
app.UseCors("AdminUI");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.MapGet("/", () => Results.Redirect("/swagger"));

// Seed data
using (var scope = app.Services.CreateScope())
{
    // Seed OpenIddict clients and scopes
    var openIddictSeeder = new OpenIddictSeeder(scope.ServiceProvider);
    await openIddictSeeder.SeedAsync();

    // Seed Identity roles and admin user
    var identitySeeder = new IdentityServer.Infrastructure.Data.IdentitySeeder(scope.ServiceProvider);
    await identitySeeder.SeedAsync();

    // Seed Permissions and assign to roles
    var permissionSeeder = new IdentityServer.Infrastructure.Authorization.PermissionSeeder(scope.ServiceProvider);
    await permissionSeeder.SeedAsync();
}

app.Run();
