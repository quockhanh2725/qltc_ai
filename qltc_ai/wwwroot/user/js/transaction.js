'use strict';

let curTab = 'expense';
let currentBudgetId = null;
let currentIncome = 0;
let currentBudgetDate = null;

document.addEventListener("DOMContentLoaded", async () => {

    const $ = id => document.getElementById(id);
    const $$ = sel => document.querySelectorAll(sel);
    const btnSaveT = document.getElementById("btnSaveTx");
    const btnResetT = document.getElementById("btnResetTx");
    const btnSaveB = document.getElementById("btnSaveBud");
    const btnResetB = document.getElementById("btnResetBud");

    const CAT_ICON = {
        1: '🍜',
        2: '🚗',
        3: '🏠',
        4: '🎮',
        5: '📚',
        6: '💊',
        7: '🛍',
        8: '➕'
    };


    async function safeJson(res) {
        const text = await res.text();
        try { return JSON.parse(text); } catch { return { message: text }; }
    }


    function switchTab(tab) {
        curTab = tab;

        $$('#formTabs .form-tab').forEach(b => {
            b.classList.remove('active-exp', 'active-bud');
            if (b.dataset.tab === tab) {
                b.classList.add(tab === 'expense' ? 'active-exp' : 'active-bud');
            }
        });

        $('panelExpense').style.display = tab === 'expense' ? 'grid' : 'none';
        $('panelBudget').style.display = tab === 'budget' ? 'grid' : 'none';

        $('topbarTitle').textContent = tab === 'expense' ? 'Thêm giao dịch' : 'Nhập ngân sách';
        $('topbarSub').textContent = tab === 'expense'
            ? 'AI tự nhận diện · Nhập tự nhiên'
            : 'Nhập ngân sách · Kiểm soát chi tiêu';

        if (tab === 'expense') loadCategories();
    }


    function showToast(msg, type) {
        const existing = document.querySelector('.auth-toast');
        if (existing) existing.remove();

        const toast = document.createElement('div');
        toast.className = 'auth-toast auth-toast--' + (type || 'ok');
        toast.textContent = msg;

        toast.style.cssText = `
            position:fixed;bottom:24px;left:50%;
            transform:translateX(-50%) translateY(20px);
            padding:10px 20px;border-radius:10px;
            font-size:13px;font-weight:600;
            color:#fff;z-index:9999;opacity:0;
            transition:all .25s;white-space:nowrap;
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


    async function loadCurrentBudget() {
        try {
            const res = await fetch('/budget/current');
            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message || "Lỗi budget");
            currentBudgetId = data.idNganSach || data.id;
            currentIncome = data.tongTien || 0;
            currentBudgetDate = data.thang || data.date;
        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        }
    }

    async function loadAll() {
        try {
            await loadCurrentBudget();
            if (!currentBudgetId) return;

            const res = await fetch(`/category/by-budget?budgetId=${currentBudgetId}`);
            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message || "Lỗi category");

            renderCategories(data);
            renderBudgetBars(data);
            renderBudgetOverview(data);

        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        }
    }


    async function saveTransaction() {
        const name = document.getElementById('fName').value.trim();
        const amtRaw = document.getElementById('fAmt').value.replace(/[^\d-]/g, '');
        const money = parseInt(amtRaw);
        const selected = document.querySelector('.cat-chip.on');
        const idDetail = selected?.dataset.id;

        if (!name) return showToast('Nhập nội dung giao dịch', 'warn');
        if (!money) return showToast('Nhập số tiền', 'warn');
        if (!idDetail) return showToast('Chọn danh mục', 'warn');
        if (money <= 0) return showToast('Số tiền không được âm', 'warn');

        const btn = document.getElementById('btnSaveTx');
        btn.disabled = true;
        btn.textContent = '⏳ Đang lưu...';

        try {
            const res = await fetch('/transaction/add', {
                method: 'POST',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ idDetail, money, note: name, typeTran: "ChiTieu" })
            });
            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message || "Lỗi thêm giao dịch");

            showToast('Thêm giao dịch thành công', 'ok');
            document.getElementById('fName').value = '';
            document.getElementById('fAmt').value = '';
            await loadAll();

        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        } finally {
            btn.disabled = false;
            btn.textContent = '💾 Lưu giao dịch';
        }
    }

    function resetFormT() {
        document.getElementById('fName').value = '';
        document.getElementById('fAmt').value = '';
    }


    function renderBudgetOverview(categoryList) {
        const tongChiTieu = categoryList.reduce((sum, c) => sum + (c.tienDaTieu || 0), 0);

        const conLai = currentIncome - tongChiTieu;

        const tietKiem = currentIncome > 0
            ? ((conLai / currentIncome) * 100).toFixed(1)
            : '0.0';

        const fmt = n => n.toLocaleString('vi-VN') + 'đ';
        const set = (id, val) => { const el = $(id); if (el) el.textContent = val; };

        set('bud-thu-nhap', fmt(currentIncome));
        set('bud-da-chi', fmt(tongChiTieu));
        set('bud-con-lai', fmt(conLai));
        set('bud-tiet-kiem', tietKiem + '%');

        if (currentBudgetDate) {
            const d = new Date(currentBudgetDate);
            const month = d.getMonth() + 1;
            const year = d.getFullYear();

            const title = document.getElementById('budgetTitle');
            if (title) {
                title.textContent = `📊 Tháng ${month}/${year}`;
            }
        }
    }


    async function saveBudget() {
        const amtRaw = document.getElementById('bAmt').value.replace(/[^\d-]/g, '');
        const money = parseInt(amtRaw);
        let month = null;
        if (currentBudgetDate) {
            const d = new Date(currentBudgetDate);
            month = d.getMonth() + 1;
        }

        if (!money) return showToast('Nhập số tiền ngân sách', 'warn');
        if (money <= 0) return showToast('Số tiền không được âm', 'warn');

        const btn = document.getElementById('btnSaveBud');
        btn.disabled = true;
        btn.textContent = '⏳ Đang lưu...';

        try {
            const res = await fetch(`/budget/add`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ money, note: `Thu nhập tháng ${month}`, typeTran: "ThuNhap" })

            });
            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message || "Lỗi lưu ngân sách");

            showToast('Lưu ngân sách thành công', 'ok');
            document.getElementById('bAmt').value = '';
            await loadAll();

        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        } finally {
            btn.disabled = false;
            btn.textContent = '💾 Lưu ngân sách';
        }
    }

    function resetFormB() {
        document.getElementById('bAmt').value = '';
    }


    function renderCategories(list) {
        const grid = $('catGrid');
        if (!grid) return;

        grid.innerHTML = '';

        list.forEach((c, index) => {
            const div = document.createElement('div');
            div.className = 'cat-chip' + (index === 0 ? ' on' : '');
            div.dataset.cat = c.idDanhMuc;
            div.dataset.id = c.idChiTiet;
            div.innerHTML = `
                <div class="cat-chip-ico">${CAT_ICON[c.idDanhMuc] || '🧾'}</div>
                <div class="cat-chip-lbl">${c.idDanhMucNavigation.tenDanhMuc}</div>
            `;
            div.addEventListener('click', () => {
                $$('.cat-chip').forEach(x => x.classList.remove('on'));
                div.classList.add('on');
            });
            grid.appendChild(div);
        });
    }

    function renderBudgetBars(list) {
        const box = document.querySelector('.exp-right .card:nth-child(2)');
        if (!box) return;

        box.querySelectorAll('.cat-bar-item').forEach(x => x.remove());

        const fmt = n => {
            if (n >= 1_000_000) {
                const tr = Math.floor(n / 1_000_000);
                const du = Math.floor((n % 1_000_000) / 100_000);
                return du > 0 ? `${tr}tr${du}` : `${tr}tr`;
            }

            if (n >= 1000) {
                const k = Math.floor(n / 1000);
                const du = Math.floor((n % 1000) / 100);
                return du > 0 ? `${k}k${du}` : `${k}k`;
            }

            return n;
        };

        list.forEach(c => {
            const spent = c.tienDaTieu || 0;
            const limit = c.gioiHanTien || 0;
            const percent = limit > 0 ? Math.min((spent / limit) * 100, 100) : 0;

            let color = 'var(--green-l)';
            if (percent > 80) color = 'var(--red)';
            else if (percent > 50) color = 'var(--amber)';

            const div = document.createElement('div');
            div.className = 'cat-bar-item';
            div.innerHTML = `
                <div class="cat-bar-head">
                    <span class="cat-bar-nm">${CAT_ICON[c.idDanhMuc] || '🧾'} ${c.idDanhMucNavigation.tenDanhMuc}</span>
                    <span class="cat-bar-amt" style="color:${color}">${fmt(spent)}/${fmt(limit)}</span>
                </div>
                <div class="cat-bar-track">
                    <div class="cat-bar-fill" style="width:${percent}%;background:${color}"></div>
                </div>
            `;
            box.insertBefore(div, box.lastElementChild);
        });
    }


    document.querySelectorAll('.quick-chip').forEach(btn => {
        btn.addEventListener('click', () => {
            const { name, amt, cat } = btn.dataset;
            document.getElementById('fName').value = name;
            document.getElementById('fAmt').value = amt;

            let found = false;
            document.querySelectorAll('.cat-chip').forEach(c => {
                c.classList.remove('on');
                if (c.dataset.cat === cat) { c.classList.add('on'); found = true; }
            });

            if (!found) {
                loadCategories().then(() => {
                    document.querySelectorAll('.cat-chip').forEach(c => {
                        if (c.dataset.cat === cat) c.classList.add('on');
                    });
                });
            }
        });
    });


    $$('#formTabs .form-tab').forEach(btn => {
        btn.addEventListener('click', () => switchTab(btn.dataset.tab));
    });


    await loadAll();

    btnSaveT?.addEventListener('click', saveTransaction);
    btnResetT?.addEventListener('click', resetFormT);
    btnSaveB?.addEventListener('click', saveBudget);
    btnResetB?.addEventListener('click', resetFormB);
});