using System;
using System.IO;
using ColdMint.scripts.serialization;

namespace ColdMint.scripts.mod;

/// <summary>
/// <para>Module manifest file</para>
/// <para>模组清单文件</para>
/// </summary>
public class ModManifest
{
    /// <summary>
    /// <para>模组Id</para>
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// <para>Mod name</para>
    /// <para>模组名称</para>
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// <para>Dll path list of the mod</para>
    /// <para>模组的Dll路径列表</para>
    /// </summary>
    /// <remarks>
    ///<para>Allow relative paths, such as:... / Points to the parent directory.</para>
    ///<para>允许使用相对路径，例如: ../指向上级目录。</para>
    /// </remarks>
    public string[]? DllList { get; set; }

    /// <summary>
    /// <para>Pck path list of mod</para>
    /// <para>模组的Pck路径列表</para>
    /// </summary>
    /// <remarks>
    ///<para>Allow relative paths, such as:... / Points to the parent directory.</para>
    ///<para>允许使用相对路径，例如: ../指向上级目录。</para>
    /// </remarks>
    public string[]? PckList { get; set; }

    /// <summary>
    /// <para>Creates module list information from a path</para>
    /// <para>从路径创建模组清单信息</para>
    /// </summary>
    /// <param name="filePath">
    ///<para>filePath</para>
    ///<para>文件路径</para>
    /// </param>
    /// <exception cref="ArgumentException">
    ///<para>When a given path is not to <see cref="Config.ModManifestFileName"/> end throw this exception.</para>
    ///<para>当给定的路径不是以<see cref="Config.ModManifestFileName"/>结尾时抛出此异常。</para>
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///<para>This exception is thrown when the given path does not exist.</para>
    ///<para>当给定的路径不存在时，抛出此异常。</para>
    /// </exception>
    /// <returns></returns>
    public static ModManifest? CreateModManifestFromPath(string filePath)
    {
        if (!filePath.EndsWith(Config.ModManifestFileName))
        {
            throw new ArgumentException("path must end with " + Config.ModManifestFileName + ".");
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The file at " + filePath + " does not exist.");
        }

        var content = File.ReadAllText(filePath);
        return YamlSerialization.Deserialize<ModManifest>(content);
    }
}