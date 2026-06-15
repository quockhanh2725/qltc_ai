'use strict';

document.addEventListener('DOMContentLoaded', async () => {


    const $ = id => document.getElementById(id);
    const $$ = sel => document.querySelectorAll(sel);

    const btnEditCancel = $('btnEditCancel');
    const btnEditSave = $('btnEditSave');
    const btnDelCancel = $('btnDelCancel');
    const btnDelConfirm = $('btnDelConfirm');
    const btnOpenChat = $('btnOpenChat');
    const searchInp = $('searchInp');
    const catFilter = $('catFilter');
    const typeFilter = $('typeFilter');

    const PAGE_SIZE = 5;

    const CAT_ICON = {
        1: '🍜', 2: '🚗', 3: '🏠', 4: '🎮',
        5: '📚', 6: '💊', 7: '🛒', 8: '➕',
        14: '💵'
    };

    const CAT_BG = {
        1: '#fef3c7', 2: '#dbeafe', 3: '#f0fdf4', 4: '#fce7f3',
        5: '#dbeafe', 6: '#dcfce7', 7: '#ede9fe', 8: '#dcfce7',
        14: '#dcfce7'
    };

    let allTx = [];
    let filtered = [];
    let currentPage = 1;

    async function safeJson(res) {
        const text = await res.text();
        try { return JSON.parse(text); } catch { return { message: text }; }
    }

    function fmt(n) {
        return Number(n).toLocaleString('vi-VN') + 'đ';
    }

    function fmtDate(str) {
        if (!str) return '—';
        const d = new Date(str);
        const pad = x => String(x).padStart(2, '0');
        return `${pad(d.getDate())}/${pad(d.getMonth() + 1)}/${d.getFullYear()} ${pad(d.getHours())}:${pad(d.getMinutes())}`;
    }

    function escHtml(str) {
        return String(str)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

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

    async function loadTransactions() {
        try {
            const res = await fetch('/transaction/list');
            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message || 'Lỗi tải giao dịch');

            allTx = (Array.isArray(data) ? data : (data.items ?? [])).map(x => ({
                id: x.idGiaoDich,
                note: x.noiDung,
                soTien: x.tien,
                loai: x.loaiGiaoDich === 'ThuNhap' ? 'inc' : 'exp',
                ngay: x.ngayGiaoDich,

                idDanhMuc: x.idChiTietNavigation?.idDanhMuc,
                danhMuc: x.idChiTietNavigation?.idDanhMucNavigation?.tenDanhMuc
            }));
            applyFilters();
        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        }
    }

    async function loadCategories() {
        try {
            const res = await fetch('/category/all');
            const data = await safeJson(res);

            if (!res.ok) throw new Error(data.message || 'Lỗi load danh mục');

            renderCategoryOptions(data);
        } catch (err) {
            console.error(err);
            showToast(err.message, 'warn');
        }
    }

    function applyFilters() {
        const search = searchInp.value.trim().toLowerCase();
        const catVal = catFilter.value;
        const typeVal = typeFilter.value;

        filtered = allTx.filter(tx => {
            const matchSearch = !search ||
                (tx.note ?? '').toLowerCase().includes(search) ||
                (tx.danhMuc ?? '').toLowerCase().includes(search);
            const matchCat = !catVal || String(tx.idDanhMuc) === catVal;
            const matchType = !typeVal || tx.loai === typeVal;
            return matchSearch && matchCat && matchType;
        });

        currentPage = 1;
        renderSummaryCards();
        renderTable();
        renderPagination();
    }

   
    function renderSummaryCards() {
        let inc = 0, exp = 0;
        filtered.forEach(tx => {
            if (tx.loai === 'inc') inc += tx.soTien ?? 0;
            else exp += tx.soTien ?? 0;
        });
        $('cardInc').textContent = fmt(inc);
        $('cardExp').textContent = fmt(exp);
        $('cardSav').textContent = fmt(inc - exp);
        $('cardCount').textContent = filtered.length;
    }

    function renderCategoryOptions(categories) {
        catFilter.innerHTML = `<option value="">Tất cả danh mục</option>`;

        categories.forEach(c => {
            const opt = document.createElement('option');
            opt.value = c.idDanhMuc;
            opt.textContent = c.tenDanhMuc;

            catFilter.appendChild(opt);
        });
    }

    function renderTable() {
        const tbody = $('txBody');
        const start = (currentPage - 1) * PAGE_SIZE;
        const page = filtered.slice(start, start + PAGE_SIZE);

        $('txCount').textContent =
            `Hiển thị ${page.length} / ${filtered.length} giao dịch`;

        if (!page.length) {
            tbody.innerHTML = `<tr><td colspan="6"
            style="text-align:center;padding:40px;color:var(--muted,#888)">
            Không có giao dịch nào phù hợp.</td></tr>`;
            return;
        }

        tbody.innerHTML = page.map(tx => {
            const isInc = tx.loai === 'inc';
            const icon = CAT_ICON[tx.idDanhMuc] ?? '🧾';
            const iconBg = CAT_BG[tx.idDanhMuc] ?? '#f5f3ef';
            const amtCls = isInc ? 'c-green' : 'c-red';
            const prefix = isInc ? '+' : '-';

            return `
        <tr data-id="${tx.id}">
            <td style="white-space:nowrap;color:var(--text3);font-size:11.5px">
                ${fmtDate(tx.ngay)}</td>
            <td>
                <div style="display:flex;align-items:center;gap:9px">
                    <div style="width:30px;height:30px;border-radius:8px;
                        background:${iconBg};display:flex;align-items:center;
                        justify-content:center;font-size:14px;flex-shrink:0">
                        ${icon}</div>
                    <span style="font-weight:600">${escHtml(tx.note ?? '—')}</span>
                </div>
            </td>
            <td><span style="font-size:11.5px;color:var(--text2)">
                ${escHtml(tx.danhMuc ?? '—')}</span></td>
            <td><span class="type-badge ${isInc ? 'type-inc' : 'type-exp'}">
                ${isInc ? 'Thu nhập' : 'Chi tiêu'}</span></td>
            <td style="text-align:right;font-weight:700;font-size:13px;
                color:${isInc ? 'var(--green)' : 'var(--red)'}">
                ${prefix}${fmt(tx.soTien ?? 0)}
            </td>
            <td class="th-action">
                <div style="display:flex;gap:5px;justify-content:center">
                    <button class="act-btn edit-btn" data-id="${tx.id}">✏️ Sửa</button>
                    <button class="act-btn del-btn"
                        data-id="${tx.id}"
                        data-name="${escHtml(tx.note ?? '')}">🗑 Xoá</button>
                </div>
            </td>
        </tr>`;
        }).join('');
    }

    function renderPagination() {
        const wrap = $('paginationWrap');
        const totalPages = Math.ceil(filtered.length / PAGE_SIZE);

        if (totalPages <= 1) { wrap.innerHTML = ''; return; }

        const pages = [];
        for (let i = 1; i <= totalPages; i++) {
            pages.push(
                `<button class="page-btn${i === currentPage ? ' active' : ''}" data-page="${i}">${i}</button>`
            );
        }

        wrap.innerHTML = `
            <button class="page-btn" data-page="${currentPage - 1}" ${currentPage === 1 ? 'disabled' : ''}>‹</button>
            ${pages.join('')}
            <button class="page-btn" data-page="${currentPage + 1}" ${currentPage === totalPages ? 'disabled' : ''}>›</button>
        `;
    }

    function goPage(p) {
        const totalPages = Math.ceil(filtered.length / PAGE_SIZE);
        if (p < 1 || p > totalPages) return;
        currentPage = p;
        renderTable();
        renderPagination();
    }

    function openEditModal(id) {
        const tx = allTx.find(t => String(t.id) === String(id));
        if (!tx) return showToast('Không tìm thấy giao dịch', 'warn');

        $('editId').value = tx.id;
        $('editName').value = tx.note ?? '';
        $('editAmt').value = Number(tx.soTien ?? 0).toLocaleString('vi-VN');


        $('editModal').classList.add('open');
    }

    function closeEditModal() {
        $('editModal').classList.remove('open');
    }

    async function saveEdit() {
        const idTran = $('editId').value;
        const name = $('editName').value.trim();
        const amtRaw = $('editAmt').value.replace(/[^\d]/g, '');
        const newMoney = parseInt(amtRaw);

        if (!name) return showToast('Nhập tên giao dịch', 'warn');
        if (!newMoney || newMoney <= 0) return showToast('Nhập số tiền hợp lệ', 'warn');

        btnEditSave.disabled = true;
        btnEditSave.textContent = '⏳ Đang lưu...';

        try {
            const res = await fetch(`/transaction/update`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                body: new URLSearchParams({ idTran, newMoney, newNote: name }),
            });
            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message || 'Lỗi cập nhật');

            showToast('Đã cập nhật giao dịch', 'ok');
            closeEditModal();
            await loadTransactions();
        } catch (err) {
            console.error(err);
            showToast(err.message, 'err');
        } finally {
            btnEditSave.disabled = false;
            btnEditSave.textContent = '💾 Lưu thay đổi';
        }
    }

    function openDelModal(id, name) {
        $('delId').value = id;
        $('delModalSub').textContent = `Bạn có chắc muốn xoá "${name}"?`;
        $('delModal').classList.add('open');
    }

    function closeDelModal() {
        $('delModal').classList.remove('open');
    }

    async function confirmDelete() {
        const idTran = $('delId').value;

        btnDelConfirm.disabled = true;
        btnDelConfirm.textContent = '⏳ Đang xoá...';

        try {
            const res = await fetch(`/transaction/delete/${idTran}`, { method: 'DELETE' });
            const data = await safeJson(res);
            if (!res.ok) throw new Error(data.message || 'Lỗi xoá giao dịch');

            showToast('Đã xoá giao dịch', 'ok');
            closeDelModal();
            await loadTransactions();
        } catch (err) {
            console.error(err);
            showToast(err.message, 'err');
        } finally {
            btnDelConfirm.disabled = false;
            btnDelConfirm.textContent = '🗑 Xoá';
        }
    }

   
    searchInp.addEventListener('input', applyFilters);
    catFilter.addEventListener('change', applyFilters);
    typeFilter.addEventListener('change', applyFilters);

    btnEditCancel.addEventListener('click', closeEditModal);
    btnEditSave.addEventListener('click', saveEdit);
    $('editAmt').addEventListener('input', function () {
        const raw = this.value.replace(/\D/g, '');
        this.value = raw ? Number(raw).toLocaleString('vi-VN') : '';
    });

    btnDelCancel.addEventListener('click', closeDelModal);
    btnDelConfirm.addEventListener('click', confirmDelete);

    $$('.modal-overlay').forEach(overlay => {
        overlay.addEventListener('click', e => {
            if (e.target === overlay) overlay.classList.remove('open');
        });
    });

    
    $('txBody').addEventListener('click', e => {
        const editBtn = e.target.closest('.edit-btn');
        const delBtn = e.target.closest('.del-btn');
        if (editBtn) openEditModal(editBtn.dataset.id);
        if (delBtn) openDelModal(delBtn.dataset.id, delBtn.dataset.name);
    });

    // Event delegation — pagination
    $('paginationWrap').addEventListener('click', e => {
        const btn = e.target.closest('.page-btn');
        if (btn && !btn.disabled) goPage(Number(btn.dataset.page));
    });

    await loadCategories();
    await loadTransactions();

});