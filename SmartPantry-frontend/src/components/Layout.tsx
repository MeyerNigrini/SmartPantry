// Shared layout component with Navbar and page container
import { Outlet } from 'react-router-dom';
import { Container } from '@mantine/core';
import Navbar from './Navbar';

export default function Layout() {
  return (
    <>
      {/* Persistent navigation bar displayed on all pages */}
      <Navbar />
      <Container mt="md">
        <Outlet />
      </Container>
    </>
  );
}
