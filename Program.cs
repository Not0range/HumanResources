using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources
{
    static class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Отдел кадров";
            Console.BufferWidth = 200;

            bool error = false;
            ReadEmployees(ref error);
            ReadContracts(ref error);
            ReadCourses(ref error);

            if (error)
            {
                Console.WriteLine("При чтении файла были обнаружены некорректные записи.");
                Console.WriteLine("Они не были добавлены в таблицы и будут удалены");
                Console.WriteLine("Нажмите любую клавишу, чтобы продолжить");
                Console.ReadKey(true);
            }
            EmployeesMenu.FillTable();
            ContractsMenu.FillTable();
            CoursesMenu.FillTable();

            ConsoleKeyInfo key;
            error = false;

            do
            {
                if (!error)
                    Console.WriteLine("Добро пожаловать в систему управления отдела кадров!");
                else
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }

                Console.WriteLine("Выберите необходимое действие:");
                Console.WriteLine("1 - Работа с таблицей сотрудников");
                Console.WriteLine("2 - Работа с таблицей договоров");
                Console.WriteLine("3 - Работа с таблицей курсов");
                Console.WriteLine("4 - Выход из программы");
                key = Console.ReadKey(true);
                Console.Clear();
                switch (key.KeyChar)
                {
                    case '1':
                        EmployeesMenu.WriteMenu();
                        break;
                    case '2':
                        ContractsMenu.WriteMenu();
                        break;
                    case '3':
                        CoursesMenu.WriteMenu();
                        break;
                    default:
                        error = true;
                        break;
                }
            } while (key.KeyChar != '4');
            SaveAll();
        }

        static void ReadEmployees(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("employees.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));

            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Employee.employees.Any(t => t.id == id))
                        throw new ArgumentException();

                    Employee.employees.Add(new Employee
                    {
                        id = id,
                        name = data[1],
                        birthDate = DateTime.Parse(data[2]),
                        address = data[3],
                        phone = data[4],
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadContracts(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("contracts.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Contract.contracts.Any(t => t.id == id))
                        throw new ArgumentException();

                    Contract.contracts.Add(new Contract
                    {
                        id = id,
                        employee = Employee.employees.First(t => t.id == int.Parse(data[1])),
                        date = DateTime.Parse(data[2]),
                        position = data[3],
                        salary = decimal.Parse(data[4]),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void ReadCourses(ref bool error)
        {
            int id;
            string[] data;
            var reader = new StreamReader(new FileStream("courses.txt",
                FileMode.OpenOrCreate, FileAccess.ReadWrite));
            while (!reader.EndOfStream)
            {
                try
                {
                    data = reader.ReadLine().Split('\t');
                    id = int.Parse(data[0]);
                    if (Course.courses.Any(t => t.id == id))
                        throw new ArgumentException();


                    Course.courses.Add(new Course
                    {
                        id = id,
                        title = data[1],
                        employees = data.Take(data.Length - 2).Skip(2)
                            .Select(t => Employee.employees.First(e => e.id == int.Parse(t))).ToList(),
                        beginDate = DateTime.Parse(data[data.Length - 2]),
                        endDate = DateTime.Parse(data[data.Length - 1]),
                    });
                }
                catch
                {
                    error = true;
                }
            }
            reader.Close();
        }

        static void SaveAll()
        {
            StreamWriter writer;
            writer = new StreamWriter("employees.txt");
            foreach (var e in Employee.employees)
                writer.WriteLine(string.Join("\t", e.id, e.name,
                    e.birthDate.ToShortDateString(), e.address, e.phone));
            writer.Close();

            writer = new StreamWriter("contracts.txt");
            foreach (var c in Contract.contracts)
                writer.WriteLine(string.Join("\t", c.id, c.employee.id, 
                    c.date.ToShortDateString(), c.position, c.salary));
            writer.Close();

            writer = new StreamWriter("courses.txt");
            foreach (var c in Course.courses)
                writer.WriteLine(string.Join("\t", c.id, c.title, 
                    string.Join("\t", c.employees.Select(t => t.id.ToString())),
                    c.beginDate.ToShortDateString(), c.endDate.ToShortDateString()));
            writer.Close();
        }

        public static string ReadLine(string text, bool allowEmpty = false)
        {
            bool success;
            string str;
            do
            {
                Console.Write(text);
                str = Console.ReadLine().Trim();
                success = !string.IsNullOrWhiteSpace(str) || allowEmpty;
                if (!success)
                {
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                    Console.Write(new string('\0', Console.BufferWidth));
                    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            } while (!success);
            return str;
        }
    }
}
