using System.Drawing;

namespace Rewrite_It
{
    public class GraphicObject
    {
        public Bitmap Bitmap { get; set; }
        public Point Position { get; set; }
        public int MovementSpeed { get; set; }
        public bool IsMoving
        {
            get { return shift != new Size(0, 0); }
        }

        private Size shift = new Size(0, 0);

        public GraphicObject(Bitmap bitmap, Point position, int movementSpeed)
        {
            Bitmap = bitmap;
            Position = position;
            MovementSpeed = movementSpeed;
        }

        public GraphicObject(Bitmap bitmap, int movementSpeed) : this(bitmap, Form1.Beyond, movementSpeed) { }

        public GraphicObject(Bitmap bitmap, Point position) : this(bitmap, position, 0) { }

        public GraphicObject(Bitmap bitmap) : this(bitmap, Form1.Beyond) { }

        public GraphicObject(int movementSpeed) : this(null, movementSpeed) { }

        public GraphicObject(Point position, int movementSpeed) : this(null, position, movementSpeed) { }

        public void Paint(Graphics graphics)
        {
            if (Bitmap != null)
                graphics.DrawImage(Bitmap, Position);
        }

        public void GoLeft() => shift = new Size(-MovementSpeed, 0);

        public void GoRight() => shift = new Size(MovementSpeed, 0);

        public void GoUp() => shift = new Size(0, -MovementSpeed);

        public void GoDown() => shift = new Size(0, MovementSpeed);

        public void Stop() => shift = new Size(0, 0);

        public void Move() => Position += shift;
    }
}
