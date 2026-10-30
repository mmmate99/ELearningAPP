using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ElearningAPI.Data;
using ElearningAPI.Helpers;
using ElearningAPI.Hubs;
using ElearningAPI.Models;
using ElearningAPI.Repository.Implementations;
using ElearningAPI.Repository.Interfaces;
using ElearningAPI.Services.Implementations;
using ElearningAPI.Services.Interfaces;
using ElearningAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "E-Learning API", 
        Version = "v1",
        Description = "API for E-Learning Platform",
        Contact = new OpenApiContact
        {
            Name = "E-Learning Team",
            Email = "support@elearning.com"
        }
    });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Database configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey) || jwtKey.Length < 32)
{
    throw new InvalidOperationException("JWT Key must be at least 32 characters long in appsettings.json");
}

var key = Encoding.ASCII.GetBytes(jwtKey);
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
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

//SignalR
builder.Services.AddSignalR();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repository registrations
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ILessonRepository, LessonRepository>();
builder.Services.AddScoped<IProgressRepository, ProgressRepository>();
builder.Services.AddScoped<IQuizRepository, QuizRepository>();
builder.Services.AddScoped<IForumRepository, ForumRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();

// Service registrations
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IModuleService, ModuleService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IProgressService, ProgressService>(); 
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IForumService, ForumService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Email konfiguráció
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:3000", "https://localhost:3000", "http://localhost:5173", "https://localhost:5173")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Learning API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

//app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
//app.UseMiddleware<ErrorHandlingMiddleware>(); 
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub");

// Ensure database is created and seed initial data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        // Ensure database is created
        context.Database.EnsureCreated();
        
        // Seed roles
        await SeedRoles(roleManager);
        
        // Seed admin user if not exists
        await SeedAdminUser(userManager);

        await SeedData(context, userManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();

// Helper methods for seeding
async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "Teacher", "Student" };
    
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}

async Task SeedData(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
{
    // Seed teacher user
    var teacherEmail = "teacher@elearning.com";
    var teacher = await userManager.FindByEmailAsync(teacherEmail);
    
    if (teacher == null)
    {
        teacher = new ApplicationUser
        {
            UserName = teacherEmail,
            Email = teacherEmail,
            FirstName = "Tanár",
            LastName = "Teszt",
            Role = UserRole.Teacher,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(teacher, "Teacher123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(teacher, "Teacher");
        }
    }

    // Seed student user
    var studentEmail = "student@elearning.com";
    var student = await userManager.FindByEmailAsync(studentEmail);
    
    if (student == null)
    {
        student = new ApplicationUser
        {
            UserName = studentEmail,
            Email = studentEmail,
            FirstName = "Diák",
            LastName = "Teszt",
            Role = UserRole.Student,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(student, "Student123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(student, "Student");
        }
    }

    // Seed sample course if no courses exist
    if (!context.Courses.Any())
    {
        var course = new Course
        {
            Title = "React alapok",
            Description = "Tanulj React-et alapoktól",
            ThumbnailUrl = null,
            Price = 0,
            IsFree = true,
            Difficulty = DifficultyLevel.Beginner,
            TeacherId = teacher!.Id,
            CreatedAt = DateTime.UtcNow,
            IsPublished = true
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync();

        // Add module
        var module = new Module
        {
            Title = "Bevezetés",
            Description = "React alapok megismerése",
            Order = 1,
            CourseId = course.Id
        };

        context.Modules.Add(module);
        await context.SaveChangesAsync();

        // Add lesson
        var lesson = new Lesson
        {
            Title = "Mi a React?",
            Content = "<h2>Bevezetés a React-be</h2><p>A React egy JavaScript library...</p>",
            Order = 1,
            Type = LessonType.Text,
            EstimatedDuration = 30,
            ModuleId = module.Id
        };

        context.Lessons.Add(lesson);
        await context.SaveChangesAsync();
    }
}

async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
{
    var adminEmail = "admin@elearning.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "User",
            Role = UserRole.Admin,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}