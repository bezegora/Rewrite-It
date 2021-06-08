using System.Drawing;

namespace Rewrite_It
{
    public interface IUserInterface
    {
        void Paint(Graphics graphics);
        void Change(Form1 form);
        void MouseDown();
        void Tick();
    }
}
