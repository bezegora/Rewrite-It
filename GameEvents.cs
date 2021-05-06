using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class GameEvents
    {
        public static Queue<Phrase> PhrasesQueue { get; set; } = new Queue<Phrase>();
        public static bool EventIsProgress { get; private set; } = false;
        private static Form1 form;

        public static void InitializeComponents(Form1 _form)
        {
            form = _form;
        }

        /// <summary>
        /// Запускает стандартное событие, ориентированное исключительно на проверку статьи.
        /// </summary>
        /// <param name="textFile"></param>
        public static void Article()
        {
            var random = new Random();
            var index = random.Next(form.Level.Articles.Count - 1);
            form.CheckMode.CreateArticleText(form.Level.Articles[index]);
            form.Level.Articles.RemoveAt(index);

            var possiblePhrases = new[]
            {
                "Добрый день!",
                "Здравствуйте!",
                "Доброго дня!"
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 2000));

            Action createDocument = new Action(() =>
            {
                form.Office.DocumentLocation = new Point(650, 650);
                //office.AddLabelsToControls(office.Option1, office.Option2);
                form.Office.Option1.Location = new Point(150, 520);
                form.Office.Option2.Location = new Point(317, 520);
            });
            possiblePhrases = new[]
            {
                "Вот, пожалуйста.",
                "Пожалуйста, скажите, что со статьёй всё в порядке.",
                "Может, безо всяких сложностей сразу одобрите?",
                "Мною было потрачено много времени на статью, она обязана пройти!",
                "Не терпится узнать вердикт.",
                "Давайте только побыстрее."
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 2000, createDocument));
        }

        public static void Begin()
        {
            PhrasesQueue.Enqueue(new Phrase("Добрый день!", 3000));
            PhrasesQueue.Enqueue(new Phrase("Значит, Вы наш новый главный редактор?", 3000));
            PhrasesQueue.Enqueue(new Phrase("Похвально. Вам безумно повезло.", 5000));
            PhrasesQueue.Enqueue(new Phrase("Зовут меня MisTakeman. Я работаю там, прямо за стенкой.", 3500));
            PhrasesQueue.Enqueue(new Phrase("Если что, заходите. Буду рада Вас видеть!", 4000));
            PhrasesQueue.Enqueue(new Phrase("Похоже, на подходе другие авторы со своими статьями. Мне пора.", 5000));
            PhrasesQueue.Enqueue(new Phrase("Хочу предупредить: любое решение имеет последствия, причём не всегда положительные.", 8000));
            PhrasesQueue.Enqueue(new Phrase("Будьте осторожны, редактор.", 3000, () => CharacterDeparture(2000)));
        }

        public static void RejectionArticle()
        {
            var random = new Random();
            var possiblePhrases = new[]
            {
                "К сожалению, мы не можем принять Вашу статью.",
                "Вынужден сообщить, что статья не прошла проверку.",
                "Сегодня Вам не повезло."
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 2500, false));
            possiblePhrases = new[]
            {
                "Что? Почему?",
                "О чём Вы говорите?",
                "Не может быть! В чём причина?"
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 1500));
            possiblePhrases = new[]
            {
                "В статье достаточно ошибок, чтобы не допустить её к публикации.",
                "В статье помечены все недочёты.",
                "Посмотрите сами."
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 2500, false));
            possiblePhrases = new[]
            {
                "Хм. До свидания.",
                "Эх.",
                "До свидания.",
                "Неважно.",
                "Вы меня неправильно поняли."
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 5000, () => CharacterDeparture(2000)));
        }

        public static void ApprovingArticle()
        {
            var random = new Random();
            var possiblePhrases = new[]
{
                "Статья принята. Её опубликуют в ближайшее время.",
                "Статья допущена к публикации.",
                "Хорошая новость. Статья успешно прошла проверку."
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 2500, false));
            possiblePhrases = new[]
            {
                "Замечательно!",
                "Вам удалось меня осчастливить.",
                "Разве может быть иначе?"
            };
            PhrasesQueue.Enqueue(new Phrase(possiblePhrases[random.Next(possiblePhrases.Length)], 5000, () => CharacterDeparture(2000)));
        }

        private static void CharacterDeparture(int waitingInMilliseconds)
        {
            var wait = new Timer { Interval = waitingInMilliseconds };
            wait.Tick += (sender, e) =>
            {
                form.Office.Person.Direction = MovingDirections.Left;
                form.Office.Person.StartMoving();
                wait.Stop();
            };
            wait.Start();
        }

        public static void StartEvent()
        {
            if (PhrasesQueue.Count == 0 || EventIsProgress) return;
            EventIsProgress = true;
            var wait = new Timer { Interval = 1 };
            wait.Tick += ((sender, e) =>
            {
                var phrase = PhrasesQueue.Dequeue();
                var align = ContentAlignment.MiddleRight;
                var color = Color.Black;
                if (!phrase.TalkingVisitor)
                {
                    align = ContentAlignment.MiddleLeft;
                    color = Color.DarkBlue;
                }
                form.Office.AddNewDialogPhrase(phrase.Text, color, align);
                phrase.ExtraEvent?.Invoke();
                wait.Interval = phrase.WaitingInMilliseconds;
                if (PhrasesQueue.Count == 0)
                {
                    wait.Stop();
                    EventIsProgress = false;
                }
            });
            wait.Start();
        }
    }
}
