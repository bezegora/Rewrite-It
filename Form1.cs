using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Linq;

namespace Rewrite_It
{
    public partial class Form1 : Form
    {
        public static Point Beyond { get; } = new Point(-5000, 0);

        /// <summary>
        /// Таймер, определяющий частоту перерисовки кадра
        /// </summary>
        public Timer TimerGraphicsUpdate { get; } = new Timer { Interval = 40 };

        private readonly Controller controller;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;

            TimerGraphicsUpdate.Tick += new EventHandler(OnTick);
            TimerGraphicsUpdate.Start();

            controller = new Controller(this);
        }

        private void OnTick(object sender, EventArgs e)
        {
            controller.Tick();
            Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.WindowState = FormWindowState.Maximized;
            this.BackgroundImageLayout = ImageLayout.Center;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void OnPaint(object sender, PaintEventArgs e) => controller.Paint(e.Graphics);

        private void OnMouseDown(object sender, MouseEventArgs e) => controller.MouseDown();
    }
}
