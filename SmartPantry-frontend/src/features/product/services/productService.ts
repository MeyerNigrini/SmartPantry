import type { Product } from '../types/productTypes';
import api from '../../../lib/api';

// GET product details from OpenFoodFacts
export async function fetchProductByBarcode(barcode: string): Promise<Product> {
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
export async function saveProduct(product: Product): Promise<void> {
  const dto = {
    barcode: product.barcode,
    productName: product.productName,
    quantity: product.quantity,
    brands: product.brands,
    categories: product.categories,
  };
  await api.post('/FoodProduct', dto);
}
