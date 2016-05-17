using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UpdateLib
{
    public class UpdateProgress
    {
        public List<string> DownloadedFiles;

        public UpdateProgress() {
            DownloadedFiles = new List<string>();
        }
    }
}
