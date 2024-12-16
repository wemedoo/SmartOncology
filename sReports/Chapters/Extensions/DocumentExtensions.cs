using Chapters.Helpers;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Layout;
using iText.Layout.Element;
using System.Collections.Generic;


namespace Chapters
{
    public static class DocumentExtensions
    {
        public static void AddParagraph(this Document doc, ParagraphParameters paragraphParameters, string text, int padding, int fontSize, int pageCounter, ref int offset, int additionalPadding, int additionalOffset = 0, bool isBlack = false, bool isPageDescription = false)
        {
            if (text != null)
            {
                List<string> paragraphValues = text.GetRows(paragraphParameters.TextMaxLength);

                foreach (string value in paragraphValues)
                {
                    offset++;
                    Paragraph paragraph = new Paragraph(value);
                    paragraph.SetFixedPosition(padding, GetY(paragraphParameters, - paragraphParameters.Step * offset + additionalOffset + additionalPadding), paragraphParameters.PageWidth);
                    paragraph.SetPageNumber(pageCounter);
                    paragraph.SetFontSize(fontSize - 1);

                    if (isBlack)
                    {
                        paragraph.SetFontColor(new DeviceRgb(0, 0, 0));
                    }
                    else
                    {
                        paragraph.SetFontColor(new DeviceRgb(61, 69, 69));
                    }

                    if (isPageDescription)
                    {
                        paragraph.SetFontColor(new DeviceRgb(104, 115, 165));
                        paragraph.SetItalic();
                    }

                    if (fontSize > 9 && !isPageDescription)
                    {
                        paragraph.SetBold();
                        //paragraph.SetStrokeWidth(0);
                    }

                    paragraph.SetFont(paragraphParameters.Font);
                    doc.Add(paragraph);
                    doc.Flush();
                }
            }
        }

        public static void AddParagraphs(this Document doc, List<Paragraph> paragraphs)
        {
            foreach (Paragraph paragraph in paragraphs)
            {
                doc.Add(paragraph);
                doc.Flush();
            }
        }

        public static void AddPageImage(this Document document, string imagePath, int pageNum, float defaultPageWidth, int bottom, ref int additionalPadding)
        {
            Image img = new Image(GetDataByImageExtension(imagePath));
            float imgHeight = img.GetImageHeight();
            float imgWidth = img.GetImageWidth();
            if (bottom - imgHeight > 70 && imgWidth < defaultPageWidth)
            {
                img.SetFixedPosition(pageNum, (defaultPageWidth + 94 - imgWidth) / 2, bottom - imgHeight);
                additionalPadding -= (int)img.GetImageHeight();
                document.Add(img);
            }
            else 
            {
                if (bottom - imgHeight < 70) 
                {
                    //scale by height  
                    float corectorOfImageSizeByHeight = (float)(bottom - 70) / imgHeight;
                    imgHeight = imgHeight * corectorOfImageSizeByHeight;
                    imgWidth = imgWidth * corectorOfImageSizeByHeight;
                }

                if (imgWidth > defaultPageWidth) 
                {
                    //scale by width
                    float corectorOfImageSizeByWidth = defaultPageWidth / imgWidth;
                    imgHeight = imgHeight * corectorOfImageSizeByWidth;
                    imgWidth = imgWidth * corectorOfImageSizeByWidth;
                }

                img.SetFixedPosition(pageNum, (defaultPageWidth + 94 - imgWidth) / 2 , bottom - imgHeight);
                img.SetWidth(imgWidth);
                img.SetHeight(imgHeight);

                additionalPadding -= (int)(imgHeight + 5);
                
                document.Add(img);
                document.Flush();
            }
        }
        public static void AddImage(this Document document, string imagePath, int pageNum, int left, int bottom, int height, int width)
        {
            Image img = new Image(GetDataByImageExtension(imagePath));
            img.SetHeight(height);
            img.SetWidth(width);
            img.SetFixedPosition(pageNum, left, bottom);
            document.Add(img);
            document.Flush();
        }

        public static ImageData GetDataByImageExtension(string imagePath)
        {
            ImageData data = null;
            switch (System.IO.Path.GetExtension(imagePath).ToUpperInvariant())
            {
                case PdfGeneratorType.Jpg:
                    {
                        data = ImageDataFactory.CreateJpeg(new System.Uri(imagePath));
                    }
                    break;
                case PdfGeneratorType.Png:
                    {
                        data = ImageDataFactory.CreatePng(new System.Uri(imagePath));
                    }
                    break;
            }
            return data;
        }

        private static int GetY(ParagraphParameters paragraphParameters, int customCalculation)
        {
            return paragraphParameters.PageHeight + customCalculation - paragraphParameters.PageMargin;
        }
    }
}
