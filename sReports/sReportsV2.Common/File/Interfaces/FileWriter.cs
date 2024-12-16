using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Common.File.Interfaces
{
    public interface FileWriter
    {
        void WriteRow(IEnumerable<string> cells);
        void FinalizeWriting(Stream stream);
    }
}
