'use strict';

const C = {
    green: '#22c55e',
    red: '#ef4444',
    indigo: '#6366f1',
    orange: '#f97316',
    teal: '#14b8a6',
    amber: '#f59e0b',
    blue: '#3b82f6',
    purple: '#8b5cf6',
    slate: '#94a3b8',
    bg: '#f5f3ef',
    surface: '#ffffff',
};

const alpha = (hex, a) => {
    const r = parseInt(hex.slice(1, 3), 16);
    const g = parseInt(hex.slice(3, 5), 16);
    const b = parseInt(hex.slice(5, 7), 16);
    return `rgba(${r},${g},${b},${a})`;
};

Chart.defaults.font.family = "-apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif";
Chart.defaults.font.size = 11;
Chart.defaults.color = '#7a736b';

function initChartWeekly() {
    const ctx = document.getElementById('chartWeekly');
    if (!ctx) return;
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: ['Tuần 1', 'Tuần 2', 'Tuần 3', 'Tuần 4'],
            datasets: [
                {
                    label: 'Thu nhập',
                    data: [3750, 3750, 3750, 3750],
                    backgroundColor: alpha(C.green, .85),
                    borderRadius: 5,
                    borderSkipped: false,
                },
                {
                    label: 'Chi tiêu',
                    data: [2310, 2100, 2500, 2330],
                    backgroundColor: alpha(C.red, .8),
                    borderRadius: 5,
                    borderSkipped: false,
                },
            ],
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => ` ${ctx.dataset.label}: ${ctx.parsed.y.toLocaleString('vi-VN')}k`,
                    },
                },
            },
            scales: {
                x: { grid: { display: false }, ticks: { autoSkip: false } },
                y: {
                    beginAtZero: true,
                    grid: { color: alpha('#000', .05) },
                    ticks: { callback: v => v + 'k', maxTicksLimit: 5 },
                },
            },
        },
    });
}

function initChartTrend() {
    const ctx = document.getElementById('chartTrend');
    if (!ctx) return;
    const months = ['T1/26', 'T2/26', 'T3/26', 'T4/26', 'T5/26', 'T6/26'];
    const income = [13500, 13500, 15000, 13800, 13800, 15000];
    const expense = [8200, 9100, 10500, 7800, 8240, 9240];
    const savings = income.map((v, i) => v - expense[i]);
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: months,
            datasets: [
                {
                    label: 'Thu nhập',
                    data: income,
                    borderColor: C.indigo,
                    backgroundColor: alpha(C.indigo, .08),
                    borderWidth: 2,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    fill: false,
                    tension: .35,
                },
                {
                    label: 'Chi tiêu',
                    data: expense,
                    borderColor: C.orange,
                    backgroundColor: alpha(C.orange, .08),
                    borderWidth: 2,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    fill: false,
                    tension: .35,
                },
                {
                    label: 'Tiết kiệm',
                    data: savings,
                    borderColor: C.teal,
                    backgroundColor: alpha(C.teal, .12),
                    borderWidth: 2,
                    pointRadius: 4,
                    pointHoverRadius: 6,
                    fill: true,
                    tension: .35,
                },
            ],
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    mode: 'index',
                    intersect: false,
                    callbacks: {
                        label: ctx => ` ${ctx.dataset.label}: ${ctx.parsed.y.toLocaleString('vi-VN')}k`,
                    },
                },
            },
            scales: {
                x: { grid: { display: false }, ticks: { autoSkip: false } },
                y: {
                    beginAtZero: true,
                    grid: { color: alpha('#000', .05) },
                    ticks: { callback: v => v + 'k', maxTicksLimit: 5 },
                },
            },
            interaction: { mode: 'index', intersect: false },
        },
    });
}

function initChartDonut() {
    const ctx = document.getElementById('chartDonut');
    if (!ctx) return;
    const labels = ['Ăn uống', 'Nhà ở', 'Di chuyển', 'Mua sắm', 'Sức khỏe', 'Khác'];
    const data = [3696, 2310, 1386, 924, 462, 462];
    const colors = [C.red, C.amber, C.blue, C.purple, C.teal, C.slate];
    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels,
            datasets: [{
                data,
                backgroundColor: colors,
                borderColor: C.surface,
                borderWidth: 3,
                hoverOffset: 6,
            }],
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '65%',
            plugins: {
                legend: {
                    display: true,
                    position: 'right',
                    labels: {
                        usePointStyle: true,
                        pointStyle: 'rectRounded',
                        padding: 10,
                        font: { size: 11 },
                        generateLabels: chart => {
                            const ds = chart.data.datasets[0];
                            const total = ds.data.reduce((a, b) => a + b, 0);
                            return chart.data.labels.map((lbl, i) => ({
                                text: `${lbl}  ${Math.round(ds.data[i] / total * 100)}%`,
                                fillStyle: ds.backgroundColor[i],
                                strokeStyle: ds.backgroundColor[i],
                                index: i,
                            }));
                        },
                    },
                },
                tooltip: {
                    callbacks: {
                        label: ctx => {
                            const total = ctx.dataset.data.reduce((a, b) => a + b, 0);
                            const pct = Math.round(ctx.parsed / total * 100);
                            return ` ${ctx.label}: ${ctx.parsed.toLocaleString('vi-VN')}k (${pct}%)`;
                        },
                    },
                },
            },
        },
    });
}

function initChartBudget() {
    const ctx = document.getElementById('chartBudget');
    if (!ctx) return;
    const categories = ['Ăn uống', 'Nhà ở', 'Di chuyển', 'Mua sắm', 'Sức khỏe'];
    const budget = [4200, 2500, 1500, 1200, 600];
    const spent = [3696, 2310, 1386, 924, 462];
    const pct = spent.map((s, i) => Math.round(s / budget[i] * 100));
    const barColors = pct.map(p => p >= 85 ? alpha(C.red, .8) : p >= 65 ? alpha(C.amber, .8) : alpha(C.teal, .8));
    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: categories,
            datasets: [
                {
                    label: 'Đã chi (%)',
                    data: pct,
                    backgroundColor: barColors,
                    borderRadius: 4,
                    borderSkipped: false,
                },
                {
                    label: 'Còn lại (%)',
                    data: pct.map(p => Math.max(0, 100 - p)),
                    backgroundColor: alpha('#000', .06),
                    borderRadius: 4,
                    borderSkipped: false,
                },
            ],
        },
        options: {
            indexAxis: 'y',
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => ctx.datasetIndex === 0
                            ? ` Đã chi: ${ctx.parsed.x}%`
                            : ` Còn lại: ${ctx.parsed.x}%`,
                    },
                },
            },
            scales: {
                x: {
                    stacked: true,
                    max: 100,
                    grid: { color: alpha('#000', .05) },
                    ticks: { callback: v => v + '%', maxTicksLimit: 6 },
                },
                y: {
                    stacked: true,
                    grid: { display: false },
                    ticks: { autoSkip: false },
                },
            },
        },
    });
}

function initChartRadar() {
    const ctx = document.getElementById('chartRadar');
    if (!ctx) return;
    new Chart(ctx, {
        type: 'radar',
        data: {
            labels: ['Sáng sớm\n(5–8h)', 'Buổi sáng\n(8–11h)', 'Trưa\n(11–13h)', 'Chiều\n(13–17h)', 'Tối\n(17–21h)', 'Đêm\n(21–24h)'],
            datasets: [
                {
                    label: 'Tháng 6',
                    data: [45, 210, 180, 95, 320, 150],
                    borderColor: C.indigo,
                    backgroundColor: alpha(C.indigo, .15),
                    borderWidth: 2,
                    pointRadius: 4,
                    pointBackgroundColor: C.indigo,
                },
                {
                    label: 'Tháng 5',
                    data: [30, 175, 160, 110, 280, 130],
                    borderColor: C.orange,
                    backgroundColor: alpha(C.orange, .1),
                    borderWidth: 2,
                    pointRadius: 4,
                    pointBackgroundColor: C.orange,
                    borderDash: [4, 4],
                },
            ],
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    display: true,
                    position: 'bottom',
                    labels: { usePointStyle: true, padding: 12, font: { size: 11 } },
                },
                tooltip: {
                    callbacks: {
                        label: ctx => ` ${ctx.dataset.label}: ${ctx.parsed.r.toLocaleString('vi-VN')}k`,
                    },
                },
            },
            scales: {
                r: {
                    beginAtZero: true,
                    grid: { color: alpha('#000', .07) },
                    angleLines: { color: alpha('#000', .07) },
                    pointLabels: { font: { size: 10 } },
                    ticks: {
                        backdropColor: 'transparent',
                        callback: v => v + 'k',
                        stepSize: 100,
                    },
                },
            },
        },
    });
}

function initChartSavings() {
    const ctx = document.getElementById('chartSavings');
    if (!ctx) return;
    const days = Array.from({ length: 15 }, (_, i) => `${i + 1}/6`);
    const cumulative = [0, 0, 0, 120, 120, 265, 380, 380, 510, 595, 595, 720, 830, 880, 960];
    new Chart(ctx, {
        type: 'line',
        data: {
            labels: days,
            datasets: [{
                label: 'Tiết kiệm lũy kế (k đ)',
                data: cumulative,
                borderColor: C.teal,
                backgroundColor: alpha(C.teal, .18),
                borderWidth: 2.5,
                pointRadius: 3,
                pointHoverRadius: 6,
                pointBackgroundColor: C.teal,
                fill: true,
                tension: .4,
            }],
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => ` Tiết kiệm: ${ctx.parsed.y.toLocaleString('vi-VN')}k đ`,
                    },
                },
            },
            scales: {
                x: {
                    grid: { display: false },
                    ticks: { autoSkip: true, maxTicksLimit: 8 },
                },
                y: {
                    beginAtZero: true,
                    grid: { color: alpha('#000', .05) },
                    ticks: { callback: v => v + 'k', maxTicksLimit: 5 },
                },
            },
        },
    });
}

document.addEventListener('DOMContentLoaded', () => {
    initChartWeekly();
    initChartTrend();
    initChartDonut();
    initChartBudget();
    initChartRadar();
    initChartSavings();
});