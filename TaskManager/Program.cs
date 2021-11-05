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
        private const string PATH_REJ_TASKS = @"resource\rej_tasks.txt";
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
                Console.WriteLine("заказчик, имя разработчика или имя team leadera");
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
                Console.BackgroundColor = ConsoleColor.DarkGreen;
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
                Console.BackgroundColor = ConsoleColor.DarkGreen;
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
                                int count = 0;
                                string line;
                                while ((line = strRead.ReadLine()) != null)
                                {
                                    Console.WriteLine(line);
                                    count++;
                                }
                                if (count==0)
                                {
                                    Console.WriteLine("Проектов нет");
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
                    Console.WriteLine("\"Открыть\" посмотреть отчеты\n" +
                    "\"Выдать@текст задания@кому@тип только 0@{число(месяцев,дней...)}@{Д,Н,М,Г})\"-предложить задание \n" +
                    "\"Проверить@№{Посмотреть}\"-подтвердить выполнение задачи № \n" +
                    "\"Вернуть@№\" выполненое задание\n" +
                    "\"отклоненные\"-задания\n" +
                    "\"перевыдать@№@кому\" - задание\n"+
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
                                CheckOrders(proj);
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
                                    CheckTask(proj);
                                }
                                else if(input.Length == 2&&int.TryParse(input[1],out num))
                                {
                                    AllowedTask(proj,num);
                                }
                                break;
                            case "вернуть":
                                if (input.Length == 2 && int.TryParse(input[1], out num))
                                {
                                    RejectTask(proj,num);
                                }
                                break;
                            case "закрыть":
                                project.CloseProject(proj);
                                break;
                            case "начать":
                                project.StartProject(proj);
                                break;
                            case "отмена":
                                flag = false;
                                break;
                            case "выйти":
                                flag = false;
                                break;
                            case "отклоненные":
                                ViewRejTask(proj);
                                break;
                            case "перевыдать":
                                if (input.Length==3 && int.TryParse(input[1] ,out num))
                                {
                                    ReGiveTask(num,input[2],adminName);
                                    Console.WriteLine("Вы передали задание");
                                }
                                else
                                {
                                    Console.WriteLine("Неправильный ввод");
                                }
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
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                if (proj == "" || !Directory.Exists($@"resource\{proj}"))
                {
                    Console.WriteLine("\"Посмотреть\" чтобы посмотреть задания в предложке\n" +
                        "\"Задание № {взять,делегировать,отклонить} {кому если делегировать}\"");
                }
                else
                {
                    Console.WriteLine("\"Посмотреть\" чтобы посмотреть задачи\n" +
                        "\"Выполнить №\" сделать задачу и отчет к ней \n" +
                        "\"Задание № {взять,делегировать,отклонить} {кому если делегировать}\"");
                }
                Console.ResetColor();
                string[] input = Console.ReadLine().ToLower().Split();
                switch (input[0])
                {
                    case "посмотреть":
                        CheckTasks(proj,name);
                        break;
                    case "задание":
                        int num=0;
                        if (input.Length == 3 && int.TryParse(input[1], out num) && (input[2].Equals("взять")   ))
                        {
                            if (proj=="")
                            {
                                proj = TakeTask(name, num);
                            }
                            else
                            {
                                TakeTask(name, num, proj);
                            }
                            Console.WriteLine("Вы взяли задачу");
                        }
                        else if (input.Length == 4 && int.TryParse(input[1], out num) && input[2].Equals("делегировать"))
                        {
                            DelegateTask(name,input[3],num,proj);
                            Console.WriteLine("Вы делегировали задачу");
                        }
                        else if (input.Length == 3 && int.TryParse(input[1], out num) && input[2].Equals("отклонить"))
                        {
                            RejectTask(name,num,proj);
                        }
                        else
                        {
                            Console.WriteLine("Не правильный ввод");
                        }
                        break;
                    case "выполнить":
                        if (proj!=""&&int.TryParse(input[1],out num)&& CompleteTask(name, num, proj))
                        {
                            Console.WriteLine("Вы выполнили задачу");
                        }
                        break;
                    case "отмена":
                        flag = false;
                        break;
                    case "выйти":
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("Не правильная команда");
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
                int count = 0;
                while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) ;
                while ((line = strRead.ReadLine()) != null )
                {
                    if (line.Split()[3].Equals("2"))
                    {
                        Console.WriteLine(line);
                        count++;
                    }
                }
                if (count==0)
                {
                    Console.WriteLine("Задач на проверку нет");
                }
            }
        }
        static void AllowedTask(string proj,int num)
        {
            List<string> lines = new List<string>();
            bool flag = false;
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
                    else
                    {
                        if (line.Split()[3].Equals("2"))
                        {
                            flag = true;
                        }
                    }
                }
            }
            File.WriteAllText($@"resource\{proj}\main.txt",string.Join("\r\n",lines));
            if (flag)
            {
                Console.WriteLine("Задача подтверждена");
            }
            else
            {
                Console.WriteLine("Нельзя подтвердить");
            }

        }
        static void CheckOrders(string proj)
        {
            string[] orders = Directory.GetFiles($@"resource\{proj}");
            int count = 0;
            foreach (var order in orders)
            {
                if (order.Split('\\')[order.Split('\\').Length-1].Split('.')[0]!="main")
                {
                    Console.WriteLine(order.Split('\\')[order.Split('\\').Length - 1].Split('.')[0]+"\n"+File.ReadAllText(order));
                    count++;
                }
            }
            if (count==0)
            {
                Console.WriteLine("Нет отчетов");
            }
        }

        static void CheckTasks(string proj,string name)
        {
            if (proj=="")
            {
                using (StreamReader strRead= new StreamReader(PATH_TASKS))
                {
                    string line;
                    int count = 0;
                    while ((line=strRead.ReadLine())!=null)
                    {
                        if (line.Split()[0].ToLower().Equals(name.ToLower()))
                        {
                            Console.WriteLine(line);
                        }
                    }
                    if (count == 0)
                    {
                        Console.WriteLine("Задач нет");
                    }
                }
            }
            else
            {
                using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
                {
                    string line;
                    int count = 0;
                    while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) ;
                    while ((line = strRead.ReadLine()) != null)
                    {

                        if (line.Split()[0].ToLower().Equals(name.ToLower()))
                        {
                            Console.WriteLine(line);
                            count++;
                        }
                    }
                    if (count==0)
                    {
                        Console.WriteLine("Задач нет");
                    }
                }
            }
        }
        static string TakeTask(string name,int num)
        {
            int count=0;
            string proj="";
            List<string> lines = new List<string>();
            using (StreamReader strRead = new StreamReader(PATH_TASKS))
            {
                string line;
                while ((line = strRead.ReadLine()) != null)
                {
                    if (line.Split()[0].ToLower().Equals(name.ToLower()))
                    {
                        count++;
                        if (count == num)
                        {
                            AddProjEmployee(name, line.Split()[5]);
                            proj = line.Split()[5];
                        }
                    }
                    else
                    {
                        lines.Add(line);
                    }

                }
            }
            count = 0;
            File.WriteAllText(PATH_TASKS, string.Join("\r\n", lines));
            lines = new List<string>();
            using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
            {
                string line;
                while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) lines.Add(line);
                lines.Add(line);
                while ((line = strRead.ReadLine()) != null)
                {
                    if (line.Split()[0].ToLower().Equals(name.ToLower()))
                    {
                        count++;
                        if (count != num)
                        {
                            lines.Add(line);
                        }
                        else
                        {
                            List<string> str =line.Split().ToList();
                            str[3]="1";
                            lines.Add(string.Join(" ",str));
                        }
                    }
                    else
                    {
                        lines.Add(line);
                    }

                }
            }
            File.WriteAllText($@"resource\{proj}\main.txt", string.Join("\r\n", lines));
            return proj;
        }
        static void TakeTask(string name,int num,string proj)
        {
            int count = 0;
            List<string> lines = new List<string>();
            using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
            {
                string line;
                while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) lines.Add(line);
                lines.Add(line);
                while ((line = strRead.ReadLine()) != null)
                {
                    if (line.Split()[0].ToLower().Equals(name.ToLower()))
                    {
                        count++;
                        if (count != num)
                        {
                            lines.Add(line);
                        }
                        else
                        {
                            List<string> str = line.Split().ToList();
                            str[3] = "1";
                            lines.Add(string.Join(" ", str));
                        }
                    }
                    else
                    {
                        lines.Add(line);
                    }

                }
            }
            File.WriteAllText($@"resource\{proj}\main.txt", string.Join("\r\n", lines));
        }
        static bool CompleteTask(string name,int num,string proj)
        {
            List<string> lines = new List<string>();
            bool flag=false;
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
                        if (count != num)
                        {
                            lines.Add(line);
                        }
                        else
                        {
                            List<string> task = line.Split().ToList();
                            if (task[3].Equals("1"))
                            {
                                flag = true;
                                task[3] = "2";
                                lines.Add(string.Join(" ",task));
                                CreateOReport(name, proj);
                            }
                            else
                            {
                                Console.WriteLine("Нельзя сдать на проверку не принятую(законченную) задачу");
                            }
                        }
                    }
                    else
                    {
                        lines.Add(line);
                    }

                }
            }
            File.WriteAllText($@"resource\{proj}\main.txt", string.Join("\r\n", lines));
            return flag;
        }
        static void DelegateTask(string name,string delName,int num,string proj)
        {
            if (proj=="")
            {
                int count = 0;
                List<string> lines = new List<string>();
                using (StreamReader strRead = new StreamReader(PATH_TASKS))
                {
                    string line;
                    while ((line = strRead.ReadLine()) != null)
                    {
                        if (line.Split()[0].ToLower().Equals(name.ToLower()))
                        {
                            count++;
                            if (count == num)
                            {
                                List<string> task= line.Split().ToList();
                                task[0]=delName;
                                lines.Add(string.Join(" ",task));
                                DelegateTask(name, delName, num, task[5]);
                            }
                            else
                            {
                                lines.Add(line);
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
            else
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
                            if (count != num)
                            {
                                lines.Add(line);
                            }
                            else
                            {
                                List<string> task = line.Split().ToList();
                                task[0] = delName;
                                lines.Add(string.Join(" ", task));
                            }
                        }
                        else
                        {
                            lines.Add(line);
                        }

                    }
                }
                File.WriteAllText($@"resource\{proj}\main.txt", string.Join("\r\n", lines));
            }
        }
        static void RejectTask(string name,int num,string proj)
        {
            if (proj == "")
            {
                int count = 0;
                List<string> lines = new List<string>();
                using (StreamReader strRead = new StreamReader(PATH_TASKS))
                {
                    string line;
                    while ((line = strRead.ReadLine()) != null)
                    {
                        if (line.Split()[0].ToLower().Equals(name.ToLower()))
                        {
                            count++;
                            if (count == num)
                            {
                                File.WriteAllText(PATH_REJ_TASKS,File.ReadAllText(PATH_REJ_TASKS)+"\r\n"+line);
                                DeleteTask(name,num,$@"resource\{line.Split()[5]}\main.txt");
                                Console.WriteLine("Вы отклонили задачу");

                            }
                            else
                            {
                                lines.Add(line);
                            }
                        }
                        else
                        {
                            lines.Add(line);
                        }

                    }
                }
                if (count == 0 || count < num)
                {
                    Console.WriteLine("Невозможно отклонить");
                }
                File.WriteAllText(PATH_TASKS,string.Join("\r\n",lines));
            }
            else
            {
                int count = 0;
                List<string> lines = new List<string>();
                using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
                {
                    string line;
                    while ((line = strRead.ReadLine()) != null)
                    {
                        if (line.Split()[0].ToLower().Equals(name.ToLower()))
                        {
                            count++;
                            if (count == num)
                            {
                                File.WriteAllText(PATH_REJ_TASKS, File.ReadAllText(PATH_REJ_TASKS) + "\r\n" + line);
                                Console.WriteLine("Вы отклонили задачу");
                            }
                            else
                            {
                                lines.Add(line);
                            }
                        }
                        else
                        {
                            lines.Add(line);
                        }

                    }
                }
                if (count==0||count<num)
                {
                    Console.WriteLine("Невозможно отклонить");
                }
                File.WriteAllText($@"resource\{proj}\main.txt", string.Join("\r\n", lines));
            }
        }

        static void CreateOReport(string name,string proj)
        {
            Console.WriteLine("Введите описание отчета");
            string text = Console.ReadLine();
            Console.WriteLine("Введите название отчета");
            string title = Console.ReadLine();
            Report rep = new Report(text,name,proj,title);
        }

        static void ViewRejTask(string proj)
        {
            using (StreamReader strRead = new StreamReader(PATH_REJ_TASKS))
            {
                string line;
                while ((line=strRead.ReadLine())!=null)
                {
                    if (line.Length> 5&& line.Split()[5].Equals(proj))
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }
        static void DeleteTask(string name, int num, string path)
        {
            List<string> lines = new List<string>();
            using (StreamReader strRead = new StreamReader(path))
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
                        if (count != num)
                        {
                            lines.Add(line);
                        }
                    }
                    else
                    {
                        lines.Add(line);
                    }

                }
            }
            File.WriteAllText(path, string.Join("\r\n", lines));
        }

        static void ReGiveTask(int num,string whom,string name)
        {
            List<string> text = new List<string>();
            using (StreamReader strRead= new StreamReader(PATH_REJ_TASKS))
            {
                int count=1;
                string line;
                while ((line=strRead.ReadLine())!=null)
                {
                    if (count==num && line.Split().Length>5 && line.Split()[1].Equals(name))
                    {
                        count++;
                        List<string> lin = line.Split().ToList();
                        lin[0] = whom;
                        File.WriteAllText(PATH_TASKS, File.ReadAllText(PATH_TASKS) + "\r\n" + string.Join(" ", lin));
                    }
                    else if(line.Split().Length > 5 && line.Split()[1].Equals(name)) { count++; }
                    else
                    {
                        text.Add(line);
                    }
                }
            }
            File.WriteAllText(PATH_REJ_TASKS,string.Join("\r\n",text));
        }
        static void RejectTask(string proj,int num)
        {
            List<string> text = new List<string>();
            int count = 1;
            using (StreamReader strRead = new StreamReader($@"resource\{proj}\main.txt"))
            {
                string line;
                while (!(line = strRead.ReadLine()).ToLower().Equals("задачи:")) text.Add(line);
                text.Add(line);
                while ((line = strRead.ReadLine()) != null)
                {
                    if (line.Split()[3].Equals("2")&&count==num)
                    {
                        List<string> str = line.Split().ToList();
                        str[3] = "1";
                        count++;
                        text.Add(string.Join(" ",str));
                        Console.WriteLine("Задача возвращена"); 
                    }
                    else if(line.Split()[3].Equals("2"))
                    {
                        count++;
                        text.Add(line);
                    }
                    else
                    {
                        text.Add(line);
                    }
                }
            }
            File.WriteAllText($@"resource\{proj}\main.txt", string.Join("\r\n", text));
            if (count<num||num<=0||count==num)
            {
                Console.WriteLine("Задачу вернуть невозможно");
            }
        }
    }

}
