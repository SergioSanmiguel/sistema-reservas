using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Reserva.Api.Data;
using Reserva.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Config DB
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection.GetValue<string>("Key")!;
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
        ValidAudience = jwtSection.GetValue<string>("Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy" }));

// Map controllers (we will use minimal endpoints for auth and keep controllers for reservations optional)
app.MapPost("/api/auth/register", async (UserRegisterDto dto, AppDbContext db) =>
{
    // verificar email único
    if (await db.Usuarios.AnyAsync(u => u.Email == dto.Email))
        return Results.Conflict(new { message = "Email already registered." });

    var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    var user = new Reserva.Api.Models.Usuario { Email = dto.Email, PasswordHash = hashed, Rol = "Usuario" };
    db.Usuarios.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Email });
});

app.MapPost("/api/auth/login", async (UserLoginDto dto, AppDbContext db) =>
{
    var user = await db.Usuarios.SingleOrDefaultAsync(u => u.Email == dto.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        return Results.Unauthorized();

    // generar token
    var jwt = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwt.GetValue<string>("Key")!);
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new System.Security.Claims.ClaimsIdentity(new []
        {
            new System.Security.Claims.Claim("id", user.Id.ToString()),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Rol)
        }),
        Expires = DateTime.UtcNow.AddMinutes(jwt.GetValue<int>("ExpireMinutes")),
        Issuer = jwt.GetValue<string>("Issuer"),
        Audience = jwt.GetValue<string>("Audience"),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var tokenString = tokenHandler.WriteToken(token);

    return Results.Ok(new { token = tokenString });
});

app.MapGet("/api/reservas", async (AppDbContext db) =>
    await db.Reservas.ToListAsync());

app.MapGet("/api/reservas/{id:int}", async (int id, AppDbContext db) =>
{
    var r = await db.Reservas.FindAsync(id);
    return r is not null ? Results.Ok(r) : Results.NotFound();
});

app.MapPost("/api/reservas", async (ReservaEntity reserva, AppDbContext db) =>
{
    db.Reservas.Add(reserva);
    await db.SaveChangesAsync();
    return Results.Created($"/api/reservas/{reserva.Id}", reserva);
}).RequireAuthorization(); // proteger creación si quieres

app.MapDelete("/api/reservas/{id:int}", async (int id, AppDbContext db) =>
{
    var r = await db.Reservas.FindAsync(id);
    if (r is null) return Results.NotFound();
    db.Reservas.Remove(r);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.Run();


// DTOs al final del archivo para simplicidad
public record UserRegisterDto(string Email, string Password);
public record UserLoginDto(string Email, string Password);

