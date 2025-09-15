import { useEffect, useState } from 'react';
import { Container, Title, Loader, Center, Stack, Button, Group, Card, Text } from '@mantine/core';
import { GetAllFoodProductsForUser, DeleteFoodProductsForUser } from '../services/productService';
import { getRecipeFromGemini } from '../services/geminiService';
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';
import ConfirmModal from '../../../components/ConfirmModal';

import ProductTable from '../components/ProductTable';
import RecipeCard from '../components/RecipeCard';

import type { ProductResponse, Recipe } from '../types/productTypes';

export default function ProductListPage() {
  // --- State management ---
  const [products, setProducts] = useState<ProductResponse[]>([]); // All products fetched for the user
  const [selectedIds, setSelectedIds] = useState<string[]>([]); // Currently selected product IDs
  const [loading, setLoading] = useState(true); // Loading indicator for fetching products

  const [recipe, setRecipe] = useState<Recipe | null>(null); // Generated recipe
  const [loadingRecipe, setLoadingRecipe] = useState(false); // Loading indicator for recipe generation
  const [deleting, setDeleting] = useState(false); // Loading indicator for deletion
  const [confirmOpen, setConfirmOpen] = useState(false); // State for confirm modal

  const [visibleCount, setVisibleCount] = useState(0); // Number of filtered/visible products
  const [totalCount, setTotalCount] = useState(0); // Total number of products

  // --- Fetch all products when page loads ---
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

  // --- Toggle selection for a single product ---
  const toggleSelect = (id: string) => {
    setSelectedIds((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));
  };

  // --- Toggle select all for currently visible products ---
  const toggleSelectAll = (ids: string[]) => {
    setSelectedIds((prev) => (prev.length === ids.length ? [] : ids));
  };

  // --- Generate recipe from selected products ---
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

  // --- Delete selected products ---
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
        <Text>Manage your ingredients and discover delicious recipes</Text>
        {loading ? (
          // --- Show loading state while fetching products ---
          <Center>
            <Loader />
          </Center>
        ) : (
          <>
            {/* Card wrapper around the product table */}
            <Card shadow="sm" radius="md" withBorder>
              <ProductTable
                products={products}
                selectedIds={selectedIds}
                onToggleSelect={toggleSelect}
                onToggleSelectAll={toggleSelectAll}
                onVisibleCountChange={(visible, total) => {
                  setVisibleCount(visible);
                  setTotalCount(total);
                }}
              />

              {/* --- Actions below the table --- */}
              <Group mt="md">
                <Group>
                  <Button
                    color="red.9"
                    variant="filled"
                    disabled={selectedIds.length === 0}
                    loading={deleting}
                    onClick={() => setConfirmOpen(true)}
                  >
                    Delete Selected ({selectedIds.length})
                  </Button>

                  <Button
                    color="dark"
                    variant="filled"
                    disabled={selectedIds.length === 0}
                    loading={loadingRecipe}
                    onClick={handleGenerateRecipe}
                  >
                    Generate Recipe ({selectedIds.length} items)
                  </Button>
                </Group>

                {/* --- Product counter (filtered vs total) --- */}
                <Text size="sm" c="dimmed">
                  Showing {visibleCount} of {totalCount} products
                </Text>
              </Group>
            </Card>

            {/* --- Generated recipe output --- */}
            {recipe && <RecipeCard recipe={recipe} />}
          </>
        )}
      </Stack>

      {/* --- Confirm modal for deletion --- */}
      <ConfirmModal
        opened={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        title="Delete Products"
        message={`Are you sure you want to delete ${selectedIds.length} selected product(s)?`}
        confirmLabel="Delete"
        confirmColor="red.9"
        onConfirm={handleDeleteSelected}
        cancelLabel="Cancel"
        cancelColor="gray"
      />
    </Container>
  );
}
