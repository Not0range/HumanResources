using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources
{
    static class CoursesMenu
    {
        public static List<string> table = new List<string>();
        public static void FillTable()
        {
            string[] columns = new string[] { "ID", "Наименование", "Сотрудники", "Дата начала", "Дата окончания" };
            int[] widths = new int[columns.Length];

            table.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            int max;
            foreach (var c in Course.courses)
            {
                if (c.id.ToString().Length > widths[0])
                    widths[0] = c.id.ToString().Length;
                if (c.title.Length > widths[1])
                    widths[1] = c.title.Length;
                max = c.employees.Max(t => t.ToString().Length);
                if (max > widths[2])
                    widths[2] = max;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var c in Course.courses)
            {
                temp[0] = (c.id.ToString(), widths[0]);
                temp[1] = (c.title, widths[1]);
                temp[2] = (c.employees[0].ToString(), widths[2]);
                temp[3] = (c.beginDate.ToString("dd.MM.yyyy"), widths[3]);
                temp[4] = (c.endDate.ToString("dd.MM.yyyy"), widths[4]);
                table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

                foreach (var item in c.employees.Skip(1))
                {
                    temp[0] = ("", widths[0]);
                    temp[1] = ("", widths[1]);
                    temp[2] = (item.ToString(), widths[2]);
                    temp[3] = ("", widths[3]);
                    temp[4] = ("", widths[4]);
                    table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
                }
            }
        }

        public static void WriteMenu()
        {
            var wasChanged = false;
            var error = false;
            ConsoleKeyInfo k;

            do
            {
                if (wasChanged)
                    FillTable();

                table.ForEach(t => Console.WriteLine(t));

                Console.WriteLine();
                if (error)
                {
                    Console.WriteLine("Ошибка ввода");
                    error = false;
                }
                Console.WriteLine("Выберите действие:");
                Console.WriteLine("1 - Добавить курс");
                Console.WriteLine("2 - Редактировать курс");
                Console.WriteLine("3 - Удалить курс");
                Console.WriteLine("4 - Вернуться в предыдущее меню");

                k = Console.ReadKey(true);
                Console.Clear();
                switch (k.KeyChar)
                {
                    case '1':
                        wasChanged = Add();
                        break;
                    case '2':
                        wasChanged = Edit();
                        break;
                    case '3':
                        wasChanged = Delete();
                        break;
                    default:
                        error = true;
                        break;
                }
                Console.Clear();
            } while (k.KeyChar != '4');
        }

        public static bool Add()
        {
            string title, beginStr, endStr;
            DateTime beginDate;
            DateTime endDate;
            var employees = new List<Employee>();
            bool success;

            title = Program.ReadLine("Введите наименование курса: ");

            EmployeeListEdit(employees);

            do
            {
                beginStr = Program.ReadLine("Введите дату начала курса: ");

                success = DateTime.TryParse(beginStr, out beginDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                endStr = Program.ReadLine("Введите дату окончания курса: ");

                success = DateTime.TryParse(endStr, out endDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Наименование: {0}", title);
            Console.WriteLine("Сотрудники:");
            employees.ForEach(t => Console.WriteLine(t));
            Console.WriteLine("Дата начала курса: {0}", beginDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Дата окончания курса: {0}", endDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Добавить данный курс?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            Course.courses.Add(new Course
            {
                id = Course.courses.Count > 0 ? Course.courses.Last().id + 1 : 0,
                title = title,
                employees = employees,
                beginDate = beginDate,
                endDate = endDate,
            });
            return true;
        }

        public static bool Edit()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID курса, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Course c;
            if (!success || (c = Course.courses.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();
            Console.WriteLine("В скобках указывается текущее значение.");
            Console.WriteLine("Если необходимо его оставить без изменений, оставьте строку ввода пустой.");
            Console.WriteLine();

            string title, beginStr, endStr;
            DateTime beginDate = new DateTime();
            DateTime endDate = new DateTime();
            var employees = new List<Employee>(c.employees);

            title = Program.ReadLine($"Введите наименование курса ({c.title}): ", true);

            EmployeeListEdit(employees);

            do
            {
                beginStr = Program.ReadLine($"Введите дату начала курса ({c.beginDate.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(beginStr))
                    break;

                success = DateTime.TryParse(beginStr, out beginDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                endStr = Program.ReadLine($"Введите дату окончания курса ({c.endDate.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(endStr))
                    break;

                success = DateTime.TryParse(endStr, out endDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Наименование: {0}", string.IsNullOrWhiteSpace(title) ? c.title : title);
            Console.WriteLine("Сотрудники:");
            employees.ForEach(t => Console.WriteLine("  {0}", t));
            Console.WriteLine("Дата начала курса: {0}", string.IsNullOrWhiteSpace(beginStr) ? 
                c.beginDate.ToString("dd.mm.yyyy") : beginDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Дата окончания курса: {0}", string.IsNullOrWhiteSpace(endStr) ?
                c.endDate.ToString("dd.mm.yyyy") : endDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Сохранить данный курс?");
            Console.WriteLine("1 - Да");
            Console.WriteLine("2 - Нет");
            do
            {
                var k = Console.ReadKey(true);
                if (k.KeyChar == '1')
                    break;
                else if (k.KeyChar == '2')
                    return false;

            } while (true);

            if (!string.IsNullOrWhiteSpace(title)) c.title = title;
            c.employees = employees;
            if (!string.IsNullOrWhiteSpace(beginStr)) c.beginDate = beginDate;
            if (!string.IsNullOrWhiteSpace(endStr)) c.endDate = endDate;

            return true;
        }

        static void EmployeeListEdit(List<Employee> employees)
        {
            bool success;
            ConsoleKeyInfo key;
            do
            {
                Console.Clear();
                Console.WriteLine("Текущий список сотрудников:");
                for (int i = 0; i < employees.Count; i++)
                    Console.WriteLine("{0}. {1}", i + 1, employees[i]);
                Console.WriteLine();
                Console.WriteLine("1 - Добавить сотрудника в список");
                Console.WriteLine("2 - Удалить сотрудника из списка");
                if (employees.Count > 0)
                    Console.WriteLine("3 - Продолжить");
                key = Console.ReadKey(true);
                if (key.KeyChar == '1')
                {
                    Console.Clear();
                    EmployeesMenu.table.ForEach(t => Console.WriteLine(t));
                    Console.WriteLine();

                    string tempStr;
                    int id;
                    Employee e = null;
                    do
                    {
                        tempStr = Program.ReadLine("Введите ID сотрудника: ");
                        if (string.IsNullOrWhiteSpace(tempStr))
                            break;

                        success = int.TryParse(tempStr, out id) &&
                            ((e = Employee.employees.FirstOrDefault(t => t.id == id)) != null);
                        if (!success)
                            Console.WriteLine("Ошибка ввода");
                    } while (!success);

                    employees.Add(e);
                }
                else if (key.KeyChar == '2')
                {
                    Console.Write("Введите порядковый номер сотрудника, которого необходимо удалить ");
                    int id;
                    success = int.TryParse(Console.ReadLine(), out id);
                    if (!success || id - 1 < 0 || id - 1 >= employees.Count)
                        continue;
                    employees.RemoveAt(id - 1);
                }
            } while (key.KeyChar != '3' || employees.Count == 0);
        }

        public static bool Delete()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID курса, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Course c;
            if (!success || (c = Course.courses.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Course.courses.Remove(c);
            return true;
        }
    }
}