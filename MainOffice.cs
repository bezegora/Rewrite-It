using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rewrite_It
{
    class MainOffice
    {
        /// <summary>
        /// Вызов метода Invalidate()
        /// </summary>
        private readonly Action updateGraphics;

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
        public List<string> DialogPhrases { get; set; }

        public Point DocumentLocation { get; set; }

        private readonly CheckMode checkMode;

        public MainOffice(Label option1, Label option2, Character person, Action update, CheckMode checkMode)
        {
            Option1 = option1;
            Option2 = option2;
            Person = person;
            DialogPhrases = new List<string>();
            updateGraphics = update;
            DocumentLocation = new Point(-500, 0);
            this.checkMode = checkMode;
        }

        /// <summary>
        /// Содержит все возможные события (сюжетные и не сюжетные), привязанные к различным персонажам
        /// </summary>
        public enum Events
        {
            Article
        }

        public void Paint(PaintEventArgs e)
        {
            var graphics = e.Graphics;
            graphics.DrawImage(Person.Images[Person.CurrentImage], Person.Location.X, Person.Location.Y);
            graphics.DrawImage(Properties.Resources.OfficeTable, -70, 600);
            graphics.DrawImage(Properties.Resources.Notebook, 100, 50);
            graphics.DrawImage(Properties.Resources.HeadEditorBook, 150, 625);
            graphics.DrawImage(Properties.Resources.OfficeComputer, 1020, 230);
            graphics.DrawImage(Properties.Resources.Document, DocumentLocation.X, DocumentLocation.Y);
            for (var i = 0; i < DialogPhrases.Count; i++)
                graphics.DrawString(DialogPhrases[i], new Font(StringStyle.FontFamily, 16),
                    new SolidBrush(Color.Black), new Rectangle(150, 70 + i * 70, 350, 70),
                    new StringFormat() { Alignment = StringAlignment.Far });
        }

        /// <summary>
        /// Запускает движение персонажа по офису с заданным начальным положением
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
        /// Запускает движение персонажа по офису
        /// </summary>
        /// <param name="character"></param>
        /// <param name="timerGraphicsUpdate"></param>
        public void EnterCharacter(Character character, Timer timerGraphicsUpdate)
        {
            character.IsMoving = true;
            timerGraphicsUpdate.Start();
        }

        /// <summary>
        /// Прекращает движение персонажа по офису
        /// </summary>
        /// <param name="character"></param>
        /// <param name="timerGraphicsUpdate"></param>
        public void StopCharacter(Character character, Timer timerGraphicsUpdate)
        {
            character.IsMoving = false;
            timerGraphicsUpdate.Stop();
        }

        /// <summary>
        /// Запускает очередное событие, связанное с данным персонажем
        /// </summary>
        /// <param name="_event"></param>
        public void StartEvent(MainOffice.Events _event)
        {
            switch (_event)
            {
                case MainOffice.Events.Article: StartEventArticle(new StreamReader(@"Articles\Marketing1.txt")); break;
            }
        }

        private void StartEventArticle(StreamReader textFile)
        {
            //checkMode.ArticleText = textFile;
            checkMode.CreateArticleText(textFile);

            DialogPhrases.Add("Добрый день!");
            var waitingTimer = new Timer { Interval = 3000 };
            var expectedDialogPhrases = new Queue<string>();

            expectedDialogPhrases.Enqueue("Вот, пожалуйста.");
            expectedDialogPhrases.Enqueue("Сегодня хорошая погода, не правда ли?");

            waitingTimer.Tick += ((sender, e) =>
            {
                DialogPhrases.Add(expectedDialogPhrases.Dequeue());
                if (DialogPhrases.Count == 2)
                {
                    DocumentLocation = new Point(650, 650);
                    Option1.Location = new Point(150, 520);
                    Option2.Location = new Point(317, 520);
                }
                updateGraphics();
                if (expectedDialogPhrases.Count == 0) waitingTimer.Stop();
            });
            waitingTimer.Start();
        }
    }
}
