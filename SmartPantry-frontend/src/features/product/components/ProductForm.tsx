import { Stack, TextInput } from '@mantine/core';
import type { ProductAdd } from '../types/productTypes';

type Props = {
  product: ProductAdd;
  onChange: (product: ProductAdd) => void;
};

export default function ProductForm({ product, onChange }: Props) {
  const handleChange = (key: keyof ProductAdd, value: string) => {
    onChange({ ...product, [key]: value });
  };

  return (
    <Stack>
      <TextInput
        label="Barcode"
        value={product.barcode}
        onChange={(e) => handleChange('barcode', e.currentTarget.value)}
      />
      <TextInput
        label="Product Name"
        value={product.productName}
        onChange={(e) => handleChange('productName', e.currentTarget.value)}
      />
      <TextInput
        label="Quantity"
        value={product.quantity}
        onChange={(e) => handleChange('quantity', e.currentTarget.value)}
      />
      <TextInput
        label="Brand"
        value={product.brands}
        onChange={(e) => handleChange('brands', e.currentTarget.value)}
      />
      <TextInput
        label="Categories"
        value={product.categories}
        onChange={(e) => handleChange('categories', e.currentTarget.value)}
      />
    </Stack>
  );
}
