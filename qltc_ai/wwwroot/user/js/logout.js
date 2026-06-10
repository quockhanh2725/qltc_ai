'use strict';

document.addEventListener('DOMContentLoaded', async () => {

    const $ = id => document.getElementById(id);
    const btnLogout = $('btn-logout');

    function showToast(msg, type = 'ok') {
        const existing = document.querySelector('.hist-toast-popup');
        if (existing) existing.remove();

        const toast = document.createElement('div');
        toast.className = 'hist-toast-popup';
        toast.textContent = msg;

        const BG = { ok: '#16a34a', warn: '#d97706', err: '#dc2626' };

        toast.style.cssText = `
            position:fixed;bottom:24px;left:50%;
            transform:translateX(-50%) translateY(20px);
            padding:10px 22px;border-radius:10px;
            font-size:13px;font-weight:600;
            color:#fff;z-index:9999;opacity:0;
            transition:all .25s;white-space:nowrap;
            font-family:'Be Vietnam Pro',sans-serif;
            box-shadow:0 4px 20px rgba(0,0,0,.35);
            background:${BG[type] ?? BG.ok};
            pointer-events:none;
        `;

        document.body.appendChild(toast);
        requestAnimationFrame(() => {
            toast.style.opacity = '1';
            toast.style.transform = 'translateX(-50%) translateY(0)';
        });
        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(-50%) translateY(10px)';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }

    async function Logout() {
        try {
            const res = await fetch('/account/logout', { method: 'POST' });
            if (res.ok) {
                window.location.href = '/account';
            }
        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        }
    }

    btnLogout.addEventListener('click' , Logout)
});