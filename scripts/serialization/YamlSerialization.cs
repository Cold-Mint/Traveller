using System.IO;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ColdMint.scripts.serialization;

public static class YamlSerialization
{
    /// <summary>
    /// <para>YamlDeserializer</para>
    /// <para>Yaml反序列化器</para>
    /// </summary>
    private static readonly IDeserializer YamlDeserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance) // convent snake_case
        .Build();

    private static readonly ISerializer YamlSerializer = new SerializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance) // convent snake_case
        .Build();

    /// <summary>
    /// <para>Read a Json file to type T</para>
    /// <para>读取一个Json文件到T类型</para>
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static async Task<T?> ReadYamlFileToObj<T>(string path)
    {
        await using var openStream = File.OpenRead(path);
        var yaml = await new StreamReader(openStream).ReadToEndAsync();
        return YamlDeserializer.Deserialize<T>(yaml);
    }

    /// <summary>
    /// <para>Serialize the object to Json</para>
    /// <para>将对象序列化为Yaml</para>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string Serialize(object obj)
    {
        return YamlSerializer.Serialize(obj);
    }

    /// <summary>
    /// <para>Deserialize Yaml to the object</para>
    /// <para>反序列化Yaml到对象</para>
    /// </summary>
    /// <param name="yaml"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    // ReSharper disable ReturnTypeCanBeNotNullable
    public static T? Deserialize<T>(string yaml)
    {
        return YamlDeserializer.Deserialize<T>(yaml);
    }
    // ReSharper restore ReturnTypeCanBeNotNullable
}