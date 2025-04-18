import React from 'react';

const SearchButtons = ({ onShortestPath, onAllPaths, loading }) => {
  return (
    <div className="search-buttons">
      <button onClick={onShortestPath} disabled={loading} className="btn btn-primary">
        {loading ? 'Buscando...' : 'Buscar Caminho Mínimo'}
      </button>
      <button onClick={onAllPaths} disabled={loading} className="btn btn-secondary">
        {loading ? 'Buscando...' : 'Buscar Todos os Caminhos'}
      </button>
    </div>
  );
};

export default SearchButtons;
