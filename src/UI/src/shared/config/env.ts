const DEFAULT_API_BASE_URL = 'https://localhost:7095'

export const env = {
  apiBaseUrl: import.meta.env.VITE_API_BASE_URL ?? DEFAULT_API_BASE_URL,
}
