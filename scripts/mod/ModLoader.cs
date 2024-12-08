using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
using Godot;

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

    private static readonly string[] RequiredDllList = [Config.SolutionName];

    /// <summary>
    /// <para>Initializes the mod loader</para>
    /// <para>初始化模组加载器</para>
    /// </summary>
    /// <exception cref="FileNotFoundException">
    ///<para>This exception is thrown if the built-in dll file cannot be found when it is loaded.</para>
    ///<para>如果加载内置dll文件时，找不到文件，则抛出此异常。</para>
    /// </exception>
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

        foreach (var requiredDll in RequiredDllList)
        {
            var dllPath = Path.Join(dllFolder, requiredDll + ".dll");
            //Load the necessary dll files.
            //加载必须的dll文件。
            if (!File.Exists(dllPath))
            {
                //When the dll that must be loaded does not exist, an error is reported immediately.
                //当必须加载的dll不存在时，立即报错。
                LogCat.LogErrorWithFormat("dll_not_exist", LogCat.LogLabel.ModLoader, dllPath);
                throw new FileNotFoundException("dll not exist:" + dllPath);
            }

            LoadDllFile(dllPath);
        }
    }

    /// <summary>
    /// <para>Load Dll file</para>
    /// <para>加载Dll文件</para>
    /// </summary>
    /// <param name="dllPath">
    ///<para>dll file path</para>
    ///<para>dll的文件路径</para>
    /// </param>
    /// <exception cref="NullReferenceException">
    ///<para>Throw this error if the assemblyLoadContext has not been initialized.</para>
    ///<para>如果assemblyLoadContext尚未初始化，那么抛出此错误。</para>
    /// </exception>
    private static void LoadDllFile(string dllPath)
    {
        if (_assemblyLoadContext == null)
        {
            throw new InvalidOperationException("assemblyLoadContext is null.");
        }

        //Load the dll.
        //加载dll。
        LogCat.LogWithFormat("load_dll", LogCat.LogLabel.ModLoader, dllPath);
        try
        {
            var assembly = _assemblyLoadContext.LoadFromAssemblyPath(dllPath);
            var assemblyName = assembly.GetName().Name;
            if (assemblyName == null)
            {
                return;
            }

            LogCat.LogWithFormat("dll_name", LogCat.LogLabel.ModLoader, assemblyName);
            //If the load is not its own Dll file.
            //如果加载的不是自身的Dll文件.
            if (assemblyName == Config.SolutionName)
            {
                return;
            }

            //Call the method of the entry class.
            //调用入口类的方法
            var exportedTypes = assembly.GetExportedTypes();
            LogCat.LogWarningWithFormat("dll_type_length", LogCat.LogLabel.ModLoader, dllPath,
                exportedTypes.Length);
            var modLifecycleHandlerType =
                FindTypeInTypeArray(exportedTypes, Config.ModLifecycleHandlerName);
            if (modLifecycleHandlerType == null)
            {
                //The module does not register a lifecycle processor.
                //模组没有注册生命周期处理器。
                LogCat.LogWarningWithFormat("dll_does_not_register_lifecycle_processor", LogCat.LogLabel.ModLoader,
                    dllPath, Config.ModLifecycleHandlerName);
                return;
            }

            var constructor = modLifecycleHandlerType.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                //No parameterless constructor found.
                //未找到无参构造方法。
                LogCat.LogWarningWithFormat("dll_no_parameterless_constructor", LogCat.LogLabel.ModLoader,
                    dllPath);
                return;
            }

            var modLifecycleHandler = constructor.Invoke(null);
            var methodInfo =
                modLifecycleHandlerType.GetMethod(nameof(IModLifecycleHandler.OnModLoaded));
            if (methodInfo == null)
            {
                LogCat.LogWarningWithFormat("mod_lifecycle_handler_not_implement_interface",
                    LogCat.LogLabel.ModLoader,
                    dllPath);
                return;
            }

            methodInfo.Invoke(modLifecycleHandler, null);
        }
        catch (ArgumentNullException argumentNullException)
        {
            //The assemblyPath parameter is null.
            //assemblyPath参数为空。
            LogCat.LogErrorWithFormat("load_dll_argument_null_exception", LogCat.LogLabel.ModLoader, dllPath);
            LogCat.WhenCaughtException(argumentNullException, LogCat.LogLabel.ModLoader);
            return;
        }
        catch (ArgumentException argumentException)
        {
            //Not an absolute path.
            //不是绝对路径
            LogCat.LogErrorWithFormat("load_dll_argument_exception", LogCat.LogLabel.ModLoader, dllPath);
            LogCat.WhenCaughtException(argumentException, LogCat.LogLabel.ModLoader);
            return;
        }
        catch (FileLoadException fileLoadException)
        {
            //A file that was found could not be loaded.
            //无法加载找到的文件。
            LogCat.LogErrorWithFormat("load_dll_file_load_exception", LogCat.LogLabel.ModLoader, dllPath);
            LogCat.WhenCaughtException(fileLoadException, LogCat.LogLabel.ModLoader);
            return;
        }
        catch (BadImageFormatException badImageFormatException)
        {
            //assemblyPath is not a valid assembly.
            //assemblyPath不是有效的程序集。
            LogCat.LogErrorWithFormat("load_dll_bad_image_format_exception", LogCat.LogLabel.ModLoader,
                dllPath);
            LogCat.WhenCaughtException(badImageFormatException, LogCat.LogLabel.ModLoader);
            return;
        }

        //Loading the dll succeeded.
        //加载dll成功。
        LogCat.LogWithFormat("load_dll_success", LogCat.LogLabel.ModLoader, dllPath);
    }


    /// <summary>
    /// <para>Find a specific type by class name</para>
    /// <para>通过类名查找特定的类型</para>
    /// </summary>
    /// <param name="types">
    ///<para>TypeArray</para>
    ///<para>类型数组</para>
    /// </param>
    /// <param name="className">
    ///<para>ClassName</para>
    ///<para>类名</para>
    /// </param>
    /// <returns></returns>
    public static Type? FindTypeInTypeArray(Type[] types, string className)
    {
        return types.FirstOrDefault(type => type.Name == className);
    }

    /// <summary>
    /// <para>Load all mods</para>
    /// <para>加载全部模组</para>
    /// </summary>
    /// <remarks>
    ///<para>This method scans the incoming subfolders and loads them as module folders.</para>
    ///<para>此方法会将扫描传入的子文件夹，并将其子文件夹看作模组文件夹加载。</para>
    /// </remarks>
    /// <param name="modFolder">
    ///<para>Mod folder</para>
    ///<para>模组文件夹</para>
    /// </param>
    /// <exception cref="DirectoryNotFoundException">
    ///<para>If the given folder does not exist, throw this exception.</para>
    ///<para>如果给定的文件夹不存在，则抛出此异常。</para>
    /// </exception>
    public static void LoadAllMods(string modFolder)
    {
        if (!Directory.Exists(modFolder))
        {
            //The mod directory does not exist.
            //模组目录不存在。
            throw new DirectoryNotFoundException("mod folder not exist:" + modFolder);
        }

        var directoryInfo = new DirectoryInfo(modFolder);
        foreach (var directory in directoryInfo.GetDirectories())
        {
            LoadSingleMod(directory.FullName);
        }
    }

    /// <summary>
    /// <para>Load a single mod</para>
    /// <para>加载单个模组</para>
    /// </summary>
    /// <param name="modFolderPath">
    ///<para>Mod path</para>
    ///<para>模组路径</para>
    /// </param>
    /// <exception cref="DirectoryNotFoundException">
    /// <para>If the given directory does not exist, throw this exception.</para>
    ///<para>如果给定的目录不存在，那么抛出此异常。</para>
    /// </exception>
    /// <exception cref="NullReferenceException">
    ///<para>Throw this exception if the manifest file creation deserialization fails.</para>
    ///<para>如果清单文件创建反序列化失败，则抛出此异常。</para>
    /// </exception>
    private static void LoadSingleMod(string modFolderPath)
    {
        if (!Directory.Exists(modFolderPath))
        {
            //The module folder does not exist.
            //模组文件夹不存在。
            throw new DirectoryNotFoundException("Mod folder does not exist:" + modFolderPath);
        }

        var modManifestPath = Path.Join(modFolderPath, Config.ModManifestFileName);
        var modManifest =
            ModManifest.CreateModManifestFromPath(modManifestPath);
        if (modManifest == null)
        {
            throw new InvalidOperationException("mod manifest is null:" + modManifestPath);
        }

        var pckList = modManifest.PckList;
        if (pckList == null || pckList.Length == 0)
        {
            //The module does not contain a pck file.
            //模组不包含pck文件。
            LogCat.LogWarningWithFormat("mod_not_contain_pck", LogCat.LogLabel.ModLoader,
                modFolderPath);
        }
        else
        {
            //The module contains pck files, load the pck files.
            //包含pck文件，加载pck文件。
            foreach (var pck in pckList)
            {
                var pckPath = Path.GetFullPath(pck, modFolderPath);
                LoadPckFile(pckPath);
            }
        }

        var dllList = modManifest.DllList;
        if (dllList == null || dllList.Length == 0)
        {
            //The module does not contain a dll file.
            //模组不包含dll文件。
            LogCat.LogWarningWithFormat("mod_not_contain_dll", LogCat.LogLabel.ModLoader,
                modFolderPath);
        }
        else
        {
            //The module contains dll files, load the dll files.
            //包含dll文件，加载dll文件。
            foreach (var dll in dllList)
            {
                var dllPath = Path.GetFullPath(dll, modFolderPath);
                LoadDllFile(dllPath);
            }
        }
    }

    /// <summary>
    /// <para>Load the Pck file</para>
    /// <para>加载Pck文件</para>
    /// </summary>
    /// <param name="pckPath">
    ///<para>Pck path</para>
    ///<para>Pck路径</para>
    /// </param>
    /// <exception cref="FileNotFoundException">
    ///<para>If the given path does not exist, throw this exception.</para>
    ///<para>如果给定的路径不存在，那么抛出此异常。</para>
    /// </exception>
    /// <exception cref="Exception">
    ///<para>Throw this exception if the pck package fails to load.</para>
    ///<para>如果pck包加载失败了，抛出此异常。</para>
    /// </exception>
    private static void LoadPckFile(string pckPath)
    {
        if (!File.Exists(pckPath))
        {
            throw new FileNotFoundException("pck file not exist:" + pckPath);
        }

        var success = ProjectSettings.LoadResourcePack(pckPath);
        if (success)
        {
            LogCat.LogWithFormat("load_pck_success", LogCat.LogLabel.ModLoader, pckPath);
        }
        else
        {
            LogCat.LogErrorWithFormat("load_pck_failed", LogCat.LogLabel.ModLoader, pckPath);
            //Throw a suitable exception here for handling at the caller.
            //为这里抛出合适的异常，以便在调用方处理。
            throw new DataException("load pck failed:" + pckPath);
        }
    }
}