using Raylib_cs;
using VampireSurvivorsClone.Engine;

Raylib.InitWindow(1280, 720, "Vampire Survivors Clone");
Raylib.SetTargetFPS(60);

Game game = new Game();

while (!Raylib.WindowShouldClose())
{
    game.Update();
    game.Draw();
}

Raylib.CloseWindow();
