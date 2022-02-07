using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Ozzymon
{
    internal class Transition
    {
        Random rnd = new Random();
        List<AnimatedSprite> FadeIn = new List<AnimatedSprite>();
        List<AnimatedSprite> FadeOut = new List<AnimatedSprite>();
        int curIn = 0;
        int curOut = 0;

        Texture2D blankScreen;

        // leaving inbetween entering done
        string state = "done";

        public Transition(ContentManager cm)
        {
            // for inbetween
            blankScreen = cm.Load<Texture2D>("Transitions/blankscreen");

            // All should have ten frames each
            // AKA 2 rows of 5
            FadeIn.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeIn/FadeIn1"), Vector2.Zero, 5, 2));
            FadeOut.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeOut/FadeOut1"), Vector2.Zero, 5, 2));

            FadeIn.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeIn/FadeIn2"), Vector2.Zero, 5, 2));
            FadeOut.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeOut/FadeOut2"), Vector2.Zero, 5, 2));

            FadeIn.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeIn/FadeIn3"), Vector2.Zero, 5, 2));
            FadeOut.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeOut/FadeOut3"), Vector2.Zero, 5, 2));

            FadeIn.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeIn/FadeIn4"), Vector2.Zero, 5, 2));
            FadeOut.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeOut/FadeOut4"), Vector2.Zero, 5, 2));

            FadeIn.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeIn/FadeIn5"), Vector2.Zero, 5, 2));
            FadeOut.Add(new AnimatedSprite(cm.Load<Texture2D>("Transitions/FadeOut/FadeOut5"), Vector2.Zero, 5, 2));

            // so it will be in the upper left corner
            for (int i = 0; i < FadeIn.Count; i++)
            {
                FadeIn[i].offset = Vector2.Zero;
                FadeOut[i].offset = Vector2.Zero;
            }
        }

        public void Start()
        {
            state = "leaving";
            curIn = rnd.Next(FadeIn.Count);
            curOut = rnd.Next(FadeOut.Count);
        }

        public bool isDone()
        {
            return state == "done";
        }
        public bool isInbetween()
        {
            return state == "inbetween";
        }
        private void ResetFades()
        {
            for (int i = 0; i < FadeIn.Count; i++)
            {
                FadeIn[i].Reset();
                FadeIn[i].Stop();
                FadeOut[i].Reset();
                FadeOut[i].Stop();
            }
        }

        public void Update(GameTime gametime)
        {

            // update the right transition
            if (state == "leaving")
            {
                FadeOut[curOut].Play();
                FadeOut[curOut].Update(gametime);
                if (FadeOut[curOut].getFrame() >= 4 && FadeOut[curOut].getRow() >= 1)
                {
                    ResetFades();
                    state = "inbetween";
                }
            }
            else if (state == "entering")
            {
                FadeIn[curIn].Play();
                FadeIn[curIn].Update(gametime);
                if (FadeIn[curIn].getFrame() >= 4 && FadeIn[curIn].getRow() >= 1)
                {
                    ResetFades();
                    state = "done";
                }
            }
        }

        public void setState(string s)
        {
            state = s;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (state == "leaving")
                FadeOut[curOut].Draw(spriteBatch, Color.White);
            else if (state == "entering")
                FadeIn[curIn].Draw(spriteBatch, Color.White);
            else if (state == "inbetween")
            {
                spriteBatch.Begin();
                spriteBatch.Draw(blankScreen, Vector2.Zero, Color.White);
                spriteBatch.End();
            }
        }
    }
}
