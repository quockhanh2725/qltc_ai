'use strict';

const QR_TIMEOUT = 60;

let qrStream = null;
let qrScanning = false;
let qrRafId = null;
let qrCountdownInterval = null;
let selectedPhotoFile = null;

document.addEventListener('DOMContentLoaded', () => {
    const $ = id => document.getElementById(id);

    document.querySelectorAll('.mob-tab').forEach(btn => {
        btn.addEventListener('click', () => {
            document.querySelectorAll('.mob-tab').forEach(b => b.classList.remove('active'));
            document.querySelectorAll('.mob-panel').forEach(p => p.classList.remove('active'));
            btn.classList.add('active');
            const panelId = 'tab' + btn.dataset.tab.charAt(0).toUpperCase() + btn.dataset.tab.slice(1);
            $(panelId).classList.add('active');
            if (btn.dataset.tab !== 'qr') stopQrCamera(false);
            else startQrCamera();
        });
    });

    $('btnQrStop').addEventListener('click', () => stopQrCamera(false));
    $('btnQrRestart').addEventListener('click', () => {
        $('qrResult').classList.remove('show');
        hideSubmitSection();
        startQrCamera();
    });

    $('btnCamera').addEventListener('click', () => $('photoInputCamera').click());
    $('btnGallery').addEventListener('click', () => $('photoInputGallery').click());
    $('photoInputCamera').addEventListener('change', e => onFileSelected(e.target.files[0]));
    $('photoInputGallery').addEventListener('change', e => onFileSelected(e.target.files[0]));

    $('btnRetake').addEventListener('click', () => {
        selectedPhotoFile = null;
        $('photoThumbImg').src = '';
        $('photoThumb').style.display = 'none';
        $('photoDrop').style.display = 'flex';
        $('photoFileInfo').textContent = '';
        $('photoInputCamera').value = '';
        $('photoInputGallery').value = '';
        $('btnRetake').classList.add('hidden');
        $('photoResult').classList.remove('show');
        hideSubmitSection();
    });

    $('btnRescan').addEventListener('click', () => {
        hideSubmitSection();
        $('qrResult').classList.remove('show');
        $('photoResult').classList.remove('show');
        $('photoThumb').style.display = 'none';
        $('photoDrop').style.display = 'flex';
        $('photoInputCamera').value = '';
        $('photoInputGallery').value = '';
        $('btnRetake').classList.add('hidden');
        selectedPhotoFile = null;
        if (document.querySelector('.mob-tab[data-tab="qr"]').classList.contains('active')) {
            startQrCamera();
        }
    });

    $('btnSubmit').addEventListener('click', onSubmit);

    startQrCamera();
});

const g = id => document.getElementById(id);

async function startQrCamera() {
    if (qrStream) return;
    try {
        qrStream = await navigator.mediaDevices.getUserMedia({
            video: { facingMode: { ideal: 'environment' }, width: { ideal: 640 } }
        });
        const v = g('qrVideo');
        v.srcObject = qrStream;
        await v.play();
        g('qrCamOff').style.display = 'none';
        g('btnQrStop').classList.remove('hidden');
        g('btnQrRestart').classList.add('hidden');
        qrScanning = true;
        qrRafId = requestAnimationFrame(scanQrFrame);
        startQrCountdown();
    } catch (e) {
        g('qrCamOff').style.display = 'flex';
        toast('Không mở được camera: ' + e.message, 'warn');
    }
}

function startQrCountdown() {
    clearQrCountdown();
    let sec = QR_TIMEOUT;
    g('qrCountdownSec').textContent = sec;
    g('qrCountdown').style.display = 'block';
    qrCountdownInterval = setInterval(() => {
        sec--;
        g('qrCountdownSec').textContent = sec;
        if (sec <= 0) { clearQrCountdown(); stopQrCamera(true); }
    }, 1000);
}

function clearQrCountdown() {
    if (qrCountdownInterval) { clearInterval(qrCountdownInterval); qrCountdownInterval = null; }
    g('qrCountdown').style.display = 'none';
}

function stopQrCamera(autoStopped = false) {
    qrScanning = false;
    clearQrCountdown();
    if (qrRafId) { cancelAnimationFrame(qrRafId); qrRafId = null; }
    if (qrStream) { qrStream.getTracks().forEach(t => t.stop()); qrStream = null; }
    g('qrVideo').srcObject = null;
    g('btnQrStop').classList.add('hidden');
    g('btnQrRestart').classList.remove('hidden');
    if (autoStopped) toast('⏱ Camera tự tắt sau 60 giây', 'warn');
}

function scanQrFrame() {
    if (!qrScanning) return;
    const v = g('qrVideo');
    if (v.readyState < v.HAVE_ENOUGH_DATA) { qrRafId = requestAnimationFrame(scanQrFrame); return; }
    const c = g('qrCanvas');
    c.width = v.videoWidth;
    c.height = v.videoHeight;
    const ctx = c.getContext('2d', { willReadFrequently: true });
    ctx.drawImage(v, 0, 0);
    const img = ctx.getImageData(0, 0, c.width, c.height);
    const code = jsQR(img.data, img.width, img.height, { inversionAttempts: 'dontInvert' });
    if (code) { onQrDetected(code.data); return; }
    qrRafId = requestAnimationFrame(scanQrFrame);
}

async function onQrDetected(text) {
    qrScanning = false;
    stopQrCamera(false);
    qrLoading(true, 'Đang phân tích mã QR...');
    try {
        const res = await fetch('/ai/scan-qr', {
            method: 'POST',
            body: new URLSearchParams({ qrText: text, token: SCAN_TOKEN })
        });
        const data = await res.json();
        if (!res.ok) throw new Error(data.message || 'Lỗi');
        qrLoading(false);
        renderQrResult(text, data);
    } catch (e) {
        qrLoading(false);
        toast('Lỗi QR: ' + e.message, 'warn');
    }
}

function qrLoading(on, txt) {
    g('qrLoading').classList.toggle('on', on);
    if (txt) g('qrLoadingTxt').textContent = txt;
}

function renderQrResult(rawText, d) {
    const div = g('qrResult');
    const bc = (d.type === 'payment' || d.type === 'vietqr') ? 'payment' : d.type === 'url' ? 'url' : 'other';
    const tl = { payment: '💳 Thanh toán', vietqr: '🏦 VietQR', url: '🔗 Đường dẫn', text: '📝 Văn bản', other: '📦 Khác' };
    const ir = (k, v) => `<div class="info-row"><span class="k">${k}</span><span class="v">${v}</span></div>`;

    let rows = '';
    if (d.totalAmount) rows += ir('Số tiền', Number(d.totalAmount).toLocaleString('vi-VN') + ' ' + (d.currency || 'VND'));
    if (d.bankInfo) rows += ir('Ngân hàng', d.bankInfo);
    if (d.accountNumber) rows += ir('Số TK', d.accountNumber);
    if (d.accountName) rows += ir('Chủ TK', d.accountName);
    if (d.url) rows += ir('URL', `<a href="${d.url}" target="_blank" style="color:#818cf8">${d.url}</a>`);

    div.innerHTML = `
        <span class="qr-badge ${bc}">${tl[d.type] || d.type}</span>
        <div style="font-size:14px;line-height:1.6;margin-bottom:6px">${d.summary || ''}</div>
        ${rows ? `<hr class="divider"><div>${rows}</div>` : ''}
        <hr class="divider">
        <div style="font-size:10px;text-transform:uppercase;letter-spacing:.08em;color:#475569;margin-bottom:4px">Nội dung QR gốc</div>
        <div class="qr-raw">${rawText}</div>
    `;
    div.classList.add('show');

    if (d.totalAmount) {
        g('mobMoney').value = d.totalAmount;
        g('mobNote').value = d.summary || d.accountName || 'Thanh toán QR';
        showSubmitSection();
    }
}

function onFileSelected(f) {
    if (!f) return;
    selectedPhotoFile = f;
    const r = new FileReader();
    r.onload = ev => {
        g('photoThumbImg').src = ev.target.result;
        g('photoThumb').style.display = 'block';
    };
    r.readAsDataURL(f);
    g('photoDrop').style.display = 'none';
    g('photoFileInfo').textContent = f.name + ' — ' + (f.size / 1024).toFixed(0) + ' KB';
    g('btnRetake').classList.remove('hidden');
    g('photoResult').classList.remove('show');
    hideSubmitSection();
    analyzeBill();
}

async function analyzeBill() {
    if (!selectedPhotoFile) return;
    g('photoLoading').classList.add('on');
    hideSubmitSection();
    try {
        const fd = new FormData();
        fd.append('image', selectedPhotoFile);
        if (SCAN_TOKEN) fd.append('token', SCAN_TOKEN);
        const res = await fetch('/ai/scan-image', { method: 'POST', body: fd });
        const data = await res.json();
        if (res.status === 401) throw new Error('Phiên hết hạn, quét lại QR trên PC.');
        if (!res.ok) throw new Error(data.message || 'Lỗi');
        renderBillResult(data);
    } catch (e) {
        toast('Lỗi: ' + e.message, 'warn');
    } finally {
        g('photoLoading').classList.remove('on');
    }
}

function renderBillResult(d) {
    const div = g('photoResult');
    const fmt = n => Number(n).toLocaleString('vi-VN');
    div.innerHTML =
        `<div style="font-size:10px;text-transform:uppercase;letter-spacing:.08em;color:#475569;margin-bottom:2px">Tổng tiền</div>` +
        `<div style="font-size:28px;font-weight:700;letter-spacing:-1px">${fmt(d.money || 0)}<span style="font-size:13px;color:#64748b;font-weight:400;margin-left:4px">VND</span></div>` +
        `<hr class="divider">` +
        `<div style="font-size:11px;color:#94a3b8;line-height:1.6">${d.note || ''}</div>`;
    div.classList.add('show');
    g('mobMoney').value = d.money || '';
    g('mobNote').value = d.note || '';
    showSubmitSection();
}

function showSubmitSection() {
    const s = g('submitSection');
    s.style.display = 'flex';
    s.scrollIntoView({ behavior: 'smooth' });
}

function hideSubmitSection() {
    g('submitSection').style.display = 'none';
}

async function onSubmit() {
    const note = g('mobNote').value.trim();
    const money = g('mobMoney').value.replace(/[^\d]/g, '');
    if (!note) { toast('Vui lòng nhập ghi chú', 'warn'); return; }
    if (!money) { toast('Vui lòng nhập số tiền', 'warn'); return; }

    const text = /\d/.test(note) ? note : `${note} ${money} đồng`;
    const params = { text };
    if (SCAN_TOKEN) params.token = SCAN_TOKEN;

    g('submitLoading').classList.add('on');
    g('btnSubmit').disabled = true;
    try {
        const res = await fetch('/transaction/chat', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: new URLSearchParams(params)
        });
        const data = await res.json();
        if (res.status === 401) throw new Error('Phiên hết hạn, quét lại QR trên PC.');
        if (!res.ok) throw new Error(data.message || 'Lỗi');
        toast('✅ Đã thêm giao dịch! Trang sẽ tự đóng sau 5 giây...');
        if (SCAN_TOKEN) {
            try { await fetch('/ai/scan-done?token=' + encodeURIComponent(SCAN_TOKEN), { method: 'POST' }); } catch { }
        }
        setTimeout(() => {
            window.close();
            document.body.innerHTML = `
                <div style="display:flex;flex-direction:column;align-items:center;justify-content:center;min-height:100dvh;gap:16px;padding:24px;text-align:center;background:#0f172a;color:#f1f5f9">
                    <div style="font-size:56px">✅</div>
                    <div style="font-size:20px;font-weight:700">Giao dịch đã được thêm!</div>
                    <div style="font-size:14px;color:#64748b">Bạn có thể đóng tab này.</div>
                </div>`;
        }, 5000);
    } catch (e) {
        toast('Lỗi: ' + e.message, 'warn');
    } finally {
        g('submitLoading').classList.remove('on');
        g('btnSubmit').disabled = false;
    }
}

function toast(msg, type) {
    const t = document.createElement('div');
    t.className = 'mob-toast';
    t.textContent = msg;
    t.style.background = type === 'warn' ? '#d97706' : '#16a34a';
    document.body.appendChild(t);
    requestAnimationFrame(() => {
        t.style.opacity = '1';
        t.style.transform = 'translateX(-50%) translateY(0)';
    });
    setTimeout(() => { t.style.opacity = '0'; setTimeout(() => t.remove(), 300); }, 3000);
}