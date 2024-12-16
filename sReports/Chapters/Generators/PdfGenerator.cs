using iText.Forms;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System;
using System.IO;

namespace Chapters.Generators
{
    public abstract class PdfGenerator
    {
        protected const int TextMaxLength = 90;

        protected readonly string basePath;
        protected PdfDocument pdfDocument;
        protected Document document;
        protected PdfAcroForm pdfAcroForm;
        protected PdfWriter pdfWritter;
        protected MemoryStream stream;
        protected Organization organization;
        protected PdfFont font;


        protected PdfGenerator(Organization organization, string fontName)
        {
            this.basePath = DirectoryHelper.AppDataFolder;
            this.organization = organization;
            this.font = PdfFontFactory.CreateFont($@"{basePath}\AppResource\{fontName}.ttf", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
        }

        protected abstract void PopulatePdf();

        protected void Flush()
        {
            document.Flush();
        }

        public byte[] Generate()
        {
            InitializeDocument();
            PopulatePdf();
            pdfDocument.Close();

            return GetPdfBytes();
        }

        private void InitializeDocument()
        {
            stream = new MemoryStream();
            pdfWritter = new PdfWriter(stream);
            pdfDocument = new PdfDocument(pdfWritter);
            SetDocument();
            pdfAcroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);
            pdfAcroForm.SetGenerateAppearance(true);
        }

        private void SetDocument()
        {
            // Disclaimer : for large contents we have to set immediateFlush to FALSE and call .flush() every time we add something to the pdf. See https://stackoverflow.com/questions/62482758/switch-document-renderer-cannot-draw-elements-on-already-flushed-pages
            document = new Document(pdfDocument, PageSize.Default, immediateFlush: false);
        }

        private byte[] GetPdfBytes()
        {
            byte[] pdfBytes = null;

            if (stream != null)
            {
                try
                {
                    Flush();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                pdfBytes = stream.ToArray();
            }

            return pdfBytes;
        }
    }
}
