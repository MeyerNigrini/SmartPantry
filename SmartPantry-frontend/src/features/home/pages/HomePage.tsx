// src/pages/home/HomePage.tsx
import { Title, Stack } from '@mantine/core';
import { useAuth } from '../../../context/AuthContext';

export default function HomePage() {
  const { user } = useAuth();

  return (
    <Stack>
      <Title order={2}>Welcome, {user?.firstName}</Title>
      
    </Stack>
  );
}
