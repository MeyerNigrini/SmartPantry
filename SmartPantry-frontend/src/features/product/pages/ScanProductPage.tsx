import { useEffect, useState } from 'react';
import { Title, Stack, Text, Notification, Button } from '@mantine/core';
import BarcodeScanner from '../../../components/BarcodeScanner';
import ProductForm from '../components/ProductForm';
import { fetchProductByBarcode, saveProduct } from '../services/productService';
import type { Product } from '../types/productTypes';

export default function ScanProductPage() {
  const [scannedValue, setScannedValue] = useState<string | null>(null);
  const [product, setProduct] = useState<Product>({
    barcode: '',
    productName: '',
    quantity: '',
    brands: '',
    categories: '',
  });
  const [notFound, setNotFound] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');

  useEffect(() => {
    if (!scannedValue) return;

    const loadProduct = async () => {
      try {
        const data = await fetchProductByBarcode(scannedValue);
        setProduct(data);
        setNotFound(false);
        setError('');
      } catch (err: unknown) {
        setNotFound(true);
        setProduct({
          barcode: scannedValue,
          productName: '',
          quantity: '',
          brands: '',
          categories: '',
        });

        if (err instanceof Error) {
          setError(err.message); // message: "Product not found" or "Failed to fetch product"
        } else {
          setError('Something went wrong');
        }
      }
    };

    loadProduct();
  }, [scannedValue]);

  const handleSave = async () => {
    console.log('Payload to be sent:', product);

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

      <BarcodeScanner onScan={setScannedValue} />

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
