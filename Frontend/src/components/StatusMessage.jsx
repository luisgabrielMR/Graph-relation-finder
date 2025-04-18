import React from 'react';

const StatusMessage = ({ error, success, loading, onSuggestionClick }) => {
  if (loading) return <div className="loading">Carregando...</div>;

  return (
    <div className="status-messages">
      {error && (
        <div className="error-message">
          <h3>{error.message || 'Erro'}</h3>
          
          {error.details && <p className="error-detail">{error.details}</p>}

          {(error.suggestions?.sourceSuggestions?.length > 0 || 
            error.suggestions?.targetSuggestions?.length > 0) && (
            <div className="suggestions-box">
              <p>Sugestões:</p>
              <ul>
                {error.suggestions.sourceSuggestions?.map((suggestion, index) => (
                  <li 
                    key={`source-${index}`}
                    onClick={() => onSuggestionClick && onSuggestionClick(suggestion, 'source')}
                    className="suggestion-item"
                  >
                    {suggestion}
                  </li>
                ))}
                
                {error.suggestions.targetSuggestions?.map((suggestion, index) => (
                  <li 
                    key={`target-${index}`}
                    onClick={() => onSuggestionClick && onSuggestionClick(suggestion, 'target')}
                    className="suggestion-item"
                  >
                    {suggestion}
                  </li>
                ))}
              </ul>
            </div>
          )}
        </div>
      )}
      
      {success && <div className="success-message">{success}</div>}
    </div>
  );
};

export default StatusMessage;