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
    
    chatWindow.innerHTML += `<div class="p-3 ms-3 text-white text-end border mt-5 mb-5 rounded-3 "><strong>You:</strong> ${message}</div>`;
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

    chatWindow.innerHTML += `<div class="p-3 ms-3 text-white text-end border mt-5 mb-5 rounded-3 "><strong>You:</strong> ${message}</div>`;
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