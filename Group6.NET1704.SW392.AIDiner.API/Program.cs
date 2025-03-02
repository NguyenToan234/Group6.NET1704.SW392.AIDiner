using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Group6.NET1704.SW392.AIDiner.DAL.Contract;
using Group6.NET1704.SW392.AIDiner.DAL.Data;
using Group6.NET1704.SW392.AIDiner.DAL.Implementation;
using Group6.NET1704.SW392.AIDiner.DAL.Data;
using Group6.NET1704.SW392.AIDiner.DAL.Services;
using Group6.NET1704.SW392.AIDiner.Services.Contract;
using Group6.NET1704.SW392.AIDiner.Services.Implementation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Group6.NET1704.SW392.AIDiner.DAL.Repositories.Interfaces;
using Group6.NET1704.SW392.AIDiner.DAL.Repositories;


var builder = WebApplication.CreateBuilder(args);

// Đăng ký các dịch vụ vào container
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IRestaurantRepository, RestaurantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<IRequestTypeService, RequestTypeService>();

builder.Services.AddScoped<GeminiService>(provider =>
{
    string apiKey = builder.Configuration["Gemini:Key"];
    return new GeminiService(apiKey);
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IIngredientService, IngredientService>();
builder.Services.AddScoped<IAuthenService, AuthenService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký DbContext
builder.Services.AddDbContext<DishHub5Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Lấy cấu hình JWT từ appsettings.json
var jwtIssuer = builder.Configuration["JwtConfig:Issuer"];
var jwtAudience = builder.Configuration["JwtConfig:Audience"];
var jwtKey = builder.Configuration["JwtConfig:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new Exception("JWT Key is missing in configuration.");
}

// Cấu hình Authentication với JWT
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
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authorizationHeader))
            {
                if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    var token = authorizationHeader["Bearer ".Length..].Trim();
                    if (!string.IsNullOrEmpty(token))
                    {
                        context.Token = token;
                    }
                }
                else
                {
                    context.Token = authorizationHeader;
                }
            }

            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var token = context.SecurityToken as JwtSecurityToken;
            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthenService>();

            if (token != null && authService.IsTokenRevoked(token.RawData))
            {
                context.Fail("Token has been revoked.");
            }

            return Task.CompletedTask;
        }

    };

});

// Cấu hình Swagger cho JWT Authorization
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("JWT", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "JWT"
                }
            },
            new string[] { }
        }
    });
});

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Cho phép tất cả các origin
              .AllowAnyMethod()  // Cho phép tất cả các phương thức HTTP (GET, POST, PUT, DELETE, v.v.)
              .AllowAnyHeader(); // Cho phép tất cả các header
    });
});

// Chỉ gọi Build() MỘT LẦN
var app = builder.Build();

// Cấu hình pipeline của ứng dụng
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
// Áp dụng CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();