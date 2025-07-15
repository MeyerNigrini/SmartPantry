// src/pages/home/HomePage.tsx
import { Button, Title, Stack } from '@mantine/core';
import { useAuth } from '../../context/AuthContext';
import { useNavigate } from 'react-router-dom';

export default function HomePage() {
  const { logout, user } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/'); // Redirect to login
  };

  return (
    <Stack align="start" p="xl">
      <Title order={2}>Welcome, {user?.firstName} 👋</Title>
      <Button color="red" onClick={handleLogout}>
        Logout
      </Button>
    </Stack>
  );
}