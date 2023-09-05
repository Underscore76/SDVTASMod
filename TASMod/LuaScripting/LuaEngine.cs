using System;
using System.IO;
using System.Reflection;
using System.Text;
using NLua;
using NLua.Exceptions;
using StardewValley.Characters;
using TASMod.System;

namespace TASMod.LuaScripting
{
    public class LuaEngine
    {
        public static Lua LuaState = null;

        public static void Init()
        {
            if (LuaState != null) LuaState.Close();
            LuaState = new Lua();
            LuaState.LoadCLRPackage();
        }

        public static void SetupImports()
        {
            // import namespaces for core MonoGame types
            LuaState.DoString(@"
                import ('System')
                import ('MonoGame.Framework', 'Microsoft.Xna.Framework')
                import ('MonoGame.Framework', 'Microsoft.Xna.Framework.Graphics')
                import ('MonoGame.Framework', 'Microsoft.Xna.Framework.Input')
                import ('MonoGame.Framework', 'Microsoft.Xna.Framework.Audio')
            ");

            LuaState.DoString(@"
            ");
        }

        public static void LoadAllFiles()
        {
            Path.Join(ModEntry.Instance.Helper.DirectoryPath, "assets", "lua", "?.lua");
            string scriptsFiles = Path.Join(Constants.ScriptsPath, "?.lua");
            string packagedFiles = Path.Join(ModEntry.Instance.Helper.DirectoryPath, "assets", "lua", "?.lua");
            string path = $"package.path = package.path .. ';{packagedFiles};{scriptsFiles}'";
            path = path.Replace("\\", "\\\\");
            var bytes = Encoding.ASCII.GetBytes(path.ToCharArray());
            LuaState.DoString(bytes);
        }

        public static void RegisterEnums()
        {
            LuaState["Keys"] = Activator.CreateInstance(typeof(Microsoft.Xna.Framework.Input.Keys));
        }

        public static void RegisterStaticClasses()
        {
            LuaState["Controller"] = Activator.CreateInstance(typeof(Controller));
            LuaState.RegisterFunction("RunCS", typeof(CSInterpreter).GetMethod("Eval"));
            LuaState.RegisterFunction("GetValue", typeof(Reflector).GetMethod("GetDynamicCastField"));
        }

        public static string Reload()
        {
            try
            {
                Init();
                SetupImports();
                LoadAllFiles();
                RegisterEnums();
                RegisterStaticClasses();

                LuaState["interface"] = new ScriptInterface();
                LuaState.DoString(@"require('prelaunch')");

                return "";
            }
            catch (LuaScriptException e)
            {
                string err = e.Message;
                if (e.InnerException != null)
                    err += "\n\t" + e.InnerException.Message;
                return err;
            }
        }

        public static string RunString(string command, bool cs=false)
        {
            try
            {
                string message = "";
                switch (command)
                {
                    case "reload":
                        message = Reload();
                        break;
                    default:
                        object[] results = LuaState.DoString(Encoding.ASCII.GetBytes(command));
                        foreach (var r in results)
                        {
                            if (r == null)
                                message += "null\t";
                            else
                                message += string.Format("{0}\t", r.ToString());
                        }
                        break;
                }
                return message;

            }
            catch (LuaScriptException e)
            {
                string err = e.Message;
                if (e.InnerException != null)
                    err += "\n\t" + e.InnerException.Message;
                return err;
            }
            catch (TypeInitializationException e)
            {
                string err = e.Message;
                if (e.InnerException != null)
                    err += "\n\t" + e.InnerException.Message;
                return err;
            }
            catch (Exception e)
            {
                string err = e.Message;
                if (e.InnerException != null)
                    err += "\n\t" + e.InnerException.Message;
                return err;
            }
        }
    }
}