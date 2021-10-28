using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager
{
    
    class Program
    {
        private const string  PATH_WORKER = @"resource\Employee.txt";
        private const string PATH_REQ_PROJ = @"resource\request_projects.txt";
        private const string PATH_PROJECT = @"resource\PROJ.txt";
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
                    workers.Add(new Worker(lines[0],lines[1].Equals("tl")?0:(Post)1));
                }
            }

            Post rank;
            string proj;
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
                    UserTL(proj);
                }
                else
                {
                    UserDev(proj);
                }
                user = ChangeUser(workers, out rank, out proj);
            }


        }
        static string ChangeUser(List<Worker> workers,out Post rank,out string proj)
        {
            bool flag = true;
            string user = "anonim";
            Worker worker = null;
            rank = 0;
            proj = null;
            while (flag)
            {
                Console.WriteLine("Введите за кого хотите войти ");
                string input = Console.ReadLine();
                worker = workers.Find(x => x.Name.ToLower().Equals(input.ToLower()));
                if (worker != null)
                {
                    user = worker.Name;
                    rank = worker.Post;
                    proj = worker.NameProject;
                    flag = false;
                }
                if (input.ToLower().Equals("заказчик"))
                {
                    user = "Заказчик";
                    flag = false;
                }
                if (input.ToLower().Equals("отмена"))
                {
                    flag = false;
                }
            }
            proj = (proj != null ?proj:"");
            return user;
        }
        static void UserCustomer()
        {
            bool flag = true;
            while (flag)
            {
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.WriteLine(/*"\"Посмотреть\" чтобы посмотреть проекты\n" +*/
                    "\"Создать {название проекта} {число(месяцев,дней...)} {Д,Н,М,Г}\" - добавить проект в предложку\n " +
                    "ОТМЕНА чтоб выйти" /*+
                    "\"Выбрать {название проекта}\" открыть проект и посмотреть отчеты к нему\n" +
                    "\"Посмотреть №\" - чтобы посмотреть отчет под номером к проекту"*/);
                Console.ResetColor();
                string[] input = Console.ReadLine().ToLower().Split();
                if (input.Length == 4 && input[0].Equals("создать"))
                {
                    //List<string> chars = new List<string> {"д", "н", "м", "г" };
                    //chars.IndexOf(input[3]);
                    input[0] = "";
                    using (StreamWriter strWrt=new StreamWriter(PATH_REQ_PROJ))
                    {
                        strWrt.WriteLine(string.Join(" ", input));
                    }
                    
                }
                else if (input[0].Equals("отмена"))
                {
                    flag = false;
                }
                else Console.WriteLine("Вы ввели не правильно");
            }
        }
        static void UserTL(string proj)
        {
            bool flag = true;
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

                            break;
                        case "отмена":
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
                    "\"Посмотреть №\" - чтобы посмотреть отчет под номером к проекту\n" +
                    "\"Выдать@текст задания@кому) \" - предложить задание \n" +
                    "\"Закрыть\"-проект\n" +
                    "\"Начать\"-проект в стадию исполнения");
                    Console.ResetColor();
                    string input = Console.ReadLine();
                }
                

            }
        }
        static void UserDev(string proj)
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
                        "\"Выполнить № %\" сделать задачу и отчет к ней (если задача разовая и % выполнения достиг 100)\n" +
                        "\"Сделать\" - создать отчет для  ");
                }
                Console.ResetColor();
                string input = Console.ReadLine();

            }
        }
    }
}
