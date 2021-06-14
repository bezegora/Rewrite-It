using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class MainOffice : IUserInterface
    {
        private readonly Controller controller;

        /// <summary>
        /// Функциональная кнопка "Одобрить"
        /// </summary>
        private readonly Label option1;

        /// <summary>
        /// Функциональная кнопка "Отклонить"
        /// </summary>
        private readonly Label option2;

        private readonly Label dayEndButton;

        public Character Person { get; }

        /// <summary>
        /// Содержит в себе диалоговые фразы, которые рисуются на блокноте последовательно друг за другом
        /// </summary>
        public List<Label> DialogPhrases { get; private set; }

        private readonly GraphicObject document = new GraphicObject(new Bitmap(Properties.Resources.Document, 210, 180), 20);
        private readonly GraphicObject notebook = new GraphicObject(new Bitmap(Properties.Resources.Notebook, 440, 540), new Point(100, 50));
        private readonly GraphicObject officeTable = new GraphicObject(new Bitmap(Properties.Resources.OfficeTable), new Point(-70, 600));
        private readonly GraphicObject headEditorBook = new GraphicObject(new Bitmap(Properties.Resources.HeadEditorBook2), new Point(110, 515));
        private readonly GraphicObject officeComputer = new GraphicObject(new Bitmap(Properties.Resources.OfficeComputer, 580, 530), new Point(900, 320));

        private readonly Action goToCheckMode;
        private readonly Action goToDayEnd;
        private readonly Action goToEmail;

        public string LetterText { get; private set; }

        public MainOffice(Controller controller, Action goToCheckMode, Action goToDayEnd, Action goToEmail)
        {
            this.goToEmail = goToEmail;
            this.goToCheckMode = goToCheckMode;
            this.goToDayEnd = goToDayEnd;
            option1 = new Label
            {
                Text = "Одобрить",
                Font = new Font(StringStyle.FontFamily, 19),
                AutoSize = true,
                ForeColor = Color.Green,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(-500, 0)
            };
            option2 = new Label
            {
                Text = "Отклонить",
                Font = new Font(StringStyle.FontFamily, 19),
                AutoSize = true,
                ForeColor = Color.Red,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(-500, 0)
            };
            Person = new Character(controller);
            DialogPhrases = new List<Label>();
            this.controller = controller;

            option1.MouseDown += new MouseEventHandler(OnClickOnOption);
            option2.MouseDown += new MouseEventHandler(OnClickOnOption);

            dayEndButton = new Label
            {
                Text = "Завершить день",
                Font = new Font(StringStyle.FontFamily, 24),
                AutoSize = true,
                ForeColor = Color.DarkBlue,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(1200, 770)
            };
            dayEndButton.MouseDown += (sender, e) =>
            {
                controller.Sounds.PlayClick();
                goToDayEnd();
            };
        }

        public void Paint(Graphics graphics)
        {
            Person.Paint(graphics);
            officeTable.Paint(graphics);
            notebook.Paint(graphics);
            headEditorBook.Paint(graphics);
            officeComputer.Paint(graphics);
            document.Paint(graphics);
        }

        /// <summary>
        /// Обновляет элементы управления для этого интерфейса.
        /// </summary>
        public void Change(Form1 form)
        {
            form.BackgroundImage = Properties.Resources.OfficeBackground;
            AuxiliaryMethods.AddLabelsToControls(controller.Form.Controls, DialogPhrases.ToArray());
            AuxiliaryMethods.AddLabelsToControls(controller.Form.Controls, option1, option2);
            controller.Sounds.PlayOfficeBackground();
            //controller.Form.Controls.Add(dayEndButton);
        }

        public void EnterCharacter()
        {
            if (controller.Level.Events.Count == 0)
            {
                controller.Form.Controls.Add(dayEndButton);
                return;
            }
            ClearDialog();
            Person.SetImage(controller.Level.Events.Peek().character);
            Person.LetIn();
            controller.Sounds.PlayDoorEnter();

            if (LetterText != null)
            {
                controller.Email.AddLetter(controller.Stats.Date, "Ген. директор", "Отчёт по ошибкам", LetterText);
                controller.Sounds.PlayMessage();
                officeComputer.Bitmap = new Bitmap(Properties.Resources.OfficeComputerMessage, 580, 530);
                LetterText = null;
            }
        }

        /// <summary>
        /// Добавляет в список очередную диалоговую фразу, которую нужно отобразить на форме.
        /// Причём при нехватке места на блокноте все предыдущие фразы автоматически сдвигаются вверх, 
        /// чтобы освободилось место для этой новой фразы.
        /// </summary>
        /// <param name="text">Текст фразы</param>
        /// <param name="align">Выравнивание. По умолчанию по правому краю</param>
        public void AddNewDialogPhrase(string text, Color color, ContentAlignment align = ContentAlignment.MiddleRight)
        {
            var yStep = 15;
            var x = 150;
            var y = 70;
            if (DialogPhrases.Count != 0)
            {
                var lastPhrase = DialogPhrases[DialogPhrases.Count - 1];
                y = lastPhrase.Location.Y + lastPhrase.Height + yStep;
            }
            var label = new Label()
            {
                Text = text,
                Font = StringStyle.TextFont,
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = color,
                MaximumSize = new Size(350, 600),
                Location = new Point(x, y)
            };
            label.Width = label.PreferredWidth;
            label.Height = label.PreferredHeight;
            DialogPhrases.Add(label);
            while (label.Location.Y + label.Height + yStep > 550)
            {
                var removedPhrase = DialogPhrases[0];
                controller.Form.Controls.Remove(removedPhrase);
                DialogPhrases.RemoveAt(0);
                foreach (var phrase in DialogPhrases)
                    phrase.Location = new Point(phrase.Location.X, phrase.Location.Y - removedPhrase.Height - yStep);
            }
            AlignDialogPhrase(label);
            if (controller.CurrentInterface == this)
                AuxiliaryMethods.AddLabelsToControls(controller.Form.Controls, label);

            void AlignDialogPhrase(Label phrase)
            {
                phrase.TextAlign = align;
                switch (align)
                {
                    case ContentAlignment.MiddleRight:
                        phrase.Location = new Point(
                            notebook.Position.X + Properties.Resources.Notebook.Width - phrase.Width - 120,
                            phrase.Location.Y);
                        break;
                    default: return;
                }
            }
        }

        private void OnClickOnOption(object sender, MouseEventArgs e)
        {
            controller.Sounds.PlayChosenOption();
            RemoveOptions();
            var label = (Label)sender;
            var wait = new Timer { Interval = 700 };
            document.Position = Form1.Beyond;
            wait.Tick += (s, arg) =>
            {
                if (label.Text == "Одобрить" || label.Text == "Отклонить")
                {
                    CheckForMistakes();
                    if (label.Text == "Одобрить")
                    {
                        controller.Level.AddIncreaseToPopularity(2 - controller.CheckMode.ExpectedMistakeAreas.Count);
                        GameEvents.ApprovingArticle();
                    }
                    else GameEvents.RejectionArticle();
                }
                wait.Stop();
            };
            wait.Start();
        }

        private void RemoveOptions()
        {
            option1.Location = Form1.Beyond;
            option2.Location = Form1.Beyond;
        }

        /// <summary>
        /// Проводит процедуру проверки найденных ошибок с ожидаемыми.
        /// Формирует текст сообщения о допущенных игроком неточностей, которое придёт на электронную почту.
        /// Очищает все коллекции с полями текста и ошибками, подготавливая их для следующей статьи.
        /// </summary>
        private void CheckForMistakes()
        {
            var expectedMistakes = controller.CheckMode.ExpectedMistakeAreas;
            var selectedMistakes = controller.CheckMode.SelectedMistakeAreas;
            var textAreas = controller.CheckMode.TextAreas;
            //var increaseInPopularity = 2;
            var letterText = new StringBuilder();

            foreach (var selectedMistake in selectedMistakes)
            {
                var hash = selectedMistake.Key;
                var mistake = selectedMistake.Value;
                var mistakeName = controller.CheckMode.GetMistakeText(mistake.Type).name;
                if (!expectedMistakes.ContainsKey(hash))
                {
                    letterText.Append($"В области\r\n\"{textAreas[hash].Text}\"\r\nошибки \"{mistakeName}\" нет.\r\n\r\n");
                    //increaseInPopularity--;
                }
                else
                {
                    if (mistake.Type != expectedMistakes[hash].Type)
                    {
                        var expectedMistake = expectedMistakes[hash];
                        letterText.Append($"В области\r\n\"{textAreas[hash].Text}\"\r\nошибка \"{mistakeName}\" отмечена неверно. " +
                            $"Правильная ошибка: \"{controller.CheckMode.GetMistakeText(expectedMistake.Type).name}\".\r\n");
                        if (expectedMistake.Explanation != "") letterText.Append(expectedMistake.Explanation + "\r\n");
                        letterText.Append("\r\n");
                        //increaseInPopularity--;
                    }
                    else controller.Level.IncreaseMistakesFound();
                    expectedMistakes.Remove(hash);
                }
            }

            foreach (var expectedMistake in expectedMistakes)
            {
                var hash = expectedMistake.Key;
                var mistake = expectedMistake.Value;
                letterText.Append($"В области\r\n\"{textAreas[hash].Text}\"\r\nпропущена ошибка " +
                $"\"{controller.CheckMode.GetMistakeText(mistake.Type).name}\".\r\n");
                    if (mistake.Explanation != "") letterText.Append(mistake.Explanation + "\r\n");
                letterText.Append("\r\n");
                //increaseInPopularity--;
            }

            expectedMistakes.Clear();
            selectedMistakes.Clear();
            textAreas.Clear();
            //controller.Level.AddIncreaseToPopularity(increaseInPopularity);
            controller.Level.IncreaseVerifiedArticles();
            if (letterText.Length != 0)
            {
                letterText.Append("С уважением," +
                    "\r\nГенеральный директор Флорин Н. С.");
                LetterText = letterText.ToString();
            }
        }

        public void ClearDialog()
        {
            foreach (var phrase in DialogPhrases)
                controller.Form.Controls.Remove(phrase);
            DialogPhrases.Clear();
        }

        private bool IsClickedDocument() => 
            AuxiliaryMethods.CursorIsHoveredArea(new Rectangle(document.Position, document.Bitmap.Size));

        private bool IsClickedComputer() =>
            AuxiliaryMethods.CursorIsHoveredArea(new Rectangle(officeComputer.Position + new Size(130, 50), officeComputer.Bitmap.Size - new Size(175, 200)));

        public void CreateDocument()
        {
            controller.Sounds.PlayPaper();
            document.Position = new Point(650, 630);
            option1.Location = new Point(150, 520);
            option2.Location = new Point(317, 520);
        }

        public void Tick()
        {
            Person.Tick();
        }

        public void MouseDown() 
        {
            if (IsClickedDocument())
                goToCheckMode();
            else if (IsClickedComputer())
            {
                officeComputer.Bitmap = new Bitmap(Properties.Resources.OfficeComputer, 580, 530);
                goToEmail();
            }
            else GameEvents.SkipPhrase();
        }
    }
}
