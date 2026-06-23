// Global Rank-mode leaderboard, backed by the project's Firebase Realtime
// Database via its REST API. No SDK and no auth — submission relies on open
// (but validated) security rules on the `rankBoard` path. See README note in
// result.html for the rules to paste into the Firebase console.

const DB_URL   = 'https://greater-or-less-e0877.firebaseio.com';
const PATH     = 'rankBoard';
const MAX_NAME = 12;

// RTDB server-timestamp sentinel — the server replaces this with its own clock
// at write time, so the value can't be forged by the client. The rules pin both
// `startedAt` and `t` to this, and gate score by the elapsed server time.
const SERVER_TS = { '.sv': 'timestamp' };

// Open a Rank session at game start. Returns the session id (sid) to pass to
// submitScore(). The server stamps startedAt, which bounds the max score.
export async function startSession() {
  const res = await fetch(`${DB_URL}/sessions.json`, {
    method:  'POST',
    headers: { 'Content-Type': 'application/json' },
    body:    JSON.stringify({ startedAt: SERVER_TS }),
  });
  if (!res.ok) throw new Error(`session failed: ${res.status}`);
  const { name: sid } = await res.json();
  return sid;
}

// Push a new score for the given session. Returns the created entry.
export async function submitScore(name, score, sid) {
  if (!sid) throw new Error('missing session');
  const clean = (name || 'ANON').toString().trim().slice(0, MAX_NAME).toUpperCase() || 'ANON';
  const entry = { name: clean, score: Math.max(0, Math.floor(Number(score) || 0)), t: SERVER_TS, sid };
  const res = await fetch(`${DB_URL}/${PATH}.json`, {
    method:  'POST',
    headers: { 'Content-Type': 'application/json' },
    body:    JSON.stringify(entry),
  });
  if (!res.ok) throw new Error(`submit failed: ${res.status}`);
  const { name: id } = await res.json(); // RTDB POST returns { name: "<pushId>" }
  return { id, name: clean, score: entry.score, sid };
}

// Fetch the top `limit` scores, highest first.
export async function fetchTop(limit = 20) {
  // Preferred: let the server order/trim (needs `.indexOn: "score"` in rules).
  try {
    const res = await fetch(`${DB_URL}/${PATH}.json?orderBy=%22score%22&limitToLast=${limit}`);
    if (res.ok) return toSorted(await res.json(), limit);
  } catch { /* fall through to client-side sort */ }

  // Fallback: pull everything and sort here (works even without the index).
  const res = await fetch(`${DB_URL}/${PATH}.json`);
  if (!res.ok) throw new Error(`fetch failed: ${res.status}`);
  return toSorted(await res.json(), limit);
}

function toSorted(data, limit) {
  if (!data) return [];
  return Object.entries(data)
    .map(([id, v]) => ({ id, name: v?.name ?? 'ANON', score: Number(v?.score) || 0, t: Number(v?.t) || 0 }))
    .sort((a, b) => b.score - a.score || a.t - b.t)
    .slice(0, limit);
}
