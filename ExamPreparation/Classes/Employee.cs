using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExamPreparation.Classes
{
    public class Employee
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime BirthDate { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }

        public Employee(string lastName, string firstName, DateTime birthDate, string city, string address, string position)
        {
            LastName = lastName;
            FirstName = firstName;
            BirthDate = birthDate;
            City = city;
            Address = address;
            Position = position;
        }
    }

}
