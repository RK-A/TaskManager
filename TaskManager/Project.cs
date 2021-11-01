using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    enum StatusProject {Project, Execution, Closed }
    enum TypeDeadline {D,W,M,Y }

    class Project
    {
        private string description;
        private string client;
        private StatusProject status;
        private string mainUser;
        private DateTime deadline;
        private List<Task> tasks;

        public string Description  => description;
        public string Client => client;
        internal StatusProject Status  => status; 
        public DateTime Deadline  => deadline; 
        internal List<Task> Tasks => tasks;
        public string MainUser => mainUser;

        public Project(string description, string client,string mainUser, StatusProject status, TypeDeadline deadLine,double period, string name = "")
        {
            this.description = description;
            this.client = client;
            this.mainUser = mainUser;
            this.status = status;
            DateTime deadline = DateTime.Now;
            switch ((int)deadLine)
            {
                case 1:
                    deadline = deadline.AddDays(period*31);
                    break;
                case 2:
                    deadline = deadline.AddDays(period * 7);
                    break;
                case 3:
                    deadline = deadline.AddDays(period);
                    break;
                default:
                    deadline = deadline.AddDays(period*365);
                    break;
            }
            this.deadline = deadline;
            if (name!="")
            {
                
                Directory.CreateDirectory($@"resource\{name}");
                File.WriteAllText($@"resource\{name}\main.txt", string.Join(" ", "(", description, ")", client, mainUser, (int)status, period, (int)deadLine)+"\r\nЗадачи:");
            }
            tasks = new List<Task>();
        }
        public bool TryAddTasks(Task task,string user) 
        {
            if (status==0 && user.Equals(mainUser))
            {
                tasks.Add(task);
                File.WriteAllText($@"resource\{task.Path}\main.txt", File.ReadAllText($@"resource\{task.Path}\main.txt") + "\r\n" + string.Join(" ", task.Executor,task.Initiator,task.IsOneTimeTask,(int)task.Status,"(",task.DeadLine.Date,")","(",task.Description));
                return true;
            }
            else
            {
                return false;
            }
        }
        public void AddTask(Task task)
        {
            tasks.Add(task);
        }
        public void StartProject()
        {
            if ((int)status<1)
            {
                status += 1;
            }
            else if((int)status==2)
            {
                Console.WriteLine("Проект уже закончен");
            }
            else
            {
                Console.WriteLine("Проект уже в работе");
            }
        }
        public void CloseProject()
        {
            if (tasks.Count==0)
            {
                status = StatusProject.Closed;
            }
            else
            {
                Console.WriteLine("Нельзя закрыть проект т.к он еще не начат или не все задания выполнены");
            }
        }
       
    }
}
