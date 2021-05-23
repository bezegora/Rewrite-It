using System;
using System.Collections.Generic;
using System.IO;

namespace Rewrite_It
{
    public class LevelParameters
    {
        /// <summary>
        /// Очередь запланированных на уровень событий.
        /// </summary>
        public Queue<(Action _event, CharacterImage character)> Events { get; set; }

        /// <summary>
        /// Список запланированных на уровень статей, которые будут рандомно попадаться при запуске события Article.
        /// </summary>
        public List<StreamReader> Articles { get; set; }

        /// <summary>
        /// Количество проверенных статей за уровень.
        /// </summary>
        public int VerifiedArticlesCount { get; private set; } = 0;

        /// <summary>
        /// Количество одобренных статей за уровень.
        /// </summary>
        public int ApprovedArticlesCount { get; private set; } = 0;

        /// <summary>
        /// Количество найденных ошибок за уровень.
        /// </summary>
        public int MistakesFoundCount { get; private set; } = 0;

        /// <summary>
        /// Прибавка к популярности.
        /// </summary>
        public int IncreaseInPopularity { get; private set; } = 0;

        /// <summary>
        /// Сумма полученных штрафов.
        /// </summary>
        public int FinesSum { get; private set; } = 0;

        /// <summary>
        /// Доход за уровень.
        /// </summary>
        public int Income
        {
            get { return 50 + 50 * IncreaseInPopularity + 15 * ApprovedArticlesCount; }
        }

        public LevelParameters(Queue<(Action, CharacterImage)> events, List<StreamReader> articles)
        {
            var articleEventsCount = 0;
            foreach (var act in events)
                if (act.Item1.Equals(new Action(GameEvents.Article))) articleEventsCount++;
            if (articleEventsCount != articles.Count)
                throw new ArgumentException("Количество ожидаемых событий проверки статьи и количество заданных статей не совпадают.");
            Events = events;
            Articles = articles;
        }

        public void IncreaseVerifiedArticles() => VerifiedArticlesCount++;
        public void IncreaseApprivedArticles() => ApprovedArticlesCount++;
        public void IncreaseMistakesFound() => MistakesFoundCount++;
        public void AddIncreaseToPopularity(int count) => IncreaseInPopularity += count;
        public void InreaseFinesSum(int count) => FinesSum += count;
    }
}
