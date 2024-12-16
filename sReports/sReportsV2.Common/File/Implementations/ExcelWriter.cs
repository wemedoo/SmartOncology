using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using sReportsV2.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.File.Interfaces;

namespace sReportsV2.Common.File
{
    public class ExcelWriter : FileWriter
    {
        private int rowIndex;
        private SpreadsheetDocument workbook;
        private WorksheetPart workSheetPart;
        private OpenXmlWriter writer;

        public ExcelWriter(Stream stream)
        {
            rowIndex = 1;
            Initialize(stream);
        }

        private void Initialize(Stream stream)
        {
            workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

            workbook.AddWorkbookPart();
            workSheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();

            writer = OpenXmlWriter.Create(workSheetPart);
            writer.WriteStartElement(new Worksheet());
            writer.WriteStartElement(new SheetData());
        }

        private void IncrementRowIndex() 
        {
            rowIndex++;
        }

        public void WriteRow(IEnumerable<string> cells)
        {
            List <OpenXmlAttribute> attributeList = new List<OpenXmlAttribute>();
            attributeList.Add(new OpenXmlAttribute("r", null, rowIndex.ToString()));
            writer.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Row(), attributeList);
            foreach (string cell in cells)
            {
                attributeList = new List<OpenXmlAttribute>();
                attributeList.Add(new OpenXmlAttribute("t", null, "str"));

                writer.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Cell(), attributeList);
                writer.WriteElement(new CellValue(cell));
                writer.WriteEndElement();  // Finalizing cell writing
            }
            writer.WriteEndElement();  // Finalizing row writing

            IncrementRowIndex();
        }

        public void FinalizeWriting(Stream stream)
        {
            writer.WriteEndElement();  // Finalizing SpreadShit writing
            writer.WriteEndElement();  // Finalizing Worksheet writing
            writer.Close();

            writer = OpenXmlWriter.Create(workbook.WorkbookPart);
            writer.WriteStartElement(new DocumentFormat.OpenXml.Spreadsheet.Workbook());
            writer.WriteStartElement(new Sheets());

            writer.WriteElement(new Sheet()
            {
                Name = "Sheet1",
                SheetId = 1,
                Id = workbook.WorkbookPart.GetIdOfPart(workSheetPart)
            });

            writer.WriteEndElement(); // Write end for WorkSheet Element
            writer.WriteEndElement(); // Write end for WorkBook Element
            writer.Close();

            workbook.Close();

            writer.Dispose();
            workbook.Dispose();

            stream.Flush();
            stream.Seek(0, SeekOrigin.Begin); // Rewind stream for reading
        }

    }
}
