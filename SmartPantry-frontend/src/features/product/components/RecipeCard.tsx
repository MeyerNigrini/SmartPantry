import { Card, Title, Text, Button, Group } from '@mantine/core';
import { AddRecipeForUser } from '../services/recipeService';
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';
import type { Recipe } from '../types/productTypes';

type Props = {
  recipe: Recipe;
};

export default function RecipeCard({ recipe }: Props) {
  const handleSave = async () => {
    try {
      await AddRecipeForUser({
        title: recipe.title,
        ingredients: recipe.ingredients,
        instructions: recipe.instructions,
      });
    } catch (err) {
      console.error(err);
      showCustomNotification({
        message: getErrorMessage(err, 'Failed to save recipe.'),
        type: 'error',
      });
    }
  };

  return (
    <Card shadow="sm" radius="md" p="lg" withBorder mt="xs">
      <Title order={4} mb="md">
        {recipe.title}
      </Title>

      <div>
        <Text fw={500} mb="xs">
          Ingredients:
        </Text>
        <ul style={{ marginTop: 0 }}>
          {recipe.ingredients.map((item, i) => (
            <li key={i}>{item}</li>
          ))}
        </ul>

        <Text fw={500} mt="md" mb="xs">
          Instructions:
        </Text>
        <ol style={{ marginTop: 0 }}>
          {recipe.instructions.map((step, i) => (
            <li key={i} style={{ marginBottom: 8 }}>
              {step}
            </li>
          ))}
        </ol>
      </div>
      <Group mt="md" justify="flex-end">
        <Button onClick={handleSave} color="dark" variant="filled">
          Save Recipe
        </Button>
      </Group>
    </Card>
  );
}
