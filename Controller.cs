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
        public Interface CurrentInterface { get; private set; }

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
            CheckMode = new CheckMode(this);
            Email = new Email(Stats, form.Controls);
            DayEnd = new DayEnd(this);
            Office = new MainOffice(this);

            ChangeInterface(Properties.Resources.OfficeBackground, Interface.MainOffice);
            //ChangeInterface(Interface.DayEnd);

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
            Office.Tick();
            Sounds.Tick();
            CheckMode.Tick();
            GameEvents.StartEvent();
        }

        public void Paint(Graphics graphics)
        {
            switch (CurrentInterface)
            {
                case Interface.MainOffice: Office.Paint(graphics); break;
                case Interface.CheckMode: CheckMode.Paint(graphics); break;
                case Interface.DayEnd: DayEnd.Paint(graphics); break;
            }
        }

        public void MouseDown()
        {
            if (CheckMode.CheckHasMatching()) return;
            if (Office.IsClickedDocument())
            {
                Sounds.PlayCheckMode();
                ChangeInterface(Properties.Resources.CheckModeBackground, Interface.CheckMode);
            }
            else if (CheckMode.IsClickedExitButton())
            {
                Sounds.PlayCheckMode();
                ChangeInterface(Properties.Resources.OfficeBackground, Interface.MainOffice);
            }
            else if (AuxiliaryMethods.CursorIsHoveredArea(new Rectangle(new Point(CheckMode.BookLocation.X + 143, CheckMode.BookLocation.Y + 6), new Size(112, 80))))
            {
                Sounds.PlayPaper();
                CheckMode.UpdateStatus(Tab.MistakesList);
            }
            else if (AuxiliaryMethods.CursorIsHoveredArea(new Rectangle(new Point(CheckMode.BookLocation.X + 28, CheckMode.BookLocation.Y + 6), new Size(112, 80))))
            {
                Sounds.PlayPaper();
                CheckMode.UpdateStatus(Tab.Guide);
            }
            else GameEvents.SkipPhrase();
        }

        /// <summary>
        /// Изменяет игровой интерфейс
        /// </summary>
        /// <param name="backgroundImage">Фоновое изображение интерфейса</param>
        /// <param name="_interface">Новый интерфейс</param>
        public void ChangeInterface(Image backgroundImage, Interface _interface)
        {
            CurrentInterface = _interface;
            if (backgroundImage != null) Form.BackgroundImage = backgroundImage;
            //CheckMode.SetSelectedTextArea(null);
            CheckMode.SelectedTextArea = null;
            Form.Controls.Clear();
            if (CurrentInterface is Interface.MainOffice)
                Office.UpdateStatus();
            else if (CurrentInterface is Interface.CheckMode)
                CheckMode.UpdateStatus(CheckMode.CurrentBookMode);
            else if (CurrentInterface is Interface.DayEnd)
                Sounds.StopOfficeBackground();
        }

        public void ChangeInterface(Interface _interface) => ChangeInterface(null, _interface);
    }
}
