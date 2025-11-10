export type ProductAdd = {
  productName: string;
  quantity: string;
  brands: string;
  category: string;
  expirationDate: string;
};

export type ProductResponse = {
  id: string;
  productName: string;
  quantity: string;
  brands: string;
  category: string;
  expirationDate: string;
  status: string;
  addedDate: string;
};

export type Recipe = {
  id: string;
  title: string;
  ingredients: string[];
  instructions: string[];
};

export type RecipeCreateDTO = {
  title: string;
  ingredients: string[];
  instructions: string[];
};

export type RecipeUpdateDTO = {
  title?: string;
  ingredients?: string[];
  instructions?: string[];
};

export type ProductVisionExtract = {
  productName: string;
  quantity?: string;
  brand?: string;
  category?: string;
  expirationDate?: string | null;
};
