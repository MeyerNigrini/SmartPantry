import {
  Card,
  CardSection,
  Stack,
  Group,
  Text,
  TextInput,
  Select,
  Box,
  Button,
  Badge,
  Image,
  Divider,
  Grid,
} from '@mantine/core';
import classes from '../styles/ProductForm.module.css';
import { DateInput } from '@mantine/dates';
import { CheckCircle, Package, Tag, Calendar } from 'lucide-react';
import dayjs from 'dayjs';
import type { ProductAdd } from '../types/productTypes';
import { FOOD_CATEGORIES } from '../types/constants/foodCategories';

type Props = {
  product: ProductAdd;
  onChange: (product: ProductAdd) => void;
  onBack?: () => void; // optional back navigation (form → camera)
  onSave?: () => void; // save callback
  previewUrl?: string; // optional image preview
  aiDetected?: boolean; // optional AI banner flag
};

export default function ProductForm({ product, onChange, onSave, previewUrl, aiDetected }: Props) {
  /** Controlled field updates — merges key/value changes into product object */
  const handleChange = (key: keyof ProductAdd, value: string) => {
    onChange({ ...product, [key]: value });
  };

  return (
    <Card shadow="sm" radius="lg" withBorder>
      {/* --- HEADER --- */}
      <CardSection inheritPadding py="md">
        <Group justify="space-between">
          <Group gap="xs">
            <Package size={20} strokeWidth={1.8} />
            <Text fw={600}>Product Details</Text>
          </Group>

          {/* AI-detected badge indicator */}
          {aiDetected && (
            <Badge
              color="green"
              variant="light"
              radius="sm"
              leftSection={<CheckCircle size={12} />}
            >
              AI-detected
            </Badge>
          )}
        </Group>
      </CardSection>

      {/* --- AI SUCCESS BANNER --- */}
      {aiDetected && (
        <CardSection px="lg" pb="xs">
          <Box bg="green.0" p="sm" className={classes.aiBanner}>
            <Group align="flex-start" gap="xs">
              <CheckCircle size={16} color="var(--mantine-color-green-7)" />
              <Stack gap={2}>
                <Text fw={500} c="green.8">
                  AI successfully identified your product!
                </Text>
                <Text size="sm" c="green.7">
                  Review and edit the detected information below before saving.
                </Text>
              </Stack>
            </Group>
          </Box>
        </CardSection>
      )}

      {/* --- IMAGE PREVIEW --- */}
      {previewUrl && (
        <CardSection px="lg" pb="sm">
          <Stack gap={4}>
            <Text size="sm" fw={500}>
              Analyzed Image
            </Text>
            <Image
              src={previewUrl}
              alt="Analyzed product"
              w={100}
              h={100}
              radius="md"
              fit="cover"
              fallbackSrc="https://placehold.co/100x100?text=No+Image"
            />
          </Stack>
        </CardSection>
      )}

      {/* --- FORM FIELDS --- */}
      <CardSection pt="sm" px="lg" pb="md">
        <Grid gutter="lg">
          {/* Product Name — required field */}
          <Grid.Col span={{ base: 12, md: 12 }}>
            <TextInput
              label={
                <Group gap={4}>
                  <Package size={14} />
                  <Text size="sm">Product Name *</Text>
                </Group>
              }
              value={product.productName}
              onChange={(e) => handleChange('productName', e.currentTarget.value)}
              placeholder="Enter product name"
            />
          </Grid.Col>

          {/* Quantity */}
          <Grid.Col span={{ base: 12, md: 6 }}>
            <TextInput
              label={
                <Group gap={4}>
                  <Tag size={14} />
                  <Text size="sm">Quantity</Text>
                </Group>
              }
              value={product.quantity}
              onChange={(e) => handleChange('quantity', e.currentTarget.value)}
              placeholder="e.g. 2 pieces, 500g"
            />
          </Grid.Col>

          {/* Brand */}
          <Grid.Col span={{ base: 12, md: 6 }}>
            <TextInput
              label="Brand"
              value={product.brands}
              onChange={(e) => handleChange('brands', e.currentTarget.value)}
              placeholder="Enter brand name"
            />
          </Grid.Col>

          {/* Category */}
          <Grid.Col span={{ base: 12, md: 6 }}>
            <Select
              label="Category"
              data={FOOD_CATEGORIES}
              value={product.categories}
              onChange={(value) => handleChange('categories', value || '')}
              placeholder="Select category"
              searchable
            />
          </Grid.Col>

          {/* Expiration Date */}
          <Grid.Col span={{ base: 12, md: 6 }}>
            <DateInput
              label={
                <Group gap={4}>
                  <Calendar size={14} />
                  <Text size="sm">Expiration Date</Text>
                </Group>
              }
              value={product.expirationDate ? new Date(product.expirationDate) : null}
              onChange={(date) =>
                handleChange('expirationDate', date ? dayjs(date).format('YYYY-MM-DD') : '')
              }
              placeholder="Select date"
              valueFormat="YYYY-MM-DD"
            />
          </Grid.Col>
        </Grid>
      </CardSection>

      {/* --- ACTION BUTTONS --- */}
      <CardSection px="lg" pt="sm" pb="lg" className={classes.actionSection}>
        <Divider pb="lg" />

        {/* Save button — disabled if product name empty */}
        <Button
          onClick={onSave}
          color="dark"
          leftSection={<CheckCircle size={16} />}
          disabled={!product.productName}
          fullWidth
        >
          Save Product
        </Button>
      </CardSection>
    </Card>
  );
}
