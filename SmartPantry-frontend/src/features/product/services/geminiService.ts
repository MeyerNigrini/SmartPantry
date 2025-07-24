import api from '../../../lib/api';
import type { Recipe } from '../types/productTypes';

export const getRecipeFromGemini = async (productIds: string[]): Promise<Recipe> => {
  const res = await api.post<Recipe>('/Gemini/recipes', productIds);
  return res.data;
};
