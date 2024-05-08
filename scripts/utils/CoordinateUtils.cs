using Godot;

namespace ColdMint.scripts.utils;

public class CoordinateUtils
{
    /// <summary>
    /// <para>方向描述</para>
    /// </summary>
    public enum OrientationDescribe
    {
        //上
        Up,

        //下
        Down,

        //左
        Left,

        //右
        Right,

        //原点
        Origin
    }

    /// <summary>
    /// <para>Vector to direction description(Relative to the origin)</para>
    /// <para>向量到方向描述（相对原点而言）</para>
    /// </summary>
    /// <param name="origin">
    ///<para>origin</para>
    ///<para>原点坐标</para>
    /// </param>
    /// <param name="position">
    ///<para>position</para>
    ///<para>位置</para>
    /// </param>
    /// <returns></returns>
    public static OrientationDescribe[] VectorToOrientationArray(Vector2 origin, Vector2 position)
    {
        var vector2 = position - origin;
        var orientationDescribes = new OrientationDescribe[2];
        if (vector2.X < 0)
        {
            orientationDescribes[0] = OrientationDescribe.Left;
        }
        else if (vector2.X == 0)
        {
            orientationDescribes[0] = OrientationDescribe.Origin;
        }
        else
        {
            orientationDescribes[0] = OrientationDescribe.Right;
        }

        if (vector2.Y > 0)
        {
            orientationDescribes[1] = OrientationDescribe.Down;
        }
        else if (vector2.Y == 0)
        {
            orientationDescribes[0] = OrientationDescribe.Origin;
        }
        else
        {
            orientationDescribes[1] = OrientationDescribe.Up;
        }

        return orientationDescribes;
    }
}