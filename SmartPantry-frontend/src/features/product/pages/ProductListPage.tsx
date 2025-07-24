import { useEffect, useState } from 'react';
import { Container, Title, Loader, Center, Stack, Button, Text } from '@mantine/core';
import { getAllUserProducts } from '../services/productService';
import ProductTable from '../components/ProductTable';
import type { ProductResponse, Recipe } from '../types/productTypes';
import { getRecipeFromGemini } from '../services/geminiService';

export default function ProductListPage() {
  const [products, setProducts] = useState<ProductResponse[]>([]);
  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);

  const [recipe, setRecipe] = useState<Recipe | null>(null);
  const [loadingRecipe, setLoadingRecipe] = useState(false);

  useEffect(() => {
    getAllUserProducts()
      .then(setProducts)
      .catch((err) => console.error('Failed to load products', err))
      .finally(() => setLoading(false));
  }, []);

  const toggleSelect = (id: string) => {
    setSelectedIds((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));
  };

  const handleGenerateRecipe = async () => {
    try {
      setLoadingRecipe(true);
      const result = await getRecipeFromGemini(selectedIds);
      setRecipe(result);
    } catch (err) {
      console.error('Failed to get recipe', err);
    } finally {
      setLoadingRecipe(false);
    }
  };

  return (
    <Container size="lg" py="md">
      <Stack>
        <Title order={2}>My Pantry Products</Title>

        {loading ? (
          <Center>
            <Loader />
          </Center>
        ) : (
          <>
            <ProductTable
              products={products}
              selectedIds={selectedIds}
              onToggleSelect={toggleSelect}
            />

            <Button
              disabled={selectedIds.length === 0}
              loading={loadingRecipe}
              onClick={handleGenerateRecipe}
            >
              Generate Recipe
            </Button>

            {recipe && (
              <Stack mt="md" p="md" style={{ backgroundColor: '#f8f9fa', borderRadius: 8 }}>
                <Title order={4}>{recipe.title}</Title>
                <Text fw={500}>Ingredients:</Text>
                <ul>
                  {recipe.ingredients.map((item, i) => (
                    <li key={i}>{item}</li>
                  ))}
                </ul>
                <Text fw={500}>Instructions:</Text>
                <ol>
                  {recipe.instructions.map((step, i) => (
                    <li key={i}>{step}</li>
                  ))}
                </ol>
              </Stack>
            )}
          </>
        )}
      </Stack>
    </Container>
  );
}
