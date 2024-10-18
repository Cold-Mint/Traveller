using System.Threading.Tasks;
using ColdMint.scripts.damage;
using ColdMint.scripts.deathInfo;
using ColdMint.scripts.debug;
using ColdMint.scripts.inventory;
using ColdMint.scripts.map.events;
using ColdMint.scripts.utils;
using ColdMint.scripts.pickable;
using Godot;

namespace ColdMint.scripts.character;

/// <summary>
/// <para>玩家角色</para>
/// </summary>
public partial class Player : CharacterTemplate
{
    //Empty object projectile
    //空的物品抛射线
    private readonly Vector2[] _emptyVector2Array = [Vector2.Zero];

    //抛物线
    private Line2D? _parabola;

    //用于检测玩家是否站在平台上的射线
    [Export] private RayCast2D[]? _platformDetectionRayCast2DList;

    //抛出物品的飞行速度
    private float _throwingVelocity = Config.CellSize * 13;

    //射线是否与平台碰撞
    private bool _collidingWithPlatform;

    private bool _canUseItem;

    //How long does it take for the character to recover from a collision with the platform after jumping off the platform (in seconds)
    //角色从平台上跳下后，多少时间后恢复与平台的碰撞（单位：秒）
    private double _platformCollisionRecoveryTime = 0.2f;


    public override void _Ready()
    {
        base._Ready();
        if (_platformDetectionRayCast2DList == null || _platformDetectionRayCast2DList.Length == 0)
        {
            LogCat.LogError("no_platform_detection_raycast_found");
            return;
        }
        CharacterName = TranslationServerUtils.Translate("default_player_name");
        LogCat.LogWithFormat("player_spawn_debug", LogCat.LogLabel.Default, ReadOnlyCharacterName,
            GlobalPosition);
        _parabola = GetNode<Line2D>("Parabola");
        var healthBarUi = GameSceneDepend.GameGuiTemplate?.HealthBar;
        if (healthBarUi != null)
        {
            healthBarUi.MaxHp = MaxHp;
            healthBarUi.CurrentHp = CurrentHp;
        }

        //Mount the camera.
        //挂载相机。
        var mainCameraPackedScene = ResourceLoader.Load<PackedScene>("res://prefab/MainCamera.tscn");
        var camera2D = NodeUtils.InstantiatePackedScene<Camera2D>(mainCameraPackedScene);
        if (camera2D != null)
        {
            NodeUtils.CallDeferredAddChild(this, camera2D);
        }
    }

    protected override void WhenBindItemContainer(IItemContainer? itemContainer)
    {
        if (itemContainer == null)
        {
            return;
        }

        //Subscribe to events when the item container is bound to the player.
        //在物品容器与玩家绑定时订阅事件。
        itemContainer.SelectedItemChangeEvent += SelectedItemChangeEvent;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (ProtectedItemContainer != null)
        {
            //Unsubscribe to events when this object is destroyed.
            //此节点被销毁时，取消订阅事件。
            ProtectedItemContainer.SelectedItemChangeEvent -= SelectedItemChangeEvent;
        }
    }

    private void SelectedItemChangeEvent(SelectedItemChangeEvent selectedItemChangeEvent)
    {
        var item = selectedItemChangeEvent.NewItem;
        if (item is Node2D node2D)
        {
            CurrentItem = node2D;
        }
        else
        {
            CurrentItem = null;
        }
    }

    public override void _MouseEnter()
    {
    }

    public override void _MouseExit()
    {
    }

    /// <summary>
    /// <para>UpdateCollidingWithPlatform</para>
    /// <para>更新与平台发生碰撞的状态</para>
    /// </summary>
    private void UpdateCollidingWithPlatform()
    {
        //When the collision state between the platform detection ray and the platform changes
        //在平台检测射线与平台碰撞状态改变时
        if (_platformDetectionRayCast2DList is not { Length: > 0 }) return;
        foreach (var rayCast2D in _platformDetectionRayCast2DList)
        {
            if (!rayCast2D.IsColliding()) continue;
            _collidingWithPlatform = true;
            return;
        }
        _collidingWithPlatform = false;
    }

    protected override void HookPhysicsProcess(ref Vector2 velocity, double delta)
    {
        UpdateCollidingWithPlatform();
        //If the character is on the ground, give an upward velocity when the jump button is pressed
        //如果角色正在地面上，按下跳跃键时，给予一个向上的速度
        if (GameSceneDepend.GameGuiTemplate?.JumpButton != null)
        {
            if (GameSceneDepend.GameGuiTemplate.JumpButton.IsPressed() && IsOnFloor())
                velocity.Y = JumpVelocity;
        }
        else
        {
            if (Input.IsActionJustPressed("ui_up") && IsOnFloor())
                velocity.Y = JumpVelocity;
        }

        //Moving left and right
        //左右移动
        if (GameSceneDepend.GameGuiTemplate?.LeftButton != null && GameSceneDepend.GameGuiTemplate?.RightButton != null)
        {
            var left = GameSceneDepend.GameGuiTemplate.LeftButton.IsPressed() ? -1 : 0;
            var right = GameSceneDepend.GameGuiTemplate.RightButton.IsPressed() ? 1 : 0;
            var axis = left + right;
            velocity.X = axis * Speed * Config.CellSize * ProtectedSpeedScale;
        }
        else
        {
            var axis = Input.GetAxis("ui_left", "ui_right");
            velocity.X = axis * Speed * Config.CellSize * ProtectedSpeedScale;
        }



        if (Input.IsActionJustPressed("use_item"))
        {
            _canUseItem = !GameSceneDepend.IsMouseOverFurnitureGui && !GameSceneDepend.IsMouseOverItemSlotNode;
        }
        //Use items
        //使用物品
        if (Input.IsActionPressed("use_item"))
        {
            if (_canUseItem)
            {
                UseItem(GetGlobalMousePosition());
            }
        }
        //Pick up an item
        //捡起物品
        if (GameSceneDepend.GameGuiTemplate?.PickButton != null)
        {
            if (GameSceneDepend.GameGuiTemplate.PickButton.IsPressed())
            {
                PressedPick();
            }
        }
        else
        {
            if (Input.IsActionJustPressed("pick_up"))
            {
                PressedPick();
            }
        }


        if (Input.IsActionJustPressed("ui_down"))
        {
            if (_collidingWithPlatform)
            {
                //When the character stands on the platform and presses the ui_down key, we cancel the collision between the character and the platform
                //当角色站在平台上按下 ui_down 键时，我们取消角色与平台的碰撞
                var timer = new Timer();
                AddChild(timer);
                timer.WaitTime = _platformCollisionRecoveryTime;
                timer.OneShot = true;
                timer.Start();
                timer.Timeout += () =>
                {
                    SetCollisionMaskValue(Config.LayerNumber.Platform, true);
                    timer.QueueFree();
                };
                SetCollisionMaskValue(Config.LayerNumber.Platform, false);
            }
        }


        //Display a parabola when an item is thrown
        //抛出物品时，显示抛物线
        if (Input.IsActionPressed("throw"))
        {
            if (_parabola == null)
            {
                return;
            }

            if (ItemMarker2D == null)
            {
                //Cannot get the marked location of the item, then do not draw a line
                //无法获取物品的标记位置，那么不绘制线
                return;
            }

            _parabola.Points = CurrentItem == null
                ? _emptyVector2Array
                : ParabolicUtils.ComputeParabolic(ItemMarker2D.Position, GetThrowVelocity(), Gravity, 0.1f);
        }


        //When you raise your hand, throw the object
        //抬起手时，抛出物品
        if (Input.IsActionJustReleased("throw"))
        {
            if (ItemContainer == null)
            {
                return;
            }

            if (_parabola != null)
            {
                _parabola.Points = [Vector2.Zero];
            }

            ThrowItem(ItemContainer.GetSelectIndex(), 1, GetThrowVelocity());
            CurrentItem = null;
        }
    }

    private void PressedPick()
    {
        var pickAbleItem = FindTheNearestItem();
        var success = PickItem(pickAbleItem);
        if (success)
        {
            if (pickAbleItem != null)
            {
                PickingRangeBodiesList?.Remove(pickAbleItem);
            }
        }
    }

    /// <summary>
    /// <para>当玩家手动抛出物品时，施加到物品上的速度值</para>
    /// </summary>
    /// <returns></returns>
    private Vector2 GetThrowVelocity()
    {
        //We take the mouse position, normalize it, and then multiply it by the distance the player can throw
        //我们拿到鼠标的位置，将其归一化处理，然后乘以玩家可扔出的距离
        return GetLocalMousePosition().Normalized() * _throwingVelocity;
    }

    public override void _Process(double delta)
    {
        if (!Visible)
        {
            return;
        }

        AimTheCurrentItemAtAPoint(GetGlobalMousePosition());
        var itemMarker2DPosition = Vector2.Zero;
        if (ItemMarker2D != null)
        {
            itemMarker2DPosition = ItemMarker2D.Position;
        }

        var axis = Input.GetAxis("ui_left", "ui_right");
        switch (axis)
        {
            case -1:
                //Minus 1, we move to the left
                //-1，向左移动
                FacingLeft = true;
                if (ItemMarker2D != null)
                {
                    itemMarker2DPosition.X = -ReadOnlyItemMarkerOriginalX;
                }

                Flip();
                break;
            case 1:
                //1, move to the right
                //1，向右移动
                FacingLeft = false;
                if (ItemMarker2D != null)
                {
                    itemMarker2DPosition.X = ReadOnlyItemMarkerOriginalX;
                }

                Flip();
                break;
        }

        if (ItemMarker2D != null)
        {
            ItemMarker2D.Position = itemMarker2DPosition;
        }
    }

    protected override void Flip()
    {
        base.Flip();
        //If there is a weapon, flip it too
        //如果有武器的话，也要翻转
        if (CurrentItem is PickAbleTemplate pickAbleTemplate)
        {
            pickAbleTemplate.Flip(FacingLeft);
        }
    }

    public override void Revive(int newHp)
    {
        base.Revive(newHp);
        var healthBarUi = GameSceneDepend.GameGuiTemplate?.HealthBar;
        if (healthBarUi != null)
        {
            //The purpose of setting Hp to the current Hp is to cause the life bar to refresh.
            //将Hp设置为当前Hp的目的是，使生命条刷新。
            healthBarUi.CurrentHp = CurrentHp;
        }
        ProcessMode = ProcessModeEnum.Inherit;
        Show();
    }

    /// <summary>
    /// <para>When the player dies</para>
    /// <para>当玩家死亡的时候</para>
    /// </summary>
    /// <param name="damageTemplate"></param>
    protected override async Task OnDie(DamageTemplate damageTemplate)
    {
        Hide();
        ProcessMode = ProcessModeEnum.Disabled;
        ProtectedItemContainer?.ClearAllItems();
        if (EventBus.GameOverEvent == null)
        {
            LogCat.Log("game_over_event_is_empty");
            return;
        }

        var gameOverEvent = new GameOverEvent();
        if (damageTemplate.Attacker != null)
        {
            gameOverEvent.DeathInfo = await DeathInfoGenerator.GenerateDeathInfo(this, damageTemplate.Attacker);
        }

        EventBus.GameOverEvent.Invoke(gameOverEvent);
    }


    protected override void OnHit(DamageTemplate damageTemplate)
    {
        base.OnHit(damageTemplate);
        var healthBarUi = GameSceneDepend.GameGuiTemplate?.HealthBar;
        if (healthBarUi != null)
        {
            healthBarUi.CurrentHp = CurrentHp;
        }
    }
}