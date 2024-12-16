using iText.Kernel.Font;

namespace Chapters.Helpers
{
    public class ParagraphParameters
    {
        public int TextMaxLength {  get; set; }
        public int Step {  get; set; }
        public int PageWidth {  get; set; }
        public int PageHeight {  get; set; }
        public int PageMargin {  get; set; }
        public PdfFont Font {  get; set; }
    }
}
