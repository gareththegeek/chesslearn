using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chess.Featuriser.Fen
{
    public class CsvScanner
    {
        public IEnumerable<string> Scan(Stream stream)
        {
            var result = new List<string>();
            using (var reader = new StreamReader(stream))
            {
                reader.ReadLine(); // Discard headings
                while (!reader.EndOfStream)
                {
                    result.Add(reader.ReadLine().Split(',').First());
                }
                return result;
            }
        }
    }
}
