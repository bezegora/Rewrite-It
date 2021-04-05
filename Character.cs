using System.Collections.Generic;
using System.Drawing;

namespace Rewrite_It
{
    class Character
    {
        /// <summary>
        /// Содержит все изображения персонажей. Key - название изображения. Value - само изображение
        /// </summary>
        public Dictionary<NamesImages, Image> Images { get; }

        /// <summary>
        /// Название текущего изображения персонажа
        /// </summary>
        public NamesImages CurrentImage { get; set; }

        /// <summary>
        /// Определяет, движется ли персонаж в какую-либо сторону, заданную свойством Direction
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        /// Координаты отрисовки персонажа на форме
        /// </summary>
        public Point Location { get; set; }

        /// <summary>
        /// Определяет направление движения персонажа. Движение начинается, если свойство IsMoving = true
        /// </summary>
        public MovingDirections Direction { get; set; }

        public Character(Dictionary<NamesImages, Image> images, NamesImages initialImage)
        {
            Images = images;
            IsMoving = false;
            Direction = MovingDirections.Left;
            CurrentImage = initialImage;
        }

        /// <summary>
        /// Содержит все возможные названия изображений персонажа
        /// </summary>
        public enum NamesImages
        {
            Women1
        }

        /// <summary>
        /// Содержит все возможные направления движения персонажа
        /// </summary>
        public enum MovingDirections
        {
            Left,
            Right
        }
    }
}
