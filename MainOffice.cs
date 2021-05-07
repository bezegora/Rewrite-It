using System;
using System.Collections.Generic;
using System.Drawing;
using System.Media;
using System.Text;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class MainOffice
    {
        private readonly Form1 form;

        /// <summary>
        /// Функциональная кнопка "Одобрить"
        /// </summary>
        public Label Option1 { get; }

        /// <summary>
        /// Функциональная кнопка "Отклонить"
        /// </summary>
        public Label Option2 { get; }

        public Character Person { get; }

        /// <summary>
        /// Содержит в себе диалоговые фразы, которые рисуются на блокноте последовательно друг за другом
        /// </summary>
        public List<Label> DialogPhrases { get; private set; }

        public Point DocumentLocation { get; set; } = new Point(-500, 0);
        public Point NotebookLocation { get; set; } = new Point(100, 50);

        public string LetterText { get; private set; }

        public SoundPlayer PlayerBackground { get; } = new SoundPlayer(Properties.Resources.Background);

        public MainOffice(Label option1, Label option2, Character person, Form1 form)
        {
            Option1 = option1;
            Option2 = option2;
            Person = person;
            DialogPhrases = new List<Label>();
            this.form = form;

            Option1.MouseDown += new MouseEventHandler(OnClickOnOption);
            Option2.MouseDown += new MouseEventHandler(OnClickOnOption);
        }

        public void Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.DrawImage(Person.Images[Person.CurrentImage], Person.Location.X, Person.Location.Y);
            graphics.DrawImage(Properties.Resources.OfficeTable, -70, 600);
            graphics.DrawImage(Properties.Resources.Notebook, NotebookLocation.X, NotebookLocation.Y);
            graphics.DrawImage(Properties.Resources.HeadEditorBook, 150, 625);
            graphics.DrawImage(Properties.Resources.OfficeComputer, 1020, 230);
            graphics.DrawImage(Properties.Resources.Document, DocumentLocation.X, DocumentLocation.Y);
        }

        /// <summary>
        /// Обновляет элементы управления для этого интерфейса.
        /// </summary>
        public void UpdateStatus()
        {
            AddLabelsToControls(DialogPhrases.ToArray());
            AddLabelsToControls(Option1, Option2);
        }

        public void EnterCharacter()
        {
            //Здесь должно быть прописано завершение уровня, если очередь пуста.
            if (form.Level.Events.Count == 0) return;
            ClearDialog();
            Person.Direction = MovingDirections.Right;
            Person.CurrentImage = form.Level.Events.Peek().character;
            Person.StartMoving();
            form.PlaySound(Properties.Resources.DoorEnter);

            if (LetterText != null)
            {
                form.Email.AddLetter("От: Генеральный директор Флорин Н. С.", LetterText);
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
                Font = StringStyle.Font,
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
                form.Controls.Remove(removedPhrase);
                DialogPhrases.RemoveAt(0);
                foreach (var phrase in DialogPhrases)
                    phrase.Location = new Point(phrase.Location.X, phrase.Location.Y - removedPhrase.Height - yStep);
            }
            AlignDialogPhrase(label);
            AddLabelsToControls(label);

            void AlignDialogPhrase(Label phrase)
            {
                phrase.TextAlign = align;
                switch (align)
                {
                    case ContentAlignment.MiddleRight:
                        phrase.Location = new Point(
                            NotebookLocation.X + Properties.Resources.Notebook.Width - phrase.Width - 120,
                            phrase.Location.Y);
                        break;
                    default: return;
                }
            }
        }

        private void OnClickOnOption(object sender, MouseEventArgs e)
        {
            form.PlaySound(Properties.Resources.ChosenOption);
            RemoveOptions();
            var label = (Label)sender;
            var wait = new Timer { Interval = 700 };
            DocumentLocation = new Point(-500, 0);
            wait.Tick += (s, arg) =>
            {
                if (label.Text == "Одобрить" || label.Text == "Отклонить")
                {
                    CheckForMistakes();
                    if (label.Text == "Одобрить") GameEvents.ApprovingArticle();
                    else GameEvents.RejectionArticle();
                }
                wait.Stop();
            };
            wait.Start();
        }

        private void RemoveOptions()
        {
            Option1.Location = new Point(-500, 0);
            Option2.Location = new Point(-500, 0);
        }

        /// <summary>
        /// Проводит процедуру проверки найденных ошибок с ожидаемыми.
        /// Формирует текст сообщения о допущенных игроком неточностей, которое придёт на электронную почту.
        /// Очищает все коллекции с полями текста и ошибками, подготавливая их для следующей статьи.
        /// </summary>
        private void CheckForMistakes()
        {
            var expectedMistakes = form.CheckMode.ExpectedMistakeAreas;
            var selectedMistakes = form.CheckMode.SelectedMistakeAreas;
            var textAreas = form.CheckMode.TextAreas;
            var increaseInPopularity = 2;
            var letterText = new StringBuilder();

            foreach (var selectedMistake in selectedMistakes)
            {
                var hash = selectedMistake.Key;
                var mistake = selectedMistake.Value;
                var mistakeName = form.CheckMode.GetMistakeText(mistake.Type).name;
                if (!expectedMistakes.ContainsKey(hash))
                {
                    letterText.Append($"В области\n\"{textAreas[hash].Text}\"\nошибки \"{mistakeName}\" нет.\n\n");
                    increaseInPopularity--;
                }
                else
                {
                    if (mistake.Type != expectedMistakes[hash].Type)
                    {
                        var expectedMistake = expectedMistakes[hash];
                        letterText.Append($"В области\n\"{textAreas[hash].Text}\"\nошибка \"{mistakeName}\" отмечена неверно. " +
                            $"Правильная ошибка: \"{form.CheckMode.GetMistakeText(expectedMistake.Type).name}\".\n");
                        if (expectedMistake.Explanation != "") letterText.Append(expectedMistake.Explanation + "\n");
                        letterText.Append("\n");
                        increaseInPopularity--;
                    }
                    else form.Level.IncreaseMistakesFound();
                    expectedMistakes.Remove(hash);
                }
            }

            foreach (var expectedMistake in expectedMistakes)
            {
                var hash = expectedMistake.Key;
                var mistake = expectedMistake.Value;
                letterText.Append($"В области\n\"{textAreas[hash].Text}\"\nпропущена ошибка " +
                $"\"{form.CheckMode.GetMistakeText(mistake.Type).name}\".\n");
                    if (mistake.Explanation != "") letterText.Append(mistake.Explanation + "\n");
                letterText.Append("\n");
                increaseInPopularity--;
            }

            expectedMistakes.Clear();
            selectedMistakes.Clear();
            textAreas.Clear();
            form.Level.AddIncreaseToPopularity(increaseInPopularity);
            form.Level.IncreaseVerifiedArticles();
            LetterText = letterText.ToString();
        }

        public void ClearDialog()
        {
            foreach (var phrase in DialogPhrases)
                form.Controls.Remove(phrase);
            DialogPhrases.Clear();
        }

        public void AddLabelsToControls(params Label[] labels)
        {
            if (!(Form1.CurrentInterface is Interface.MainOffice)) return;
            foreach (var label in labels)
                form.Controls.Add(label);
        }
    }
}
