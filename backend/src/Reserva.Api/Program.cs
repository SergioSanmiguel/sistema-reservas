using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Reserva.Api.Data;
using Reserva.Api.Models;

var builder = WebApplication.CreateBuilder(args);

//  Configuración DB
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

//  Swagger con soporte JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Reserva API", Version = "v1" });

    //  Configuración JWT
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer {token}' para autenticar."
    };

    options.AddSecurityDefinition("Bearer", securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, Array.Empty<string>() }
    });
});

//  CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

//  JWT
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UsuarioOrAdmin", policy => policy.RequireRole("Usuario", "Admin"));
});

var app = builder.Build();

//  Middlewares
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

//  Endpoint de prueba
app.MapGet("/api/health", () => Results.Ok(new { status = "Healthy" }));

//  Registro de usuario
app.MapPost("/api/auth/register", async (UserRegisterDto dto, AppDbContext db) =>
{
    if (await db.Usuarios.AnyAsync(u => u.Email == dto.Email))
        return Results.Conflict(new { message = "Email already registered." });

    var hashed = BCrypt.Net.BCrypt.HashPassword(dto.Password);
    var user = new Usuario { Email = dto.Email, PasswordHash = hashed, Rol = "Usuario" };
    db.Usuarios.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", new { user.Id, user.Email, user.Rol });
});

//  Login con token JWT
app.MapPost("/api/auth/login", async (UserLoginDto dto, AppDbContext db) =>
{
    var user = await db.Usuarios.SingleOrDefaultAsync(u => u.Email == dto.Email);
    if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        return Results.Unauthorized();

    var jwt = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwt.GetValue<string>("Key")!);
    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new System.Security.Claims.ClaimsIdentity(new[]
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

    return Results.Ok(new { token = tokenString, role = user.Rol });
});

//  Reservas
app.MapGet("/api/reservas", async (AppDbContext db) =>
    await db.Reservas.ToListAsync())
    .RequireAuthorization("UsuarioOrAdmin"); // 👈 Solo usuarios autenticados

app.MapGet("/api/reservas/{id:int}", async (int id, AppDbContext db) =>
{
    var r = await db.Reservas.FindAsync(id);
    return r is not null ? Results.Ok(r) : Results.NotFound();
}).RequireAuthorization("UsuarioOrAdmin");

app.MapPost("/api/reservas", async (ReservaEntity reserva, AppDbContext db) =>
{
    db.Reservas.Add(reserva);
    await db.SaveChangesAsync();
    return Results.Created($"/api/reservas/{reserva.Id}", reserva);
}).RequireAuthorization("UsuarioOrAdmin");

app.MapDelete("/api/reservas/{id:int}", async (int id, AppDbContext db) =>
{
    var r = await db.Reservas.FindAsync(id);
    if (r is null) return Results.NotFound();
    db.Reservas.Remove(r);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization("AdminOnly"); // 👈 Solo admins pueden borrar

app.Run();

// DTOs
public record UserRegisterDto(string Email, string Password);
public record UserLoginDto(string Email, string Password);


