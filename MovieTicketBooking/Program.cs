
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.Data;
using MovieTicketBooking.Models;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interface;
using MovieTicketBooking.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using MovieTicketBooking.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var key = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT Key missing");

var issuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT Issuer missing");

var audience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT Audience missing");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = issuer,
        ValidAudience = audience,

        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key)),

        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role,

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("BookingOwnerOrAdmin", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.AddRequirements(new BookingOwnerRequirement());
    });
});

builder.Services.AddScoped<IAuthorizationHandler,
    BookingOwnerAuthorizationHandler>();

builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 1. HTTPS Redirection
app.UseHttpsRedirection();

// 2. Routing
app.UseRouting();

// 3. Endpoint Metadata Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<EndpointMetadataMiddleware>();
}

// 4. Authentication
app.UseAuthentication();

// 5. Authentication Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthenticationDiagnosticsMiddleware>();
}

// 6. Authorization Diagnostics
if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<AuthorizationDiagnosticsMiddleware>();
}

// 7. Authorization
app.UseAuthorization();

// 8. Map Controllers
app.MapControllers();

// 9. Run Application
app.Run();

