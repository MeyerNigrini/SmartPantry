import { useRef, useState, useEffect } from 'react';
import Webcam from 'react-webcam';
import {
  Stack,
  Box,
  Button,
  Group,
  Text,
  Card,
  CardSection,
  Center,
  Loader,
  Divider,
  Badge,
  SimpleGrid,
} from '@mantine/core';
import cardClasses from '../../../styles/common/SharedCard.module.css';
import classes from '../styles/ProductCapture.module.css'
import { Camera, Upload, Lightbulb, CheckCircle, Package, X } from 'lucide-react';
import { extractProductFromImage } from '../services/geminiService';
import { showCustomNotification } from '../../../components/CustomNotification';
import type { ProductVisionExtract } from '../types/productTypes';

type Props = {
  onExtract: (result: ProductVisionExtract) => void;
  onSkip?: () => void;
};

export default function ProductCapture({ onExtract, onSkip }: Props) {
  // --- Refs ---
  const webcamRef = useRef<Webcam>(null);
  const fileInputRef = useRef<HTMLInputElement | null>(null);

  // --- State ---
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [isCameraOn, setIsCameraOn] = useState(false);
  const [scanResult, setScanResult] = useState<'none' | 'success' | 'error'>('none');

  // Cleanup camera when unmounting
  useEffect(() => () => stopCamera(), []);

  // --- Camera Controls ---
  const startCamera = () => setIsCameraOn(true);

  const stopCamera = () => {
    // Stop all active video tracks safely
    const stream = webcamRef.current?.video?.srcObject as MediaStream | null;
    stream?.getTracks().forEach((t) => t.stop());
    setIsCameraOn(false);
  };

  // --- Send image to backend for Gemini AI extraction ---
  const sendToBackend = async (file: File) => {
    try {
      setIsLoading(true);
      const result = await extractProductFromImage(file);
      onExtract(result);
      setScanResult('success');
      showCustomNotification({ message: 'Product identified successfully!', type: 'success' });
    } catch (err) {
      console.error(err);
      setScanResult('error');
      showCustomNotification({
        message: 'Failed to analyze image. Please try again.',
        type: 'error',
      });
      setPreviewUrl(null);
    } finally {
      setIsLoading(false);
    }
  };

  // --- Capture image from webcam ---
  const capture = async () => {
    if (!webcamRef.current) return;

    const screenshot = webcamRef.current.getScreenshot();
    if (!screenshot) return;

    // Convert base64 → blob for upload
    const res = await fetch(screenshot);
    const blob = await res.blob();
    const file = new File([blob], `capture-${Date.now()}.jpg`, { type: 'image/jpeg' });

    // Show preview immediately (UX) and stop camera stream
    setPreviewUrl(screenshot);
    stopCamera();

    // Send to AI backend asynchronously
    await sendToBackend(file);
  };

  // --- Handle manual file upload ---
  const handleUpload = async (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (!file) return;

    setPreviewUrl(URL.createObjectURL(file));
    await sendToBackend(file);

    // Reset input to allow same file reupload
    event.target.value = '';
  };


  return (
    <Stack gap="lg">
      <Card shadow="sm" radius="lg" withBorder>
        {/* --- Header Section --- */}
        <CardSection inheritPadding py="md">
          <Center mb="sm">
            <Group gap="xs">
              <Camera size={20} strokeWidth={1.8} />
              <Text fw={600}>AI Product Recognition</Text>
            </Group>
          </Center>
          <Text ta="center" c="dimmed" size="sm" mb="md">
            Take a photo or upload an image to automatically identify your product
          </Text>
        </CardSection>

        {/* --- Camera / Preview Area --- */}
        <CardSection pr="md" pl="md">
          <Box pos="relative" bg="var(--mantine-color-gray-0)" className={classes.captureArea}>
            {/* Loading Overlay */}
            {isLoading && (
              <Center pos="absolute" inset={0} className={classes.loadingOverlay}>
                <Loader color="blue" size="lg" />
              </Center>
            )}

            {/* Camera / Image / Placeholder */}
            {isCameraOn ? (
              <Box pos="relative" className={classes.webcamWrapper}>
                <Webcam
                  ref={webcamRef}
                  audio={false}
                  screenshotFormat="image/jpeg"
                  videoConstraints={{ facingMode: 'environment' }}
                  className={classes.webcam}
                  disablePictureInPicture
                  controls={false}
                />

                {/* Capture button overlay */}
                <Box onClick={capture} className={classes.shutterButton}>
                  <Box className={classes.shutterInner} />
                </Box>

                {/* Stop camera button */}
                <Button onClick={stopCamera} variant="subtle" radius="xl" className={classes.stopButton}>
                  <X size={18} color="white" />
                </Button>
              </Box>
            ) : previewUrl ? (
              // --- Preview of captured or uploaded image ---
              <img src={previewUrl} alt="Captured preview" className={classes.previewImage} />
            ) : scanResult === 'success' ? (
              // --- Success confirmation ---
              <Stack align="center" gap={4}>
                <CheckCircle size={48} color="green" />
                <Text fw={500} c="green.7">
                  Product identified successfully!
                </Text>
                <Text size="sm" c="dimmed">
                  Review the details on the next step
                </Text>
              </Stack>
            ) : (
              // --- Default placeholder ---
              <Stack align="center" gap={4}>
                <Camera size={48} color="var(--mantine-color-gray-6)" />
                <Text fw={500} c="dimmed">
                  Ready to identify your product
                </Text>
                <Text size="sm" c="dimmed">
                  Choose an option below to get started
                </Text>
              </Stack>
            )}
          </Box>
        </CardSection>

        {/* --- Action Cards --- */}
        <CardSection p="lg">
          <SimpleGrid cols={{ base: 1, md: 2 }} spacing="lg">
            {/* Capture via camera */}
            <Card
              withBorder
              radius="md"
              shadow="xs"
              className={`${cardClasses.sharedCard} ${classes.actionCard}`}
              onClick={async () => {
                if (isCameraOn) await capture();
                else startCamera();
              }}
            >
              <Stack align="center" justify="center" gap="sm" py="md">
                <Box bg="blue.0" w={64} h={64} className={classes.actionIconCircle}>
                  <Camera size={28} color="var(--mantine-color-blue-6)" />
                </Box>
                <Text fw={600}>Take Photo</Text>
                <Text size="sm" c="dimmed" ta="center">
                  Use your camera to capture the product
                </Text>
                <Badge variant="light" color="blue" radius="sm">
                  Quick & Easy
                </Badge>
              </Stack>
            </Card>

            {/* Upload via file input */}
            <Card
              withBorder
              radius="md"
              shadow="xs"
              className={`${cardClasses.sharedCard} ${classes.actionCard}`}
              onClick={() => fileInputRef.current?.click()}
            >
              <Stack align="center" justify="center" gap="sm" py="md">
                <Box bg="green.0" w={64} h={64} className={classes.actionIconCircle}>
                  <Upload size={28} color="var(--mantine-color-green-6)" />
                </Box>
                <Text fw={600}>Upload Image</Text>
                <Text size="sm" c="dimmed" ta="center">
                  Upload an existing photo from your device
                </Text>
                <Badge variant="light" color="green" radius="sm">
                  From Gallery
                </Badge>
              </Stack>
            </Card>
          </SimpleGrid>

          {/* Hidden upload input */}
          <input
            ref={fileInputRef}
            type="file"
            accept="image/*"
            capture="environment"
            className={classes.hiddenInput}
            onChange={handleUpload}
          />
        </CardSection>

        {/* --- Info Panel --- */}
        <CardSection px="lg">
          <Box p="md" className={classes.infoPanel}>
            <Box w={32} h={32} className={classes.infoIcon}>
              <Lightbulb size={16} color="var(--mantine-color-gray-9)" />
            </Box>
            <Stack gap={2} className={classes.infoContent}>
              <Text fw={600} c="var(--mantine-color-gray-8)">
                AI-Powered Recognition
              </Text>
              <Text size="sm" c="var(--mantine-color-gray-7)">
                Our AI can identify products, estimate quantities, detect brands, and even suggest
                expiration dates based on your images.
              </Text>
            </Stack>
          </Box>
        </CardSection>

        {/* --- Skip Manual Entry --- */}
        <CardSection pt="sm" pb="lg" px="lg" className={classes.skipSection}>
          <Divider my="sm" />
          <Center>
            <Stack align="center" gap="xs">
              <Text size="sm" c="gray">
                Prefer to enter details manually?
              </Text>
              <Button
                variant="outline"
                color="dark"
                leftSection={<Package size={14} />}
                onClick={() => onSkip?.()}
                className={classes.skipButton}
              >
                Skip AI Recognition
              </Button>
            </Stack>
          </Center>
        </CardSection>
      </Card>
    </Stack>
  );
}