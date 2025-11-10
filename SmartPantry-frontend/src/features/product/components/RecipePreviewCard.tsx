import { Card, Stack, Text } from '@mantine/core';
import type { Recipe } from '../types/productTypes';

type Props = {
  recipe: Recipe;
  onClick?: () => void;
};

export function RecipePreviewCard({ recipe, onClick }: Props) {
  const preview =
    Array.isArray(recipe.ingredients)
      ? recipe.ingredients.slice(0, 5).join(', ')
      : String(recipe.ingredients).split('\n').slice(0, 3).join(', ');

 return (
    <Card
      withBorder
      radius="md"
      p="lg"
      onClick={onClick}
      style={{
        cursor: 'pointer',
        transition: 'box-shadow 150ms ease, transform 150ms ease',
        minHeight: 130,
        display: 'flex',
        alignItems: 'center',
      }}
      shadow="xs"
      onMouseEnter={(e) => {
        e.currentTarget.style.boxShadow = 'var(--mantine-shadow-md)';
        e.currentTarget.style.transform = 'translateY(-2px)';
      }}
      onMouseLeave={(e) => {
        e.currentTarget.style.boxShadow = 'var(--mantine-shadow-xs)';
        e.currentTarget.style.transform = 'translateY(0)';
      }}
    >
      <Stack gap={6} style={{ width: '100%' }}>
        <Text fw={600} lineClamp={1}>
          {recipe.title}
        </Text>
        <Text c="dimmed" size="sm" lineClamp={2}>
          {preview}
        </Text>
      </Stack>
    </Card>
  );
}