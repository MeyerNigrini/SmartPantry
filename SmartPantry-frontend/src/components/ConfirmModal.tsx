import { Modal, Button, Group, Text } from '@mantine/core';

type ConfirmModalProps = {
  opened: boolean;
  onClose: () => void;

  title: string;
  message: string;

  confirmLabel: string;
  confirmColor?: string;
  onConfirm: () => void;

  cancelLabel: string;
  cancelColor?: string;
  onCancel?: () => void;
};

export default function ConfirmModal({
  opened,
  onClose,
  title,
  message,
  confirmLabel,
  confirmColor = 'green',
  onConfirm,
  cancelLabel,
  cancelColor = 'red',
  onCancel,
}: ConfirmModalProps) {
  return (
    <Modal opened={opened} onClose={onClose} title={title} centered>
      <Text mb="md">{message}</Text>

      <Group justify="flex-end">
        <Button
          variant="outline"
          color={cancelColor}
          onClick={() => {
            onCancel?.();
            onClose();
          }}
        >
          {cancelLabel}
        </Button>

        <Button
          color={confirmColor}
          onClick={() => {
            onConfirm();
            onClose();
          }}
        >
          {confirmLabel}
        </Button>
      </Group>
    </Modal>
  );
}
