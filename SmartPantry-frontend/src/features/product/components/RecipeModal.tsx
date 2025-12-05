import { useState } from 'react';
import {
  Modal,
  Title,
  Text,
  Stack,
  Group,
  Textarea,
  TextInput,
  Button,
  Box,
  ScrollArea,
  Divider,
} from '@mantine/core';
import { IconPencil, IconTrash } from '@tabler/icons-react';
import type { Recipe } from '../types/productTypes';
import ConfirmModal from '../../../components/ConfirmModal';
import { DeleteRecipeForUser, UpdateRecipeForUser } from '../services/recipeService';

type Props = {
  recipe: Recipe;
  onClose: () => void;
  onDeleted: (recipeId: string) => void;
  onUpdated?: (updatedRecipe: Recipe) => void;
};

export function RecipeModal({ recipe, onClose, onDeleted, onUpdated }: Props) {
  const [isEdit, setIsEdit] = useState(false);
  const [confirmOpen, setConfirmOpen] = useState(false);

  const [formData, setFormData] = useState({
    title: recipe.title,
    ingredients: Array.isArray(recipe.ingredients)
      ? recipe.ingredients.join('\n')
      : String(recipe.ingredients),
    instructions: Array.isArray(recipe.instructions)
      ? recipe.instructions.join('\n')
      : String(recipe.instructions),
  });

  const handleUpdate = async () => {
    try {
      const dto = {
        title: formData.title.trim(),
        ingredients: formData.ingredients
          .split('\n')
          .map((i) => i.trim())
          .filter(Boolean),
        instructions: formData.instructions
          .split('\n')
          .map((s) => s.trim())
          .filter(Boolean),
      };

      const updated = await UpdateRecipeForUser(recipe.id, dto);
      onUpdated?.(updated);
      setIsEdit(false);
      onClose();
    } catch (error) {
      console.error('Failed to update recipe:', error);
    }
  };

  const handleDelete = async () => {
    try {
      await DeleteRecipeForUser(recipe.id);
      onDeleted(recipe.id);
      onClose();
    } catch (error) {
      console.error('Failed to delete recipe:', error);
    }
  };

  // normalize for rendering
  const ingredientsArray = Array.isArray(recipe.ingredients)
    ? recipe.ingredients
    : String(recipe.ingredients)
        .split(/[,|\n]/)
        .map((i) => i.trim())
        .filter(Boolean);

  const instructionsArray = Array.isArray(recipe.instructions)
    ? recipe.instructions
    : String(recipe.instructions)
        .split(/[,|\n]/)
        .map((s) => s.trim())
        .filter(Boolean);

  return (
    <>
      <Modal
        opened
        onClose={onClose}
        centered
        size="lg"
        radius="md"
        overlayProps={{ opacity: 0.55, blur: 3 }}
        title={<Title order={3}>{isEdit ? 'Edit Recipe' : recipe.title}</Title>}
      >
        <ScrollArea.Autosize mah="70vh">
          <Stack gap="md" pt="sm">
            {isEdit ? (
              <>
                <TextInput
                  label="Recipe name"
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.currentTarget.value })}
                />
                <Textarea
                  label="Ingredients"
                  minRows={6}
                  autosize
                  value={formData.ingredients}
                  onChange={(e) => setFormData({ ...formData, ingredients: e.currentTarget.value })}
                />
                <Textarea
                  label="Instructions"
                  minRows={8}
                  autosize
                  value={formData.instructions}
                  onChange={(e) =>
                    setFormData({ ...formData, instructions: e.currentTarget.value })
                  }
                />
              </>
            ) : (
              <>
                <Box>
                  <Text fw={600} mb={4}>
                    Ingredients
                  </Text>
                  <Box p="md" bg="gray.0" style={{ borderRadius: 8 }}>
                    <ul style={{ margin: 0, paddingLeft: '1.2rem' }}>
                      {ingredientsArray.map((i, idx) => (
                        <li key={idx}>
                          <Text size="sm">{i}</Text>
                        </li>
                      ))}
                    </ul>
                  </Box>
                </Box>

                <Box>
                  <Text fw={600} mb={4}>
                    Instructions
                  </Text>
                  <Box p="md" bg="gray.0" style={{ borderRadius: 8 }}>
                    <ol style={{ margin: 0, paddingLeft: '1.2rem' }}>
                      {instructionsArray.map((step, idx) => (
                        <li key={idx}>
                          <Text size="sm">{step}</Text>
                        </li>
                      ))}
                    </ol>
                  </Box>
                </Box>
              </>
            )}
          </Stack>
        </ScrollArea.Autosize>

        <Divider my="md" />

        <Group justify="flex-end" mt="sm">
          {isEdit ? (
            <>
              <Button variant="default" onClick={() => setIsEdit(false)}>
                Cancel
              </Button>
              <Button onClick={handleUpdate}>Save changes</Button>
            </>
          ) : (
            <>
              <Button
                variant="default"
                leftSection={<IconTrash size={16} />}
                onClick={() => setConfirmOpen(true)}
                color="red"
              >
                Delete
              </Button>
              <Button
                leftSection={<IconPencil size={16} />}
                onClick={() => setIsEdit(true)}
                color="dark"
                variant="filled"
              >
                Edit
              </Button>
            </>
          )}
        </Group>
      </Modal>

      <ConfirmModal
        opened={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        title="Delete Recipe"
        message="Are you sure you want to delete this recipe? This action cannot be undone."
        confirmLabel="Delete"
        confirmColor="red.9"
        onConfirm={handleDelete}
        cancelLabel="Cancel"
        cancelColor="gray"
      />
    </>
  );
}
