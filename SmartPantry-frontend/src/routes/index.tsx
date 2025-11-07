// Declares application routes and maps paths to pages
import { Route, Routes } from 'react-router-dom';

// Pages
import LoginPage from '../features/auth/pages/LoginPage';
import HomePage from '../features/home/pages/HomePage';
import ScanProductPage from '../features/product/pages/ScanProductPage';
import ProductListPage from '../features/product/pages/ProductListPage';
import RegisterPage from '../features/auth/pages/RegisterPage';

// Guards and shared layout
import ProtectedRoute from './ProtectedRoute';
import Layout from '../components/Layout';
import RecipeListPage from '../features/product/pages/RecipeListPage';

export default function AppRoutes() {
  return (
    <Routes>
      {/* Public routes */}
      <Route path="/" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />

      {/* Protected routes */}
      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route path="/home" element={<HomePage />} />
        <Route path="/scan-product" element={<ScanProductPage />} />
        <Route path="/products" element={<ProductListPage />} />
        <Route path="/recipes" element={<RecipeListPage />} />
        {/* Add more protected routes here */}
      </Route>
    </Routes>
  );
}
