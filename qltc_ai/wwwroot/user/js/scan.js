'use strict';

const getEl = id => document.getElementById(id);

let currentStab = 'upload';
let capturedFile = null;
let ngrokQrInterval = null;
let stopPolling = null;
let countdownTimer = null;
let currentToken = null;

document.addEventListener('DOMContentLoaded', () => {

    getEl('btnOpenScan').addEventListener('click', openScanModal);
    getEl('btnCloseScan').addEventListener('click', closeScanModal);
    getEl('scanOverlay').addEventListener('click', e => {
        if (e.target === getEl('scanOverlay')) closeScanModal();
    });

    document.querySelectorAll('.scan-tab').forEach(btn => {
        btn.addEventListener('click', () => switchStab(btn.dataset.stab));
    });

    const fileInput = getEl('fileInput');
    const dropZone = getEl('dropZone');

    dropZone.addEventListener('click', () => fileInput.click());
    dropZone.addEventListener('dragover', e => { e.preventDefault(); dropZone.classList.add('drag-over'); });
    dropZone.addEventListener('dragleave', () => dropZone.classList.remove('drag-over'));
    dropZone.addEventListener('drop', e => {
        e.preventDefault();
        dropZone.classList.remove('drag-over');
        const file = e.dataTransfer.files[0];
        if (file && file.type.startsWith('image/')) handleUploadFile(file);
    });
    fileInput.addEventListener('change', () => {
        if (fileInput.files[0]) handleUploadFile(fileInput.files[0]);
    });

    getEl('btnRescan').addEventListener('click', () => {
        hideResult();
        capturedFile = null;
        getEl('uploadThumb').style.display = 'none';
        getEl('uploadThumbImg').src = '';
        if (fileInput) fileInput.value = '';
    });

    getEl('btnFillForm').addEventListener('click', () => submitScannedTransaction());

    const btnSaveNgrok = getEl('btnSaveNgrok');
    if (btnSaveNgrok) {
        btnSaveNgrok.addEventListener('click', async () => {
            const url = getEl('ngrokUrlInput').value.trim();
            if (!url) { showScanToast('Nhập URL ngrok', 'warn'); return; }
            try {
                const res = await fetch('/ai/ngrok-url', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    body: new URLSearchParams({ url })
                });
                const data = await res.json();
                if (!res.ok) throw new Error(data.message);
                renderNgrokQr(data.url, data.token);
                showScanToast('Đã lưu ngrok URL ✅');
            } catch (err) {
                showScanToast('Lỗi: ' + err.message, 'warn');
            }
        });
    }

    window.ScanWidget = { open: openScanModal, close: closeScanModal };
});


async function submitScannedTransaction() {
    const note = getEl('rNote').value.trim();
    const money = getEl('rMoney').value.replace(/[^\d]/g, '');
    if (!note) { showScanToast('Vui lòng có ghi chú', 'warn'); return; }

    const text = /\d/.test(note) ? note : `${note} ${money} đồng`;
    showScanLoading(true, 'Đang thêm giao dịch...');
    try {
        const res = await fetch('/transaction/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams({ text })
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Lỗi');
        closeScanModal();
        showScanToast('✅ Đã thêm giao dịch!');
        setTimeout(async () => {
            if (window.reloadTransactions) await window.reloadTransactions();
            else location.reload();
        }, 800);
    } catch (err) {
        showScanToast('Lỗi: ' + err.message, 'warn');
    } finally {
        showScanLoading(false);
    }
}

function openScanModal() {
    getEl('scanOverlay').classList.add('open');
    document.body.style.overflow = 'hidden';
    if (ngrokQrInterval) { clearInterval(ngrokQrInterval); ngrokQrInterval = null; }
    if (stopPolling) { stopPolling(); stopPolling = null; }
    if (countdownTimer) { clearInterval(countdownTimer); countdownTimer = null; }
    capturedFile = null;
    getEl('uploadThumb').style.display = 'none';
    getEl('uploadThumbImg').src = '';
    const fi = getEl('fileInput');
    if (fi) fi.value = '';
    switchStab('ngrok');
    loadAndRenderNgrokQr();
}

function closeScanModal() {
    getEl('scanOverlay').classList.remove('open');
    document.body.style.overflow = '';
    if (ngrokQrInterval) { clearInterval(ngrokQrInterval); ngrokQrInterval = null; }
    if (stopPolling) { stopPolling(); stopPolling = null; }
    if (countdownTimer) { clearInterval(countdownTimer); countdownTimer = null; }
    currentToken = null;
    hideResult();
}

function switchStab(stab) {
    currentStab = stab;
    document.querySelectorAll('.scan-tab').forEach(b =>
        b.classList.toggle('active', b.dataset.stab === stab)
    );
    document.querySelectorAll('.scan-panel').forEach(p => p.classList.remove('active'));
    const panelId = 'stab' + stab.charAt(0).toUpperCase() + stab.slice(1);
    const panel = getEl(panelId);
    if (panel) panel.classList.add('active');
    hideResult();
}

function handleUploadFile(file) {
    capturedFile = file;
    const reader = new FileReader();
    reader.onload = e => {
        getEl('uploadThumbImg').src = e.target.result;
        getEl('uploadThumb').style.display = 'block';
    };
    reader.readAsDataURL(file);
    sendImageToServer(file);
}

async function sendImageToServer(file) {
    showScanLoading(true, 'Đang phân tích ảnh bằng AI...');
    hideResult();
    try {
        const fd = new FormData();
        fd.append('image', file);
        const res = await fetch('/ai/scan-image', { method: 'POST', body: fd });
        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Lỗi');
        showResult(data.note, data.money);
    } catch (err) {
        showScanToast('Lỗi nhận diện ảnh: ' + err.message, 'warn');
    } finally {
        showScanLoading(false);
    }
}

async function loadAndRenderNgrokQr() {
    try {
        const res = await fetch('/ai/ngrok-url');
        const data = await res.json();
        console.log('[ngrok] response:', data);
        if (data.url) renderNgrokQr(data.url, data.token);
    } catch (e) {
        console.error('[ngrok] fetch error:', e);
    }
}

function renderNgrokQr(baseUrl, token) {
    const tokenPart = token ? `&token=${encodeURIComponent(token)}` : '';
    const mobileUrl = baseUrl.replace(/\/$/, '') + '/ai/scan?mode=mobile' + tokenPart;

    console.log('[ngrok] token:', token);
    console.log('[ngrok] mobileUrl:', mobileUrl);

    const urlEl = getEl('ngrokUrlDisplay');
    if (urlEl) urlEl.textContent = mobileUrl;

    startCountdown();

    if (!stopPolling && token) stopPolling = startScanPolling(token);

    const qrDiv = getEl('ngrokQrCode');
    if (!qrDiv) return;
    qrDiv.innerHTML = '';

    const img = document.createElement('img');
    img.src = `https://api.qrserver.com/v1/create-qr-code/?size=190x190&data=${encodeURIComponent(mobileUrl)}&bgcolor=ffffff&_=${Date.now()}`;
    img.alt = 'QR Scan Bill';
    img.width = 190;
    img.height = 190;
    img.style.cssText = 'border-radius:10px;display:block';
    img.onerror = () => {
        qrDiv.innerHTML = `<p style="font-size:12px;color:#64748b;text-align:center">Không tải được QR.<br>Mở URL trên trực tiếp.</p>`;
    };
    qrDiv.appendChild(img);
}

function startScanPolling(token) {
    let stopped = false;

    async function poll() {
        if (stopped) return;
        try {
            const res = await fetch('/ai/scan-status?token=' + encodeURIComponent(token));
            if (res.ok) {
                const data = await res.json();
                if (data.done) {
                    stopped = true;
                    showScanToast('✅ Giao dịch đã được thêm từ điện thoại!');
                    closeScanModal();
                    setTimeout(async () => {
                        if (window.reloadTransactions) await window.reloadTransactions();
                        else location.reload();
                    }, 1200);
                    return;
                }
            }
        } catch (e) { console.warn('poll error:', e); }
        setTimeout(poll, 2000);
    }

    poll();
    return () => { stopped = true; };
}

function startCountdown() {
    const el = getEl('ngrokCountdown');
    if (!el) return;
    let sec = 60;
    if (countdownTimer) { clearInterval(countdownTimer); countdownTimer = null; }
    el.textContent = `Tự đổi sau ${sec}s`;
    countdownTimer = setInterval(async () => {
        sec--;
        if (sec <= 0) {
            clearInterval(countdownTimer);
            countdownTimer = null;
            el.textContent = 'Đang làm mới...';
            if (stopPolling) { stopPolling(); stopPolling = null; }
            await loadAndRenderNgrokQr();
        } else {
            el.textContent = `Tự đổi sau ${sec}s`;
        }
    }, 1000);
}

function showResult(note, money) {
    getEl('rNote').value = note || '';
    getEl('scanResultArea').style.display = 'block';
}

function hideResult() {
    getEl('scanResultArea').style.display = 'none';
    getEl('rNote').value = '';
    getEl('rMoney').value = '';
}

function showScanLoading(on, txt) {
    const el = getEl('scanLoading');
    if (!el) return;
    el.style.display = on ? 'flex' : 'none';
    if (txt) { const t = el.querySelector('.scan-loading-txt'); if (t) t.textContent = txt; }
}

function showScanToast(msg, type) {
    if (window.showToast) { window.showToast(msg, type); return; }
    const t = document.createElement('div');
    t.textContent = msg;
    t.style.cssText = `
        position:fixed;bottom:24px;left:50%;
        transform:translateX(-50%) translateY(20px);
        padding:10px 20px;border-radius:10px;font-size:13px;font-weight:600;
        color:#fff;z-index:99999;opacity:0;transition:all .25s;
        background:${type === 'warn' ? '#d97706' : '#16a34a'};
        box-shadow:0 4px 20px rgba(0,0,0,.4);white-space:nowrap;
    `;
    document.body.appendChild(t);
    requestAnimationFrame(() => { t.style.opacity = '1'; t.style.transform = 'translateX(-50%) translateY(0)'; });
    setTimeout(() => { t.style.opacity = '0'; setTimeout(() => t.remove(), 300); }, 3000);
}