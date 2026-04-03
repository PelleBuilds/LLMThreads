// I din React-mapp: src/main.jsx
import React, { useState, useEffect } from 'react';
import ReactDOM from 'react-dom/client';
import { Sandpack } from "@codesandbox/sandpack-react";
import { SandpackProvider, SandpackPreview } from "@codesandbox/sandpack-react";

const SandpackWidget = ({ initialCode }) => {
    const [code, setCode] = useState(initialCode);

    useEffect(() => {
        
        const handleUpdate = (event) => {
            if (event?.detail) setCode(event.detail);
        };
        window.addEventListener('update-ai-code', handleUpdate);
        return () => window.removeEventListener('update-ai-code', handleUpdate);
    }, []);

    return (
        <div style={{
            height: "100vh",
            width: "100%",
            display: "flex",
            flex: 1,
            border: "1px solid #444",
            borderRadius: "8px",
            overflow: "hidden",
            minHeight: 0,
            minWidth: 0

        }}>
      
            <SandpackProvider
            template="react"
            theme="dark"
            files={{
                "/App.js": code,
                }}
                style={{ width: "100%", height: "100%" }}
        >
            {/* Här ritar vi bara ut preview-delen, ingen editor! */}
                
                    <SandpackPreview style={{  flex: 1, height: "100%", width:"100%"}} />
                
        </SandpackProvider>


        </div>
       
    );
};

const rootElement = document.getElementById('sandpack-widget-root');
if (rootElement) {
    const initialCode = rootElement.dataset.initialCode || "";
    ReactDOM.createRoot(rootElement).render(<SandpackWidget initialCode={initialCode} />);
}