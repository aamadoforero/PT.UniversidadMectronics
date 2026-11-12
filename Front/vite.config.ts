import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

const URL_API_GATEWAY: string = process.env.URL_API_GATEWAY ?? 'http://localhost:5080'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    strictPort: true,
    proxy: {
      '/api': { target: URL_API_GATEWAY, changeOrigin: true },
    },
  },
})
