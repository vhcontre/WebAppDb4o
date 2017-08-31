using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAppDb4o.Models
{
    public  class Person
    {
        public string RowGuid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Title { get; set; }
        public Person()
        {
                //RowGuid= Guid.NewGuid();
        }
        public Person(string firstName, string lastName, byte age)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
        }

        public Person(string id, string firstName, string lastName, byte age, string title)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Age = age;
            this.Title = title;
        }

        public override string ToString()
        {
            return Title + " " + FirstName + " " + LastName + ": " + Age;
        }
    }
}