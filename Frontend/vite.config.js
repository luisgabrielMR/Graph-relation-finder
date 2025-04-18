import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
  plugins: [
    react({
      jsxImportSource: '@emotion/react',
      babel: {
        plugins: ['@emotion/babel-plugin'],
      },
    })
  ],
  resolve: {
    extensions: ['.js', '.jsx', '.ts', '.tsx'],
  },
  server: {
    hmr: {
      overlay: false
    },
    watch: {
      usePolling: true 
    }
  },
  esbuild: {
    logOverride: { 'this-is-undefined-in-esm': 'silent' } 
  },
  optimizeDeps: {
    exclude: ['js-big-decimal'] 
  }
});