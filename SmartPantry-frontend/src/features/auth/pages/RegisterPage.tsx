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
import { IconMail, IconLock, IconUser } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { useForm } from 'react-hook-form';

import { register } from '../services/authService';

// Notification helpers
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';

type RegisterFormValues = {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
};

/**
 * RegisterPage Component
 *
 * Allows a new user to sign up with their personal details and credentials.
 * On successful registration, redirects to the login page.
 */
export default function RegisterPage() {
  const navigate = useNavigate();

  const {
    register: formRegister,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<RegisterFormValues>({
    mode: 'onSubmit',
  });

  const passwordValue = watch('password');

  const onSubmit = async (data: RegisterFormValues) => {
    try {
      await register({
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        password: data.password,
      });

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
              <form onSubmit={handleSubmit(onSubmit)}>
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
                      leftSection={<IconUser size={16} />}
                      autoComplete="given-name"
                      {...formRegister('firstName', {
                        required: 'First name is required',
                        minLength: {
                          value: 2,
                          message: 'At least 2 characters',
                        },
                      })}
                      error={errors.firstName?.message as string}
                    />

                    <TextInput
                      label="Last Name"
                      placeholder="Last name"
                      leftSection={<IconUser size={16} />}
                      autoComplete="family-name"
                      {...formRegister('lastName', {
                        required: 'Last name is required',
                        minLength: {
                          value: 2,
                          message: 'At least 2 characters',
                        },
                      })}
                      error={errors.lastName?.message as string}
                    />
                  </SimpleGrid>

                  {/* Email */}
                  <TextInput
                    label="Email"
                    placeholder="Enter your email"
                    leftSection={<IconMail size={16} />}
                    autoComplete="email"
                    {...formRegister('email', {
                      required: 'Email is required',
                      pattern: {
                        value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                        message: 'Invalid email format',
                      },
                    })}
                    error={errors.email?.message as string}
                  />

                  <PasswordInput
                    label="Password"
                    placeholder="Create a password"
                    leftSection={<IconLock size={16} />}
                    autoComplete="new-password"
                    {...formRegister('password', {
                      required: 'Password is required',
                      minLength: {
                        value: 6,
                        message: 'Minimum 6 characters',
                      },
                    })}
                    error={errors.password?.message as string}
                  />

                  <PasswordInput
                    label="Confirm Password"
                    placeholder="Confirm your password"
                    leftSection={<IconLock size={16} />}
                    autoComplete="new-password"
                    {...formRegister('confirmPassword', {
                      required: 'Please confirm your password',
                      validate: (value) => value === passwordValue || 'Passwords do not match',
                    })}
                    error={errors.confirmPassword?.message as string}
                  />
                  {/* Submit */}
                  <Button fullWidth type="submit" loading={isSubmitting} disabled={isSubmitting}>
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
