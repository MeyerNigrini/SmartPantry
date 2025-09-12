import { Stack, TextInput } from '@mantine/core';
import type { ProductAdd } from '../types/productTypes';

type Props = {
  product: ProductAdd;
  onChange: (product: ProductAdd) => void;
};

export default function ProductForm({ product, onChange }: Props) {
  // Updates a specific field in the product object
  const handleChange = (key: keyof ProductAdd, value: string) => {
    onChange({ ...product, [key]: value });
  };

  return (
    <Stack>
      {/* Input for product barcode */}
      <TextInput
        label="Barcode"
        value={product.barcode}
        onChange={(e) => handleChange('barcode', e.currentTarget.value)}
      />
      {/* Input for product name */}
      <TextInput
        label="Product Name"
        value={product.productName}
        onChange={(e) => handleChange('productName', e.currentTarget.value)}
      />
      {/* Input for quantity (e.g. "1L", "500g") */}
      <TextInput
        label="Quantity"
        value={product.quantity}
        onChange={(e) => handleChange('quantity', e.currentTarget.value)}
      />
      {/* Input for brand name(s) */}
      <TextInput
        label="Brand"
        value={product.brands}
        onChange={(e) => handleChange('brands', e.currentTarget.value)}
      />
      {/* Input for product categories */}
      <TextInput
        label="Categories"
        value={product.categories}
        onChange={(e) => handleChange('categories', e.currentTarget.value)}
      />
      <TextInput
        label="Expiration Date"
        value={product.expirationDate}
        onChange={(e) => handleChange('expirationDate', e.currentTarget.value)}
      />
    </Stack>
  );
}
