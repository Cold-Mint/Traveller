using ColdMint.scripts.map.interfaces;

namespace ColdMint.scripts.map.room;

public class RoomTemplate : IRoomTemplate
{
    /// <summary>
    /// <para>Unlimited use</para>
    /// <para>无限次使用</para>
    /// </summary>
    public const int Infinite = -1;

    private int _usedNumber;

    public RoomTemplate(string roomResPath)
    {
        RoomResPath = roomResPath;
        MaxNumber = Infinite;
        _usedNumber = 0;
    }

    public string RoomResPath { get; }

    public bool CanUse
    {
        get
        {
            if (MaxNumber == Infinite)
            {
                return true;
            }

            return _usedNumber < MaxNumber;
        }
    }

    public int MaxNumber { get; set; }


    public void AddUsedNumber()
    {
        _usedNumber++;
    }

    public int UsedNumber => _usedNumber;
}