import { Box, Button, Group, Title } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

export default function Navbar() {
  const { logout } = useAuth();
  const navigate = useNavigate();

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
      <Title order={3} onClick={() => navigate('/home')} style={{ cursor: 'pointer' }}>
        SmartPantry
      </Title>
      <Group>
        <Button size="xs" variant="light" onClick={() => navigate('/products')}>
          My Products
        </Button>
        <Button size="xs" variant="light" onClick={() => navigate('/scan-product')}>
          Scan Product
        </Button>
        <Button color="red" size="xs" onClick={handleLogout}>
          Logout
        </Button>
      </Group>
    </Box>
  );
}
