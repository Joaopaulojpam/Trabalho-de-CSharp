using System.Text.Json.Serialization;
using Franquias.Api.Configurations;
using Franquias.Api.Data;
using Franquias.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos Controllers com conversão de Enums para string no JSON
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Configurações de Infraestrutura (SQLite, JWT, Injeção de Dependências)
builder.Services.AddInfrastructure(builder.Configuration);

// Configuração do Swagger com suporte a Bearer Token e documentação do Aluno
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

// Inicialização e Seed automático do Banco de Dados SQLite
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await DbInitializer.InitializeAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao inicializar e alimentar o banco de dados SQLite.");
    }
}

// Middleware global para tratamento de erros
app.UseMiddleware<ExceptionMiddleware>();

// Habilita o Swagger para documentação e testes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Gestão de Franquias API v1");
    c.RoutePrefix = string.Empty; // Define o Swagger na raiz da aplicação (http://localhost:5000/)
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
