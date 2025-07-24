import { Table } from '@mantine/core';
import type { ProductResponse } from '../types/productTypes';

type Props = {
  products: ProductResponse[];
};

export default function ProductTable({ products }: Props) {
  return (
    <Table
      striped
      highlightOnHover
      withTableBorder
      withColumnBorders
    >
      <Table.Thead>
        <Table.Tr>
          <Table.Th>Product Name</Table.Th>
          <Table.Th>Barcode</Table.Th>
          <Table.Th>Quantity</Table.Th>
          <Table.Th>Brands</Table.Th>
          <Table.Th>Categories</Table.Th>
          <Table.Th>Added</Table.Th>
        </Table.Tr>
      </Table.Thead>
      <Table.Tbody>
        {products.map((p) => (
          <Table.Tr key={p.id}>
            <Table.Td>{p.productName}</Table.Td>
            <Table.Td>{p.barcode}</Table.Td>
            <Table.Td>{p.quantity}</Table.Td>
            <Table.Td>{p.brands}</Table.Td>
            <Table.Td>{p.categories}</Table.Td>
            <Table.Td>{new Date(p.addedDate).toLocaleDateString()}</Table.Td>
          </Table.Tr>
        ))}
      </Table.Tbody>
    </Table>
  );
}
