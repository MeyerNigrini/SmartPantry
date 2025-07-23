import { useEffect, useRef, useState } from "react";
import { Html5Qrcode } from "html5-qrcode";
import { Box, Button, Stack, Text } from "@mantine/core";

type BarcodeScannerProps = {
  onScan: (decodedText: string) => void;
};

export default function BarcodeScanner({ onScan }: BarcodeScannerProps) {
  const [isScanning, setIsScanning] = useState(false);
  const [error, setError] = useState("");
  const scannerRef = useRef<Html5Qrcode | null>(null);
  const cameraIdRef = useRef<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    return () => stopScanner(); // Cleanup on unmount
  }, []);

  const startScanner = async () => {
    setError("");
    setIsScanning(true);

    try {
      const devices = await Html5Qrcode.getCameras();
      if (!devices.length) {
        setError("No cameras found.");
        return;
      }

      cameraIdRef.current = devices[0].id;

      const config = {
        fps: 10,
        qrbox: { width: 500, height: 250 },
        formatsToSupport: ["EAN_13", "UPC_A", "CODE_128", "QR_CODE"],
      };

      const scanner = new Html5Qrcode("scanner-container");
      scannerRef.current = scanner;

      await scanner.start(
        cameraIdRef.current,
        config,
        (decodedText) => {
          onScan(decodedText);
          stopScanner();
        },
        (err) => {
          console.warn("Scan attempt failed:", err);
        }
      );
    } catch (err) {
      console.error("Error starting scanner:", err);
      setError("Failed to start scanner.");
      setIsScanning(false);
    }
  };

  const stopScanner = () => {
    if (scannerRef.current) {
      scannerRef.current.stop().then(() => {
        scannerRef.current?.clear();
        scannerRef.current = null;
      });
    }
    setIsScanning(false);
  };

  const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    try {
      const scanner = new Html5Qrcode("scanner-container");
      const result = await scanner.scanFile(file, true);
      onScan(result);
    } catch (err) {
      console.error("Image scan error:", err);
      setError("Failed to scan image.");
    } finally {
      event.target.value = ""; // allow same file re-upload
    }
  };

  return (
    <Stack align="center">
      {!isScanning ? (
        <Button onClick={startScanner}>Start Scanner</Button>
      ) : (
        <Button color="red" onClick={stopScanner}>
          Stop Scanner
        </Button>
      )}

      <Button variant="light" onClick={() => fileInputRef.current?.click()}>
        Upload Image to Scan
      </Button>

      <input
        type="file"
        accept="image/*"
        ref={fileInputRef}
        style={{ display: "none" }}
        onChange={handleFileUpload}
      />

      <Box
        id="scanner-container"
        mt="sm"
        style={{
          width: 600,
          height: 400,
          border: "1px solid #ccc",
          borderRadius: "8px",
          backgroundColor: "#f8f8f8",
          overflow: "hidden",
        }}
      />

      {error && <Text color="red">{error}</Text>}
    </Stack>
  );
}
