import type { Product } from '../types/productTypes';
import api from '../../../lib/api';
import axios from 'axios';

export async function fetchProductByBarcode(barcode: string): Promise<Product> {
  try {
    const res = await api.get<Product>(`/OpenFoodFacts/${barcode}`);
    return res.data;
  } catch (err: unknown) {
    if (axios.isAxiosError(err)) {
      if (err.response?.status === 404) {
        throw new Error('Product not found');
      }
    }

    throw new Error('Failed to fetch product');
  }
}
