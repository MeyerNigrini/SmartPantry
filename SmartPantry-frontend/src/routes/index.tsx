// Declares app routes
import { Route, Routes } from 'react-router-dom';
import LoginPage from '../features/auth/pages/LoginPage';
import HomePage from '../features/home/pages/HomePage';
import ScanProductPage from '../features/product/pages/ScanProductPage';
import ProductListPage from '../features/product/pages/ProductListPage';
import RegisterPage from '../features/auth/pages/RegisterPage';
import ProtectedRoute from './ProtectedRoute';
import Layout from '../components/Layout';

export default function AppRoutes() {
  return (
    <Routes>
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
        {/* Add more protected routes here */}
      </Route>
    </Routes>
  );
}
