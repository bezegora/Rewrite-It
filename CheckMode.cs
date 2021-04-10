using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class CheckMode
    {
        /// <summary>
        /// Вызов метода Invalidate()
        /// </summary>
        private readonly Action updateGraphics;

        /// <summary>
        /// Текст статьи, разделённый на название, направленность, целевую аудиторию и содержание
        /// </summary>
        public ArticleText ArticleText { get; set; }

        /// <summary>
        /// Содержит список интерактивных текстовых кнопок в перечне ошибок.
        /// Элемент списка - номер страницы в книге.
        /// Элемент массива внутри списка - очередной Label, который рисуется на текущей странице в книге.
        /// </summary>
        public List<Label[]> ErrorsListOptions { get; set; }

        /// <summary>
        /// Содержит список интерактивных текстовых кнопок в цензурном перечне.
        /// Элемент списка - номер страницы в книге.
        /// Элемент массива внутри списка - очередной Label с текстом "string", который рисуется на текущей странице в книге.
        /// </summary>
        public List<string[]> CensoredListOptions { get; set; }

        /// <summary>
        /// Содержит фреймы книги с соответствующими открытыми вкладками
        /// </summary>
        public Dictionary<Tabs, Bitmap> BookModes { get; }

        /// <summary>
        /// Текщая открытая вкладка в книге
        /// </summary>
        public Tabs CurrentBookMode { get; set; }

        /// <summary>
        /// Координаты кнопки выхода из режима проверки
        /// </summary>
        public Point ExitButtonLocation { get; set; }

        /// <summary>
        /// Координаты книги
        /// </summary>
        public Point BookLocation { get; set; }

        public CheckMode(Dictionary<Tabs, Bitmap> bookFrames, Action update)
        {
            updateGraphics = update;
            BookModes = bookFrames;
            CurrentBookMode = Tabs.Guide;
            ExitButtonLocation = new Point(1490, 30);
            BookLocation = new Point(-10, 10);
        }

        /// <summary>
        /// Содержит все вкладки книги
        /// </summary>
        public enum Tabs
        {
            Guide,
            ErrorsList,
            CensoredList
        }

        /// <summary>
        /// Открывает заданную вкладку в книге
        /// </summary>
        /// <param name="tab"></param>
        public void ChangeBookMode(Tabs tab)
        {
            CurrentBookMode = tab;
            updateGraphics();
        }

        public void Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.DrawImage(BookModes[CurrentBookMode], BookLocation.X, BookLocation.Y);
            graphics.DrawImage(Properties.Resources.ExitFromCheckMode, ExitButtonLocation.X, ExitButtonLocation.Y);
            switch (CurrentBookMode)
            {
                case Tabs.Guide:
                    CreateTitle("Руководство главного редактора");
                    break;
                case Tabs.ErrorsList:
                    CreateTitle("Перечень ошибок");
                    break;
                case Tabs.CensoredList:
                    CreateTitle("Цензурный перечень");
                    break;
            }

            void CreateTitle(string text) =>
                graphics.DrawString(text,
                        new Font(StringStyle.FontFamily, 19, FontStyle.Bold),
                        StringStyle.Brush, new Rectangle(15, 90, 500, 70),
                        new StringFormat() { Alignment = StringAlignment.Center });
        }
    }
}
