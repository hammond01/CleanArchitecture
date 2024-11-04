var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddSwaggerGen();
    builder.Services.AddPersistence(builder.Configuration.GetConnectionString("SQL")!);
    builder.Services.AddIdentityPersistence(builder.Configuration.GetConnectionString("IDENTITY")!);
    builder.Services.ApplicationConfigureServices();
    builder.Services.InfrastructureConfigureServices();
    builder.Services.Configure<IdentityConfig>(builder.Configuration.GetSection(IdentityConfig.ConfigName));

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
}

var app = builder.Build();
{
    // Configure the HTTP request pipeline.
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
    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
