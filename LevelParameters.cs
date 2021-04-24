using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Rewrite_It
{
    public class LevelParameters
    {
        /// <summary>
        /// Очередь запланированных на уровень событий.
        /// </summary>
        public Queue<Action> Events { get; set; }

        /// <summary>
        /// Список запланированных на уровень статей, которые будут рандомно попадаться при запуске события Article.
        /// </summary>
        public List<StreamReader> Articles { get; set; }

        public LevelParameters(Queue<Action> events, List<StreamReader> articles)
        {
            var articleEventsCount = 0;
            foreach (var act in events)
                if (act.Equals(new Action(GameEvents.Article))) articleEventsCount++;
            if (articleEventsCount != articles.Count)
                throw new ArgumentException("Количество ожидаемых событий проверки статьи и количество заданных статей не совпадают.");
            Events = events;
            Articles = articles;
        }
    }
}
