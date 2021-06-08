using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class DayEnd : IUserInterface
    {
        private readonly Controller controller;
        private readonly SolidBrush brush = new SolidBrush(Color.White);
        private readonly Font font = new Font(StringStyle.FontFamily, 23);
        private readonly Rectangle upperRectangle = new Rectangle(300, 150, 960, 300);
        private readonly Rectangle middleRectangle = new Rectangle(300, 290, 960, 450);
        private readonly StringFormat leftAlignment = new StringFormat() { Alignment = StringAlignment.Far };

        public DayEnd(Controller controller)
        {
            this.controller = controller;
        }

        public void Change(Form1 form)
        {
            controller.Sounds.StopOfficeBackground();
        }

        public void MouseDown() { }

        public void Paint(Graphics graphics)
        {
            graphics.Clear(Color.Black);
            graphics.DrawString("Ежедневный рабочий отчёт", font, brush, new Point(500, 20));
            graphics.DrawString($"Проверено статей: {controller.Level.VerifiedArticlesCount} \nИз них одобрено: {controller.Level.ApprovedArticlesCount}",
            font,
            brush,
            upperRectangle);
            graphics.DrawString($"Найдено ошибок: {controller.Level.MistakesFoundCount} \nОтказов из-за цензуры: 0",
            font,
            brush,
            upperRectangle,
            leftAlignment);
            graphics.DrawString("Бюджет компании: \nДоход: \nВнутренние затраты издательства: \nГосударственные штрафы: \nИтого:",
            font,
            brush,
            middleRectangle);
            graphics.DrawString($"{controller.Stats.Money} руб.\n{controller.Level.Income} руб.\n{controller.Stats.InternalCosts} руб.\n{controller.Level.FinesSum} руб.\n{controller.Stats.Money + controller.Level.Income - controller.Stats.InternalCosts - controller.Level.FinesSum} руб.",
            font,
            brush,
            middleRectangle,
            leftAlignment);
            graphics.DrawString($"Популярность: {GetStringMeaning(controller.Level.IncreaseInPopularity)} \nВ целом читатели довольны выпускаемыми статьями, но явно ждут чего-то большего...",
            font,
            brush,
            new Rectangle(300, 540, 960, 450));
        }

        public void Tick() { }

        private string GetStringMeaning(int increase)
        {
            if (increase <= -10)
                return "огромное падение";
            if (increase <= -7)
                return "большое падение";
            if (increase <= -4)
                return "среднее падение";
            if (increase <= -1)
                return "незначительное падение";
            if (increase >= 10)
                return "огромный прирост";
            if (increase >= 7)
                return "большой прирост";
            if (increase >= 4)
                return "средний прирост";
            if (increase >= 1)
                return "незначительный прирост";
            return "осталась прежней";
        }
    }
}