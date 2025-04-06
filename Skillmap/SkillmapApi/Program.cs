using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SkillmapApi.Data;
using SkillmapLib1.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders(); 

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Skillmap Api",
        Description = "Skillmap Api",
        Version = "v1",
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT con el formato: Bearer {token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {{
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
            }
        }, Array.Empty<string>() }
    });
});


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scoped = app.Services.CreateScope())
{
    var serviceProvider = scoped.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var dataContext = serviceProvider.GetRequiredService<DataContext>();
    await Seeder.Seed(roleManager, userManager, dataContext);
}

app.Run();
