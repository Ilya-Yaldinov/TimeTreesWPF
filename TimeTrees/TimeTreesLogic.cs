using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;

namespace TimeTrees
{
    class TimeTreesLogic
    {
        public DateTime ParseDate(string value)
        {
            DateTime date;
            if (!DateTime.TryParseExact(value, "yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                if (!DateTime.TryParseExact(value, "yyyy-MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    if (!DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                        date = default;

            return date;
        }

        public string OutputFormat(string input)
        {
            if (input.Length == 4) return "yyyy";
            if (input.Length == 7) return "yyyy-MM";
            return "yyyy-MM-dd";
        }

        public Person[] ReadPerson(string path)
        {
            string[] file = File.ReadAllLines(path);
            Person[] peoples = new Person[file.Length];
            for (int i = 0; i < file.Length; i++)
            {
                string[] line = file[i].Split(";");
                Person person = new Person();
                person.Id = Convert.ToInt32(line[0]);
                person.Name = line[1];
                person.Birth = ParseDate(line[2]);
                person.Death = string.IsNullOrEmpty(line[3]) ? null : ParseDate(line[3]);
                person.Parrents = line[4].Split(",");
                bool hasSpouse = int.TryParse(line[5], out int spouse);
                if (hasSpouse) person.Spouse = spouse;
                person.Childrens = line[6].Split(",");
                string[] position = line[7].Split(",");
                person.PositionX = Convert.ToInt32(position[0]);
                person.PositionY = Convert.ToInt32(position[1]);

                peoples[i] = person;
            }

            return peoples;
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public DateTime? Death { get; set; }
        public string[] Parrents { get; set; }
        public int? Spouse { get; set; }
        public string[] Childrens { get; set; }
        public double PositionX { get; set; }
        public double PositionY { get; set; }
    }
}
