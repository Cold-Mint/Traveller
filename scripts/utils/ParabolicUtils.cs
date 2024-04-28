using ColdMint.scripts.debug;
using Godot;

namespace ColdMint.scripts.utils;

/// <summary>
/// <para>ParabolicUtils</para>
/// <para>抛物线工具</para>
/// </summary>
public class ParabolicUtils
{
    /// <summary>
    /// <para>Calculated parabola</para>
    /// <para>计算抛物线</para>
    /// </summary>
    /// <param name="origin">
    ///<para>Origin of coordinates</para>
    ///<para>坐标的原点</para>
    /// </param>
    /// <param name="velocityVector">
    ///<para>speed</para>
    ///<para>速度</para>
    /// <para>Represents the distance traveled per second</para>
    /// <para>表示每秒移动的距离</para>
    /// </param>
    /// <param name="gravity">
    ///<para>gravity</para>
    ///<para>重力</para>
    /// </param>
    /// <param name="steps">
    ///<para>Sampling step size</para>
    ///<para>采样步长</para>
    /// <para>How many seconds to take a point</para>
    /// <para>多少秒取一次点</para>
    /// </param>
    /// <returns></returns>
    public static Vector2[] ComputeParabolic(Vector2 origin, Vector2 velocityVector, float gravity, float steps)
    {
        // 计算采样点数量
        var numSteps = Mathf.CeilToInt(1f / steps);
        // 初始化结果数组
        var points = new Vector2[numSteps];
        // 计算每个采样点的位置
        for (int i = 0; i < numSteps; i++)
        {
            // 计算当前时间
            var t = i * steps;
            // 计算当前位置
            var x = origin.X + velocityVector.X * t;
            var y = origin.Y + velocityVector.Y * t + 0.5f * gravity * t * t;
            // 将位置添加到结果数组中
            points[i] = new Vector2(x, y);
        }
        
        return points;
    }
}