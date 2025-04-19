using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Grafo
{
    public class Graph
    {
        // O grafo armazenado como lista de adjacência usando LinkedList
        private readonly Dictionary<string, LinkedList<string>> _adjacency;
        // Conjunto para identificar os filmes (a partir dos dados do JSON)
        private readonly HashSet<string> _movies;

        public Graph()
        {
            _adjacency = new Dictionary<string, LinkedList<string>>(StringComparer.OrdinalIgnoreCase);
            _movies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        // ========== OPERAÇÕES BÁSICAS ==========
        public void AddVertex(string vertex)
        {
            if (string.IsNullOrWhiteSpace(vertex))
                return;

            if (!_adjacency.ContainsKey(vertex))
                _adjacency[vertex] = new LinkedList<string>();
        }

        public void AddEdge(string v1, string v2)
        {
            if (string.IsNullOrWhiteSpace(v1) ||
                string.IsNullOrWhiteSpace(v2) ||
                v1.Equals(v2, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            AddVertex(v1);
            AddVertex(v2);

            if (!_adjacency[v1].Contains(v2))
                _adjacency[v1].AddLast(v2);
            if (!_adjacency[v2].Contains(v1))
                _adjacency[v2].AddLast(v1);
        }

        // ========== CARREGAMENTO DE DADOS ==========
        public void SeedGraphFromJson(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Arquivo não encontrado: {filePath}");

            try
            {
                string jsonText = File.ReadAllText(filePath);
                using JsonDocument doc = JsonDocument.Parse(jsonText);
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    // O título é o identificador do filme
                    string? title = element.GetProperty("title").GetString();
                    if (string.IsNullOrWhiteSpace(title))
                        continue;

                    AddVertex(title);
                    _movies.Add(title);

                    // Para cada ator no elenco, cria conexão filme-ator
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

            // Garante que os dois vértices sejam atores (não estando no conjunto _movies)
            if (_movies.Contains(origin) || _movies.Contains(target))
                return null;

            if (origin.Equals(target, StringComparison.OrdinalIgnoreCase))
                return DecoratePath(new List<string> { origin });

            var queue = new Queue<(string Node, int Depth)>();
            queue.Enqueue((origin, 0));

            var visited = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            {
                [origin] = null
            };

            while (queue.Count > 0)
            {
                var (current, depth) = queue.Dequeue();
                if (depth >= 6)
                    continue;  // Limite de 6 arestas

                foreach (var neighbor in _adjacency[current])
                {
                    if (!visited.ContainsKey(neighbor))
                    {
                        visited[neighbor] = current;
                        int nextDepth = depth + 1;
                        if (neighbor.Equals(target, StringComparison.OrdinalIgnoreCase))
                        {
                            var path = ReconstructPath(visited, target);
                            if (path.Count <= 7 && IsValidPath(path))
                                return DecoratePath(path);
                            else
                                return null;
                        }
                        queue.Enqueue((neighbor, nextDepth));
                    }
                }
            }
            return null;
        }

        public List<List<string>> FindAllPathsUpTo6Edges(string origin, string target)
        {
            var rawPaths = new List<List<string>>();
            if (!ContainsVertex(origin) || !ContainsVertex(target))
                return rawPaths;

            if (_movies.Contains(origin) || _movies.Contains(target))
                return rawPaths;

            var path = new List<string>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            DFSFindAllPaths(origin, target, 0, path, visited, rawPaths);

            var decoratedPaths = rawPaths.Select(p => DecoratePath(p)).ToList();
            return decoratedPaths;
        }

        private void DFSFindAllPaths(
            string current,
            string target,
            int depth,
            List<string> path,
            HashSet<string> visited,
            List<List<string>> paths)
        {
            path.Add(current);
            visited.Add(current);

            if (current.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                if (path.Count <= 7 && IsValidPath(path))
                    paths.Add(new List<string>(path));
            }
            else if (depth < 6)
            {
                if (_adjacency.ContainsKey(current))
                {
                    foreach (var neighbor in _adjacency[current])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            DFSFindAllPaths(neighbor, target, depth + 1, path, visited, paths);
                        }
                    }
                }
            }

            path.RemoveAt(path.Count - 1);
            visited.Remove(current);
        }

        private List<string> ReconstructPath(Dictionary<string, string?> visited, string target)
        {
            var path = new List<string>();
            for (string? node = target; node != null; node = visited[node])
            {
                path.Add(node);
            }
            path.Reverse();
            return path;
        }

        // Valida o caminho garantindo que haja alternância entre ator e filme
        private bool IsValidPath(List<string> path)
        {
            if (path == null || path.Count == 0)
                return false;

            // Primeiro e último devem ser atores (não pertencem a _movies)
            if (_movies.Contains(path[0]) || _movies.Contains(path[path.Count - 1]))
                return false;

            for (int i = 0; i < path.Count; i++)
            {
                if (i % 2 == 0)
                {
                    // Posições pares devem ser atores
                    if (_movies.Contains(path[i]))
                        return false;
                }
                else
                {
                    // Posições ímpares devem ser filmes
                    if (!_movies.Contains(path[i]))
                        return false;
                }
            }
            return true;
        }

        // ========== DECORAÇÃO DOS NÓS (FUNÇÃO RECURSIVA) ==========
        public List<string> DecoratePath(List<string> path)
        {
            var decorated = new List<string>();
            DecoratePathRecursive(path, 0, decorated);
            return decorated;
        }

        private void DecoratePathRecursive(List<string> path, int index, List<string> decorated)
        {
            if (index >= path.Count)
                return;

            string node = path[index];
            string prefix = _movies.Contains(node) ? "[Filme]" : "[Ator]";
            decorated.Add($"{prefix} {node}");
            DecoratePathRecursive(path, index + 1, decorated);
        }

        // ========== MÉTODOS AUXILIARES ==========
        public bool ContainsVertex(string vertex)
        {
            return _adjacency.ContainsKey(vertex);
        }

        // Adicionamos o método GetClosestMatches conforme esperado pelo frontend
        public List<string> GetClosestMatches(string input, int maxSuggestions = 3)
        {
            if (string.IsNullOrWhiteSpace(input))
                return new List<string>();

            return _adjacency.Keys
                .Where(k => k.Contains(input, StringComparison.OrdinalIgnoreCase))
                .Take(maxSuggestions)
                .ToList();
        }

        public string GetGraphSummary()
        {
            int edgeCount = _adjacency.Sum(kvp => kvp.Value.Count) / 2;
            return $"Grafo com {_adjacency.Count} vértices e {edgeCount} arestas";
        }
    }
}
