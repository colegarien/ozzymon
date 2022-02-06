using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ozzymon
{
    internal class Sprite
    {

        public Vector2 offset = new Vector2(0, 0);
        public Vector2 pos = new Vector2(0, 0);
        Texture2D texture;
        public float fade = 1;
        public float scale = 1f;
        public Sprite(Texture2D t, Vector2 p)
        {
            texture = t;
            offset = new Vector2(t.Width / 2, t.Height);
            pos = p;
        }
        public Sprite(Texture2D t, Vector2 p, float f)
        {
            texture = t;
            offset = new Vector2(t.Width / 2, t.Height);
            pos = p;
            fade = f;
        }
        public Sprite(Texture2D t, Vector2 p, float f, float s)
        {
            texture = t;
            offset = new Vector2(t.Width / 2, t.Height);
            pos = p;
            fade = f;
            scale = s;
        }

        public int getWidth()
        {
            return texture.Width;
        }
        public int getHeight()
        {
            return texture.Height;
        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(texture, pos - offset, new Rectangle(0, 0, texture.Width, texture.Height), new Color(fade, fade, fade), 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            //batch.Draw(texture, pos, new Color(fade,fade,fade,fade));
            batch.End();
        }
        public void Draw(SpriteBatch batch, Color clr)
        {
            batch.Begin();
            batch.Draw(texture, pos - offset, new Rectangle(0, 0, texture.Width, texture.Height), clr, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            //batch.Draw(texture, pos, new Color(fade,fade,fade,fade));
            batch.End();
        }
        public void Draw(SpriteBatch batch, float f)
        {
            batch.Begin();
            batch.Draw(texture, pos - offset, new Rectangle(0, 0, texture.Width, texture.Height), new Color(fade, fade, fade, fade), 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            //batch.Draw(texture, pos, new Color(fade,fade,fade,fade));
            batch.End();
        }
    }
}
