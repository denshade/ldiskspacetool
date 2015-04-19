using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;


namespace Utils
{
    public class DuFileSystemCrawler
    {
        Dictionary<string, long> Sources;
        BackgroundWorker worker;
        
        public long currentSize = 1000;
        
        public DuFileSystemCrawler(BackgroundWorker worker)
        {
            Sources = new Dictionary<string, long>();
            this.worker = worker;
        }

        public Dictionary<string, long> getFilesystemFileDictionary(string SourceDirectory)
        {
            fillDirectoryMapping(SourceDirectory);
            return Sources;
        }

        private long fillDirectoryMapping(string p)
        {
            // 1.
            // Get array of all file names.
            string[] a = new string[0];
            try
            {
                a = Directory.GetFiles(p, "*.*");
            }
            catch (UnauthorizedAccessException e)
            {
            }
            // 2.
            // Calculate total bytes of all files in a loop.
            long b = 0;
            foreach (string name in a)
            {
                // 3.
                // Use FileInfo to get length of each file.
                FileInfo info = new FileInfo(name);
                b += info.Length;
                currentSize += info.Length;
                worker.ReportProgress((int)(currentSize / 1000));
            }

            string[] dirs = new string[0];
            try
            {
                dirs = Directory.GetDirectories(p, "*.*");
            }
            catch (UnauthorizedAccessException e)
            { 
            }
            foreach (string dir in dirs)
            {
                b += fillDirectoryMapping(dir);
            }
            // 4.
            // Return total size
            Sources.Add(p, b);
            return b;
        }

        static void DirSearch(string sDir)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        Console.WriteLine(f);
                    }
                    DirSearch(d);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }

};