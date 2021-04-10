using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rewrite_It
{
    static class StringStyle
    {
        /// <summary>
        /// Тип шрифта
        /// </summary>
        public static FontFamily FontFamily { get; private set; }

        /// <summary>
        /// Цвет кисти
        /// </summary>
        public static SolidBrush Brush { get; private set; }

        /// <summary>
        /// Стиль шрифта
        /// </summary>
        public static Font Font { get; private set; }

        public static void SetFontFamily(FontFamily fontFamily) => FontFamily = fontFamily;
        public static void SetSolidBrush(SolidBrush brush) => Brush = brush;
        public static void SetFont(Font font) => Font = font;
    }
}
