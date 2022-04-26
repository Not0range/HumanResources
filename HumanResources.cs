using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumanResources
{
    class Employee
    {
        public static List<Employee> employees = new List<Employee>();

        public int id;
        public string name;
        public DateTime birthDate;
        public string address;
        public string phone;

        public override string ToString()
        {
            return name;
        }
    }

    class Contract
    {
        public static List<Contract> contracts = new List<Contract>();

        public int id;
        public Employee employee;
        public DateTime date;
        public string position;
        public decimal salary;
    }

    class Course
    {
        public static List<Course> courses = new List<Course>();

        public int id;
        public string title;
        public List<Employee> employees;
        public DateTime beginDate;
        public DateTime endDate;
    }
}
