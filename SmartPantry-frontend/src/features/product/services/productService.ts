import type { ProductAdd, ProductResponse } from '../types/productTypes';
import api from '../../../lib/api';

// POST product to backend
export async function AddFoodProductForUser(product: ProductAdd): Promise<void> {
  const dto = {
    productName: product.productName,
    quantity: product.quantity,
    brands: product.brands,
    category: product.category,
    expirationDate: product.expirationDate,
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
