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
  SimpleGrid,
  Anchor,
} from '@mantine/core';
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { IconMail, IconLock, IconUser } from '@tabler/icons-react';

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
    // --- Full-height split layout
    <Box mih="100vh">
      <Grid gutter={0}>
        {/* --- Left: hero image with overlay text (hidden on small screens) */}
        <Grid.Col span={{ base: 0, lg: 6 }}>
          <Box
            h="100vh"
            pos="relative"
            style={{
              backgroundImage:
                'url(https://images.unsplash.com/photo-1504674900247-0877df9cc836?q=80&w=1440&auto=format&fit=crop)',
              backgroundSize: 'cover',
              backgroundPosition: 'center',
            }}
          >
            {/* Gradient overlay */}
            <Box
              pos="absolute"
              inset={0}
              style={{
                background: 'linear-gradient(135deg, rgba(3,2,19,0.20), rgba(3,2,19,0.40))',
              }}
            />
            {/* Centered messaging */}
            <Box
              pos="absolute"
              inset={0}
              p="xl"
              style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}
            >
              <Box maw={520} ta="center" c="white">
                <Title order={2} fw={500} mb="sm">
                  Start Your Culinary Journey
                </Title>
                <Text size="lg" style={{ opacity: 0.9 }}>
                  Join thousands of home cooks who are revolutionizing their kitchens with smart
                  inventory management and AI-powered recipe discovery.
                </Text>
              </Box>
            </Box>
          </Box>
        </Grid.Col>

        {/* --- Right: registration card */}
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
            <Paper withBorder shadow="sm" p="xl" radius="md" maw={520} w="100%">
              {/* Form wrapper: allows Enter key to submit registration */}
              <form
                onSubmit={(e) => {
                  // Prevent full page reload, run React handler instead
                  e.preventDefault();
                  handleRegister();
                }}
              >
                <Stack gap="sm">
                  {/* Header */}
                  <Box ta="center" mb="sm">
                    <Title order={2} mb={4}>
                      Join SmartPantry
                    </Title>
                    <Text c="dimmed" size="sm">
                      Create your account to start building your smart pantry
                    </Text>
                  </Box>

                  {/* Name fields */}
                  <SimpleGrid cols={{ base: 1, sm: 2 }} spacing="md">
                    <TextInput
                      label="First Name"
                      placeholder="First name"
                      value={firstName}
                      onChange={(e) => setFirstName(e.currentTarget.value)}
                      leftSection={<IconUser size={16} />}
                      autoComplete="given-name"
                    />
                    <TextInput
                      label="Last Name"
                      placeholder="Last name"
                      value={lastName}
                      onChange={(e) => setLastName(e.currentTarget.value)}
                      leftSection={<IconUser size={16} />}
                      autoComplete="family-name"
                    />
                  </SimpleGrid>

                  {/* Email */}
                  <TextInput
                    label="Email"
                    placeholder="Enter your email"
                    value={email}
                    onChange={(e) => setEmail(e.currentTarget.value)}
                    leftSection={<IconMail size={16} />}
                    autoComplete="email"
                  />

                  {/* Passwords */}
                  <PasswordInput
                    label="Password"
                    placeholder="Create a password"
                    value={password}
                    onChange={(e) => setPassword(e.currentTarget.value)}
                    leftSection={<IconLock size={16} />}
                    autoComplete="new-password"
                  />
                  <PasswordInput
                    label="Confirm Password"
                    placeholder="Confirm your password"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.currentTarget.value)}
                    leftSection={<IconLock size={16} />}
                    autoComplete="new-password"
                  />

                  {/* Submit */}
                  <Button fullWidth type="submit" loading={submitting} disabled={submitting}>
                    Create Account
                  </Button>

                  {/* Footer */}
                  <Text ta="center" size="sm" c="dimmed">
                    Already have an account?{' '}
                    <Anchor
                      component="button"
                      type="button"
                      underline="hover"
                      onClick={() => navigate('/')}
                    >
                      Sign in
                    </Anchor>
                  </Text>
                </Stack>
              </form>
            </Paper>
          </Box>
        </Grid.Col>
      </Grid>
    </Box>
  );
}
