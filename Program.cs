using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.CompilerServices;

namespace CountLogFilesAvailable
{
    class Program
    {
        private const int NUMBER_OF_DAYS = -7;
        static private DateTime dtRange = DateTime.Today.AddDays(NUMBER_OF_DAYS);

        static void Main(string[] args)
        {
            string inputFolder = string.Empty;
            Console.WriteLine("Please enter folder: ");
            inputFolder = Console.ReadLine();

            if (Directory.Exists(inputFolder))
            {
                DisplayResult(CountLogFiles(inputFolder));
            }
            else
                Console.WriteLine("Path not exist!!"); 
            Console.ReadLine(); 
        }

        //Store the created date for all the log files 
        static private List<FileDate> CountLogFiles(string inputFolder)
        {
            
            List<FileDate> filesDate = new List<FileDate>();
            filesDate.AddRange(
                            (from f in Directory.GetFiles(inputFolder)
                             where f.Contains(".log")
                             select f)
                                .Select(t =>
                                            new FileDate
                                            {
                                                fileDateTime = File.GetCreationTime(t).Date
                                            }
                                        ).OrderBy(o => o.fileDateTime).Where(d => d.fileDateTime >= dtRange)
            );
            return filesDate;
        }

        //Display the result
        static private void DisplayResult(List<FileDate> filesDate)
        {
            var results = from f in filesDate
                          group f by f.fileDateTime into dateGroup
                          select new
                          {
                              dt = dateGroup.Key,
                              count = dateGroup.Count()
                          };

            for (DateTime dt = dtRange; dt <= DateTime.Today; dt = dt.AddDays(1))
            {
                var res = from r in results
                          where r.dt == dt
                          select r.count;

                if (res.Any())
                    Console.WriteLine("{0} : {1} {2}", dt.Date.ToString("yyyy-MM-dd"), res.First(), res.First() > 1 ? "files" : "file");
                else
                    Console.WriteLine("{0} : 0 file", dt.Date.ToString("yyyy-MM-dd"));
            }
        }
    }

    //Class to store the File Creation DateTime.
    class FileDate
    {
        public DateTime fileDateTime { get; set; } 
    }
}
