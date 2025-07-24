// Redirects to login if user is not authenticated
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import type { JSX } from 'react';

/**
 * Wraps a route and blocks access if the user is not authenticated
 *
 * @param children - The protected page to render
 * @returns The page if authenticated, otherwise redirects to login
 */
export default function ProtectedRoute({ children }: { children: JSX.Element }) {
  const { isAuthenticated } = useAuth();

  // Allow access if logged in, otherwise redirect to login page
  return isAuthenticated ? children : <Navigate to="/" replace />;
}
