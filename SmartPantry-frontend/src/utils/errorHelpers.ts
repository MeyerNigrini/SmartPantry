import axios from 'axios';

/**
 * Extracts a user-facing message from an Axios error, falling back if not present.
 *
 * @param err - Any unknown error thrown during an API call
 * @param fallback - A safe fallback message to display if no message is found
 * @returns A user-friendly error message
 */
export const getErrorMessage = (err: unknown, fallback: string): string => {
  if (axios.isAxiosError(err) && typeof err.response?.data?.message === 'string') {
    return err.response.data.message;
  }

  return fallback;
};
