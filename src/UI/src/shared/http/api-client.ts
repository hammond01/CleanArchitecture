import axios from 'axios'
import { env } from '@/shared/config/env'

export const apiClient = axios.create({
  baseURL: env.apiBaseUrl,
  timeout: 30_000,
})
