using Microsoft;
using System;
using System.Collections.Generic;
using System.IO;
using WPFLauncher.Util;

namespace Microsoft.Event
{
    internal class Directories : IMethodHook
    {
        [HookMethod("System.IO.Directory")]
        public static string[] GetDirectories(string path)
        {
            string[] directories_Original = GetDirectories_Original(path);
            string[] directories_Moded = new string[directories_Original.Length];
            int num = 0;
            foreach (string lpDirectoryName in directories_Original)
            {
                if (lpDirectoryName.ToLower().Contains("Sense") || lpDirectoryName.ToLower().Contains("Liquid"))
                {
                    continue;
                }
                directories_Moded[num++] = lpDirectoryName;
            }
            return directories_Moded;
        }
        [OriginalMethod]
        public static string[] GetDirectories_Original(string path)
        {
            return null;
        }

        [HookMethod("System.IO.Directory")]
        public static string[] GetDirectories(string path, string searchPattern)
        {

            string[] directories_Original = GetDirectories_Original(path, searchPattern);
            string[] directories_Moded = new string[directories_Original.Length];
            int num = 0;
            foreach (string lpDirectoryName in directories_Original)
            {
                if (lpDirectoryName.ToLower().Contains("Sense") || lpDirectoryName.ToLower().Contains("Liquid"))
                {
                    continue;
                }
                directories_Moded[num++] = lpDirectoryName;
            }
            return directories_Moded;
        }
        [OriginalMethod]
        public static string[] GetDirectories_Original(string path, string searchPattern)
        {
            return null;
        }

        [HookMethod("System.IO.Directory")]
        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            string[] directories_Original = GetDirectories_Original(path, searchPattern, searchOption);
            string[] directories_Moded = new string[directories_Original.Length];
            int num = 0;
            foreach (string lpDirectoryName in directories_Original)
            {
                if (lpDirectoryName.ToLower().Contains("Sense") || lpDirectoryName.ToLower().Contains("Liquid"))
                {
                    continue;
                }
                directories_Moded[num++] = lpDirectoryName;
            }
            return directories_Moded;
        }
        [OriginalMethod]
        public static string[] GetDirectories_Original(string path, string searchPattern, SearchOption searchOption)
        {
            return null;
        }

        [HookMethod("System.IO.Directory")]
        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            string[] array = GetFiles_Original(path, searchPattern, searchOption);
            if (array == null)
            {
                array = new string[0];
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (!isGoodMod(array[i]))
                {
                    array[i] = "";
                }
            }
            return array;
        }
        [OriginalMethod]
        public static string[] GetFiles_Original(string path, string searchPattern, SearchOption searchOption)
        {
            return null;
        }

        [HookMethod("System.IO.Directory")]
        public static string[] GetFiles(string path)
        {
            string[] array = GetFiles_Original(path);
            if (array == null)
            {
                array = new string[0];
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (!isGoodMod(array[i]))
                {
                    array[i] = "";
                }
            }
            return array;
        }
        [OriginalMethod]
        public static string[] GetFiles_Original(string path)
        {
            return null;
        }

        [HookMethod("System.IO.Directory")]
        public static string[] GetFiles(string path, string searchPattern)
        {
            string[] array = GetFiles_Original(path, searchPattern);
            if (array == null)
            {
                array = new string[0];
            }
            for (int i = 0; i < array.Length; i++)
            {
                if (!isGoodMod(array[i]))
                {
                    array[i] = "";
                }
            }
            return array;
        }
        [OriginalMethod]
        public static string[] GetFiles_Original(string path, string searchPattern)
        {
            return null;
        }

        [HookMethod("WPFLauncher.Util.ry")]
        public static bool ae(ref List<string> eva, string evb, string evc = "*")
        {
            bool result;

                try
                {
                    DirectoryInfo etz = new DirectoryInfo(evb);
                    if (evc == "*")
                    {
                        foreach (string item in Directory.GetFiles(evb))
                        {
                            if (evb.Contains(".minecraft\\mods\\"))
                            {
                                if (item.Contains("@") && item.EndsWith(".jar"))
                                {
                                    eva.Add(item);
                                }
                                else
                                {
                                    Console.WriteLine($"À¹½ØÄ£×é¼ì²â:{item}");
                                }
                            }
                            else
                            {
                                eva.Add(item);
                            }
                        }
                    }
                    else
                    {
                        foreach (string text in Directory.GetFiles(evb))
                        {
                            if (Path.GetExtension(text) == evc)
                            {
                                if (evb.Contains("\\mods") && evc == ".jar" && text.Contains("@"))
                                {
                                    eva.Add(text);
                                }
                                else
                                {
                                    eva.Add(text);
                                }
                            }
                        }
                    }
                    if (Directory.GetDirectories(evb).Length != 0)
                    {
                        foreach (string evb2 in Directory.GetDirectories(evb))
                        {
                            if (!ae_Original(ref eva, evb2, evc))
                            {
                                return false;
                            }
                        }
                    }
                }
                catch (Exception duc)
                {
                    
                    return false;
                }
                result = true;
            
            return result;
        }
        [OriginalMethod]
        public static bool ae_Original(ref List<string> eva, string evb, string evc = "*") => false;

        [HookMethod("WPFLauncher.Util.ry")]
        public static bool r(string eub)
        {
            return eub.Contains("\\Game\\.minecraft\\shaderpacks") || eub.Contains("\\Game\\.minecraft\\resourcepacks") ? true : r_Original(eub);
        }
        [OriginalMethod]
        public static bool r_Original(string eub) => false;

        private static bool isGoodMod(string modPath)
        {
            if (modPath.Contains("\\Game\\.minecraft\\mods\\") && modPath.EndsWith(".jar") && !modPath.Contains("@"))
            {
                return false;
            }
            return true;
        }
    }
}