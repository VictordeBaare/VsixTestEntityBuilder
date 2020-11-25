using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXTestEntity
{
    public class BaseDialogWindow : DialogWindow
    {
        public BaseDialogWindow()
        {
            MaxHeight = 160;
            MaxWidth = 300;
        }
    }
}
