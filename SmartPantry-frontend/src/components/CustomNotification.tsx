import { showNotification } from '@mantine/notifications';
import { IconCheck, IconX, IconInfoCircle, IconAlertTriangle } from '@tabler/icons-react';
import type { ReactNode } from 'react';

type CustomNotificationType = 'success' | 'error' | 'info' | 'warning';

type CustomNotificationOptions = {
  title?: string;
  message: string | ReactNode;
  type?: CustomNotificationType;
  autoClose?: number;
  withCloseButton?: boolean;
};

export const showCustomNotification = ({
  title,
  message,
  type = 'info',
  autoClose = 4000,
  withCloseButton = true,
}: CustomNotificationOptions) => {
  const { color, icon, defaultTitle } = getTypeMeta(type);

  showNotification({
    title: title ?? defaultTitle,
    message,
    color,
    icon,
    autoClose,
    withCloseButton,
  });
};

const getTypeMeta = (type: CustomNotificationType) => {
  switch (type) {
    case 'success':
      return {
        color: 'green',
        icon: <IconCheck size={18} />,
        defaultTitle: 'Success',
      };
    case 'error':
      return {
        color: 'red',
        icon: <IconX size={18} />,
        defaultTitle: 'Error',
      };
    case 'warning':
      return {
        color: 'yellow',
        icon: <IconAlertTriangle size={18} />,
        defaultTitle: 'Warning',
      };
    case 'info':
    default:
      return {
        color: 'blue',
        icon: <IconInfoCircle size={18} />,
        defaultTitle: 'Notice',
      };
  }
};
