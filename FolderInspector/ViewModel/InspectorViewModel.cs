using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using FolderInspector;

namespace FolderInspectorViewModel
{
    public class InspectorViewModel : ObservableObject, IDataErrorInfo
    {
        private bool _isReady = true;
        public bool IsReady
        {
            get
            {
                return _isReady;
            }
            set
            {
                _isReady = value;
                NotifyPropertyChanged("IsReady");
            }
        }

        public bool CanGoBack
        {
            get
            {
                return PathIndex != 0;
            }
        }
        
        public bool CanGoForward
        {
            get
            {
                return PathIndex < pathHistory.Count - 1;
            }
        }
        
        private int _pathIndex;
        public int PathIndex
        {
            get
            {
                return _pathIndex;
            }
            set
            {
                _pathIndex = value;

                NotifyPropertyChanged("CanGoBack");
                NotifyPropertyChanged("CanGoForward");
                NotifyPropertyChanged("FolderPath");
                NotifyPropertyChanged("FolderMembers");
            }
        }
        
        public string FolderPath
        {
            get
            {
                if (pathHistory.Count != 0)
                {
                    return pathHistory[PathIndex].FolderPath;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (FolderPath != value)
                {
                    GoToFolder(value);
                }
            }
        }
        
        public List<FolderMember> FolderMembers
        {
            get
            {
                if (pathHistory.Count != 0)
                {
                    return pathHistory[PathIndex].FolderMembers;
                }
                else
                {
                    return new List<FolderMember>();
                }
            }
        }

        private List<Folder> pathHistory = new List<Folder>();

        private RelayCommand _folderDialog;
        public ICommand FolderDialog
        {
            get
            {
                return _folderDialog ?? (_folderDialog = new RelayCommand(param => ShowFolderDialog()));
            }
        }

        private RelayCommand _goTo;
        public ICommand GoTo
        {
            get
            {
                return _goTo ?? (_goTo = new RelayCommand(param => GoToFolder(param)));
            }
        }

        private RelayCommand _goBack;
        public ICommand GoBack
        {
            get
            {
                return _goBack ?? (_goBack = new RelayCommand(param => GoToIndex(PathIndex - 1)));
            }
        }

        private RelayCommand _goForward;
        public ICommand GoForward
        {
            get
            {
                return _goForward ?? (_goForward = new RelayCommand(param => GoToIndex(PathIndex + 1)));
            }
        }

        private RelayCommand _open;
        public ICommand Open
        {
            get
            {
                return _open ?? (_open = new RelayCommand(param => OpenFileOrFolder(param)));
            }
        }

        private void ShowFolderDialog()
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select folder";
                fbd.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    FolderPath = fbd.SelectedPath;
                }
            }
        }

        private void GoToIndex(int newPathIndex)
        {
            PathIndex = newPathIndex;
        }
        
        private void GoToFolder(object path)
        {
            Task.Run(() =>
            {
                IsReady = false;
                List<FolderMember> tempList = new List<FolderMember>();
                string requestedPath = null;

                try
                {
                    requestedPath = path.ToString();
                    List<string> filesAndFolders = Directory.GetDirectories(requestedPath).ToList();
                    filesAndFolders.AddRange(Directory.GetFiles(requestedPath).ToList());

                    foreach (string childPath in filesAndFolders)
                    {
                        tempList.Add(new FolderMember(childPath));
                    }
                }
                catch
                {
                    tempList = new List<FolderMember>();
                }
                finally
                {
                    AddFolderToList(new Folder(requestedPath, tempList));
                    PathIndex = pathHistory.Count - 1;
                    IsReady = true;
                }
            });
        }
        
        private void AddFolderToList(Folder folder)
        {
            if (PathIndex < pathHistory.Count - 1)
            {
                pathHistory.RemoveRange(PathIndex + 1, pathHistory.Count - (PathIndex + 1));
            }

            if (pathHistory.Count == 10)
            {
                pathHistory.RemoveAt(0);
            }

            pathHistory.Add(folder);
        }

        private void OpenFileOrFolder(object path)
        {
            try
            {
                Process.Start(path.ToString());
            }
            catch
            {
                Process.Start(Path.GetDirectoryName(path.ToString()));
            }
        }

        public string Error { get { return string.Empty; } }
        
        public string this[string propertyName]
        {
            get
            {
                string errorMessage = null;

                if (propertyName == "FolderPath")
                {
                    if (!string.IsNullOrEmpty(FolderPath))
                    {
                        errorMessage = ValidateFolder(FolderPath);
                    }
                }

                return errorMessage;
            }
        }

        private static string ValidateFolder(string folder)
        {
            try
            {
                if (Path.GetFullPath(folder) != folder)
                {
                    return "The folder name is not valid.";
                }
            }
            catch
            {
                return "The folder name is not valid.";
            }

            if (Directory.Exists(folder))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);

                try
                {
                    DirectorySecurity acl = directoryInfo.GetAccessControl();
                    return null;
                }
                catch (UnauthorizedAccessException uae)
                {
                    if (uae.Message.ToUpper().Contains("READ-ONLY"))
                    {
                        return null;
                    }
                    else
                    {
                        return "You don't have read access to the given folder.";
                    }
                }
                catch
                {
                    return "The folder can't be reached by the program.";
                }
            }
            else
            {
                return "The folder doesn't exist or can't be reached.";
            }
        }
    }
}