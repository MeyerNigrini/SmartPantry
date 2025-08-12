import { useEffect, useRef, useState } from 'react';
import { Html5Qrcode } from 'html5-qrcode';
import { Box, Button, Group, Stack } from '@mantine/core';
import { showCustomNotification } from '../../../components/CustomNotification';

type BarcodeScannerProps = {
  onScan: (decodedText: string) => void;
};

export default function BarcodeScanner({ onScan }: BarcodeScannerProps) {
  // State for managing scanner status
  const [isScanning, setIsScanning] = useState(false);

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
    setIsScanning(true);

    try {
      const devices = await Html5Qrcode.getCameras();
      if (!devices.length) {
        showCustomNotification({
          message: 'No cameras found.',
          type: 'error',
        });
        setIsScanning(false);
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
      showCustomNotification({
        message: 'Failed to start scanner.',
        type: 'error',
      });
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
      showCustomNotification({
        message: 'Failed to scan image.',
        type: 'error',
      });
    } finally {
      event.target.value = ''; // allow same file re-upload
    }
  };

  return (
    <Stack>
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
      <Box>
        <Group grow w="100%">
          {/* Toggle live camera scanning */}
          <Button
            onClick={isScanning ? stopScanner : startScanner}
            color={isScanning ? 'red' : 'blue'}
          >
            {isScanning ? 'Stop Scanner' : 'Start Scanner'}
          </Button>

          {/* Trigger image file input to scan a static image */}
          <Button onClick={() => fileInputRef.current?.click()}>Upload Image to Scan</Button>
        </Group>

        {/* Hidden file input for image upload */}
        <input
          type="file"
          accept="image/*"
          ref={fileInputRef}
          style={{ display: 'none' }}
          onChange={handleFileUpload}
        />
      </Box>
    </Stack>
  );
}
