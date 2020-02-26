using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ExchangeAdvisor.ML.SourceGenerator
{
    public class FileSize
    {
        public FileSize(string filePath)
        {
            SizeInBytes = new FileInfo(filePath).Length;
        }

        public long SizeInBytes { get; private set; }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder(100);
            StrFormatByteSize(SizeInBytes, stringBuilder, stringBuilder.Capacity);

            return stringBuilder.ToString();
        }

        [DllImport("Shlwapi.dll", CharSet = CharSet.Auto)]
        [SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "Auto-generated code")]
        private static extern long StrFormatByteSize(
            long fileSize,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer,
            int bufferSize);
    }
}
