import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig({
    plugins: [react()], // Det är här 'react' används, så importen ovan är livsviktig
    build: {
        // VIKTIGT: Kontrollera att denna mapp-sökväg stämmer med din dator
        outDir: '../LLMThreads/wwwroot/js/react-widget',
        emptyOutDir: true,
        rollupOptions: {
            output: {
                entryFileNames: 'sandpack-widget.js',
            }
        }
    }
});