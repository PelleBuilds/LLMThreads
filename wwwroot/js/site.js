// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function sendMessage() {
    const input = document.getElementById("userInputField");
    const chatWindow = document.getElementById("chat-window");
    const welcomeText = document.getElementById("welcome-text");
    const message = input.value;
    const params = new URLSearchParams();

    params.append("userInput", message);
    params.append("generateCode", false);

    if (!message) return;

    if (welcomeText) welcomeText.style.display = 'none';

    chatWindow.insertAdjacentHTML('beforeend', `
                <div class="d-flex justify-content-end mb-3">
                 <div class="user-message">${message}</div></div>`);
    input.value = "";

    try
    {
        const response = await fetch('/Home/UserMessage', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: params
        });

        if (response.ok) {
            const html = await response.text();
            chatWindow.insertAdjacentHTML('beforeend', html);
            chatWindow.scrollTop = chatWindow.scrollHeight;
        }
    }
    catch (err)
    {
        console.error("Fel vid anrop:", err);
    }
}
//async function loadPreviousChats() {

//    const sidebar = document.getElementById("sidebar-left")

//    try {
//        const response = await fetch('/home/GetChat', {
//            method: 'POST',
//            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
//            body: params
//        });

//        if (response.ok) {
//            const html = await response.text();
//            for (Const C in html) {
//                sidebar.innerHTML += `<div class="p-3 ms-3 text-white text-end border mt-5 mb-5 rounded-3 "><strong>You:</strong> ${C}</div>`;
//            }
//            sidebar.insertAdjacentHTML('beforeend', html);
//            /*chatWindow.scrollTop = chatWindow.scrollHeight;*/
//        }
//    }
//    catch (ex) {
//        console.error("Fel vid anrop:", ex);
//    }

//}

async function codeChatToggle() {



}


async function generateCode() {

    const input = document.getElementById("userInputField");
    const chatWindow = document.getElementById("chat-window");
    const welcomeText = document.getElementById("welcome-text");
    const message = input.value;
    const params = new URLSearchParams();

    params.append("userInput", message);
    params.append("generateCode", true);

    if (!message) return;

    if (welcomeText) welcomeText.style.display = 'none';

    chatWindow.insertAdjacentHTML('beforeend', `
                <div class="d-flex justify-content-end mb-3">
                 <div class="user-message">${message}</div></div>`);
    input.value = "";

    try
    {
        const response = await fetch('/Home/UserMessage', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: params
        });

        if (response.ok)
        {
            const data = await response.json();
            const event = new CustomEvent('update-ai-code', { detail: data.generatedCode });
            window.dispatchEvent(event);
        }
    }
    catch (err)
    {
        console.error("Fel vid anrop:", err);
    }
}

(function () {
    const appRow = document.getElementById('app-row');
    const chat = document.getElementById('chat');
    const sidebarRight = document.getElementById('sidebar-right');
    const resizerLeft = document.getElementById('resizerLeft');
    const resizerRight = document.getElementById('resizerRight');

    const LEFT_PCT = 2 / 12;
    const RIGHT_PCT = 2 / 12;
    const MIN_RIGHT_W = 240;
    const MIN_CHAT_VISIBLE = 200;

    let chatLeftRatio = LEFT_PCT;
    let rightLeftRatio = 1 - RIGHT_PCT;

    function applyChatLeft(px) {
        const leftW = appRow.clientWidth * LEFT_PCT;
        const clamped = Math.max(0, Math.min(leftW, px));
        chat.style.left = clamped + 'px';
        resizerLeft.style.left = clamped + 'px';
        chatLeftRatio = clamped / appRow.clientWidth;
    }

    function applyRightLeft(px) {
        const w = appRow.clientWidth;
        const clamped = Math.max(MIN_CHAT_VISIBLE, Math.min(w - MIN_RIGHT_W, px));
        sidebarRight.style.left = clamped + 'px';
        resizerRight.style.left = clamped + 'px';
        rightLeftRatio = clamped / w;
    }

    function init() {
        const w = appRow.clientWidth;
        applyChatLeft(w * LEFT_PCT);
        applyRightLeft(w * (1 - RIGHT_PCT));
    }

    function onResize() {
        const w = appRow.clientWidth;
        applyChatLeft(w * chatLeftRatio);
        applyRightLeft(w * rightLeftRatio);
    }

    window.addEventListener('load', init);
    window.addEventListener('resize', onResize);

    function makeDraggable(handle, onMove) {
        let dragging = false;
        handle.addEventListener('pointerdown', e => {
            dragging = true;
            handle.classList.add('dragging');
            handle.setPointerCapture(e.pointerId);
            document.body.style.userSelect = 'none';
        });
        handle.addEventListener('pointermove', e => {
            if (!dragging) return;
            onMove(e.clientX - appRow.getBoundingClientRect().left);
        });
        const stop = () => {
            dragging = false;
            handle.classList.remove('dragging');
            document.body.style.userSelect = '';
        };
        handle.addEventListener('pointerup', stop);
        handle.addEventListener('pointercancel', stop);
    }

    makeDraggable(resizerLeft, applyChatLeft);
    makeDraggable(resizerRight, applyRightLeft);
})();