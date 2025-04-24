# Game Design Document

## 1. High‑Level Concept

### 1.1 Game Title

- The Lost Fleet
  - Original Name, but used on other games
- Echoes of the Far Rim
  - I like this one a lot
  - Retains the sense of isolation but sidesteps direct overlap.
- Fleet of the Lost

### 1.2 Genre & Core Gameplay Pillars

**Hybrid Strategy Genre**

- *Real‑Time Tactical Maneuvering* — minute‑to‑minute ship positioning and formation shifts occur in continuous time, with a **pause‑and‑command** feature that lets players issue complex orders without twitch pressure.
- *Turn‑Based Command Phase* — when the player commits orders, the game resolves weapon salvos, abilities, and movement in discrete "ticks," enabling deterministic outcomes and deep planning.
- *Exploration‑Driven Roguelite Structure* — procedurally generated star sectors, fog of war, and limited resources create high stakes and strong replayability.
- *Narrative Survival Focus* — the fleet is stranded in hostile space; every choice balances progress toward home against attrition.

**Core Gameplay Pillars**

1. **Tactical Fleet Command** – Multi‑ship control, formation systems, shield facings, and synchronized ability timings define combat depth.
2. **Exploration & Navigation** – Scanning, charting jump lanes, deciphering anomalies, and securing safe harbor.
3. **Resource & Crew Management** – Fuel, munitions, spare parts, morale, and command XP; scarcity drives meaningful trade‑offs.
4. **Ship & Technology Progression** – Modular hull upgrades, weapon research trees, and crew specialization enable evolving strategies.
5. **Emergent Threat Ecosystem** – Alien factions, spaceborne hazards, and environmental effects adapt to player actions, ensuring no two runs feel the same.
6. **Persistent Consequences** – Hull breaches, crew casualties, and reputation shifts carry forward, raising tension and rewarding foresight.

### 1.3 Vision Statement (“The X of Y” Elevator Pitch)

**Vision Statement — “The Tactical Survival of a Stranded Armada”**  
*A rogue‑lite fusion of real‑time fleet tactics and turn‑based hex exploration where every decision carries existential weight. Marshal a handful of battered starships, out‑think adaptive alien threats, and gamble scarce resources to chart a path home—turning desperation into mastery one harrowing jump at a time.*

### 1.4 Target Audience & Platforms

**Primary Platform**

- **PC (Windows & Linux via Steam, GOG, Epic)** — mouse‑keyboard is the reference control scheme; optional game‑pad fallback supported. MacOS release considered post‑launch once Metal performance is confirmed.

**Secondary Distribution Goals**

- **Steam Deck “Playable”** certification with responsive, scalable UI (1280 × 800 baseline).
- Console ports (Xbox Series X | S, PlayStation 5) evaluated after break‑even on PC.

**Core Audience Segments**

| Segment | Archetypal Player | Genre Touchstones | Engagement Drivers |
|---------|------------------|------------------|--------------------|
| Tactical Strategists | 25–45 yo PC gamers who relish pausable real‑time tactics blended with turn‑based planning | *Homeworld*, *Battlestar Galactica Deadlock*, *Company of Heroes* | Formation nuance, deterministic order resolution |
| Roguelite Explorers | Fans of *FTL*, *Into the Breach*, *Slay the Spire* | Procedural runs, meta‑progression | Varied seeds, high replayability |
| Narrative Sci‑Fi Enthusiasts | Readers of hard‑sci novels, *The Expanse* watchers | Story‑driven strategy hybrids | Mystery of the Wall, moral choices |

**Accessibility Commitments**

- Full key rebinding, input dead‑zone tuning, and combat slow‑down slider.
- Font scaling to 200 % and dyslexia‑friendly typeface option.
- Color‑blind palettes (deuteranopia, protanopia, tritanopia) and high‑contrast UI icons.
- Screen‑reader tags for critical HUD elements and narrated menus.

**Community & Longevity Hooks**

- **Data‑driven mod support** (JSON ship stats, Lua event scripts) with Steam Workshop integration.
- Daily challenge seeds and season‑style content drops to sustain engagement.

---

## 2. Gameplay Overview

### 2.1 Player Objectives & Win/Loss Conditions

**Primary Campaign Objective**  
Chart a navigable route from the stranded sector to the nearest **Home‑Beacon Jump Gate** and execute the jump back to known space with at least one operational flagship.

**Win Conditions**
- Enter the final sector exit hex and survive the jump‑sequence event.
- Any bonus objectives completed (see Side Objectives) add to final score but are *not* mandatory for victory.

**Fail Conditions**
- **Total Fleet Loss:** All player‑controlled ships destroyed.
- **Critical Resource Collapse:** Fleet becomes unable to move or fight for three consecutive turns due to zero fuel/reactor integrity (optional iron‑man setting).

**Side Objectives (Optional per Run)**
| Category | Example Goal | Reward Type |
|----------|--------------|-------------|
| Distress Signals | Rescue trapped civilian convoy within 5 turns | Supplies + morale boost |
| Tech Salvage | Board derelict research cruiser | Random advanced blueprint |
| Faction Contracts | Eliminate pirate raider nest | Reputation credit + rare parts |
| Artifact Hunts | Decode star‑chart riddles to locate relic vault | **Game‑changer Artifact** |
| Crew Arcs | Fulfill officer’s personal quest | Permanent commander trait |

Side objectives deepen narrative context, boost score, and meaningfully power‑spike the fleet but remain optional—allowing risk‑averse players to race for the exit while optimizers chase high‑value detours.

**Partial Fail & Recovery Mechanics**
- Flagship destruction triggers a one‑time emergency command transfer to the next highest‑ranked vessel.
- Shipyard hexes allow rebuilding lost hulls (high cost), giving comeback routes short of total defeat.

**Session vs. Campaign Framing**
- One *run* spans 3–5 procedural sectors (~90–120 min); successful return unlocks higher difficulty tiers.
- Meta‑progression: collected Artifacts and commander XP persist between runs, motivating repeat attempts.*

### 2.2 Core Loop

```
Turn‑Based Map Phase → Event Hex Trigger → Real‑Time Encounter → Reward & Refit → Next Strategic Decision
```

| Phase | Player Action | System Resolution | Outcome |
|-------|---------------|-------------------|---------|
| **Plan** | Plot course across hex grid; allocate fuel & sensor pings | Pathfinding validation, intel roll | Risk profile established |
| **Engage** | Command fleet in real‑time combat or narrative decision | Combat physics & branching logic | Victory, retreat, or loss |
| **Reward** | Accept or select loot | Weighted drop table by difficulty | **New Ship**, **Tech Blueprint**, **Supplies**, or **Artifact** |
| **Refit / Progress** | Install upgrades, redistribute crew, repair | Stat & ability updates | Power curve climbs |

*30‑second loop* – micro‑maneuvers during battle.  
*5‑minute loop* – one hex traversal → encounter → loot/repairs.  
*30‑minute loop* – clear a sector ring and unlock next jump gate.

### 2.3 Key Mechanics

| System      | Brief Description | Player Skill/Stat Driven? |
| ----------- | ----------------- | ------------------------- |
| Strategic Movement | Turn‑based navigation across a **hexagonal sector grid**; each hex represents a star system or deep‑space region. Movement consumes fuel, advances time, reveals fog‑of‑war, and may trigger encounters. | Hybrid (player planning & ship stats) |                   |                           |
| Tactical Engagements | **Real‑time (with pause) fleet battles** that occur when entering designated event hexes or ambush scenarios; uses formation commands, facing shields, and ability cooldowns. | Player Skill + Crew/Tech Stats |                   |                           |
| Economy | Resource acquisition & expenditure: fuel, supplies, repair parts; trade or salvage drives exploration tempo. | Strategic planning |                   |                           |
| Crafting    | On‑ship fabrication of ammo, drones, and hull mods using salvaged materials. | Crew skill checks |
| Artifacts | Rare **game‑changer relics** from high‑tier events; alter core rules (e.g., shield inversion, instant‑jump thrusters, time‑dilation field). Stackable for run‑defining builds. | Passive modifiers chosen by player |
| Multiplayer |                   |                           |

### 2.4 Controls & User Input

*\<Primary input schemes, accessibility options>*

---

## 3. Narrative & World‑Building

### 3.1 Setting & Theme

**Era & Civilization**  
Humanity’s *Interstellar Commonwealth* spans thousands of settled worlds, linked by Alcubierre‑style jump corridors and dominated by megacorporate logistics guilds. Routine spaceflight, modular habitation, and cyber‑spliced cultures form a cosmopolitan baseline.

**The Frontier & “The Wall”**  
At the rim of charted space lies a vast barrier of quantum‑flux anomalies—the **Wall**—through which no conventional jump drive can pass. Sensor ghosts, gravitational shear, temporal echoes: explorers who probe too close rarely return.

**Tone & Themes**  
- *Isolation & Discovery* – waking beyond the Wall with no contact home.  
- *Corporate Hubris* – evidence that the employer’s clandestine project triggered the catastrophe.  
- *Ruins & Relics* – graveyards of shattered hulls juxtaposed with eerily pristine alien megastructures.  
- *Survival vs. Curiosity* – every scrap salvaged may fuel escape—or unlock deeper truths.

**Aesthetic Touchstones**  
Hard‑sci silhouettes, cold color palette punctuated by ion‑storm neons, derelict geometries reminiscent of *Event Horizon*, *Homeworld*, and Ian McQue concept art.

### 3.2 Story Synopsis

**Act I – Through the Breach**  
Contracted by *Orion Dynamics Ltd.* to survey mineral anomalies near the Wall, the player’s escort fleet is caught in a destabilizing rift. They awaken adrift on the far side amid a **corp‑branded graveyard**, with long‑dead distress beacons orbiting a dormant alien station. Initial objectives: assess damage, secure supplies, and locate surviving crew.

**Act II – Echoes in the Void**  
Exploration reveals the station is partially powered and running cryptic subspace pulses. Logs from wrecks implicate Orion Dynamics in experimental “fold‑tunneling” tech that breached the Wall decades prior—dooming multiple expeditions. Adaptive biomechanical drones (“Scavengers”) begin harassing the fleet, repurposing wreckage to evolve. The player must salvage tech, forge uneasy truces with other stranded factions, and decipher the station’s control lattice.

**Act III – Exodus Protocol**  
By restoring key station subsystems, the fleet can repoint its ancient gate array toward Commonwealth coordinates—but doing so will awaken the station’s failsafe guardian AI and trigger a last, massive hostile response. Choices made earlier (rescuing allies, hoarding artifacts, or cannibalizing wrecks) determine whether the exodus battle is a desperate sprint, a coordinated flotilla offensive, or a stealth infiltration that hijacks the AI itself.

**Twists & Branches**  
- Discovery that some Scavenger drones contain human neuro‑prints—crew of earlier corporate runs subsumed into the swarm.  
- The Wall is a containment membrane erected by a long‑dead precursor species; ruptures risk unleashing the Scavengers into known space.  
- Optional ending: sacrifice the jump home to seal the breach permanently, earning a bittersweet victory.

**Narrative Delivery**  
Crew banter, collectible black‑box logs, and dynamic event chains tied to hex sectors ensure story beats emerge organically with roguelite variability.

### 3.3 Characters

| Name | Role | Motivation | Mechanics Impact |
| ---- | ---- | ---------- | ---------------- |

---

## 4. Level & Content Design

### 4.1 Level Progression Structure

Procedurally generated **hex‑grid star sectors**. The overworld map comprises concentric "rings" of hexes leading toward a distant exit beacon. Sector topology, hazard density, and faction presence vary per run, blending roguelite unpredictability with strategic planning.

- **Exploration Layer:** Turn‑based hex traversal with limited scanning range.
- **Encounter Layer:** Real‑time battles or narrative events triggered by specific hex types (e.g., derelict, anomaly, hostile fleet).
- **Progression Gates:** Nebula walls, warp rifts, or blockade fleets force the player to weigh risk vs. resource expenditure before advancing.*

### 4.2 Environmental Puzzles & Challenges

*\<Unique hooks per biome/zone>*

### 4.3 Enemy & Encounter Design

| Enemy Archetype | Behavior | Counterplay |
| --------------- | -------- | ----------- |

---

## 5. Art & Audio Direction

### 5.1 Visual Style Guide

*\<Reference titles, color palette, UI philosophy>*

### 5.2 Character & Environment Art Requirements

### 5.3 Animation Principles

### 5.4 Audio & Music Goals

---

## 6. Technical Specifications

### 6.1 Engine & Tooling

*\<Unity, Unreal, Godot, custom…>*

### 6.2 Target Performance Metrics

*\<FPS, load times, memory budget>*

### 6.3 Online / Networking Architecture

---

## 7. Production Plan

### 7.1 Team Roles & Responsibilities

### 7.2 Milestones & Timeline (Gantt Overview)

### 7.3 Budget & Resource Allocation

---

## 8. Monetization & Market Strategy

### 8.1 Business Model (Premium, F2P, DLC, Live‑Ops)

### 8.2 Competitive Analysis & Positioning

### 8.3 Community & Marketing Beats

---

## 9. Accessibility & Inclusivity Considerations

---

## 10. Risks & Mitigations

| Risk | Probability | Impact | Mitigation |
| ---- | ----------- | ------ | ---------- |

---

## 11. Appendices

- Prototypes & Mock‑ups links
- Reference material & inspiration board
- Glossary of in‑game terminology

