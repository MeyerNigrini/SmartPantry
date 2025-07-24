import { Table, Checkbox } from '@mantine/core';
import type { ProductResponse } from '../types/productTypes';

type Props = {
  products: ProductResponse[];
  selectedIds: string[];
  onToggleSelect: (id: string) => void;
};

export default function ProductTable({ products, selectedIds, onToggleSelect }: Props) {
  return (
    <Table striped highlightOnHover withTableBorder withColumnBorders>
      <Table.Thead>
        <Table.Tr>
          <Table.Th>Select</Table.Th>
          <Table.Th>Product Name</Table.Th>
          <Table.Th>Barcode</Table.Th>
          <Table.Th>Quantity</Table.Th>
          <Table.Th>Brands</Table.Th>
          <Table.Th>Categories</Table.Th>
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>
        {products.map((p) => (
          <Table.Tr key={p.id}>
            <Table.Td>
              <Checkbox
                checked={selectedIds.includes(p.id)}
                onChange={() => onToggleSelect(p.id)}
              />
            </Table.Td>
            <Table.Td>{p.productName}</Table.Td>
            <Table.Td>{p.barcode}</Table.Td>
            <Table.Td>{p.quantity}</Table.Td>
            <Table.Td>{p.brands}</Table.Td>
            <Table.Td>{p.categories}</Table.Td>
          </Table.Tr>
        ))}
      </Table.Tbody>
    </Table>
  );
}
