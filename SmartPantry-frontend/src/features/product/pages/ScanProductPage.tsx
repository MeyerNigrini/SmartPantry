import { useState } from 'react';
import { Title, Stack, Text, Button, TextInput, Group, Flex } from '@mantine/core';
import BarcodeScanner from '../components/BarcodeScanner';
import ProductForm from '../components/ProductForm';
import { fetchProductByBarcode, AddFoodProductForUser } from '../services/productService';
import type { ProductAdd } from '../types/productTypes';

// Notification helpers
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';

export default function ScanProductPage() {
  const [barcode, setBarcode] = useState('');
  const [notFound, setNotFound] = useState(false);

  const [product, setProduct] = useState<ProductAdd>({
    barcode: '',
    productName: '',
    quantity: '',
    brands: '',
    categories: '',
  });

  const handleScan = (scanned: string) => {
    setBarcode(scanned);
    handleSearch(scanned);
  };

  const handleSearch = async (code = barcode) => {
    if (!code) return;

    try {
      const data = await fetchProductByBarcode(code);
      setProduct(data);
      setNotFound(false);
    } catch (err) {
      setNotFound(true);
      setProduct({
        barcode: code,
        productName: '',
        quantity: '',
        brands: '',
        categories: '',
      });

      showCustomNotification({
        message: getErrorMessage(err, 'Product not found or lookup failed.'),
        type: 'error',
      });
    }
  };

  const handleSave = async () => {
    try {
      await AddFoodProductForUser(product);

      showCustomNotification({
        message: 'Product saved successfully!',
        type: 'success',
      });
    } catch (err) {
      console.error(err);
      showCustomNotification({
        message: getErrorMessage(err, 'Failed to save product.'),
        type: 'error',
      });
    }
  };

  return (
    <Stack>
      <Title>Scan and Add Product</Title>
      <Flex align="start" gap="xl">
        {/* Left side: Scanner */}
        <Stack flex={1}>
          <BarcodeScanner onScan={handleScan} />
          <Group align="end" grow w="100%">
            <TextInput
              placeholder="Enter or scan a barcode"
              value={barcode}
              onChange={(e) => setBarcode(e.currentTarget.value)}
            />
            <Button onClick={() => handleSearch()}>Search Product</Button>
          </Group>
        </Stack>

        {/* Right side: Form and controls */}
        <Stack flex={2}>
          {notFound && (
            <Text color="red">Product not found. Please enter the details manually.</Text>
          )}

          <ProductForm product={product} onChange={setProduct} />

          <Button onClick={handleSave} disabled={!product.productName}>
            Save Product
          </Button>
        </Stack>
      </Flex>
    </Stack>
  );
}
