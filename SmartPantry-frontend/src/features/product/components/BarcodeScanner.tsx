import { useEffect, useRef, useState } from 'react';
import { Html5Qrcode } from 'html5-qrcode';
import { Box, Button, Stack, Text } from '@mantine/core';

type BarcodeScannerProps = {
  onScan: (decodedText: string) => void;
};

export default function BarcodeScanner({ onScan }: BarcodeScannerProps) {
  // State for managing scanner status and results
  const [isScanning, setIsScanning] = useState(false);
  const [error, setError] = useState('');

  // Refs for scanner instance and device/camera
  const scannerRef = useRef<Html5Qrcode | null>(null);
  const cameraIdRef = useRef<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  // Clean up scanner when component unmounts
  useEffect(() => {
    return () => stopScanner();
  }, []);

  // Starts the live camera scanner
  const startScanner = async () => {
    setError('');
    setIsScanning(true);

    try {
      const devices = await Html5Qrcode.getCameras();
      if (!devices.length) {
        setError('No cameras found.');
        return;
      }

      cameraIdRef.current = devices[0].id;

      const config = {
        fps: 10,
        qrbox: { width: 500, height: 250 },
        formatsToSupport: ['EAN_13', 'UPC_A', 'CODE_128', 'QR_CODE'],
      };

      const scanner = new Html5Qrcode('scanner-container');
      scannerRef.current = scanner;

      // Start the camera stream and listen for barcode detections
      await scanner.start(
        cameraIdRef.current,
        config,
        (decodedText) => {
          onScan(decodedText);
          stopScanner(); // Auto-stop on successful scan
        },
        (err: unknown) => {
          if (err instanceof Error && err.name !== 'NotFoundException') {
            console.warn('Scan error:', err);
          }
        },
      );
    } catch (err) {
      console.error('Error starting scanner:', err);
      setError('Failed to start scanner.');
      setIsScanning(false);
    }
  };

  // Stops the live scanner and clears the container
  const stopScanner = () => {
    if (scannerRef.current) {
      scannerRef.current.stop().then(() => {
        scannerRef.current?.clear();
        scannerRef.current = null;
      });
    }
    setIsScanning(false);
  };

  // Scans a barcode from an uploaded image
  const handleFileUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    try {
      const scanner = new Html5Qrcode('scanner-container');
      const result = await scanner.scanFile(file, true);
      onScan(result);
    } catch (err) {
      console.error('Image scan error:', err);
      setError('Failed to scan image.');
    } finally {
      event.target.value = ''; // allow same file re-upload
    }
  };

  return (
    <Stack>
      {/* Control Panel */}
      <Box>
        <Stack>
          {/* Toggle live camera scanning */}
          <Button
            onClick={isScanning ? stopScanner : startScanner}
            color={isScanning ? 'red' : 'blue'}
          >
            {isScanning ? 'Stop Scanner' : 'Start Scanner'}
          </Button>

          {/* Trigger image file input to scan a static image */}
          <Button onClick={() => fileInputRef.current?.click()}>
            Upload Image to Scan
          </Button>
        </Stack>

        {/* Hidden file input for image upload */}
        <input
          type="file"
          accept="image/*"
          ref={fileInputRef}
          style={{ display: 'none' }}
          onChange={handleFileUpload}
        />
      </Box>

      {/* Camera stream renders into this container */}
      <Box
        id="scanner-container"
        w={600}
        h={400}
        style={{
          border: '1px solid #ccc',
          borderRadius: '8px',
          backgroundColor: '#f8f8f8',
          overflow: 'hidden',
        }}
      />

      {/* Error message displayed if any issue occurs */}
      {error && <Text color="red">{error}</Text>}
    </Stack>
  );
}
