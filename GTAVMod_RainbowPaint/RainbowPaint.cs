﻿/*
 * Rainbow Paint
 * Author: libertylocked
 * Version: 0.1
 */
using System;
using System.Drawing;
using System.Windows.Forms;
using GTA;
using GTA.Native;

namespace GTAVMod_RainbowPaint
{
    public class RainbowPaint : Script
    {
        // You can change the tick interval if you want to adjust transition speed
        const int TICK_INTERVAL = 50;

        int huePercent = 0;
        int huePercent2 = 0;
        int mult = 1;
        int mult2 = 1;

        bool playerWasInVeh = false;

        public RainbowPaint()
        {
            this.Interval = TICK_INTERVAL;
            this.Tick += OnTick;
        }

        void OnTick(object sender, EventArgs e)
        {
            Player player = Game.Player;

            if (player != null && player.CanControlCharacter && player.IsAlive
                && player.Character != null && player.Character.IsInVehicle())
            {
                if (!playerWasInVeh)
                {
                    // randomize hues when player gets into a vehicle
                    Random rng = new Random();
                    huePercent = rng.Next(100);
                    huePercent2 = rng.Next(100);
                    //UI.Notify("Randomized " + huePercent + " " + huePercent2);
                }

                // Increment hue for primary color
                huePercent += 1 * mult;
                if (huePercent >= 99 || huePercent <= 0) mult *= -1;

                // Increment hue for secondary color
                huePercent2 += 1 * mult2;
                if (huePercent2 >= 99 || huePercent2 <= 0) mult2 *= -1;

                player.Character.CurrentVehicle.CustomPrimaryColor = HSL2RGB((double)huePercent / 100, 0.5, 0.5);
                player.Character.CurrentVehicle.CustomSecondaryColor = HSL2RGB((double)huePercent2 / 100, 0.5, 0.5);
            }

            if (player != null && player.Character != null)
            {
                playerWasInVeh = player.Character.IsInVehicle();
            }
        }

        // Given H,S,L in range of 0-1
        // Returns a Color (RGB struct) in range of 0-255
        public static Color HSL2RGB(double h, double sl, double l)
        {
            double v;
            double r, g, b;

            r = l;   // default to gray
            g = l;
            b = l;
            v = (l <= 0.5) ? (l * (1.0 + sl)) : (l + sl - l * sl);
            if (v > 0)
            {
                double m;
                double sv;
                int sextant;
                double fract, vsf, mid1, mid2;

                m = l + l - v;
                sv = (v - m) / v;
                h *= 6.0;
                sextant = (int)h;
                fract = h - sextant;
                vsf = v * sv * fract;
                mid1 = m + vsf;
                mid2 = v - vsf;
                switch (sextant)
                {
                    case 0:
                        r = v;
                        g = mid1;
                        b = m;
                        break;
                    case 1:
                        r = mid2;
                        g = v;
                        b = m;
                        break;
                    case 2:
                        r = m;
                        g = v;
                        b = mid1;
                        break;
                    case 3:
                        r = m;
                        g = mid2;
                        b = v;
                        break;
                    case 4:
                        r = mid1;
                        g = m;
                        b = v;
                        break;
                    case 5:
                        r = v;
                        g = m;
                        b = mid2;
                        break;
                }
            }
            int colorR = Math.Min(Convert.ToInt32(r * 255.0f), 255);
            int colorG = Math.Min(Convert.ToInt32(g * 255.0f), 255);
            int colorB = Math.Min(Convert.ToInt32(b * 255.0f), 255);
            return Color.FromArgb(colorR, colorG, colorB);
        }

        public static void RGB2HSL(Color rgb, out double h, out double s, out double l)
        {
            double r = rgb.R / 255.0;
            double g = rgb.G / 255.0;
            double b = rgb.B / 255.0;
            double v;
            double m;
            double vm;
            double r2, g2, b2;

            h = 0; // default to black
            s = 0;
            l = 0;
            v = Math.Max(r, g);
            v = Math.Max(v, b);
            m = Math.Min(r, g);
            m = Math.Min(m, b);

            l = (m + v) / 2.0;
            if (l <= 0.0)
            {
                return;
            }
            vm = v - m;
            s = vm;
            if (s > 0.0)
            {
                s /= (l <= 0.5) ? (v + m) : (2.0 - v - m);
            }
            else
            {
                return;
            }
            r2 = (v - r) / vm;
            g2 = (v - g) / vm;
            b2 = (v - b) / vm;
            if (r == v)
            {
                h = (g == m ? 5.0 + b2 : 1.0 - g2);
            }
            else if (g == v)
            {
                h = (b == m ? 1.0 + r2 : 3.0 - b2);
            }
            else
            {
                h = (r == m ? 3.0 + g2 : 5.0 - r2);
            }
            h /= 6.0;
        }
    }
}
