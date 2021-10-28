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
        private Worker initiator;
        private Worker executor;
        private Project project;
        private bool isOneTimeTask;
        private Report report;
        private List<Report> reports;

        public Task(string description,
                    TypeDeadline deadLine, 
                    double period,
                    StatusTask status,
                    Project project,
                    Worker initiator,
                    Worker executor,
                    bool isOneTimeTask,
                    Report report,
                    List<Report> reports)
        {
            this.description = description;
            this.isOneTimeTask = isOneTimeTask;
            this.report = report;
            this.project = project;
            DateTime deadline = DateTime.Now;
            switch ((int)deadLine)
            {
                case 1:
                    deadline.AddDays(period * 31);
                    break;
                case 2:
                    deadline.AddDays(period * 7);
                    break;
                case 3:
                    deadline.AddDays(period);
                    break;
                default:
                    deadline.AddDays(period * 365);
                    break;
            }
            this.deadline = deadline;
            this.status = status;
            this.initiator = initiator;
            this.executor = executor;
            this.reports = reports;
        }

        public string Description  => description;
        public bool IsOneTimeTask  => isOneTimeTask; 
        public DateTime DeadLine  => deadline;

        public Worker Initiator  => initiator;
        public Worker Executor => executor;

        internal Report Report  => report; 
        internal List<Report> Reports  => reports;
        internal StatusTask Status => status;
        internal Project Project => project; 
    }
}
