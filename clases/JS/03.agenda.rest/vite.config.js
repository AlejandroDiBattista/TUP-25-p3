import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Proxy para CrudCrud API - intercepta llamadas a /api
      '/api': {
        target: 'https://crudcrud.com',
        changeOrigin: true,
        secure: true,
      }
    }
  }
})
