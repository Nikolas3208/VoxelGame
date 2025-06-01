using SFML.Graphics;
using SFML.System;
using VoxelGame.Physics;
using VoxelGame.Resources;

namespace VoxelGame
{
    public static class DebugRender
    {
        private static List<Drawable> objects = new();

        public static bool Enabled = true;
        public static void AddRectangle(float x, float y, float w, float h, Color color, bool isBorder = true)
        {
            if (!Enabled) return;

            var obj = new RectangleShape(new Vector2f(w, h));
            obj.Position = new Vector2f(x, y);

            if (isBorder)
            {
                obj.FillColor = Color.Transparent;
                obj.OutlineColor = color;
                obj.OutlineThickness = 2;
            }
            else
            {
                obj.FillColor = color;
            }
            objects.Add(obj);
        }

        public static void AddRectangle(FloatRect rect, Color color)
        {
            AddRectangle(rect.Left, rect.Top, rect.Width, rect.Height, color);
        }

        public static void AddRectangle(AABB aabb, Color color, bool isBorder = true)
        {
            AddRectangle(aabb.Min.X, aabb.Min.Y, aabb.Max.X - aabb.Min.X, aabb.Max.Y - aabb.Min.Y, color, isBorder);
            AddRectangle(new FloatRect(aabb.Min, new Vector2f(10, 10)), Color.Magenta);
            AddRectangle(new FloatRect(aabb.Max, new Vector2f(10, 10)), Color.Yellow);
        }

        public static void AddVector(Vector2f startPos, Vector2f vec, Color color, float lineLength = 10)
        {
            VertexBuffer line = new VertexBuffer(2, PrimitiveType.Lines, VertexBuffer.UsageSpecifier.Static);
            line.Update(new Vertex[] { new Vertex(startPos, color), new Vertex(startPos + (vec * lineLength), color) });

            objects.Add(line);
        }

        public static void AddText(Font font, Vector2f pos, string mess, Color color, int fontSize)
        {
            var obj = new Text(mess, font);
            obj.FillColor = color;
            obj.CharacterSize = (uint)fontSize;
            obj.Position = pos;
            objects.Add(obj);
        }

        public static void AddText(Vector2f possition, string mess, int fontSize = 16)
        {
            AddText(TextureManager.GetFont("Arial"), possition, mess, Color.White, fontSize);
        }

        public static void AddText(Vector2f possition, string mess, Color color, int fontSize = 16)
        {
            AddText(TextureManager.GetFont("Arial"), possition, mess, color, fontSize);
        }

        public static void AddImage(Texture tx, Vector2f pos)
        {
            var obj = new Sprite(tx);
            obj.Position = pos;
            obj.Scale = new Vector2f(0.7f, 0.7f);
            objects.Add(obj);
        }

        public static void AddImage(Sprite sp, Vector2f pos)
        {
            var obj = sp;
            obj.Position = pos;
            obj.Scale = new Vector2f(0.7f, 0.7f);
            objects.Add(obj);
        }

        public static void Draw(RenderTarget target, RenderStates states)
        {
            if(Enabled)
                foreach(var obj in objects)
                    obj.Draw(target, states);

            objects.Clear();
        }
    }
}
