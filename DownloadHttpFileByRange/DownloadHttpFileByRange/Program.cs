using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace DownloadHttpFileByRange
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var numberOfThreads = 3;

            string url = "https://download.microsoft.com/download/8/D/D/8DD7BDBA-CEF7-4D8E-8C16-D9F69527F909/ENU/x64/SQLEXPRADV_x64_ENU.exe";

            string filename = string.Empty;
            filename = Path.GetFileName(new Uri(url).LocalPath);

            var files = new List<string>();

            var tasks = new List<Task>();

            long totalSize = CheckFile(url);
            if (totalSize > 0)
            {
                long threadSize = totalSize / numberOfThreads;

                Console.WriteLine($"Total file size: {totalSize} Bytes");
                Console.WriteLine($"File size by thread: {threadSize} Bytes");
                Console.WriteLine("\n");
                for (var i = 0; i < numberOfThreads; i++)
                {
                    var from = (i * threadSize);
                    var to = from + threadSize - 1;
                    if (i == numberOfThreads - 1)
                    {
                        to = totalSize;
                    }

                    Console.WriteLine($"Thread# {i + 1} Size: {threadSize}, Bytes range from: {from} - {to}");

                    var file = $@"C:\Temp\HttpDownloadByRange\~{filename}_{(i + 1) }.tmp";
                    files.Add(file);

                    tasks.Add(Task.Factory.StartNew(() => DownloadPartialFile(url, from, to, file)));
                }

                Task.WaitAll(tasks.ToArray());
                ConcatenateFiles($@"C:\Temp\HttpDownloadByRange\{filename}", files.ToArray());

                stopWatch.Stop();
                Console.WriteLine($"Time Elapsed: {stopWatch.Elapsed}");
            }
            else
            {
                Console.WriteLine("The file to download is empty. (0 bytes)");
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public static void DownloadPartialFile(string url, long from, long to, string fileName)
        {
            /* Create an Http Request */
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.KeepAlive = false;
            httpRequest.AddRange(from, to);

            /* Establish Return Communication with the Http Server */
            using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
            {
                /* Get the Http Server's Response Stream */
                using (var ftpStream = httpResponse.GetResponseStream())
                {
                    /* Open a File Stream to Write the Downloaded File */
                    using (FileStream localFileStream = new FileStream(fileName, FileMode.Create))
                    {
                        ftpStream.CopyTo(localFileStream, 8192);
                    }
                }
            }
            httpRequest = null;
        }

        public static long CheckFile(string url)
        {
            long fileSize = 0;
            /* Create an Http Request */
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);

            /* Establish Return Communication with the Http Server */
            using (var httpResponse = (HttpWebResponse)httpRequest.GetResponse())
            {
                fileSize = httpResponse.ContentLength;
            }
            httpRequest = null;

            return fileSize;
        }

        private static void ConcatenateFiles(string outputFile, params string[] inputFiles)
        {
            using (Stream output = File.OpenWrite(outputFile))
            {
                foreach (string inputFile in inputFiles)
                {
                    using (Stream input = File.OpenRead(inputFile))
                    {
                        input.CopyTo(output);
                    }
                }
            }
        }
    }
}
