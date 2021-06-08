using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class Email : IUserInterface
    {
        private readonly Control.ControlCollection controls;
        private readonly GameStats stats;
        private readonly Sounds sounds;

        private readonly List<Label> letters = new List<Label>();
        private readonly Dictionary<int, (string Date, string Content, bool Unread)> lettersInformation 
            = new Dictionary<int, (string, string, bool)>();
        private readonly TextBox contentText;

        public Email(GameStats stats, Control.ControlCollection controls, Sounds sounds)
        {
            this.stats = stats;
            this.controls = controls;
            this.sounds = sounds;
            contentText = new TextBox
            {
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Font = StringStyle.TextFont,
                Size = new Size(800, 800),
                Location = new Point(400, 100)
            };
        }

        public void AddLetter(string title, string content)
        {
            // Здесь обработка ситуации, когда количество писем сейчас превысит максимум.
            // Нужно убрать первое письмо из letters, а у остальных сдвинуть вверх координату Y на фиксированное значение
            // Причём нужно также убрать из lettersInformation содержание удалённого письма.
            // Лучше это всё вынести в отдельный метод.
            var label = new Label()
            {
                Text = title,
                Font = StringStyle.TextFont,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                //Location = new Point(...)       Позиция пришедшего в конец очереди письма. Можно задать не сейчас, а потом вот так: label.Location = new Point(...)
            };
            letters.Add(label);
            lettersInformation.Add(label.GetHashCode(), (stats.Date.ToString("D"), content, true));
        }

        public void Change(Form1 form)
        {
            form.BackgroundImage = Properties.Resources.EmailBackground;
        }

        public void MouseDown() { }

        public void Paint(Graphics graphics) { }

        public void Tick()
        {
            throw new NotImplementedException();
        }

        // Далее обработка события клика мышью по Label-письму, приводящее к отображению его содержания.
        // При первом клике на Label-письмо оно должно считаться прочитанным и изменить цвет фона (чтобы понять, что оно уже прочитано)
    }

    class Letter
    {
        public DateTime Date { get; }
        public string Content { get; }
        public bool Readed { get; set; } = false;

        public Letter(DateTime date, string content)
        {
            Date = date;
            Content = content;
        }
    }
}
