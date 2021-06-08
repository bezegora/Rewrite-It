using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class Controller
    {
        public Form1 Form { get; }

        /// <summary>
        /// Определяет текущий нарисованный в форме игровой интерфейс
        /// </summary>
        public IUserInterface CurrentInterface { get; private set; }

        public MainOffice Office { get; }
        public CheckMode CheckMode { get; }
        public DayEnd DayEnd { get; }
        public Email Email { get; }
        public GameStats Stats { get; }
        public LevelParameters Level { get; }

        public Sounds Sounds { get; } = new Sounds();

        public Controller(Form1 form)
        {
            StringStyle.Initialize();

            this.Form = form;
            Stats = new GameStats();
            CheckMode = new CheckMode(this, () => ChangeInterface(Office));
            Email = new Email(Stats, form.Controls, Sounds);
            DayEnd = new DayEnd(this);
            Office = new MainOffice(this, () => ChangeInterface(CheckMode), () => ChangeInterface(DayEnd));

            ChangeInterface(Office);

            var events = new Queue<(Action, CharacterImage)>();

            // В следующих строках мы складываем в очередь события, которые поочерёдно произойдут за уровень,
            // а также наименования персонажей, привязанных к соответствующему событию.
            events.Enqueue((GameEvents.Article, CharacterImage.Woman1));
            events.Enqueue((GameEvents.Begin, CharacterImage.MissTakeman));
            events.Enqueue((GameEvents.Article, CharacterImage.Man1));
            events.Enqueue((GameEvents.Article, CharacterImage.Woman2));
            events.Enqueue((GameEvents.Article, CharacterImage.Man2));

            // В следующий список мы складываем все статьи, которые рандомно будут попадаться на уровне
            // при воспроизведении события Article.
            var articles = new List<StreamReader>()
            {
                new StreamReader(@"Articles\Marketing1.txt"),
                new StreamReader(@"Articles\Marketing2.txt"),
                new StreamReader(@"Articles\Marketing3.txt"),
                new StreamReader(@"Articles\Marketing4.txt")
            };
            Level = new LevelParameters(events, articles);
            GameEvents.Initialize(this);

            Sounds.PlayOfficeBackground();

            Office.EnterCharacter();
        }

        public void Tick()
        {
            CurrentInterface.Tick();
            GameEvents.StartEvent();
        }

        public void Paint(Graphics graphics) => CurrentInterface.Paint(graphics);

        public void MouseDown() => CurrentInterface.MouseDown();

        /// <summary>
        /// Изменяет игровой интерфейс
        /// </summary>
        /// <param name="backgroundImage">Фоновое изображение интерфейса</param>
        /// <param name="_interface">Новый интерфейс</param>
        public void ChangeInterface(IUserInterface value)
        {
            Form.Controls.Clear();
            CurrentInterface = value;
            CurrentInterface.Change(Form);
        }
    }
}
