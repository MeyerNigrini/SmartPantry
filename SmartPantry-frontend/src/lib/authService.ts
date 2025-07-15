// Handles login API call
import api from './api';

interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  token: string;
}

export async function login(request: LoginRequest): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>('/User/login', request);
  return response.data;
}

interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

// Adjust the return type if your backend responds with a token or just status
export async function register(request: RegisterRequest): Promise<void> {
  await api.post('/User/register', request); // ✅ Match your actual backend endpoint
}