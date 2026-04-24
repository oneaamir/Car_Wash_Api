using System.Text;
using CarWash.Backend.Data;
using CarWash.Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<JwtService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authentication -> "Tum kaun ho?"
// User login karta hai, credentials verify hote hain,
// aur successful login ke baad JWT token issue hota hai.
app.UseAuthentication();

// Authorization -> "Tumhe kya karne ki permission hai?"
// JWT ke andar roles/claims hote hain.
// Server check karta hai ki user requested resource access kar sakta hai ya nahi.
app.UseAuthorization();

// Incoming request me Authorization header se Bearer token read hota hai.
// Bearer ek keyword hai jo batata hai ki request ke saath access token bheja gaya hai.
app.MapControllers();

app.Run();
