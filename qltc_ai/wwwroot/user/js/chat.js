'use strict';

document.addEventListener('DOMContentLoaded', () => {

    const $ = id => document.getElementById(id);
    const $$ = sel => document.querySelectorAll(sel);

    function open() {
        $('cw-overlay').classList.add('open');
        $('cw-drawer').classList.add('open');
        setTimeout(() => $('cw-inp').focus(), 350);
    }

    function close() {
        $('cw-overlay').classList.remove('open');
        $('cw-drawer').classList.remove('open');
    }

    $('cw-overlay').addEventListener('click', close);
    $('cw-close').addEventListener('click', close);

    $$('[data-chat-trigger]').forEach(el => el.addEventListener('click', open));

    window.ChatWidget = { open, close };

});