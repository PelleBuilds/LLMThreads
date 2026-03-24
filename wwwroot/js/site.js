// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

async function sendMessage() {
    const input = document.getElementById("userInputField");
    const chatWindow = document.getElementById("chat-window");
    const welcomeText = document.getElementById("welcome-text");
    const message = input.value;

    if (!message) return;

    // Dölj välkomsttexten vid första meddelandet
    if (welcomeText) welcomeText.style.display = 'none';

    // Visa användarens meddelande direkt (valfritt men snyggt)
    chatWindow.innerHTML += `<div class="p-3 ms-3 text-white text-end border mt-5 mb-5 rounded-3 "><strong>Du:</strong> ${message}</div>`;
    input.value = "";

    try {
        // Anropa din Controller
        const response = await fetch('/Home/Chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: `userInput=${encodeURIComponent(message)}`
        });

        if (response.ok) {
            const html = await response.text();
            chatWindow.insertAdjacentHTML('beforeend', html);
            chatWindow.scrollTop = chatWindow.scrollHeight;
        }
    } catch (err) {
        console.error("Fel vid anrop:", err);
    }
}
//async function nestedMessage() {
//    const thread = document.getElementById("thread");
//    const input = document.getElementById("threadInputField");
//    const message = input.value;

//    thread.innerHTML += `<div class="p-3 ms-3 text-white text-end border mt-5 mb-5 rounded-3 "><strong>Du:</strong> ${message}</div>`;
//    input.value = "";

//    try {
//        // Anropa din Controller
//        const response = await fetch('/Home/Chat', {
//            method: 'POST',
//            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
//            body: `userInput=${encodeURIComponent(message)}`
//        });

//        if (response.ok) {
//            const html = await response.text();
//            thread.insertAdjacentHTML('beforeend', html);
            
//        }
//    } catch (err) {
//        console.error("Fel vid anrop:", err);
//    }

//    thread.innerHTML += document.html("StartNewThread");
//}
//async function newThread() {
//    const chatWindow = document.getElementById("chat-window");
//    const thread = document.getElementById("thread");

//    // Anropa din Controller
//    const response = await fetch('/Home/NewThread', {
//        method: 'POST'
//    });
//    if (response.ok) {
//        const html = await response.text();
//        thread.insertAdjacentHTML('beforeend', html);
//    }

//    thread.innerHTML += document.html("StartNewThread");

   

//}

// Denna kod ligger i ditt MVC-projekt
async function genereraKodFrånAI() {
    const input = document.getElementById("userInputField");
    const message = input.value;
    /*const chatWindow = document.getElementById("chat-window");*/
    // 1. Anropa din C# Controller
    const response = await fetch('/Home/GenerateCode', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `userInput=${encodeURIComponent(message)}`
    });
    const data = await response.text(); // Anta att detta är ren kodsträng
    
    const event = new CustomEvent('update-ai-code', { detail: data });
    window.dispatchEvent(event);

    //chatWindow.insertAdjacentHTML('beforeend', data);
    //chatWindow.scrollTop = chatWindow.scrollHeight;
    // 2. Skicka koden till React-widgeten (Sandpack)
    // Vi använder ett "CustomEvent" för att prata över gränsen
   
    
}