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
    public class CheckMode
    {
        /// <summary>
        /// Вызов метода Invalidate()
        /// </summary>
        private readonly Action updateGraphics;

        /// <summary>
        /// Делегат, создающий событие клика мыши на области текста
        /// </summary>
        private readonly Action<Label[]> createEventClickForTextAreas;

        private Control.ControlCollection controls;

        //private readonly Action<Label[]> createEventClickForMistakeAreas;

        /// <summary>
        /// Содержит список интерактивных текстовых кнопок в перечне ошибок.
        /// Элемент списка - номер страницы в книге.
        /// Элемент массива внутри списка - очередной Label, который рисуется на текущей странице в книге.
        /// </summary>
        public List<Label[]> ErrorsListOptions { get; }

        /// <summary>
        /// Содержит список интерактивных текстовых кнопок в цензурном перечне.
        /// Элемент списка - номер страницы в книге.
        /// Элемент массива внутри списка - очередной Label, который рисуется на текущей странице в книге.
        /// </summary>
        public List<Label[]> CensoredListOptions { get; }

        /// <summary>
        /// Содержит фреймы книги с соответствующими открытыми вкладками
        /// </summary>
        public Dictionary<Tabs, Bitmap> BookModes { get; }

        /// <summary>
        /// Текщая открытая вкладка в книге
        /// </summary>
        public Tabs CurrentBookMode { get; private set; }

        public Point ExitButtonLocation { get; }

        public Point BookLocation { get; }

        public Point PaperLocation { get; }

        public Dictionary<int, (Mistakes, Label, string)> MistakesList { get; }

        /// <summary>
        /// Хранит информацию о текстовых областях статьи.
        /// Key - идентификатор (HashCode) области.
        /// Value.Item1 - ошибка, привязанная к данной области.
        /// Value.Item2 - сама область.
        /// </summary>
        public Dictionary<int, (Mistakes, Label)> TextAreas { get; }

        public Dictionary<int, (Mistakes, Label)> ExpectedMistakeAreas { get; }

        public Label SelectedTextArea { get; private set; }

        /// <summary>
        /// Цвет выбранной текстовой области
        /// </summary>
        private readonly Color colorSelectedTextArea;
        private int currentMistakeY;

        public CheckMode(Dictionary<Tabs, Bitmap> bookFrames, Action update, Action<Label[]> eventTextAreas,
                         Color colorSelectedTextArea, ControlCollection controls)
        {
            this.controls = controls;
            updateGraphics = update;
            createEventClickForTextAreas = eventTextAreas;
            //createEventClickForMistakeAreas = eventMistakeAreas;
            BookModes = bookFrames;
            CurrentBookMode = Tabs.Guide;
            MistakesList = new Dictionary<int, (Mistakes, Label, string)>();
            currentMistakeY = 170;
            AddNewMistake(Mistakes.NoNumbers, "Отсутствие чисел", "Однозначные числа (меньше 10) записываются буквами," +
                " многозначные — в цифровой форме. \n В цифровой форме пишутся все даты. " +
                "\n Цифровая форма при написании однозначных чисел используется," +
                " когда однозначные целые числа образуют сочетание с единицами физических величин, денежными единицами и т. п.");
            TextAreas = new Dictionary<int, (Mistakes, Label)>();
            ExpectedMistakeAreas = new Dictionary<int, (Mistakes, Label)>();
            SelectedTextArea = null;
            ExitButtonLocation = new Point(1490, 30);
            BookLocation = new Point(-10, 10);
            PaperLocation = new Point(650, 0);
            this.colorSelectedTextArea = colorSelectedTextArea;
        }

        /// <summary>
        /// Содержит все вкладки книги
        /// </summary>
        public enum Tabs
        {
            Guide,
            MistakesList,
            CensoredList
        }

        /// <summary>
        /// Содержит все виды ошибок, которые могут встретиться в тексте
        /// </summary>
        public enum Mistakes
        {
            None,
            NoNumbers
        }

        /// <summary>
        /// Устанавливает текущую открытую вкладку книги
        /// </summary>
        /// <param name="tab"></param>
        public void ChangeBookMode(Tabs tab)
        {
            CurrentBookMode = tab;
            var listMistakeLabels = MistakesList.Values.Select(tuple => tuple.Item2);
            foreach (var e in listMistakeLabels) controls.Remove(e);
            if (tab == Tabs.MistakesList)
                foreach (var e in listMistakeLabels) controls.Add(e);
            updateGraphics();
        }

        /// <summary>
        /// Устанавливает текущую выбранную область текста
        /// </summary>
        /// <param name="area"></param>
        public void SetSelectedTextArea(Label area)
        {
            if (SelectedTextArea != null) SelectedTextArea.BackColor = Color.Transparent;
            SelectedTextArea = area;
            if (area != null) area.BackColor = colorSelectedTextArea;
        }

        public void AddNewMistake(Mistakes mistake, string name, string description)
        {
            var label = new Label()
            {
                Text = name,
                Font = StringStyle.Font,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Transparent,
                MaximumSize = new Size(475, 0),
                Location = new Point(BookLocation.X + 20, currentMistakeY)
            };
            label.Width = label.PreferredWidth;
            label.Height = label.PreferredHeight;
            currentMistakeY += label.Height + 7;
            MistakesList.Add(label.GetHashCode(), (mistake, label, description));
            //createEventClickForMistakeAreas(MistakesList.)
        }

        /// <summary>
        /// Оформляет текст статьи на бумаге, разделяя его на интерактивные области.
        /// </summary>
        /// <param name="textFile">Файл формата .txt со статьёй</param>
        public void CreateArticleText(StreamReader textFile)
        {
            var labelX = PaperLocation.X + 10;
            var labelY = PaperLocation.Y + 5;
            var paperImage = Properties.Resources.PaperBackground;

            // Следующий код идентичен. Нужен рефакторинг!
            
            //Название статьи
            var name = GetLabel(textFile.ReadLine(), new Point(labelX, labelY));
            //Центрируем
            name.Location = new Point((PaperLocation.X + paperImage.Width - name.Width) / 2 + 115, name.Location.Y);
            name.TextAlign = ContentAlignment.MiddleCenter;
            //Добавляем в словарь интерктивности
            TextAreas.Add(name.GetHashCode(), (Mistakes.None, name));

            //Направленность
            var genre = GetLabel(textFile.ReadLine(), new Point(labelX, labelY));
            //Выравнимаем по правому краю
            genre.Location = new Point(PaperLocation.X + paperImage.Width - genre.Width - 420, genre.Location.Y);
            genre.TextAlign = ContentAlignment.MiddleRight;
            //Добавляем в словарь интерктивности
            TextAreas.Add(genre.GetHashCode(), (Mistakes.None, genre));

            //ЦА
            var targetAudience = GetLabel(textFile.ReadLine(), new Point(labelX, labelY));
            //Выравнимаем по правому краю
            targetAudience.Location = new Point(PaperLocation.X + paperImage.Width - targetAudience.Width - 420, targetAudience.Location.Y);
            targetAudience.TextAlign = ContentAlignment.MiddleRight;
            //Добавляем в словарь интерктивности
            TextAreas.Add(targetAudience.GetHashCode(), (Mistakes.None, targetAudience));

            //Следующие области текста автоматически выравниваются по левому краю
            var line = textFile.ReadLine();
            while (line != null)
            {
                var area = GetLabel(line, new Point(labelX, labelY));
                if (area.Text[0] == '[') area = ReadMistakeName(area);
                TextAreas.Add(area.GetHashCode(), (Mistakes.None, area));
                line = textFile.ReadLine();
            }
            createEventClickForTextAreas(TextAreas.Values.Select(tuple => tuple.Item2).ToArray());

            Label GetLabel(string text, Point location)
            {
                var result = new Label()
                {
                    Text = text,
                    Font = StringStyle.Font,
                    AutoSize = true,
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.Transparent,
                    Location = location,
                    MaximumSize = new Size(720, 0),
                };
                result.Height = result.PreferredHeight;
                result.Width = result.PreferredWidth;
                labelY += result.Height + 7;
                if (result.Text.Substring(0, 3) == "ЦА:") labelY += 30;
                return result;
            }
        }

        /// <summary>
        /// Считывает со строки label тип ошибки, которую должен отметить игрок.
        /// Тип ошибки всегда помечается в начале строки в [] скобках.
        /// </summary>
        /// <param name="label"></param>
        /// <returns>Label с текстом без наименования ошибки в [] скобках</returns>
        private Label ReadMistakeName(Label label)
        {
            var result = new StringBuilder();
            var index = 1;
            try
            {
                while (label.Text[index] != ']')
                {
                    result.Append(label.Text[index]);
                    index++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception("Закрывающая скобка \"]\" не была найдена", e);
            }
            index += 2;

            switch (result.ToString())
            {
                case "NoNumbers":
                    ExpectedMistakeAreas.Add(label.GetHashCode(), (Mistakes.NoNumbers, label));
                    break;
                default: throw new ArgumentException("Такого типа ошибки не существует");
            }
            label.Text = label.Text.Substring(index);
            return label;
        }

        public void Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.DrawImage(BookModes[CurrentBookMode], BookLocation.X, BookLocation.Y);
            graphics.DrawImage(Properties.Resources.ExitFromCheckMode, ExitButtonLocation.X, ExitButtonLocation.Y);
            graphics.DrawImage(Properties.Resources.PaperBackground, PaperLocation.X, PaperLocation.Y);
            switch (CurrentBookMode)
            {
                case Tabs.Guide: CreateTitle("Руководство главного редактора"); break;
                case Tabs.MistakesList: CreateTitle("Перечень ошибок"); break;
                case Tabs.CensoredList: CreateTitle("Цензурный перечень"); break;
            }

            void CreateTitle(string text) =>
                graphics.DrawString(text,
                        new Font(StringStyle.FontFamily, 19, FontStyle.Bold),
                        StringStyle.Brush, new Rectangle(15, 90, 500, 70),
                        new StringFormat() { Alignment = StringAlignment.Center });
        }
    }
}
