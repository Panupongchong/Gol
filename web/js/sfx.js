// Web Audio SFX — low-latency, polyphonic.
// Swap file numbers in SFX_FILES after auditioning the sounds.

const _ctx = new AudioContext();
const _buf = new Map();

// ── Assignment — change numbers here to remap ─────────────────────────
const SFX_FILES = {
  tap:        'sfx/DM-CGS-20.wav',  // light input tap
  correct:    'sfx/DM-CGS-04.wav',  // row answered correctly
  quizDone:   'sfx/DM-CGS-14.wav',  // whole panel cleared
  incorrect:  'sfx/DM-CGS-09.wav',  // wrong answer / game over
  itemFreeze: 'sfx/DM-CGS-31.wav',  // Freeze Time item activated
  itemDouble: 'sfx/DM-CGS-25.wav',  // Double Score item activated
  itemShield: 'sfx/DM-CGS-27.wav',  // Shield absorbed a hit
  countTick:  'sfx/DM-CGS-33.wav',  // countdown 3 / 2 / 1 beep
  countGo:    'sfx/DM-CGS-40.wav',  // countdown START
};

// ── Mute state — persisted across pages/sessions in localStorage ──────
const MUTE_KEY = 'gol-muted';
let _muted = false;
try { _muted = localStorage.getItem(MUTE_KEY) === '1'; } catch {}

export function isMuted() { return _muted; }

export function setMuted(m) {
  _muted = !!m;
  try { localStorage.setItem(MUTE_KEY, _muted ? '1' : '0'); } catch {}
}

// Flip the mute flag and return the new state.
export function toggleMute() { setMuted(!_muted); return _muted; }

export async function loadSfx() {
  await Promise.all(
    Object.entries(SFX_FILES).map(async ([name, path]) => {
      try {
        const ab = await fetch(path).then(r => r.arrayBuffer());
        _buf.set(name, await _ctx.decodeAudioData(ab));
      } catch (e) {
        console.warn('[sfx] failed to load', path, e);
      }
    })
  );
}

// Call on the first user gesture to satisfy browser autoplay policy.
export function unlockAudio() {
  if (_ctx.state === 'suspended') _ctx.resume();
}

export function play(name, volume = 1) {
  if (_muted) return;
  const buf = _buf.get(name);
  if (!buf) return;
  if (_ctx.state === 'suspended') _ctx.resume();
  const src  = _ctx.createBufferSource();
  const gain = _ctx.createGain();
  src.buffer      = buf;
  gain.gain.value = volume;
  src.connect(gain).connect(_ctx.destination);
  src.start(0);
}
