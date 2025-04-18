using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Grafo;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => 
{
    options.AddPolicy("AllowFrontend", policy => 
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();

var graph = new Graph();
try
{
    graph.SeedGraphFromJson("latest_movies.json");
    Console.WriteLine("Grafo carregado com sucesso.");
}
catch (Exception ex)
{
    Console.WriteLine($"Erro ao carregar grafo: {ex.Message}");
}

// Endpoint para caminho mais curto
app.MapGet("/api/path/shortest", (string source, string target) => 
{
    try
    {
        source = (source ?? "").Trim();
        target = (target ?? "").Trim();

        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target))
            return Results.BadRequest(new { 
                error = "Campos obrigatórios vazios", 
                details = "Preencha ambos os campos de origem e destino",
                suggestion = "Verifique se digitou os nomes corretamente"
            });

        bool sourceExists = graph.ContainsVertex(source);
        bool targetExists = graph.ContainsVertex(target);
        
        if (!sourceExists || !targetExists)
        {
            var suggestions = new {
                sourceSuggestions = !sourceExists ? graph.GetClosestMatches(source) : new List<string>(),
                targetSuggestions = !targetExists ? graph.GetClosestMatches(target) : new List<string>()
            };

            return Results.NotFound(new { 
                error = "Não encontrado",
                details = sourceExists ? $"Destino '{target}' não encontrado" : $"Origem '{source}' não encontrada",
                suggestions,
                vertex = sourceExists ? target : source
            });
        }

        var path = graph.FindShortestPath(source, target);
        
        return path != null 
            ? Results.Ok(new { found = true, path = path }) 
            : Results.NotFound(new { 
                error = "Conexão não encontrada", 
                details = "Não há caminho entre estes itens",
                suggestion = "Tente usar nomes completos ou verifique a grafia"
            });
    }
    catch (Exception ex)
    {
        return Results.Problem(title: "Erro interno", detail: ex.Message);
    }
});

app.MapGet("/api/path/all", (string source, string target) => 
{
    try
    {
        source = (source ?? "").Trim();
        target = (target ?? "").Trim();

        if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target))
            return Results.BadRequest(new { 
                error = "Campos obrigatórios vazios", 
                details = "Preencha ambos os campos de origem e destino",
                suggestion = "Verifique se os nomes estão completos e sem espaços extras"
            });

        bool sourceExists = graph.ContainsVertex(source);
        bool targetExists = graph.ContainsVertex(target);
        
        if (!sourceExists || !targetExists)
        {
            var suggestions = new {
                sourceSuggestions = !sourceExists ? graph.GetClosestMatches(source) : new List<string>(),
                targetSuggestions = !targetExists ? graph.GetClosestMatches(target) : new List<string>()
            };

            return Results.NotFound(new { 
                error = "Não Encontrado",
                details = sourceExists ? $"Destino '{target}' não existe" : $"Origem '{source}' não existe",
                suggestions,
                vertex = sourceExists ? target : source
            });
        }

        var paths = graph.FindAllPathsUpTo6Edges(source, target);
        
        return paths.Count > 0
            ? Results.Ok(new { found = true, paths = paths })
            : Results.NotFound(new { 
                error = "Nenhuma conexão encontrada", 
                details = "Não há caminhos dentro do limite de 6 conexões",
                suggestion = "Tente aumentar o limite de conexões ou verifique nomes alternativos"
            });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: "Erro na busca de caminhos", 
            detail: ex.Message,
            statusCode: 500
        );
    }
});

app.Run();