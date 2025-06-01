using SFML.Graphics;
using SFML.System;
using VoxelGame.Physics;
using VoxelGame.UI;
using VoxelGame.Worlds;

namespace VoxelGame
{
    public static class DebugMenu
    {
        public static void DrawChunkBorders(Chunk chunk)
        {
            if (chunk == null) return;

            DebugRender.AddRectangle(chunk.GetAABB(), Color.Blue, isBorder: true);
        }

        public static void DrawBaseInfo(int drawChunkCount, int entityCount, float deltaTime, float time, Player player)
        {
            DebugRender.AddText(player.Position - Game.GetWindowSizeWithZoom() / 2, $"\n\n\nFps: {(int)(1f / deltaTime)}\n" +
                                                  $"WindowSize: {Game.GetWindowSize()}\n" +
                                                  $"WindowSizeWithZoom: {Game.GetWindowSizeWithZoom()}\n" +
                                                  $"Visible chunks: {drawChunkCount}\n" +
                                                  $"Entities: {entityCount}\n" +
                                                  $"Player position: {player.Position}\n" +
                                                  $"Player health: {player.Health}\n" +
                                                  $"Time: {MathF.Round(time, 3)}.  0.0 — полночь, 0.5 — полдень, 1.0 — снова полночь\n" +
                                                  $"UI count: {UIManager.WindowCount}");

            DebugRender.AddRectangle(new FloatRect(Game.GetMousePosition(), new Vector2f(10, 10)), Color.Magenta);
            DebugRender.AddRectangle(new FloatRect(Game.GetMousePositionByWorld(), new Vector2f(10, 10)), Color.Yellow);
        }

        public static void DrawEntityCollider(List<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                DebugRender.AddRectangle(entity.GetAABB(), Color.Red, isBorder: true);
            }
        }

        public static void DrawCollide(AABB a, AABB b, Color color)
        {
            DebugRender.AddRectangle(a, color, isBorder: true);
            DebugRender.AddRectangle(b, color, isBorder: true);
        }
    }
}