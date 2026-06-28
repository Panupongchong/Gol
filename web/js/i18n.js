// Lightweight i18n — language is persisted in localStorage and shared across
// all pages (menu / game / result). Markup opts in with `data-i18n="key"`
// (sets textContent) or `data-i18n-ph="key"` (sets the placeholder).

const LANG_KEY = 'gol-lang';
export const SUPPORTED = ['en', 'th'];
const DEFAULT_LANG = 'en';

// Friendly labels for the language pickers.
export const LANG_LABELS = { en: 'English', th: 'ไทย' };

const STRINGS = {
  en: {
    'menu.title':        'NUMBER PARTY',
    'menu.subtitle':     'HighLow Rush',
    'menu.normal':       'NORMAL',
    'menu.rank':         'RANK',

    'lang.select':       'SELECT LANGUAGE',
    'howto.title':       'HOW TO PLAY',
    'howto.close':       'GOT IT',
    'settings.title':    'SETTINGS',
    'settings.language': 'LANGUAGE',
    'settings.sound':    'SOUND',
    'settings.on':       'ON',
    'settings.off':      'OFF',
    'settings.close':    'CLOSE',

    'hud.score':         'SCORE',
    'hud.combo':         'COMBO',
    'hud.level':         'LEVEL',

    'game.loading':      'LOADING',
    'game.high':         'HIGH',
    'game.low':          'LOW',
    'game.over':         'GAME OVER',
    'game.start':        'START',

    'items.header':      'ITEMS',
    'items.freeze':      'FREEZE',
    'items.double':      'DOUBLE',
    'items.shield':      'SHIELD',

    'result.title':      'RESULT',
    'result.score':      'SCORE',
    'result.combo':      'COMBO',
    'result.bonus':      'BONUS',
    'result.best':       'BEST',
    'result.share':      'SHARE',
    'result.retry':      'RETRY',
    'result.home':       'HOME',
    'result.record':     'New Record!',
    'result.back':       'BACK',

    'rank.title':        'RANK LEADERBOARD',
    'rank.name.ph':      'YOUR NAME',
    'rank.submit':       'SUBMIT',
    'rank.loading':      'Loading…',
    'rank.unavailable':  'Leaderboard unavailable',
    'rank.empty':        'No scores yet — be the first!',
    'rank.ineligible':   'Score not eligible for ranking this run',
    'rank.submitting':   'Submitting…',
    'rank.submitted':    'Submitted — {score} pts',
    'rank.submitfail':   'Submit failed — try again',

    'challenge.title':   'CHALLENGE',
    'challenge.label':   'CAN YOU BEAT',
    'challenge.combo':   'combo {combo}',
    'challenge.playnow': 'PLAY NOW',

    'share.title':       'SHARE',
    'share.preview':     'I scored {score} in NUMBER PARTY! Can you beat it?',
    'share.messenger':   'Messenger',
    'share.share':       'Share',
    'share.copy':        'Copy',
    'share.copied':      'Copied!',
    'share.cancel':      'CANCEL',
  },
  th: {
    'menu.title':        'ปาร์ตี้ตัวเลข',
    'menu.subtitle':     'ศึกสูง–ต่ำ',
    'menu.normal':       'ปกติ',
    'menu.rank':         'จัดอันดับ',

    'lang.select':       'เลือกภาษา',
    'howto.title':       'วิธีเล่น',
    'howto.close':       'เข้าใจแล้ว',
    'settings.title':    'ตั้งค่า',
    'settings.language': 'ภาษา',
    'settings.sound':    'เสียง',
    'settings.on':       'เปิด',
    'settings.off':      'ปิด',
    'settings.close':    'ปิด',

    'hud.score':         'คะแนน',
    'hud.combo':         'คอมโบ',
    'hud.level':         'เลเวล',

    'game.loading':      'กำลังโหลด',
    'game.high':         'สูง',
    'game.low':          'ต่ำ',
    'game.over':         'เกมจบแล้ว',
    'game.start':        'เริ่ม',

    'items.header':      'ไอเทม',
    'items.freeze':      'หยุดเวลา',
    'items.double':      'คูณสอง',
    'items.shield':      'โล่',

    'result.title':      'ผลลัพธ์',
    'result.score':      'คะแนน',
    'result.combo':      'คอมโบ',
    'result.bonus':      'โบนัส',
    'result.best':       'สูงสุด',
    'result.share':      'แชร์',
    'result.retry':      'เล่นอีกครั้ง',
    'result.home':       'หน้าหลัก',
    'result.record':     'สถิติใหม่!',
    'result.back':       'กลับ',

    'rank.title':        'กระดานจัดอันดับ',
    'rank.name.ph':      'ชื่อของคุณ',
    'rank.submit':       'ส่ง',
    'rank.loading':      'กำลังโหลด…',
    'rank.unavailable':  'ไม่สามารถโหลดกระดานอันดับได้',
    'rank.empty':        'ยังไม่มีคะแนน — มาเป็นคนแรกกันเถอะ!',
    'rank.ineligible':   'คะแนนรอบนี้ไม่เข้าเกณฑ์การจัดอันดับ',
    'rank.submitting':   'กำลังส่ง…',
    'rank.submitted':    'ส่งแล้ว — {score} คะแนน',
    'rank.submitfail':   'ส่งไม่สำเร็จ — ลองอีกครั้ง',

    'challenge.title':   'คำท้า',
    'challenge.label':   'เอาชนะได้ไหม',
    'challenge.combo':   'คอมโบ {combo}',
    'challenge.playnow': 'เล่นเลย',

    'share.title':       'แชร์',
    'share.preview':     'ฉันทำได้ {score} คะแนนใน NUMBER PARTY! เอาชนะได้ไหม?',
    'share.messenger':   'เมสเซนเจอร์',
    'share.share':       'แชร์',
    'share.copy':        'คัดลอก',
    'share.copied':      'คัดลอกแล้ว!',
    'share.cancel':      'ยกเลิก',
  },
};

let _lang = DEFAULT_LANG;
try {
  const saved = localStorage.getItem(LANG_KEY);
  if (SUPPORTED.includes(saved)) _lang = saved;
} catch {}

// Whether the player has ever made an explicit choice (drives the first-run picker).
export function hasLangChoice() {
  try { return SUPPORTED.includes(localStorage.getItem(LANG_KEY)); } catch { return false; }
}

export function getLang() { return _lang; }

export function setLang(lang) {
  if (!SUPPORTED.includes(lang)) return;
  _lang = lang;
  try { localStorage.setItem(LANG_KEY, lang); } catch {}
  document.documentElement.lang = lang;
}

// Translate a key, with optional {placeholder} interpolation.
// Falls back to English, then to the raw key, so a missing string is visible
// rather than blank.
export function t(key, vars) {
  let s = (STRINGS[_lang] && STRINGS[_lang][key])
       ?? STRINGS.en[key]
       ?? key;
  if (vars) {
    for (const k in vars) s = s.replace(new RegExp(`\\{${k}\\}`, 'g'), vars[k]);
  }
  return s;
}

// Fill every [data-i18n] / [data-i18n-ph] element under `root` (default: document).
export function applyTranslations(root = document) {
  document.documentElement.lang = _lang;
  root.querySelectorAll('[data-i18n]').forEach(el => {
    el.textContent = t(el.getAttribute('data-i18n'));
  });
  root.querySelectorAll('[data-i18n-ph]').forEach(el => {
    el.setAttribute('placeholder', t(el.getAttribute('data-i18n-ph')));
  });
}
