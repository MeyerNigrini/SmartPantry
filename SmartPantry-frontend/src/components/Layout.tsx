import { Outlet } from "react-router-dom";
import { Container } from "@mantine/core";
import Navbar from "./Navbar";

export default function Layout() {
  return (
    <>
      <Navbar />
      <Container mt="md">
        <Outlet />
      </Container>
    </>
  );
}
