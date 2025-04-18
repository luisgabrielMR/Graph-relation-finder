using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Grafo
{
    public class Graph
    {
        private readonly Dictionary<string, HashSet<string>> _adjacency;

        public Graph()
        {
            _adjacency = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        }

        // ========== OPERAÇÕES BÁSICAS ==========
        public void AddVertex(string vertex)
        {
            if (string.IsNullOrWhiteSpace(vertex)) 
                return;

            if (!_adjacency.ContainsKey(vertex))
            {
                _adjacency[vertex] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }
        }

        public void AddEdge(string v1, string v2)
        {
            if (v1 == null || v2 == null || v1 == v2)
                return;

            AddVertex(v1);
            AddVertex(v2);

            _adjacency[v1].Add(v2);
            _adjacency[v2].Add(v1);
        }

        // ========== CARREGAMENTO DE DADOS ==========
        public void SeedGraphFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

            try
            {
                using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(filePath));
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    string? title = element.GetProperty("title").GetString();
                    if (string.IsNullOrWhiteSpace(title)) continue;

                    AddVertex(title);

                    foreach (var actor in element.GetProperty("cast").EnumerateArray())
                    {
                        string? actorName = actor.GetString();
                        if (!string.IsNullOrWhiteSpace(actorName))
                        {
                            AddEdge(title, actorName);
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Erro ao ler JSON: {ex.Message}");
            }
        }

        // ========== ALGORITMOS DE BUSCA ==========
        public List<string>? FindShortestPath(string origin, string target)
        {
            if (!ContainsVertex(origin) || !ContainsVertex(target))
                return null;

            if (origin.Equals(target, StringComparison.OrdinalIgnoreCase))
                return new List<string> { origin };

            var visited = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var queue = new Queue<string>();
            queue.Enqueue(origin);
            visited[origin] = null;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in _adjacency[current])
                {
                    if (!visited.ContainsKey(neighbor))
                    {
                        visited[neighbor] = current;
                        queue.Enqueue(neighbor);

                        if (neighbor.Equals(target, StringComparison.OrdinalIgnoreCase))
                        {
                            return ReconstructPath(visited, target);
                        }
                    }
                }
            }
            return null;
        }

        public List<List<string>> FindAllPathsUpTo6Edges(string origin, string target)
        {
            var paths = new List<List<string>>();
            if (!ContainsVertex(origin) || !ContainsVertex(target))
                return paths;

            var stack = new Stack<(string Node, List<string> Path, HashSet<string> Visited)>();
            stack.Push((origin, new List<string> { origin }, new HashSet<string> { origin }));

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (current.Node.Equals(target, StringComparison.OrdinalIgnoreCase))
                {
                    paths.Add(current.Path);
                    continue;
                }

                if (current.Path.Count >= 6) continue;

                foreach (var neighbor in _adjacency[current.Node])
                {
                    if (!current.Visited.Contains(neighbor))
                    {
                        var newPath = new List<string>(current.Path) { neighbor };
                        var newVisited = new HashSet<string>(current.Visited) { neighbor };
                        stack.Push((neighbor, newPath, newVisited));
                    }
                }
            }
            return paths;
        }

        // ========== MÉTODOS AUXILIARES ==========
        private List<string> ReconstructPath(Dictionary<string, string?> visited, string target)
        {
            var path = new List<string>();
            for (var node = target; node != null; node = visited.GetValueOrDefault(node))
            {
                path.Add(node);
            }
            path.Reverse();
            return path;
        }

        public bool ContainsVertex(string vertex)
        {
            return _adjacency.ContainsKey(vertex);
        }

        public List<string> GetClosestMatches(string input, int maxSuggestions = 3)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return _adjacency.Keys
                .Where(k => k.Contains(input, StringComparison.OrdinalIgnoreCase))
                .Take(maxSuggestions)
                .ToList();
        }

        // ========== UTILIDADES ==========
        public string GetGraphSummary()
        {
            return $"Grafo com {_adjacency.Count} vértices e {_adjacency.Sum(v => v.Value.Count)/2} arestas";
        }
    }
}