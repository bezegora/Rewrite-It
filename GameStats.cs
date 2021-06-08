using System;

namespace Rewrite_It
{
    public class GameStats
    {
        /// <summary>
        /// Максимальное значение популярности.
        /// </summary>
        public static int MaxPopularity { get; } = 50;

        /// <summary>
        /// Внутренние затраты издательства, которые вычитаются из общего бюджета в конце каждого дня.
        /// </summary>
        public int InternalCosts { get; set; } = 250;

        /// <summary>
        /// Текущее значение популярности.
        /// </summary>
        public int Popularity { get; private set; }

        /// <summary>
        /// Текущее значение бюджета.
        /// </summary>
        public int Money { get; private set; }

        public DateTime Date { get; private set; } = new DateTime(2003, 8, 21);

        public GameStats(int popularity = 25, int money = 1000)
        {
            Popularity = popularity;
            Money = money;
        }

        /// <summary>
        /// Изменяет значение популярности, прибавляя или вычитая указанное число.
        /// </summary>
        /// <param name="delta"></param>
        public void ChangePopularity(int delta)
        {
            Popularity += delta;
            if (Popularity > MaxPopularity) Popularity = MaxPopularity;
            else if (Popularity < 0) Popularity = 0;
        }

        /// <summary>
        /// Увеличивает число месяца на 1.
        /// При необходимости переходит на новый месяц.
        /// </summary>
        public void IncreaseDate() => Date = Date.AddDays(1);
    }
}
