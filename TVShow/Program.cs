using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Xml.Linq;

namespace TVShow
{
    class Program
    {

        static void Main(string[] args)
        {
            string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string Diagonal = Convert.ToChar(92).ToString();
            string IsDebug = "bin" + Diagonal + "Debug" + Diagonal;
            string IsRelease = "bin" + Diagonal + "Release" + Diagonal;
            string FullPathFile = BaseDirectory.Replace(IsDebug, "").Replace(IsRelease, "") + "TVGuide.xml";

            if (!File.Exists(FullPathFile)) 
            {
                new XDocument(new XElement("shows"));
            }

            Boolean favorites = false;
            ListShows(FullPathFile, favorites);

            string command = string.Empty;
            while (!command.Equals("exit")) 
            {
                command = Console.ReadLine().ToLower().Trim();
                switch (command) 
                {
                    case "list":
                        favorites = false;
                        ListShows(FullPathFile, favorites);
                        break;

                    case "favorites":
                        favorites = true;
                        ListShows(FullPathFile, favorites);
                        break;

                    default:
                        int id = 0;
                        Boolean isNumber = int.TryParse(command, out id);
                        if (isNumber) { UpdateShow(FullPathFile, id); }
                        ListShows(FullPathFile, favorites);
                        break;
                }
            }
        }

        private static void ListShows(string FullPathFile, Boolean favorites = false) 
        {
            string Tab = Convert.ToChar(09).ToString();
            string line;

            Console.Clear();

            // Instructions
            line = "Welcome to the list of shows.";
            Console.WriteLine(line);
            Console.WriteLine("");

            line = "Instructions:";
            Console.WriteLine(line);
            line = Tab + "Type(list) to display the list of available shows.";
            Console.WriteLine(line);
            line = Tab + "Type(favorites) to show only shows marked as favorites.";
            Console.WriteLine(line);
            line = Tab + "Type (show number) to mark or unmark a show as a favorite.";
            Console.WriteLine(line);
            line = Tab + "Type(Exit) to end.";
            Console.WriteLine(line);
            Console.WriteLine("");

            var xlm = XDocument.Load(FullPathFile);
            
            var query = from c in xlm.Root.Descendants("show")
                        orderby (string) c.Element("name")
                        select c.Element("id").Value 
                        + "|" + c.Element("name").Value 
                        + "|" + c.Element("favourite").Value;

            line = "==================================";
            Console.WriteLine(line);
            line = "Id" + Tab + "Name" + Tab +"Favourite (*)";
            Console.WriteLine(line);
            line = "==================================";
            Console.WriteLine(line);
           
            if (query.Count() > 0)
            {
                if (favorites)
                {
                    foreach (string row in query)
                    {
                        string[] item = row.Split('|');
                        if (item[2].Equals("1")) 
                        {
                            line = item[0] + Tab + item[1] + " *";
                            Console.WriteLine(line);
                        }
                    }
                }
                else 
                {
                    foreach (string row in query)
                    {
                        string[] item = row.Split('|');
                        line = item[0] + Tab + item[1];
                        if (item[2].Equals("1")) { line += " * "; }
                        Console.WriteLine(line);
                    }
                }
            }
            else 
            {
                Console.WriteLine("Show list empty");
            }
            line = "----------------------------------";
            Console.WriteLine(line);
            Console.WriteLine("");

            line = "Type your command:";
            Console.WriteLine(line);
        }

        private static void UpdateShow(string FullPathFile, int id)
        {
            Boolean updated = false;

            var xlm = XDocument.Load(FullPathFile);

            var items = from item in xlm.Descendants("show")
                        where item.Element("id").Value == id.ToString()
                        select item;

            if (items != null) 
            {
                foreach (XElement itemElement in items)
                {

                    if (itemElement.Element("favourite").Value.Equals("1")) 
                    { 
                        itemElement.Element("favourite").Value = "0"; 
                    }
                    else 
                    { 
                        itemElement.Element("favourite").Value = "1"; 
                    }
                }
                updated = true;
            }
            xlm.Save(FullPathFile);

            if (updated) { Console.WriteLine("Show updated."); }

            Console.ReadKey();
        }

    }


  



}
