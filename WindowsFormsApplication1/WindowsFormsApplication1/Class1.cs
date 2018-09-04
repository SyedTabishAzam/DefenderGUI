using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
namespace WindowsFormsApplication1
{
    public static class StaticClass
    {
        public static Entity SelectedEntity { get; set; }
        public static Process Publisher { get; set; }

        public static Process Subscriber { get; set; }
    }
}
