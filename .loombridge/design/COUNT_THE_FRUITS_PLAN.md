# Count the Fruits — Build Plan

> Bespoke plan for a **Kidopia-style tap-to-count mini-game**, authored because the
> Loomtide runtime ships only a `platformer-2d` genre pack. The candy **Design Target**
> is locked (`.loombridge/design/hero-shot.{html,png}`, sha256 `9932a500…`); this plan is
> the slice + asset roadmap the build follows.

## Verification status (read this first)

This game is **intentionally off the Loomtide verified pipeline.** The runtime's
`REQUIRED_ASSET_ROLES` and slice DAG are hard-wired to platformer mechanics
(player-character, platform-tiles, one-way-platform, hazard, jump/dash feel,
reachability). A static tap-to-count screen has none of those, so `loomtide verify`
will **not** go green for this game — confirmed and accepted by the developer.
`STATE.md` still reads `genre=platformer-2d`; that is vestigial. The polish bar here
is the **frozen hero shot + this plan**, self-checked by eye and screenshot, not by the
Tier-1 gates. (To get a *green* counting-game verification later, a `count-2d` genre
pack would have to be added to the runtime first.)

---

## 1. Game design

**Pitch.** A bright, poppy counting game for young kids. Each round poses a question
("How many apples?"), scatters N glossy fruit on screen, and the child taps each fruit
(it pops with a "+1") then picks the matching number from three big buttons. Correct →
celebration + star; wrong → gentle shake and a retry. Five rounds, then a "Great job!"
summary.

**Round loop.**
1. Mascot poses the question in the banner; fruit count N is chosen for the round.
2. N fruit spawn in a non-overlapping scattered layout.
3. Child taps fruit → each pops (squash-stretch), sparkles, floats "+1", live count ×N rises.
4. Three number buttons appear: the correct N plus two near-miss distractors.
5. Tap a button → correct: green check, confetti, chime, +1 star, advance; wrong: red
   shake, soft buzz, buttons stay, encourage retry (no fail state — kids never "lose").
6. After 5 rounds → summary screen with stars earned + a big replay button.

**Difficulty ramp.** Round 1–2: N = 3–5, distractors ±1. Round 3–4: N = 5–7. Round 5:
N = 6–9, distractors closer. Always within easy visual subitizing/counting range.

**Tone rules (kid-safe).** No timers pressuring the child (the hero-shot timer is
decorative/removed), no lose state, no scary elements. Every interaction is rewarding.

---

## 2. Visual identity (LOCKED — from the approved hero shot)

| Token | Value |
|---|---|
| Palette | apple `#ff5d72`, gold `#ffce4f`, mint `#5fe0bd`, peach `#ffb27b`, lavender `#b79cff`, sky `#a8e6ff`, ground `#56cf89`, ink `#46365e` |
| Display font | **Fredoka** (rounded) |
| UI font | **Nunito** |
| Style | Soft rounded vector — glossy shapes, thick white outlines, chunky drop-shadows, no pixels |
| Frame | 1280×720, 16:9 |

---

## 3. Asset manifest (source = **generated**, to match the frozen hero shot)

Every asset is generated/exported in soft rounded vector to match the hero shot. No
registry/pixel assets.

| Role | Asset | Notes / states |
|---|---|---|
| `mascot` | Counting guide character | idle-bob, blink, cheer (correct), encourage (wrong) |
| `fruit-apple` | Primary fruit | idle + 6-frame pop/burst; gloss, leaf, stem, soft shadow |
| `fruit-set` *(opt)* | cherry / banana / orange | for round variety; same render language |
| `basket-icon` | Live-count chip icon | HUD |
| `number-button` | Big rounded button | 3 color skins (peach/mint/lavender) × states: idle, press, correct, wrong |
| `check-badge` | Green correct check | pops on correct |
| `question-banner` | Rounded card frame | holds prompt text + mascot |
| `progress-star` | Star | filled (gold) + empty (grey) |
| `bg-sky` | Gradient sky | parallax layer 0 |
| `bg-clouds` | 3 soft clouds | slow drift |
| `bg-sun` | Warm sun glow | static |
| `bg-hills` | 2 hill silhouettes | parallax layers 1–2 |
| `bg-ground` | Grassy ground strip | foreground |
| `vfx-sparkle` | Gold star particle | tap burst |
| `vfx-confetti` | Multi-color confetti | celebration |
| `fonts` | Fredoka + Nunito | imported to Unity TMP |
| `audio-sfx` | pop, chime-correct, buzz-wrong, round-jingle | procedural or generated |
| `audio-music` | gentle music bed | loopable, soft |

---

## 4. Slice roadmap (ordered, dependency-aware)

Built one polished slice at a time; each verified by eye + screenshot against the hero
shot before the next.

| # | Slice id | Title | Depends on | Done when |
|---|---|---|---|---|
| 1 | `scene-frame` | Candy background + camera | — | 16:9 frame, sky/sun/clouds/hills/ground, gentle parallax drift; matches hero-shot bg |
| 2 | `fruit-system` | Fruit prefab + scatter spawn | 1 | N glossy apples spawn non-overlapping in the play area per round |
| 3 | `tap-feel` | Tap → pop / +1 / sparkle / count | 2 | Tapping a fruit pops it (squash-stretch), sparkles, floats +1, increments live count, no double-count, pop SFX |
| 4 | `hud-prompt` | Banner + mascot + count chip + stars | 1 | Question banner, idle-bobbing/blinking mascot, basket ×N chip, 5-star progress row |
| 5 | `answer-buttons` | 3 number buttons + feedback | 3, 4 | Chunky buttons (idle bounce, press squish); correct → check+confetti+chime; wrong → shake+buzz, retry |
| 6 | `round-loop` | Game manager / 5 rounds / scoring | 5 | Poses question, randomizes N + distractors, evaluates, awards star, advances, ends after 5 |
| 7 | `audio` | SFX + music bed | 3 | All cues wired + soft looping music |
| 8 | `celebration-polish` | Confetti, juice curves, mascot reactions | 6 | Global pop-in, screen celebration on correct/round-win, eased everything |
| 9 | `end-summary` | Final summary + replay | 6, 8 | "Great job!" screen, stars earned, big replay button restarts cleanly |

**Critical path:** 1 → 2 → 3 → 5 → 6 → 8 → 9. Slices 4 and 7 run alongside.

---

## 5. Build approach

- **Engine/scaffold:** Unity 2D via the `unity-2d-game` skill + Loomtide MCP bridge
  (the Unity project does not exist yet — slice 1 scaffolds it).
- **Polish:** `game-polish-2d` skill for HUD, animation, juice, audio, end-state.
- **Assets:** generated in the locked soft-rounded-vector language; imported as
  sprites/TMP fonts/audio to match the hero shot per the manifest above.
- **Self-check loop:** after each slice, screenshot the Game view and compare to the
  hero shot; iterate until it matches before advancing.

## 6. Decisions

- **Fruit variety:** 🔒 **Apples only** — every round uses the glossy hero-shot apple.
  One fruit asset, polished to a high bar. (`fruit-set` role dropped from the manifest.)
- Voice-over / spoken numbers? Out of scope by default.
- Music: generated loop (default).
