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
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStudentService, StudentService>();

// Teachers
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IAcademicService, AcademicService>();
builder.Services.AddScoped<ICourseService, CourseService>();

// SchoolYear
builder.Services.AddScoped<ISchoolYearRepository, SchoolYearRepository>();
builder.Services.AddScoped<ISchoolYearService, SchoolYearService>();

// Permissions
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionGroupRepository, PermissionGroupRepository>();
builder.Services.AddScoped<IUserPermissionRepository, UserPermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

// Enrollment
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();

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

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

var app = builder.Build();

// Auto-migrate database + seed
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();

        // Seed default organization
        var defaultOrg = db.Organizations.FirstOrDefault();
        if (defaultOrg == null)
        {
            defaultOrg = new ErpEscolar.Core.Entities.Organization
            {
                Name = "Escola Demo",
                Slug = "escola-demo",
                Status = "active",
            };
            db.Organizations.Add(defaultOrg);
            db.SaveChanges();
        }

        // Seed admin user
        if (!db.Users.Any())
        {
            var admin = new ErpEscolar.Core.Entities.User
            {
                Name = "Administrador",
                Email = "admin@escola.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "org_admin",
                OrganizationId = defaultOrg.Id,
            };
            db.Users.Add(admin);

            // Seed demo teacher
            var teacher = new ErpEscolar.Core.Entities.User
            {
                Name = "Professor Demo",
                Email = "professor@escola.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "teacher",
                OrganizationId = defaultOrg.Id,
            };
            db.Users.Add(teacher);
            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Database seed skipped (non-critical)");
    }
}

// Seed default permissions (separado)
try
{
    using (var permScope = app.Services.CreateScope())
    {
        var permService = permScope.ServiceProvider.GetRequiredService<IPermissionService>();
        await permService.SeedDefaultPermissionsAsync();
        var logger = permScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Permissoes padrao seedadas com sucesso");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Seed de permissoes skipped");
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
