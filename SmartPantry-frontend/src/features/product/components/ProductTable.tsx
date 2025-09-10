import { Table, Checkbox } from '@mantine/core';
import type { ProductResponse } from '../types/productTypes';

type Props = {
  products: ProductResponse[];
  selectedIds: string[];
  onToggleSelect: (id: string) => void;
  onToggleSelectAll: () => void;
};

export default function ProductTable({
  products,
  selectedIds,
  onToggleSelect,
  onToggleSelectAll,
}: Props) {
  const allSelected = products.length > 0 && selectedIds.length === products.length;

  return (
    <Table highlightOnHover withTableBorder striped style={{ borderRadius: 8, overflow: 'hidden' }}>
      <Table.Thead>
        <Table.Tr>
          <Table.Th>
            <Checkbox
              color="dark"
              size="xs"
              checked={allSelected}
              indeterminate={selectedIds.length > 0 && selectedIds.length < products.length}
              onChange={onToggleSelectAll}
            />
          </Table.Th>
          <Table.Th>Product Name</Table.Th>
          <Table.Th>Quantity</Table.Th>
          <Table.Th>Brand</Table.Th>
          <Table.Th>Category</Table.Th>
        </Table.Tr>
      </Table.Thead>

      <Table.Tbody>
        {products.map((p, index) => (
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
            <Table.Td>{p.categories}</Table.Td>
          </Table.Tr>
        ))}
      </Table.Tbody>
    </Table>
  );
}
