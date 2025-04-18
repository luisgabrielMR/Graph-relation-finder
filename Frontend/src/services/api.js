const API_BASE_URL = 'http://localhost:5024/api';

const handleResponse = async (response) => {
  if (!response.ok) {
    const errorData = await response.json().catch(() => ({}));
    const errorMessage = errorData.error || `Erro ${response.status}: ${response.statusText}`;
    const error = new Error(errorMessage);
    
    // Mapeamento completo das propriedades do erro
    error.details = errorData.details || 'Erro desconhecido';
    error.suggestions = errorData.suggestions || {};
    error.vertex = errorData.vertex || null;
    
    throw error;
  }
  return response.json();
};

const apiHandler = async (endpoint, source, target) => {
  try {
    const response = await fetch(
      `${API_BASE_URL}/path/${endpoint}?source=${encodeURIComponent(source)}&target=${encodeURIComponent(target)}`
    );
    return await handleResponse(response);
  } catch (error) {
    console.error(`API Error (${endpoint}):`, error);
    
    // Estrutura padronizada de erro
    throw {
      message: error.message,
      details: error.details,
      suggestions: error.suggestions || {},
      vertex: error.vertex,
      status: error.response?.status
    };
  }
};

export const findShortestPath = async (source, target) => {
  const data = await apiHandler('shortest', source, target);
  return data.path || [];
};

export const findAllPaths = async (source, target) => {
  const data = await apiHandler('all', source, target);
  return data.paths || [];
};