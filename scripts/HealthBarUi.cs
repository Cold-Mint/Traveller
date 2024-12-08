using ColdMint.scripts.utils;
using Godot;

namespace ColdMint.scripts;

/// <summary>
/// <para>HealthBarUi</para>
/// <para>健康条UI</para>
/// </summary>
public partial class HealthBarUi : HBoxContainer
{
    private int _maxHp;
    private int _currentHp;
    private Texture2D? _heartFull;
    private Texture2D? _heartEmpty;
    private Texture2D? _heartHalf;
    private Texture2D? _heartQuarter;
    private Texture2D? _heartThreeFourths;

    public int CurrentHp
    {
        get => _currentHp;
        set
        {
            if (value == _currentHp)
            {
                return;
            }

            if (_currentHp > _maxHp)
            {
                //Prohibit the current health to exceed the maximum health. When the maximum health is exceeded, the UI cannot be drawn.
                //禁止当前血量超过最大血量，当超过最大值时，无法绘制UI。
                return;
            }

            var heartCount = GetChildCount();
            //A few hearts are full
            //有几颗心是满的
            var fullHeartCount = value / Config.HeartRepresentsHealthValue;
            for (int i = 0; i < fullHeartCount; i++)
            {
                //Brush up the Ui
                //把Ui刷满
                var textureRect = GetChild<TextureRect>(i);
                textureRect.Texture = _heartFull;
            }

            //How many hollows
            //有多少空心
            var emptyHeartCount = heartCount - fullHeartCount;
            if (emptyHeartCount > 0)
            {
                //How much blood is left on the last one
                //最后那颗剩余多少血
                var leftOverTextureRect = GetChild<TextureRect>(fullHeartCount);
                var leftOver = value % Config.HeartRepresentsHealthValue;
                if (leftOver > 0)
                {
                    //Percentage of total
                    //占总数的百分比
                    var percent = leftOver / (float)Config.HeartRepresentsHealthValue;
                    leftOverTextureRect.Texture = GetTexture2DByPercent(percent);
                    emptyHeartCount--;
                }
            }


            for (int i = heartCount - emptyHeartCount; i < heartCount; i++)
            {
                var textureRect = GetChild<TextureRect>(i);
                textureRect.Texture = _heartEmpty;
            }

            _currentHp = value;
        }
    }

    public int MaxHp
    {
        get => _maxHp;
        set
        {
            if (value == _maxHp)
            {
                return;
            }

            var heartCount = value / Config.HeartRepresentsHealthValue;
            for (var i = 0; i < heartCount; i++)
            {
                var heart = CreateTextureRect();
                heart.Texture = _heartFull;
                AddChild(heart);
            }

            //How much blood is left on the last one
            //最后那颗剩余多少血
            var leftOver = value % Config.HeartRepresentsHealthValue;
            if (leftOver > 0)
            {
                var lastHeart = CreateTextureRect();
                //Percentage of total
                //占总数的百分比
                var percent = leftOver / (float)Config.HeartRepresentsHealthValue;
                lastHeart.Texture = GetTexture2DByPercent(percent);
                AddChild(lastHeart);
            }

            _maxHp = value;
        }
    }

    private TextureRect CreateTextureRect()
    {
        var textureRect = new TextureRect();
        return textureRect;
    }

    /// <summary>
    /// <para>Get the texture based on percentage</para>
    /// <para>根据百分比获取纹理</para>
    /// </summary>
    /// <param name="percent"></param>
    /// <returns></returns>
    private Texture2D? GetTexture2DByPercent(float percent)
    {
        if (percent == 0)
        {
            return _heartEmpty;
        }
        if (percent <= 0.25f)
        {
            return _heartQuarter;
        }
        if (percent <= 0.5f)
        {
            return _heartHalf;
        }
        if (percent <= 0.75f)
        {
            return _heartThreeFourths;
        }
        return _heartFull;
    }


    public override void _Ready()
    {
        base._Ready();
        NodeUtils.DeleteAllChild(this);
        _heartEmpty = ResourceLoader.Load<Texture2D>("res://sprites/ui/HeartEmpty.png");
        _heartQuarter = ResourceLoader.Load<Texture2D>("res://sprites/ui/HeartQuarter.png");
        _heartHalf = ResourceLoader.Load<Texture2D>("res://sprites/ui/HeartHalf.png");
        _heartThreeFourths = ResourceLoader.Load<Texture2D>("res://sprites/ui/HeartThreeFourths.png");
        _heartFull = ResourceLoader.Load<Texture2D>("res://sprites/ui/HeartFull.png");
    }
}