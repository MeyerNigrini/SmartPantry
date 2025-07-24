import { useEffect, useState } from 'react';
import { Container, Title, Loader, Center, Stack } from '@mantine/core';
import { getAllUserProducts } from '../services/productService';
import ProductTable from '../components/ProductTable';
import type { ProductResponse } from '../types/productTypes';

export default function ProductListPage() {
  const [products, setProducts] = useState<ProductResponse[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getAllUserProducts()
      .then(setProducts)
      .catch((error) => console.error('Failed to fetch products:', error))
      .finally(() => setLoading(false));
  }, []);

  return (
    <Container size="lg" py="md">
      <Stack>
        <Title order={2}>My Pantry Products</Title>
        {loading ? (
          <Center>
            <Loader />
          </Center>
        ) : (
          <ProductTable products={products} />
        )}
      </Stack>
    </Container>
  );
}
