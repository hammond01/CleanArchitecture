var builder = WebApplication.CreateBuilder(args);
{
    // Configure logging
    builder.Services.AddLoggingConfiguration();
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddPersistence(builder.Configuration.GetConnectionString("SQL")!);
    builder.Services.ApplicationConfigureServices();
    builder.Services.InfrastructureConfigureServices();
    builder.Services.Configure<IdentityConfig>(builder.Configuration.GetSection(IdentityConfig.ConfigName));
    builder.Services.AddSingleton<ILogger, ConsoleLogger>();

    builder.Host.UseSerilog();
    var audience = builder.Configuration["IdentityConfig:AUDIENCE"];
    var issUser = builder.Configuration["IdentityConfig:ISSUER"];
    var key = builder.Configuration["IdentityConfig:SECRET"];

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidIssuer = issUser,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });

    builder.Services.Configure<IdentityOptions>(options =>
    {
        options.Password.RequireDigit = PasswordPolicy.RequireDigit;
        options.Password.RequiredLength = PasswordPolicy.RequiredLength;
        options.Password.RequireNonAlphanumeric = PasswordPolicy.RequireNonAlphanumeric;
        options.Password.RequireUppercase = PasswordPolicy.RequireUppercase;
        options.Password.RequireLowercase = PasswordPolicy.RequireLowercase;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 10;
        options.Lockout.AllowedForNewUsers = true;
    });

    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false, true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
        .AddEnvironmentVariables();
}

var app = builder.Build();
{
    // Add Serilog request logging
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    using (var serviceScope =
           ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
    {
        var databaseInitializer = serviceScope.ServiceProvider.GetService<IDatabaseInitializer>();
        databaseInitializer?.SeedAsync().Wait();
    }

    app.UseGlobalExceptionHandlerMiddleware();
    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    // Ensure any buffered events are sent at shutdown
    app.Lifetime.ApplicationStopped.Register(Log.CloseAndFlush);

    app.Run();
}
