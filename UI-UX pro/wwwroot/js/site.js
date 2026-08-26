// UIAnimate gallery — shared front-end helpers
window.UIAnimate = window.UIAnimate || {};

// Copy text to clipboard with fallback for sandboxed environments
UIAnimate.copyText = async function (text, button) {
    const done = () => {
        if (!button) return;
        const original = button.textContent;
        button.textContent = '✅ Copied!';
        button.classList.add('liked');
        setTimeout(() => {
            button.textContent = original;
            button.classList.remove('liked');
        }, 1600);
    };

    try {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            done();
            return;
        }
    } catch { /* fall through */ }

    try {
        const ta = document.createElement('textarea');
        ta.value = text;
        ta.style.position = 'fixed';
        ta.style.opacity = '0';
        document.body.appendChild(ta);
        ta.select();
        document.execCommand('copy');
        document.body.removeChild(ta);
        done();
    } catch {
        if (button) {
            button.textContent = '⚠ Copy failed';
            setTimeout(() => { button.textContent = 'Copy'; }, 1600);
        }
    }
};

// Like buttons on gallery cards (increments via API)
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.card-stats .like-count').forEach(el => {
        el.addEventListener('click', async e => {
            e.stopPropagation();
            const id = el.dataset.id;
            if (!id || el.classList.contains('liked')) return;
            const res = await fetch('/api/animations/' + id + '/like', { method: 'POST' });
            if (res.ok) {
                el.classList.add('liked');
                el.textContent = '♥ ' + (parseInt(el.textContent.replace(/\D/g, '') || '0') + 1);
            }
        });
    });
});
