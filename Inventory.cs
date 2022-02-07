using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Ozzymon
{
    internal class Inventory
    {
        // DELAY stuff for controls
        bool delaying = false;
        double maxDelay = 200;
        double delay = 0;

        Vector2 screenPos = new Vector2(211, 54);
        Texture2D staticScreen;
        Texture2D topBorder;
        Texture2D bottomBorder;
        AnimatedSprite open;
        AnimatedSprite close;
        Sprite splashScreen;
        SpriteFont font;

        // Pixel for drawing
        Texture2D pixel;

        // base items ozzydex settings ozzymon ¿map? ribbons
        string currentMenu = "base";

        // elements in base menu
        string[] baseMenu = { "Return to Game", "Ozzymon", "Ozzydex", "Items", "Map", "Ribbons", "Save", "Settings", "Quit" };



        // which list element is currently selected
        int menuIndex = 0;

        // closed opening splash menu splash2 closing
        string state = "closed";

        public Inventory(ContentManager cm)
        {
            pixel = cm.Load<Texture2D>("PDA/pixel");

            staticScreen = cm.Load<Texture2D>("PDA/pdaStatic");
            topBorder = cm.Load<Texture2D>("PDA/pdaTopBorder");
            bottomBorder = cm.Load<Texture2D>("PDA/pdaBottomBorder");
            font = cm.Load<SpriteFont>("Font/pdaFont");

            splashScreen = new Sprite(cm.Load<Texture2D>("PDA/splashScreen"), screenPos);
            open = new AnimatedSprite(cm.Load<Texture2D>("PDA/pdaOpen"), Vector2.Zero, 5, 4);
            close = new AnimatedSprite(cm.Load<Texture2D>("PDA/pdaClose"), Vector2.Zero, 5, 4);

            close.offset = Vector2.Zero;
            open.offset = Vector2.Zero;
            splashScreen.offset = Vector2.Zero;

            splashScreen.fade = 0f;
        }

        private int menuLength()
        {
            if (currentMenu == "base")
                return baseMenu.Length;
            else
                return -1;
        }

        public bool isClosed()
        {
            return state == "closed";
        }

        private void updateDelay(GameTime gameTime)
        {
            if (delaying)
            {
                delay += gameTime.ElapsedGameTime.TotalMilliseconds;
                if (delay >= maxDelay)
                {
                    delaying = false;
                    delay = 0;
                }
            }
        }

        public void Start()
        {
            state = "opening";
            open.Play();
        }

        private int selectPressed()
        {
            if (currentMenu == "base")
            {
                string menuItem = baseMenu[menuIndex];
                if (menuItem == "Quit")
                    return 0;
                else if (menuItem == "Return to Game")
                    state = "splash2";
                else if (menuItem == "Ozzymon")
                {
                    menuIndex = 0;
                    currentMenu = "ozzymon";

                }
                else if (menuItem == "Ozzydex")
                {
                    menuIndex = 0;
                    currentMenu = "ozzydex";

                }
                else if (menuItem == "Items")
                {
                    menuIndex = 0;
                    currentMenu = "items";
                }
                else if (menuItem == "Map")
                {
                    menuIndex = 0;
                    currentMenu = "map";
                }
                else if (menuItem == "Ribbons")
                {
                    menuIndex = 0;
                    currentMenu = "ribbons";
                }
                else if (menuItem == "Save")
                {
                    menuIndex = 0;
                    currentMenu = "save";
                }
                else if (menuItem == "Settings")
                {
                    menuIndex = 0;
                    currentMenu = "settings";
                }
            }

            // got through without needing to exit the program
            return 1;
        }

        // if it can close the game make it bool
        public bool Update(GameTime gameTime)
        {
            // controls delay
            updateDelay(gameTime);

            if (state == "opening")
            {
                open.Update(gameTime);
                if (open.getFrame() >= 4 && open.getRow() >= 3)
                {
                    open.Stop();
                    open.Reset();
                    state = "splash";
                }
            }
            else if (state == "splash")
            {
                splashScreen.fade += .01f;
                if (splashScreen.fade >= 1.2f)
                {
                    splashScreen.fade = 0f;
                    state = "menu";
                }
            }
            else if (state == "menu")
            {
                if (!delaying && (Keyboard.GetState().IsKeyDown(Keys.Enter) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)))
                {
                    if (selectPressed() == 0)
                        return true;
                    // state = "splash2";
                }

                if (!delaying && (Keyboard.GetState().IsKeyDown(Keys.W) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y >= .01f))
                {
                    delaying = true;
                    if (menuIndex - 1 >= 0)
                        menuIndex -= 1;
                    else
                        menuIndex = menuLength() - 1;
                }
                else if (!delaying && (Keyboard.GetState().IsKeyDown(Keys.S) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y <= -.01f))
                {
                    delaying = true;
                    if (menuIndex + 1 < menuLength())
                        menuIndex += 1;
                    else
                        menuIndex = 0;
                }
            }
            else if (state == "splash2")
            {
                splashScreen.fade += .01f;
                if (splashScreen.fade >= 1.2f)
                {
                    splashScreen.fade = 0f;
                    state = "closing";
                    close.Play();
                }
            }
            else if (state == "closing")
            {
                close.Update(gameTime);
                if (close.getFrame() >= 4 && close.getRow() >= 3)
                {
                    close.Stop();
                    close.Reset();
                    state = "closed";
                }
            }

            // end of method and not exitting
            return false;
        }

        public void Draw(SpriteBatch spriteBatch, string time)
        {
            if (state == "opening")
            {
                open.Draw(spriteBatch);
            }
            else if (state == "splash")
            {
                spriteBatch.Begin();
                spriteBatch.Draw(staticScreen, Vector2.Zero, Color.White);
                spriteBatch.End();
                splashScreen.Draw(spriteBatch);
            }
            else if (state == "menu")
            {
                spriteBatch.Begin();
                spriteBatch.Draw(staticScreen, Vector2.Zero, Color.White);

                // HERE DRAW THE CURRENT MENU
                if (currentMenu == "base")
                {
                    for (int i = 0; i < baseMenu.Length; i++)
                    {

                        if (i == menuIndex)
                        {
                            spriteBatch.Draw(pixel, screenPos + new Vector2(0, 20 + (font.MeasureString("A").Y * i)), new Rectangle(0, 0, 1, 1), Color.Black, 0f, Vector2.Zero, new Vector2(splashScreen.getWidth(), font.MeasureString("A").Y), SpriteEffects.None, 0f);
                            //spriteBatch.DrawString(font, baseMenu[i], screenPos + new Vector2(12, 22 + (font.MeasureString("A").Y * i)), Color.Gray);
                            spriteBatch.DrawString(font, baseMenu[i], screenPos + new Vector2(10, 20 + (font.MeasureString("A").Y * i)), Color.White);
                        }
                        else
                        {
                            //spriteBatch.DrawString(font, baseMenu[i], screenPos + new Vector2(12, 22 + (font.MeasureString("A").Y * i)), Color.LightGray);
                            spriteBatch.DrawString(font, baseMenu[i], screenPos + new Vector2(10, 20 + (font.MeasureString("A").Y * i)), Color.Black);
                        }
                    }
                }
                else if (currentMenu == "items")
                {

                }
                else if (currentMenu == "ozzydex")
                {

                }
                else if (currentMenu == "settings")
                {

                }
                else if (currentMenu == "ozzymon")
                {

                }
                else if (currentMenu == "map")
                {

                }
                else if (currentMenu == "ribbons")
                {

                }

                // bottom and top bars
                spriteBatch.Draw(topBorder, screenPos, Color.White);
                spriteBatch.Draw(bottomBorder, screenPos, Color.White);

                //top text
                spriteBatch.DrawString(font, time, screenPos + new Vector2(15, 1), Color.Black);
                //bottom text
                if (!GamePad.GetState(PlayerIndex.One).IsConnected)
                {
                    spriteBatch.DrawString(font, "A/W - Up/Down", screenPos + new Vector2(15, bottomBorder.Height - (font.MeasureString("A").Y) + 1), Color.Black);
                    spriteBatch.DrawString(font, "Enter - Select", screenPos + new Vector2(bottomBorder.Width - (font.MeasureString("Enter - Select").X + 15), bottomBorder.Height - (font.MeasureString("A").Y) + 1), Color.Black);
                }
                else
                {
                    spriteBatch.DrawString(font, "Left Stick - Up/Down", screenPos + new Vector2(15, bottomBorder.Height - (font.MeasureString("A").Y) + 1), Color.Black);
                    spriteBatch.DrawString(font, "A - Select", screenPos + new Vector2(bottomBorder.Width - (font.MeasureString("Enter - Select").X + 15), bottomBorder.Height - (font.MeasureString("A").Y) + 1), Color.Black);
                }
                spriteBatch.End();
            }
            else if (state == "splash2")
            {
                spriteBatch.Begin();
                spriteBatch.Draw(staticScreen, Vector2.Zero, Color.White);
                spriteBatch.End();
                splashScreen.Draw(spriteBatch);
            }
            else if (state == "closing")
            {
                close.Draw(spriteBatch);
            }
        }
    }
}
