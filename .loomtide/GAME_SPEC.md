# Game Spec

> Auto-derived from `ACCEPTANCE.json` by `loomtide plan` — a human-readable mirror of the
> machine-checkable contract. Enrich it during the Design Target Phase; the contract stays
> the source of truth for the gates.

## One-line pitch

A platformer-2d game ("CountTheFruits") built with Loomtide on unity. Objective: **all-fruit**.

## Core mechanics

- a player character
- terrain / platforms
- collectibles
- hazards
- a level goal
- audio cues

## Win / lose condition

**Win:** all-fruit. Phase F reconcile: the build's all-fruit rule (score >= totalCoins) is now the ACCEPTED win rule. The mock framed the flag as the goal, but collecting all fruit is the consciously-adopted primary objective; reaching the flag stays as a secondary win trigger. The earlier reach-flag/all-fruit mismatch is resolved in favor of all-fruit.

## Feel targets

| metric | target |
|---|---|
| runSpeed | 7 u/s |
| jumpApex | 2.2 u |
| timeToApex | 325 ms |
| shortHopApex | 0.72 u |
| dashDistance | 2.8125 u |
| dashTime | 0.15 s |
| dashCooldown | 0.4 s |
| coyoteTime | 0.1 s |
| jumpBuffer | 0.1 s |

## HUD

- `score` — Fruit counter (top-left)
- `timer` — Run timer (top-center)
- `lives` — Heart row (top-right)

## Framing

- 16:9, static camera

## Audio cues

- jump, dash, collect, hit, bounce, win
