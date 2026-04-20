'use strict';

let otpTimer = null;
let otpTimeLeft = 0;

document.addEventListener("DOMContentLoaded", () => {
    const btnLogin = document.getElementById("btn-login");
    const btnRegister = document.getElementById("btn-register");
    const btnVerify = document.getElementById("btn-verify");
    const btnResend = document.getElementById("btn-resend");
    const otpBoxes = document.querySelectorAll(".otp-box");
    const otpIds = [
        'otp-box-1',
        'otp-box-2',
        'otp-box-3',
        'otp-box-4',
        'otp-box-5',
        'otp-box-6'
    ];


    function switchTab(tab) {

        document.querySelectorAll('.auth-form-panel').forEach(p => p.classList.remove('active'));

        document.querySelectorAll('.auth-tab').forEach(t => t.classList.remove('active'));

        const panel = document.getElementById('panel-' + tab);
        const tabBtn = document.getElementById('tab-' + tab);
        if (panel) panel.classList.add('active');
        if (tabBtn) tabBtn.classList.add('active');
    }

    function togglePw(inputId, btn) {
        const input = document.getElementById(inputId);
        if (!input) return;
        const isHidden = input.type === 'password';
        input.type = isHidden ? 'text' : 'password';
        btn.textContent = isHidden ? '🙈' : '👁';
    }

    function showToast(msg, type) {
        const existing = document.querySelector('.auth-toast');
        if (existing) existing.remove();

        const toast = document.createElement('div');
        toast.className = 'auth-toast auth-toast--' + (type || 'ok');
        toast.textContent = msg;

        const style = toast.style;
        style.cssText = `
        position:fixed;bottom:24px;left:50%;transform:translateX(-50%) translateY(20px);
        padding:10px 20px;border-radius:10px;font-size:13px;font-weight:600;
        color:#fff;z-index:9999;opacity:0;transition:all .25s;white-space:nowrap;
        font-family:'Be Vietnam Pro',sans-serif;
        box-shadow:0 4px 20px rgba(0,0,0,.4);
        background:${type === 'warn' ? '#d97706' : '#16a34a'};
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


    function otpMove(input, prevId, nextId) {
        const val = input.value.replace(/\D/g, '');
        input.value = val;
        if (val) {
            input.classList.add('otp-filled');
            if (nextId) document.getElementById(nextId).focus();
        } else {
            input.classList.remove('otp-filled');
        }
    }

    function otpBack(e, input, prevId) {
        if (e.key === 'Backspace' && !input.value && prevId) {
            const prev = document.getElementById(prevId);
            prev.value = '';
            prev.classList.remove('otp-filled');
            prev.focus();
        }
    }

    function startOtpCountdown(seconds, btn) {
        clearInterval(otpTimer);

        otpTimeLeft = seconds;

        otpTimer = setInterval(() => {
            otpTimeLeft--;

            btn.textContent = `Gửi lại sau ${otpTimeLeft}s`;

            if (otpTimeLeft <= 0) {
                clearInterval(otpTimer);
                btn.disabled = false;
                btn.textContent = 'Gửi OTP';
            }
        }, 1000);
    }

    document.getElementById("tab-login")
        ?.addEventListener("click", () => switchTab("login"));

    document.getElementById("tab-register")
        ?.addEventListener("click", () => switchTab("register"));

    document.getElementById("go-register")
        ?.addEventListener("click", () => switchTab("register"));

    document.getElementById("go-login")
        ?.addEventListener("click", () => switchTab("login"));

    document.querySelectorAll(".auth-pw-eye").forEach(btn => {
        btn.addEventListener("click", () => {
            const id = btn.dataset.target;
            togglePw(id, btn);
        });
    });

    

    otpIds.forEach((id, index) => {
        const input = document.getElementById(id);

        input.addEventListener("input", () => {
            const prev = otpIds[index - 1] || null;
            const next = otpIds[index + 1] || null;
            otpMove(input, prev, next);
        });

        input.addEventListener("keydown", (e) => {
            const prev = otpIds[index - 1] || null;
            otpBack(e, input, prev);
        });
    });
});
