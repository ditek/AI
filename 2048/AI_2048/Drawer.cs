using System;
using System.Collections.Generic;
using System.Drawing;

namespace AI_2048
{
    static class Drawer
    {
        private static List<Bitmap> oBitmap = new List<Bitmap>();

        private static Font fFontS2 = new Font("Clear Sans", 10, FontStyle.Bold);
        private static Font fFontS = new Font("Clear Sans", 12, FontStyle.Bold);
        private static Font fFont = new Font("Clear Sans", 22, FontStyle.Bold);
        private static SizeF stringSize = new SizeF();
        private static Rectangle rRect = new Rectangle(0, 0, 416, 640);

        public static void init()
        {
            oBitmap.Add(new Bitmap(@"images/1.png"));
            oBitmap.Add(new Bitmap(@"images/2.png"));
            oBitmap.Add(new Bitmap(@"images/3.png"));
            oBitmap.Add(new Bitmap(@"images/4.png"));
            oBitmap.Add(new Bitmap(@"images/5.png"));
            oBitmap.Add(new Bitmap(@"images/6.png"));
            oBitmap.Add(new Bitmap(@"images/7.png"));
            oBitmap.Add(new Bitmap(@"images/8.png"));
            oBitmap.Add(new Bitmap(@"images/9.png"));
            oBitmap.Add(new Bitmap(@"images/k0.png"));
            oBitmap.Add(new Bitmap(@"images/10.png"));
            oBitmap.Add(new Bitmap(@"images/11.png"));
            oBitmap.Add(new Bitmap(@"images/12.png"));
            oBitmap.Add(new Bitmap(@"images/13.png"));
            oBitmap.Add(new Bitmap(@"images/14.png"));
            oBitmap.Add(new Bitmap(@"images/15.png"));
            oBitmap.Add(new Bitmap(@"images/16.png"));
            oBitmap.Add(new Bitmap(@"images/17.png"));
            oBitmap.Add(new Bitmap(@"images/18.png"));
        }

        public static void Draw(Graphics g, Game game)
        {
            DrawGame(g, game);
            if (game.gameOver)
                GameOverDraw(g);
            game.bRender = false;
        }

        public static void DrawGame(Graphics g, Game game)
        {
            g.DrawImage(oBitmap[3], new Point(18, 166));

            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                {
                    g.DrawImage(oBitmap[getBitmapID(game.iBoard[i][j])], new Point(30 + 87 * i, 178 + 87 * j));
                    if (game.iBoard[i][j] > 0)
                        DrawTextCenterWS(g, game.iBoard[i][j].ToString(), fFont, new SolidBrush(Color.FromArgb(64, 10, 10, 10)), (game.iBoard[i][j] < 8 ? new SolidBrush(Color.FromArgb(120, 110, 101)) : new SolidBrush(Color.FromArgb(249, 245, 235))), 68 + 87 * i, 217 + 87 * j);
                }
        }

        public static void GameOverDraw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(150, 251, 248, 239)), rRect);

            DrawTextCenterXWS(g, "GAME OVER", fFontS, new SolidBrush(Color.FromArgb(64, 10, 10, 10)), new SolidBrush(Color.FromArgb(120, 110, 101)), 198, 250);
        }

        /* ******************************************** */

        public static void DrawTextCenterX(Graphics g, String sText, Font nFont, SolidBrush nSolidBrush, int X, int Y)
        {
            stringSize = g.MeasureString(sText, nFont);
            g.DrawString(sText, nFont, nSolidBrush, new PointF(X - stringSize.Width / 2, Y));
        }

        public static void DrawTextCenterXWS(Graphics g, String sText, Font nFont, SolidBrush nSolidBrush, SolidBrush nSolidBrush2, int X, int Y)
        {
            stringSize = g.MeasureString(sText, nFont);
            g.DrawString(sText, nFont, nSolidBrush, new PointF(X - stringSize.Width / 2 + 1, Y + 1));
            g.DrawString(sText, nFont, nSolidBrush2, new PointF(X - stringSize.Width / 2, Y));
        }

        public static void DrawTextCenterWS(Graphics g, String sText, Font nFont, SolidBrush nSolidBrush, SolidBrush nSolidBrush2, int X, int Y)
        {
            stringSize = g.MeasureString(sText, nFont);
            g.DrawString(sText, nFont, nSolidBrush, new PointF(X - stringSize.Width / 2 + 1, Y - stringSize.Height / 2 + 1));
            g.DrawString(sText, nFont, nSolidBrush2, new PointF(X - stringSize.Width / 2, Y - stringSize.Height / 2));
        }

        /* ******************************************** */

        public static int getBitmapID(int iNum)
        {
            switch (iNum)
            {
                case 0:
                    return 4;
                case 2:
                    return 5;
                case 4:
                    return 6;
                case 8:
                    return 7;
                case 16:
                    return 8;
                case 32:
                    return 10;
                case 64:
                    return 11;
                case 128:
                    return 12;
                case 256:
                    return 13;
                case 512:
                    return 14;
                case 1024:
                    return 15;
                case 2048:
                    return 16;
                case 4096:
                case 8192:
                case 16384:
                    return 17;
            }
            return 4;
        }
    }
}
