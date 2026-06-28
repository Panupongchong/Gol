import { Mini1QuizFactory } from './quizFactory.js';

const TIME_LIMIT     = 5;   // seconds per quiz (matches EndlessGameplayController._timeLimit)
const MAX_QUIZ       = 4;   // matches BaseGameplayController._maxQuiz
const ANSWER_DELAY   = 0.3; // seconds of input lockout after each answer
const FREEZE_DURATION = 10; // seconds the Freeze Time item pauses the timer
const DOUBLE_DURATION = 10; // seconds the Double Score item doubles points

export class GameController {
  constructor(lvData, rng = () => Math.random()) {
    this._factory  = new Mini1QuizFactory(lvData, rng);
    this._playing  = false;
    this._quizList = [];
    this._lvData   = lvData;

    // Duel mode — set before calling startDuelGame()
    this.isDuel        = false;
    this.duelChallenge = null;

    // Rank mode — no items (Double/Shield/Freeze disabled). Set via startRankGame().
    this.isRank        = false;
    this.itemsEnabled  = true;

    // View callbacks — assign before startGame()
    this.onScoreUpdate = null; // (score: number) => void
    this.onComboUpdate = null; // (combo: number) => void
    this.onTimeFill    = null; // (fill: number 0-1) => void
    this.onQuizAdded   = null; // (quiz: Mini1Quiz) => void
    this.onCorrect     = null; // (side: 0|1|2, done: bool) => void
    this.onIncorrect   = null; // (side: 0|1|2) => void
    this.onGameEnd     = null; // (result: object) => void
    this.onLevelUpdate = null; // (lv: number) => void
    this.onItemUpdate  = null; // (id: 'freeze'|'double'|'shield', state: {active, charges?}) => void
  }

  startGame() {
    this._lv         = 1;
    this._score      = 0;
    this._combo      = 0;
    this._countCombo = 0;
    this._bonus      = 0;
    this._life       = 1;
    this._miss       = 0;
    this._hit        = 0;
    this._timeLeft   = TIME_LIMIT + 0.7;
    this._quizList   = [];
    this._playing    = true;
    this._locked     = false;
    this._lastTime   = performance.now();

    // Items — 3 free per run, one of each. Shield is passive (active from the
    // start). Freeze and Double each have a single charge the player triggers
    // via useFreeze() / useDouble(). In Rank mode (itemsEnabled = false) the
    // player gets none of them.
    this._shieldActive   = this.itemsEnabled;
    this._freezeCharges  = this.itemsEnabled ? 1 : 0;
    this._frozen         = false;
    this._freezeRemaining = 0;
    this._doubleCharges   = this.itemsEnabled ? 1 : 0;
    this._doubleActive    = false;
    this._doubleRemaining = 0;

    this._generatePlay();
    this._loop();
  }

  startDuelGame(challenge) {
    this.isDuel        = true;
    this.duelChallenge = challenge;
    this.startGame();
  }

  // Rank mode — competitive run with no items.
  startRankGame() {
    this.isRank       = true;
    this.itemsEnabled = false;
    this.startGame();
  }

  // --- Input entry points (called by game.html event handlers) ---

  answerLeft()  { this._answer(0); } // tap left  → claim left side is heavier
  answerRight() { this._answer(1); } // tap right → claim right side is heavier
  answerSwipe() { this._answer(2); } // swipe     → claim equal/balanced
  answerMid()   { this._answer(2); } // mid tap   → claim equal/balanced

  // Freeze Time item — pauses the timer drain for FREEZE_DURATION seconds.
  useFreeze() {
    if (!this._playing || this._freezeCharges <= 0 || this._frozen) return;
    this._freezeCharges--;
    this._frozen          = true;
    this._freezeRemaining = FREEZE_DURATION;
    this.onItemUpdate?.('freeze', { active: true, charges: this._freezeCharges });
  }

  // Double Score item — doubles every point scored for DOUBLE_DURATION seconds.
  useDouble() {
    if (!this._playing || this._doubleCharges <= 0 || this._doubleActive) return;
    this._doubleCharges--;
    this._doubleActive    = true;
    this._doubleRemaining = DOUBLE_DURATION;
    this.onItemUpdate?.('double', { active: true, charges: this._doubleCharges });
  }

  // --- Private ---

  _loop() {
    if (!this._playing) return;
    const now = performance.now();
    const dt  = (now - this._lastTime) / 1000;
    this._lastTime = now;

    // Double Score runs on real time, independent of the (freeze-pausable) quiz timer.
    if (this._doubleActive) {
      this._doubleRemaining -= dt;
      if (this._doubleRemaining <= 0) {
        this._doubleActive = false;
        this.onItemUpdate?.('double', { active: false, charges: this._doubleCharges });
      }
    }

    if (this._frozen) {
      // Freeze Time active — timer drain is paused until the charge expires.
      this._freezeRemaining -= dt;
      if (this._freezeRemaining <= 0) {
        this._frozen = false;
        this.onItemUpdate?.('freeze', { active: false, charges: this._freezeCharges });
      }
    } else {
      this._timeLeft -= dt;
    }
    this.onTimeFill?.(Math.max(0, this._timeLeft) / TIME_LIMIT);

    if (this._timeLeft <= 0) {
      this._endGame();
      return;
    }
    requestAnimationFrame(() => this._loop());
  }

  _answer(side) {
    if (!this._playing || this._locked || this._quizList.length === 0) return;
    this._locked = true;
    setTimeout(() => { this._locked = false; }, ANSWER_DELAY * 1000);
    if (this._quizList[0].checkAnswer(side)) {
      this._onCorrect(side);
    } else {
      this._onIncorrect(side);
    }
  }

  _onCorrect(side) {
    const done = this._quizList[0].next();
    const mult = this._doubleActive ? 2 : 1; // Double Score item

    this._hit++;
    this._score += mult;
    this._countCombo++;

    if (done) {
      this._quizList.shift();
      this._timeLeft = TIME_LIMIT; // reset timer on quiz completion
      this._score += mult;         // bonus point for completing the quiz
    }

    this.onCorrect?.(side, done);
    this.onScoreUpdate?.(this._score);
    this.onComboUpdate?.(this._countCombo);

    if (this._quizList.length === 0) {
      // All 4 quizzes in this wave done → refill
      if (this._lvData[(this._lv + 1).toString()]) this._lv++;
      this.onLevelUpdate?.(this._lv);
      this._generatePlay();
    }
  }

  _onIncorrect(side) {
    this._miss++;
    if (this._countCombo > this._combo) this._combo = this._countCombo;
    this._countCombo = 0;

    this.onIncorrect?.(side);

    // Shield absorbs the hit before life is touched.
    if (this._shieldActive) {
      this._shieldActive = false;
      this.onItemUpdate?.('shield', { active: false });
      return;
    }

    this._life--;
    if (this._life <= 0) this._endGame();
  }

  _endGame() {
    this._playing = false;
    if (this._countCombo > this._combo) this._combo = this._countCombo;

    const result = {
      score:         this._score,
      combo:         this._combo,
      level:         this._lv,
      bonus:         this._bonus,
      isDuel:        this.isDuel,
      isRank:        this.isRank,
      duelChallenge: this.duelChallenge,
    };

    this.isDuel        = false;
    this.duelChallenge = null;
    this.isRank        = false;
    this.itemsEnabled  = true;

    this.onGameEnd?.(result);
  }

  _generatePlay() {
    while (this._quizList.length < MAX_QUIZ) {
      const quiz = this._factory.generateQuiz(this._lv);
      if (!quiz) break;
      this._quizList.push(quiz);
      this.onQuizAdded?.(quiz);
    }
  }
}
