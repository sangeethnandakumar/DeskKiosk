import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [plugin()],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        proxy: {
            '^/weatherforecast': {
                target: 'http://localhost:5173',
                secure: false
            }
        },
        cors: {
            origin: 'http://localhost:5173', // Specify the exact origin
            credentials: true,
        },
        port: 5173,
        https: false,
    }
});
