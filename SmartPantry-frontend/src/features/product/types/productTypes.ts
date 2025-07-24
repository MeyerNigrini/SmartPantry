export type Product = {
  barcode: string;
  productName: string;
  quantity: string;
  brands: string;
  categories: string;
};

export type ProductResponse = {
  id: string;
  barcode: string;
  productName: string;
  quantity: string;
  brands: string;
  categories: string;
  addedDate: string;
};

export type Recipe = {
  title: string;
  ingredients: string[];
  instructions: string[];
};