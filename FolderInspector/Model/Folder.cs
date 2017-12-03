using System.Collections.Generic;

namespace FolderInspector
{
    public class Folder
    {
        public Folder(string folderPath, List<FolderMember> folderMembers)
        {
            FolderPath = folderPath;
            FolderMembers = folderMembers;
        }

        public string FolderPath { get; set; }
        public List<FolderMember> FolderMembers { get; set; }
    }
}