# Overrun

Overrun is a top-down 2D survival game in which you enter a ruined desert, fight endless waves of monsters, collect coins, and buy equipment to survive for as long as possible.

## Demo

- [Download the latest build](https://github.com/4ndr3jS/Overrun/releases/latest)
- [Watch the gameplay demo on YouTube](https://youtu.be/watch?v=oXHWty-i8YU)

## What is the project?

Overrun is a pixelized wave-survival game built with Unity and C#. You are the player who begins in a safe village, speaks with its villagers, prepares equipment, and goes through a gate into a combat area. Once the waves begin, each completed wave is followed by a bigger one.

The main gameplay loop is:

1. Go into the desert and fight off endless monsters.
2. Collect the coins dropped by defeated enemies.
3. Buy weapons, healing items, stamina items, and bombs from the merchant.
4. Arrange and use items through the inventory and eight-slot hotbar.
5. Return to the desert and try to reach a higher wave.

## Why did I build it?

I wanted to build a complete game rather than a website (I already made tons of those). For a long time I didn't make any games so I thought this would refresh me a bit.

I also wanted the game to be easy to understand while still giving the player a reason to improve. The rules are simple defeat enemies, collect coins, and upgrade but every wave increases the pressure and makes the player's equipment choices more important.

## Inspiration

Overrun was inspired by wave-survival games and top-down pixel RPGs. I especially liked the loop of leaving a safe area, taking on a dangerous challenge, earning resources, and returning better prepared. I used that idea a little, then built my own combat, progression, shop, inventory, and endless wave systems around it. The look was inspired by Stardew Valley.

## Theme: Endless

Overrun was created for the **Endless** theme.

It belongs in Endless because the game has no final wave or end. After the player defeats every monster in a wave, the next wave begins immediately. Every new wave contains more enemies, while animation curves increase enemy health, damage, and movement speed as the wave number rises. The game continues forever as long as the player survives.

## How to play

| Input | Action |
| --- | --- |
| `WASD` or arrow keys | Move |
| `E` | Interact with NPCs, Shop, and chests |
| Left click | Punch, attack with the selected weapon, or use the selected bomb |
| `1` - `8` or click a hotbar slot | Select or use a hotbar item |
| Drag and drop | Move items between inventory and hotbar slots |
| Right click an item stack | Split the stack |
| `Tab` | Open or close the inventory/player menu |
| `Escape` | Open settings or close the current menu |

## How to test it

1. Open the [GitHub Release](https://github.com/4ndr3jS/Overrun/releases/latest).
2. Download the ZIP file from **Assets**.
3. Extract the ZIP.
4. Run `Overrun.exe`.
5. IF WINDOWS PROTECTION APPEARS: Don't worry it happens because the build is not digitally signed. To continue select **More info**, then **Run anyway**.


## Built with

- Unity 6
- C#
