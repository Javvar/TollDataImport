using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConsoleApplication3
{
    class Program
    {
        static List<Lane> Lanes = new List<Lane>();

        static void Main(string[] args)
        {
            var d = IgnoreList.Ignore("Plazas/Quagga/Lanes/10MS/TLC/MOP/Printer");

            
            Console.ReadLine();
        }

        class Lane
        {
            private string Code;
            public List<string> Items = new List<string>();

            public Lane(string code)
            {
                Code = code;
            }
        }
    }

    public static class IgnoreList
    {
        static List<Lane> Lanes = new List<Lane>();

        static IgnoreList()
        {
            XDocument doc = XDocument.Load(@"E:\TFS\TPS\Services\TollDataImport\ConsoleApplication3\ConsoleApplication3\IgnoreList.xml");
            var lanes = doc.Descendants("Lanes");

            foreach (var lane in lanes.Descendants("Lane"))
            {
                var laneItem = new Lane(lane.Attribute("code").Value);

                foreach (var item in lane.Descendants())
                {
                    laneItem.Items.Add(item.Name.ToString());
                }

                Lanes.Add(laneItem);
            }
        }

        public static bool Ignore(string status)
        {
            var tokens = status.Split('/').ToArray();

            return Lanes.Any(x => x.Code == tokens[Array.IndexOf(tokens, "Lanes") + 1] &&
                                  x.Items.Contains(tokens[tokens.Length - 1]));
        }

        class Lane
        {
            public string Code;
            public List<string> Items = new List<string>();

            public Lane(string code)
            {
                Code = code;
            }
        }
    }




}
