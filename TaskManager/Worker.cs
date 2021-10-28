using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    enum Post {Teamlead, Developer }

    class Worker
    {
        private string name;
        private Post post;
        private List<Task> tasks;
        private Project project;
        private string nameProject;

        public Worker(string name, Post post)
        {
            this.name = name;
            this.post = post;
        }
        public Worker(string name, Post post, string nameProject) : this(name, post)
        {
            this.nameProject = nameProject;
        }

        public string Name => name;
        public string NameProject => nameProject;

        internal Post Post => post; 
        internal List<Task> Tasks => tasks;
        public void AddTask(Task task)
        {
            if (project==null)
            {
                tasks.Add(task);
                AttachedProject(task.Project);
            }
            else if(task.Project.Equals(project))
            {
                tasks.Add(task);
            }
        }
        public void AttachedProject(Project project) => this.project = project;
    }
}
