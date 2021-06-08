using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class Email : IUserInterface
    {
        private readonly Control.ControlCollection controls;
        private readonly Sounds sounds;

        private readonly Dictionary<int, Letter> lettersInfo = new Dictionary<int, Letter>();
        private readonly List<Letter> letters = new List<Letter>();
        private readonly TextBox content;
        private Label selected;

        private readonly Action goToMainOffice;

        public Email(Control.ControlCollection controls, Sounds sounds, Action goToMainOffice)
        {
            this.goToMainOffice = goToMainOffice;
            this.controls = controls;
            this.sounds = sounds;
            content = new TextBox
            {
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font(StringStyle.FontFamily, 18),
                Size = new Size(1190, 650),
                Location = new Point(323, 160),
                Multiline = true
            };
        }

        public void AddLetter(DateTime date, string sender, string title, string content)
        {
            if (lettersInfo.Count > 4)
            {
                lettersInfo.Remove(letters[0].Label.GetHashCode());
                letters.RemoveAt(letters.Count - 1);
            }

            var label = new Label
            {
                Text = $"От: {sender}" +
                    $"\nТема: {title}" +
                    $"\n{date:D}",
                Font = new Font(StringStyle.FontFamily, 14),
                Size = new Size(300, 110),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.LightYellow
            };
            label.MouseDown += MouseDownOnLetter;
            var letter = new Letter(label, content);
            letters.Insert(0, letter);
            lettersInfo.Add(label.GetHashCode(), letter);
            ReculculatePositions();
        }

        private void ReculculatePositions()
        {
            for (var i = 0; i < letters.Count; i++)
            {
                letters[i].Label.Location = new Point(22, 240 + i * 120);
            }
        }

        private void MouseDownOnLetter(object sender, MouseEventArgs e)
        {
            var label = sender as Label;
            if (label.BackColor == Color.White || label.BackColor == Color.LightYellow)
            {
                RemoveSelected();
                sounds.PlayChosenOption();
                SetSelected(label);
            }
            else
            {
                sounds.PlayCancel();
                RemoveSelected();
            }
        }

        private void SetSelected(Label value)
        {
            content.Text = lettersInfo[value.GetHashCode()].Content;
            value.BackColor = Color.CornflowerBlue;
            selected = value;
        }

        private void RemoveSelected()
        {
            content.Text = "";
            if (selected != null)
                selected.BackColor = Color.White;
            selected = null;
        }

        public void Change(Form1 form)
        {
            form.BackgroundImage = Properties.Resources.EmailBackground;
            if (letters.Count == 0)
                content.Text = "Входящих писем пока нет.";
            else RemoveSelected();
            foreach (var letter in letters)
                AuxiliaryMethods.AddLabelsToControls(controls, letter.Label);
            controls.Add(content);
        }

        public void MouseDown() 
        {
            if (AuxiliaryMethods.CursorIsHoveredArea(new Rectangle(1475, 0, 50, 80)))
                goToMainOffice();
        }

        public void Paint(Graphics graphics) { }

        public void Tick() { }
    }

    class Letter
    {
        public Label Label { get; }
        public string Content { get; }

        public Letter(Label label, string content)
        {
            Content = content;
            Label = label;
        }
    }
}
