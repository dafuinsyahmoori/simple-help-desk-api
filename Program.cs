using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleHelpDeskAPI.AuthorizationMiddlewareResultHandlers;
using SimpleHelpDeskAPI.DbContexts;
using SimpleHelpDeskAPI.JsonConverters;
using SimpleHelpDeskAPI.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("SimpleHelpDesk"));
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.MaxAge = TimeSpan.FromMinutes(5);
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserOnly", policy =>
    {
        policy
            .RequireClaim("Role", "User")
            .RequireClaim("ID");
    });

    options.AddPolicy("AdministratorOnly", policy =>
    {
        policy
            .RequireClaim("Role", "Administrator")
            .RequireClaim("ID");
    });
});

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;

        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    });

builder.Services.AddMemoryCache();

builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, UnauthorizedAuthorizationMiddlewareResultHandler>();

builder.Services.AddTransient<IPasswordHasher<object>, PasswordHasher<object>>();

builder.Services.AddTransient<AuthenticationUtility>();
builder.Services.AddTransient<ComplaintUtility>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();