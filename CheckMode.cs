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
        //private readonly Action<Label[]> createEventClickForTextAreas;

        private readonly Control.ControlCollection controls;

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
        public Tabs CurrentBookMode { get; private set; } = Tabs.Guide;

        public Point ExitButtonLocation { get; set; } = new Point(1490, 30);

        public Point BookLocation { get; set; } = new Point(-10, 10);

        public Point PaperLocation { get; set; } = new Point(650, 0);

        public Dictionary<int, (Mistakes, Label, string)> MistakesList { get; } 
            = new Dictionary<int, (Mistakes mistake, Label area, string description)>();

        /// <summary>
        /// Содержит все текстовые области, которые рисуются непросредственно на форме.
        /// Key - идентификатор (HashCode) области.
        /// Value - сама область.
        /// </summary>
        public Dictionary<int, Label> TextAreas { get; } = new Dictionary<int, Label>();

        /// <summary>
        /// Содержит ожидаемые ошибочные текстовые области, которые должен отметить игрок.
        /// Данный словарь формируется автоматически из указанных типов ошибок в квадратных скобках [] в файлах формата .txt со статьёй.
        /// Key - идентификатор (HashCode) области.
        /// Value - ошибка, содержащаяся в данной области.
        /// </summary>
        public Dictionary<int, Mistakes> ExpectedMistakeAreas { get; } = new Dictionary<int, Mistakes>();

        /// <summary>
        /// Содержит выбранные игроком ошибочные области текста.
        /// Key - идентификатор (HashCode) области.
        /// Value - ошибка, которую выбрал игрок для данной области.
        /// </summary>
        public Dictionary<int, Mistakes> SelectedMistakeAreas { get; } = new Dictionary<int, Mistakes>();

        /// <summary>
        /// Текущая выбранная область текста.
        /// Возвращает null, если область не выбрана.
        /// </summary>
        public Label SelectedTextArea { get; private set; } = null;

        public (Label, Label) IsMatching { get; private set; } = (null, null);

        /// <summary>
        /// Цвет выбранной текстовой области
        /// </summary>
        private readonly Color colorSelectedTextArea;
        private int currentMistakeY = 170;
        private readonly Label textDescription;
        private readonly Label mistakeDescription;

        public CheckMode(Dictionary<Tabs, Bitmap> bookFrames,
                         Action update,
                         Color colorSelectedTextArea,
                         Color colorPopUpWindowText,
                         ControlCollection controls)
        {
            this.controls = controls;
            updateGraphics = update;
            BookModes = bookFrames;
            AddNewMistake(Mistakes.NoNumbers);
            AddNewMistake(Mistakes.IncorrectDefinitionTargetAudience);
            this.colorSelectedTextArea = colorSelectedTextArea;
            textDescription = new Label()
            {
                Font = StringStyle.Font,
                ForeColor = colorPopUpWindowText,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                MaximumSize = new Size(500, 0)
            };
            mistakeDescription = new Label()
            {
                Font = StringStyle.Font,
                ForeColor = colorPopUpWindowText,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(542, 73),
                MaximumSize = new Size(600, 0)
            };

            var mistakesList = MistakesList.Values.Select(tuple => tuple.Item2).ToArray();
            CreateEventEnteringMouseOnMistakeAreas(mistakesList);
            CreateEventClickForAreas(mistakesList);
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
            NoNumbers,
            IncorrectDefinitionTargetAudience
        }

        /// <summary>
        /// Устанавливает текущую открытую вкладку книги
        /// </summary>
        /// <param name="tab"></param>
        public void UpdateStatus(Tabs tab)
        {
            CurrentBookMode = tab;
            var listMistakeLabels = MistakesList.Values.Select(tuple => tuple.Item2);
            foreach (var e in listMistakeLabels) controls.Remove(e);
            if (SelectedTextArea != null && MistakesList.ContainsKey(SelectedTextArea.GetHashCode()))
                SelectedTextArea = null;
            if (tab == Tabs.MistakesList)
            {
                foreach (var e in listMistakeLabels) controls.Add(e);
            }
            foreach (var e in TextAreas.Values) controls.Add(e);
            updateGraphics();
        }

        private (string name, string description) GetMistakeText(Mistakes mistake)
        {
            switch (mistake)
            {
                case Mistakes.NoNumbers: return
                        ("Отсутствие чисел",
                        "Однозначные числа (меньше 10) записываются буквами," +
                        " многозначные — в цифровой форме.\n\nВ цифровой форме пишутся все даты." +
                        "\n\nЦифровая форма при написании однозначных чисел используется," +
                        " когда однозначные целые числа образуют сочетание с единицами физических величин, денежными единицами и т. п.");
                case Mistakes.IncorrectDefinitionTargetAudience: return
                        ("Неверное таргетирование целевой аудитории",
                        "Вы должны четко понимать группу людей, для которой пишете свое послание." +
                        "\nПромах по целевой аудитории является серьёзным ударом по убедительности");
            }
            throw new ArgumentException();
        }

        /// <summary>
        /// Устанавливает текущую выбранную область.
        /// Причём если выбранная область сопоставима с предыдущей, сработает метод сопоставления Match(Label, Label).
        /// </summary>
        /// <param name="nextArea"></param>
        public void SetSelectedTextArea(Label nextArea)
        {
            if (SelectedTextArea != null && nextArea != null)
            {
                var selectedAreaHash = SelectedTextArea.GetHashCode();
                var nextAreaHash = nextArea.GetHashCode();
                if ((MistakesList.ContainsKey(selectedAreaHash) && TextAreas.ContainsKey(nextAreaHash)))
                    Match(nextArea, SelectedTextArea);
                else if (TextAreas.ContainsKey(selectedAreaHash) && MistakesList.ContainsKey(nextAreaHash))
                    Match(SelectedTextArea, nextArea);
                else ChangeSelected();
                return;
            }
            ChangeSelected();

            void ChangeSelected()
            {
                if (SelectedTextArea != null) SelectedTextArea.BackColor = Color.Transparent;
                if (SelectedTextArea == nextArea)
                {
                    SelectedTextArea = null;
                    return;
                }
                SelectedTextArea = nextArea;
                if (nextArea != null) nextArea.BackColor = colorSelectedTextArea;
            }
        }

        public void SetIsMatching(Label textArea = null, Label mistakeArea = null) => IsMatching = (textArea, mistakeArea);

        public bool CheckHasMatching() => IsMatching.Item1 != null && IsMatching.Item2 != null;

        /// <summary>
        /// Проигрывает анимацию сопоставления двух областей.
        /// Сохраняет выбранную текстовую область в SelectedMistakeAreas.
        /// </summary>
        /// <param name="textArea"></param>
        /// <param name="mistakeArea"></param>
        private void Match(Label textArea, Label mistakeArea)
        {
            var textHash = textArea.GetHashCode();
            var mistakeHash = mistakeArea.GetHashCode();
            if (SelectedMistakeAreas.ContainsKey(textHash))
            {
                if (SelectedMistakeAreas[textHash] == MistakesList[mistakeHash].Item1) return;
                SelectedMistakeAreas.Remove(textHash);
            }
            textArea.BackColor = colorSelectedTextArea;
            mistakeArea.BackColor = colorSelectedTextArea;
            RemoveTextDescription(mistakeDescription);
            SelectedMistakeAreas.Add(textArea.GetHashCode(), MistakesList[mistakeArea.GetHashCode()].Item1);
            SetIsMatching(textArea, mistakeArea);
            updateGraphics();
            var wait = new Timer() { Interval = 1000 };
            wait.Start();
            wait.Tick += (sender, e) =>
            {
                SetIsMatching();
                updateGraphics();
                textArea.BackColor = Color.Transparent;
                mistakeArea.BackColor = Color.Transparent;
                textArea.ForeColor = Color.Red;
                SetSelectedTextArea(null);
                wait.Stop();
            };
        }

        private void Undo(Label textArea)
        {
            SelectedMistakeAreas.Remove(textArea.GetHashCode());
            textArea.ForeColor = StringStyle.Brush.Color;
            SetSelectedTextArea(null);
            RemoveTextDescription(textDescription);
        }

        private void RemoveTextDescription(Label popUpWindow) => controls.Remove(popUpWindow);

        /// <summary>
        /// Добавляет новую область с ошибкой в словарь, из которого эти области рисуются на форме последовательно друг за другом.
        /// </summary>
        /// <param name="mistake"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public void AddNewMistake(Mistakes mistake)
        {
            var (name, description) = GetMistakeText(mistake);
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
        }

        /// <summary>
        /// Оформляет текст статьи на бумаге, разделяя его на интерактивные области.
        /// </summary>
        /// <param name="textFile">Файл формата .txt со статьёй</param>
        public void CreateArticleText(StreamReader textFile)
        {
            var labelX = PaperLocation.X + 10;
            var labelY = PaperLocation.Y + 5;

            CreateAndAlignLabel(textFile.ReadLine(), ContentAlignment.MiddleCenter);
            CreateAndAlignLabel(textFile.ReadLine(), ContentAlignment.MiddleRight);
            CreateAndAlignLabel(textFile.ReadLine(), ContentAlignment.MiddleRight);

            var line = textFile.ReadLine();
            while (line != null)
            {
                var area = GetLabel(line, new Point(labelX, labelY));
                if (area.Text[0] == '[') area = ReadMistakeName(area);
                TextAreas.Add(area.GetHashCode(), area);
                line = textFile.ReadLine();
            }
            CreateEventClickForAreas(TextAreas.Values.ToArray());
            CreateEventEnteringMouseOnTextAreas(TextAreas.Values.ToArray());

            void CreateAndAlignLabel(string text, ContentAlignment align)
            {
                var image = Properties.Resources.PaperBackground;
                var label = GetLabel(text, new Point(labelX, labelY));
                label.Location = align == ContentAlignment.MiddleCenter
                    ? new Point((PaperLocation.X + image.Width - label.Width) / 2 + 115, label.Location.Y)
                    : new Point(PaperLocation.X + image.Width - label.Width - 420, label.Location.Y);
                TextAreas.Add(label.GetHashCode(), label);
            }

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
        /// Считывает тип ошибки, который дан в начале строки в квадратных скобках []
        /// </summary>
        /// <param name="label">Label с текстом, содержащим тип ошибки в []</param>
        /// <returns>Label с убранной пометкой в []</returns>
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
                    ExpectedMistakeAreas.Add(label.GetHashCode(), Mistakes.NoNumbers);
                    break;
                default: throw new ArgumentException("Такого типа ошибки не существует");
            }
            label.Text = label.Text.Substring(index);
            return label;
        }

        private void CreateEventClickForAreas(Label[] labels)
        {
            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].MouseDown += (sender, e) =>
                {
                    if (CheckHasMatching()) return;
                    var label = sender as Label;
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        SetSelectedTextArea(label);
                    else if (e.Button == System.Windows.Forms.MouseButtons.Right &&
                             SelectedMistakeAreas.ContainsKey(label.GetHashCode()))
                        Undo(label);
                };
            }
        }

        private void CreateEventEnteringMouseOnMistakeAreas(Label[] labels)
        {
            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].MouseEnter += (sender, e) =>
                {
                    var label = sender as Label;
                    mistakeDescription.Text = MistakesList[label.GetHashCode()].Item3;
                    controls.Add(mistakeDescription);
                    mistakeDescription.BringToFront();
                };
                labels[i].MouseLeave += (sender, e) => controls.Remove(mistakeDescription);
            }
        }

        private void CreateEventEnteringMouseOnTextAreas(Label[] labels)
        {
            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].MouseEnter += (sender, e) =>
                {
                    var label = sender as Label;
                    if (!SelectedMistakeAreas.ContainsKey(label.GetHashCode())) return;
                    textDescription.Text = $"Помечено как:\n{GetMistakeText(SelectedMistakeAreas[label.GetHashCode()]).name}" +
                    "\n\nКликните ЛКМ, чтобы изменить" +
                    "\nКликните ПКМ для отмены";
                    textDescription.Width = textDescription.PreferredWidth;
                    var x = label.Location.X;
                    if (x > 700) x = label.Location.X + label.Width - textDescription.Width;
                    textDescription.Location = new Point(x, label.Location.Y + label.Height);
                    controls.Add(textDescription);
                    textDescription.BringToFront();
                };
                labels[i].MouseLeave += (sender, e) => RemoveTextDescription(textDescription);
            }
        }

        public void Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.DrawImage(BookModes[CurrentBookMode], BookLocation.X, BookLocation.Y);
            graphics.DrawImage(Properties.Resources.ExitFromCheckMode, ExitButtonLocation.X, ExitButtonLocation.Y);
            graphics.DrawImage(Properties.Resources.PaperBackground, PaperLocation.X, PaperLocation.Y);
            if (IsMatching.Item1 != null)
                graphics.DrawLine(new Pen(colorSelectedTextArea),
                    IsMatching.Item1.Location.X, IsMatching.Item1.Location.Y + IsMatching.Item1.Height / 2,
                    IsMatching.Item2.Location.X + IsMatching.Item2.Width, IsMatching.Item2.Location.Y + IsMatching.Item2.Height / 2);
            switch (CurrentBookMode)
            {
                case Tabs.Guide: CreateTitle("Руководство главного редактора"); break;
                case Tabs.MistakesList: CreateTitle("Перечень ошибок"); break;
                case Tabs.CensoredList: CreateTitle("Цензурный перечень"); break;
            }

            void CreateTitle(string text) =>
                graphics.DrawString(text,
                                    new Font(StringStyle.FontFamily, 19, FontStyle.Bold),
                                    StringStyle.Brush,
                                    new Rectangle(15, 90, 500, 70),
                                    new StringFormat() { Alignment = StringAlignment.Center });
        }
    }
}
