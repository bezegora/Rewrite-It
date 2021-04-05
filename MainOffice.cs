using System;
using System.Collections.Generic;
using System.Drawing;
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

        public MainOffice(Label option1, Label option2, Character person, Action update)
        {
            Option1 = option1;
            Option2 = option2;
            Person = person;
            DialogPhrases = new List<string>();
            this.updateGraphics = update;
        }

        /// <summary>
        /// Содержит все возможные события (сюжетные и не сюжетные), привязанные к различным персонажам
        /// </summary>
        public enum Events
        {
            Article
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
                case MainOffice.Events.Article:
                    StartEventArticle(new ArticleText());
                    break;
            }
        }

        private void StartEventArticle(ArticleText articleText)
        {
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
