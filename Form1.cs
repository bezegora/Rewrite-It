using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Таймер, определяющий частоту перерисовки кадра
        /// </summary>
        private readonly Timer timerGraphicsUpdate;
        private readonly MainOffice office;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            office = new MainOffice(
                 new Label
                 {
                     Text = "Одобрить",
                     Font = new Font("Pixel Georgia", 19),
                     AutoSize = true,
                     ForeColor = Color.Green,
                     BorderStyle = BorderStyle.FixedSingle,
                     TextAlign = ContentAlignment.MiddleCenter,
                     Location = new Point(-500, 0)
                 },
                 new Label
                 {
                     Text = "Отклонить",
                     Font = new Font("Pixel Georgia", 19),
                     AutoSize = true,
                     ForeColor = Color.Red,
                     BorderStyle = BorderStyle.FixedSingle,
                     TextAlign = ContentAlignment.MiddleCenter,
                     Location = new Point(-500, 0)
                 },
                 new Character(new Dictionary<Character.NamesImages, Image>()
                 {
                     [Character.NamesImages.Women1] = Properties.Resources.Women1
                 },
                 Character.NamesImages.Women1),
                 () => Invalidate());
            Controls.Add(office.Option1);
            Controls.Add(office.Option2);

            timerGraphicsUpdate = new Timer { Interval = 10 };
            timerGraphicsUpdate.Tick += new EventHandler(Update);

            office.EnterCharacter(office.Person, timerGraphicsUpdate, new Point(150, 150));
        }

        public void Update(object sender, EventArgs e)
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
            var graphics = e.Graphics;
            var person = office.Person;
            graphics.DrawImage(person.Images[person.CurrentImage], person.Location.X, person.Location.Y);
            graphics.DrawImage(Properties.Resources.OfficeTable, -70, 600);
            graphics.DrawImage(Properties.Resources.Notebook, 100, 50);
            graphics.DrawImage(Properties.Resources.HeadEditorBook, 150, 625);
            graphics.DrawImage(Properties.Resources.OfficeComputer, 1020, 230);
            for (var i = 0; i < office.DialogPhrases.Count; i++)
                graphics.DrawString(office.DialogPhrases[i], new Font("Pixel Georgia", 16),
                    new SolidBrush(Color.Black), new Rectangle(150, 70 + i*70, 350, 70),
                    new StringFormat() { Alignment = StringAlignment.Far });
        }
    }
}
