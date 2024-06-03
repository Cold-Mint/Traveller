using System;
using System.Text;
using ColdMint.scripts.damage;
using ColdMint.scripts.map.events;
using ColdMint.scripts.utils;
using ColdMint.scripts.weapon;
using Godot;

namespace ColdMint.scripts.character;

/// <summary>
/// <para>玩家角色</para>
/// </summary>
public partial class Player : CharacterTemplate
{
    private PackedScene? _floatLabelPackedScene;

    private Control? _floatLabel;

    //Empty object projectile
    //空的物品抛射线
    private readonly Vector2[] _emptyVector2Array = new[] { Vector2.Zero };

    //抛物线
    private Line2D? _parabola;

    //用于检测玩家是否站在平台上的射线
    private RayCast2D? _platformDetectionRayCast2D;


    private const float PromptTextDistance = 50;


    //抛出物品的飞行速度
    private float _throwingVelocity = Config.CellSize * 13;

    //射线是否与平台碰撞
    private bool _collidingWithPlatform;

    //How long does it take for the character to recover from a collision with the platform after jumping off the platform (in seconds)
    //角色从平台上跳下后，多少时间后恢复与平台的碰撞（单位：秒）
    private double _platformCollisionRecoveryTime = 0.2f;

    //物品被扔出后多长时间恢复与地面和平台的碰撞（单位：秒）
    private readonly double _itemCollisionRecoveryTime = 0.045f;


    public override void _Ready()
    {
        base._Ready();
        CharacterName = TranslationServerUtils.Translate("default_player_name");
        _floatLabelPackedScene = GD.Load<PackedScene>("res://prefab/ui/FloatLabel.tscn");
        _parabola = GetNode<Line2D>("Parabola");
        _platformDetectionRayCast2D = GetNode<RayCast2D>("PlatformDetectionRayCast");
        UpdateOperationTip();
        var healthBarUi = GameSceneNodeHolder.HealthBarUi;
        if (healthBarUi != null)
        {
            healthBarUi.MaxHp = MaxHp;
            healthBarUi.CurrentHp = CurrentHp;
        }
    }

    /// <summary>
    /// <para>Update operation prompt</para>
    /// <para>更新操作提示</para>
    /// </summary>
    private void UpdateOperationTip()
    {
        var operationTipLabel = GameSceneNodeHolder.OperationTipLabel;
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
        operationTipBuilder.Append(TranslationServerUtils.Translate("move_left"));
        operationTipBuilder.Append(' ');
        operationTipBuilder.Append("[color=");
        operationTipBuilder.Append(Config.OperationTipActionColor);
        operationTipBuilder.Append(']');
        operationTipBuilder.Append(TranslationServerUtils.Translate(InputMap.ActionGetEvents("ui_right")[0].AsText()));
        operationTipBuilder.Append("[/color]");
        operationTipBuilder.Append(TranslationServerUtils.Translate("move_right"));
        operationTipBuilder.Append(' ');
        operationTipBuilder.Append("[color=");
        operationTipBuilder.Append(Config.OperationTipActionColor);
        operationTipBuilder.Append(']');
        operationTipBuilder.Append(TranslationServerUtils.Translate(InputMap.ActionGetEvents("ui_up")[0].AsText()));
        operationTipBuilder.Append("[/color]");
        operationTipBuilder.Append(TranslationServerUtils.Translate("jump"));
        if (_collidingWithPlatform)
        {
            operationTipBuilder.Append(' ');
            operationTipBuilder.Append("[color=");
            operationTipBuilder.Append(Config.OperationTipActionColor);
            operationTipBuilder.Append(']');
            operationTipBuilder.Append(
                TranslationServerUtils.Translate(InputMap.ActionGetEvents("ui_down")[0].AsText()));
            operationTipBuilder.Append("[/color]");
            operationTipBuilder.Append(TranslationServerUtils.Translate("jump_down"));
        }

        //If the PickingRangeBodiesList is not null and the length is greater than 0
        //如果PickingRangeBodiesList不是null，且长度大于0
        if (PickingRangeBodiesList is { Count: > 0 })
        {
            operationTipBuilder.Append(' ');
            operationTipBuilder.Append("[color=");
            operationTipBuilder.Append(Config.OperationTipActionColor);
            operationTipBuilder.Append(']');
            operationTipBuilder.Append(
                TranslationServerUtils.Translate(InputMap.ActionGetEvents("pick_up")[0].AsText()));
            operationTipBuilder.Append("[/color]");
            operationTipBuilder.Append(TranslationServerUtils.Translate("pick_up"));
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
            operationTipBuilder.Append(TranslationServerUtils.Translate("throw"));
            if (CurrentItem is WeaponTemplate weaponTemplate)
            {
                operationTipBuilder.Append(TranslationServerUtils.Translate(weaponTemplate.Name));
                //提示武器攻击
                operationTipBuilder.Append(' ');
                operationTipBuilder.Append("[color=");
                operationTipBuilder.Append(Config.OperationTipActionColor);
                operationTipBuilder.Append(']');
                operationTipBuilder.Append(
                    TranslationServerUtils.Translate(InputMap.ActionGetEvents("use_item")[0].AsText()));
                operationTipBuilder.Append("[/color]");
                operationTipBuilder.Append(TranslationServerUtils.Translate("use_item"));
                operationTipBuilder.Append(TranslationServerUtils.Translate(weaponTemplate.Name));
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
        velocity.X = axis * Speed;

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

                if (_floatLabel != null)
                {
                    _floatLabel.QueueFree();
                    _floatLabel = null;
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
            if (CurrentItem == null)
            {
                return;
            }

            if (_parabola != null)
            {
                _parabola.Points = new[] { Vector2.Zero };
            }

            CurrentItem.Reparent(GameSceneNodeHolder.WeaponContainer);
            switch (CurrentItem)
            {
                case WeaponTemplate weaponTemplate:
                {
                    var timer = new Timer();
                    weaponTemplate.AddChild(timer);
                    timer.WaitTime = _itemCollisionRecoveryTime;
                    timer.OneShot = true;
                    timer.Timeout += () =>
                    {
                        //We cannot immediately resume the physical collision when the weapon is discharged, which will cause the weapon to collide with the ground and platform earlier, preventing the weapon from flying.
                        //仍出武器时，我们不能立即恢复物理碰撞，立即恢复会导致武器更早的与地面和平台碰撞，阻止武器的飞行。
                        weaponTemplate.EnableContactInjury = true;
                        weaponTemplate.SetCollisionMaskValue(Config.LayerNumber.Ground, true);
                        weaponTemplate.SetCollisionMaskValue(Config.LayerNumber.Platform, true);
                        timer.QueueFree();
                    };
                    timer.Start();
                    weaponTemplate.Sleeping = false;
                    weaponTemplate.LinearVelocity = Vector2.Zero;
                    break;
                }
            }

            //We apply force to objects.
            //我们给物品施加力。
            switch (CurrentItem)
            {
                case CharacterBody2D characterBody2D:
                    characterBody2D.Velocity = GetThrowVelocity();
                    break;
                case RigidBody2D rigidBody2D:
                    rigidBody2D.LinearVelocity = GetThrowVelocity();
                    break;
            }

            CurrentItem = null;
            var hotBar = GameSceneNodeHolder.HotBar;
            hotBar?.RemoveItemFromItemSlotBySelectIndex(1);
        }
    }

    protected override void WhenUpdateCurrentItem(Node2D? currentItem)
    {
        UpdateOperationTip();
    }

    private Vector2 GetThrowVelocity()
    {
        //We take the mouse position, normalize it, and then multiply it by the distance the player can throw
        //我们拿到鼠标的位置，将其归一化处理，然后乘以玩家可扔出的距离
        return GetLocalMousePosition().Normalized() * _throwingVelocity;
    }

    public override void _Process(double delta)
    {
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
        if (CurrentItem is WeaponTemplate weapon)
        {
            weapon.Flip(FacingLeft);
        }
    }

    protected override void OnDie(DamageTemplate damageTemplate)
    {
        if (EventManager.GameOverEvent != null)
        {
            var gameOverEvent = new GameOverEvent
            {
                DeathInfo = "\"白纸\"失手将自己杀死。"
            };
            EventManager.GameOverEvent(gameOverEvent);
        }

        Visible = false;
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

        if (node is not Node2D node2D)
        {
            return;
        }

        if (_floatLabelPackedScene != null)
        {
            //If there is a scene of floating text, then we generate floating text.
            //如果有悬浮文本的场景，那么我们生成悬浮文本。
            _floatLabel?.QueueFree();
            _floatLabel = (Control)_floatLabelPackedScene.Instantiate();
            var rotationDegreesNode2D = node2D.RotationDegrees;
            var rotationDegreesNode2DAbs = Math.Abs(rotationDegreesNode2D);
            _floatLabel.Position = rotationDegreesNode2DAbs > 90
                ? new Vector2(0, PromptTextDistance)
                : new Vector2(0, -PromptTextDistance);
            _floatLabel.RotationDegrees = 0 - rotationDegreesNode2D;
            var label = _floatLabel.GetNode<Label>("Label");
            if (node is WeaponTemplate weapon)
            {
                var stringBuilder = new StringBuilder();
                if (weapon.Owner is CharacterTemplate characterTemplate)
                {
                    stringBuilder.Append(characterTemplate.ReadOnlyCharacterName);
                    stringBuilder.Append(TranslationServerUtils.Translate("de"));
                }

                stringBuilder.Append(TranslationServerUtils.Translate(weapon.Name));
                label.Text = stringBuilder.ToString();
            }

            node.AddChild(_floatLabel);
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

        if (_floatLabel != null)
        {
            _floatLabel.QueueFree();
            _floatLabel = null;
        }

        UpdateOperationTip();
    }

    protected override void OnHit(DamageTemplate damageTemplate)
    {
        base.OnHit(damageTemplate);
        var healthBarUi = GameSceneNodeHolder.HealthBarUi;
        if (healthBarUi != null)
        {
            healthBarUi.CurrentHp = CurrentHp;
        }
    }
}