using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MineImages
{
    class Program
    {
        static void Main(string[] args)
        {
            int rate = Convert.ToInt32(ConfigurationManager.AppSettings["rate"]);
            var destination = ConfigurationManager.AppSettings["dest"];

            using (var ctx = new PCSEntities())
            {
                var imgs = ctx.TransactionImages.Where(x => x.LicensePlate != null).Take(1000);

                foreach (var img in imgs)
                {
                    using (Image image = Image.FromStream(new MemoryStream(img.Image)))
                    {
                        Console.WriteLine(string.Format("{0}.jpg", img.ImageDate.ToString("yyyy-MM-dd_HH-mm-ss")));
                        image.Save(Path.Combine(destination,string.Format("{0}.jpg", img.ImageDate.ToString("yyyy-MM-dd_HH-mm-ss"))), ImageFormat.Jpeg);
                    }

                    Thread.Sleep(rate);
                }
            }
        }
    }
}
