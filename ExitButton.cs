using System.Drawing;

namespace Rewrite_It
{
    public class ExitButton : GraphicObject
    {
        private readonly Sounds sounds;
        private bool visible;

        public ExitButton(Sounds sounds) : base(new Bitmap(Properties.Resources.ExitFromCheckMode, 70, 170), new Point(1500, 30))
            => this.sounds = sounds;

        public void Tick()
        {
            if (AuxiliaryMethods.CursorIsHoveredArea(new Rectangle(Position, Bitmap.Size)))
            {
                if (!visible)
                {
                    sounds.PlayCursorHovered();
                    visible = true;
                    Position = new Point(1475, 30);
                }
            }
            else if (visible)
            {
                sounds.PlayCursorNotHovered();
                visible = false;
                Position = new Point(1500, 30);
            }
        }
    }
}
