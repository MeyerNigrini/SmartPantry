import {
  TextInput,
  PasswordInput,
  Button,
  Paper,
  Title,
  Stack,
  Notification,
} from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { register } from '../../lib/authService';

export default function RegisterPage() {
  const navigate = useNavigate();

  // Form state
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');

  // Handle register form submission
  const handleRegister = async () => {
    setError('');

    if (password !== confirmPassword) {
      setError("Passwords don't match");
      return;
    }

    try {
      await register({ firstName, lastName, email, password });
      navigate('/'); // Redirect to login on success
    } catch (err: unknown) {
      if (err && typeof err === 'object' && 'response' in err) {
        const errorTyped = err as { response?: { data?: { message?: string } } };
        setError(errorTyped.response?.data?.message || 'Registration failed');
      } else {
        setError('Registration failed');
      }
    }
  };

  return (
    <Paper withBorder shadow="sm" p="xl" radius="md" maw={400} mx="auto" mt="xl">
      <Title order={2} mb="md">Register</Title>
      <Stack>
        <TextInput
          label="First Name"
          value={firstName}
          onChange={(e) => setFirstName(e.currentTarget.value)}
        />
        <TextInput
          label="Last Name"
          value={lastName}
          onChange={(e) => setLastName(e.currentTarget.value)}
        />
        <TextInput
          label="Email"
          value={email}
          onChange={(e) => setEmail(e.currentTarget.value)}
        />
        <PasswordInput
          label="Password"
          value={password}
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
        <PasswordInput
          label="Confirm Password"
          value={confirmPassword}
          onChange={(e) => setConfirmPassword(e.currentTarget.value)}
        />
        <Button fullWidth onClick={handleRegister}>Register</Button>

        {error && (
          <Notification color="red" mt="md">
            {error}
          </Notification>
        )}
      </Stack>
    </Paper>
  );
}
