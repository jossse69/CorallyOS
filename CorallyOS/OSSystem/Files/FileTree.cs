using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CorallyOS.OSSystem.Files
{
    public class FileTree
    {
        public List<FileNode<object>> Files = new List<FileNode<object>>();
        public FileTree()
        {

        }

        public FileNode<object>? Root
        {
            get
            {
                return Files.Count > 0 ? Files[0] : null;
            }
        }

        public FileNode<object>? GetFile(string path)
        {
            string[] pathParts = path.Split('/');
            FileNode<object>? current = null;
            foreach (string part in pathParts)
            {
                if (current == null)
                {
                    foreach (FileNode<object> file in Files)
                    {
                        if (file.Name == part)
                        {
                            current = file;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (FileNode<object> file in current.Children)
                    {
                        if (file.Name == part)
                        {
                            current = file;
                            break;
                        }
                    }
                }
            }
            return current;
        }

        public string GetFileExtension(string path)
        {
            string[] pathParts = path.Split('.');
            return pathParts[pathParts.Length - 1];
        }

        public string GetFileName(string path)
        {
            string[] pathParts = path.Split('/');
            return pathParts[pathParts.Length - 1];
        }

        public string GetFileNameWithoutExtension(string path)
        {
            string[] pathParts = GetFileName(path).Split('.');
            return pathParts[0];
        }

        public FileNode<object>? AddFile(string path, string data, bool isFolder = false)
        {
            string[] pathParts = path.Split('/');
            FileNode<object>? current = null;
            foreach (string part in pathParts)
            {
                if (current == null)
                {
                    foreach (FileNode<object> file in Files)
                    {
                        if (file.Name == part)
                        {
                            current = file;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (FileNode<object> file in current.Children)
                    {
                        if (file.Name == part)
                        {
                            current = file;
                            break;
                        }
                    }
                }
            }
            FileNode<object>? result = null;
            if (current != null)
            {
                var existingNode = current.Children.FirstOrDefault(f => f.Name == GetFileName(path));
                if (existingNode != null)
                {
                    if (existingNode.IsFolder && isFolder)
                    {
                        // Merge folders
                        foreach (var child in existingNode.Children)
                        {
                            current.Children.Add(child);
                        }
                    }
                    else
                    {
                        // Replace file
                        current.Children.Remove(existingNode);
                        result = new FileNode<object>(GetFileName(path), data, current);
                        result.IsFolder = isFolder;
                        current.Children.Add(result);
                    }
                }
                else
                {
                    result = new FileNode<object>(GetFileName(path), data, current);
                    result.IsFolder = isFolder;
                    current.Children.Add(result);
                }
            }
            else
            {
                result = new FileNode<object>(GetFileName(path), data, null);
                result.IsFolder = isFolder;
                Files.Add(result);
            }

            return result;
        }


        public void SaveTree()
        {
            List<FileNodeData<object>> data_list = new List<FileNodeData<object>>();
            foreach (FileNode<object> file in Files)
            {
                var data = file.GetData();
                data_list.Add(data);
            }

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(data_list, settings);
            System.IO.File.WriteAllText("filetree.json", json);
        }

        public void PrintTreePretty(FileNode<object> node, int depth)
        {
            // Print the loaded tree in a pretty way, for debugging purposes
            string indent = new string(' ', depth * 2);
            Console.WriteLine(indent + node.Name);
            foreach (var child in node.Children)
            {
                PrintTreePretty(child, depth + 1);
            }
        }

        public void LoadTree()
        {
            string json = System.IO.File.ReadAllText("filetree.json");
            var files_loaded = JsonConvert.DeserializeObject<List<FileNode<object>>>(json);

            if (files_loaded != null)
            {
                Files.Clear();
                foreach (var fileData in files_loaded)
                {
                    ReconstructTree(fileData, null);
                }
            }
        }

        private void ReconstructTree(FileNode<object> fileData, FileNode<object>? parent)
        {
            var newNode = new FileNode<object>(fileData.Name, fileData.Data, parent)
            {
                IsFolder = fileData.IsFolder
            };

            if (parent == null)
            {
                Files.Add(newNode);
            }
            else
            {
                parent.Children.Add(newNode);
            }

            foreach (var child in fileData.Children)
            {
                ReconstructTree(child, newNode);
            }
        }
    }
}
