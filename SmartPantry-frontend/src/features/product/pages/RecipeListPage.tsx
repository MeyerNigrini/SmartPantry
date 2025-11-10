import { useEffect, useState } from 'react';
import {
  Container,
  Title,
  Loader,
  Center,
  Stack,
  Text,
  SimpleGrid,
  Group,
  Button,
} from '@mantine/core';
import { IconPlus } from '@tabler/icons-react';
import { useNavigate } from 'react-router-dom';
import { GetAllRecipesForUser } from '../services/recipeService';
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';
import type { Recipe } from '../types/productTypes';
import { RecipePreviewCard } from '../components/RecipePreviewCard';
import { RecipeModal } from '../components/RecipeModal';

export default function RecipeListPage() {
  const [recipes, setRecipes] = useState<Recipe[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedRecipe, setSelectedRecipe] = useState<Recipe | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchRecipes = async () => {
      try {
        const data = await GetAllRecipesForUser();
        setRecipes(data);
      } catch (err) {
        showCustomNotification({
          message: getErrorMessage(err, 'Failed to load recipes.'),
          type: 'error',
        });
      } finally {
        setLoading(false);
      }
    };

    fetchRecipes();
  }, []);

  if (loading) {
    return (
      <Center h="60vh">
        <Loader color="dark" size="lg" />
      </Center>
    );
  }

  return (
    <Container size="lg" py="xl">
      <Stack gap="md">
        <Group justify="space-between" align="center">
          <div>
            <Title order={2}>My Recipes</Title>
            <Text c="dimmed" size="sm">
              Browse and manage your recipe collection
            </Text>
          </div>

          <Button
            onClick={() => navigate('/products')}
            leftSection={<IconPlus size={16} stroke={2} />}
            bg="#0A0A14" // dark background from your screenshot
            c="white" // white text
            radius="md"
            px="md"
            py="xs"
            styles={{
              root: {
                fontWeight: 600,
                transition: 'background-color 150ms ease',
                '&:hover': {
                  backgroundColor: '#151524',
                },
              },
            }}
          >
            Add Recipe
          </Button>
        </Group>

        {recipes.length === 0 ? (
          <Center py="xl">
            <Text c="dimmed" size="sm">
              You haven’t saved any recipes yet.
            </Text>
          </Center>
        ) : (
          <SimpleGrid cols={{ base: 1, sm: 2, lg: 3 }} spacing="lg">
            {recipes.map((recipe) => (
              <RecipePreviewCard
                key={recipe.title}
                recipe={recipe}
                onClick={() => setSelectedRecipe(recipe)}
              />
            ))}
          </SimpleGrid>
        )}
      </Stack>

      {selectedRecipe && (
        <RecipeModal
          recipe={selectedRecipe}
          onClose={() => setSelectedRecipe(null)}
          onDeleted={(deletedId) => setRecipes((prev) => prev.filter((r) => r.id !== deletedId))}
          onUpdated={(updatedRecipe) =>
            setRecipes((prev) => prev.map((r) => (r.id === updatedRecipe.id ? updatedRecipe : r)))
          }
        />
      )}
    </Container>
  );
}
