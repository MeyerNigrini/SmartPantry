// Navbar component with navigation and logout functionality
import { Button, Flex, Group, Title, type ButtonProps } from '@mantine/core';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

type NavButtonProps = ButtonProps & {
  to?: string;
};

/**
 * Reusable Navbar button.
 * - Transparent, gray text by default
 * - Navigates when `to` is set
 */
function NavButton({ to, children, ...props }: NavButtonProps) {
  const navigate = useNavigate();
  return (
    <Button
      variant="transparent"
      c="gray.6"
      fz="md"
      onClick={to ? () => navigate(to) : undefined}
      {...props}
    >
      {children}
    </Button>
  );
}

export default function Navbar() {
  const { logout } = useAuth(); // Logout method from auth context
  const navigate = useNavigate();

  // Handles logout and redirects to login page
  const handleLogout = () => {
    logout();
    navigate('/');
  };

  return (
    <Flex
      component="nav"
      px="md"
      py="sm"
      justify="space-between"
      align="center"
      bg="white"
      bd="1px solid var(--mantine-color-gray-3)"
    >
      {/* App Title / Home Navigation button */}
      <Title
        order={3}
        onClick={() => navigate('/home')}
        c="black"
        fw={600}
        style={{ cursor: 'pointer' }}
      >
        SmartPantry
      </Title>

      {/* Center Navigation Links */}
      <Group gap="xs" visibleFrom="md">
        <NavButton to="/products">My Products</NavButton>
        <NavButton to="/scan-product">Scan Product</NavButton>
      </Group>

      {/* Right side buttons */}

      <Button size="sm" color="dark" fw={500} onClick={handleLogout}>
        Logout
      </Button>
    </Flex>
  );
}
