// Handles login API call
import api from '../../../lib/api';

/**
 * Login request payload
 */
interface LoginRequest {
  email: string;
  password: string;
}

/**
 * Login response returned by the backend
 */
export interface LoginResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  token: string;
}

/**
 * Calls the backend login endpoint to authenticate a user
 *
 * @param request LoginRequest - user's email and password
 * @returns LoginResponse - user info and access token on success
 */
export async function login(request: LoginRequest): Promise<LoginResponse> {
  const response = await api.post<LoginResponse>('/User/login', request);
  return response.data;
}

/**
 * Registration request payload
 */
interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

/**
 * Calls the backend register endpoint to create a new user
 *
 * @param request RegisterRequest - new user's basic info and password
 */
export async function register(request: RegisterRequest): Promise<void> {
  await api.post('/User/register', request);
}
