using System;
using System.IO;
using System.Runtime.Loader;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;

namespace ColdMint.scripts.mod;

/// <summary>
/// <para>Mod Loader</para>
/// <para>模组加载器</para>
/// </summary>
public class ModLoader
{
    /// <summary>
    /// <para>AssemblyLoadContext</para>
    /// <para>装配加载上下文</para>
    /// </summary>
    private static AssemblyLoadContext? _assemblyLoadContext;

    private static string[] _requiredDllList = new[] { Config.SolutionName };

    public static void Init()
    {
        //Initialize the context.
        //初始化上下文环境。
        LogCat.Log("initialize_the_context", LogCat.LogLabel.ModLoader);
        _assemblyLoadContext = AssemblyLoadContext.GetLoadContext(typeof(Godot.Bridge.ScriptManagerBridge).Assembly);
        if (_assemblyLoadContext == null)
        {
            LogCat.LogError("initialize_the_context_failed", LogCat.LogLabel.ModLoader);
            return;
        }

        var dllFolder = ResUtils.GetSelfDllFolder();
        if (dllFolder == null)
        {
            LogCat.LogError("get_dll_folder_failed", LogCat.LogLabel.ModLoader);
            return;
        }

        foreach (var requiredDll in _requiredDllList)
        {
            var dllPath = Path.Join(dllFolder, requiredDll + ".dll");
            //Load the necessary dll files.
            //加载必须的dll文件。
            if (!File.Exists(dllPath))
            {
                //When the dll that must be loaded does not exist, an error is reported immediately.
                //当必须加载的dll不存在时，立即报错。
                LogCat.LogErrorWithFormat("dll_not_exist", LogCat.LogLabel.ModLoader, true, dllPath);
                throw new NullReferenceException("dll not exist:" + dllPath);
            }

            //Load the dll.
            //加载dll。
            LogCat.LogWithFormat("load_dll", LogCat.LogLabel.ModLoader, true, dllPath);
            try
            {
                _assemblyLoadContext.LoadFromAssemblyPath(dllPath);
            }
            catch (ArgumentNullException argumentNullException)
            {
                //The assemblyPath parameter is null.
                //assemblyPath参数为空。
                LogCat.LogErrorWithFormat("load_dll_argument_null_exception", LogCat.LogLabel.ModLoader, true, dllPath);
                LogCat.WhenCaughtException(argumentNullException, LogCat.LogLabel.ModLoader);
                return;
            }
            catch (ArgumentException argumentException)
            {
                //Not an absolute path.
                //不是绝对路径
                LogCat.LogErrorWithFormat("load_dll_argument_exception", LogCat.LogLabel.ModLoader, true, dllPath);
                LogCat.WhenCaughtException(argumentException, LogCat.LogLabel.ModLoader);
                return;
            }
            catch (FileLoadException fileLoadException)
            {
                //A file that was found could not be loaded.
                //无法加载找到的文件。
                LogCat.LogErrorWithFormat("load_dll_file_load_exception", LogCat.LogLabel.ModLoader, true, dllPath);
                LogCat.WhenCaughtException(fileLoadException, LogCat.LogLabel.ModLoader);
                return;
            }
            catch (BadImageFormatException badImageFormatException)
            {
                //assemblyPath is not a valid assembly.
                //assemblyPath不是有效的程序集。
                LogCat.LogErrorWithFormat("load_dll_bad_image_format_exception", LogCat.LogLabel.ModLoader, true,
                    dllPath);
                LogCat.WhenCaughtException(badImageFormatException, LogCat.LogLabel.ModLoader);
                return;
            }

            //Loading the dll succeeded.
            //加载dll成功。
            LogCat.LogWithFormat("load_dll_success", LogCat.LogLabel.ModLoader, true, dllPath);
        }
    }

    /// <summary>
    /// <para>Load a module for a directory</para>
    /// <para>加载某个目录的模组</para>
    /// </summary>
    /// <param name="modFolderPath">
    ///<para>Mod path</para>
    ///<para>模组路径</para>
    /// </param>
    public static void LoadMod(string modFolderPath)
    {
        if (!Directory.Exists(modFolderPath))
        {
            //The module folder does not exist.
            //模组文件夹不存在。
            LogCat.LogErrorWithFormat("mod_folder_does_not_exist", LogCat.LogLabel.ModLoader, true, modFolderPath);
            return;
        }

        try
        {
            var modManifest =
                ModManifest.CreateModManifestFromPath(Path.Join(modFolderPath, Config.ModManifestFileName));
        }
        catch (FileNotFoundException fileNotFoundException)
        {
            //Do not continue to load the file when it does not exist.
            //当文件不存在时就不要继续加载了。
            LogCat.WhenCaughtException(fileNotFoundException, LogCat.LogLabel.ModLoader);
            return;
        }
    }
}