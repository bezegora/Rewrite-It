using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    /// <summary>
    /// Содержит все возможные названия изображений персонажа
    /// </summary>
    public enum NamesImages
    {
        Woman1,
        Woman2,
        MissTakeman,
        Man1,
        Man2
    }

    /// <summary>
    /// Содержит все возможные направления движения персонажа
    /// </summary>
    public enum MovingDirections
    {
        Left,
        Right
    }

    public class Character
    {
        private readonly Form1 form;

        /// <summary>
        /// Содержит все изображения персонажей. 
        /// Key - название изображения. 
        /// Value - само изображение
        /// </summary>
        public Dictionary<NamesImages, Image> Images { get; }

        /// <summary>
        /// Название текущего изображения персонажа
        /// </summary>
        public NamesImages CurrentImage { get; set; }

        /// <summary>
        /// Определяет, движется ли персонаж в какую-либо сторону, заданную свойством Direction
        /// </summary>
        public bool IsMoving { get; private set; }

        /// <summary>
        /// Координаты отрисовки персонажа на форме
        /// </summary>
        public Point Location { get; set; } = new Point(-500, 150);

        /// <summary>
        /// Определяет направление движения персонажа. Движение начинается, если свойство IsMoving = true
        /// </summary>
        public MovingDirections Direction { get; set; }

        public Character(Dictionary<NamesImages, Image> images, Form1 form)
        {
            Images = images;
            IsMoving = false;
            Direction = MovingDirections.Right;
            this.form = form;
        }

        private bool wasPlayedDoorCloseSound = false;
        public void Move()
        {
            if (!IsMoving) return;
            if (Direction is MovingDirections.Right)
            {
                Location = new Point(Location.X + 20, Location.Y);
                if (Location.X > 530)
                {
                    Location = new Point(530, Location.Y);
                    StopMoving();
                    form.Level.Events.Dequeue()._event();
                }
            }
            if (Direction is MovingDirections.Left)
            {
                Location = new Point(Location.X - 30, Location.Y);
                if (Location.X < -300 && !wasPlayedDoorCloseSound)
                {
                    wasPlayedDoorCloseSound = true;
                    form.PlaySound(Properties.Resources.DoorClose);
                }
                if (Location.X < -500)
                {
                    wasPlayedDoorCloseSound = false;
                    Location = new Point(-500, Location.Y); 
                    form.Office.EnterCharacter();
                }
            }
        }

        public void StartMoving() => IsMoving = true;
        public void StopMoving() => IsMoving = false;
    }
}
