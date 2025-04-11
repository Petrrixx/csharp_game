# 🧛 Vampire Survivors Clone (C# + .NET 9 + Raylib-cs)

A 2D bullet-hell survival game made from scratch in C# using .NET 9.0 and [Raylib-cs](https://github.com/ChrisDill/Raylib-cs). Inspired by [Vampire Survivors](https://store.steampowered.com/app/1794680/Vampire_Survivors/), this game recreates the core loop of auto-attacking, dodging, leveling up, and surviving increasingly intense waves of enemies.

---

## 🎮 Features Implemented

- 🟦 Player movement with WASD
- 🔺 Enemies that chase the player
- 🔫 Auto-firing projectile system
- 💥 Projectile-enemy collision logic
- 🔁 Enemy spawner over time
- 🧪 XP gem drops + pickups
- ❤️ Player health system
- 🧱 Modular architecture for future expansion

> Note: Currently using simple geometric shapes instead of textures or sounds for rapid prototyping.

---

## 🧰 Tech Stack

- .NET 9.0 (C#)
- Raylib-cs (C# bindings for Raylib)
- VSCode or any C# editor
- Custom game loop (no engine)

---

## 🚀 Getting Started

### Prerequisites

- Install [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### Install Raylib-cs package

Run this inside the project folder:

```bash
dotnet add package Raylib-cs --version 4.5.0.2
```

### Build & Run

```bash
dotnet run
```


---

## 📁 Project Structure

VampireSurvivorsClone/  
├── Program.cs - Entry point  
├── Engine/  
│   └── Game.cs - Main game loop  
├── Entities/  
│   ├── Player.cs  
│   ├── Enemy.cs  
│   ├── Projectile.cs  
│   └── XpGem.cs  
├── Assets/ *(optional)*  
└── README.md

---

## 🧱 Upcoming Features

- [ ] Level-up system with random upgrade choices
- [ ] Weapon evolution logic
- [ ] Game over & restart screen
- [ ] Meta progression (gold + unlocks)
- [ ] Texture and sound integration
- [ ] Stage layouts and patterns

---

## 📸 Screenshots

Screenshots will come after visuals are fleshed out — currently using shape-based placeholders.

---

## 👨‍💻 Contributing

This is an open educational project — contributions, ideas, and refactors are all welcome!

---

## 📝 License

MIT License — use it freely, credit appreciated.

