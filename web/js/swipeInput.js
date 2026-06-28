// Ports SwipeController.cs — emits CustomEvents on the target element.
// Events: 'lefttap', 'righttap', 'midtap', 'swipe'
// Keyboard: ArrowLeft → lefttap, ArrowRight → righttap, ArrowUp → swipe
//
// If a `scrollTarget` is given, a predominantly-vertical drag scrolls that
// element instead of being read as an answer — so an overflowing quiz stack
// can be panned through during play.
export class SwipeInput {
  constructor(el, scrollTarget = null) {
    this._el = el;
    this._scrollTarget = scrollTarget;
    this._startX = 0;
    this._startY = 0;
    this._lastY = 0;
    this._tracking = false;
    this._scrolling = false;

    el.addEventListener('pointerdown',  e => this._onDown(e));
    el.addEventListener('pointermove',  e => this._onMove(e));
    el.addEventListener('pointerup',    e => this._onUp(e));
    el.addEventListener('pointercancel', () => { this._tracking = false; this._scrolling = false; });
    document.addEventListener('keydown', e => this._onKey(e));
  }

  _onDown(e) {
    this._startX = e.clientX;
    this._startY = e.clientY;
    this._lastY = e.clientY;
    this._tracking = true;
    this._scrolling = false;
    this._el.setPointerCapture(e.pointerId);
  }

  _onMove(e) {
    if (!this._tracking || !this._scrollTarget) return;

    // Latch into scroll mode once the gesture is clearly vertical, then stay
    // there for the rest of the drag so it can't flip back to an answer.
    if (!this._scrolling) {
      const dx = Math.abs(e.clientX - this._startX);
      const dy = Math.abs(e.clientY - this._startY);
      if (dy > 8 && dy > dx) this._scrolling = true;
      else return;
    }

    // clientY deltas are in screen pixels; the board is CSS-scaled by scaler.js,
    // so divide by the rendered/layout ratio to keep the scroll under the finger.
    const t = this._scrollTarget;
    const scale = t.getBoundingClientRect().height / (t.clientHeight || 1);
    t.scrollTop -= (e.clientY - this._lastY) / (scale || 1);
    this._lastY = e.clientY;
  }

  _onUp(e) {
    if (!this._tracking) return;
    this._tracking = false;

    // A drag that scrolled the quiz is not an answer.
    if (this._scrolling) { this._scrolling = false; return; }

    const dx = e.clientX - this._startX;
    const dy = e.clientY - this._startY;
    const absDx = Math.abs(dx);
    const absDy = Math.abs(dy);

    // Horizontal swipe threshold (≈ SwipeController deadzone 100px on desktop)
    if (absDx > 60 && absDx > absDy) {
      this._emit('swipe');
      return;
    }

    // Tap on the = divider hexagon → equal answer
    if (document.elementsFromPoint(this._startX, this._startY)
          .some(el => el.closest && el.closest('.divider'))) {
      this._emit('midtap');
      return;
    }

    // Tap — left or right half of the screen
    const rect = this._el.getBoundingClientRect();
    const x    = this._startX - rect.left;
    if (x < rect.width / 2) this._emit('lefttap');
    else                     this._emit('righttap');
  }

  _onKey(e) {
    switch (e.key) {
      case 'ArrowLeft':  e.preventDefault(); this._emit('lefttap');  break;
      case 'ArrowRight': e.preventDefault(); this._emit('righttap'); break;
      case 'ArrowUp':    e.preventDefault(); this._emit('swipe');    break;
    }
  }

  _emit(type) {
    this._el.dispatchEvent(new CustomEvent(type, { bubbles: true }));
  }
}
