using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WuyiMusic_DAL.Helper;
using WuyiMusic_DAL.IReponsitories;
using WuyiMusic_DAL.Models;
using WuyiMusic_DAL.Reponsitories;
using WuyiMusic_Services.IServices;
using WuyiMusic_Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ vào container.
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        });
builder.Services.AddDbContext<WuyiMusic_DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Cấu hình dịch vụ xác thực
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["ValidIssuer"],
        ValidAudience = jwtSettings["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
    };
});

builder.Services.AddAutoMapper(typeof(MappingProfiles)); // Đăng ký profile

// Thêm các dịch vụ repository và service
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
<<<<<<< HEAD
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
=======
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
>>>>>>> 8b21784ebdd9a0a14a9d08211c14ef821522d68f

var app = builder.Build();

// Cấu hình pipeline yêu cầu HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
