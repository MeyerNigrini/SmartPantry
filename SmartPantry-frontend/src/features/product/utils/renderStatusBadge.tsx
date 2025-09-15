import { Badge } from '@mantine/core';
import { IconCheck, IconClock, IconAlertTriangle } from '@tabler/icons-react';

/**
 * Returns a styled <Badge> component based on product status.
 * Handles "Fresh", "Expiring", and "Expired" with distinct
 * colors and icons for clarity.
 */

export function renderStatusBadge(status: string) {
  switch (status.toLowerCase()) {
    case 'fresh':
      return (
        <Badge
          variant="light"
          leftSection={<IconCheck size={14} style={{ marginRight: 6 }} />}
          styles={{
            root: {
              textTransform: 'none',
              fontWeight: 500,
              color: '#008236',
              backgroundColor: '#f0fdf4',
              border: '1px solid #b9f8cf',
              borderRadius: '6px',
            },
          }}
        >
          Fresh
        </Badge>
      );
    case 'expiring':
      return (
        <Badge
          variant="light"
          leftSection={<IconClock size={14} style={{ marginRight: 6 }} />}
          styles={{
            root: {
              textTransform: 'none',
              fontWeight: 500,
              color: '#bb4d00',
              backgroundColor: '#fffceb',
              border: '1px solid #fee685',
              borderRadius: '6px',
            },
          }}
        >
          Expiring
        </Badge>
      );
    case 'expired':
      return (
        <Badge
          variant="light"
          leftSection={<IconAlertTriangle size={14} style={{ marginRight: 6 }} />}
          styles={{
            root: {
              textTransform: 'none',
              fontWeight: 500,
              color: '#c10007',
              backgroundColor: '#fef2f2',
              border: '1px solid #ffc9c9',
              borderRadius: '6px',
            },
          }}
        >
          Expired
        </Badge>
      );
    default:
      return status;
  }
}
