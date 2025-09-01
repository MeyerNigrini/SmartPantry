import type { ProductAdd, ProductResponse } from '../types/productTypes';
import api from '../../../lib/api';

// GET product details from OpenFoodFacts
export async function fetchProductByBarcode(barcode: string): Promise<ProductAdd> {
  const res = await api.get(`/OpenFoodFacts/${barcode}`);
  const raw = res.data;

  return {
    barcode,
    productName: raw.name,
    quantity: raw.quantity,
    brands: raw.brands,
    categories: raw.categories,
  };
}

// POST product to backend
export async function AddFoodProductForUser(product: ProductAdd): Promise<void> {
  const dto = {
    barcode: product.barcode,
    productName: product.productName,
    quantity: product.quantity,
    brands: product.brands,
    categories: product.categories,
  };
  await api.post('/FoodProduct/addFoodProductForUser', dto);
}

// GET all products of a user
export const GetAllFoodProductsForUser = async (): Promise<ProductResponse[]> => {
  const res = await api.get('/FoodProduct/getAllFoodProductsForUser');
  return res.data;
};

// DELETE selected products
export async function DeleteFoodProductsForUser(productIds: string[]): Promise<void> {
  await api.delete('/FoodProduct/deleteFoodProductsForUser', {
    data: { productIds },
  });
}
