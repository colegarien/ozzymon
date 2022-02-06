using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ozzymon
{
    internal class AnimatedSprite
    {
        public Vector2 offset = new Vector2(0, 0);
        public Vector2 pos = new Vector2(0, 0);
        int frames = 0;
        int fps = 3;
        Texture2D texture;
        double time = 0;
        int currentFrame = 0;
        int currentRow = 0;
        int shifter = 0;
        int width;
        int rows = 1;
        bool active = true;
        public float fade = 1;
        public float scale = 1f;
        int height;
        public bool tick = false;
        public AnimatedSprite(Texture2D t, Vector2 p, int f)
        {
            texture = t;

            pos = p;
            frames = f;
            width = t.Width / f;
            height = t.Height;

            offset = new Vector2(width / 2, height);
        }
        public AnimatedSprite(Texture2D t, Vector2 p, int c, int r)
        {
            texture = t;

            pos = p;
            frames = c;
            rows = r;
            width = t.Width / c;
            height = t.Height / r;

            offset = new Vector2(width / 2, height);
        }
        public AnimatedSprite(Texture2D t, Vector2 p, int f, float s)
        {
            texture = t;

            pos = p;
            frames = f;
            width = t.Width / f;
            height = t.Height;
            scale = s;

            offset = new Vector2(width / 2, height);
        }
        public AnimatedSprite(Texture2D t, Vector2 p, int c, int r, float s)
        {
            texture = t;

            pos = p;
            frames = r;
            rows = r;
            width = t.Width / c;
            height = t.Height / r;
            scale = s;

            offset = new Vector2(width / 2, height);
        }
        public void setShifter(int s)
        {
            shifter = s;
        }
        public void shiftOffset(Vector2 shift)
        {
            offset += shift;
        }
        public int getFrame()
        {
            return currentFrame;
        }
        public int getRow()
        {
            return currentRow;
        }
        public void Stop()
        {
            active = false;
        }
        public void Play()
        {
            active = true;
        }
        public void setFPS(int n) { fps = n; }
        public void setFrame(int f)
        {
            if (f < frames && f >= 0)
            {
                currentFrame = f;
            }
        }

        public void setFrame(int f, int r)
        {
            if (f < frames && f >= 0)
            {
                if (r < rows && r >= 0)
                {
                    currentFrame = f;
                    currentRow = r;
                }
            }
        }

        public void Reset()
        {
            setFrame(0, 0);
        }

        public void Update(GameTime gameTime)
        {
            tick = false;
            if (active == true)
            {

                time += gameTime.ElapsedGameTime.Milliseconds / 10;
                if (time >= fps)
                {
                    tick = true;
                    time = 0;
                    // DOJT FORGET TO CHANGE TO + 1
                    if (currentFrame + 1 < frames)
                        currentFrame++;
                    else if (currentRow + 1 < rows)
                    {
                        currentFrame = 0;
                        currentRow++;
                    }
                    else
                    {
                        currentFrame = 0;
                        currentRow = 0;
                    }
                }
            }

        }

        public void Draw(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(texture, pos - offset, new Rectangle(width * currentFrame + 1, height * currentRow, width + shifter, height), new Color(fade, fade, fade), 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            //batch.Draw(texture, pos, new Rectangle(width * currentFrame, 0, width, texture.Height), new Color(fade, fade, fade, fade));
            batch.End();
        }

        public void Draw(SpriteBatch batch, Color clr)
        {
            batch.Begin();
            batch.Draw(texture, pos - offset, new Rectangle(width * currentFrame + 1, height * currentRow, width + shifter, height), clr, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            //batch.Draw(texture, pos, new Rectangle(width * currentFrame, 0, width, texture.Height), new Color(fade, fade, fade, fade));
            batch.End();
        }

        public void Draw(SpriteBatch batch, float f)
        {
            batch.Begin();
            batch.Draw(texture, pos - offset, new Rectangle(width * currentFrame + 1, height * currentRow, width + shifter, height), new Color(fade, fade, fade, fade), 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            //batch.Draw(texture, pos, new Rectangle(width * currentFrame, 0, width, texture.Height), new Color(fade, fade, fade, fade));
            batch.End();
        }
    }
}
