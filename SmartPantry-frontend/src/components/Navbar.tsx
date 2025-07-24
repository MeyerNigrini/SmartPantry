// Navbar component with navigation and logout functionality
import { Box, Button, Group, Title } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { logout } = useAuth(); // Logout method from auth context
  const navigate = useNavigate();

  // Handles logout and redirects to login page
  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <Box
      component="nav"
      px="md"
      py="sm"
      style={{
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        borderBottom: '1px solid #dee2e6',
      }}
    >
      {/* App title with navigation to home */}
      <Title order={3} onClick={() => navigate('/home')} style={{ cursor: 'pointer' }}>
        SmartPantry
      </Title>
      <Group>
        {/* Navigates to the ProductList page (/products) */}
        <Button size="xs" variant="light" onClick={() => navigate('/products')}>
          My Products
        </Button>
        {/* Navigates to the ScanProduct page (/scan-product) */}
        <Button size="xs" variant="light" onClick={() => navigate('/scan-product')}>
          Scan Product
        </Button>
        {/* Logs the user out and redirects to the Login page (/) */}
        <Button color="red" size="xs" onClick={handleLogout}>
          Logout
        </Button>
      </Group>
    </Box>
  );
}
