import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      // Interceptar CUALQUIER petición a crudcrud.com y redirigirla
      '^https://crudcrud\\.com/api/.*': {
        target: 'https://crudcrud.com',
        changeOrigin: true,
        secure: true,
        rewrite: (path) => {
          // Extraer la parte después de crudcrud.com y mantenerla
          const match = path.match(/^https:\/\/crudcrud\.com(.*)$/);
          return match ? match[1] : path;
        }
      }
    }
  }
})
