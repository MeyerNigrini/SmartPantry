// src/pages/home/HomePage.tsx
import { Title, Stack, Text } from "@mantine/core";
import { useAuth } from "../../../context/AuthContext";
import { useState } from "react";
import BarcodeScanner from "../../../components/BarcodeScanner";

export default function HomePage() {
  const { user } = useAuth();
    const [scannedValue, setScannedValue] = useState<string | null>(null);
   const handleScan = (value: string) => {
    console.log("📦 Scanned value:", value);
    setScannedValue(value);
  };
  return (
    <Stack align="start" p="xl">
      <Title order={2}>Welcome, {user?.firstName} 👋</Title>

      <BarcodeScanner onScan={handleScan} />

      {scannedValue && (
        <Text mt="md" fw={500}>
          ✅ Scanned Code: <code>{scannedValue}</code>
        </Text>
      )}
    </Stack>
  );
}
