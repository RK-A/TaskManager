using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    class Report
    {
        private string text;
        private DateTime data;
        private string executor;

        public Report(string text, string executor)
        {
            this.text = text;
            data = DateTime.Now;
            this.executor = executor;
        }
        public string Text => text; 
        public DateTime Data => data; 
        public string Executor => executor;
    }
}
