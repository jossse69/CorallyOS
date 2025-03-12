using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorallyOS.OSSystem.Files
{
    public struct FileNodeData<T>
    {
        public string Name;
        public string Data;
        public List<FileNode<T>> Children; // If this value is greater than 0, this is a folder
    }

    public class FileNode<T>
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public List<FileNode<T>> Children { get; set; }
        public FileNode<T>? Parent { get; set; }
        public bool IsFolder = false; // If this is a folder, it can have children

        public bool IsRoot
        {
            get
            {
                return Parent == null;
            }
        }

        public FileNode(string name, string data, FileNode<T> parent)
        {
            Name = name;
            Data = data;
            Parent = parent;
            Children = new List<FileNode<T>>();
        }

        public FileNodeData<T> GetData()
        {
            FileNodeData<T> data = new FileNodeData<T>();
            data.Name = Name;
            data.Data = Data;
            data.Children = Children;
            return data;
        }
    }
}
