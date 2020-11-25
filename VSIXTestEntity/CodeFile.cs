using System;
using System.Collections.Generic;
using System.Text;

namespace VSIXTestEntity
{
    public class CodeFile
    {
        public string Path { get; internal set; }

        public string GeneratedCode { get; internal set; }

        public bool Succesfull { get; internal set; }

    }
}
