using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Rewrite_It
{
    public class Character : GraphicObject
    {
        private readonly Controller controller;
        private int waitBeforeChangingPosition;
        private bool isUp;

        /// <summary>
        /// Содержит все изображения персонажей. 
        /// Key - название изображения. 
        /// Value - само изображение
        /// </summary>
        private readonly Dictionary<CharacterImage, Image> images;

        public Character(Controller controller) : base(new Point(-500, 170), 15)
        {
            images = new Dictionary<CharacterImage, Image>()
            {
                [CharacterImage.Woman1] = Properties.Resources.Woman1,
                [CharacterImage.Woman2] = Properties.Resources.Woman2,
                [CharacterImage.MissTakeman] = Properties.Resources.MissTakeman,
                [CharacterImage.Man1] = Properties.Resources.Man1,
                [CharacterImage.Man2] = Properties.Resources.Man2
            };
            this.controller = controller;
        }

        public void Tick()
        {
            Move();
            if (Position.X > 530)
            {
                Position = new Point(530, Position.Y);
                Stop();
                controller.Level.Events.Dequeue()._event();
                MovementSpeed = 1;
                waitBeforeChangingPosition = 20;
            }
            else if (Position.X < -500)
            {
                Stop();
                Position = new Point(-500, Position.Y);
                controller.Office.EnterCharacter();
                controller.Sounds.PlayDoorClose();
            }

            if (waitBeforeChangingPosition > 0)
            {
                waitBeforeChangingPosition--;
                if (waitBeforeChangingPosition == 0)
                    Swing();
            }

            if (Position.Y < 165)
            {
                Position = new Point(Position.X, 165);
                Stop();
                waitBeforeChangingPosition = 30;
            }
            else if (Position.Y > 175)
            {
                Position = new Point(Position.X, 175);
                Stop();
                waitBeforeChangingPosition = 30;
            }
        }

        public void LetIn() => GoRight();

        public void Leave()
        {
            MovementSpeed = 20;
            waitBeforeChangingPosition = 0;
            Position = new Point(Position.X, 170);
            GoLeft();
        }

        public void SetImage(CharacterImage image) => Bitmap = new Bitmap(images[image]);

        private void Swing()
        {
            if (isUp)
            {
                isUp = false;
                GoDown();
            }
            else
            {
                isUp = true;
                GoUp();
            }
        }
    }
}
