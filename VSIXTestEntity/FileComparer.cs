using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace VSIXTestEntity
{
    public static class FileComparer
    {
        public static bool IsFilePathContentEqueal(string filePath1, string filePath2)
        {
            if (string.Equals(filePath1, filePath2))
            {
                return true;
            }

            using (FileStream fileStream1 = new FileStream(filePath1, FileMode.Open, FileAccess.Read))
            using (FileStream fileStream2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read))
            {
                if (!fileStream1.Length.Equals(fileStream2.Length))
                {
                    return false;
                }

                int file1Byte;
                int file2Byte;
                do
                {
                    file1Byte = fileStream1.ReadByte();
                    file2Byte = fileStream2.ReadByte();
                } while ((file1Byte == file2Byte) && file1Byte != -1);

                return file1Byte == file2Byte;
            }
        }

        public static bool IsFileContentEqual(string filepath, string fileContent)
        {
            var fileContentOfFilePath = File.ReadAllText(filepath);

            return string.CompareOrdinal(fileContentOfFilePath, fileContent) == 0;
        }
    }
}
