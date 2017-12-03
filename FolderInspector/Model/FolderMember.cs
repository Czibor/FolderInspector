using System;
using System.IO;

namespace FolderInspector
{
    public class FolderMember
    {
        public FolderMember(string filePath)
        {
            FilePath = filePath;
        }

        public bool IsFolder { get; private set; }
        public double SizeOnDisk { get; private set; }
        public DateTime CreationDate { get; private set; }
        public DateTime LastModified { get; private set; }
        public DateTime LastOpened { get; private set; }
        public string FileName { get; private set; }

        private string _filePath;
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                
                IsFolder = Directory.Exists(FilePath);
                SizeOnDisk = GetSize(FilePath, 1024 * 1024, IsFolder);
                CreationDate = File.GetCreationTime(FilePath);
                LastModified = File.GetLastWriteTime(FilePath);
                LastOpened = File.GetLastAccessTime(FilePath);
                FileName = Path.GetFileName(FilePath);
            }
        }
        
        private static double GetSize(string path, double divider, bool isFolder)
        {
            long size;

            if (isFolder)
            {
                size = GetFolderSize(new DirectoryInfo(path));
            }
            else
            {
                size = new FileInfo(path).Length;
            }

            if (size > 0)
            {
                return Math.Round(size / divider * 10) / 10;
            }
            else
            {
                return size;
            }
        }

        private static long GetFolderSize(DirectoryInfo di)
        {
            try
            {
                long size = 0;

                foreach (FileInfo file in di.GetFiles())
                {
                    size += file.Length;
                }
                
                foreach (DirectoryInfo directory in di.GetDirectories())
                {
                    size += GetFolderSize(directory);
                }

                return size;
            }
            catch
            {
                return -1;
            }
        }
    }
}