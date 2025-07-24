// Create a preconfigured Axios instance with base URL and JSON headers
import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7193/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

/**
 * Sets or removes the Authorization header for authenticated requests
 *
 * @param token - JWT token string, or null to clear the header
 */
export const setAuthToken = (token: string | null) => {
  if (token) {
    api.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    delete api.defaults.headers.common['Authorization'];
  }
};

export default api;
