'use strict';

let curTab = 'expense';
let currentBudgetId = null;
let currentIncome = 0;
let currentBudgetDate = null;

document.addEventListener("DOMContentLoaded", async () => {

    const $ = id => document.getElementById(id);
    const $$ = sel => document.querySelectorAll(sel);

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


    $$('#formTabs .form-tab').forEach(btn => {
        btn.addEventListener('click', () => switchTab(btn.dataset.tab));
    });

});