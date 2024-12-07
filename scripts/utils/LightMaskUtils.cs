using System;
using System.Collections.Generic;
using System.Linq;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>Light layer utils</para>
/// <para>光照层工具</para>
/// </summary>
public static class LightMaskUtils
{
    private static readonly int[] PowInts =
        [1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288];

    /// <summary>
    /// <para>Pass in a number and return the largest subscript that matches it.</para>
    /// <para>传入一个数字，返回与其匹配的最大下标</para>
    /// </summary>
    /// <param name="number"></param>
    /// <param name="startIndex"></param>
    /// <returns></returns>
    private static int GetMaxPow(int number, int startIndex)
    {
        for (var i = startIndex - 1; i >= 0; i--)
        {
            var pow = PowInts[i];
            if (number >= pow)
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// <para>ParseMaskValue</para>
    /// <para>解析Mask的值</para>
    /// </summary>
    /// <param name="maskValue">
    ///<para>maskValue</para>
    ///<para>值</para>
    /// </param>
    /// <param name="afterParsingTheValue">
    ///<para>This callback returns true when the value is parsed, stopping the parsing immediately.</para>
    ///<para>当解析到值时，此回调返回true，立即停止解析。</para>
    /// </param>
    /// <returns>
    ///<para>The position of the element corresponding to its value, For example, passing in 10, returning [1,3]</para>
    ///<para>与其值对应的元素位置，例如传入10,返回[1,3]</para>
    /// </returns>
    public static int[] ParseMaskValue(int maskValue, Func<int, bool>? afterParsingTheValue = null)
    {
        var result = new List<int>();
        var startIndex = PowInts.Length;
        var indexInPowIntArray = GetMaxPow(maskValue, startIndex);
        while (indexInPowIntArray > -1)
        {
            result.Insert(0, indexInPowIntArray);
            if (afterParsingTheValue?.Invoke(indexInPowIntArray) == true)
            {
                //If it needs to be stopped, then the result is returned directly.
                //如果需要停止，那么直接返回结果。
                return result.ToArray();
            }

            maskValue -= PowInts[indexInPowIntArray];
            startIndex = indexInPowIntArray;
            indexInPowIntArray = GetMaxPow(maskValue, startIndex);
        }

        return result.ToArray();
    }

    /// <summary>
    /// <para>Is there a location within the mask value?</para>
    /// <para>mask值内是否包含某个位置？</para>
    /// </summary>
    /// <param name="maskValue"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool ContainsMaskValue(int maskValue, int index)
    {
        var result = false;
        ParseMaskValue(maskValue, i =>
        {
            result = i == index;
            return result;
        });
        return result;
    }

    /// <summary>
    /// <para>Add a location to MaskValue</para>
    /// <para>为MaskValue添加某个位置</para>
    /// </summary>
    /// <param name="maskValue"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static int AddIndexToMaskValue(int maskValue, int index)
    {
        if (ContainsMaskValue(maskValue, index))
        {
            return maskValue;
        }
        return maskValue + PowInts[index];
    }

    /// <summary>
    /// <para>Converting an array to its corresponding value</para>
    /// <para>将数组转化为与其对应的值</para>
    /// </summary>
    /// <param name="array"></param>
    /// <returns></returns>
    public static int ArrayToMaskValue(int[] array)
    {
        return array.Sum(index => PowInts[index]);
    }
}