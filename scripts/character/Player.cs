using System.Text;
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
    private RayCast2D? _platformDetectionRayCast2D;

    //抛出物品的飞行速度
    private float _throwingVelocity = Config.CellSize * 13;

    //射线是否与平台碰撞
    private bool _collidingWithPlatform;

    //How long does it take for the character to recover from a collision with the platform after jumping off the platform (in seconds)
    //角色从平台上跳下后，多少时间后恢复与平台的碰撞（单位：秒）
    private double _platformCollisionRecoveryTime = 0.2f;


    public override void _Ready()
    {
        base._Ready();
        CharacterName = TranslationServerUtils.Translate("default_player_name");
        LogCat.LogWithFormat("player_spawn_debug", LogCat.LogLabel.Default, LogCat.UploadFormat, ReadOnlyCharacterName,
            GlobalPosition);
        _parabola = GetNode<Line2D>("Parabola");
        _platformDetectionRayCast2D = GetNode<RayCast2D>("PlatformDetectionRayCast");
        UpdateOperationTip();
        var healthBarUi = GameSceneDepend.HealthBarUi;
        if (healthBarUi != null)
        {
            healthBarUi.MaxHp = MaxHp;
            healthBarUi.CurrentHp = CurrentHp;
        }

        //Mount the camera.
        //挂载相机。
        var mainCameraPackedScene = GD.Load<PackedScene>("res://prefab/MainCamera.tscn");
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
        GameSceneDepend.DynamicUiGroup?.HideAllControl();
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
    /// <para>Update operation prompt</para>
    /// <para>更新操作提示</para>
    /// </summary>
    private void UpdateOperationTip()
    {
        var operationTipLabel = GameSceneDepend.OperationTipLabel;
        if (operationTipLabel == null)
        {
            return;
        }

        var operationTipBuilder = new StringBuilder();

        operationTipBuilder.Append("[color=");
        operationTipBuilder.Append(Config.OperationTipActionColor);
        operationTipBuilder.Append(']');
        operationTipBuilder.Append(TranslationServerUtils.Translate(InputMap.ActionGetEvents("ui_left")[0].AsText()));
        operationTipBuilder.Append("[/color]");
        operationTipBuilder.Append(TranslationServerUtils.Translate("action_move_left"));
        operationTipBuilder.Append(' ');
        operationTipBuilder.Append("[color=");
        operationTipBuilder.Append(Config.OperationTipActionColor);
        operationTipBuilder.Append(']');
        operationTipBuilder.Append(TranslationServerUtils.Translate(InputMap.ActionGetEvents("ui_right")[0].AsText()));
        operationTipBuilder.Append("[/color]");
        operationTipBuilder.Append(TranslationServerUtils.Translate("action_move_right"));
        operationTipBuilder.Append(' ');
        operationTipBuilder.Append("[color=");
        operationTipBuilder.Append(Config.OperationTipActionColor);
        operationTipBuilder.Append(']');
        operationTipBuilder.Append(TranslationServerUtils.Translate(InputMap.ActionGetEvents("ui_up")[0].AsText()));
        operationTipBuilder.Append("[/color]");
        operationTipBuilder.Append(TranslationServerUtils.Translate("action_jump"));
        if (_collidingWithPlatform)
        {
            operationTipBuilder.Append(' ');
            operationTipBuilder.Append("[color=");
            operationTipBuilder.Append(Config.OperationTipActionColor);
            operationTipBuilder.Append(']');
            operationTipBuilder.Append(
                TranslationServerUtils.Translate(InputMap.ActionGetEvents("ui_down")[0].AsText()));
            operationTipBuilder.Append("[/color]");
            operationTipBuilder.Append(TranslationServerUtils.Translate("action_jump_down"));
        }

        var nearestItem = FindTheNearestItem();
        if (nearestItem != null)
        {
            operationTipBuilder.Append(' ');
            operationTipBuilder.Append("[color=");
            operationTipBuilder.Append(Config.OperationTipActionColor);
            operationTipBuilder.Append(']');
            operationTipBuilder.Append(
                TranslationServerUtils.Translate(InputMap.ActionGetEvents("pick_up")[0].AsText()));
            operationTipBuilder.Append("[/color]");
            operationTipBuilder.Append(TranslationServerUtils.Translate("action_pick_up"));
            if (nearestItem is IItem item)
            {
                operationTipBuilder.Append(item.Name);
            }

            operationTipLabel.Text = operationTipBuilder.ToString();
        }

        if (CurrentItem != null)
        {
            operationTipBuilder.Append(' ');
            operationTipBuilder.Append("[color=");
            operationTipBuilder.Append(Config.OperationTipActionColor);
            operationTipBuilder.Append(']');
            operationTipBuilder.Append(TranslationServerUtils.Translate(InputMap.ActionGetEvents("throw")[0].AsText()));
            operationTipBuilder.Append("[/color]");
            operationTipBuilder.Append(TranslationServerUtils.Translate("action_throw"));
            if (CurrentItem is IItem item)
            {
                operationTipBuilder.Append(TranslationServerUtils.Translate(item.Name));
                operationTipBuilder.Append(' ');
                operationTipBuilder.Append("[color=");
                operationTipBuilder.Append(Config.OperationTipActionColor);
                operationTipBuilder.Append(']');
                operationTipBuilder.Append(
                    TranslationServerUtils.Translate(InputMap.ActionGetEvents("use_item")[0].AsText()));
                operationTipBuilder.Append("[/color]");
                operationTipBuilder.Append(TranslationServerUtils.Translate("action_use_item"));
                operationTipBuilder.Append(TranslationServerUtils.Translate(item.Name));
            }
        }

        operationTipLabel.Text = operationTipBuilder.ToString();
    }


    protected override void HookPhysicsProcess(ref Vector2 velocity, double delta)
    {
        //When the collision state between the platform detection ray and the platform changes
        //在平台检测射线与平台碰撞状态改变时
        if (_platformDetectionRayCast2D != null && _platformDetectionRayCast2D.IsColliding() != _collidingWithPlatform)
        {
            //When the state changes, update the action hint
            //当状态改变时，更新操作提示
            _collidingWithPlatform = _platformDetectionRayCast2D.IsColliding();
            UpdateOperationTip();
        }

        //If the character is on the ground, give an upward velocity when the jump button is pressed
        //如果角色正在地面上，按下跳跃键时，给予一个向上的速度
        if (Input.IsActionJustPressed("ui_up") && IsOnFloor())
            velocity.Y = JumpVelocity;

        //Moving left and right
        //左右移动
        var axis = Input.GetAxis("ui_left", "ui_right");
        velocity.X = axis * Speed * Config.CellSize * ProtectedSpeedScale;

        //Use items
        //使用物品
        if (Input.IsActionPressed("use_item"))
        {
            UseItem(GetGlobalMousePosition());
        }

        

        //Pick up an item
        //捡起物品
        if (Input.IsActionJustPressed("pick_up"))
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
            GameSceneDepend.DynamicUiGroup?.HideAllControl();
            CurrentItem = null;
        }
    }

    protected override void WhenUpdateCurrentItem(Node2D? currentItem)
    {
        UpdateOperationTip();
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
        var healthBarUi = GameSceneDepend.HealthBarUi;
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
        if (EventBus.GameOverEvent == null)
        {
            return;
        }

        var gameOverEvent = new GameOverEvent();
        if (damageTemplate.Attacker != null)
        {
            gameOverEvent.DeathInfo = await DeathInfoGenerator.GenerateDeathInfo(this, damageTemplate.Attacker);
        }

        EventBus.GameOverEvent.Invoke(gameOverEvent);
    }

    protected override void EnterThePickingRangeBody(Node node)
    {
        base.EnterThePickingRangeBody(node);
        if (CurrentItem == node)
        {
            //If the node entering the pick range is the node held by the player, then return.
            //如果说进入拾捡范围的节点是玩家所持有的节点，那么返回。
            return;
        }

        if (node is not Node2D)
        {
            return;
        }

        UpdateOperationTip();
    }

    protected override void ExitThePickingRangeBody(Node node)
    {
        base.ExitThePickingRangeBody(node);
        if (node is not Node2D)
        {
            return;
        }

        UpdateOperationTip();
    }

    protected override void OnHit(DamageTemplate damageTemplate)
    {
        base.OnHit(damageTemplate);
        var healthBarUi = GameSceneDepend.HealthBarUi;
        if (healthBarUi != null)
        {
            healthBarUi.CurrentHp = CurrentHp;
        }
    }
}