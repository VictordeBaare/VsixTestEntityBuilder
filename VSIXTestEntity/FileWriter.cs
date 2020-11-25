using System.IO;
using System.Text;

namespace VSIXTestEntity
{
    public class FileWriter
    {
        public bool WriteOutputToFile(CodeFile codeFile)
        {
            if (File.Exists(codeFile.Path))
            {
                if (!FileComparer.IsFileContentEqual(codeFile.Path, codeFile.GeneratedCode))
                {
                    File.WriteAllText(codeFile.Path, codeFile.GeneratedCode, Encoding.UTF8);
                }
                return false;
            }
            else
            {
                File.WriteAllText(codeFile.Path, codeFile.GeneratedCode);
                return true;
            }
        }
    }
}
