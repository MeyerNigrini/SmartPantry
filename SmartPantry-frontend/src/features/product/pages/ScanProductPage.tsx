import { useState } from 'react';
import { Container, Title, Text, Stack, Group, Button, Stepper, Center, Box } from '@mantine/core';
import { ArrowLeft } from 'lucide-react';
import ProductCapture from '../components/ProductCapture';
import ProductForm from '../components/ProductForm';
import { AddFoodProductForUser } from '../services/productService';
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';
import type { ProductAdd, ProductVisionExtract } from '../types/productTypes';

export default function ScanProductPage() {
  const EMPTY_PRODUCT: ProductAdd = {
    barcode: '',
    productName: '',
    quantity: '',
    brands: '',
    categories: '',
    expirationDate: '',
  };

  const [product, setProduct] = useState<ProductAdd>(EMPTY_PRODUCT);
  const [activeStep, setActiveStep] = useState(0);

  const applyExtract = (extracted: ProductVisionExtract) => {
    setProduct((prev) => ({
      ...prev,
      productName: extracted.productName || prev.productName,
      quantity: extracted.quantity || prev.quantity,
      brands: extracted.brand || prev.brands,
      categories: extracted.category || prev.categories,
      expirationDate: extracted.expirationDate || prev.expirationDate,
    }));
    setActiveStep(1);
  };

  const handleSave = async () => {
    try {
      await AddFoodProductForUser(product);
      showCustomNotification({ message: 'Product saved successfully!', type: 'success' });

      // Reset product and return to camera
      setProduct(EMPTY_PRODUCT);
      setActiveStep(0);
    } catch (err) {
      console.error(err);
      showCustomNotification({
        message: getErrorMessage(err, 'Failed to save product.'),
        type: 'error',
      });
    }
  };

  return (
    <Container size="lg" py="xl">
      {/* Header */}
      <Group mb="xl" justify="space-between" align="center" wrap="nowrap">
        {activeStep === 1 ? (
          <Button
            variant="subtle"
            leftSection={<ArrowLeft size={20} strokeWidth={2.2} />}
            onClick={() => {
              setProduct(EMPTY_PRODUCT); // clear form
              setActiveStep(0); // go back to camera
            }}
            color="dark"
          >
            <Text visibleFrom="xs" fw={600} fz="md">
              Back
            </Text>
          </Button>
        ) : (
          <Box w={100} /> // ✅ keeps alignment consistent when button hidden
        )}
        <Stack gap={0}>
          <Title order={2}>Scan and Add Product</Title>
          <Text c="dimmed" size="sm">
            {activeStep === 0
              ? 'Use AI to identify your product from an image'
              : 'Review and edit product details'}
          </Text>
        </Stack>
        <Box w={40} /> {/* Spacer for alignment */}
      </Group>

      {/* Step indicator */}
      <Stepper active={activeStep} onStepClick={setActiveStep} size="sm" mb="xl" color="dark">
        <Stepper.Step label={<Text visibleFrom="xs">AI Recognition</Text>} />
        <Stepper.Step label={<Text visibleFrom="xs">Product Details</Text>} />
      </Stepper>

      {/* Step content */}
      {activeStep === 0 && (
        <Center>
          <Box w="100%" maw={700}>
            <ProductCapture onExtract={applyExtract} onSkip={() => setActiveStep(1)} />
          </Box>
        </Center>
      )}

      {activeStep === 1 && (
        <Center>
          <Box w="100%" maw={600}>
            <ProductForm product={product} onChange={setProduct} onSave={handleSave} />
          </Box>
        </Center>
      )}
    </Container>
  );
}
