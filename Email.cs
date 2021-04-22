using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    class Email
    {
        // controls - коллекция интерактивных элементов. В нашем проекте там лежат только Label-ы.
        // Чтобы Label-ы отображались на форме, нужно обязательно добавить их в controls.
        // И наоборот, если мы не хотим рисовать Label-ы на форме, мы убираем их из controls с помощью метода Remove
        // Следует написать методы, которые добавляют или убирают из controls все письма из letters.
        // Эти методы будут вызываться при смене игрового интерфейса в классе Form1.
        // Вход и выход из эл. почты писать не нужно!
        private readonly Control.ControlCollection controls;

        private readonly List<Label> letters = new List<Label>();
        private readonly Dictionary<int, (string Content, bool Unread)> lettersInformation = new Dictionary<int, (string, bool)>();

        public Email(Control.ControlCollection controls)
        {
            this.controls = controls;
        }

        public void AddLetter(string title, string content)
        {
            // Здесь обработка ситуации, когда количество писем сейчас превысит максимум.
            // Нужно убрать первое письмо из letters, а у остальных сдвинуть вверх координату Y на фиксированное значение
            // Причём нужно также убрать из lettersContents содержание удалённого письма.
            // Лучше это всё вынести в отдельный метод.
            var label = new Label()
            {
                Text = title,
                Font = StringStyle.Font,
                AutoSize = true,
                BorderStyle = BorderStyle.FixedSingle,
                //Location = new Point(...)       Позиция пришедшего в конец очереди письма
            };
            letters.Add(label);
            lettersInformation.Add(label.GetHashCode(), (content, true));
        }

        // Далее обработка события клика мышью по Label-письму, приводящее к отображению его содержания.
        // При первом клике на Label-письмо оно должно считаться прочитанным и изменить цвет фона (чтобы понять, что оно уже прочитано)
    }
}
