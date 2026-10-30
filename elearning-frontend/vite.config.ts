// vite.config.ts
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'http://localhost:5000',  // HTTP port
        changeOrigin: true,
        secure: false
      },
      '/notificationHub': {
        target: 'http://localhost:5000',  // HTTP port
        ws: true,  // WebSocket support
        changeOrigin: true,
        secure: false
      }
    }
  }
})