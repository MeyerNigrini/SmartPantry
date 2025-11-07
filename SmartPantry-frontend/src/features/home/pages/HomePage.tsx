import { useNavigate } from 'react-router-dom';
import { Box, Button, Card, Center, SimpleGrid, Stack, Text, Title } from '@mantine/core';
import { Package, Scan, BookOpen } from 'lucide-react';
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
      <SimpleGrid
        cols={3}
        spacing="xl"
        w="100%"
        maw={1000}
        mt="lg"
        className={classes.responsiveGrid}
      >
        {/* Add Product */}
        <Card
          withBorder
          radius="md"
          shadow="sm"
          className={cardClasses.sharedCard}
          style={{ minWidth: 280 }}
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
              variant="outline"
              mt="sm"
              fullWidth
              className={classes.outlineButton}
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
          style={{ minWidth: 280 }}
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
              mt="sm"
              fullWidth
              className={classes.primaryButton}
              onClick={() => navigate('/products')}
            >
              Open Pantry
            </Button>
          </Stack>
        </Card>

        {/* Recipes */}
        <Card
          withBorder
          radius="md"
          shadow="sm"
          className={cardClasses.sharedCard}
          style={{ minWidth: 280 }}
          onClick={() => navigate('/recipes')}
        >
          <Card.Section>
            <Center>
              <Box className={`${classes.iconWrapper} ${classes.icon}`}>
                <BookOpen size={36} />
              </Box>
            </Center>
          </Card.Section>

          <Stack align="center" mt="md" ta="center">
            <Text fw={600}>Recipes</Text>
            <Text c="dimmed" fz="sm" maw={260}>
              Browse, edit, and manage your saved recipes
            </Text>
            <Button
              variant="outline"
              mt="sm"
              fullWidth
              className={classes.outlineButton}
              onClick={() => navigate('/recipes')}
            >
              Open Recipes
            </Button>
          </Stack>
        </Card>
      </SimpleGrid>
    </Stack>
  );
}
