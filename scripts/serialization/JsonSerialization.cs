using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ColdMint.scripts.serialization;

public static class JsonSerialization
{
    private static JsonSerializerOptions _options = new JsonSerializerOptions
    {
        //Case-insensitive attribute matching
        //不区分大小写的属性匹配
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// <para>Read a Json file to type T</para>
    /// <para>读取一个Json文件到T类型</para>
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> ReadJsonFileToObj<T>(string path)
    {
        await using var openStream = File.OpenRead(path);
        return await JsonSerializer.DeserializeAsync<T>(openStream, _options);
    }

    public static async Task<T?> ReadJsonFileToObj<T>(Stream openStream)
    {
        return await JsonSerializer.DeserializeAsync<T>(openStream, _options);
    }
}