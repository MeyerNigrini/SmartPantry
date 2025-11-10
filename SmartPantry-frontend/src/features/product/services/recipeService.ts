import api from '../../../lib/api';
import { showCustomNotification } from '../../../components/CustomNotification';
import { getErrorMessage } from '../../../utils/errorHelpers';
import type { Recipe, RecipeCreateDTO, RecipeUpdateDTO } from '../types/productTypes';

/**
 * Sends a new recipe to the backend for saving under the authenticated user.
 */
export async function AddRecipeForUser(recipe: RecipeCreateDTO): Promise<void> {
  try {
    await api.post('/Recipe/addRecipeForUser', recipe);
    showCustomNotification({
      message: 'Recipe saved successfully!',
      type: 'success',
    });
  } catch (err) {
    console.error(err);
    showCustomNotification({
      message: getErrorMessage(err, 'Failed to save recipe.'),
      type: 'error',
    });
    throw err;
  }
}

// GET all recipes of a user
export const GetAllRecipesForUser = async (): Promise<Recipe[]> => {
  const res = await api.get('/Recipe/getAllRecipesForUser');
  return res.data;
};

// DELETE a recipe by ID
export async function DeleteRecipeForUser(recipeId: string): Promise<void> {
  try {
    await api.delete(`/Recipe/deleteRecipeForUser/${recipeId}`);
    showCustomNotification({
      message: 'Recipe deleted successfully!',
      type: 'success',
    });
  } catch (err) {
    console.error(err);
    showCustomNotification({
      message: getErrorMessage(err, 'Failed to delete recipe.'),
      type: 'error',
    });
    throw err;
  }
}

/**
 * Updates a recipe belonging to the authenticated user.
 */
export async function UpdateRecipeForUser(recipeId: string, dto: RecipeUpdateDTO): Promise<Recipe> {
  try {
    const res = await api.patch(`/Recipe/updateRecipeForUser/${recipeId}`, dto);
    showCustomNotification({
      message: 'Recipe updated successfully!',
      type: 'success',
    });
    return res.data;
  } catch (err) {
    console.error(err);
    showCustomNotification({
      message: getErrorMessage(err, 'Failed to update recipe.'),
      type: 'error',
    });
    throw err;
  }
}
