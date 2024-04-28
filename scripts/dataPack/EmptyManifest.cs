using Godot;

namespace ColdMint.scripts.dataPack;

public class EmptyManifest : IDataPackManifest
{

    private EmptyManifest()
    {
        
    }
    
    public string? ID { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? VersionName { get; set; }
    public int? VersionCode { get; set; }
    public string? Author { get; set; }
    public string? Namespace { get; set; }

    /// <summary>
    /// <para>Create an empty manifest file</para>
    /// <para>创建空的清单文件</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static EmptyManifest CreateEmptyManifest(string id)
    {
        var emptyManifest = new EmptyManifest();
        var unknown = TranslationServer.Translate("unknown");
        emptyManifest.ID = id;
        emptyManifest.Author = unknown;
        emptyManifest.Name = unknown;
        emptyManifest.Description = unknown;
        emptyManifest.VersionName = unknown;
        emptyManifest.Namespace = Config.EmptyNamespace;
        emptyManifest.VersionCode = 0;
        return emptyManifest;
    }
}