using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

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

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllers();

app.Run();
