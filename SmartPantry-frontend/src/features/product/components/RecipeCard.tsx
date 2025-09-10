import { Card, Title, Text } from '@mantine/core';
import type { Recipe } from '../types/productTypes';

type Props = {
  recipe: Recipe;
};

export default function RecipeCard({ recipe }: Props) {
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
    </Card>
  );
}
