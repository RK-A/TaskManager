using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    class Report
    {
        private string text;
        private string executor;

        public Report(string text, string executor,string proj,string title)
        {
            this.text = text;
            this.executor = executor;
            File.WriteAllText($@"resource\{proj}\{executor}_{title}.txt",text);
        }
        public string Text => text; 
        public string Executor => executor;
    }
}
