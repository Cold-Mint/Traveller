using System.Linq;
using ColdMint.scripts.buff;
using Godot;

namespace ColdMint.scripts.utils;

class ColorUtils
{
    public static Color WeightedAverageMix(IStatusEffect[] statusEffectArray) => WeightedAverageMix(statusEffectArray
        .Select(statusEffect => new WeightedAverageMixInput { Color = statusEffect.Color, Weight = statusEffect.Level })
        .ToArray());

    /// <summary>
    /// <para>Weighted average mixture</para>
    /// <para>加权平均混合</para>
    /// </summary>
    /// <param name="weightedAverageMixInputArray"></param>
    /// <returns></returns>
    public static Color WeightedAverageMix(WeightedAverageMixInput[] weightedAverageMixInputArray)
    {
        if (weightedAverageMixInputArray.Length == 0)
        {
            return Colors.Black;
        }

        if (weightedAverageMixInputArray.Length == 1)
        {
            return weightedAverageMixInputArray[0].Color;
        }

        float totalWeight = 0;
        float rSum = 0, gSum = 0, bSum = 0;
        float maxA = 0;

        foreach (var input in weightedAverageMixInputArray)
        {
            totalWeight += input.Weight;
            rSum += input.Color.R * input.Weight;
            gSum += input.Color.G * input.Weight;
            bSum += input.Color.B * input.Weight;
            maxA = Mathf.Max(maxA, input.Color.A);
        }

        var a = (int)(maxA * 255);
        var r = (int)(rSum / totalWeight * 255);
        var g = (int)(gSum / totalWeight * 255);
        var b = (int)(bSum / totalWeight * 255);

        return Color.FromHtml($"#{a:X2}{r:X2}{g:X2}{b:X2}");
    }
}

/// <summary>
/// <para>The input object of the weighting algorithm</para>
/// <para>加权算法的输入对象</para>
/// </summary>
class WeightedAverageMixInput
{
    /// <summary>
    /// <para>Color</para>
    /// <para>颜色值</para>
    /// </summary>
    public Color Color = Colors.White;

    /// <summary>
    /// <para>Weight</para>
    /// <para>权重值</para>
    /// </summary>
    public int Weight = 1;
}