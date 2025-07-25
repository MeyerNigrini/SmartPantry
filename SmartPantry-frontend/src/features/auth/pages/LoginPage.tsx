// UI components and hooks
import { TextInput, PasswordInput, Button, Paper, Title, Stack } from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

// API service for performing the login HTTP request
import { login as loginApi } from '../services/authService';

// Custom React Context hook to persist user login state globally
import { useAuth } from '../../../context/AuthContext';

// Notification helpers
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';

/**
 * LoginPage Component
 *
 * Renders a login form with email/password inputs and handles user authentication.
 * On successful login, saves the user to context and navigates to the homepage.
 */
export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [submitting, setSubmitting] = useState(false);

  /**
   * Handle form submission
   *
   * Attempts to log in with the entered email and password.
   * If successful, persists auth state and redirects to `/home`.
   * If failed, displays an error message via a notification.
   */
  const handleLogin = async () => {
    setSubmitting(true);

    try {
      // Call backend login API with credentials
      const user = await loginApi({ email, password });

      // Save user in context
      login(user);

      // Redirect to home page on success
      navigate('/home');
    } catch (err) {
      showCustomNotification({
        message: getErrorMessage(err, 'Login failed. Please try again.'),
        type: 'error',
      });
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Paper withBorder shadow="sm" p="xl" radius="md" maw={400} mx="auto" mt="xl">
      <Title order={2} mb="md">
        Login
      </Title>
      <Stack>
        <TextInput label="Email" value={email} onChange={(e) => setEmail(e.currentTarget.value)} />
        <PasswordInput
          label="Password"
          value={password}
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
        <Button fullWidth loading={submitting} disabled={submitting || !email || !password} onClick={handleLogin}>
          Login
        </Button>

        {/* Register Button */}
        <Button variant="outline" fullWidth mt="md" onClick={() => navigate('/register')}>
          Don't have an account? Register
        </Button>
      </Stack>
    </Paper>
  );
}
