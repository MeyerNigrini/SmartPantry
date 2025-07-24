// Axios instance + auth header logic
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7193/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Export token helper
export const setAuthToken = (token: string | null) => {
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete api.defaults.headers.common['Authorization'];
  }
};

export default api;
