
using CorallyOS.OSSystem;

namespace CorallyOS;
public static class CorallyOSProgram
{
    public static CorallySystem CorallySystem = new CorallySystem();
    public static void Main()
    {

        // Create the filetree.json file if it doesn't exist
        if (!System.IO.File.Exists("filetree.json"))
        {
            System.IO.File.WriteAllText("filetree.json", "[]");
        }

        CorallySystem.Initialize();
    }
}