import { Table, Checkbox, Group, TextInput, Select, Text } from '@mantine/core';
import { useEffect, useState } from 'react';
import type { ProductResponse } from '../types/productTypes';
import { FOOD_CATEGORIES } from '../types/constants/foodCategories';
import { renderStatusBadge } from '../utils/renderStatusBadge';
import { IconFilter, IconSearch } from '@tabler/icons-react';

type Props = {
  products: ProductResponse[];
  selectedIds: string[];
  onToggleSelect: (id: string) => void;
  onToggleSelectAll: (ids: string[]) => void;
  onVisibleCountChange?: (visible: number, total: number) => void;
};

export default function ProductTable({
  products,
  selectedIds,
  onToggleSelect,
  onToggleSelectAll,
  onVisibleCountChange,
}: Props) {
  // --- Filters ---
  const [searchTerm, setSearchTerm] = useState(''); // Product name text search filter
  const [categoryFilter, setCategoryFilter] = useState<string | null>(null); // Category filter
  const [statusFilter, setStatusFilter] = useState<string | null>(null); // Status filter

  // --- Apply filters to product list ---
  const filteredProducts = products.filter((p) => {
    // Search filter (productName only, case-insensitive)
    const matchesSearch = p.productName.toLowerCase().includes(searchTerm.toLowerCase());

    // Category filter
    const matchesCategory =
      !categoryFilter || categoryFilter === 'All Categories' || p.category === categoryFilter;

    // Status filter
    const matchesStatus =
      !statusFilter ||
      statusFilter === 'All Status' ||
      p.status.toLowerCase() === statusFilter.toLowerCase();

    return matchesSearch && matchesCategory && matchesStatus;
  });

  // --- Notify parent when visible/total counts change ---
  useEffect(() => {
    if (onVisibleCountChange) {
      onVisibleCountChange(filteredProducts.length, products.length);
    }
  }, [filteredProducts.length, products.length, onVisibleCountChange]);

  // --- Check if all visible products are selected ---
  const allSelected = filteredProducts.length > 0 && selectedIds.length === filteredProducts.length;

  return (
    <div>
      {/* --- Filter controls (search, category, status) --- */}
      <Group mb="sm" justify="space-between">
        <Text>My Pantry Products</Text>
        <Group gap="md">
          <TextInput
            variant="filled"
            placeholder="Search products..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.currentTarget.value)}
            leftSection={<IconSearch size={16} />}
            w={200}
          />
          <Select
            variant="filled"
            placeholder="All Categories"
            data={['All Categories', ...FOOD_CATEGORIES]}
            value={categoryFilter}
            onChange={setCategoryFilter}
            clearable
            leftSection={<IconFilter size={16} />}
            w={180}
          />
          <Select
            variant="filled"
            placeholder="All Status"
            data={['All Status', 'Fresh', 'Expiring', 'Expired']}
            value={statusFilter}
            onChange={setStatusFilter}
            clearable
            leftSection={<IconFilter size={16} />}
            w={150}
          />
        </Group>
      </Group>

      {/* --- Product table --- */}
      <Table
        highlightOnHover
        withTableBorder
        striped
        style={{ borderRadius: 8, overflow: 'hidden' }}
      >
        <Table.Thead>
          <Table.Tr>
            <Table.Th>
              <Checkbox
                color="dark"
                size="xs"
                checked={allSelected}
                indeterminate={
                  selectedIds.length > 0 && selectedIds.length < filteredProducts.length
                }
                onChange={() => onToggleSelectAll(filteredProducts.map((p) => p.id))}
              />
            </Table.Th>
            <Table.Th>Product</Table.Th>
            <Table.Th>Quantity</Table.Th>
            <Table.Th>Brand</Table.Th>
            <Table.Th>Category</Table.Th>
            <Table.Th>Status</Table.Th>
            <Table.Th>Expires</Table.Th>
          </Table.Tr>
        </Table.Thead>

        <Table.Tbody>
          {/* --- Render filtered products --- */}
          {filteredProducts.map((p, index) => (
            <Table.Tr
              key={p.id}
              style={(theme) => ({
                backgroundColor: index % 2 === 0 ? theme.colors.gray[0] : theme.white,
                transition: 'background-color 150ms ease',
              })}
            >
              <Table.Td>
                <Checkbox
                  color="dark"
                  size="xs"
                  checked={selectedIds.includes(p.id)}
                  onChange={() => onToggleSelect(p.id)}
                />
              </Table.Td>
              <Table.Td>{p.productName}</Table.Td>
              <Table.Td>{p.quantity}</Table.Td>
              <Table.Td>{p.brands}</Table.Td>
              <Table.Td>{p.category}</Table.Td>
              <Table.Td>{renderStatusBadge(p.status)}</Table.Td>
              <Table.Td>{p.expirationDate.split('T')[0]}</Table.Td>
            </Table.Tr>
          ))}

          {/* --- Empty state message --- */}
          {filteredProducts.length === 0 && (
            <Table.Tr>
              <Table.Td colSpan={7}>
                <Text ta="center" c="dimmed" py="md">
                  No products found matching your criteria.
                </Text>
              </Table.Td>
            </Table.Tr>
          )}
        </Table.Tbody>
      </Table>
    </div>
  );
}
