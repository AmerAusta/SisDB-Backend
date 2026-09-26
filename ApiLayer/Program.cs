using ApiLayer.Authorization;
using BusinessLayer.User.Login;
using DataLayer.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.StaticFiles;


var builder = WebApplication.CreateBuilder(args);

// ===============================
// Database Configuration
// ===============================
builder.Services.AddDbContext<SiSDBDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ===============================
// Dependency Injection (DI)
// ===============================
builder.Services.Scan(scan => scan
    .FromAssemblyOf<BusinessLayer.User.User>()
    .AddClasses(classes => classes.Where(c => !c.Name.EndsWith("Dto")))
    .AsSelf()
    .WithScopedLifetime());

// ===============================
// JWT Authentication Configuration
// ===============================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "UserApi",
            ValidAudience = "ApiUsers",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SIS_SYSTEM_SECRET_KEY_FOR_JWT_AUTHENTICATION_2026")),

            // ربط الأدوار والمعرف بما يتم إنتاجه بالفعل داخل التوكن
            RoleClaimType = ClaimTypes.Role, // يقرأ الرابط http://schemas.microsoft.com/ws/2008/06/identity/claims/role
            NameClaimType = "id"             // يقرأ المفتاح id
        };
    });

// ===============================
// Authorization Configuration
// ===============================
builder.Services.AddScoped<IAuthorizationHandler, UserOwnerOrAdminHandler>();

builder.Services.AddScoped<
    IAuthorizationHandler,
    TeacherOwnerOrAdminHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanAccessStudentData", policy =>
        policy.Requirements.Add(
            new UserOwnerOrAdminRequirement()));

    options.AddPolicy("CanAccessTeacherData", policy =>
        policy.Requirements.Add(
            new TeacherOwnerOrAdminRequirement()));
});

// ===============================
// Rate Limiter Configuration
// ===============================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthLimiter", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ===============================
// Swagger Configuration
// ===============================
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "ادخل الـ JWT Token فقط دون كلمة Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});

// ===============================
// CORS Configuration
// ===============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("UserApiCorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});





// 1. رفع حد IIS / IIS Express
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 524_288_000; // 500 MB
});

// 2. رفع حد Kestrel
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 524_288_000; // 500 MB
});

// 3. رفع حد Multipart Form (الخاص بالملفات المرفوعة)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524_288_000; // 500 MB
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

var app = builder.Build();

// ===============================
// HTTP Request Pipeline
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("UserApiCorsPolicy");

app.UseRateLimiter();
app.UseStaticFiles();

// --- الحل هنا: إضافة دعم لملفات الـ mkv والامتدادات الخاصة ---
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".mkv"] = "video/x-matroska"; // تعريف امتداد الماركوف

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider
});
// ----------------------------------------------------

app.UseAuthentication();
app.UseAuthorization();

// Custom Logging Middleware
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
    {
        await context.Response.WriteAsync("Too many login attempts. Please try again later.");
    }

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        var userId = context.User.FindFirstValue("id")
                  ?? context.User.FindFirstValue(ClaimTypes.NameIdentifier)
                  ?? "anonymous";

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var path = context.Request.Path.ToString();

        app.Logger.LogWarning(
            "Forbidden access. UserId={UserId}, Path={Path}, IP={IP}",
            userId,
            path,
            ip
        );
    }
});

app.MapControllers();

app.Run();