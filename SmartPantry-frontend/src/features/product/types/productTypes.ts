export type ProductAdd = {
  barcode: string;
  productName: string;
  quantity: string;
  brands: string;
  categories: string;
  expirationDate: string;
};

export type ProductResponse = {
  id: string;
  productName: string;
  quantity: string;
  brands: string;
  categories: string;
  expirationDate: string;
  status: string;
  addedDate: string;
};

export type Recipe = {
  title: string;
  ingredients: string[];
  instructions: string[];
};

export type ProductVisionExtract = {
  productName: string;
  quantity?: string;
  brand?: string;
  categories?: string[];
};
