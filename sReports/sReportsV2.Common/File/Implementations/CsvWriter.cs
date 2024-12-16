using sReportsV2.Common.Extensions;
using sReportsV2.Common.File.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace sReportsV2.Common.File.Implementations
{
    public class CsvWriter : FileWriter
    {
        private readonly StreamWriter writer;

        public CsvWriter(Stream stream)
        {
            writer = new StreamWriter(stream);
        }

        public void WriteRow(IEnumerable<string> cells)
        {
            cells = cells.Where(field => field != null).Select(field => field.ToString().SanitizeForCsvExport());
            writer.WriteLine(string.Join(",", cells));
        }

        public void FinalizeWriting(Stream stream)
        {
            // Note: not calling writer.Dispose() because it's closing the Stream too -> Error when reading the Stream to the client
            writer.Flush();
            stream.Seek(0, SeekOrigin.Begin); // Rewind stream for reading            
        }
    }
}
