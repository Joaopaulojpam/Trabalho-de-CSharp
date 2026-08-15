using System.Text;
using Franquias.Api.Data;
using Franquias.Api.Services.Implementations;
using Franquias.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Franquias.Api.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuração do SQLite com EF Core
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=franquias.db";
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // 2. Configurações de JWT
        var jwtSection = configuration.GetSection("JwtSettings");
        services.Configure<JwtSettings>(jwtSection);
        var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

        services.AddAuthentication(options =>
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
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        // 3. Injeção de Dependência dos Serviços de Negócio
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUnidadeService, UnidadeService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IEstoqueService, EstoqueService>();
        services.AddScoped<IVendaService, VendaService>();
        services.AddScoped<IRoyaltyService, RoyaltyService>();
        services.AddScoped<IChamadoService, ChamadoService>();
        services.AddScoped<IRelatorioService, RelatorioService>();

        // 4. CORS permissivo para testes via frontend ou clientes REST
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        });

        return services;
    }
}
