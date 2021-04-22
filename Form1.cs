using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Linq;

namespace Rewrite_It
{
    /// <summary>
    /// Содержит все игровые интерфейсы
    /// </summary>
    public enum Interfaces
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
        private readonly Timer timerGraphicsUpdate;

        /// <summary>
        /// Определяет текущий нарисованный в форме игровой интерфейс
        /// </summary>
        private Interfaces currentInterface;

        private readonly MainOffice office;
        private readonly CheckMode checkMode;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyleFont("PixelGeorgia.ttf", Color.Black, 16);
            //currentInterface = Interfaces.MainOffice;

            Action updateGraphics = () => Invalidate();
            checkMode = new CheckMode(new Dictionary<CheckMode.Tabs, Bitmap>
            {
                [CheckMode.Tabs.Guide] = new Bitmap(Properties.Resources.HeadBookTab1),
                [CheckMode.Tabs.MistakesList] = new Bitmap(Properties.Resources.HeadBookTab2),
                [CheckMode.Tabs.CensoredList] = new Bitmap(Properties.Resources.HeadBookTab3)
            },
            updateGraphics,
            Color.CornflowerBlue,
            Color.DarkSlateGray,
            Controls);

            office = new MainOffice(
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
                 new Character(new Dictionary<Character.NamesImages, Image>()
                 {
                     [Character.NamesImages.Women1] = Properties.Resources.Women1
                 },
                 Character.NamesImages.Women1),
                 updateGraphics,
                 checkMode);
            ChangeInterface(Properties.Resources.OfficeBackground, Interfaces.MainOffice);

            timerGraphicsUpdate = new Timer { Interval = 10 };
            timerGraphicsUpdate.Tick += new EventHandler(Update);

            office.EnterCharacter(office.Person, timerGraphicsUpdate, new Point(150, 150));
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
            var person = office.Person;
            if (person.IsMoving)
            {
                if (person.Direction == Character.MovingDirections.Left)
                {
                    person.Location = new Point(person.Location.X + 15, person.Location.Y);
                    if (person.Location.X > 530)
                    {
                        office.StopCharacter(person, timerGraphicsUpdate);
                        office.StartEvent(MainOffice.Events.Article);
                    }
                }
                else if (person.Direction == Character.MovingDirections.Right)
                {
                    person.Location = new Point(person.Location.X - 15, person.Location.Y);
                    if (person.Location.X < -100) office.StopCharacter(person, timerGraphicsUpdate);
                }
            }
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
            switch (currentInterface)
            {
                case Interfaces.MainOffice:
                    office.Paint(e);
                    break;
                case Interfaces.CheckMode:
                    checkMode.Paint(e);
                    break;
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (IsClickedArea(e, office.DocumentLocation, new Point(Properties.Resources.Document.Size)))
                ChangeInterface(Properties.Resources.CheckModeBackground, Interfaces.CheckMode);
            if (checkMode.CheckHasMatching()) return;
            if (IsClickedArea(e, checkMode.ExitButtonLocation, new Point(Properties.Resources.ExitFromCheckMode.Size)))
                ChangeInterface(Properties.Resources.OfficeBackground, Interfaces.MainOffice);
            if (IsClickedArea(e, new Point(checkMode.BookLocation.X + 143, checkMode.BookLocation.Y + 6), new Point(112, 56)))
                checkMode.UpdateStatus(CheckMode.Tabs.MistakesList);
            if (IsClickedArea(e, new Point(checkMode.BookLocation.X + 28, checkMode.BookLocation.Y + 6), new Point(112, 56)))
                checkMode.UpdateStatus(CheckMode.Tabs.Guide);

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
        public void ChangeInterface(Image backgroundImage, Interfaces _interface)
        {
            currentInterface = _interface;
            this.BackgroundImage = backgroundImage;
            checkMode.SetSelectedTextArea(null);
            Controls.Clear();
            if (currentInterface == Interfaces.MainOffice)
                AddLabelsToControls(office.Option1, office.Option2);
            if (currentInterface == Interfaces.CheckMode)
                checkMode.UpdateStatus(checkMode.CurrentBookMode);
        }

        public void AddLabelsToControls(params Label[] labels)
        {
            foreach (var label in labels)
                Controls.Add(label);
        }
    }
}
