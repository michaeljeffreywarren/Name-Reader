using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace NameReader
{
    //Requirements clarification:
    //1. Attached pdf required file to be built into string
    //2. Email instructions stated "Read file"

    //I opted to go with #2
    class Program
    {
        private static List<Dictionary<string, string>> recs = null;
        private static List<PersonRecord> persons = null;

        static void Main(string[] args)
        {
            //read file into memory
            ReadFileIntoMemory();

            //dynamically set to PersonRecord
            ParseFieldsIntoObjectList();

            //write to console to prove process worked correctly
            WriteRecordsToConsole();
        }

        static void ReadFileIntoMemory()
        {
            string line, fieldName, fieldValue;
            const int openPIndex = 0;
            int closingPIndex, stateIndex = -1;
            StreamReader file = new StreamReader(@"c:\SampleInputFile.txt");

            recs = null;
            Dictionary<string, string> dict = null;

            while ((line = file.ReadLine()) != null)
            {
                if (line == "")
                {
                    recs = recs ?? new List<Dictionary<string, string>>();
                    if (dict != null && dict.Count > 0)
                        recs.Add(dict);
                    dict = new Dictionary<string, string>();
                    dict.Clear();
                }
                else if (line[openPIndex] == '(')
                {
                    closingPIndex = line.IndexOf(')');
                    fieldName = line.Substring(openPIndex + 1, closingPIndex - 1).Trim();
                    fieldValue = line.Substring(closingPIndex + 1).Trim();

                    if (dict == null)
                    {
                        dict = new Dictionary<string, string>();
                    }

                    //check for state
                    if (fieldValue.IndexOf(',') > -1)
                    {
                        stateIndex = fieldValue.IndexOf(',');
                        dict.Add("City", fieldValue.Substring(0, stateIndex).Trim());
                        dict.Add("State", fieldValue.Substring(stateIndex + 1).Trim());
                    }
                    else
                    {
                        dict.Add(fieldName, fieldValue);
                    }
                }
            }

            recs.Add(dict);

            file.Close();
        }

        static void ParseFieldsIntoObjectList()
        {
            PersonRecord pr = new PersonRecord();
            persons = new List<PersonRecord>();

            Type myType = pr.GetType();
            IList<PropertyInfo> props = new List<PropertyInfo>(myType.GetProperties());

            foreach (Dictionary<string, string> personRecord in recs)
            {
                pr = new PersonRecord();
                foreach (PropertyInfo prop in props)
                {
                    if (personRecord.ContainsKey(prop.Name))
                    {
                        object ob = personRecord[prop.Name];
                        prop.SetValue(pr, Convert.ChangeType(ob, prop.PropertyType), null);
                    }
                }
                persons.Add(pr);
            }
        }

        static void WriteRecordsToConsole()
        {
            foreach(PersonRecord pr in persons)
            {
                Console.WriteLine("Name: " + pr.Name);
                Console.WriteLine("Age: " + pr.Age);
                Console.WriteLine("Flags: " + pr.Flags);
                Console.WriteLine("City: " + pr.City);
                Console.WriteLine("State: " + pr.State);

                Console.WriteLine("\n");
            }

            Console.WriteLine("Press ENTER to conclude.");
            Console.Read();
        }
    }
}
