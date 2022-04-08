using System;
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
                person.FirstPurrent = string.IsNullOrEmpty(line[4]) ? null : line[4];
                person.SecondPurrent = string.IsNullOrEmpty(line[5]) ? null : line[5];

                peoples[i] = person;
            }

            return peoples;
        }

        public Person[] ReadJsonPerson(string path)
        {
            string jsonPerson = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Person[]>(jsonPerson);
        }
    }

    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birth { get; set; }
        public DateTime? Death { get; set; }
        public string? FirstPurrent { get; set; }
        public string? SecondPurrent { get; set; }
    }
}
