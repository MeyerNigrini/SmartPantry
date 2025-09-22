// Shared layout component with Navbar and page container
import { Outlet } from 'react-router-dom';
import { Container } from '@mantine/core';
import Navbar from './Navbar';

export default function Layout() {
  return (
    <>
      {/* Persistent navigation bar displayed on all pages */}
      <Navbar />
      <Container fluid mt="md" style={{ maxWidth: '100%' }}>
        <Outlet />
      </Container>
    </>
  );
}
