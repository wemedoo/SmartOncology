using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Color = iText.Kernel.Colors.Color;
using Image = iText.Layout.Element.Image;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using iText.Layout.Borders;
using iText.Svg.Converter;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using sReportsV2.Common.Helpers;
using sReportsV2.Common.Constants;
using System;
using Chapters.Generators;
using System.Globalization;

namespace Chapters
{
    public class SynopticFontStyle
    {
        public readonly int chapterSize = 16;
        public readonly int pageSize = 14;
        public readonly int fieldSetSize = 12;
        public readonly int labelsValuesSize = 10;
        public readonly int footerTextSize = 8;
        public readonly PdfFont fontType;
        public readonly float spacing = 1.15f;

        public SynopticFontStyle(PdfFont fontType)
        {
            this.fontType = fontType;
        }
    }


    public class PdfParameters
    {
        public readonly int DocumentWidth = 595;
        public readonly int DocumentHeight = 842;

        public readonly int RightMargin, LeftMargin, TopMargin, BottomMargin;

        public readonly int headerUpperLimit = 24;
        public readonly int footerLowerLimit = 24;

        public readonly int logoMaxWidth = 70;
        public readonly int logoMaxHeight = 40;

        public readonly SynopticFontStyle Font;

        public PdfParameters(int rightMargin, int leftMargin, int topMargin, int bottomMargin, SynopticFontStyle fontStyle)
        {
            RightMargin = rightMargin;
            LeftMargin = leftMargin;
            TopMargin = topMargin;
            BottomMargin = bottomMargin;
            Font = fontStyle;
        }

        public int GetAvailableWidth()
        {
            return DocumentWidth - RightMargin - LeftMargin;
        }
        public int GetAvailableHeight()
        {
            return DocumentHeight - TopMargin - BottomMargin;
        }
        public DeviceRgb Green()
        {
            return new DeviceRgb(18, 112, 124);
        }
        public DeviceRgb LightGray()
        {
            return new DeviceRgb(237, 238, 240);
        }
        public DeviceRgb Fucsia()
        {
            return new DeviceRgb(227, 50, 130);
        }
        public DeviceRgb DarkGray()
        {
            return new DeviceRgb(73, 80, 87);
        }
    }
    public class SynopticPdfGenerator : PdfGenerator
    {
        private readonly string signingUserCompleteName;
        private readonly FormDataOut formDataOut;
        private readonly PdfParameters parameters;

        public SynopticPdfGenerator(FormDataOut formDataOut, string signingUserCompleteName, Organization organization) : base(organization, "NUNITOSANS-REGULAR")
        {
            this.formDataOut = formDataOut;
            parameters = new PdfParameters(70, 70, 100, 120, new SynopticFontStyle(this.font));
            this.signingUserCompleteName = signingUserCompleteName;
        }

        protected override void PopulatePdf()
        {
            document.SetMargins(parameters.TopMargin, parameters.RightMargin, parameters.BottomMargin, parameters.LeftMargin);

            pdfDocument.AddNewPage();

            AddSynopticElements();
            AddHeaderAndFooter();
        }

        private void AddHeaderAndFooter()
        {
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                AddHeaderText(i);
                AddHeaderLogo(i);
                AddFooter(i);
            }
        }

        private void AddHeaderText(int pageNumber)
        {
            string title = StringShortener($"{formDataOut.Title} (v{formDataOut.Version.Major}.{formDataOut.Version.Minor})", TextMaxLength);

            Paragraph headerLabel = CreateCustomParagraph(" Document:", parameters.Font.labelsValuesSize, parameters.Green(), isBold: true);
            headerLabel.SetWidth(200);  // setting width for constraining text in limited area

            Paragraph headerTitle = CreateCustomParagraph(title, parameters.Font.labelsValuesSize, parameters.DarkGray());
            headerTitle.SetWidth(200);  // setting width for constraining text in limited area

            Table lineTable = new Table(1).SetWidth(parameters.GetAvailableWidth()).SetHeight(1).SetBorderBottom(new SolidBorder(parameters.LightGray(), 1));

            document.ShowTextAligned(new Paragraph().Add(headerLabel), parameters.LeftMargin, parameters.DocumentHeight - parameters.headerUpperLimit, pageNumber, TextAlignment.LEFT, VerticalAlignment.MIDDLE, 0);
            Flush();
            document.ShowTextAligned(new Paragraph().Add(headerTitle), parameters.LeftMargin, parameters.DocumentHeight - parameters.headerUpperLimit - 2, pageNumber, TextAlignment.LEFT, VerticalAlignment.TOP, 0);
            Flush();
            document.ShowTextAligned(new Paragraph().Add(lineTable), parameters.LeftMargin, parameters.DocumentHeight - parameters.headerUpperLimit - 50, pageNumber, TextAlignment.LEFT, VerticalAlignment.MIDDLE, 0);
            Flush();
        }

        private void AddHeaderLogo(int pageNumber)
        {
            string imagePath = organization.LogoUrl;
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                Image img = LoadCustomImage(imagePath);

                if (img != null)
                {
                    img.ScaleToFit(parameters.logoMaxWidth, parameters.logoMaxHeight);
                    img.SetFixedPosition(
                        pageNumber,
                        parameters.DocumentWidth - parameters.RightMargin - img.GetImageScaledWidth(),
                        parameters.DocumentHeight - parameters.headerUpperLimit - img.GetImageScaledHeight());
                    document.Add(img);

                    Flush();
                }
            }
        }

        private void AddFooter(int pageNumber)
        {
            string organizationImpressum = StringShortener(!string.IsNullOrWhiteSpace(organization.Impressum) ? organization.Impressum : string.Empty, 600);
            Paragraph impressumParagraph = CreateCustomParagraph(organizationImpressum, parameters.Font.footerTextSize, parameters.DarkGray());
            impressumParagraph.SetWidth(parameters.GetAvailableWidth()).SetMinHeight(55);
            impressumParagraph.SetFixedLeading(parameters.Font.labelsValuesSize).SetWordSpacing(0.1f);

            // Last Update and Signature line 
            Paragraph lastUpdateLabel = CreateCustomParagraph("Last Update: ", parameters.Font.labelsValuesSize, parameters.Green(), isBold: true);
            Paragraph lastUpdateValue = CreateCustomParagraph(formDataOut.LastUpdate.Value.ToString(DateConstants.DateFormat, CultureInfo.InvariantCulture), parameters.Font.labelsValuesSize, parameters.DarkGray());
            lastUpdateLabel.Add(lastUpdateValue).SetFixedLeading(parameters.Font.labelsValuesSize);

            Paragraph SignatureLabel = CreateCustomParagraph(!string.IsNullOrWhiteSpace(signingUserCompleteName) ? "Electronically Signed: " : string.Empty, 
                parameters.Font.labelsValuesSize, parameters.Green(), isBold: true);
            Paragraph SignatureValue = CreateCustomParagraph(signingUserCompleteName, parameters.Font.labelsValuesSize, parameters.DarkGray());
            SignatureLabel.Add(SignatureValue).SetFixedLeading(parameters.Font.labelsValuesSize);

            Table updateAndSignatureTable = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100f))
                .SetFixedLayout();

            updateAndSignatureTable
                .AddCell(new Cell().Add(lastUpdateLabel).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT))
                .AddCell(new Cell().Add(SignatureLabel).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));

            // Table used to draw a horizontal line
            Table lineTable = new Table(1).SetWidth(parameters.GetAvailableWidth()).SetHeight(1).SetBorderBottom(new SolidBorder(parameters.LightGray(), 1));

            // WeMedoo Informations line
            Paragraph wemedooPoweredByText = CreateCustomParagraph("Powered by: ", parameters.Font.labelsValuesSize, parameters.DarkGray())
                .Add(CreateCustomParagraph("WeMedoo ", parameters.Font.labelsValuesSize, parameters.DarkGray(), isBold: true))
                .Add(CreateCustomParagraph("AG", parameters.Font.labelsValuesSize, parameters.DarkGray()));

            Paragraph wemedooInfo = CreateCustomParagraph("info@wemedoo.com", parameters.Font.labelsValuesSize, parameters.DarkGray());

            // Adding elements from the bottom of the page upwards
            document.ShowTextAligned(wemedooPoweredByText, parameters.LeftMargin, parameters.footerLowerLimit, pageNumber, TextAlignment.LEFT, VerticalAlignment.MIDDLE, 0);
            Flush();
            document.ShowTextAligned(wemedooInfo, parameters.DocumentWidth - parameters.RightMargin, parameters.footerLowerLimit, pageNumber, TextAlignment.RIGHT, VerticalAlignment.MIDDLE, 0);
            Flush();
            document.ShowTextAligned(new Paragraph().Add(lineTable), parameters.LeftMargin, parameters.footerLowerLimit + 10, pageNumber, TextAlignment.LEFT, VerticalAlignment.MIDDLE, 0);
            Flush();

            int impressumOffset = !string.IsNullOrWhiteSpace(organizationImpressum) ? 75 : 10; // if Impressum not present, we put LastUpdate+Signature down
            
            document.ShowTextAligned(new Paragraph().Add(impressumParagraph), parameters.LeftMargin, parameters.footerLowerLimit + impressumOffset, pageNumber, TextAlignment.LEFT, VerticalAlignment.TOP, 0);
            Flush();
            document.ShowTextAligned(new Paragraph().Add(lineTable), parameters.LeftMargin, parameters.footerLowerLimit + impressumOffset, pageNumber, TextAlignment.LEFT, VerticalAlignment.MIDDLE, 0);
            Flush();
            updateAndSignatureTable.SetFixedPosition(pageNumber, parameters.LeftMargin, parameters.footerLowerLimit + impressumOffset, parameters.GetAvailableWidth());
            document.Add(updateAndSignatureTable);
            Flush();
        }

        private void AddSynopticElements()
        {
            foreach (FormChapterDataOut chapter in formDataOut.Chapters)
            {
                AddChapter(chapter.Title);

                foreach (FormPageDataOut page in chapter.Pages)
                {
                    AddPage(page.Title);
                    AddSpacing(15);

                    foreach (List<FormFieldSetDataOut> fieldSet in page.ListOfFieldSets)
                    {
                        int numOfFieldSetInstanceRepetitions = fieldSet.Count;
                        bool addFiledSetRepetitionToLabel = numOfFieldSetInstanceRepetitions > 1;
                        for (int i = 0; i < numOfFieldSetInstanceRepetitions; i++)
                        {
                            FormFieldSetDataOut repetitiveFieldSet = fieldSet[i];
                            AddFieldSetLabel(repetitiveFieldSet.Label + (addFiledSetRepetitionToLabel ? $" ({i + 1})" : string.Empty));
                            AddRepetitiveFieldSetValues(repetitiveFieldSet);
                        }
                    }
                }
            }
        }

        private void AddChapter(string text)
        {
            document.Add(
                    CreateCustomParagraph(text, parameters.Font.chapterSize, parameters.Green(), isBold: true));
            Flush();
        }

        private void AddPage(string text)
        {
            Table pageTitle = CreateCustomTable(
                        text,
                        parameters.Font.pageSize,
                        parameters.Green(),
                        isBold: true,
                        textLeftPadding: 10);

            pageTitle.SetBorderLeft(new SolidBorder(parameters.Fucsia(), 2));

            document.Add(new Paragraph().Add(pageTitle).SetFixedLeading(parameters.Font.pageSize));
            Flush();
        }

        private void AddFieldSetLabel(string label)
        {
            if (label != null)
            {
                document.Add(CreateCustomTable(
                    label,
                    parameters.Font.fieldSetSize,
                    isBold: true,
                    textLeftPadding: 10,
                    background: parameters.LightGray()));

                Flush();
            }
        }

        private void AddRepetitiveFieldSetValues(FormFieldSetDataOut repetitiveFieldSet)
        {
            Table labelValue = new Table(2, false);
            labelValue.SetWidth(UnitValue.CreatePercentValue(100f));
            labelValue.SetFixedLayout();
            int fieldsCounter = 1;

            foreach (FieldDataOut field in repetitiveFieldSet.Fields.Where(f => f.IsVisible))
            {
                int numOfFieldInstanceRepetitions = field.GetRepetitiveFieldCount();

                for (int j = 0; j < numOfFieldInstanceRepetitions; j++)
                {
                    if (!string.IsNullOrWhiteSpace(field.Label) && field.HasValue(j))
                    {
                        AddLabelAndValueToTable(field.Label, field.GetSynopticValue(j, ResourceTypes.NotDefined, Environment.NewLine), repetitiveFieldSet.Fields, fieldsCounter, ref labelValue);
                    }
                }
                fieldsCounter++;
            }
            document.Add(labelValue);
            Flush();
            AddSpacing(10);
        }

        private void AddLabelAndValueToTable(string fieldLabel, string fieldValue, List<FieldDataOut> fields, int fieldsCounter, ref Table labelValue)
        {
            Paragraph label = CreateCustomParagraph(fieldLabel, parameters.Font.labelsValuesSize, parameters.DarkGray(), textLeftPadding: 10);
            Paragraph value = CreateCustomParagraph(fieldValue, parameters.Font.labelsValuesSize, textLeftPadding: 10);

            Cell labelCell = new Cell().Add(label).SetBorder(Border.NO_BORDER).SetKeepTogether(true);
            Cell valueCell = new Cell().Add(value).SetBorder(Border.NO_BORDER).SetKeepTogether(true);

            if (fieldsCounter < fields.Count(f => f.HasValue()))  // last one row doesn't need the bottom border
            {
                labelCell.SetBorderBottom(new SolidBorder(parameters.LightGray(), 1));
                valueCell.SetBorderBottom(new SolidBorder(parameters.LightGray(), 1));
            }

            labelValue.AddCell(labelCell);
            labelValue.AddCell(valueCell);
        }

        // Helper Methods

        private Paragraph CreateCustomParagraph(string text, int textSize, Color textColor = null, bool isBold = false, int textLeftPadding = 0)
        {
            Text t = new Text(text);
            t.SetFont(parameters.Font.fontType);
            t.SetFontSize(textSize);
            t.SetFontColor(textColor);
            if (isBold)
                t.SetBold();

            Paragraph p = new Paragraph(t);
            p.SetMarginLeft(textLeftPadding);
            p.SetWordSpacing(parameters.Font.spacing);

            return p;
        }

        private Table CreateCustomTable(string text, int textSize, Color textColor = null, bool isBold = false, int textLeftPadding = 0, Border border = null, Color background = null)
        {
            Paragraph p = CreateCustomParagraph(text, textSize, textColor, isBold, textLeftPadding);

            Table t = new Table(1);
            t.SetWidth(UnitValue.CreatePercentValue(100f));
            t.SetFixedLayout();

            Cell c = new Cell().Add(p);
            c.SetBorder(border);
            c.SetBackgroundColor(background);
            t.AddCell(c);

            return t;
        }

        private Image LoadCustomImage(string imagePath)
        {
            Image img = null;

            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                try
                {
                    if (imagePath.Contains(".png") || imagePath.Contains(".jpg") || imagePath.Contains(".jpeg"))
                    {
                        ImageData data = ImageDataFactory.Create(new System.Uri(imagePath));
                        img = new Image(data);
                    }
                    else if (imagePath.Contains(".svg"))
                    {
                        var req = System.Net.WebRequest.Create(new System.Uri(imagePath));
                        using (Stream stream = req.GetResponse().GetResponseStream())
                        {
                            img = SvgConverter.ConvertToImage(stream, pdfDocument);
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    LogHelper.Error(ex.Message);
                }
            }
            return img;
        }

        private string StringShortener(string input, int charLimit)
        {
            if (input.Length > charLimit)
                input = input.Substring(0, charLimit - 3) + "...";

            return input;
        }

        private void AddSpacing(float height)
        {
            document.Add(new Table(1).SetHeight(height));
            Flush();
        }
    }
}
