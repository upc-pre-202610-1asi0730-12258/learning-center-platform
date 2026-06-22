using Acme.Center.Platform.Iam.Application.Acl;
using Acme.Center.Platform.Iam.Application.CommandServices;
using Acme.Center.Platform.Iam.Application.Internal.CommandServices;
using Acme.Center.Platform.Iam.Application.Internal.OutboundServices;
using Acme.Center.Platform.Iam.Application.Internal.QueryServices;
using Acme.Center.Platform.Iam.Application.QueryServices;
using Acme.Center.Platform.Iam.Domain.Repositories;
using Acme.Center.Platform.Iam.Infrastructure.Hashing.BCrypt.Services;
using Acme.Center.Platform.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Iam.Infrastructure.Pipeline.Middleware.Extensions;
using Acme.Center.Platform.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Acme.Center.Platform.Iam.Infrastructure.Tokens.Jwt.Services;
using Acme.Center.Platform.Iam.Interfaces.Acl;
using Acme.Center.Platform.Iam.Resources;
using Acme.Center.Platform.Profiles.Application.CommandServices;
using Acme.Center.Platform.Profiles.Application.Internal.CommandServices;
using Acme.Center.Platform.Profiles.Application.Internal.QueryServices;
using Acme.Center.Platform.Profiles.Application.QueryServices;
using Acme.Center.Platform.Profiles.Domain.Repositories;
using Acme.Center.Platform.Profiles.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Profiles.Resources;
using Acme.Center.Platform.Publishing.Application.CommandServices;
using Acme.Center.Platform.Publishing.Application.Internal.CommandServices;
using Acme.Center.Platform.Publishing.Application.Internal.QueryServices;
using Acme.Center.Platform.Publishing.Application.QueryServices;
using Acme.Center.Platform.Publishing.Domain.Repositories;
using Acme.Center.Platform.Publishing.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Publishing.Resources;
using Acme.Center.Platform.Resources.Errors;
using Acme.Center.Platform.Resources.Shared;
using Acme.Center.Platform.Shared.Domain.Repositories;
using Acme.Center.Platform.Shared.Infrastructure.Interfaces.AspNetCore.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Mediator.Cortex.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Acme.Center.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Acme.Center.Platform.Shared.Infrastructure.Pipeline.Middleware.Extensions;
using Cortex.Mediator.Commands;
using Cortex.Mediator.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi;
// Added for ProblemDetailsFactory
// Added for base ProblemDetailsFactory
// Added for IamMessages
// Added for ProfilesMessages
using ProblemDetailsFactory = Acme.Center.Platform.Shared.Interfaces.Rest.ProblemDetails.ProblemDetailsFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

// Add ProblemDetails services
builder.Services.AddProblemDetails();


// Add CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllPolicy",
        policy => policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add Database Connection

// Configure Database Context and route EF logs through the app logger pipeline.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Explicitly register IStringLocalizer for ErrorMessages and Commons
builder.Services.AddSingleton<IStringLocalizer<ErrorMessages>, StringLocalizer<ErrorMessages>>();
builder.Services
    .AddSingleton<IStringLocalizer<CommonMessages>,
        StringLocalizer<CommonMessages>>(); // Corrected from Common to Commons
builder.Services.AddSingleton<IStringLocalizer<IamMessages>, StringLocalizer<IamMessages>>(); // Added for IamMessages
builder.Services
    .AddSingleton<IStringLocalizer<ProfilesMessages>,
        StringLocalizer<ProfilesMessages>>(); // Added for ProfilesMessages
builder.Services
    .AddSingleton<IStringLocalizer<PublishingMessages>,
        StringLocalizer<PublishingMessages>>(); // Added for PublishingMessages

// Register the custom ProblemDetailsFactory
builder.Services.AddSingleton<ProblemDetailsFactory>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1",
        new OpenApiInfo
        {
            Title = "Acme.Center.Platform",
            Version = "v1",
            Description = "ACME Learning Center Platform API",
            TermsOfService = new Uri("https://acme-learning.com/tos"),
            Contact = new OpenApiContact
            {
                Name = "ACME Studios",
                Email = "contact@acme.com"
            },
            License = new OpenApiLicense
            {
                Name = "Apache 2.0",
                Url = new Uri("https://www.apache.org/licenses/LICENSE-2.0.html")
            }
        });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] });
    options.EnableAnnotations();
});

// Dependency Injection

// Shared Bounded Context
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Publishing Bounded Context
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITutorialRepository, TutorialRepository>();
builder.Services.AddScoped<ICategoryCommandService, CategoryCommandService>();
builder.Services.AddScoped<ICategoryQueryService, CategoryQueryService>();
builder.Services.AddScoped<ITutorialCommandService, TutorialCommandService>();
builder.Services.AddScoped<ITutorialQueryService, TutorialQueryService>();

// Profiles Bounded Context Dependency Injection Configuration
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IProfileCommandService, ProfileCommandService>();
builder.Services.AddScoped<IProfileQueryService, ProfileQueryService>();

// IAM Bounded Context Injection Configuration

// TokenSettings Configuration

builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// Mediator Configuration

// Add Mediator Injection Configuration
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<>), typeof(LoggingCommandBehavior<>));

// Add Cortex Mediator for Event Handling
builder.Services.AddCortexMediator(
    [typeof(Program)]);


var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseGlobalExceptionHandler();

var supportedCultures = new[] { "en", "es" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS Policy
app.UseCors("AllowAllPolicy");

// Add Authorization Middleware to Pipeline
app.UseRequestAuthorization();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();