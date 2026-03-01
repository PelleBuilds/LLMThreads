// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function sendMessage() {
    const input = document.getElementById("userInputField");
    const chatWindow = document.getElementById("chat-window");
    const welcomeText = document.getElementById("welcome-text");
    const message = input.value;

    if (!message) return;

    if (welcomeText) welcomeText.style.display = 'none';

    chatWindow.innerHTML += `<div class="p-3 ms-3 text-white text-end border mt-5 mb-5 rounded-3 "><strong>Du:</strong> ${escapeHtml(message)}</div>`;
    input.value = "";

    try {
        const response = await fetch('/Home/Chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `userInput=${encodeURIComponent(message)}`
        });

        if (response.ok) {
            const html = await response.text();
            chatWindow.insertAdjacentHTML('beforeend', html);

            // After inserting a new message container, ensure its thread preview is initialized
            const lastContainer = chatWindow.querySelector('.message-container:last-of-type');
            if (lastContainer) {
                const thread = lastContainer.querySelector('.thread');
                if (thread) {
                    ensureThreadStructure(thread);
                    refreshPreview(thread);
                }
            }

            chatWindow.scrollTop = chatWindow.scrollHeight;
        }
    } catch (err) {
        console.error("Fel vid anrop:", err);
    }
}

// Ensure a thread element contains .thread-full (hidden storage) and .thread-preview (visible)
function ensureThreadStructure(thread) {
    if (!thread) return;
    if (thread.querySelector('.thread-full')) return;

    const full = document.createElement('div');
    full.className = 'thread-full';
    // Move existing children into full
    while (thread.firstChild) {
        full.appendChild(thread.firstChild);
    }
    // preview container visible by default
    const preview = document.createElement('div');
    preview.className = 'thread-preview';

    // keep full hidden in main UI
    full.style.display = 'none';

    thread.appendChild(preview);
    thread.appendChild(full);
}

// Refresh the preview: show only the first message and a "+ N more" note if applicable
function refreshPreview(thread) {
    if (!thread) return;
    ensureThreadStructure(thread);
    const full = thread.querySelector('.thread-full');
    const preview = thread.querySelector('.thread-preview');
    if (!full || !preview) return;

    const children = Array.from(full.children).filter(n => n.nodeType === 1); // element nodes
    if (children.length === 0) {
        preview.innerHTML = '';
        preview.style.display = 'none';
        return;
    }

    // Show first message in preview
    preview.style.display = 'block';
    preview.innerHTML = children[0].outerHTML;

    // If more messages exist, add a clickable notice to open the overlay
    if (children.length > 1) {
        const moreCount = children.length - 1;
        const note = document.createElement('div');
        note.className = 'thread-preview-note';
        note.style.cssText = 'padding:0.5rem;color:#9aa;border-top:1px solid rgba(255,255,255,0.03);cursor:pointer;font-size:0.95rem';
        note.textContent = `+ ${moreCount} more — Click to open thread`;
        // clicking the note opens the overlay for this thread (find the container's open button)
        note.addEventListener('click', (e) => {
            const container = thread.closest('.message-container');
            if (!container) return;
            // prefer existing button that calls openThread(this)
            let btn = container.querySelector('button[onclick*="openThread"]');
            if (!btn) btn = container.querySelector('button');
            if (btn) openThread(btn);
        });

        preview.appendChild(note);
    }
}

// Append HTML into the full storage of a thread and refresh preview
function appendToFull(thread, html) {
    if (!thread) return;
    ensureThreadStructure(thread);
    const full = thread.querySelector('.thread-full');
    full.insertAdjacentHTML('beforeend', html);
    refreshPreview(thread);
}

// Open a full-screen overlay that shows ONLY the clicked thread (the full thread view)
// and allows replying directly. Replies are appended both to the original thread DOM
// and the overlay view so they stay in sync.
function openThread(button) {
    const container = button.closest('.message-container') || button.parentElement;
    let originalThread = container.querySelector('.thread');

    // Ensure the original thread exists and has the structure we expect
    if (!originalThread) {
        const t = document.createElement('div');
        t.className = 'thread';
        container.appendChild(t);
        originalThread = t;
    }
    ensureThreadStructure(originalThread);

    // Create overlay
    const overlay = document.createElement('div');
    overlay.className = 'thread-overlay';
    Object.assign(overlay.style, {
        position: 'fixed',
        inset: '0',
        background: 'rgba(0,0,0,0.92)',
        display: 'flex',
        alignItems: 'stretch',
        justifyContent: 'center',
        zIndex: '9999',
        padding: '1rem',
        overflow: 'hidden'
    });

    // Create centered box that will contain the thread view (full height, large width)
    const box = document.createElement('div');
    box.className = 'thread-overlay-box';
    Object.assign(box.style, {
        background: '#0d1117',
        color: '#fff',
        width: '100%',
        maxWidth: '1100px',
        height: '100%',
        borderRadius: '8px',
        display: 'flex',
        flexDirection: 'column',
        boxShadow: '0 6px 24px rgba(0,0,0,0.6)',
        overflow: 'hidden'
    });

    // Header with close button and optional title
    const header = document.createElement('div');
    Object.assign(header.style, {
        display: 'flex',
        justifyContent: 'space-between',
        alignItems: 'center',
        padding: '0.5rem 1rem',
        borderBottom: '1px solid rgba(255,255,255,0.06)'
    });
    const title = document.createElement('strong');
    title.textContent = 'Thread';
    const closeBtn = document.createElement('button');
    closeBtn.type = 'button';
    closeBtn.innerHTML = '&times;';
    Object.assign(closeBtn.style, {
        background: 'none',
        border: '0',
        color: '#fff',
        fontSize: '1.25rem',
        cursor: 'pointer'
    });
    header.appendChild(title);
    header.appendChild(closeBtn);

    // Content area: show the container (clone) but ensure the full messages are visible
    const content = document.createElement('div');
    content.className = 'thread-overlay-content';
    Object.assign(content.style, {
        padding: '1rem',
        overflow: 'auto',
        flex: '1 1 auto',
        background: '#071019'
    });

    // Deep clone container so overlay doesn't share interactive elements
    const containerClone = container.cloneNode(true);

    // Remove interactive buttons inside the clone so they don't open nested overlays
    containerClone.querySelectorAll && containerClone.querySelectorAll('button').forEach(b => b.remove());

    // Ensure clone has thread structure; then populate its visible area with the full messages from original
    const cloneThread = containerClone.querySelector('.thread') || document.createElement('div');
    cloneThread.className = 'thread';
    // If original has .thread-full, copy its innerHTML; otherwise copy original thread's content
    const originalFull = originalThread.querySelector('.thread-full');
    if (originalFull) {
        cloneThread.innerHTML = originalFull.innerHTML;
    } else {
        cloneThread.innerHTML = originalThread.innerHTML;
    }

    // Replace any existing thread in clone with the populated cloneThread
    const existingCloneThread = containerClone.querySelector('.thread');
    if (existingCloneThread) existingCloneThread.replaceWith(cloneThread);
    else containerClone.appendChild(cloneThread);

    content.appendChild(containerClone);

    // Footer with input and send button
    const footer = document.createElement('div');
    Object.assign(footer.style, {
        padding: '0.75rem 1rem',
        borderTop: '1px solid rgba(255,255,255,0.06)',
        display: 'flex',
        gap: '0.5rem',
        alignItems: 'center'
    });

    const input = document.createElement('input');
    input.type = 'text';
    input.placeholder = 'Write a reply...';
    input.autocomplete = 'off';
    Object.assign(input.style, {
        flex: '1',
        padding: '0.6rem 0.8rem',
        borderRadius: '6px',
        border: '1px solid rgba(255,255,255,0.06)',
        background: '#08111a',
        color: '#fff'
    });

    const sendBtn = document.createElement('button');
    sendBtn.className = 'btn btn-primary';
    sendBtn.textContent = 'Send';
    Object.assign(sendBtn.style, {
        padding: '0.55rem 0.9rem'
    });

    footer.appendChild(input);
    footer.appendChild(sendBtn);

    // assemble overlay
    box.appendChild(header);
    box.appendChild(content);
    box.appendChild(footer);
    overlay.appendChild(box);
    document.body.appendChild(overlay);

    // Keep references to original thread (for sync) and to the overlay's thread view
    const overlayThreadView = content.querySelector('.thread');
    const realThread = originalThread;

    // Focus input
    input.focus();

    // Send handler: append to both overlay view and real thread (using appendToFull), then POST to server
    const sendHandler = async () => {
        const message = input.value.trim();
        if (!message) return;

        // Append locally in overlay view
        const messageHtml = `<div class="p-3 ms-3 text-white text-end border mt-3 mb-3 rounded-3 "><strong>Du:</strong> ${escapeHtml(message)}</div>`;
        if (overlayThreadView) overlayThreadView.insertAdjacentHTML('beforeend', messageHtml);

        // Append to the real thread's full storage and refresh preview
        appendToFull(realThread, messageHtml);

        input.value = '';
        input.focus();

        // Call server for AI reply and append result into both views
        try {
            const response = await fetch('/Home/Chat', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: `userInput=${encodeURIComponent(message)}`
            });

            if (response.ok) {
                const html = await response.text();
                // append server returned HTML to overlay view
                if (overlayThreadView) overlayThreadView.insertAdjacentHTML('beforeend', html);
                // append to real thread full storage and refresh preview
                appendToFull(realThread, html);
                // scroll overlay content to bottom
                content.scrollTop = content.scrollHeight;
            }
        } catch (err) {
            console.error('Error sending nested message:', err);
        }
    };

    // Enter to send, Escape to close
    const keyHandler = (e) => {
        if (e.key === 'Enter') {
            e.preventDefault();
            sendHandler();
        } else if (e.key === 'Escape') {
            cleanup();
        }
    };

    sendBtn.addEventListener('click', sendHandler);
    input.addEventListener('keydown', keyHandler);
    closeBtn.addEventListener('click', cleanup);

    // Clicking outside the box closes the overlay (optional)
    const outsideClickHandler = (e) => {
        if (!box.contains(e.target)) cleanup();
    };
    overlay.addEventListener('mousedown', outsideClickHandler);

    function cleanup() {
        sendBtn.removeEventListener('click', sendHandler);
        input.removeEventListener('keydown', keyHandler);
        closeBtn.removeEventListener('click', cleanup);
        overlay.removeEventListener('mousedown', outsideClickHandler);
        if (overlay && overlay.parentElement) overlay.parentElement.removeChild(overlay);
        // Ensure the real thread preview is up-to-date after closing
        refreshPreview(realThread);
    }
}

// small helper to escape user-provided content when injecting into innerHTML
function escapeHtml(unsafe) {
    if (unsafe === null || unsafe === undefined) return '';
    return String(unsafe)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// Initialize previews on page load for any existing threads
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.thread').forEach(t => {
        ensureThreadStructure(t);
        refreshPreview(t);
    });
});
