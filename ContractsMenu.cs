using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources
{
    static class ContractsMenu
    {
        public static List<string> table = new List<string>();
        public static void FillTable()
        {
            string[] columns = new string[] { "ID", "Сотрудник", "Дата заключения договора", "Должность",
                "Заработная плата" };
            int[] widths = new int[columns.Length];

            table.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var c in Contract.contracts)
            {
                if (c.id.ToString().Length > widths[0])
                    widths[0] = c.id.ToString().Length;
                if (c.employee.ToString().Length > widths[1])
                    widths[1] = c.employee.ToString().Length;
                if (c.position.Length > widths[3])
                    widths[3] = c.position.Length;
                if (c.salary.ToString().Length > widths[4])
                    widths[4] = c.salary.ToString().Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var c in Contract.contracts)
            {
                temp[0] = (c.id.ToString(), widths[0]);
                temp[1] = (c.employee.ToString(), widths[1]);
                temp[2] = (c.date.ToString("dd.MM.yyyy"), widths[2]);
                temp[3] = (c.position, widths[3]);
                temp[4] = (c.salary.ToString(), widths[4]);
                table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));
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
                Console.WriteLine("1 - Добавить договор");
                Console.WriteLine("2 - Редактировать договор");
                Console.WriteLine("3 - Удалить договор");
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
            string employeeStr, dateStr, position, salaryStr;
            int employee;
            DateTime date;
            decimal salary;
            bool success;

            EmployeesMenu.table.ForEach(t => Console.WriteLine(t));
            do
            {
                employeeStr = Program.ReadLine("Введите ID сотрудника, с которым заключается договор: ");

                success = int.TryParse(employeeStr, out employee) && 
                    Employee.employees.Any(t => t.id == employee);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                dateStr = Program.ReadLine("Введите дату заключения договор: ");

                success = DateTime.TryParse(dateStr, out date);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            position = Program.ReadLine("Введите должность сотрудника: ");
            do
            {
                salaryStr = Program.ReadLine("Введите заработную плату: ");

                success = decimal.TryParse(salaryStr, out salary);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Сотрудник: {0}", Employee.employees.First(t => t.id == employee));
            Console.WriteLine("Дата заключения договора: {0}", date.ToString("dd.mm.yyyy"));
            Console.WriteLine("Должность: {0}", position);
            Console.WriteLine("Заработная плата: {0}", salary);
            Console.WriteLine("Добавить данный договор?");
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

            Contract.contracts.Add(new Contract
            {
                id = Contract.contracts.Count > 0 ? Contract.contracts.Last().id + 1 : 0,
                employee = Employee.employees.First(t => t.id == employee),
                date = date,
                position = position,
                salary = salary,
            });
            return true;
        }

        public static bool Edit()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID договора, который необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Contract c;
            if (!success || (c = Contract.contracts.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();
            Console.WriteLine("В скобках указывается текущее значение.");
            Console.WriteLine("Если необходимо его оставить без изменений, оставьте строку ввода пустой.");
            Console.WriteLine();

            string employeeStr, dateStr, position, salaryStr;
            int employee = 0;
            DateTime date = new DateTime();
            decimal salary = 0;

            EmployeesMenu.table.ForEach(t => Console.WriteLine(t));
            do
            {
                employeeStr = Program.ReadLine($"Введите ID сотрудника, с которым заключается договор ({c.employee.id}): ", true);
                if (string.IsNullOrWhiteSpace(employeeStr))
                    break;

                success = int.TryParse(employeeStr, out employee) &&
                    Employee.employees.Any(t => t.id == employee);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            do
            {
                dateStr = Program.ReadLine($"Введите дату заключения договор ({c.date.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(dateStr))
                    break;

                success = DateTime.TryParse(dateStr, out date);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            position = Program.ReadLine($"Введите должность сотрудника ({c.position}): ", true);
            do
            {
                salaryStr = Program.ReadLine($"Введите заработную плату ({c.salary}): ", true);
                if (string.IsNullOrWhiteSpace(salaryStr))
                    break;

                success = decimal.TryParse(salaryStr, out salary);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            Console.Clear();
            Console.WriteLine("Сотрудник: {0}", string.IsNullOrWhiteSpace(employeeStr) ? 
                c.employee : Employee.employees.First(t => t.id == employee));
            Console.WriteLine("Дата заключения договора: {0}", string.IsNullOrWhiteSpace(dateStr) ? 
                c.date.ToString("dd.mm.yyyy") : date.ToString("dd.mm.yyyy"));
            Console.WriteLine("Должность: {0}", string.IsNullOrWhiteSpace(position) ? c.position : position);
            Console.WriteLine("Заработная плата: {0}", string.IsNullOrWhiteSpace(salaryStr) ? c.salary : salary);
            Console.WriteLine("Сохранить данный договор?");
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

            if (!string.IsNullOrWhiteSpace(employeeStr))
                c.employee = Employee.employees.First(t => t.id == employee);
            if (!string.IsNullOrWhiteSpace(dateStr)) c.date = date;
            if (!string.IsNullOrWhiteSpace(position)) c.position = position;
            if (!string.IsNullOrWhiteSpace(salaryStr)) c.salary = salary;

            return true;
        }

        public static bool Delete()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID договора, который необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Contract c;
            if (!success || (c = Contract.contracts.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Contract.contracts.Remove(c);
            return true;
        }
    }
}