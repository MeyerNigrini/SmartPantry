import { TextInput, PasswordInput, Button, Paper, Title, Stack } from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { register } from '../services/authService';

// Notification helpers
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';
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

  const [submitting, setSubmitting] = useState(false);

  /**
   * Handle registration form submission
   *
   * Validates password match before calling API.
   * On success, redirects to login page.
   * On failure, displays appropriate error message.
   */
  const handleRegister = async () => {
    setSubmitting(true);

    // Ensure user confirms their password correctly
    if (password !== confirmPassword) {
      showCustomNotification({
        message: "Passwords don't match",
        type: 'error',
      });
      setSubmitting(false);
      return;
    }

    try {
      // Attempt to register the user with backend API
      await register({ firstName, lastName, email, password });

      showCustomNotification({
        message: 'Registration successful! Please log in.',
        type: 'success',
      });

      // On success, redirect user to login page
      navigate('/');
    } catch (err) {
      showCustomNotification({
        message: getErrorMessage(err, 'Registration failed. Please try again.'),
        type: 'error',
      });
    } finally {
      setSubmitting(false);
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
        <Button fullWidth loading={submitting} disabled={submitting} onClick={handleRegister}>
          Register
        </Button>
      </Stack>
    </Paper>
  );
}
