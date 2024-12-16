using System.Collections.Generic;
using System.IO;

namespace sReportsV2.Common.File
{
    public static class CsvReader
    {
        public static IEnumerable<IEnumerable<string>> ReadBatches(Stream inputStream, int batchSize)
        {
            List<string> batchItems = new List<string>();

            using (StreamReader csvreader = new StreamReader(inputStream))
            {
                while (!csvreader.EndOfStream)
                {
                    batchItems.Clear();

                    for (int i = 0; i < batchSize; i++)
                    {
                        if (csvreader.EndOfStream)
                            break;

                        batchItems.Add(csvreader.ReadLine());
                    }

                    yield return batchItems;
                }
            }
        }

        public static IEnumerable<IEnumerable<string>> ReadBatches(string fileName, int batchSize)
        {
            List<string> batchItems = new List<string>();

            using (StreamReader file = System.IO.File.OpenText(fileName))
            {
                while (!file.EndOfStream)
                {
                    batchItems.Clear();

                    for (int i = 0; i < batchSize; i++)
                    {
                        if (file.EndOfStream)
                            break;

                        batchItems.Add(file.ReadLine());
                    }

                    yield return batchItems;
                }
            }

        }
    }
}
