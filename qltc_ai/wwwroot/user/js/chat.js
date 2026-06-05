'use strict';

let isLoading = false;

document.addEventListener('DOMContentLoaded', () => {

    const $ = id => document.getElementById(id);
    const $$ = sel => document.querySelectorAll(sel);

    const history = [];

    const TRANSACTION_VERBS = /(ăn|uống|mua|đặt|order|grab|thanh toán|trả|nạp|đổ|thuê|đóng|chuyển|nhận|lương|thưởng|freelance|bán)\b/i;
    const ADVISORY_SIGNALS = /(\?|nên|có nên|tư vấn|gợi ý|phân tích|kế hoạch|đầu tư|tiết kiệm|lãi|lãi suất|hiệu quả|so sánh|tháng này|tháng sau|bao nhiêu là|có đủ|có tốt)/i;

    function classifyIntent(text) {
        const hasAmount = /\d[\d.,]*\s*(tr|m|k|đồng|vnd|d)?\b/i.test(text);
        if (!hasAmount) return 'advice';

        if (ADVISORY_SIGNALS.test(text)) return 'advice';
        if (TRANSACTION_VERBS.test(text.trim())) return 'transaction';

        const wordCount = text.trim().split(/\s+/).length;
        if (wordCount <= 6) return 'transaction';

        return 'advice';
    }

    async function open() {
        $('cw-overlay').classList.add('open');
        $('cw-drawer').classList.add('open');
        setTimeout(() => $('cw-inp').focus(), 350);
        await loadHistory();
    }

    function close() {
        $('cw-overlay').classList.remove('open');
        $('cw-drawer').classList.remove('open');
    }

    async function loadHistory() {
        const msgs = $('cw-msgs');
        msgs.innerHTML = '';
        history.length = 0;

        try {
            const res = await fetch('/ai/history');
            if (!res.ok) return;

            const data = await res.json();
            if (!data.messages?.length) return;

            data.messages.forEach(m => {
                addBubble(m.content, m.role === 'assistant' ? 'ai' : 'user', true);
                history.push({ role: m.role, content: m.content });
            });

            msgs.scrollTop = msgs.scrollHeight;
        } catch (e) {
            console.warn('Không thể tải lịch sử chat:', e);
        }
    }

    async function sendMsg() {
        if (isLoading) return;
        const inp = $('cw-inp');
        const text = inp.value.trim();
        if (!text) return;

        inp.value = '';
        inp.style.height = 'auto';
        addBubble(text, 'user');
        history.push({ role: 'user', content: text });

        const tid = addTyping();
        isLoading = true;
        setSendLoading(true);

        try {
            const intent = classifyIntent(text);
            const reply = intent === 'transaction'
                ? await handleTransaction(text)
                : await Azusa(history);

            removeTyping(tid);
            addBubble(reply, 'ai');
            history.push({ role: 'assistant', content: reply });

        } catch (e) {
            removeTyping(tid);
            addBubble('Xin lỗi, lỗi kết nối. Vui lòng thử lại! 🔄', 'ai');
            console.error(e);
        } finally {
            isLoading = false;
            setSendLoading(false);
        }
    }

    async function handleTransaction(text) {
        const res = await fetch('/transaction/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ text }),
        });

        const data = await res.json();
        if (!res.ok) return `⚠️ ${data.message}`;

        const { message, note, money } = data;
        const fmt = money ? Number(money).toLocaleString('vi-VN') + 'đ' : '';
        return `✅ **${note || text}**${fmt ? ` — **${fmt}**` : ''}\n${message}`;
    }

    async function Azusa(messages) {
        const res = await fetch('/ai/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ messages: JSON.stringify(messages) }),
        });

        if (!res.ok) throw new Error('Azusa ' + res.status);
        const data = await res.json();
        return data.reply || '...';
    }

    function addBubble(raw, role, skipScroll = false) {
        const msgs = $('cw-msgs');
        const wrap = document.createElement('div');
        wrap.className = 'cw-bwrap' + (role === 'user' ? ' user' : '');

        const ava = document.createElement('div');
        ava.className = `cw-ava ${role}`;
        ava.textContent = role === 'ai' ? 'AI' : 'US';

        const bub = document.createElement('div');
        bub.className = `cw-bubble ${role}`;
        bub.innerHTML = raw
            .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
            .replace(/\n/g, '<br>');

        wrap.appendChild(ava);
        wrap.appendChild(bub);
        msgs.appendChild(wrap);

        if (!skipScroll) msgs.scrollTop = msgs.scrollHeight;
    }

    function addTyping() {
        const id = 'cw-ty-' + Date.now();
        const msgs = $('cw-msgs');
        const wrap = document.createElement('div');
        wrap.id = id;
        wrap.className = 'cw-bwrap';
        wrap.innerHTML = `<div class="cw-ava ai">AI</div>
                          <div class="cw-bubble ai">
                            <div class="cw-dots"><span></span><span></span><span></span></div>
                          </div>`;
        msgs.appendChild(wrap);
        msgs.scrollTop = msgs.scrollHeight;
        return id;
    }

    function removeTyping(id) { $(id)?.remove(); }

    function setSendLoading(on) {
        const btn = $('cw-send');
        if (!btn) return;
        btn.classList.toggle('loading', on);
        btn.textContent = on ? '⏳' : '↑';
    }

    function showToast(msg, dur = 2600) {
        const t = $('cw-toast');
        if (!t) return;
        t.textContent = msg;
        t.classList.add('show');
        clearTimeout(t._timer);
        t._timer = setTimeout(() => t.classList.remove('show'), dur);
    }

    $('cw-overlay').addEventListener('click', close);
    $('cw-close').addEventListener('click', close);
    $('cw-send').addEventListener('click', sendMsg);

    const inp = $('cw-inp');
    inp.addEventListener('keydown', e => {
        if (e.key === 'Enter' && !e.shiftKey) { e.preventDefault(); sendMsg(); }
    });
    inp.addEventListener('input', () => {
        inp.style.height = 'auto';
        inp.style.height = Math.min(inp.scrollHeight, 100) + 'px';
    });

    $('cw-suggs').addEventListener('click', e => {
        const btn = e.target.closest('.cw-sugg');
        if (!btn) return;
        $('cw-inp').value = btn.textContent.trim();
        sendMsg();
    });

    $$('[data-chat-trigger]').forEach(el => el.addEventListener('click', open));

    window.ChatWidget = { open, close, showToast };

    if (location.pathname.includes('ai-tips')) setTimeout(open, 500);
});