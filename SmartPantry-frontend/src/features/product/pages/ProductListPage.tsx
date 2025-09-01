import { useEffect, useState } from 'react';
import { Container, Title, Loader, Center, Stack, Button, Text, Group } from '@mantine/core';
import { GetAllFoodProductsForUser } from '../services/productService';
import ProductTable from '../components/ProductTable';
import type { ProductResponse, Recipe } from '../types/productTypes';
import { getRecipeFromGemini } from '../services/geminiService';
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';
import { DeleteFoodProductsForUser } from '../services/productService';
import ConfirmModal from '../../../components/ConfirmModal';

export default function ProductListPage() {
  const [products, setProducts] = useState<ProductResponse[]>([]);
  const [selectedIds, setSelectedIds] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);

  const [recipe, setRecipe] = useState<Recipe | null>(null);
  const [loadingRecipe, setLoadingRecipe] = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);

  useEffect(() => {
    GetAllFoodProductsForUser()
      .then((data) => {
        setProducts(data);
      })
      .catch((err) => {
        showCustomNotification({
          message: getErrorMessage(err, 'Failed to load your products.'),
          type: 'error',
        });
      })
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
      showCustomNotification({
        message: getErrorMessage(err, 'Could not generate recipe.'),
        type: 'error',
      });
    } finally {
      setLoadingRecipe(false);
    }
  };

  const handleDeleteSelected = async () => {
    if (selectedIds.length === 0) return;
    setDeleting(true);
    try {
      await DeleteFoodProductsForUser(selectedIds);
      setProducts((prev) => prev.filter((p) => !selectedIds.includes(p.id)));
      setSelectedIds([]);
      showCustomNotification({
        message: 'Selected products deleted successfully.',
        type: 'success',
      });
    } catch (err) {
      showCustomNotification({
        message: getErrorMessage(err, 'Could not delete selected products.'),
        type: 'error',
      });
    } finally {
      setDeleting(false);
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

            <Group>
              <Button
                color="red"
                disabled={selectedIds.length === 0}
                loading={deleting}
                onClick={() => setConfirmOpen(true)}
              >
                Delete Selected
              </Button>

              <Button
                disabled={selectedIds.length === 0}
                loading={loadingRecipe}
                onClick={handleGenerateRecipe}
              >
                Generate Recipe
              </Button>
            </Group>

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
      <ConfirmModal
        opened={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        title="Delete Products"
        message={`Are you sure you want to delete ${selectedIds.length} selected product(s)?`}
        confirmLabel="Yes, Delete"
        confirmColor="red"
        onConfirm={handleDeleteSelected}
        cancelLabel="Cancel"
        cancelColor="gray"
      />
    </Container>
  );
}
