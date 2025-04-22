using System.Text;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Multitenancy.Authentication;
using Multitenancy.Finbuckle_Multitenant;
using Microsoft.EntityFrameworkCore;
using Finbuckle.MultiTenant;
using Multitenancy.Context;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
builder.Services.AddScoped<IAuth, Auth>();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHeaderStrategy("Tenant")
    .WithInMemoryStore(options =>
    {
        options.Tenants.Add(new AppTenantInfo { Id = "1", Identifier = "CenteralOwner", ConnectionString = "Server=localhost;Database=CenteralDb;Integrated Security=True;TrustServerCertificate=True;" });
        options.Tenants.Add(new AppTenantInfo { Id = "2", Identifier = "Diggit", ConnectionString = "Server=localhost;Database=EcommApp;Integrated Security=True;TrustServerCertificate=True;" });
        options.Tenants.Add(new AppTenantInfo { Id = "3", Identifier = "Progatix", ConnectionString = "Server=localhost;Database=Demo;Integrated Security=True;TrustServerCertificate=True;" });
    });
builder.Services.AddDbContext<TenantContext>((serviceProvider, options) =>
{
    var accessor = serviceProvider.GetRequiredService<IMultiTenantContextAccessor<AppTenantInfo>>();
    var tenantInfo = accessor.MultiTenantContext?.TenantInfo;

    if (tenantInfo == null || string.IsNullOrEmpty(tenantInfo.ConnectionString))
        throw new InvalidOperationException("Please provide proper tenant key while logging");

    options.UseSqlServer(tenantInfo.ConnectionString);
});
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "InventoryAPI",
        Version = "v1"
    });

    // Define security scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    // Apply security to endpoints
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
                Array.Empty<string>()
            }
        });
});
var app = builder.Build();
app.UseMultiTenant();
// Configure the HTTP request pipeline.
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
