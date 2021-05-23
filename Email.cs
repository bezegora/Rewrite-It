using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class Email
    {
        // controls - коллекция интерактивных элементов. В нашем проекте там лежат только Label-ы.
        // Чтобы Label-ы отображались на форме, нужно обязательно добавить их в controls.
        // И наоборот, если мы не хотим рисовать Label-ы на форме, мы убираем их из controls с помощью метода Remove
        // Следует написать методы, которые добавляют или убирают из controls все письма из letters.
        // Эти методы будут вызываться при смене игрового интерфейса в классе Form1.
        // Вход и выход из эл. почты здесь писать не нужно!
        private readonly Control.ControlCollection controls;
        private readonly GameStats stats;

        private readonly List<Label> letters = new List<Label>();
        private readonly Dictionary<int, (string Date, string Content, bool Unread)> lettersInformation 
            = new Dictionary<int, (string, string, bool)>();

        public Email(GameStats stats, Control.ControlCollection controls)
        {
            this.stats = stats;
            this.controls = controls;
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
            lettersInformation.Add(label.GetHashCode(), (stats.GetDate(), content, true));
        }

        // Далее обработка события клика мышью по Label-письму, приводящее к отображению его содержания.
        // При первом клике на Label-письмо оно должно считаться прочитанным и изменить цвет фона (чтобы понять, что оно уже прочитано)
    }
}
