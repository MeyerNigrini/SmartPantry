// Login form and logic
import {
  TextInput,
  PasswordInput,
  Button,
  Paper,
  Title,
  Stack,
  Notification,
} from "@mantine/core";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { login as loginApi } from "../services/authService";
import { useAuth } from "../../../context/AuthContext";

export default function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async () => {
    setError("");
    try {
      const user = await loginApi({ email, password });
      login(user);
      navigate("/home");
    } catch (err: unknown) {
      if (err && typeof err === "object" && "response" in err) {
        const errorTyped = err as {
          response?: { data?: { message?: string } };
        };
        setError(
          errorTyped.response?.data?.message || "Invalid email or password"
        );
      } else {
        setError("Login failed");
      }
    }
  };

  return (
    <Paper
      withBorder
      shadow="sm"
      p="xl"
      radius="md"
      maw={400}
      mx="auto"
      mt="xl"
    >
      <Title order={2} mb="md">
        Login
      </Title>
      <Stack>
        <TextInput
          label="Email"
          value={email}
          onChange={(e) => setEmail(e.currentTarget.value)}
        />
        <PasswordInput
          label="Password"
          value={password}
          onChange={(e) => setPassword(e.currentTarget.value)}
        />
        <Button fullWidth onClick={handleLogin}>
          Login
        </Button>
        {error && <Notification color="red">{error}</Notification>}
        {/* Register Button */}
        <Button
          variant="outline"
          fullWidth
          mt="md"
          onClick={() => navigate("/register")}
        >
          Don't have an account? Register
        </Button>
      </Stack>
    </Paper>
  );
}
