import api from '../../../lib/api';
import type { Recipe, ProductVisionExtract } from '../types/productTypes';

export const extractProductFromImage = async (file: File): Promise<ProductVisionExtract> => {
  const form = new FormData();
  form.append('image', file, file.name);

  const res = await api.post<ProductVisionExtract>('/Gemini/extract-product', form, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });

  return res.data;
};

export const getRecipeFromGemini = async (productIds: string[]): Promise<Recipe> => {
  const res = await api.post<Recipe>('/Gemini/generate-recipe', productIds);
  return res.data;
};
