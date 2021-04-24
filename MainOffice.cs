using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class MainOffice
    {
        /// <summary>
        /// Вызов метода Invalidate()
        /// </summary>
        public readonly Action updateGraphics;

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
        public List<Label> DialogPhrases { get; set; }

        public Point DocumentLocation { get; set; } = new Point(-500, 0);
        public Point NotebookLocation { get; set; } = new Point(100, 50);

        //private readonly CheckMode checkMode;
        private readonly Control.ControlCollection controls;

        public MainOffice(Label option1, Label option2, Character person, Action update, //CheckMode checkMode,
                          Control.ControlCollection controls)
        {
            Option1 = option1;
            Option2 = option2;
            Person = person;
            DialogPhrases = new List<Label>();
            updateGraphics = update;
            //this.checkMode = checkMode;
            this.controls = controls;
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

        /// <summary>
        /// Запускает движение персонажа по офису с заданным начальным положением.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="timerGraphicsUpdate"></param>
        /// <param name="initialLocation"></param>
        public void EnterCharacter(Character character, Timer timerGraphicsUpdate, Point initialLocation)
        {
            character.Location = initialLocation;
            EnterCharacter(character, timerGraphicsUpdate);
        }

        /// <summary>
        /// Запускает движение персонажа по офису.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="timerGraphicsUpdate"></param>
        public void EnterCharacter(Character character, Timer timerGraphicsUpdate)
        {
            character.IsMoving = true;
            timerGraphicsUpdate.Start();
        }

        /// <summary>
        /// Прекращает движение персонажа по офису.
        /// </summary>
        /// <param name="character"></param>
        /// <param name="timerGraphicsUpdate"></param>
        public void StopCharacter(Character character, Timer timerGraphicsUpdate)
        {
            character.IsMoving = false;
            timerGraphicsUpdate.Stop();
        }

        /// <summary>
        /// Добавляет в список очередную диалоговую фразу, которую нужно отобразить на форме.
        /// Причём при нехватке места на блокноте все предыдущие фразы автоматически сдвигаются вверх, 
        /// чтобы освободилось место для этой новой фразы.
        /// </summary>
        /// <param name="text">Текст фразы</param>
        /// <param name="align">Выравнивание. По умолчанию по правому краю</param>
        public void AddNewDialogPhrase(string text, ContentAlignment align = ContentAlignment.MiddleRight)
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
                MaximumSize = new Size(350, 600),
                Location = new Point(x, y)
            };
            label.Width = label.PreferredWidth;
            label.Height = label.PreferredHeight;
            DialogPhrases.Add(label);
            while (label.Location.Y + label.Height + yStep > 550)
            {
                var removedPhrase = DialogPhrases[0];
                controls.Remove(removedPhrase);
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

        public void AddLabelsToControls(params Label[] labels)
        {
            if (!(Form1.CurrentInterface is Interface.MainOffice)) return;
            foreach (var label in labels)
                controls.Add(label);
        }
    }
}
