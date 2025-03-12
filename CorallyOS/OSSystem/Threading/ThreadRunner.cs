using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorallyOS.OSSystem.Threading
{
    public class ThreadRunner
    {
        public List<LuaScriptThread> Threads = new List<LuaScriptThread>();

        public void AddThread(LuaScriptThread thread)
        {
            Threads.Add(thread);
        }

        public void RemoveThread(LuaScriptThread thread)
        {
            Threads.Remove(thread);
        }

        public LuaScriptThread GetThreadByName(string name)
        {
            foreach (LuaScriptThread thread in Threads)
            {
                if (thread.Name == name)
                {
                    return thread;
                }
            }
            return null;
        }

        public void RemoveThreadWithID(string id)
        {
            foreach (LuaScriptThread thread in Threads)
            {
                if (thread.Name == id)
                {
                    Threads.Remove(thread);
                    break;
                }
            }
        }

        public void InitializeThreads()
        {
            foreach (LuaScriptThread thread in Threads)
            {
                thread.Initialize();
            }
        }

        public void Tick()
        {
            foreach (LuaScriptThread thread in Threads)
            {
                if (thread.IsRunning)
                {
                    try
                    {
                        thread.Tick();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"An error occurred while ticking thread {thread.Name}: {e.Message}, stopping the thread now.");
                        thread.IsRunning = false;
                        thread.ErrorOccurred = true;
                    }
                }
            }
        }
    }
}
