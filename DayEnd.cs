using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using static System.Windows.Forms.Control;

namespace Rewrite_It
{
    public class DayEnd
    {
        private readonly ControlCollection controls;

        private readonly double earnedMoney;
        private readonly double fines;

        private readonly string[] text = { "Заработано: ", "Штраф: ", "Итого: " };

        private readonly Action updateGraphics;

        public DayEnd(Action update, ControlCollection controls, double earned, double minusMoney)
        {
            this.controls = controls;
            updateGraphics = update;
            earnedMoney = earned;
            fines = minusMoney;
        }

        public void Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            updateGraphics();
            graphics.Clear(Color.Black);
            var x = 512;
            var y = 512;
            graphics.DrawString("Заработано: " + earnedMoney, StringStyle.Font, StringStyle.Brush, x, y);
            graphics.DrawString("Штраф: " + fines, StringStyle.Font, StringStyle.Brush, x: x + 20, y: y + 20);
            graphics.DrawString("Итого: " + (fines + earnedMoney), StringStyle.Font, StringStyle.Brush, x: x + 40, y: y + 40);
        }
    }
}
