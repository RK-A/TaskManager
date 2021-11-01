using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TaskManager
{
    
    class Program
    {
        private const string  PATH_WORKER = @"resource\Employee.txt";
        private const string PATH_REQ_PROJ = @"resource\request_projects.txt";
        private const string PATH_TASKS = @"resource\req_tasks.txt";
        static void Main(string[] args)
        {
            List<Worker> workers= new List<Worker>();
            using (StreamReader strRead = new StreamReader(PATH_WORKER))
            {
                string line;
                while ((line=strRead.ReadLine())!=null)
                {
                    string[] lines = line.ToLower().Split();
                    if (lines.Length==3)
                    {
                        workers.Add(new Worker(lines[0], lines[1].Equals("tl") ? 0 : (Post)1,lines[2]));
                    }
                    else
                    {

                        workers.Add(new Worker(lines[0], lines[1].Equals("tl") ? 0 : (Post)1));
                    }
                }
            }

            Post rank;//
            string proj;//
            string user=" ";
            user = ChangeUser(workers, out rank, out proj);
            while (!user.Equals("anonim"))
            {
                Console.Write("Вы авторизовались как ");
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write($"{user}\n");
                Console.ResetColor();
                Console.WriteLine("Вам доступны следующие команды");
                if (user.Equals("Заказчик"))
                {
                    
                    UserCustomer();
                }
                else if (rank == 0)
                {
                    UserTL(proj,user);
                }
                else
                {
                    UserDev(proj,user);
                }
                user = ChangeUser(workers, out rank, out proj);
            }


        }
        static string ChangeUser(List<Worker> workers, out Post rank, out string proj)
        {
            bool flag = true;
            string user = "anonim";
            Worker worker = null;
            rank = 0;//
            proj = null;//
            while (flag)
            {
                Console.WriteLine("Введите за кого хотите войти ");
                string input = Console.ReadLine();
                worker = workers.Find(x => x.Name.ToLower().Equals(input.ToLower()));
                if (worker != null)
                {
                    user = worker.Name;   
                    rank = worker.Post;   //
                    proj = worker.NameProject;//
                    flag = false;
                }
                if (input.ToLower().Equals("заказчик"))
                {
                    user = "Заказчик";
                    flag = false;
                }
                if (input.ToLower().Equals("отмена")|| input.ToLower().Equals("выйти"))
                {
                    flag = false;
                }
            }
            proj = (proj != null ?proj:"");
            return user;
        }
        static void UserCustomer()
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Введите свое имя");
            Console.ResetColor();
            string name = Console.ReadLine();
            
            bool flag = true;
            while (flag)
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine(/*"\"Посмотреть\" чтобы посмотреть проекты\n" +*/
                    "\"Создать@{название проекта}@{Содержание проекта}@{число(месяцев,дней...)}@{Д,Н,М,Г}\" - добавить проект в предложку\n " +
                    "ОТМЕНА или ВЫЙТИ чтоб выйти" /*+
                    "\"Выбрать {название проекта}\" открыть проект и посмотреть отчеты к нему\n" +
                    "\"Посмотреть №\" - чтобы посмотреть отчет под номером к проекту"*/);
                Console.ResetColor();
                List<string> input = Console.ReadLine().ToLower().Split('@').ToList();
                List<string> chars = new List<string> { "д", "н", "м", "г" };
                int prov;
                if (input.Count == 5 && input[0].Equals("создать") && int.TryParse(input[3], out prov)&& chars.IndexOf(input[4])!=-1)
                {
                    input[0] = "\r\n";
                    input[2] = "(" + input[2] +")";
                    string text = File.ReadAllText(PATH_REQ_PROJ);
                    File.WriteAllText(PATH_REQ_PROJ, string.Concat(text, string.Join(" ", input).Trim()," ",name));
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("Ваш проект в рассмотрении");
                    Console.ResetColor();
                    
                }
                else if (input[0].Equals("отмена")|| input[0].Equals("выйти"))
                {
                    flag = false;
                }
                else Console.WriteLine("Вы ввели не правильно");
            }
        }
        static void UserTL(string proj,string adminName)
        {
            bool flag = true;
            Project project = null;
            while (flag)
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                if (proj=="")
                {
                    Console.WriteLine("\"Посмотреть\" чтобы посмотреть проекты в предложке\n" +
                    "\"Выбрать №\" выбрать проект из предложки\n" +
                    "ОТМЕНА чтоб выйти");
                    Console.ResetColor();
                    string[] input = Console.ReadLine().ToLower().Split();
                    switch (input[0])
                    {
                        case "выбрать":
                            int num;
                            if (input.Length != 2 || !int.TryParse(input[1],out num))break ;
                            List<string> text = new List<string>();
                            using (StreamReader strRead = new StreamReader(PATH_REQ_PROJ))
                            {
                                string line;
                                int c = 0;
                                while ((line= strRead.ReadLine())!=null)
                                {
                                    string linestr = line.Split(')')[0].Split('(')[1];
                                    string[] lines = line.Replace(" ("+linestr +") "," ").Split();
                                    c++;
                                    if (c==num)
                                    {
                                        double period;
                                        if (!double.TryParse(lines[1],out period))
                                        {
                                            period = 1;
                                        }
                                        List<string> chars = new List<string> { "г", "м", "н", "д" };
                                        project = new Project(linestr,lines[3],adminName,StatusProject.Project,(TypeDeadline)chars.IndexOf(lines[2]),period,lines[0]);
                                        proj = lines[0];
                                        AddProjEmployee(adminName,lines[0]);
                                        Console.WriteLine("Вы начали проект");
                                    }
                                    else
                                    {
                                        text.Add(line);
                                    }
                                }
                                if (project==null)
                                {
                                    Console.WriteLine("Не правильно указали проект");
                                }
                            }
                            File.WriteAllText(PATH_REQ_PROJ, string.Join("\r\n", text));
                            break;
                        case "отмена":
                            flag = false;
                            break;
                        case "выйти":
                            flag = false;
                            break;
                        case "посмотреть":
                            using (StreamReader strRead = new StreamReader(PATH_REQ_PROJ))
                            {
                                string line;
                                while ((line = strRead.ReadLine()) != null)
                                {
                                    Console.WriteLine(line);
                                }
                            }
                            break;
                        default:
                            Console.WriteLine("Вы ничего не выбрали");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("\"Открыть\" проект и посмотреть отчеты к нему\n" +
                    "\"Выдать@текст задания@кому@тип(1 если много отчетная 0 если разовая)@{число(месяцев,дней...)}@{Д,Н,М,Г}) \" - предложить задание \n" +
                    "\"Проверить №\"-подтвердить выполнение задачи №\n" +
                    "\"Закрыть\"-проект\n" +
                    "\"Начать\"-проект в стадию исполнения");
                    Console.ResetColor();
                    if (project==null)
                    {
                        flag = InitialProject(out project,proj,adminName);
                    }
                    if (flag)
                    { 
                        string[] input = Console.ReadLine().ToLower().Split('@');
                        switch (input[0])
                        {
                            case "открыть":
                                CheckTask(proj);
                                break;
                            case "выдать":
                                int stat;
                                int period;
                                List<string> chars = new List<string> { "д", "н", "м", "г" };
                                if (int.TryParse(input[4], out period)
                                    && int.TryParse(input[3], out stat)
                                    && chars.IndexOf(input[5]) != -1
                                    && project.TryAddTasks(new Task(input[1], (TypeDeadline)chars.IndexOf(input[5]), period, 0, project, adminName, input[2], stat == 1 ? false : true, proj), adminName))
                                {
                                    Console.WriteLine("Вы запросили задачу");
                                    File.WriteAllText(PATH_TASKS, File.ReadAllText(PATH_TASKS) +"\r\n"+string.Join(" ",input[2],adminName,stat,period,input[5],proj,"("+input[1]+")"));
                                    //выдать@написать парсер@андрей@0@2@н
                                }
                                else
                                {
                                    Console.WriteLine("Неправильный ввод");
                                }
                                break;
                            case "проверить":
                                int num;
                                if (input.Length==1)
                                {
                                    CheckOrders(proj);
                                }
                                else if(int.TryParse(input[1],out num))
                                {
                                    AllowedTask(proj,num);
                                }
                                break;
                            case "закрыть":
                                project.CloseProject();
                                break;
                            case "начать":
                                project.StartProject();
                                break;
                            case "отмена":
                                flag = false;
                                break;
                            case "выйти":
                                flag = false;
                                break;
                            default:
                                Console.WriteLine("Вы ввели неправильно");
                                break;
                        }
                    }
                }


            }
        }
        static void UserDev(string proj,string name)
        {
            bool flag = true;
            while (flag)
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                if (proj == "" || !Directory.Exists($@"resource\{proj}"))
                {
                    Console.WriteLine("\"Посмотреть\" чтобы посмотреть задания в предложке\n" +
                        "\"Задание № {брать,делегировать,отклонить}\"");
                }
                else
                {
                    Console.WriteLine("\"Посмотреть\" чтобы посмотреть задачи\n" +
                        "\"Выполнить № %\" сделать задачу и отчет к ней (если задача разовая и % выполнения достиг 100)\n" /*+
                        "\"Сделать \" - создать отчет для  "*/);
                }
                Console.ResetColor();
                string[] input = Console.ReadLine().ToLower().Split();
                switch (input[0])
                {
                    case "посмотреть":
                        CheckTasks(proj,name);
                        break;
                    case "задание":
                        int num;
                        if (input.Length==3&&int.TryParse(input[1],out num)&& (input[2].Equals("брать") || input[2].Equals("делегировать") || input[2].Equals("отклонить") ))
                        {
                            TakeTask(name,num,input[2]);
                        }
                        break;
                    case "выполнить":
                        if (proj!=""&&int.TryParse(input[1],out num))
                        {
                            CompleteTask(name, num,proj);
                        }
                        break;
                    default:
                        break;
                }

            }
        }
        static void AddProjEmployee(string name, string proj)
        {
            List<string> workers = new List<string>();
            using (StreamReader strRead = new StreamReader(PATH_WORKER))
            {
                string line;
                while ((line= strRead.ReadLine())!=null)
                {
                    if (line.Split()[0].ToLower().Equals(name.ToLower()))
                    {
                        workers.Add(line+" "+proj);
                    }
                    else
                    {
                        workers.Add(line);
                    }
                }
            }
            File.WriteAllText(PATH_WORKER,string.Join("\r\n",workers));
        }
        static bool InitialProject(out Project project,string proj,string adminName)
        {
            using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
            {
                project = null;
                string line = strRead.ReadLine();
                string decription = line.Split(')')[0].Split('(')[1];
                string[] lines = line.Replace("(" + decription + ") ", "").Split();
                double period;
                int status, deadline;
                if (double.TryParse(lines[3], out period) && int.TryParse(lines[2], out status) && int.TryParse(lines[4], out deadline))
                {
                    project = new Project(decription, lines[0], adminName, (StatusProject)status, (TypeDeadline)deadline, period);

                }
                if (strRead.ReadLine().ToLower().Equals("задачи:") && project != null)
                {
                    while ((line = strRead.ReadLine()) != null)
                    {
                        decription = line.Split('(')[2];
                        DateTime time = DateTime.Parse(line.Split('(')[1].Split(')')[0]);
                        lines = line.Replace(" (" + decription, "").Split();
                        int stat;
                        if (int.TryParse(lines[3], out stat))
                        {
                            project.AddTask(new Task(decription, time, (StatusTask)stat, lines[1], lines[0], project, lines[2].ToLower().Equals("true") ? true : false, proj));
                        }

                    }
                }
                else
                {
                    Console.WriteLine("Проект сломан");
                    return false;
                }
                return true;

            }
        }
        static void CheckTask(string proj)
        {
            Console.WriteLine("Исполнитель инициатор одноразовая статус срок");
            using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
            {
                string line;
                while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) ;
                while ((line = strRead.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
        static void AllowedTask(string proj,int num)
        {
            Console.WriteLine("Исполнитель инициатор срок");
            List<string> lines = new List<string>();
            using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
            {
                int count = 0;
                string line;
                while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) lines.Add(line);
                lines.Add(line);
                while ((line = strRead.ReadLine()) != null)
                {
                    count++;
                    if (count!=num)
                    {
                        lines.Add(line);
                    }
                }
            }
            File.WriteAllText($@"resource\{proj}\main.txt",string.Join("\r\n",lines));

        }
        static void CheckOrders(string proj)
        {
            string[] orders = Directory.GetFiles($@"resource\{proj}");
            foreach (var order in orders)
            {
                if (order.Split('\\')[order.Split('\\').Length-1].Split('.')[0]!="main")
                {
                    Console.WriteLine(order.Split('\\')[order.Length - 1].Split('.')[0]+File.ReadAllText(order));
                }
            }
        }

        static void CheckTasks(string proj,string name)
        {
            if (proj=="")
            {
                using (StreamReader strRead= new StreamReader(PATH_TASKS))
                {
                    string line;
                    while ((line=strRead.ReadLine())!=null)
                    {
                        if (line.Split()[0].ToLower().Equals(name.ToLower()))
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }
            else
            {
                using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
                {
                    string line;
                    while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) ;
                    while ((line = strRead.ReadLine()) != null)
                    {
                        if (line.Split()[0].ToLower().Equals(name.ToLower()))
                        {
                            Console.WriteLine(line);
                        }
                    }
                }
            }
        }
        static void TakeTask(string name,int num, string command)
        {
            
        }
        static void CompleteTask(string name,int num,string proj)
        {
            List<string> lines = new List<string>();
            using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
            {
                string line;
                int count = 0;
                while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) lines.Add(line);
                lines.Add(line);
                while ((line = strRead.ReadLine()) != null)
                {
                    if (line.Split()[0].ToLower().Equals(name.ToLower()))
                    {
                        count++;
                        if (count == num)
                        {
                            AddProjEmployee(name, line);
                        }
                    }
                    else
                    {
                        lines.Add(line);
                    }

                }
            }
            File.WriteAllText(PATH_TASKS, string.Join("\r\n", lines));
        }
    }
}
