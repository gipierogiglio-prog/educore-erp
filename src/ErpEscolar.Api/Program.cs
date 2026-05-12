using ErpEscolar.Core.Interfaces;
using ErpEscolar.Core.Services;
using ErpEscolar.Infra.Data;
using ErpEscolar.Infra.Repositories;
using ErpEscolar.Infra.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database - PostgreSQL (ou InMemory como fallback)
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrEmpty(connStr))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connStr));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("EduCoreDB"));
}

// Auth
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ErpEscolar-SuperSecret-Key-2024!@#$%";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "ErpEscolar",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "ErpEscolar-App",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services.AddAuthorization();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();
builder.Services.AddScoped<IClassRepository, ClassRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IAcademicService, AcademicService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ErpEscolar API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
});

builder.Services.AddControllers();

var app = builder.Build();

// Auto-migrate database + seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    // Seed admin user
    if (!db.Users.Any())
    {
        var admin = new ErpEscolar.Core.Entities.User
        {
            Name = "Administrador",
            Email = "admin@escola.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "admin",
        };
        db.Users.Add(admin);

        // Seed demo teacher
        var teacher = new ErpEscolar.Core.Entities.User
        {
            Name = "Professor Demo",
            Email = "professor@escola.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
            Role = "teacher",
        };
        db.Users.Add(teacher);
        db.SaveChanges();
    }
}

// Serve static frontend in production
if (!app.Environment.IsDevelopment())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// SPA fallback
if (!app.Environment.IsDevelopment())
{
    app.MapFallbackToFile("index.html");
}

app.Run();
