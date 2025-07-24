import { useState } from 'react';
import {
  Title,
  Stack,
  Text,
  Notification,
  Button,
  TextInput,
  Group,
} from '@mantine/core';
import BarcodeScanner from '../components/BarcodeScanner';
import ProductForm from '../components/ProductForm';
import { fetchProductByBarcode, saveProduct } from '../services/productService';
import type { ProductAdd } from '../types/productTypes';

export default function ScanProductPage() {
  const [barcode, setBarcode] = useState('');
  const [product, setProduct] = useState<ProductAdd>({
    barcode: '',
    productName: '',
    quantity: '',
    brands: '',
    categories: '',
  });
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

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
      setError('');
    } catch (err: unknown) {
      setNotFound(true);
      setProduct({
        barcode: code,
        productName: '',
        quantity: '',
        brands: '',
        categories: '',
      });

      if (err instanceof Error) {
        setError(err.message);
      } else {
        setError('Something went wrong');
      }
    }
  };

  const handleSave = async () => {
    try {
      await saveProduct(product);
      setSuccess('Product saved successfully');
      setError('');
    } catch (err) {
      console.error(err);
      setError('Failed to save product');
      setSuccess('');
    }
  };

  return (
    <Stack>
      <Title>Scan or Add Product</Title>

      <BarcodeScanner onScan={handleScan} />

      <Group align="end">
        <TextInput
          label="Barcode"
          placeholder="Enter or scan a barcode"
          value={barcode}
          onChange={(e) => setBarcode(e.currentTarget.value)}
        />
        <Button onClick={() => handleSearch()}>Search Product</Button>
      </Group>

      {notFound && <Text color="red">Product not found. Please enter the details manually.</Text>}

      {error && (
        <Notification color="red" withCloseButton onClose={() => setError('')}>
          {error}
        </Notification>
      )}

      {success && (
        <Notification color="green" withCloseButton onClose={() => setSuccess('')}>
          {success}
        </Notification>
      )}

      <ProductForm product={product} onChange={setProduct} />

      <Button onClick={handleSave} disabled={!product.productName}>
        Save Product
      </Button>
    </Stack>
  );
}
