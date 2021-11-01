using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    enum StatusTask { Assigned, InWork, OnCheking, Completed}

    class Task
    {
        private string description;
        private DateTime deadline;
        private StatusTask status;
        private string initiator;
        private string executor;
        private Project project;
        private bool isOneTimeTask;
        private Report report;
        private List<Report> reports;
        private string path;

        public Task(string description, DateTime deadline, StatusTask status, string initiator, string executor, Project project, bool isOneTimeTask, string path)
        {
            this.description = description;
            this.deadline = deadline;
            this.status = status;
            this.initiator = initiator;
            this.executor = executor;
            this.project = project;
            this.isOneTimeTask = isOneTimeTask;
            this.path = path;
        }

        public Task(string description,
                    TypeDeadline deadLine, 
                    double period,
                    StatusTask status,
                    Project project,
                    string initiator,
                    string executor,
                    bool isOneTimeTask,
                    string path)
        {
            this.description = description;
            this.isOneTimeTask = isOneTimeTask;
            this.project = project;
            DateTime deadline = DateTime.Now;
            switch ((int)deadLine)
            {
                case 1:
                    deadline = deadline.AddDays(period * 31);
                    break;
                case 2:
                    deadline = deadline.AddDays(period * 7);
                    break;
                case 3:
                    deadline=deadline.AddDays(period);
                    break;
                default:
                    deadline = deadline.AddDays(period * 365);
                    break;
            }
            this.deadline = deadline;
            this.status = status;
            this.initiator = initiator;
            this.executor = executor;
            this.path = path;
        }

        public string Path => path;
        public string Description  => description;
        public bool IsOneTimeTask  => isOneTimeTask; 
        public DateTime DeadLine  => deadline;

        public string Initiator  => initiator;
        public string Executor => executor;

        internal Report Report  => report; 
        internal List<Report> Reports  => reports;
        internal StatusTask Status => status;
        internal Project Project => project; 
    }
}
