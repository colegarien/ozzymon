using Microsoft.Xna.Framework;
using System;

namespace Ozzymon
{
    internal class GameClock
    {
        // outside sky color based on time
        Color skyColor = new Color(45, 45, 45);

        // ratio of 1 real time second = 8 game seconds
        int timeFactor = 8;

        // minimum color
        int minColor = 60;

        // is the sun rising
        bool rising = true;

        // in seconds
        int dayLength = 24 * 3600;

        // seconds will be total
        float seconds = 0;

        // current time in hours and minutes
        float hours = 0;
        float minutes = 0;

        public GameClock(float s, int f)
        {
            timeFactor = f;
            seconds = s;
            float temp = s / 60;
            minutes = temp % 60;
            hours = temp / 60;


            // if half way done then the sun will set ELSE the sun will rise if at beginning of day
            if (hours == (dayLength / 3600 / 2))
                rising = false;
            else if (hours == 0)
                rising = true;

            // if rising get brighter if falling get darker
            if (rising)
            {
                //int colorVal = 40 + (int)(hours) * (215/11);
                //             left value                          255 - left val
                int colorVal = minColor + (int)((seconds / (dayLength / 2)) * (255 - minColor));
                skyColor = new Color(colorVal, colorVal, colorVal);
            }
            else if (!rising)
            {
                int colorVal = 255 - ((int)(((seconds - (dayLength / 2)) / (dayLength / 2)) * (215)));
                skyColor = new Color(colorVal, colorVal, colorVal);
            }
        }

        public void Update(GameTime gameTime)
        {
            // add the seconds since last tick and change by time factor
            seconds += gameTime.ElapsedGameTime.Milliseconds / 1000f * timeFactor;

            // if the day is done then start over
            if (seconds >= dayLength)
                seconds = 0;

            // get hours and minutes
            float temp = seconds / 60;
            minutes = temp % 60;
            hours = temp / 60;

            // if half way done then the sun will set ELSE the sun will rise if at beginning of day
            if (hours == (dayLength / 3600 / 2))
                rising = false;
            else if (hours == 0)
                rising = true;

            // if rising get brighter if falling get darker
            if (rising)
            {
                //int colorVal = 40 + (int)(hours) * (215/11);
                int colorVal = 60 + (int)((seconds / (dayLength / 2)) * (215));
                skyColor = new Color(colorVal, colorVal, colorVal);
            }
            else if (!rising)
            {
                int colorVal = 255 - ((int)(((seconds - (dayLength / 2)) / (dayLength / 2)) * (215)));
                skyColor = new Color(colorVal, colorVal, colorVal);
            }
        }

        // return the string in a nice am pm format
        public string getTime()
        {
            string theTime = "";
            string h = Math.Floor(hours).ToString();
            string m = Math.Floor(minutes).ToString();
            if (int.Parse(h) <= 9)
                theTime += "0";
            theTime += h + ":";
            if (int.Parse(m) <= 9)
                theTime += "0";
            theTime += m;

            if (rising)
                theTime += " am";
            else
                theTime += " pm";
            return theTime;
        }

        // gets the color of the sky outside
        public Color getColor() { return skyColor; }
    }
}
