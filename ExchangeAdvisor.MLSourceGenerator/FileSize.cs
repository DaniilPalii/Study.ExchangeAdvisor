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
        private static extern long StrFormatByteSize(
            long fileSize,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder buffer,
            int bufferSize);
    }
}
