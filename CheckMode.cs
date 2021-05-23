using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Text;

namespace Rewrite_It
{
    public class CheckMode
    {
        /// <summary>
        /// Содержит фреймы книги с соответствующими открытыми вкладками
        /// </summary>
        public Dictionary<Tab, Bitmap> BookModes { get; }

        /// <summary>
        /// Текщая открытая вкладка в книге
        /// </summary>
        public Tab CurrentBookMode { get; private set; } = Tab.Guide;

        private readonly ExitButton exitButton;
        private readonly GraphicObject paper = new GraphicObject(new Bitmap(Properties.Resources.PaperBackground, 740, 840), new Point(650, 0));

        //private readonly GraphicObject book = new GraphicObject(new Bitmap(Properties.Resources.HeadEditorBook2, 1338, 1146))
        public Point BookLocation { get; set; } = new Point(-10, 10);

        /// <summary>
        /// Соедржит все области с типами ошибок, которые рисуются непросредственно на форме.
        /// Key - идентификатор (HashCode) области.
        /// Value.Item1 - ошибка, относящаящая к данной области.
        /// Value.Item2 - сама область.
        /// Value.Item3 - описание ошибки.
        /// </summary>
        public Dictionary<int, (MistakeType, Label, string)> MistakesList { get; } 
            = new Dictionary<int, (MistakeType mistake, Label area, string description)>();

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
        public Dictionary<int, Mistake> ExpectedMistakeAreas { get; } = new Dictionary<int, Mistake>();

        /// <summary>
        /// Содержит выбранные игроком ошибочные области текста.
        /// Key - идентификатор (HashCode) области.
        /// Value - ошибка, которую выбрал игрок для данной области.
        /// </summary>
        public Dictionary<int, Mistake> SelectedMistakeAreas { get; } = new Dictionary<int, Mistake>();

        /// <summary>
        /// Текущая выбранная область текста.
        /// Возвращает null, если область не выбрана.
        /// </summary>
        public Label SelectedTextArea { get; set; }

        private readonly List<string> guidePage = new List<string>()
        { "Здравствуйте, редактор!" +
            "\n\nС сегодняшнего дня Вы ответственны за популярность журнала и за все влекущиеся изменения." +
            "\n\nОбращаем Ваше внимание на изменение популярности в зависимости от количества найденных ошибок:" +
            "\n\n0-1 ошибка:  + к популярности" +
            "\n2 ошибки:  популярность не изменится" +
            "\n3+ ошибок:  - к популярности"
        };

        private bool showGuideText;

        /// <summary>
        /// Кортеж областей, которые сопоставляются прямо сейчас.
        /// Если оба элемента != null, любые клики мышью по объектам игнорируются.
        /// </summary>
        public (Label, Label) IsMatching { get; private set; }

        /// <summary>
        /// Цвет выбранной текстовой области
        /// </summary>
        private readonly Color colorSelectedTextArea = Color.CornflowerBlue;
        private int currentMistakeY = 170;

        /// <summary>
        /// Описание текстовой области.
        /// </summary>
        private readonly Label descriptionTextArea;

        /// <summary>
        /// Описание ошибки.
        /// </summary>
        private readonly Label descriptionMistake;

        private readonly Controller controller;

        public CheckMode(Controller controller)
        {
            exitButton = new ExitButton(controller.Sounds);
            this.controller = controller;
            BookModes = new Dictionary<Tab, Bitmap>
            {
                [Tab.Guide] = new Bitmap(Properties.Resources.HeadBookTab1),
                [Tab.MistakesList] = new Bitmap(Properties.Resources.HeadBookTab2),
                [Tab.CensoredList] = new Bitmap(Properties.Resources.HeadBookTab3)
            };
            descriptionTextArea = new Label()
            {
                Font = StringStyle.TextFont,
                ForeColor = Color.DarkSlateGray,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                MaximumSize = new Size(500, 0)
            };
            descriptionMistake = new Label()
            {
                Font = StringStyle.TextFont,
                ForeColor = Color.DarkSlateGray,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(542, 73),
                MaximumSize = new Size(600, 0)
            };

            AddMistake(MistakeType.NoNumbers);
            AddMistake(MistakeType.IncorrectDefinitionTargetAudience);
            AddMistake(MistakeType.TooMuchDetails);
            AddMistake(MistakeType.ConfusedParagraphs);

            var mistakesList = MistakesList.Values.Select(tuple => tuple.Item2).ToArray();

            CreateEventEnteringMouseOnMistakeAreas(mistakesList);
            CreateEventClickForAreas(mistakesList);
        }

        /// <summary>
        /// Устанавливает текущую открытую вкладку книги.
        /// Обновляет элементы управления для данного интерфейса.
        /// </summary>
        /// <param name="tab">Вкладка, которую нужно открыть</param>
        public void UpdateStatus(Tab tab)
        {
            CurrentBookMode = tab;
            showGuideText = false;
            var listMistakeLabels = MistakesList.Values.Select(tuple => tuple.Item2);
            foreach (var e in listMistakeLabels) controller.Form.Controls.Remove(e);
            if (SelectedTextArea != null && MistakesList.ContainsKey(SelectedTextArea.GetHashCode()))
                SelectedTextArea = null;
            if (tab == Tab.MistakesList)
            {
                foreach (var e in listMistakeLabels) controller.Form.Controls.Add(e);
            }
            if (tab == Tab.Guide)
                showGuideText = true;

            foreach (var e in TextAreas.Values) controller.Form.Controls.Add(e);
        }

        /// <summary>
        /// Возвращает информацию о существующем типе ошибки.
        /// </summary>
        /// <param name="mistake">Тип ошибки, информацию о которой нужно получить</param>
        /// <returns>Кортеж, состоящий из названия и описания типа данной ошибки</returns>
        public (string name, string description) GetMistakeText(MistakeType mistake)
        {
            switch (mistake)
            {
                case MistakeType.NoNumbers: return
                        ("Отсутствие чисел",
                        "Тексты без цифр и статистики менее убедительны");
                case MistakeType.IncorrectDefinitionTargetAudience: return
                        ("Неверное таргетирование целевой аудитории",
                        "Вы должны четко понимать группу людей, для которой пишете свое послание." +
                        "\nПромах по целевой аудитории является серьёзным ударом по убедительности");
                case MistakeType.TooMuchDetails: return
                        ("Слишком много деталей",
                        "Переизбыток деталей мешает читателю уловить логическую нить.");
                case MistakeType.ConfusedParagraphs: return 
                        ("Перепутаны абзацы",
                            "Если абзацы перепутаны местами, то нарушена логика повествования," +
                            " что влияет на восприятие текста");
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
                    controller.Sounds.PlayCancel();
                    return;
                }
                SelectedTextArea = nextArea;
                if (nextArea != null)
                {
                    nextArea.BackColor = colorSelectedTextArea;
                    controller.Sounds.PlayChosenOption();
                }
            }
        }

        /// <summary>
        /// Устанавливает области, для которых нужно проиграть анимацию сопоставления.
        /// </summary>
        /// <param name="textArea"></param>
        /// <param name="mistakeArea"></param>
        public void SetIsMatching(Label textArea = null, Label mistakeArea = null) => IsMatching = (textArea, mistakeArea);

        /// <summary>
        /// Проверяет, идёт ли прямо сейчас сопоставление областей.
        /// </summary>
        /// <returns></returns>
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
                if (SelectedMistakeAreas[textHash].Type == MistakesList[mistakeHash].Item1) return;
                SelectedMistakeAreas.Remove(textHash);
            }
            controller.Sounds.PlayClick();
            textArea.BackColor = colorSelectedTextArea;
            mistakeArea.BackColor = colorSelectedTextArea;
            RemoveTextDescription(descriptionMistake);
            SelectedMistakeAreas.Add(textArea.GetHashCode(), new Mistake(MistakesList[mistakeArea.GetHashCode()].Item1));
            SetIsMatching(textArea, mistakeArea);
            var wait = new Timer() { Interval = 1000 };
            wait.Start();
            wait.Tick += (sender, e) =>
            {
                SetIsMatching();
                textArea.BackColor = Color.Transparent;
                mistakeArea.BackColor = Color.Transparent;
                textArea.ForeColor = Color.Red;
                //SetSelectedTextArea(null);
                SelectedTextArea = null;
                wait.Stop();
            };
        }

        /// <summary>
        /// Отменает указание ошибки для текстовой области.
        /// </summary>
        /// <param name="textArea">Текстовая область, из которой нужно убрать указание ошибки</param>
        private void Undo(Label textArea)
        {
            SelectedMistakeAreas.Remove(textArea.GetHashCode());
            textArea.ForeColor = StringStyle.Brush.Color;
            //SetSelectedTextArea(null);
            SelectedTextArea = null;
            RemoveTextDescription(descriptionTextArea);
            controller.Sounds.PlayPromotion();
        }

        /// <summary>
        /// Заставлет исчезнуть всплывающее окно с описанием.
        /// </summary>
        /// <param name="popUpWindow"></param>
        private void RemoveTextDescription(Label popUpWindow) => controller.Form.Controls.Remove(popUpWindow);

        /// <summary>
        /// Добавляет новую область с ошибкой в словарь, из которого эти области рисуются на форме последовательно друг за другом.
        /// </summary>
        /// <param name="mistake"></param>
        public void AddMistake(MistakeType mistake)
        {
            var (name, description) = GetMistakeText(mistake);
            var label = new Label()
            {
                Text = name,
                Font = StringStyle.TextFont,
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
            var labelX = paper.Position.X + 10;
            var labelY = paper.Position.Y + 5;

            CreateAndAlignLabel(textFile.ReadLine(), ContentAlignment.MiddleCenter);
            CreateAndAlignLabel(textFile.ReadLine(), ContentAlignment.MiddleRight);
            CreateAndAlignLabel(textFile.ReadLine(), ContentAlignment.MiddleRight);

            var line = textFile.ReadLine();
            while (line != null)
            {
                var textArea = line;
                Mistake mistake = null;
                if (textArea[0] == '[') (textArea, mistake) = ReadMistakeName(textArea);
                var area = GetLabel(textArea, new Point(labelX, labelY));
                if (mistake != null) ExpectedMistakeAreas.Add(area.GetHashCode(), mistake);
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
                    ? new Point((paper.Position.X + image.Width - label.Width) / 2 + 115, label.Location.Y)
                    : new Point(paper.Position.X + image.Width - label.Width - 420, label.Location.Y);
                TextAreas.Add(label.GetHashCode(), label);
            }

            Label GetLabel(string text, Point location)
            {
                var result = new Label()
                {
                    Text = text,
                    Font = StringStyle.TextFont,
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
        /// Считывает тип ошибки, который дан в начале строки в квадратных скобках [].
        /// Считывает, если есть, пояснение к ошибке, которое дано после запятой в тех же квадратных скобках [].
        /// </summary>
        /// <param name="text">Текст, содержащий тип ошибки (и пояснение к ней, если есть) в []</param>
        /// <returns>(Текст с убранной пометкой в [], Ошибка, содержащаяся в данном тексте)</returns>
        private (string, Mistake) ReadMistakeName(string text)
        {
            var mistakeName = new StringBuilder();
            var explanation = new StringBuilder();
            var index = 1;
            var wasSeparator = false;
            try
            {
                while (text[index] != ']')
                {
                    if (text[index] == ',' && !wasSeparator)
                    {
                        wasSeparator = true;
                        index += 2;
                        continue;
                    }
                    if (wasSeparator) explanation.Append(text[index]);
                    else mistakeName.Append(text[index]);
                    index++;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                throw new Exception("Закрывающая скобка \"]\" не была найдена", e);
            }
            index += 2;

            var mistake = new Mistake();
            mistake.SetType(mistakeName.ToString());
            mistake.Explanation = explanation.ToString();
            text = text.Substring(index);
            return (text, mistake);
        }

        /// <summary>
        /// Создаёт событие клика мышью по заданным текстовым областям.
        /// </summary>
        /// <param name="labels"></param>
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

        /// <summary>
        /// Создаёт событие наведения курсора мыши на заданные области с типами ошибок.
        /// </summary>
        /// <param name="labels"></param>
        private void CreateEventEnteringMouseOnMistakeAreas(Label[] labels)
        {
            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].MouseEnter += (sender, e) =>
                {
                    var label = sender as Label;
                    descriptionMistake.Text = MistakesList[label.GetHashCode()].Item3;
                    controller.Form.Controls.Add(descriptionMistake);
                    descriptionMistake.BringToFront();
                };
                labels[i].MouseLeave += (sender, e) => controller.Form.Controls.Remove(descriptionMistake);
            }
        }

        /// <summary>
        /// Создаёт событие наведения курсора мыши на заданные текстовые области.
        /// </summary>
        /// <param name="labels"></param>
        private void CreateEventEnteringMouseOnTextAreas(Label[] labels)
        {
            for (var i = 0; i < labels.Length; i++)
            {
                labels[i].MouseEnter += (sender, e) =>
                {
                    var label = sender as Label;
                    if (!SelectedMistakeAreas.ContainsKey(label.GetHashCode())) return;
                    descriptionTextArea.Text = $"Помечено как:\n{GetMistakeText(SelectedMistakeAreas[label.GetHashCode()].Type).name}" +
                    "\n\nКликните ЛКМ, чтобы изменить" +
                    "\nКликните ПКМ для отмены";
                    descriptionTextArea.Width = descriptionTextArea.PreferredWidth;
                    var x = label.Location.X;
                    if (x > 700) x = label.Location.X + label.Width - descriptionTextArea.Width;
                    descriptionTextArea.Location = new Point(x, label.Location.Y + label.Height);
                    controller.Form.Controls.Add(descriptionTextArea);
                    descriptionTextArea.BringToFront();
                };
                labels[i].MouseLeave += (sender, e) => RemoveTextDescription(descriptionTextArea);
            }
        }

        public void Paint(Graphics graphics)
        {
            graphics.DrawImage(BookModes[CurrentBookMode], BookLocation.X, BookLocation.Y);
            exitButton.Paint(graphics);
            paper.Paint(graphics);
            if (IsMatching.Item1 != null)
                graphics.DrawLine(new Pen(colorSelectedTextArea),
                    IsMatching.Item1.Location.X, IsMatching.Item1.Location.Y + IsMatching.Item1.Height / 2,
                    IsMatching.Item2.Location.X + IsMatching.Item2.Width, IsMatching.Item2.Location.Y + IsMatching.Item2.Height / 2);
            switch (CurrentBookMode)
            {
                case Tab.Guide: CreateTitle("Руководство главного редактора"); break;
                case Tab.MistakesList: CreateTitle("Перечень ошибок"); break;
                case Tab.CensoredList: CreateTitle("Цензурный перечень"); break;
            }

            if (showGuideText)
                graphics.DrawString(guidePage[0], StringStyle.TextFont, StringStyle.Brush, new Rectangle(
                    BookLocation.X + 20, BookLocation.Y + 160, 500, 600));

            void CreateTitle(string text) =>
                graphics.DrawString(text,
                                    new Font(StringStyle.FontFamily, 19, FontStyle.Bold),
                                    StringStyle.Brush,
                                    new Rectangle(15, 90, 500, 70),
                                    new StringFormat() { Alignment = StringAlignment.Center });
        }

        public bool IsClickedExitButton() => AuxiliaryMethods.CursorIsHoveredArea(new Rectangle(exitButton.Position, exitButton.Bitmap.Size));

        public void Tick()
        {
            exitButton.Tick();
        }
    }
}
