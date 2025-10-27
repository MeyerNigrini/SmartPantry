import { useNavigate } from 'react-router-dom';
import { Box, Button, Card, Center, SimpleGrid, Stack, Text, Title } from '@mantine/core';
import { Package, Scan } from 'lucide-react';
import cardClasses from '../../../styles/common/SharedCard.module.css';
import classes from '../styles/HomePage.module.css';

export default function HomePage() {
  const navigate = useNavigate();

  return (
    <Stack align="center" gap="xl" p="md">
      {/* Header */}
      <Stack align="center" gap={4}>
        <Title order={2} fw={700}>
          Good afternoon!
        </Title>
        <Text c="dimmed" fz="md">
          Welcome to your SmartPantry
        </Text>
      </Stack>

      {/* Action Cards */}
      <SimpleGrid cols={{ base: 1, sm: 2 }} spacing="xl" maw={600} w="100%" mt="lg">
        {/* Add Product */}
        <Card
          withBorder
          radius="md"
          shadow="sm"
          className={cardClasses.sharedCard}
          onClick={() => navigate('/scan-product')}
        >
          <Card.Section>
            <Center>
              <Box className={`${classes.iconWrapper} ${classes.icon}`}>
                <Scan size={36} />
              </Box>
            </Center>
          </Card.Section>

          <Stack align="center" mt="md" ta="center">
            <Text fw={600}>Add Product</Text>
            <Text c="dimmed" fz="sm" maw={260}>
              Use AI to scan and add new products to your pantry
            </Text>
            <Button
              mt="sm"
              fullWidth
              className={classes.primaryButton}
              onClick={() => navigate('/scan-product')}
            >
              Start Scanning
            </Button>
          </Stack>
        </Card>

        {/* View Pantry */}
        <Card
          withBorder
          radius="md"
          shadow="sm"
          className={cardClasses.sharedCard}
          onClick={() => navigate('/products')}
        >
          <Card.Section>
            <Center>
              <Box className={`${classes.iconWrapper} ${classes.icon}`}>
                <Package size={36} />
              </Box>
            </Center>
          </Card.Section>

          <Stack align="center" mt="md" ta="center">
            <Text fw={600}>View Pantry</Text>
            <Text c="dimmed" fz="sm" maw={260}>
              Manage your products and generate recipes
            </Text>
            <Button
              variant="outline"
              mt="sm"
              fullWidth
              className={classes.outlineButton}
              onClick={() => navigate('/products')}
            >
              Open Pantry
            </Button>
          </Stack>
        </Card>
      </SimpleGrid>
    </Stack>
  );
}
