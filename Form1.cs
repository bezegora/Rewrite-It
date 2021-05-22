using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Media;

namespace Rewrite_It
{
    /// <summary>
    /// Содержит все игровые интерфейсы
    /// </summary>
    public enum Interface
    {
        MainMenu,
        MainOffice,
        CheckMode,
        Email,
        DayEnd
    }

    public partial class Form1 : Form
    {
        /// <summary>
        /// Таймер, определяющий частоту перерисовки кадра
        /// </summary>
        public Timer TimerGraphicsUpdate { get; } = new Timer { Interval = 40 };

        /// <summary>
        /// Определяет текущий нарисованный в форме игровой интерфейс
        /// </summary>
        public static Interface CurrentInterface { get; private set; }

        public MainOffice Office { get; }
        public CheckMode CheckMode { get; }
        public DayEnd DayEnd { get; }
        public Email Email { get; }
        public GameStats Stats { get; }
        public LevelParameters Level { get; }

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyleFont("PixelGeorgia.ttf", Color.Black, 16);

            Stats = new GameStats();

            CheckMode = new CheckMode(new Dictionary<CheckMode.Tabs, Bitmap>
            {
                [CheckMode.Tabs.Guide] = new Bitmap(Properties.Resources.HeadBookTab1),
                [CheckMode.Tabs.MistakesList] = new Bitmap(Properties.Resources.HeadBookTab2),
                [CheckMode.Tabs.CensoredList] = new Bitmap(Properties.Resources.HeadBookTab3)
            },
            Color.CornflowerBlue,
            Color.DarkSlateGray,
            this);

            Email = new Email(Stats, Controls);

            Office = new MainOffice(
                 new Label
                 {
                     Text = "Одобрить",
                     Font = new Font(StringStyle.FontFamily, 19),
                     AutoSize = true,
                     ForeColor = Color.Green,
                     BorderStyle = BorderStyle.FixedSingle,
                     Location = new Point(-500, 0)
                 },
                 new Label
                 {
                     Text = "Отклонить",
                     Font = new Font(StringStyle.FontFamily, 19),
                     AutoSize = true,
                     ForeColor = Color.Red,
                     BorderStyle = BorderStyle.FixedSingle,
                     Location = new Point(-500, 0)
                 },
                 new Character(new Dictionary<NamesImages, Image>()
                 {
                     [NamesImages.Woman1] = Properties.Resources.Woman1,
                     [NamesImages.Woman2] = Properties.Resources.Woman2,
                     [NamesImages.MissTakeman] = Properties.Resources.MissTakeman,
                     [NamesImages.Man1] = Properties.Resources.Man1,
                     [NamesImages.Man2] = Properties.Resources.Man2
                 },
                 this), this);
            ChangeInterface(Properties.Resources.OfficeBackground, Interface.MainOffice);

            var events = new Queue<(Action, NamesImages)>();

            // В следующих строках мы складываем в очередь события, которые поочерёдно произойдут за уровень,
            // а также наименования персонажей, привязанных к соответствующему событию.
            events.Enqueue((GameEvents.Article, NamesImages.Woman1));
            events.Enqueue((GameEvents.Begin, NamesImages.MissTakeman));
            events.Enqueue((GameEvents.Article, NamesImages.Man1));
            events.Enqueue((GameEvents.Article, NamesImages.Woman2));
            events.Enqueue((GameEvents.Article, NamesImages.Man2));

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
            GameEvents.InitializeComponents(this);

            TimerGraphicsUpdate.Tick += new EventHandler(Update);
            TimerGraphicsUpdate.Start();

            Office.EnterCharacter();
        }

        private void SetStyleFont(string fileFont, Color color, int size)
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile(fileFont);
            StringStyle.SetFontFamily(fontCollection.Families[0]);
            StringStyle.SetSolidBrush(new SolidBrush(color));
            StringStyle.SetFont(new Font(StringStyle.FontFamily, size));
        }

        private void Update(object sender, EventArgs e)
        {
            Invalidate();
            var person = Office.Person;
            person.Move();
            GameEvents.StartEvent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.FormBorderStyle = FormBorderStyle.None;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.WindowState = FormWindowState.Maximized;
            this.BackgroundImageLayout = ImageLayout.Center;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            Invalidate();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            switch (CurrentInterface)
            {
                case Interface.MainOffice: Office.Paint(e); break;
                case Interface.CheckMode: CheckMode.Paint(e); break;
                case Interface.DayEnd: DayEnd.Paint(e); break;
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (IsClickedArea(e, Office.DocumentLocation, new Point(Properties.Resources.Document.Size)))
            {
                PlaySound(Properties.Resources.CheckMode);
                ChangeInterface(Properties.Resources.CheckModeBackground, Interface.CheckMode);
            }
            if (CheckMode.CheckHasMatching()) return;
            if (IsClickedArea(e, CheckMode.ExitButtonLocation, new Point(Properties.Resources.ExitFromCheckMode.Size)))
            {
                PlaySound(Properties.Resources.CheckMode);
                ChangeInterface(Properties.Resources.OfficeBackground, Interface.MainOffice);
            }
            if (IsClickedArea(e, new Point(CheckMode.BookLocation.X + 143, CheckMode.BookLocation.Y + 6), new Point(112, 56)))
            {
                PlaySound(Properties.Resources.Paper);
                CheckMode.UpdateStatus(CheckMode.Tabs.MistakesList);
            }
            if (IsClickedArea(e, new Point(CheckMode.BookLocation.X + 28, CheckMode.BookLocation.Y + 6), new Point(112, 56)))
            {
                PlaySound(Properties.Resources.Paper);
                CheckMode.UpdateStatus(CheckMode.Tabs.Guide);
            }

            Invalidate();
        }

        private bool IsClickedArea(MouseEventArgs e, Point location, Point shift) =>
            e.X >= location.X && e.Y >= location.Y &&
            e.X <= location.X + shift.X &&
            e.Y <= location.Y + shift.Y;

        /// <summary>
        /// Изменяет игровой интерфейс
        /// </summary>
        /// <param name="backgroundImage">Фоновое изображение интерфейса</param>
        /// <param name="_interface">Новый интерфейс</param>
        public void ChangeInterface(Image backgroundImage, Interface _interface)
        {
            CurrentInterface = _interface;
            this.BackgroundImage = backgroundImage;
            CheckMode.SetSelectedTextArea(null);
            Controls.Clear();
            if (CurrentInterface is Interface.MainOffice)
                Office.UpdateStatus();
            if (CurrentInterface is Interface.CheckMode)
                CheckMode.UpdateStatus(CheckMode.CurrentBookMode);
        }

        public void PlaySound(UnmanagedMemoryStream sound, bool loop = false)
        {
            var soundPlayer = new SoundPlayer(sound);
            if (!loop) soundPlayer.Play();
            else soundPlayer.PlayLooping();
        }

        public void AddLabelsToControls(params Label[] labels)
        {
            foreach (var label in labels)
                Controls.Add(label);
        }
    }
}
