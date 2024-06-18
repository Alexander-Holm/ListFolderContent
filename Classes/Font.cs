using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ListFolderContent.Classes
{
    public class Font
    {
        public string Name { get; set; }
        public bool IsInstalled { get; set; }

        public Font(string name, bool isInstalled)
        {
            Name = name;
            IsInstalled = isInstalled;
        }
    }
}
