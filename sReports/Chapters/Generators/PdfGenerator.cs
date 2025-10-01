using Chapters.Helpers;
using Chapters.Resources;
using iText.Forms;
using iText.IO.Font;
using iText.Kernel.Events;
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
        protected const int RectanglePadding = 81;
        protected const int ChapterPaddingOffset = 60;
        protected const int RectangleWidth = 557;

        protected readonly string basePath;
        protected PdfDocument pdfDocument;
        protected Document document;
        protected PdfAcroForm pdfAcroForm;
        protected PdfWriter pdfWritter;
        protected MemoryStream stream;
        protected Organization organization;
        protected PdfFont font;
        protected readonly string activeUserNameInfo;
        protected readonly FormPdfMetadata formMetadata;

        protected PdfGenerator(Organization organization, string fontName, string activeUserNameInfo, FormPdfMetadata formMetadata)
        {
            this.basePath = DirectoryHelper.AppDataFolder;
            this.organization = organization;
            this.font = PdfFontFactory.CreateFont($@"{basePath}\AppResource\{fontName}.ttf", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);
            this.activeUserNameInfo = activeUserNameInfo;   
            this.formMetadata = formMetadata;
        }

        protected abstract void PopulatePdf();

        protected void Flush()
        {
            document.Flush();
        }

        public byte[] Generate()
        {
            InitializeDocument();
            SetHeaderAndFooter();
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

        private void SetHeaderAndFooter()
        {
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderEventHandler(document, basePath, RectangleWidth, RectanglePadding - ChapterPaddingOffset, formMetadata, font));
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler(document, basePath, RectangleWidth, RectanglePadding - ChapterPaddingOffset, activeUserNameInfo, organization, font));
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
                    LogHelper.Error($"An error occurred: {ex.GetExceptionStackMessages()}");
                }
                pdfBytes = stream.ToArray();
            }

            return pdfBytes;
        }
    }
}
