// Centralised popup + Back handling.
//
// A popup opened via openPopup() pushes a history entry, so the browser/device
// Back button (and the Esc key) closes the top popup instead of leaving the
// page. Every dismissal routes through history.back(), so the back stack stays
// in sync no matter how the popup was closed (Esc, backdrop tap, a button, or
// the Back gesture). Nested popups close one at a time, newest first.

const _stack = [];

// Falls back to direct show/hide if the History API is unavailable (e.g. a
// sandboxed preview iframe), so popups still work even without Back integration.
let _historyOk = true;

// Show `el` as a popup. `onClose` (optional) runs when it is dismissed.
export function openPopup(el, onClose = null) {
  el.style.display = '';
  _stack.push({ el, onClose });
  if (_historyOk) {
    try { history.pushState({ popup: _stack.length }, ''); }
    catch { _historyOk = false; }
  }
}

// Dismiss the top popup. Goes through history (so popstate performs the hide and
// the pushed entry is consumed) when available, else hides directly. No-op when
// nothing is open.
export function closePopup() {
  if (!_stack.length) return;
  if (_historyOk) history.back();
  else _hideTop();
}

export function popupOpen() { return _stack.length > 0; }

function _hideTop() {
  const top = _stack.pop();
  if (!top) return;
  top.el.style.display = 'none';
  top.onClose?.();
}

// Back button / gesture: the entry we pushed was just popped — hide our popup.
// With no popup open, do nothing and let the browser navigate normally.
window.addEventListener('popstate', () => {
  if (_stack.length) _hideTop();
});

// Esc mirrors Back: close the top popup if one is open.
document.addEventListener('keydown', e => {
  if (e.key === 'Escape' && _stack.length) {
    e.preventDefault();
    closePopup();
  }
});
