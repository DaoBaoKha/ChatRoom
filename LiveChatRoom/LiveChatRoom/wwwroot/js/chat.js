(() => {
    const statusEl = document.getElementById('status');
    const messagesEl = document.getElementById('messages');
    const inputEl = document.getElementById('input');
    const sendBtn = document.getElementById('send');
    let socket;

    function addMessage(text, isMine) {
        const div = document.createElement('div');
        div.className = `message ${isMine ? 'mine' : 'other'}`;
        div.textContent = text;
        messagesEl.appendChild(div);
        messagesEl.scrollTop = messagesEl.scrollHeight;
    }

    function setStatus(connected) {
        statusEl.textContent = connected ? 'Connected' : 'Disconnected';
        statusEl.className = `status ${connected ? 'connected' : 'disconnected'}`;
        inputEl.disabled = !connected;
        sendBtn.disabled = !connected;
    }

    function connect() {
        socket = new WebSocket('ws://localhost:8181');
        socket.onopen = () => setStatus(true);
        socket.onmessage = e => addMessage(e.data, false);
        socket.onclose = () => {
            setStatus(false);
            setTimeout(connect, 2000);
        };
        socket.onerror = () => {
            socket.close();
        };
    }

    sendBtn.addEventListener('click', () => {
        const msg = inputEl.value.trim();
        if (!msg) return;
        socket.send(msg);
        addMessage(msg, true);
        inputEl.value = '';
    });

    inputEl.addEventListener('keypress', e => {
        if (e.key === 'Enter') sendBtn.click();
    });

    // Start
    connect();
})();
