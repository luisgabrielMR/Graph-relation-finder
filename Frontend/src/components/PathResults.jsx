import React from 'react';

const PathResults = ({ shortestPath = [], allPaths = [] }) => {
  const renderNode = (node, index) => (
    <div key={index} className={`node ${index % 2 === 0 ? 'color-0' : 'color-1'}`}>
      <span className="node-icon">
        {index % 2 === 0 ? '⭐' : '🎬'}
      </span>
      {node}
    </div>
  );

  const renderPath = (path, pathIndex) => (
    <div key={pathIndex} className="path-card">
      <div className="path-length">{path.length - 1} graus</div>
      <div className="path-nodes" style={{ display: 'flex', alignItems: 'center', flexWrap: 'wrap' }}>
        {path.map((node, nodeIndex) => (
          <React.Fragment key={nodeIndex}>
            {renderNode(node, nodeIndex)}
            {nodeIndex < path.length - 1 && (
              <span className="arrow" style={{ margin: '0 8px' }}>→</span>
            )}
          </React.Fragment>
        ))}
      </div>
    </div>
  );

  return (
    <div className="results-container">
      {shortestPath && shortestPath.length > 0 && (
        <div className="section">
          <h2>Caminho Mais Curto</h2>
          {renderPath(shortestPath, 0)}
        </div>
      )}

      {allPaths && allPaths.length > 0 && (
        <div className="section">
          <h2>Todos os Caminhos (Até 6 Graus)</h2>
          <div className="paths-grid">
            {allPaths.map((path, index) => renderPath(path, index))}
          </div>
        </div>
      )}
    </div>
  );
};

export default PathResults;