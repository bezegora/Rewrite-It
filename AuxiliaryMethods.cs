using System.Windows.Forms;
using System.Drawing;

namespace Rewrite_It
{
    public static class AuxiliaryMethods
    {
        public static bool CursorIsHoveredArea(Rectangle rectangle) =>
            Cursor.Position.X >= rectangle.X && Cursor.Position.Y >= rectangle.Y &&
            Cursor.Position.X <= rectangle.X + rectangle.Width &&
            Cursor.Position.Y <= rectangle.Y + rectangle.Height;

        public static void AddLabelsToControls(Control.ControlCollection controls, params Label[] labels) =>
            controls.AddRange(labels);
    }
}
