using iText.Forms.Fields;
using iText.Html2pdf;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Color = iText.Kernel.Colors.Color;
using Rectangle = iText.Kernel.Geom.Rectangle;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.Cache.Resources;
using Chapters.Generators;
using System;
using iText.Kernel.Events;
using Chapters.Helpers;
using iText.Html2pdf.Resolver.Font;

namespace Chapters
{
    public class FormPdfGenerator : PdfGenerator
    {
        #region Drawing constant attributes

        private const int PageFontSize = 12;
        private const int FieldFontSize = 10;
        private const int CheckAndRadioFontSize = 8;
        private const int PagePaddingX = 47;
        private const int FormFieldPadding = 87;
        private const int RectanglePadding = 81;
        private const int ChapterPaddingOffset = 60;
        private const int RectangleWidth = 557;
        private const int PageHeight = 833;
        private const int PageMargin = 50;
        private const int Step = 20;

        #endregion /Drawing constant attributes

        #region Form and User attributes

        private FormChapter currentChapter;
        private readonly Form form;
        private readonly Dictionary<string, Field> fields;

        private readonly string activeUserNameInfo;

        #endregion /Form and User attributes

        #region Helper properties

        private int pdfRowCounter;
        private int pageCounter = 1;
        private int additionalPadding;
        private int numberOfFieldsInChapter;
        private int chapterCount;

        private string html;
        private int endPosition;
        private bool isPageChanged;
        private int totalFieldCounter;

        private readonly List<RectangleParameters> rectangles;
        private readonly Dictionary<string, int> pageLinePositions;
        private readonly ParagraphParameters paragraphParameters;

        #endregion /Helper properties

        public FormPdfGenerator(Form form, Organization organization, string activeUserNameInfo) : base(organization, "Roboto-Regular")
        {
            this.activeUserNameInfo = activeUserNameInfo;
            this.form = form;
            this.fields = form.GetAllFields().ToDictionary(f => f.Id, f => f);

            this.rectangles = new List<RectangleParameters>();
            this.pageLinePositions = new Dictionary<string, int>();
            this.html = string.Empty;
            this.paragraphParameters = new ParagraphParameters
            {
                PageHeight = PageHeight,
                PageWidth = 425,
                Step = Step,
                TextMaxLength = TextMaxLength,
                PageMargin = PageMargin,
                Font = font
            };
        }

        protected override void PopulatePdf()
        {
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new HeaderEventHandler(document, basePath, RectangleWidth, RectanglePadding - ChapterPaddingOffset, form, font));
            pdfDocument.AddEventHandler(PdfDocumentEvent.END_PAGE, new FooterEventHandler(document, basePath, RectangleWidth, RectanglePadding - ChapterPaddingOffset, activeUserNameInfo, organization, font));
            pdfDocument.AddNewPage();
            AddMetadataInfo();
            AddChapters();
            AddAdditionalElements();

            SetNotes();
            MergeHtmlWithPdf();

            AddAllRectangles();
            AddPageLines();
        }

        private void AddAdditionalElements() 
        {
            IncreasePageNumber(addPageInPdf: false);
            // notes, date and formState
            AddLabelFieldPair(TextLanguage.Note + " :","note");
            AddLabelFieldPair(TextLanguage.Date + " (YYYY-MM-DD) :", "date");
        }

        private void AddLabelFieldPair(string Label, string keyName) 
        {
            CheckPagePosition();
            document.AddParagraph(paragraphParameters, Label, FormFieldPadding, FieldFontSize, pageCounter, ref pdfRowCounter, additionalPadding, 22);
            CheckPagePosition();
            AddField(keyName, "");
            ChangeAditionalPadding("-=", 7);
        }

        private string GetStyledHtmlString()
        {
            string styledHtml = $@"<style>
                    h2{{font-size:18px;}}
                    h3{{font-size: 16px; margin-left : 47px; margin-right : 47px; }}
                    p {{font-size: 12px; margin-left : 87px; margin-right : 87px; }}
                    u {{font-size: 12px; margin-left : 87px; margin-right : 87px; }}
                    img {{margin-left: 87px; margin-right : 87px; max-width : 80%; max-height : 750px; }}
                 </style> 
                <div style=""margin-bottom: {2*PageMargin}px; margin-top: {PageMargin}px; color:#3d4545; overflow: hidden;"" > {html.ReplaceAllBr()}</div>";

            return styledHtml;
        }

        private ConverterProperties GetConverterProperties()
        {
            ConverterProperties properties = new ConverterProperties();
            properties.SetFontProvider(new DefaultFontProvider());
            return properties;
        }

        private void MergeHtmlWithPdf() 
        {
            if (!string.IsNullOrWhiteSpace(html)) 
            {
                string pathToTempPdfFile = basePath + Path.DirectorySeparatorChar.ToString() + Guid.NewGuid() + "tempPdf.pdf";

                FileStream fileStream = new FileStream(pathToTempPdfFile, FileMode.Append);
                HtmlConverter.ConvertToPdf(GetStyledHtmlString(), fileStream, GetConverterProperties());

                fileStream = new FileStream(pathToTempPdfFile, FileMode.Open);
                PdfDocument htmlPdf = new PdfDocument(new PdfReader(fileStream));
                htmlPdf.CopyPagesTo(1, htmlPdf.GetNumberOfPages(), pdfDocument);

                pageCounter += htmlPdf.GetNumberOfPages();
                fileStream.Dispose();
                File.Delete(pathToTempPdfFile);
            }
        }

        private void AddNote(Help note)
        {
           html += note.Content;
        }

        private void SetNotes() 
        {
            foreach (FieldSet fieldSet in form.GetAllFieldSets()) 
            {
                if (fieldSet.Help != null) 
                {
                    AddNote(fieldSet.Help);
                }

                foreach (Field field in fieldSet.Fields) 
                {
                    if (field.Help != null)
                    {
                        AddNote(field.Help);
                    }
                }
            }   
        }

        private void AddPageLines()
        {
            int position = 0;
            int addPadding = 0;

            foreach (var kvp in pageLinePositions)
            {
                int.TryParse(kvp.Key.Split('-')[0], out position);
                int.TryParse(kvp.Key.Split('*')[1], out addPadding);

                AddLine(kvp.Value, 21, GetY(-position * Step + addPadding), 12, 3.3);
            }
        }

        private void AddIconRectangle(RectangleParameters rectangle)
        {
            int bottom = GetY(- Step * rectangle.Position + rectangle.AdditionalPadding);
            int additionalPadd = 0;
            if (rectangle.NumOfRows % 2 == 0 && rectangle.IsPencil)
            {
                additionalPadd = 11;
                document.AddImage($@"{basePath}\AppResource\fieldSetBackground.png", rectangle.PageCounter, 45, bottom, 25, 25);
            }
            string imagePath = rectangle.IsPencil ? $@"{basePath}\AppResource\icon_pencil.png" : $@"{basePath}\AppResource\fieldSetBackground.png";
            document.AddImage(imagePath, rectangle.PageCounter, 45, bottom + additionalPadd, 25, 25);
        }

        private void AddAllRectangles()
        {
            foreach (var rectangle in rectangles)
            {
                switch (rectangle.Type)
                {
                    case "fieldSets":
                        {
                            ChangePdfRowCounter("+=", rectangle.NumOfRows - 1);
                            AddRectangle(rectangle.Position, rectangle.PageCounter, rectangle.AdditionalPadding);
                            AddTextIntoRectangle(rectangle, "black", true, 74 , rectangle.AdditionalPadding, true);
                            AddIconRectangle(rectangle);
                        }
                        break;
                    case "chapters":
                        {
                            
                            AddRectangleForChapter(rectangle.Position, rectangle.PageCounter, rectangle.AdditionalPadding);
                            AddTextIntoRectangle(rectangle, "white", true, 34, rectangle.AdditionalPadding);

                        }
                        break;
                }
            }
        }

        private void AddTextIntoRectangle(RectangleParameters rectangle, string colorOfText, bool isBolded, int padding, int addPadding, bool isFieldSetTitle = false)
        {
            PdfCanvas over = new PdfCanvas(pdfDocument, rectangle.PageCounter);
            Paragraph p = GetParagraphForRectangles(rectangle.Text, colorOfText, isBolded);

            int paddingCenterText = isFieldSetTitle ? 6 : 4;
            new Canvas(over, pdfDocument.GetDefaultPageSize()).ShowTextAligned(p, padding + 10, GetY(paddingCenterText - Step * rectangle.Position + addPadding), 1, TextAlignment.LEFT, VerticalAlignment.BOTTOM, 0);

            over.SaveState();
            over.RestoreState();
        }

        private Paragraph GetParagraph(string text, List<string> values)
        {
            Paragraph p = new Paragraph();
            if (text != null)
            {
                if (values.Count > 1)
                {
                    p.SetFontSize(10);
                    for (int i = 0; i < values.Count - 1; i++)
                    {
                        p.Add(new Text(values[i] + ": ").SetBold());
                    }

                    p.Add(new Text(values[values.Count - 1]).SetBold());
                }
                else
                {
                    p = new Paragraph(text).SetFontSize(10);
                }
            }
            return p;
        }

        private Paragraph GetParagraphForRectangles(string text, string colorOfText, bool isBolded)
        {
            List<string> values = text.Split(':').ToList();
            Paragraph p = GetParagraph(text, values);
            p.SetColor(colorOfText);
            p.SetFont(font);

            if (isBolded && (values.Count < 2 || string.IsNullOrEmpty(values[1])))
            {
                p.SetBold();
            }
            
            return p;
        }

        private Color GetOrganizationColor()
        {
            Color colorHelper = organization != null && !string.IsNullOrEmpty(organization.PrimaryColor) ? new DeviceRgb(System.Drawing.ColorTranslator.FromHtml(organization.PrimaryColor)) : new DeviceRgb(236, 236, 236);
            float sum = colorHelper.GetColorValue().Sum();
            Color color = organization == null || string.IsNullOrEmpty(organization.PrimaryColor) || sum < 1.5f ? new DeviceRgb(236, 236, 236) : new DeviceRgb(System.Drawing.ColorTranslator.FromHtml(organization.PrimaryColor));

            return color;
        }

        private void AddRectangle(int position, int pageNum, int addPadding)
        {
            PdfCanvas canvas = new PdfCanvas(pdfDocument, pageNum);
            canvas.Rectangle(RectanglePadding - 35, GetY(- Step * position + addPadding), RectangleWidth - 25, 25);
            canvas.SetColor(GetOrganizationColor(), true);
            canvas.Fill();
        }

        private void AddLine(int pageNum, int x, int y,double width, double height)
        {
            PdfCanvas canvas = new PdfCanvas(pdfDocument, pageNum);
            Color color = new DeviceRgb(230, 230, 230);
            canvas.Rectangle(x, y, width, height );
            canvas.SetColor(color, true);
            canvas.Fill();
        }

        private void AddRectangleForChapter(int position, int pageNum, int addPadding)
        {
            PdfCanvas canvas = new PdfCanvas(pdfDocument, pageNum);
            Color color = new DeviceRgb(37, 43, 91);
            canvas.Rectangle(RectanglePadding - ChapterPaddingOffset, GetY(-Step * position + addPadding), RectangleWidth, 24);
            canvas.SetColor(color, true);
            canvas.Fill();
        }

        private void AddMetadataInfo()
        {
            Dictionary<string, string> dicInfo = new Dictionary<string, string>
            {
                { "formId", form.Id.ToString() }
            };
            PdfDocumentInfo info = pdfDocument.GetDocumentInfo();
            info.SetMoreInfo(dicInfo);
        }

        private void AddMultiRowRectangles(string text, string type, int partSize)
        {
            List<string> rows = text.GetRows(partSize);

            foreach (var row in rows)
            {
                ChangePdfRowCounter("+=", 1);
                rectangles.Add(new RectangleParameters(row, pdfRowCounter, pageCounter, type, additionalPadding));

            }
        }

        private void AddChapters()
        {
            chapterCount = form.Chapters.Count;
            int count = 0;
            foreach (FormChapter fc in form.Chapters)
            {
                count++;
                currentChapter = fc;
                ChangeAditionalPadding("+=", 6);
                if (fc.Title != null)
                {
                    ChangePdfRowCounter("+=", 1);
                    AddMultiRowRectangles(fc.Title, "chapters", 87);
                }

                ChangePdfRowCounter("+=", 1);
                AddPages(fc);
                CheckIsLastChapter(count);
                numberOfFieldsInChapter = 0;
                ChangePdfRowCounter("=", 3);
            }
        }

        private void CheckIsLastChapter(int count)
        {
            if (count != chapterCount)
            {
                IncreasePageNumber();
                ChangeAditionalPadding("=", 50);

            }
        }

        private void AddPages(FormChapter chapter)
        {
            ChangeAditionalPadding("+=", 4);
            int i = 0;
            foreach (FormPage fp in chapter.Pages)
            {
                i++;
                if (fp.IsVisible)
                {
                    document.AddParagraph(paragraphParameters, fp.Title.ToUpper(), PagePaddingX, PageFontSize, pageCounter, ref pdfRowCounter, additionalPadding, 0, true);
                    pageLinePositions.Add(pdfRowCounter + "-" + pageCounter + "*" + additionalPadding, pageCounter);
                    document.AddParagraph(paragraphParameters, fp.Description, PagePaddingX, 10, pageCounter, ref pdfRowCounter, additionalPadding + 6, 0, true, true);
                }

                //add form page if exist
                if (fp.ImageMap != null && fp.ImageMap.Url != null)
                {
                    document.AddPageImage(fp.ImageMap.Url.ToString(), pageCounter, pdfDocument.GetDefaultPageSize().GetWidth() - 94, GetY(- Step * pdfRowCounter + additionalPadding), ref additionalPadding);
                    ChangeAditionalPadding("-=", 5);
                }

                ChangePdfRowCounter("+=", 1);

                AddListOfFieldSets(fp.ListOfFieldSets);
                if (i != chapter.Pages.Count)
                {
                    IncreasePageNumber();
                }
                
                ChangeAditionalPadding("=", 0);
                ChangePdfRowCounter("=", 2);
            }
        }

        private void AddListOfFieldSets(List<List<FieldSet>> listOfFieldSets) 
        {
            CreateRepetitionsOfFieldSets(listOfFieldSets);

            foreach (List<FieldSet> list in listOfFieldSets) 
            {
                AddFieldSets(list);
            }
        }

        private void CreateRepetitionsOfFieldSets(List<List<FieldSet>> listOfFieldSets) 
        {
            foreach (List<FieldSet> list in listOfFieldSets)
            {
                if (list[0].IsRepetitive)
                {
                    int numberOfRepetitions = list[0].NumberOfRepetitions > 0 ? list[0].NumberOfRepetitions : 3;

                    for (int i = 0; i < numberOfRepetitions - 1; i++)
                    {
                        list.Add(list[0].Clone());
                    }
                }
            }
        }

        private void AddFieldSetRectangles(List<string> rows)
        {
            int i = 0;
            ChangeAditionalPadding("+=", 10);

            foreach (var row in rows)
            {
                CheckPagePosition();
                rectangles.Add(new RectangleParameters(row, pdfRowCounter, pageCounter, "fieldSets", i.Equals(rows.Count / 2), rows.Count, additionalPadding));
                ChangePdfRowCounter("+=", 1);
                i++;
            }
        }

        private void AddFieldSets(List<FieldSet> fieldSets)
        {
            for (int fieldSetPosition = 0; fieldSetPosition < fieldSets.Count; fieldSetPosition++)
            {
                FieldSet fieldSet = fieldSets[fieldSetPosition];
                if (fieldSet.Label != null)
                {
                    ChangePdfRowCounter("+=", 1);
                    AddFieldSetRectangles(fieldSet.Label.GetRows(100));
                    ChangePdfRowCounter("+=", 1);
                }
                ChangeAditionalPadding("+=", 7);
                AddFormField(fieldSet, fieldSetPosition);
            }
        }

        private void AddFormField(FieldSet fieldSet, int fieldSetPosition)
        {
            int i = 0;
            CheckPagePosition();

            foreach (Field formField in fieldSet.Fields.Where(x => !x.IsHiddenOnPdf))
            {
                AddFormFieldElements(formField, formField.GenerateDependentSuffix(fields), fieldSet.Id, fieldSetPosition);
                if (++i == fieldSet.Fields.Count)
                {
                    ChangeAditionalPadding("+=", 7);
                }
            }
            ChangePdfRowCounter("+=", 1);
        }

        private void CheckPagePosition(int valuesCount = 1, bool addPageInPdf = true)
        {
            if (GetY(- (Step * pdfRowCounter) + additionalPadding)  < 20 * valuesCount + 80 && NewPageOccurred())
            {
                IncreasePageNumber(addPageInPdf);
                ChangePdfRowCounter("=", 2);
                ChangeAditionalPadding("=", 0);
            }
        }

        private bool NewPageOccurred()
        {
            int numOfFieldsInChapterByDefinition = currentChapter.GetNumberOfFieldsForChapter();

            return (numberOfFieldsInChapter != numOfFieldsInChapterByDefinition) || (numberOfFieldsInChapter == numOfFieldsInChapterByDefinition && chapterCount == form.Chapters.Count);
        }

        private void AddField(string keyName, string value)
        {
            string fieldValue = value ?? string.Empty;
            PdfTextFormField field = PdfTextFormField.CreateText(pdfDocument, new Rectangle(FormFieldPadding, GetY(-Step * (pdfRowCounter++) + additionalPadding), 322, 15), keyName, fieldValue);
            field.SetPage(pageCounter);
            field.SetFontSize(9);
            pdfAcroForm.AddField(field);
        }

        private void AddRadioField(FieldRadio formField,string fieldSetId, int fieldSetPosition)
        {
            PdfButtonFormField group = PdfFormField.CreateRadioGroup(pdfDocument, $"{fieldSetId}-{formField.Id}-{pdfRowCounter}-{pageCounter}-{fieldSetPosition}", " ");

            foreach (var radioOption in formField.Values)
            {
                CheckPagePosition(radioOption.Value.Length < 100 ? 1 : radioOption.Value.Length / 100 + 1);
                AddRadioInput(group);

                document.AddParagraphs(AddRadioParagraph(radioOption));

                pdfAcroForm.AddField(group);
                ChangePdfRowCounter("+=", 1);
            }
        }

        private void AddRadioInput(PdfButtonFormField group)
        {
            group.SetRadio(true);
        }

        private List<Paragraph> AddRadioParagraph(FormFieldValue radio)
        {
            int position = pdfRowCounter;
            var rows = radio.Label.GetRows(108);
            ChangePdfRowCounter("+=", rows.Count - 1);
            return GetParagraphsForRadio(rows, position);
        }

        private List<Paragraph> GetParagraphsForRadio(List<string> rows, int position)
        {
            List<Paragraph> paragraphs = new List<Paragraph>();

            int counterOfRows = 0;

            foreach (var row in rows)
            {
                Paragraph radioPara = AddRadioParagraphMultiLine(row, pdfRowCounter.Equals(2) ? 2 : position, counterOfRows);

                counterOfRows++;
                paragraphs.Add(radioPara);
            }

            return paragraphs;
        }

        private Paragraph AddRadioParagraphMultiLine(string row, int position, int counterOfRows)
        {
            Paragraph radioPara = new Paragraph(row);

            radioPara.SetFontSize(CheckAndRadioFontSize);
            radioPara.SetFixedPosition(FormFieldPadding + 20, GetY(-Step * position - counterOfRows * 20 + additionalPadding), 400);
            radioPara.SetPageNumber(pageCounter);
            radioPara.SetFont(font);
            radioPara.SetFontColor(new DeviceRgb(61, 69, 69));

            return radioPara;
        }

        private PdfFormField CreateCheckBoxInput(FieldCheckbox formField, int position, string checkboxOptionId, string fieldSetId, int fieldSetPosition )
        {
            PdfButtonFormField checkField = PdfFormField.CreateCheckBox(pdfDocument, new Rectangle(FormFieldPadding + 250 * position, GetY(-Step * pdfRowCounter + additionalPadding), 15, 15), $"{fieldSetId}-{formField.Id}-{pdfRowCounter}-{position}-{checkboxOptionId}-{pageCounter}-{fieldSetPosition}", "Off", PdfFormField.TYPE_CHECK);
            return checkField;
        }

        private void AddCheckBoxField(FieldCheckbox formField,string fieldSetId, int fieldSetPosition)
        {
            int checkBoxCounter = 0;
            endPosition = pdfRowCounter;
            int numberChekcboxValues = formField.Values.Count;
            int i = 0;

            foreach (var fieldValue in formField.Values.OrderByDescending(x => x.Label.Length).ToList())
            {
                i++;
                int position = checkBoxCounter % 2;
                CheckPagePosition();

                pdfAcroForm.AddField(CreateCheckBoxInput(formField, position, fieldValue.Id, fieldSetId, fieldSetPosition), pdfDocument.GetPage(pageCounter));

                AddCheckBoxPara(fieldValue.Label, position, i == numberChekcboxValues);
                checkBoxCounter++;

            }
            ChangePdfRowCounter("+=", 1);
            CheckPagePosition();
        }

        private void AddCheckBoxPara(string value, int position, bool isLast) 
        {
            int startPage = pageCounter;
            int startAdditionalPadding = additionalPadding;

            List<string> rows = value.GetRows(55);

            foreach (string row in rows) 
            {
                Paragraph paragraph = new Paragraph(row);
                paragraph.SetFixedPosition(FormFieldPadding + 20 + 250 * position, GetY(-Step * pdfRowCounter + additionalPadding), 240);
                paragraph.SetPageNumber(pageCounter);
                paragraph.SetFontSize(CheckAndRadioFontSize);
                paragraph.SetFont(font);

                document.Add(paragraph);
                Flush();
                ChangePdfRowCounter("+=", 1);
                CheckPagePosition(addPageInPdf: position == 0);
            }

            if (position == 0 && !isLast)
            {
                endPosition = pdfRowCounter;

                if (PassOverNextPage(startPage))
                {
                    ChangePdfRowCounter("=", pdfRowCounter);
                    pageCounter--;
                    ChangeAditionalPadding("=", startAdditionalPadding);
                    isPageChanged = true;
                }
                else 
                {
                    ChangePdfRowCounter("-=", rows.Count);
                }
            }
            
            if(position == 1)
            {
                if (isPageChanged)
                {
                    isPageChanged = false;
                    if (startPage == pageCounter)
                    {
                        IncreasePageNumber(addPageInPdf: false);
                        ChangeAditionalPadding("=", 0);
                    }
                }

                ChangePdfRowCounter("=", endPosition);
            }

        }

        private bool PassOverNextPage(int pageCounterBefore)
        {
            return pageCounter != pageCounterBefore;
        }

        private void AddSelectField(FieldSelect formField, string fieldSetId, int fieldSetPosition)
        {
            List<string> availableOptions = formField.Values.Select(x => x.Label).ToList();
            PdfChoiceFormField choiceField = PdfFormField.CreateComboBox(pdfDocument, new Rectangle(FormFieldPadding, GetY(-Step * pdfRowCounter + additionalPadding), 322, 15), $"{fieldSetId}-{formField.Id}-{pdfRowCounter}-{fieldSetPosition}", string.Empty, availableOptions.ToArray());
            choiceField.SetPage(pageCounter);
            choiceField.SetFontSize(9);

            pdfAcroForm.AddField(choiceField);
            ChangePdfRowCounter("+=", 1);
        }

        private void AddFieldLabel(Field formField, string dependencySuffix)
        {
            if(formField is FieldParagraph fieldParagraph)
            {
                html += fieldParagraph.Paragraph;
                return;
            }

            string value = !string.IsNullOrEmpty(formField.Label) ? formField.Label : formField is FieldLink fieldLink ? fieldLink.Link : string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                int currentFieldPosPdf = ++totalFieldCounter;
                value = $"[{currentFieldPosPdf}] {value}";
                
                if (!string.IsNullOrEmpty(dependencySuffix))
                {
                    value = string.Format("{0}, specify If {1} is true", value, dependencySuffix);
                }

                if (formField.Type.Equals(PdfGeneratorType.Date) || formField.Type.Equals(PdfGeneratorType.DateTime))
                {
                    value += " (ex: YYYY-MM-DD)";
                }
            }

            value += formField.IsFieldRepetitive() ? " (Repetititve)" : "";
            value += GetFieldTypeSuffix(formField);

            if (!string.IsNullOrWhiteSpace(value))
            {
                CheckPagePosition();
                document.AddParagraph(paragraphParameters, value, FormFieldPadding, FieldFontSize, pageCounter, ref pdfRowCounter, additionalPadding, 22);
            }
            else
            {
                ChangePdfRowCounter("+=", 1);
            }
        }

        private string GetFieldTypeSuffix(Field field)
        {
            if (field is FieldCalculative)
            {
                return " (Calculative)";
            } 
            else if (field is FieldFile)
            {
                return " (File)";
            }
            else if (field is FieldAudio)
            {
                return " (Audio File)";
            }
            else
            {
                return string.Empty;
            }
        }

        private void AddFormFieldElements(Field formField, string dependencySuffix, string fieldSetId, int fieldSetPosition)
        {
            AddFieldLabel(formField, dependencySuffix);

            int numOfRepetitons = 1;
            if (formField is FieldString fieldString && fieldString.IsRepetitive) 
            {
                numOfRepetitons = fieldString.NumberOfRepetitions > 0 ? fieldString.NumberOfRepetitions : 3;
            }
            for (int i = 0; i < numOfRepetitons; i++)
            {
                AddFieldInput(formField, fieldSetId, fieldSetPosition);
            }
        }

        private void AddFieldInput(Field formField, string fieldSetId, int fieldSetPosition) 
        {
            string keyName = $"{fieldSetId}-{formField.Id}-{pdfRowCounter}-{fieldSetPosition}";
            switch (formField.Type)
            {
                case PdfGeneratorType.Text:
                case PdfGeneratorType.File:
                case PdfGeneratorType.LongText:
                case PdfGeneratorType.Digits:
                case PdfGeneratorType.Url:
                case PdfGeneratorType.Date:
                case PdfGeneratorType.DateTime:
                case PdfGeneratorType.Email:
                    {
                        ChangeAditionalPadding("+=", 7);
                        CheckPagePosition(2);
                        IncrementNumberOfFieldsInChapter();
                        AddField(keyName, formField.GetFirstFieldInstanceValue());
                        ChangeAditionalPadding("-=", 13);
                    }
                    break;
                case PdfGeneratorType.Radio:
                    {
                        ChangeAditionalPadding("+=", 7);
                        //CheckPagePosition(formField.Values.Count != 0 ? formField.Values.Count + 1 : 2);
                        IncrementNumberOfFieldsInChapter();
                        AddRadioField((FieldRadio)formField, fieldSetId, fieldSetPosition);
                        ChangeAditionalPadding("+=", 2);
                        ChangePdfRowCounter("+=", 1);
                    }
                    break;
                case PdfGeneratorType.Number:
                    {
                        ChangeAditionalPadding("+=", 7);
                        CheckPagePosition(2);
                        IncrementNumberOfFieldsInChapter();
                        AddField($"{fieldSetId}-{formField.Id}-{pdfRowCounter}-number-{fieldSetPosition}", formField.GetFirstFieldInstanceValue());
                        ChangeAditionalPadding("+=", 4);
                        ChangePdfRowCounter("+=", 1);
                    }
                    break;
                case PdfGeneratorType.Checkbox:
                    {
                        ChangeAditionalPadding("+=", 7);
                        IncrementNumberOfFieldsInChapter();
                        AddCheckBoxField((FieldCheckbox)formField, fieldSetId, fieldSetPosition);
                        ChangeAditionalPadding("+=", 22);
                        ChangePdfRowCounter("+=", 1);
                    }
                    break;
                case PdfGeneratorType.Select:
                    {
                        ChangeAditionalPadding("+=", 7);
                        CheckPagePosition();
                        IncrementNumberOfFieldsInChapter();
                        AddSelectField((FieldSelect)formField, fieldSetId,fieldSetPosition);
                        ChangeAditionalPadding("-=", 13);
                    }
                    break;
                case PdfGeneratorType.Link:
                case PdfGeneratorType.Paragraph:
                    break;
                default:
                    {
                        ChangeAditionalPadding("+=", 7);
                        CheckPagePosition(formField is FieldSelectable fieldSelectable && fieldSelectable.Values.Count != 0 ? fieldSelectable.Values.Count + 1 : 2);
                        AddField(keyName, formField.GetFirstFieldInstanceValue());
                        ChangeAditionalPadding("-=", 13);
                    }
                    break;
            }
        }

        private int GetY(int customCalculation)
        {
            return PageHeight + customCalculation - PageMargin;
        }

        private void ChangePdfRowCounter(string action, int value)
        {
            ChangePropertyCounter(ref pdfRowCounter, action, value);
        }

        private void ChangeAditionalPadding(string action, int value)
        {
            ChangePropertyCounter(ref additionalPadding, action, value);
        }

        private void ChangePropertyCounter(ref int property, string action, int value)
        {
            switch (action)
            {
                case "+=": property += value; break;
                case "-=": property -= value; break;
                case "=": property = value; break;
                default: break;
            }
        }

        private void IncrementNumberOfFieldsInChapter()
        {
            numberOfFieldsInChapter++;
        }

        private void IncreasePageNumber(bool addPageInPdf = true)
        {
            if (addPageInPdf)
            {
                pdfDocument.AddNewPage();
            }
            pageCounter++;
        }
    }
    
}
