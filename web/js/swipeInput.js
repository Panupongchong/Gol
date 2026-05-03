// Ports SwipeController.cs — emits CustomEvents on the target element.
// Events: 'lefttap', 'righttap', 'midtap', 'swipe'
// Keyboard: ArrowLeft → lefttap, ArrowRight → righttap, ArrowUp → swipe
export class SwipeInput {
  constructor(el) {
    this._el = el;
    this._startX = 0;
    this._startY = 0;
    this._tracking = false;

    el.addEventListener('pointerdown',  e => this._onDown(e));
    el.addEventListener('pointerup',    e => this._onUp(e));
    el.addEventListener('pointercancel', () => { this._tracking = false; });
    document.addEventListener('keydown', e => this._onKey(e));
  }

  _onDown(e) {
    this._startX = e.clientX;
    this._startY = e.clientY;
    this._tracking = true;
    this._el.setPointerCapture(e.pointerId);
  }

  _onUp(e) {
    if (!this._tracking) return;
    this._tracking = false;

    const dx = e.clientX - this._startX;
    const dy = e.clientY - this._startY;
    const absDx = Math.abs(dx);
    const absDy = Math.abs(dy);

    // Horizontal swipe threshold (≈ SwipeController deadzone 100px on desktop)
    if (absDx > 60 && absDx > absDy) {
      this._emit('swipe');
      return;
    }

    // Tap — determine zone by X position relative to element width
    const rect   = this._el.getBoundingClientRect();
    const x      = this._startX - rect.left;
    const mid    = rect.width / 2;
    const margin = 40; // ±40px from centre = mid zone

    if (x < mid - margin)      this._emit('lefttap');
    else if (x > mid + margin) this._emit('righttap');
    else                        this._emit('midtap');
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
