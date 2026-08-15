using Microsoft.OpenApi.Models;

namespace Franquias.Api.Configurations;

public static class SwaggerConfig
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sistema de Gestão de Franquias - API",
                Version = "v1",
                Description = "Trabalho Acadêmico de Desenvolvimento Web Back-end em C# / ASP.NET Core\n\n" +
                              "**Aluno:** JOÃO PAULO ANDRADE MATHIAS\n" +
                              "**RU:** 5180040\n" +
                              "**Professor:** Rodrigo da S. do Nascimento\n" +
                              "**Repositório:** https://github.com/Joaopaulojpam/Trabalho-de-C-",
                Contact = new OpenApiContact
                {
                    Name = "JOÃO PAULO ANDRADE MATHIAS",
                    Url = new Uri("https://github.com/Joaopaulojpam/Trabalho-de-C-")
                }
            });

            // Configuração do JWT no Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT no formato: Bearer {seu_token}",
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

            // Incluir comentários XML se existirem
            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}
