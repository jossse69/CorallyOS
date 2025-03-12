using CorallyOS.OSSystem.Files;
using CorallyOS.OSSystem.Threading;
using NLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CorallyOS.OSSystem
{
    public class CorallySystem
    {
        public readonly static string Version = "0.0.1";
        public static ThreadRunner ThreadRunner = new ThreadRunner();
        public static FileTree FileTree = new FileTree();
        public static List<LuaTable> RegisteredLibs = new List<LuaTable>();
        public static Lua LuaInstance = new Lua();

        public void Initialize()
        {
            Console.WriteLine("CorallyOSProgram is starting...");
            Console.WriteLine("CorallyOSProgram has been started.");

            LuaInstance["CorallySystem"] = this;

            FileTree.LoadTree();

            // Check if the file tree is empty, if so, add a test file
            if (FileTree.Files.Count == 0)
            {
                FileTree.AddFile("system", "", true);
                FileTree.AddFile("system/scripts", "", true);

                FileTree.AddFile("system/hello.txt", "Hello, world!");
                FileTree.AddFile("system/startup_scripts.txt", "system/scripts/test1.lua\nsystem/scripts/test2.lua");
            }



            ThreadRunner.AddThread(new LuaScriptThread("TestThread", false, @"
                function on_thread_create(name)
                    if name == ThreadName then
                        CorallySystem:RegisterLibrary({
                            test = function()
                                print('I was defined in TestThread!')
                            end,
                            ID = 'TestLib'
                        })
                        CorallySystem:SetMetadata(name, 'initialized', 'true')
                    end
                end

                function on_thread_suspend(name)
                
                end

                function on_thread_resume(name)
                
                end

                function on_thread_tick(name)

                end
            "));

            ThreadRunner.AddThread(new LuaScriptThread("TestThread2", false, @"
                function on_thread_create(name)
                    if name == ThreadName then
                        CorallySystem:SetMetadata(name, 'initialized', 'true')
                    end
                end

                function on_thread_suspend(name)
                
                end

                function on_thread_resume(name)
                
                end

                function on_thread_tick(name)
                    local initialized = CorallySystem:GetMetadata(name, 'initialized')
                    local TestLib = CorallySystem:GetLibrary('TestLib') -- If the lib was got in the on create function, if TestLib should update, it should be updated here too
                    if initialized == 'true' and name == ThreadName then
                        if TestLib then
                            TestLib.test()
                        else
                            print('TestLib was not found!')
                        end
                    end
                end
            "));

            ThreadRunner.InitializeThreads();

            int tick = 0;
            while (true)
            {
                ThreadRunner.Tick();
                tick++;
                if (tick == 10)
                {
                    break;
                }
            }

            var testFile = FileTree.GetFile("system/startup_scripts.txt");
            if (testFile != null)
            {
                Console.WriteLine("I got the file's data! it is this:\n"+ testFile.Data);
            }

            FileTree.SaveTree();


            FileTree.PrintTreePretty(FileTree.Root, 0);
        }

        public void RegisterLibrary(LuaTable lib)
        {
            string libId = lib["ID"].ToString();
            var existingLib = RegisteredLibs.FirstOrDefault(l => l["ID"].ToString() == libId);
            if (existingLib != null)
            {
                RegisteredLibs.Remove(existingLib);
            }
            RegisteredLibs.Add(lib);
        }

        public void UnregisterLibrary(string id)
        {
            foreach (LuaTable lib in RegisteredLibs)
            {
                if (lib["ID"].ToString() == id)
                {
                    RegisteredLibs.Remove(lib);
                    break;
                }
            }
        }

        public LuaTable? GetLibrary(string id)
        {
            foreach (LuaTable lib in RegisteredLibs)
            {
                if (lib["ID"].ToString() == id)
                {
                    return lib;
                }
            }
            return null;
        }

        public string GetMetadata(string threadName, string key)
        {
            //Console.WriteLine($"Attempting to retrieve metadata {key} for thread {threadName}.");
            var thread = ThreadRunner.GetThreadByName(threadName);
            if (thread == null)
            {
                //Console.WriteLine($"Thread {threadName} not found.");
                return null;
            }
            var value = thread.GetMetadata(key);
            if (value == null)
            {
                //Console.WriteLine($"Metadata {key} for thread {threadName} is null.");
            }
            return value;
        }

        public void SetMetadata(string threadName, string key, string value)
        {
            //Console.WriteLine($"Attempting to set metadata {key} to {value} for thread {threadName}.");
            var thread = ThreadRunner.GetThreadByName(threadName);
            if (thread == null)
            {
                //Console.WriteLine($"Thread {threadName} not found.");
                return;
            }
            thread.SetMetadata(key, value);
        }
    }

}
