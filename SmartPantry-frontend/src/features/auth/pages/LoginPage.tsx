// UI components and hooks
import {
  TextInput,
  PasswordInput,
  Button,
  Paper,
  Title,
  Text,
  Stack,
  Box,
  Grid,
  Group,
  Anchor,
} from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

import { IconMail, IconLock } from '@tabler/icons-react';

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
    // Page container (full-height)
    <Box mih="100vh">
      <Grid gutter={0}>
        {/* Left section: hero image with overlay, hidden on small screens */}
        <Grid.Col span={{ base: 0, lg: 6 }}>
          <Box
            h="100vh"
            pos="relative"
            style={{
              backgroundImage:
                'url(https://images.unsplash.com/photo-1621318551436-68573392fd5c?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxraXRjaGVuJTIwcGFudHJ5JTIwZm9vZCUyMHN0b3JhZ2V8ZW58MXx8fHwxNzU3MzIyMjI5fDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral)',
              backgroundSize: 'cover',
              backgroundPosition: 'center',
            }}
          >
            {/* --- Gradient overlay */}
            <Box
              pos="absolute"
              inset={0}
              style={{
                background: 'linear-gradient(135deg, rgba(3,2,19,0.20), rgba(3,2,19,0.40))',
              }}
            />
            {/* Centered headline */}
            <Box
              pos="absolute"
              inset={0}
              p="xl"
              style={{
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
              }}
            >
              <Box maw={480} ta="center" c="white">
                <Title order={2} fw={500} mb="sm">
                  Smart Pantry Management
                </Title>
                <Text size="lg" style={{ opacity: 0.9 }}>
                  Organize your food inventory, track expiration dates, and discover delicious
                  AI-generated recipes based on what you have.
                </Text>
              </Box>
            </Box>
          </Box>
        </Grid.Col>

        {/* Right section: login card */}
        <Grid.Col span={{ base: 12, lg: 6 }}>
          <Box
            h="100vh"
            style={{
              background: '#f6f6f8',
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'center',
              padding: '2rem',
            }}
          >
            {/* Auth card */}
            <Paper withBorder shadow="sm" p="xl" radius="md" maw={420} w="100%">
              <Stack gap="sm">
                <Box ta="center" mb="sm">
                  <Title order={2} mb={4}>
                    Welcome to SmartPantry
                  </Title>
                  <Text c="dimmed" size="sm">
                    Sign in to manage your pantry and discover AI-powered recipes
                  </Text>
                </Box>

                {/* Email field */}
                <TextInput
                  label="Email"
                  placeholder="Enter your email"
                  value={email}
                  onChange={(e) => setEmail(e.currentTarget.value)}
                  leftSection={<IconMail size={16} />}
                  autoComplete="email"
                />

                {/* Password field */}
                <PasswordInput
                  label="Password"
                  placeholder="Enter your password"
                  value={password}
                  onChange={(e) => setPassword(e.currentTarget.value)}
                  leftSection={<IconLock size={16} />}
                  autoComplete="current-password"
                />

                {/* Forgot Password link */}
                <Group justify="flex-end" mt={-6}>
                  <Anchor href="#" underline="hover" fz="sm" c="dimmed">
                    Forgot password?
                  </Anchor>
                </Group>

                {/* Primary action: Sign In */}
                <Button
                  fullWidth
                  loading={submitting}
                  disabled={submitting || !email || !password}
                  onClick={handleLogin}
                >
                  Sign in
                </Button>

                {/* Registration link */}
                <Text ta="center" size="sm" c="dimmed">
                  Don&apos;t have an account?{' '}
                  <Anchor
                    component="button"
                    type="button"
                    underline="hover"
                    onClick={() => navigate('/register')}
                  >
                    Sign up
                  </Anchor>
                </Text>
              </Stack>
            </Paper>
          </Box>
        </Grid.Col>
      </Grid>
    </Box>
  );
}
