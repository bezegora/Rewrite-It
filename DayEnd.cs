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
        //Вместо controls создал form. Здесь Controls не понадобится.
        private readonly Form1 form;

        private readonly double earnedMoney;
        private readonly double fines;

        private readonly string[] text = { "Заработано: ", "Штраф: ", "Итого: " };

        //Теперь не нужно обновлять графику вручную, она обновляется автоматически.
        //private readonly Action updateGraphics;

        //В классе form можно обращаться к 2-м классам: Stats и Level.
        //Level содержит показатели, характерные только для данного уровня. 
        //Stats содержит общие показатели, которые изменяются на протяжении всей игры.
        //Напиши form.Level. или form.Stats. и увидишь нужные тебе свойства во всплывающем списке.
        //Все свойства задокументированы (к ним есть пояснения).
        //С текущим статусом популярности и с прибавкой к популярности сложнее.
        //Мы не показываем популярность в числовом виде, а руководствуемся таблицами, которую я скинул в ЛС.
        //Текущую дату можно получить так: form.Stats.GetDate()

        public DayEnd(Form1 form, double earned, double minusMoney)
        {
            this.form = form;
            //updateGraphics = update;
            earnedMoney = earned;
            fines = minusMoney;
        }

        public void Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.Clear(Color.Black);
            var x = 512;
            var y = 512;
            graphics.DrawString("Заработано: " + earnedMoney, StringStyle.Font, StringStyle.Brush, x, y);
            graphics.DrawString("Штраф: " + fines, StringStyle.Font, StringStyle.Brush, x: x + 20, y: y + 20);
            graphics.DrawString("Итого: " + (fines + earnedMoney), StringStyle.Font, StringStyle.Brush, x: x + 40, y: y + 40);
        }
    }
}
