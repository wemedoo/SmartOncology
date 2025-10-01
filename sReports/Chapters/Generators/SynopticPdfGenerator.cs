using iText.Layout.Element;
using iText.Layout.Properties;
using System.Collections.Generic;
using System.Linq;
using Color = iText.Kernel.Colors.Color;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.Field.DataOut;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using sReportsV2.Common.Constants;
using System;
using Chapters.Generators;
using System.Globalization;
using Chapters.Resources;
using Chapters.Extensions;
using sReportsV2.Common.Extensions;

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


    public class SynopticPdfParameters
    {
        public readonly int DocumentWidth = 595;

        public readonly int RightMargin, LeftMargin, TopMargin, BottomMargin;

        public readonly int footerLowerLimit = 45;

        public readonly SynopticFontStyle Font;

        public SynopticPdfParameters(int rightMargin, int leftMargin, int topMargin, int bottomMargin, SynopticFontStyle fontStyle)
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
        private readonly SynopticPdfParameters parameters;

        public SynopticPdfGenerator(FormDataOut formDataOut, string signingUserCompleteName, Organization organization, string activeUserNameInfo, string patientIdentifier) : base(organization, "NUNITOSANS-REGULAR", activeUserNameInfo, new FormPdfMetadata(formDataOut.Title, formDataOut.Version, formDataOut.EntryDatetime.Value, patientIdentifier))
        {
            this.formDataOut = formDataOut;
            this.parameters = new SynopticPdfParameters(50, 50, 60, 150, new SynopticFontStyle(this.font));
            this.signingUserCompleteName = signingUserCompleteName;
        }

        public string GetDownloadedSynopticPdfName()
        {
            string identifierPart = !string.IsNullOrEmpty(this.formMetadata.PatientIdentifier) ? $"_{this.formMetadata.PatientIdentifier}" : string.Empty;
            return $"{formMetadata.Title}_{formMetadata.Version.Major}.{formMetadata.Version.Minor}{identifierPart}_{DateTime.Now.ToString(DateTimeConstants.DateFormat)}";
        }
        protected override void PopulatePdf()
        {
            document.SetMargins(parameters.TopMargin, parameters.RightMargin, parameters.BottomMargin, parameters.LeftMargin);
            pdfDocument.AddNewPage();

            AddSynopticElements();
            AddFormInstanceMetadataAtBottom();
        }

        #region FormInstance Metadata

        private void AddFormInstanceMetadataAtBottom()
        {
            for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
            {
                AddFormInstanceMetadata(i);
            }
        }

        private void AddFormInstanceMetadata(int pageNumber)
        {
            string organizationImpressum = organization.Impressum.StringShortener(600);
            Paragraph impressumParagraph = CreateCustomParagraph(organizationImpressum, parameters.Font.footerTextSize, parameters.DarkGray());
            impressumParagraph.SetWidth(parameters.GetAvailableWidth()).SetMinHeight(55);
            impressumParagraph.SetFixedLeading(parameters.Font.labelsValuesSize).SetWordSpacing(0.1f);

            // Last Update and Signature line 
            Paragraph lastUpdateLabel = CreateCustomParagraph("Last Update: ", parameters.Font.labelsValuesSize, parameters.Green(), isBold: true);
            Paragraph lastUpdateValue = CreateCustomParagraph(formDataOut.LastUpdate.Value.GetDateTimeDisplay(DateTimeConstants.DateFormat, excludeTimePart: true), parameters.Font.labelsValuesSize, parameters.DarkGray());
            lastUpdateLabel.Add(lastUpdateValue).SetFixedLeading(parameters.Font.labelsValuesSize);

            Paragraph SignatureLabel = CreateCustomParagraph(!string.IsNullOrWhiteSpace(signingUserCompleteName) ? "Electronically Signed: " : string.Empty, 
                parameters.Font.labelsValuesSize, parameters.Green(), isBold: true);
            Paragraph SignatureValue = CreateCustomParagraph(signingUserCompleteName, parameters.Font.labelsValuesSize, parameters.DarkGray());
            SignatureLabel.Add(SignatureValue).SetFixedLeading(parameters.Font.labelsValuesSize);

            Table updateAndSignatureTable = new Table(2)
                .SetWidth(UnitValue.CreatePercentValue(100f))
                .SetBorderBottom(new SolidBorder(parameters.LightGray(), 1))
                .SetFixedLayout();

            updateAndSignatureTable
                .AddCell(new Cell().Add(lastUpdateLabel).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.LEFT))
                .AddCell(new Cell().Add(SignatureLabel).SetBorder(Border.NO_BORDER).SetTextAlignment(TextAlignment.RIGHT));

            int impressumOffset = !string.IsNullOrWhiteSpace(organizationImpressum) ? 75 : 10; // if Impressum not present, we put LastUpdate+Signature down

            int bottom = parameters.footerLowerLimit + impressumOffset;
            document.ShowTextAligned(new Paragraph().Add(impressumParagraph), parameters.LeftMargin, bottom, pageNumber, TextAlignment.LEFT, VerticalAlignment.TOP, 0);
            Flush();
            updateAndSignatureTable.SetFixedPosition(pageNumber, parameters.LeftMargin, bottom, parameters.GetAvailableWidth());
            document.Add(updateAndSignatureTable);
            Flush();
        }

        #endregion /FormInstance Metadata

        #region Synoptic Elements

        private void AddSynopticElements()
        {
            foreach (FormChapterDataOut chapter in formDataOut.Chapters)
            {
                AddChapter(chapter.Title);

                foreach (FormPageDataOut page in chapter.Pages)
                {
                    AddPage(page.Title);
                    AddSpacing(15);

                    foreach (List<FormFieldSetDataOut> fieldSets in page.ListOfFieldSets)
                    {
                        List<FormFieldSetDataOut> listOfFieldsets = fieldSets.GetEffectiveFieldSets();

                        int numOfFieldSetInstanceRepetitions = listOfFieldsets.Count;
                        bool addFiledSetRepetitionToLabel = numOfFieldSetInstanceRepetitions > 1;
                        for (int i = 0; i < numOfFieldSetInstanceRepetitions; i++)
                        {
                            FormFieldSetDataOut repetitiveFieldSet = listOfFieldsets[i];
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

            int fieldIndex = 1;
            int numOfFieldsWithSynopticValue = repetitiveFieldSet.Fields.Count(f => f.IsVisible && f.HasAnyValue());
            foreach (FieldDataOut field in repetitiveFieldSet.Fields.Where(f => f.IsVisible && f.HasAnyValue()))
            {
                List<int> repetitiveFieldInstanceIndexesWithValues = field.GetRepetitiveFieldInstanceIndexesWithValues();

                for (int i = 0; i < repetitiveFieldInstanceIndexesWithValues.Count; i++) 
                {
                    AddLabelAndValueToTable(
                        field.Label, 
                        field.GetSynopticValue(repetitiveFieldInstanceIndexesWithValues[i], ResourceTypes.NotDefined, Environment.NewLine), 
                        fieldIndex == numOfFieldsWithSynopticValue && i == repetitiveFieldInstanceIndexesWithValues.Count - 1,
                        ref labelValue);
                }

                ++fieldIndex;
            }
            document.Add(labelValue);
            Flush();
            AddSpacing(10);
        }

        private void AddLabelAndValueToTable(string fieldLabel, string fieldValue, bool lastSynopticValue, ref Table labelValue)
        {
            Paragraph label = CreateCustomParagraph(fieldLabel, parameters.Font.labelsValuesSize, parameters.DarkGray(), textLeftPadding: 10);
            Paragraph value = CreateCustomParagraph(fieldValue, parameters.Font.labelsValuesSize, textLeftPadding: 10);

            Cell labelCell = new Cell().Add(label).SetBorder(Border.NO_BORDER).SetKeepTogether(true);
            Cell valueCell = new Cell().Add(value).SetBorder(Border.NO_BORDER).SetKeepTogether(true);

            if (!lastSynopticValue) // last one row doesn't need the bottom border
            {
                labelCell.SetBorderBottom(new SolidBorder(parameters.LightGray(), 1));
                valueCell.SetBorderBottom(new SolidBorder(parameters.LightGray(), 1));
            }

            labelValue.AddCell(labelCell);
            labelValue.AddCell(valueCell);
        }

        #endregion /Synoptic Elements

        #region Helper Methods

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

        private void AddSpacing(float height)
        {
            document.Add(new Table(1).SetHeight(height));
            Flush();
        }

        #endregion Helper Methods
    }
}
