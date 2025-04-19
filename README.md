6 Graus de Network
Este repositório contém o código-fonte do sistema que explora conexões entre atores e filmes, inspirado na teoria dos “6 graus de separação”. Ele utiliza estruturas de grafos para descobrir ligações entre atores por meio dos filmes em que atuaram.

🛠 Tecnologias Utilizadas
Backend: C# (.NET 7 ou superior)

Frontend: (em desenvolvimento)

Banco de Dados: Nenhum — os dados são carregados a partir de um arquivo JSON

Ferramentas: Visual Studio / VS Code, Postman, Docker (opcional)

📡 API RESTful
A API foi desenvolvida seguindo a arquitetura RESTful, permitindo operações de consulta para encontrar:

O caminho mais curto entre dois atores.

Todos os caminhos possíveis (com até 6 arestas).

Ela trabalha sobre um grafo construído a partir de filmes e seus respectivos elencos.

🎯 Funcionalidades
📥 Leitura de dados a partir do arquivo latest_movies.json.

🔗 Construção de grafo com conexões entre atores e filmes.

🔎 Busca pelo menor caminho entre dois atores (BFS).

🧭 Busca por todos os caminhos possíveis (DFS limitada a 6 arestas).

🚫 Apenas conexões do tipo ator-filme-ator são permitidas (nunca ator-ator direto ou filme-filme).