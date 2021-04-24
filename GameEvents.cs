using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class GameEvents
    {
        private static LevelParameters level;
        private static MainOffice office;
        private static CheckMode checkMode;

        public static void InitializeComponents(LevelParameters iLevel, MainOffice iOffice, CheckMode iCheckMode)
        {
            if (iLevel == null || iOffice == null || iCheckMode == null)
                throw new ArgumentException("Один из компонентов имел значение null");
            level = iLevel;
            office = iOffice;
            checkMode = iCheckMode;
        }

        /// <summary>
        /// Запускает стандартное событие, ориентированное исключительно на проверку статьи.
        /// </summary>
        /// <param name="textFile"></param>
        public static void Article()
        {
            var index = new Random().Next(level.Articles.Count - 1);
            checkMode.CreateArticleText(level.Articles[index]);
            level.Articles.RemoveAt(index);

            var phrasesQueue = new Queue<(string, int, Action)>();
            phrasesQueue.Enqueue(("Добрый день!", 3000, null));

            Action createArticleImage = new Action(() =>
            {
                office.DocumentLocation = new Point(650, 650);
                office.Option1.Location = new Point(150, 520);
                office.Option2.Location = new Point(317, 520);
                office.updateGraphics();
            });

            phrasesQueue.Enqueue(("Вот, пожалуйста.", 2000, createArticleImage));
            phrasesQueue.Enqueue(("Сегодня хорошая погода, не правда ли?", 1, null));

            StartEvent(phrasesQueue);
        }

        public static void Begin()
        {
            var phrasesQueue = new Queue<(string, int, Action)>();
            phrasesQueue.Enqueue(("Добрый день!", 3000, null));
            phrasesQueue.Enqueue(("Значит, Вы наш новый главный редактор?", 3000, null));
            phrasesQueue.Enqueue(("Похвально. Вам безумно повезло.", 5000, null));
            phrasesQueue.Enqueue(("Зовут меня MisTakeman. Я работаю там, прямо за стенкой.", 3500, null));
            phrasesQueue.Enqueue(("Если что, заходите. Буду рада Вас видеть!", 4000, null));
            phrasesQueue.Enqueue(("Читатели высказали недовольство, что в наших последних выпусках слишком много научных статей.", 6000, null));
            phrasesQueue.Enqueue(("Отложите их сегодня на потом.", 5000, null));
            phrasesQueue.Enqueue(("А ещё мой коллега попросил передать свою статью на проверку.", 6000, null));
            phrasesQueue.Enqueue(("Удостоверьтесь, что она не имеет прямого отношения к науке.", 7000, null));
            phrasesQueue.Enqueue(("Похоже, на подходе другие авторы со своими статьями. Мне пора.", 5000, null));
            phrasesQueue.Enqueue(("Хочу предупредить: любое решение имеет последствия, причём не всегда положительные.", 8000, null));
            phrasesQueue.Enqueue(("Будьте осторожны, редактор.", 3000, null));

            StartEvent(phrasesQueue);
        }

        private static void StartEvent(Queue<(string Text, int WaitingInMilliseconds, Action ExtraEvent)> phrasesQueue)
        {
            var wait = new Timer { Interval = 1 };
            wait.Tick += ((sender, e) =>
            {
                var (Text, WaitingInMilliseconds, ExtraEvent) = phrasesQueue.Dequeue();
                office.AddNewDialogPhrase(Text);
                ExtraEvent?.Invoke();
                wait.Interval = WaitingInMilliseconds;
                if (phrasesQueue.Count == 0) wait.Stop();
            });
            wait.Start();
        }
    }
}
