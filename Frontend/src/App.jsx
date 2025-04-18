import React, { useState } from 'react';
import './App.css';
import ActorInput from './components/ActorInput.jsx';
import SearchButtons from './components/SearchButtons.jsx';
import PathResults from './components/PathResults.jsx';
import StatusMessage from './components/StatusMessage.jsx'; 
import { findShortestPath, findAllPaths } from './services/api.js';

function App() {
  const [sourceActor, setSourceActor] = useState('');
  const [targetActor, setTargetActor] = useState('');
  const [shortestPath, setShortestPath] = useState(null);
  const [allPaths, setAllPaths] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const handleSearch = async (searchType) => {
    const cleanSource = sourceActor.trim();
    const cleanTarget = targetActor.trim();

    if (!cleanSource || !cleanTarget) {
      setError({ 
        message: 'Preencha ambos os campos', 
        details: 'Nomes de ator/filme são obrigatórios' 
      });
      return;
    }

    setLoading(true);
    setError(null);
    setSuccess(null);
    setShortestPath(null);
    setAllPaths([]);

    try {
      if (searchType === 'shortest') {
        const path = await findShortestPath(cleanSource, cleanTarget);
        if (path?.length > 0) {
          setShortestPath(path);
          setSuccess(`Caminho encontrado: ${path.join(' → ')}`);
        } else {
          setError({ 
            message: 'Caminho não encontrado', 
            details: 'Não há conexão entre os itens solicitados' 
          });
        }
      } else {
        const paths = await findAllPaths(cleanSource, cleanTarget);
        if (paths?.length > 0) {
          setAllPaths(paths);
          setSuccess(`${paths.length} caminhos encontrados`);
        } else {
          setError({ 
            message: 'Nenhum caminho encontrado', 
            details: 'Tente aumentar o limite de conexões' 
          });
        }
      }
    } catch (err) {
      setError({
        message: err.message || 'Erro na busca',
        details: err.details,
        suggestions: err.suggestions,
        vertex: err.vertex
      });
    } finally {
      setLoading(false);
    }
  };
  
  const handleSuggestionClick = (suggestionText, fieldType) => {
    if (fieldType === 'source') {
      setSourceActor(suggestionText);
    } else {
      setTargetActor(suggestionText);
    }
    setError(null);
    if (fieldType === 'source') {
      document.querySelector('input[name="source"]')?.focus();
    } else {
      document.querySelector('input[name="target"]')?.focus();
    }
  };


  return (
    <div className="app-container">
      <header className="app-header">
        <h1>6 GRAUS DE SEPARAÇÃO</h1>
      </header>

      <div className="search-section">
        <ActorInput
          label="Origem"
          value={sourceActor}
          onChange={(e) => setSourceActor(e.target.value)}
          placeholder="Ex: Tom Hanks"
        />
        
        <ActorInput
          label="Destino"
          value={targetActor}
          onChange={(e) => setTargetActor(e.target.value)}
          placeholder="Ex: Leonardo DiCaprio"
        />

        <SearchButtons 
          onShortestPath={() => handleSearch('shortest')}
          onAllPaths={() => handleSearch('all')}
          loading={loading}
        />
      </div>

      <StatusMessage 
        error={error}
        success={success}
        loading={loading}
        onSuggestionClick={handleSuggestionClick}
      />

      <PathResults 
        shortestPath={shortestPath}
        allPaths={allPaths}
      />
    </div>
    
  );
  
}

export default App;