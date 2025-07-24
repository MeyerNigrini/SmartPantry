import { TextInput, PasswordInput, Button, Paper, Title, Stack, Notification } from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { register } from '../services/authService';

/**
 * RegisterPage Component
 *
 * Allows a new user to sign up with their personal details and credentials.
 * On successful registration, redirects to the login page.
 */
export default function RegisterPage() {
  const navigate = useNavigate();

  // Form state
  const [firstName, setFirstName] = useState('');
  const [lastName, setLastName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');

  /**
   * Handle registration form submission
   *
   * Validates password match before calling API.
   * On success, redirects to login page.
   * On failure, displays appropriate error message.
   */
  const handleRegister = async () => {
    setError('');

    // Ensure user confirms their password correctly
    if (password !== confirmPassword) {
      setError("Passwords don't match");
      return;
    }

    try {
      // Attempt to register the user with backend API
      await register({ firstName, lastName, email, password });

      // On success, redirect user to login page
      navigate('/');
    } catch (err: unknown) {
      if (err && typeof err === 'object' && 'response' in err) {
        const errorTyped = err as {
          response?: { data?: { message?: string } };
        };
        setError(errorTyped.response?.data?.message || 'Registration failed');
      } else {
        setError('Registration failed');
      }
    }
  };

  return (
    <Paper withBorder shadow="sm" p="xl" radius="md" maw={400} mx="auto" mt="xl">
      <Title order={2} mb="md">
        Register
      </Title>
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
        <TextInput label="Email" value={email} onChange={(e) => setEmail(e.currentTarget.value)} />
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
        <Button fullWidth onClick={handleRegister}>
          Register
        </Button>

        {error && (
          <Notification color="red" mt="md">
            {error}
          </Notification>
        )}
      </Stack>
    </Paper>
  );
}
