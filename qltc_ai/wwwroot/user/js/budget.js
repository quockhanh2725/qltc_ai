'use strict';

let currentBudgetId = null;
let currentIncome = 0;
let currentBudgetDate = null;

document.addEventListener('DOMContentLoaded', async () => {

const BUDGETS = [];
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

const fmt = n => n.toLocaleString('vi-VN') + 'đ';

    const $ = id => document.getElementById(id);
    const $$ = sel => document.querySelectorAll(sel);


    async function safeJson(res) {
        const text = await res.text();
        try { return JSON.parse(text); }
        catch { return { message: text }; }
    }

    function getStatus(pct) {
        if (pct >= 80) return { cls: 'danger', icon: '⚠️' };
        if (pct >= 50) return { cls: 'warn', icon: '⚡' };
        return { cls: 'safe', icon: '✅' };
    }

    function getStatusLabel(pct) {
        if (pct >= 80) return 'Sắp hết!';
        if (pct >= 50) return 'Chú ý nhé';
        if (pct <= 15) return 'Rất tốt 🌟';
        return 'Đang tốt 👍';
    }

    function showToast(msg, type = 'ok') {
        document.querySelector('.bud-toast')?.remove();
        const toast = document.createElement('div');
        toast.className = 'bud-toast';
        toast.textContent = msg;

        toast.style.cssText = `
            position:fixed;bottom:24px;left:50%;
            transform:translateX(-50%) translateY(20px);
            padding:10px 20px;border-radius:10px;
            font-size:13px;font-weight:600;color:#fff;
            z-index:9999;opacity:0;transition:all .25s;
            white-space:nowrap;box-shadow:0 4px 20px rgba(0,0,0,.3);
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
            if (currentBudgetDate) {
                const d = new Date(currentBudgetDate);
                const month = d.getMonth() + 1;
                const year = d.getFullYear();

                const title = document.getElementById('month');
                if (title) {
                    title.textContent = `📊 Ngân sách tháng ${month}/${year}`;
                }
            }
        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        }
    }

    async function loadBudgets() {
        try {
            await loadCurrentBudget();
            if (!currentBudgetId) return;

            const res = await fetch(`/category/by-budget?budgetId=${currentBudgetId}`);
            const data = await safeJson(res);

            if (!res.ok) throw new Error(data.message || 'Lỗi load');

            BUDGETS.length = 0;

            data.forEach(x => {
                BUDGETS.push({
                    id: x.idChiTiet,
                    name: x.idDanhMucNavigation.tenDanhMuc,
                    total: x.gioiHanTien,
                    spent: x.tienDaTieu,
                    icon: CAT_ICON[x.idDanhMuc] || '🧾',
                    bg: '#eee',
                });
            });



        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        }
    }


    function renderKPI() {
        const totalAlloc = BUDGETS.reduce((s, b) => s + b.total, 0);
        const totalSpent = BUDGETS.reduce((s, b) => s + b.spent, 0);
        const remaining = currentIncome - totalSpent;

        const set = (key, val) => {
            const el = document.querySelector(`[data-kpi="${key}"]`);
            if (el) el.textContent = val;
        };

        set('budget', fmt(currentIncome));
        set('alloc', fmt(totalAlloc));
        set('spent', fmt(totalSpent));
        set('left', fmt(remaining));
    }

    function cardHTML(b) {
        const pct = b.total ? Math.round((b.spent / b.total) * 100) : 0;
        const left = b.total - b.spent;

        const { cls, icon } = getStatus(pct);
        const label = getStatusLabel(pct, b.daysLeft);

        const fillColor =
            cls === 'danger' ? 'var(--red)' :
                cls === 'warn' ? 'var(--amber)' : 'var(--green-l)';

        return `
        <div class="bud-card ${cls}" data-id="${b.id}">
            <div class="bud-head">
                <div class="bud-icon" style="background:${b.bg}">${b.icon}</div>
                <div>
                    <div class="bud-name">${b.name}</div>
                    <div class="bud-type">ngân sách tháng</div>
                </div>
                <span class="bud-status ${cls}">${icon} ${pct}%</span>
            </div>

            <div class="bud-spent">${fmt(b.spent)}</div>
            <div class="bud-of">/ ${fmt(b.total)}</div>

            <div class="bud-track">
                <div class="bud-fill" style="width:0%;background:${fillColor}" data-w="${pct}"></div>
            </div>

            <div class="bud-meta">
                <span>Còn lại ${fmt(left)}</span>
                <span>${label}</span>
            </div>

            <div class="bud-actions">
                <button class="bud-btn" data-action="ai">💡 AI</button>
                <button class="bud-btn" data-action="edit">✏️ Sửa</button>
            </div>
        </div>`;
    }

    function renderCards() {
        const grid = document.querySelector('.bud-grid');
        grid.innerHTML = BUDGETS.map(cardHTML).join('');

        requestAnimationFrame(() => {
            $$('.bud-fill').forEach(f => {
                f.style.width = f.dataset.w + '%';
            });
        });
    }

    function openEditModal(id) {
        const b = BUDGETS.find(x => x.id == id);
        if (!b) return;

        $('editLabel').textContent = `Ngân sách "${b.name}"`;
        $('editInput').value = b.total;
        $('editOverlay').dataset.id = id;
        $('editOverlay').style.display = 'flex';
    }

    function closeEditModal() {
        $('editOverlay').style.display = 'none';
    }

    async function saveEdit() {
        const val = parseInt($('editInput').value, 10);
        if (!val || val <= 0) return showToast('Số tiền không hợp lệ', 'warn');

        const id = $('editOverlay').dataset.id;

        try {
            const res = await fetch('/category/ulimit', {
                method: 'PUT',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ idDetail: id, newLimit: val })
            });

            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message);

            const b = BUDGETS.find(x => x.id == id);
            if (b) b.total = val;

            closeEditModal();
            renderCards();
            renderKPI();

            showToast('Đã cập nhật');

        } catch (err) {
            showToast(err.message, 'warn');
        }
    }


    document.querySelector('.bud-grid').addEventListener('click', e => {
        const btn = e.target.closest('[data-action]');
        if (!btn) return;

        const id = btn.closest('.bud-card')?.dataset.id;

        if (btn.dataset.action === 'edit') openEditModal(id);
        if (btn.dataset.action === 'ai') showToast('AI đang phát triển 😅');
    });

    $('editOverlay').addEventListener('click', e => {
        if (e.target === $('editOverlay')) closeEditModal();
    });

    $('btnSaveEdit').addEventListener('click', saveEdit);
    $('btnCancelEdit').addEventListener('click', closeEditModal);



    await loadBudgets();
    renderCards();
    renderKPI();

});