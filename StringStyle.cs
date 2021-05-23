using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rewrite_It
{
    /// <summary>
    /// Класс стандартного стиля шрифта, используемого в игре.
    /// </summary>
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
        public static Font TextFont { get; private set; }

        public static void Initialize()
        {
            var fontCollection = new PrivateFontCollection();
            fontCollection.AddFontFile("PixelGeorgia.ttf");
            FontFamily = fontCollection.Families[0];
            Brush = new SolidBrush(Color.Black);
            TextFont = new Font(FontFamily, 16);
        }
    }
}
