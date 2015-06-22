using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoLSkinChecker
{
    public class ProgressEventArgs : EventArgs
    {
        public int Progress { get; set; }
        public string progressString { get; set; }
        public ProgressEventArgs(int progress,string str)
        {
            Progress = progress;
            progressString = str;
        }


    }
}
