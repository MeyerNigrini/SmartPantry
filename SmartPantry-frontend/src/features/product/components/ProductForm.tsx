import { Select, Stack, TextInput } from '@mantine/core';
import { DateInput } from '@mantine/dates';
import dayjs from 'dayjs';
import type { ProductAdd } from '../types/productTypes';
import { FOOD_CATEGORIES } from '../types/constants/foodCategories';

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
      {/* Input for quantity */}
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
      <Select
        label="Category"
        data={FOOD_CATEGORIES}
        value={product.categories}
        onChange={(value) => handleChange('categories', value || '')}
        placeholder="Select category"
        searchable
      />
      {/* Date picker for expiration date */}
      <DateInput
        label="Expiration Date"
        value={product.expirationDate ? new Date(product.expirationDate) : null}
        onChange={(date) =>
          handleChange('expirationDate', date ? dayjs(date).format('YYYY-MM-DD') : '')
        }
        placeholder="Select date"
        valueFormat="YYYY-MM-DD"
      />
    </Stack>
  );
}
