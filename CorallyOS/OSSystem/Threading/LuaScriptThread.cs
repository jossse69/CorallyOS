using NLua;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CorallyOS.OSSystem.Threading
{
    public class LuaScriptThread
    {
        public string Name { get; set; }
        public bool IsBackground { get; set; }
        public bool IsSuspended { get; set; }
        public bool IsRunning { get; set; }
        public bool ErrorOccurred { get; set; }
        public Lua LuaInstance { get; set; }
        private string Code { get; set; }
        public Dictionary<string, string> Metadata { get; private set; }

        public LuaScriptThread(string name, bool isBackground, string code)
        {
            Name = name;
            IsBackground = isBackground;
            IsSuspended = false;
            IsRunning = true;
            ErrorOccurred = false;
            LuaInstance = CorallySystem.LuaInstance;
            Code = code;
            Metadata = new Dictionary<string, string>();
        }

        public void Initialize()
        {
            LuaInstance.DoString(Code);
            LuaInstance["ThreadName"] = Name;
            LuaInstance.GetFunction("on_thread_create")?.Call(Name);
        }

        public void Suspend()
        {
            IsSuspended = true;
            IsRunning = false;
            LuaInstance["ThreadName"] = Name;
            LuaInstance.GetFunction("on_thread_suspend")?.Call(Name);
        }

        public void Resume()
        {
            IsSuspended = false;
            IsRunning = true;
            LuaInstance["ThreadName"] = Name;
            LuaInstance.GetFunction("on_thread_resume")?.Call(Name);
        }

        public void Tick()
        {
            if (IsRunning && !IsSuspended)
            {
                LuaInstance["ThreadName"] = Name;
                LuaInstance.GetFunction("on_thread_tick")?.Call(Name);
            }
        }

        public string? GetMetadata(string key)
        {
            if (Metadata.TryGetValue(key, out string? value))
            {
                //Console.WriteLine($"Metadata {key} retrieved for thread {Name}.");
                return value;
            }
            return null;
        }

        public void SetMetadata(string key, string value)
        {
            if (Metadata.ContainsKey(key))
                Metadata[key] = value;
            else
                Metadata.Add(key, value);

            //Console.WriteLine($"Metadata {key} set to {value} for thread {Name}.");
        }
    }
}
