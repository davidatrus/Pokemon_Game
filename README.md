
## 🎮 Controls

| Action                                | Key         |
|---------------------------------------|-------------|
| Pause Menu                            | `Enter`     |
| Confirm / Interact / Progress Dialog  | `Z`         |
| Go Back / Cancel                      | `X`         |
| Move Character                        | Arrow Keys  |

---

## 🔧 Features Implemented

### 1. *Battle System*
A core mechanic of the game:
- Battle both wild Pokémon and NPC trainers.
- Seamless scene transitions using fade animations.
- Custom move animations for attacks, damage, and experience gain.
- Trainers trigger battles if the player enters their line of sight (using Unity OnTrigger).
- Defeating trainers grants money and custom post-battle dialog.
- Unique backgrounds for land and water battles.
- Full battle menu system: Attack, Use Bag, Switch Pokémon, or Run.
  - You cannot run or catch Pokémon in trainer battles.
  - In wild battles, you can run or attempt to catch Pokémon.

### 2. *Catching Mechanics*
- Catch wild Pokémon using various Pokéballs.
- Catch rates follow the official Pokémon formula.
- Each ball type has unique catch success weighting.
- Custom animations for throwing, shaking, catching, and failure.
- Built using the DOTween animation library.
- Accessible during battle via the Bag → Arrow Keys → Pokéballs → Select desired ball.

### 3. *Pokémon Inventory / Party Screen*
- View your Pokémon, their levels, and current HP.
- Access a Pokémon's summary and moves from both overworld and battle.
- In the summary screen:  
  - Use `Up/Down` to switch Pokémon  
  - Use `Left/Right` to cycle through their moves  

### 4. *Shop System*
- Buy and sell items from NPC shopkeepers.
- Located in the house to the left of the starting area (right-side NPC = shopkeeper, left = healer).
- Players earn money from defeating trainers and can spend it in shops.

### 5. *Quest System*
- NPCs can initiate quests that reward items or gate story progression.
- Example:  
  - An NPC asks for a Revive → triggers item spawn → upon return, you’re rewarded with HM01 (Cut).
  - Another quest prevents you from leaving town until a professor encounter is completed. After dialog, the professor gives you a Mudkip so you can proceed.

### 6. *Interactable Overworld*
- Fully implemented **Surf** and **Cut** mechanics with confirmation prompts.
- One-way ledges, tall grass, and water areas function as encounter zones.
- Pokémon populations are assigned via Unity’s `MapArea` script:
  - Define encounter percentage, level range, and species per area.
> 📝 *To use Cut, ensure at least one of your Pokémon has been taught the move.*

### 7. *Pokémon System*
- Each Pokémon is built as a ScriptableObject with stats, types, EXP curves, evolution logic, and learnable moves.
- Type advantages and weaknesses follow original Pokémon logic.
- Pokémon evolve through leveling or item use, with animations on evolution.
- Healing and status curing available through item usage.

**Move System:**
- Moves are categorized as Physical or Special and include:
  - Power, accuracy, priority level, secondary effects (e.g. burn, poison, freeze).
- Moves can affect self or opponent and include boost, debuff, or status effects.
- Pokémon can learn moves via level-up or TMs/HMs.
- Each Pokémon can hold up to 4 moves; level-up prompts for replacing old moves.
- In trainer and wild battles, move usage is randomly chosen from the available 4.
> 💡 *Future improvement: smarter AI logic that prioritizes super-effective moves or finishing blows.*

### 8. *Additive Scene Loading*
- Only scenes connected to the player’s current location are actively loaded.
- Scene transitions handled using Unity’s additive loading system.
- Scenes are modular and defined using a `SceneDetails` script.
- Centralized in a `Gameplay` scene which dynamically pulls connected areas.

### 9. *Item System*
- Fully functioning inventory with categories:  
  - Pokéballs  
  - Healing Items  
  - TMs/HMs  
  - Evolution Items
- Access items via `Enter → Bag → Arrow Keys` to switch categories → `Z` to use.
- Invalid uses trigger an in-game warning message.

### 10. *Audio*
- Dynamic background music for overworld and battle.
- Sound effects for attacking, catching, and item acquisition.

### 11. *Save / Load System*
- In-game save and load via `Enter → Save` and `Enter → Load`.
- Saves persist Pokémon status, items, quest progress, inventory, money, and more.
> ⚠️ *Main menu Load functionality is not fully implemented—load only works from in-game.*

- Built using Unity’s `ISavable` pattern and GameDev.tv’s save/load framework.

### 12. *Dialog Manager*
- Handles all in-game text interactions with NPCs and battle announcements.

### 13. *Unity Editor Tools & Quality-of-Life Improvements*
- All Pokémon, moves, and items are created as ScriptableObjects for modular design.
- Custom inspector UI using `PropertyDrawer` for a cleaner development experience.
- **Cutscene Editor**:
  - Add and organize narrative events (dialog, movement, fades, item triggers).
  - Action types include: Dialog, Move, Turn, Teleport, Enable/Disable Object, NPC Interact, Fade In/Out.
- State machine architecture:
  - `Enter`, `Execute`, and `Exit` logic
  - Push/pop functionality
  - Exposed globally via namespace
- Dynamic text boxes for shops, party screens, and move summary UI.
