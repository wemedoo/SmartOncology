using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Events;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf;
using iText.Layout.Properties;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Form;
using System;
using iText.Layout;
using iText.Layout.Element;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System.Globalization;

namespace Chapters.Helpers
{
    public abstract class HeaderFooterEventHandler : IEventHandler
    {
        protected const int footerY = 20;

        protected readonly int _footerMargin;
        protected Document _document;
        protected readonly string _basePath;
        protected readonly int _rectangleWidth;
        protected readonly PdfFont _pdfFont;

        protected HeaderFooterEventHandler(Document document, string basePath, int rectangleWidth, int footerMargin, PdfFont pdfFont)
        {
            this._document = document;
            this._basePath = basePath;
            this._rectangleWidth = rectangleWidth;
            this._footerMargin = footerMargin;
            this._pdfFont = pdfFont;
        }

        public void HandleEvent(Event currentEvent)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)currentEvent;
            PdfDocument pdfDoc = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            PdfCanvas pdfCanvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdfDoc);

            Table headerFooterTable = new Table(2);
            headerFooterTable.SetWidth(UnitValue.CreatePercentValue(100));  // Set table width to 100%
            headerFooterTable.SetFontSize(10);
            headerFooterTable.SetFontColor(new DeviceRgb(89, 89, 89));
            headerFooterTable.SetFont(_pdfFont);

            RenderContent(pdfDoc, page, headerFooterTable);

            new Canvas(pdfCanvas, pdfDoc, page.GetPageSize()).Add(headerFooterTable);
            pdfCanvas.Release();
        }

        public abstract void RenderContent(PdfDocument pdfDoc, PdfPage page, Table headerFooterTable);

        protected void SetCellPaddingAndWidth(Cell cell1, Cell cell2)
        {
            cell1.SetPaddingLeft(10f);
            cell1.SetWidth(UnitValue.CreatePercentValue(50));
            cell2.SetPaddingRight(10f);
            cell2.SetWidth(UnitValue.CreatePercentValue(50));
        }
    }

    public class HeaderEventHandler : HeaderFooterEventHandler
    {
        private readonly Form form;

        public HeaderEventHandler(Document document, string basePath, int rectangleWidth, int footerMargin, Form form, PdfFont pdfFont) : base(document, basePath, rectangleWidth, footerMargin, pdfFont)
        {
            this.form = form;
        }

        public override void RenderContent(PdfDocument pdfDoc, PdfPage page, Table headerFooterTable)
        {
            int pageNumber = pdfDoc.GetPageNumber(page);
            string formInfoText = $"{form.Title} v{form.Version.Major}.{form.Version.Minor} {form.EntryDatetime.ToString(DateConstants.DateFormat, CultureInfo.InvariantCulture)}";
            Cell cell1 = new Cell().Add(new Paragraph(formInfoText));
            cell1.SetVerticalAlignment(VerticalAlignment.MIDDLE);

            string imagePath = $@"{_basePath}\AppResource\footerLogo.png";
            ImageData data = ImageDataFactory.CreatePng(new System.Uri(imagePath));
            Image img = new Image(data).ScaleToFit(100, 100);
            img.SetHorizontalAlignment(HorizontalAlignment.RIGHT);
            Cell cell2 = new Cell();
            SetCellPaddingAndWidth(cell1, cell2);
            cell2.SetPaddingTop(6f);
            cell2.Add(img);

            headerFooterTable.AddCell(cell1);
            headerFooterTable.AddCell(cell2);

            float bottom = page.GetPageSize().GetTop() - _document.GetTopMargin() - footerY;
            headerFooterTable.SetFixedPosition(pageNumber, _footerMargin, bottom, _rectangleWidth);
        }
    }

    public class FooterEventHandler : HeaderFooterEventHandler
    {
        private readonly string activeUserNameInfo;
        private readonly Organization organization;

        public FooterEventHandler(Document document, string basePath, int rectangleWidth, int footerMargin, string activeUserNameInfo, Organization organization, PdfFont pdfFont) : base(document, basePath, rectangleWidth, footerMargin, pdfFont)
        {
            this.activeUserNameInfo = activeUserNameInfo;
            this.organization = organization;
        }

        public override void RenderContent(PdfDocument pdfDoc, PdfPage page, Table headerFooterTable)
        {
            int pageNumber = pdfDoc.GetPageNumber(page);
            int totalPageNumber = pdfDoc.GetNumberOfPages();
            string organizationDateNow = DateTimeOffset.Now.ToTimeZoned(DateConstants.DateFormat, timezoneOffset: organization.TimeZoneOffset, seconds: true);
            string printedByText = $"Printed by: {activeUserNameInfo} \n {organizationDateNow} (UTC {organization.TimeZoneOffset})";
            Cell cell1 = new Cell().Add(new Paragraph(printedByText).SetMultipliedLeading(0.8f));
            Cell cell2 = new Cell().Add(new Paragraph($"Page {pageNumber} of {totalPageNumber}"));
            SetCellPaddingAndWidth(cell1, cell2);
            cell2.SetTextAlignment(TextAlignment.RIGHT);
            headerFooterTable.AddCell(cell1);
            headerFooterTable.AddCell(cell2);

            headerFooterTable.SetFixedPosition(pageNumber, _footerMargin, footerY, _rectangleWidth);
        }
    }
}
