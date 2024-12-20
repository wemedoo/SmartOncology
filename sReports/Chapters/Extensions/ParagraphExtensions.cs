﻿using iText.Kernel.Colors;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chapters
{
    public static class ParagraphExtensions
    {
        public static void SetColor(this Paragraph paragraph, string colorOfText)
        {
            if (colorOfText.Equals("white"))
            {
                paragraph.SetFontColor(ColorConstants.WHITE);
                paragraph.SetFontSize(12);
                paragraph.SetBold();
            }
            else
            {
                var color = new DeviceRgb(61, 69, 69);
                paragraph.SetFontColor(color);

            }
        }

    }
}
