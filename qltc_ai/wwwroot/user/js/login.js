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

    btnLogin?.addEventListener("click", doLogin);
    btnRegister?.addEventListener("click", doRegister);
    btnVerify?.addEventListener("click", verifyOtp);
    btnResend?.addEventListener("click", resendOtp);

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

    function checkPwStrength(input) {
        const val = input.value;
        const fill = document.getElementById('pw-strength-fill');
        if (!fill) return;

        let score = 0;
        if (val.length >= 6) score++;
        if (/[A-Z]/.test(val)) score++;
        if (/[0-9]/.test(val)) score++;
        if (/[^A-Za-z0-9]/.test(val)) score++;

        const widths = ['0%', '25%', '50%', '75%', '100%'];
        const colors = ['#ef4444', '#f59e0b', '#eab308', '#22c55e', '#16a34a'];
        fill.style.width = widths[score] || '0%';
        fill.style.background = colors[score - 1] || '#ef4444';
    }


    async function doLogin() {

        const email = document.querySelector('#panel-login input[type=text]').value.trim();
        const password = document.getElementById('pw-login').value;

        if (!email || !password) {
            return showToast('Vui lòng nhập email và mật khẩu.', 'warn');
        }

        if (password.length < 6) {
            return showToast('Mật khẩu ít nhất 6 ký tự', 'warn');
        }


        const btn = document.querySelector('#panel-login .auth-btn.primary');
        btn.disabled = true;
        btn.textContent = '⏳ Đang xử lý...';


        try {
            const res = await fetch('/account/login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ email, password })
            });

            const data = await res.json();

            if (!res.ok) throw new Error(data.message);

            showToast('Đăng nhập thành công', 'ok');
            setTimeout(() => window.location.href = '/', 1000);

        } catch (err) {
            showToast(err.message, 'warn');
        } finally {
            btn.disabled = false;
            btn.textContent = 'Đăng nhập';
        }
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


    async function doRegister() {
        const email = document.getElementById('reg-email').value.trim();
        const password = document.getElementById('pw-reg').value;
        const terms = document.getElementById('reg-terms').checked;
        const btnr = document.getElementById('btn-resend');

        if (!email || !password)
            return showToast('Thiếu thông tin', 'warn');

        if (!email.endsWith('@gmail.com')) {
            showToast('Email phải có đuôi @gmail.com', 'warn');
            return;
        }

        if (password.length < 6)
            return showToast('Mật khẩu có ít nhất 6 ký tự', 'warn');

        if (!terms)
            return showToast('Chưa đồng ý điều khoản', 'warn');

        const btn = document.querySelector('#register-step-1 .auth-btn.primary');
        btn.disabled = true;
        btn.textContent = 'Đang gửi OTP...';

        try {
            const res = await fetch('/account/send', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ email, password })
            });

            const data = await res.json();
            if (!res.ok) throw new Error(data.message);

            document.getElementById('otp-hint').textContent = `OTP đã gửi tới ${email}`;
            document.getElementById('register-step-1').style.display = 'none';
            document.getElementById('register-step-2').style.display = '';

            showToast('Đã gửi OTP', 'ok');
            startOtpCountdown(60,btnr)

        } catch (err) {
            showToast(err.message, 'warn');
        } finally {
            btn.disabled = false;
            btn.textContent = 'Tạo tài khoản';
        }
    }


    function clearOtpBoxes() {
        ['otp-box-1', 'otp-box-2', 'otp-box-3', 'otp-box-4', 'otp-box-5', 'otp-box-6'].forEach(id => {
            const el = document.getElementById(id);
            if (el) { el.value = ''; el.classList.remove('otp-filled'); }
        });
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

    async function verifyOtp() {
        const otp = ['otp-box-1', 'otp-box-2', 'otp-box-3', 'otp-box-4', 'otp-box-5', 'otp-box-6']
            .map(id => document.getElementById(id).value)
            .join('');

        const email = document.getElementById('reg-email').value.trim();

        if (otp.length < 6) {
            showToast('Vui lòng nhập đủ 6 chữ số OTP.', 'warn');
            return;
        }

        const btn = document.querySelector('#register-step-2 .auth-btn.primary');
        btn.disabled = true;
        btn.textContent = '⏳ Đang xác nhận...';

        try {

            let res = await fetch('/account/verify', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ email, otp })
            });

            let data = await res.json();
            if (!res.ok) throw new Error(data.message);


            res = await fetch('/account/register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ email })
            });

            data = await res.json();
            if (!res.ok) throw new Error(data.message);

            showToast('Đăng ký thành công', 'ok');
            setTimeout(() => switchTab('login'), 1000);

        } catch (err) {
            showToast(err.message, 'warn');
        } finally {
            btn.disabled = false;
            btn.textContent = 'Xác nhận';
        }
    }

    async function resendOtp() {
        const email = document.getElementById('reg-email').value.trim();
        const password = document.getElementById('pw-reg').value;
        const btn = document.getElementById('btn-resend');

        btn.disabled = true;
        btn.textContent = 'Đang gửi...';

        try {
            const res = await fetch('/account/send', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ email, password })
            });

            const data = await res.json();
            if (!res.ok) throw new Error(data.message);

            clearOtpBoxes();
            document.getElementById('otp-box-1').focus();

            showToast('Đã gửi lại OTP', 'ok');

            startOtpCountdown(60, btn);

        } catch (err) {
            showToast(err.message, 'warn');
            btn.disabled = false;
            btn.textContent = 'Gửi lại OTP';
        }
    }

    function startOtpCountdown(seconds, btn) {
        clearInterval(otpTimer);

        otpTimeLeft = seconds;

        otpTimer = setInterval(() => {
            otpTimeLeft--;

            btn.textContent = `Gửi lại sau ${otpTimeLeft}s`;
            btn.disabled = true;

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

    document.getElementById("pw-reg")
        ?.addEventListener("input", (e) => checkPwStrength(e.target));

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
