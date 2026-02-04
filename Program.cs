using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VEHICLE_CREATE",
        policy => policy.Requirements.Add(new PermissionRequirement("VEHICLE_CREATE"))
    );
    options.AddPolicy("VEHICLE_UPDATE",
        policy => policy.Requirements.Add(new PermissionRequirement("VEHICLE_UPDATE"))
    );
    options.AddPolicy("VEHICLE_DELETE",
        policy => policy.Requirements.Add(new PermissionRequirement("VEHICLE_DELETE"))
    );
    options.AddPolicy("VEHICLE_VIEW",
        policy => policy.Requirements.Add(new PermissionRequirement("VEHICLE_VIEW"))
    );
    options.AddPolicy("CUSTOMER_CREATE",
        policy => policy.Requirements.Add(new PermissionRequirement("CUSTOMER_CREATE"))
    );
    options.AddPolicy("CUSTOMER_UPDATE",
        policy => policy.Requirements.Add(new PermissionRequirement("CUSTOMER_UPDATE"))
    );
    options.AddPolicy("CUSTOMER_DELETE",
        policy => policy.Requirements.Add(new PermissionRequirement("CUSTOMER_DELETE"))
    );
    options.AddPolicy("CUSTOMER_VIEW",
        policy => policy.Requirements.Add(new PermissionRequirement("CUSTOMER_VIEW"))
    );
    options.AddPolicy("VEHICLEREPAIR_CREATE",
       policy => policy.Requirements.Add(new PermissionRequirement("VEHICLEREPAIR_CREATE"))
   );
    options.AddPolicy("VEHICLEREPAIR_UPDATE",
       policy => policy.Requirements.Add(new PermissionRequirement("VEHICLEREPAIR_UPDATE"))
   );
    options.AddPolicy("VEHICLEREPAIR_DELETE",
       policy => policy.Requirements.Add(new PermissionRequirement("VEHICLEREPAIR_DELETE"))
   );
    options.AddPolicy("VEHICLEREPAIR_VIEW",
       policy => policy.Requirements.Add(new PermissionRequirement("VEHICLEREPAIR_VIEW"))
   );
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");

app.UseAuthentication();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
