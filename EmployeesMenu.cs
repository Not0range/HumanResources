using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources
{
    static class EmployeesMenu
    {
        public static List<string> table = new List<string>();
        public static void FillTable()
        {
            string[] columns = new string[] { "ID", "ФИО", "Дата рождения", "Адрес",
                "Номер телефона" };
            int[] widths = new int[columns.Length];

            table.Clear();
            for (int i = 0; i < columns.Length; i++)
                widths[i] = columns[i].Length;

            foreach (var d in Employee.employees)
            {
                if (d.id.ToString().Length > widths[0])
                    widths[0] = d.id.ToString().Length;
                if (d.name.Length > widths[1])
                    widths[1] = d.name.Length;
                if (d.address.Length > widths[3])
                    widths[3] = d.address.Length;
                if (d.phone.Length > widths[4])
                    widths[4] = d.phone.Length;
            }
            for (int i = 0; i < widths.Length; i++)
                widths[i] += 3;

            var temp = new (string text, int width)[columns.Length];
            for (int i = 0; i < temp.Length; i++)
                temp[i] = (columns[i], widths[i]);
            table.Add(string.Join("", temp.Select(f => f.text.PadRight(f.width))));

            foreach (var d in Employee.employees)
            {
                temp[0] = (d.id.ToString(), widths[0]);
                temp[1] = (d.name, widths[1]);
                temp[2] = (d.birthDate.ToString("dd.MM.yyyy"), widths[2]);
                temp[3] = (d.address, widths[3]);
                temp[4] = (d.phone, widths[4]);
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
                Console.WriteLine("1 - Добавить сотрудника");
                Console.WriteLine("2 - Редактировать сотрудника");
                Console.WriteLine("3 - Удалить сотрудника");
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
            string name, address, birthStr, phone;
            DateTime birthDate;
            bool success;

            name = Program.ReadLine("Введите ФИО сотрудника: ");

            do
            {
                birthStr = Program.ReadLine("Введите дату рождения сотрудника: ");

                success = DateTime.TryParse(birthStr, out birthDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            address = Program.ReadLine("Введите адрес сотрудника: ");
            phone = Program.ReadLine("Введите номер телефона сотрудника: ");

            Console.Clear();
            Console.WriteLine("ФИО: {0}", name);
            Console.WriteLine("Дата роджения: {0}", birthDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Адрес: {0}", address);
            Console.WriteLine("Номер телефона: {0}", phone);
            Console.WriteLine("Добавить данного сотрудника?");
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

            Employee.employees.Add(new Employee
            {
                id = Employee.employees.Count > 0 ? Employee.employees.Last().id + 1 : 0,
                name = name,
                birthDate = birthDate,
                address = address,
                phone = phone,
            });
            return true;
        }

        public static bool Edit()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();

            Console.Write("Введите ID сотрудника, которого необходимо отредактировать: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Employee e;
            if (!success || (e = Employee.employees.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Console.Clear();
            Console.WriteLine("В скобках указывается текущее значение.");
            Console.WriteLine("Если необходимо его оставить без изменений, оставьте строку ввода пустой.");
            Console.WriteLine();

            string name, address, birthStr, phone;
            DateTime birthDate = new DateTime();

            name = Program.ReadLine($"Введите ФИО сотрудника ({e.name}): ", true);

            do
            {
                birthStr = Program.ReadLine($"Введите дату рождения сотрудника " +
                    $"({e.birthDate.ToString("dd.MM.yyyy")}): ", true);
                if (string.IsNullOrWhiteSpace(birthStr))
                    break;

                success = DateTime.TryParse(birthStr, out birthDate);
                if (!success)
                    Console.WriteLine("Ошибка ввода");
            } while (!success);

            address = Program.ReadLine($"Введите адрес сотрудника ({e.address}): ", true);
            phone = Program.ReadLine($"Введите номер телефона сотрудника ({e.phone}): ", true);

            Console.Clear();
            Console.WriteLine("ФИО: {0}", string.IsNullOrWhiteSpace(name) ? e.name : name);
            Console.WriteLine("Дата роджения: {0}", string.IsNullOrWhiteSpace(birthStr) ?
                e.birthDate.ToString("dd.mm.yyyy") : birthDate.ToString("dd.mm.yyyy"));
            Console.WriteLine("Адрес: {0}", string.IsNullOrWhiteSpace(address) ? e.address : address);
            Console.WriteLine("Номер телефона: {0}", string.IsNullOrWhiteSpace(phone) ? e.phone : phone);
            Console.WriteLine("Сохранить данного сотрудника?");
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

            if (!string.IsNullOrWhiteSpace(name)) e.name = name;
            if (!string.IsNullOrWhiteSpace(birthStr)) e.birthDate = birthDate;
            if (!string.IsNullOrWhiteSpace(address)) e.address = address;
            if (!string.IsNullOrWhiteSpace(phone)) e.phone = phone;

            return true;
        }

        public static bool Delete()
        {
            table.ForEach(t => Console.WriteLine(t));
            Console.WriteLine();
            Console.Write("Введите ID сотрудника, которого необходимо удалить: ");
            int id;
            bool success = int.TryParse(Console.ReadLine(), out id);
            Employee e;
            if (!success || (e = Employee.employees.FirstOrDefault(t => t.id == id)) == null)
                return false;

            Contract.contracts.RemoveAll(c => c.employee.id == e.id);
            ContractsMenu.FillTable();
            Course.courses.ForEach(c => c.employees.RemoveAll(t => t.id == e.id));
            Course.courses.RemoveAll(c => !c.employees.Any());
            CoursesMenu.FillTable();
            Employee.employees.Remove(e);
            return true;
        }
    }
}