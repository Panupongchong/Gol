// Spine canvas renderer for GOL block objects.
// Requires @esotericsoftware/spine-canvas loaded globally (window.spine).
// Call load(basePath) once before any createBlock() calls.

const CANVAS_CSS  = 120;
const SPINE_UNITS = 202;
const BLOCK_CSS   = 58;

const _instances    = new Map();
const _skeletonData = {};
let   _atlas    = null;
let   _raf      = null;
let   _lastTime = 0;
let   _nextId   = 0;

// QuizBoard — live animated skeleton (separate HiLoRush_Gameplay atlas).
let   _boardSkeletonData = null;
let   _boardCenter       = { cx: 0, cy: 0 };

// ── Public API ───────────────────────────────────────────────────────────────

export async function load(basePath = 'spine') {
  const imageNames = [
    'VisualTypes.png',
    'VisualTypes_2.png', 'VisualTypes_3.png', 'VisualTypes_4.png',
    'VisualTypes_5.png', 'VisualTypes_6.png', 'VisualTypes_7.png',
  ];

  const images = {};
  await Promise.all(imageNames.map(name => new Promise((res, rej) => {
    const img = new Image();
    img.onload  = () => { images[name] = img; res(); };
    img.onerror = () => rej(new Error(`Failed to load ${basePath}/${name}`));
    img.src = `${basePath}/${name}`;
  })));

  const atlasText = await fetch(`${basePath}/VisualTypes.atlas`).then(r => r.text());
  _atlas = new spine.TextureAtlas(atlasText, path => {
    if (!images[path]) throw new Error(`Spine atlas references unknown texture: ${path}`);
    return new spine.CanvasTexture(images[path]);
  });
  // spine-canvas 4.2 IIFE doesn't store the loader return value on page.texture —
  // set page textures manually, then propagate to each region.
  for (const page of _atlas.pages) {
    if (!page.texture) page.texture = new spine.CanvasTexture(images[page.name]);
    for (const region of page.regions) region.texture = page.texture;
  }

  const attachLoader = new spine.AtlasAttachmentLoader(_atlas);
  const jsonParser   = new spine.SkeletonJson(attachLoader);

  await Promise.all(Array.from({ length: 13 }, (_, i) => {
    const file = i.toString().padStart(2, '0');
    return fetch(`${basePath}/${file}.json`)
      .then(r => r.json())
      .then(data => { _skeletonData[i] = jsonParser.readSkeletonData(data); });
  }));

  await _loadBoard(basePath);

  _startLoop();
}

// Loads the QuizBoard skeleton (separate HiLoRush_Gameplay atlas) and keeps its
// SkeletonData + centre for createBoard() to instantiate live, animated copies.
async function _loadBoard(basePath) {
  const img = await new Promise((res, rej) => {
    const im = new Image();
    im.onload  = () => res(im);
    im.onerror = () => rej(new Error(`Failed to load ${basePath}/HiLoRush_Gameplay.png`));
    im.src = `${basePath}/HiLoRush_Gameplay.png`;
  });

  const atlasText = await fetch(`${basePath}/HiLoRush_Gameplay.atlas`).then(r => r.text());
  const atlas = new spine.TextureAtlas(atlasText, () => new spine.CanvasTexture(img));
  for (const page of atlas.pages) {
    if (!page.texture) page.texture = new spine.CanvasTexture(img);
    for (const region of page.regions) region.texture = page.texture;
  }

  const jsonParser = new spine.SkeletonJson(new spine.AtlasAttachmentLoader(atlas));
  const data       = await fetch(`${basePath}/QuizBoard.json`).then(r => r.json());
  _boardSkeletonData = jsonParser.readSkeletonData(data);

  // Board bounds (spine units) → centre, from the skeleton header.
  _boardCenter = {
    cx: _boardSkeletonData.x + _boardSkeletonData.width  / 2,
    cy: _boardSkeletonData.y + _boardSkeletonData.height / 2,
  };
}

// Creates a live QuizBoard instance. The backing buffer is fixed to the board's
// native size × dpr; CSS scales the element to fill its row. Starts on `inactive`.
export function createBoard() {
  const dpr = Math.min(window.devicePixelRatio || 1, 2);
  const canvas = document.createElement('canvas');
  if (!_boardSkeletonData) {
    console.error('[spine] board skeleton not loaded');
    return canvas;
  }
  canvas.width  = Math.round(_boardSkeletonData.width  * dpr);
  canvas.height = Math.round(_boardSkeletonData.height * dpr);
  Object.assign(canvas.style, {
    position: 'absolute',
    inset: '0',
    width:  '100%',
    height: '100%',
    pointerEvents: 'none',
  });

  const ctx      = canvas.getContext('2d');
  const skeleton = new spine.Skeleton(_boardSkeletonData);
  skeleton.scaleX = dpr;
  skeleton.scaleY = dpr;
  skeleton.setToSetupPose();

  const animState = new spine.AnimationState(new spine.AnimationStateData(_boardSkeletonData));
  animState.setAnimation(0, 'inactive', false);

  const id = _nextId++;
  _instances.set(id, {
    canvas, ctx, skeleton, animState,
    phys: canvas.width, physH: canvas.height,
    ox: _boardCenter.cx * dpr, oy: _boardCenter.cy * dpr,
    tint: true,
  });
  canvas._spineId = id;
  return canvas;
}

// Plays a board clip, optionally queuing a follow-up (e.g. flash → back to active).
export function playBoardAnim(canvas, animName, { then = null, thenLoop = false } = {}) {
  const inst = _instances.get(canvas?._spineId);
  if (!inst) return;
  inst.animState.setAnimation(0, animName, false);
  if (then) inst.animState.addAnimation(0, then, thenLoop, 0);
}

export function createBlock(number, skinName) {
  const dpr   = Math.min(window.devicePixelRatio || 1, 2);
  const phys  = CANVAS_CSS * dpr;
  const scale = (BLOCK_CSS / SPINE_UNITS) * dpr;

  const canvas  = document.createElement('canvas');
  canvas.width  = phys;
  canvas.height = phys;
  Object.assign(canvas.style, {
    position: 'absolute',
    width:  `${CANVAS_CSS}px`,
    height: `${CANVAS_CSS}px`,
    top:    '50%',
    left:   '50%',
    transform: 'translate(-50%,-50%)',
    pointerEvents: 'none',
  });

  const ctx    = canvas.getContext('2d');
  const skData = _skeletonData[number];
  if (!skData) {
    console.error('[spine] no skeletonData for index', number);
    return canvas;
  }

  const skeleton  = new spine.Skeleton(skData);
  skeleton.setSkinByName(skinName);
  skeleton.setToSetupPose();
  skeleton.scaleX = scale;
  skeleton.scaleY = scale;

  const stateData = new spine.AnimationStateData(skData);
  const animState = new spine.AnimationState(stateData);
  animState.setAnimation(0, 'idle', true);   // loop so it never expires

  const id = _nextId++;
  _instances.set(id, { canvas, ctx, skeleton, animState, phys, physH: phys, ox: 0, oy: 0, _firstFrame: true });
  canvas._spineId = id;
  return canvas;
}

export function playAnim(canvas, animName, loop = false) {
  const inst = _instances.get(canvas?._spineId);
  if (inst) inst.animState.setAnimation(0, animName, loop);
}

// Becoming the active row: play the one-shot `active` entrance, then settle into
// the looping `idle` resting pose.
export function playActive(canvas) {
  const inst = _instances.get(canvas?._spineId);
  if (!inst) return;
  inst.animState.setAnimation(0, 'active', false);
  inst.animState.addAnimation(0, 'idle', true, 0);
}

export function playIncorrect(canvas) {
  const inst = _instances.get(canvas?._spineId);
  if (!inst) return;
  inst.animState.setAnimation(0, 'incorrect', false);
  inst.animState.addAnimation(0, 'active', false, 0);
  inst.animState.addAnimation(0, 'idle', true, 0);
}

export function destroyBlock(canvas) {
  const id = canvas?._spineId;
  if (id !== undefined) {
    _instances.delete(id);
    delete canvas._spineId;
  }
}

export function destroyAll(rootEl) {
  rootEl.querySelectorAll('canvas').forEach(c => destroyBlock(c));
}

// ── Render loop ──────────────────────────────────────────────────────────────

function _startLoop() {
  if (_raf !== null) return;
  _lastTime = performance.now();
  _raf = requestAnimationFrame(_loop);
}

function _loop(now) {
  const dt = Math.min((now - _lastTime) / 1000, 0.064);
  _lastTime = now;

  for (const [, inst] of _instances) {
    const { ctx, skeleton, animState, phys, physH } = inst;

    animState.update(dt);
    animState.apply(skeleton);
    if (typeof skeleton.update === 'function') skeleton.update(dt);
    skeleton.updateWorldTransform(spine.Physics.update);

    ctx.clearRect(0, 0, phys, physH);
    ctx.save();
    ctx.translate(phys / 2, physH / 2);
    ctx.scale(1, -1);
    ctx.translate(-inst.ox, -inst.oy); // 0 for blocks; board centre offset otherwise
    _drawSkeleton(ctx, skeleton, inst);
    ctx.restore();
  }

  _raf = requestAnimationFrame(_loop);
}

// Direct region-attachment renderer — bypasses spine.SkeletonRenderer to
// avoid any class-reference or version-mismatch issues with instanceof checks.
function _drawSkeleton(ctx, skeleton, inst) {
  const drawOrder = skeleton.drawOrder;
  for (let i = 0; i < drawOrder.length; i++) {
    const slot = drawOrder[i];
    if (!slot.bone.active) continue;

    const att = slot.getAttachment();
    // Region attachments carry a 'region' property set by AtlasAttachmentLoader
    if (!att || !att.region) continue;

    const region = att.region;
    const img    = region.texture && region.texture.getImage
                   ? region.texture.getImage()
                   : null;
    if (!img) continue;

    // On the very first frame, log what we're about to draw so console shows the chain
    if (inst && inst._firstFrame) {
      console.log('[spine] drawing slot', slot.data.name,
                  'att', att.name || '?',
                  'region', region.name || '?',
                  'img', img.src ? img.src.split('/').pop() : '?',
                  'natural', img.naturalWidth + 'x' + img.naturalHeight,
                  'bone active', slot.bone.active,
                  'alpha', skeleton.color.a * slot.color.a * att.color.a);
    }

    const alpha = skeleton.color.a * slot.color.a * att.color.a;
    if (alpha <= 0) continue;

    const w  = region.width;
    const h  = region.height;
    const sx = Math.round((img.naturalWidth  || img.width)  * region.u);
    const sy = Math.round((img.naturalHeight || img.height) * region.v);

    // Two-color tint (board only). Blocks keep the alpha-only fast path unchanged.
    const light  = slot.color;
    const dark   = slot.darkColor;
    const source = !(inst && inst.tint) || (_isWhite(light) && _isDark0(dark))
                 ? img
                 : _tintedRegion(slot, img, sx, sy, w, h, light, dark);
    const ssx = source === img ? sx : 0;
    const ssy = source === img ? sy : 0;

    const bone = slot.bone;
    ctx.save();
    ctx.globalAlpha = alpha;
    ctx.transform(bone.a, bone.c, bone.b, bone.d, bone.worldX, bone.worldY);
    ctx.translate(att.offset[0], att.offset[1]);
    ctx.rotate(att.rotation * Math.PI / 180);
    const as_ = att.width / region.originalWidth;
    ctx.scale(as_ * att.scaleX, as_ * att.scaleY);
    ctx.translate(w / 2, h / 2);
    ctx.scale(1, -1);
    ctx.translate(-w / 2, -h / 2);
    ctx.drawImage(source, ssx, ssy, w, h, 0, 0, w, h);
    ctx.restore();
  }

  if (inst && inst._firstFrame) {
    inst._firstFrame = false;
  }
}

// ── Two-color tint ─────────────────────────────────────────────────────────────

function _isWhite(c) { return c.r > 0.996 && c.g > 0.996 && c.b > 0.996; }
function _isDark0(c) { return !c || (c.r < 0.004 && c.g < 0.004 && c.b < 0.004); }
function _rgb(c)     { return `rgb(${c.r*255|0},${c.g*255|0},${c.b*255|0})`; }

// Returns a w×h canvas of the region tinted as out = tex*light + (1-tex)*dark,
// masked to the texture's alpha. Cached on the slot, recomputed only on change.
function _tintedRegion(slot, img, sx, sy, w, h, light, dark) {
  const key = `${sx},${sy},${w},${h}|${_rgb(light)}|${dark ? _rgb(dark) : '_'}`;
  if (slot._tintKey === key && slot._tintCanvas) return slot._tintCanvas;

  let t = slot._tintCanvas;
  if (!t) { t = document.createElement('canvas'); slot._tintCanvas = t; }
  if (t.width !== w || t.height !== h) { t.width = w; t.height = h; }
  const tc = t.getContext('2d');

  // Light layer: tex.rgb * light.rgb, kept within tex alpha.
  tc.globalCompositeOperation = 'source-over';
  tc.clearRect(0, 0, w, h);
  tc.drawImage(img, sx, sy, w, h, 0, 0, w, h);
  if (!_isWhite(light)) {
    tc.globalCompositeOperation = 'multiply';
    tc.fillStyle = _rgb(light);
    tc.fillRect(0, 0, w, h);
    tc.globalCompositeOperation = 'destination-in';
    tc.drawImage(img, sx, sy, w, h, 0, 0, w, h);
  }

  // Dark layer: (1 - tex.rgb) * dark.rgb, added in.
  if (!_isDark0(dark)) {
    const d  = _tintScratch(w, h);
    const dc = d.getContext('2d');
    dc.globalCompositeOperation = 'source-over';
    dc.clearRect(0, 0, w, h);
    dc.drawImage(img, sx, sy, w, h, 0, 0, w, h);
    dc.globalCompositeOperation = 'difference';   // 1 - tex.rgb
    dc.fillStyle = '#fff';
    dc.fillRect(0, 0, w, h);
    dc.globalCompositeOperation = 'multiply';      // * dark.rgb
    dc.fillStyle = _rgb(dark);
    dc.fillRect(0, 0, w, h);
    dc.globalCompositeOperation = 'destination-in';
    dc.drawImage(img, sx, sy, w, h, 0, 0, w, h);

    tc.globalCompositeOperation = 'lighter';
    tc.drawImage(d, 0, 0, w, h, 0, 0, w, h);
  }

  tc.globalCompositeOperation = 'source-over';
  slot._tintKey = key;
  return t;
}

// Shared scratch canvas for the dark layer (sized up as needed).
let _scratch = null;
function _tintScratch(w, h) {
  if (!_scratch) _scratch = document.createElement('canvas');
  if (_scratch.width < w)  _scratch.width  = w;
  if (_scratch.height < h) _scratch.height = h;
  return _scratch;
}
