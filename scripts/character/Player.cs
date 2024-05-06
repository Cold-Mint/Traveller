using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColdMint.scripts;
using ColdMint.scripts.character;
using ColdMint.scripts.damage;
using ColdMint.scripts.debug;
using ColdMint.scripts.utils;
using ColdMint.scripts.weapon;

/// <summary>
/// <para>玩家角色</para>
/// </summary>
public partial class Player : CharacterTemplate
{
    private PackedScene FloatLabelPackedScene;

    protected Control FloatLabel;

    //Empty object projectile
    //空的物品抛射线
    private Vector2[] emptyVector2Array = new[] { Vector2.Zero };

    //抛物线
    private Line2D Parabola;

    //用于检测玩家是否站在平台上的射线
    private RayCast2D PlatformDetectionRayCast2D;

    //在拾捡范围内，可拾起的物品数量
    private int TotalNumberOfPickups = 0;

    private const float promptTextDistance = 50;

    //玩家可拾捡的物品
    private Node2D PickAbleItem;

    //抛出物品的飞行速度
    private float throwingVelocity = Config.CellSize * 13;

    //射线是否与平台碰撞
    private bool CollidingWithPlatform = false;

    //How long does it take for the character to recover from a collision with the platform after jumping off the platform (in seconds)
    //角色从平台上跳下后，多少时间后恢复与平台的碰撞（单位：秒）
    private double PlatformCollisionRecoveryTime = 0.2f;

    //物品被扔出后多长时间恢复与地面和平台的碰撞（单位：秒）
    private double ItemCollisionRecoveryTime = 0.045f;


    public override void _Ready()
    {
        base._Ready();
        CharacterName = TranslationServer.Translate("default_player_name");
        FloatLabelPackedScene = GD.Load<PackedScene>("res://prefab/ui/FloatLabel.tscn");
        Parabola = GetNode<Line2D>("Parabola");
        PlatformDetectionRayCast2D = GetNode<RayCast2D>("PlatformDetectionRayCast");
        UpdateOperationTip();
        GameSceneNodeHolder.HealthBarUi.MaxHp = MaxHp;
        GameSceneNodeHolder.HealthBarUi.CurrentHp = CurrentHp;
    }

    /// <summary>
    /// <para>Update operation prompt</para>
    /// <para>更新操作提示</para>
    /// </summary>
    private void UpdateOperationTip()
    {
        var operationTipBuilder = new StringBuilder();
        if (TotalNumberOfPickups > 0)
        {
            //If there's anything around to pick up
            //如果周围有能捡的东西
            if (CurrentItem == null)
            {
                if (PickAbleItem != null)
                {
                    string name = null;
                    if (PickAbleItem is WeaponTemplate weaponTemplate)
                    {
                        //When the weapon has no owner, a pick up prompt is displayed.
                        //当武器没有主人时，显示捡起提示。
                        if (weaponTemplate.Owner == null || weaponTemplate.Owner == this)
                        {
                            name = TranslationServer.Translate(weaponTemplate.Name);
                        }
                    }

                    if (name != null)
                    {
                        operationTipBuilder.Append(
                            TranslationServer.Translate(InputMap.ActionGetEvents("pick_up")[0].AsText()));
                        operationTipBuilder.Append(TranslationServer.Translate("pick_up"));
                        operationTipBuilder.Append(name);
                    }
                }
            }
            else
            {
                string pickAbleItemName = null;
                string currentItemName = null;
                string mustBeThrown = TranslationServer.Translate("must_be_thrown");
                if (PickAbleItem != null)
                {
                    //可捡的物品是武器
                    if (PickAbleItem is WeaponTemplate weaponTemplate)
                    {
                        pickAbleItemName = TranslationServer.Translate(weaponTemplate.Name);
                    }
                }

                if (CurrentItem != null)
                {
                    //当前持有的物品是武器
                    if (CurrentItem is WeaponTemplate weaponTemplate)
                    {
                        currentItemName = TranslationServer.Translate(weaponTemplate.Name);
                    }
                }

                if (pickAbleItemName != null && currentItemName != null && mustBeThrown != "must_be_thrown")
                {
                    operationTipBuilder.Append(string.Format(mustBeThrown, currentItemName, pickAbleItemName));
                    operationTipBuilder.Append(' ');
                    operationTipBuilder.Append(
                        TranslationServer.Translate(InputMap.ActionGetEvents("throw")[0].AsText()));
                    operationTipBuilder.Append(TranslationServer.Translate("throw"));
                    operationTipBuilder.Append(currentItemName);
                }
            }

            GameSceneNodeHolder.OperationTipLabel.Text = operationTipBuilder.ToString();
            return;
        }

        operationTipBuilder.Append(TranslationServer.Translate(InputMap.ActionGetEvents("ui_left")[0].AsText()));
        operationTipBuilder.Append(TranslationServer.Translate("move_left"));
        operationTipBuilder.Append(' ');
        operationTipBuilder.Append(TranslationServer.Translate(InputMap.ActionGetEvents("ui_right")[0].AsText()));
        operationTipBuilder.Append(TranslationServer.Translate("move_right"));
        operationTipBuilder.Append(' ');
        operationTipBuilder.Append(TranslationServer.Translate(InputMap.ActionGetEvents("ui_up")[0].AsText()));
        operationTipBuilder.Append(TranslationServer.Translate("jump"));
        if (CollidingWithPlatform)
        {
            operationTipBuilder.Append(' ');
            operationTipBuilder.Append(TranslationServer.Translate(InputMap.ActionGetEvents("ui_down")[0].AsText()));
            operationTipBuilder.Append(TranslationServer.Translate("jump_down"));
        }

        if (CurrentItem != null)
        {
            operationTipBuilder.Append(' ');
            operationTipBuilder.Append(TranslationServer.Translate(InputMap.ActionGetEvents("throw")[0].AsText()));
            operationTipBuilder.Append(TranslationServer.Translate("throw"));
            if (CurrentItem is WeaponTemplate weaponTemplate)
            {
                operationTipBuilder.Append(TranslationServer.Translate(weaponTemplate.Name));
                //提示武器攻击
                operationTipBuilder.Append(' ');
                operationTipBuilder.Append(
                    TranslationServer.Translate(InputMap.ActionGetEvents("use_item")[0].AsText()));
                operationTipBuilder.Append(TranslationServer.Translate("use_item"));
                operationTipBuilder.Append(TranslationServer.Translate(weaponTemplate.Name));
            }
        }

        GameSceneNodeHolder.OperationTipLabel.Text = operationTipBuilder.ToString();
    }


    protected override void HookPhysicsProcess(ref Vector2 velocity, double delta)
    {
        //When the collision state between the platform detection ray and the platform changes
        //在平台检测射线与平台碰撞状态改变时
        if (PlatformDetectionRayCast2D.IsColliding() != CollidingWithPlatform)
        {
            //When the state changes, update the action hint
            //当状态改变时，更新操作提示
            CollidingWithPlatform = PlatformDetectionRayCast2D.IsColliding();
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
            var success = PickItem(PickAbleItem);
            if (success)
            {
                PickAbleItem = null;
                TotalNumberOfPickups--;
                if (FloatLabel != null)
                {
                    FloatLabel.QueueFree();
                    FloatLabel = null;
                }

                UpdateOperationTip();
            }
        }

        if (Input.IsActionJustPressed("ui_down"))
        {
            if (CollidingWithPlatform)
            {
                //When the character stands on the platform and presses the ui_down key, we cancel the collision between the character and the platform
                //当角色站在平台上按下 ui_down 键时，我们取消角色与平台的碰撞
                var timer = new Timer();
                AddChild(timer);
                timer.WaitTime = PlatformCollisionRecoveryTime;
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
            if (CurrentItem == null)
            {
                Parabola.Points = emptyVector2Array;
            }
            else
            {
                Parabola.Points =
                    ParabolicUtils.ComputeParabolic(ItemMarker2D.Position, GetThrowVelocity(), Gravity, 0.1f);
            }
        }


        //When you raise your hand, throw the object
        //抬起手时，抛出物品
        if (Input.IsActionJustReleased("throw"))
        {
            if (CurrentItem != null)
            {
                Parabola.Points = new[] { Vector2.Zero };
                CurrentItem.Reparent(GameSceneNodeHolder.WeaponContainer);
                if (CurrentItem is WeaponTemplate weaponTemplate)
                {
                    var timer = new Timer();
                    weaponTemplate.AddChild(timer);
                    timer.WaitTime = ItemCollisionRecoveryTime;
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
                }

                if (CurrentItem is CharacterBody2D characterBody2D)
                {
                    characterBody2D.Velocity = GetThrowVelocity();
                }

                if (CurrentItem is RigidBody2D rigidBody2D)
                {
                    rigidBody2D.LinearVelocity = GetThrowVelocity();
                }

                CurrentItem = null;
                TotalNumberOfPickups++;
                GameSceneNodeHolder.HotBar.RemoveItemFromItemSlotBySelectIndex(1);
                UpdateOperationTip();
            }
        }
    }


    private Vector2 GetThrowVelocity()
    {
        //We take the mouse position, normalize it, and then multiply it by the distance the player can throw
        //我们拿到鼠标的位置，将其归一化处理，然后乘以玩家可扔出的距离
        return GetLocalMousePosition().Normalized() * throwingVelocity;
    }

    public override void _Process(double delta)
    {
        AimTheCurrentItemAtAPoint(GetGlobalMousePosition());
        var itemMarker2DPosition = ItemMarker2D.Position;
        var axis = Input.GetAxis("ui_left", "ui_right");
        switch (axis)
        {
            case -1:
                //Minus 1, we move to the left
                //-1，向左移动
                FacingLeft = true;
                itemMarker2DPosition.X = -ReadOnlyItemMarkerOriginalX;
                Flip();
                break;
            case 1:
                //1, move to the right
                //1，向右移动
                FacingLeft = false;
                itemMarker2DPosition.X = ReadOnlyItemMarkerOriginalX;
                Flip();
                break;
            default:
                //0, when it's not pressed
                //0，没有按下时
                break;
        }

        ItemMarker2D.Position = itemMarker2DPosition;
    }

    protected override void Flip()
    {
        base.Flip();
        //If there is a weapon, flip it too
        //如果有武器的话，也要翻转
        if (CurrentItem != null)
        {
            if (CurrentItem is WeaponTemplate weapon)
            {
                weapon.Flip(FacingLeft);
            }
        }
    }

    protected override void EnterThePickingRangeBody(Node node)
    {
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

        var node2D = node as Node2D;
        TotalNumberOfPickups++;
        PickAbleItem = node2D;
        if (FloatLabel != null)
        {
            FloatLabel.QueueFree();
        }

        FloatLabel = (Control)FloatLabelPackedScene.Instantiate();
        var rotationDegreesNode2D = node2D.RotationDegrees;
        var rotationDegreesNode2DAbs = Math.Abs(rotationDegreesNode2D);
        if (rotationDegreesNode2DAbs > 90)
        {
            FloatLabel.Position = new Vector2(0, promptTextDistance);
        }
        else
        {
            FloatLabel.Position = new Vector2(0, -promptTextDistance);
        }

        FloatLabel.RotationDegrees = 0 - rotationDegreesNode2D;
        var label = FloatLabel.GetNode<Label>("Label");
        if (node is WeaponTemplate weapon)
        {
            var stringBuilder = new StringBuilder();
            if (weapon.Owner != null)
            {
                if (weapon.Owner is CharacterTemplate characterTemplate)
                {
                    stringBuilder.Append(characterTemplate.ReadOnlyCharacterName);
                    stringBuilder.Append(TranslationServer.Translate("de"));
                }
            }

            stringBuilder.Append(TranslationServer.Translate(weapon.Name));
            label.Text = stringBuilder.ToString();
        }

        node.AddChild(FloatLabel);
        UpdateOperationTip();
    }

    protected override void ExitThePickingRangeBody(Node node)
    {
        if (node is not Node2D)
        {
            return;
        }

        TotalNumberOfPickups--;
        if (TotalNumberOfPickups == 0)
        {
            //Set to null if there are no more items to pick up
            //如果没有可捡的物品了，设置为null
            PickAbleItem = null;
        }

        if (FloatLabel != null)
        {
            FloatLabel.QueueFree();
            FloatLabel = null;
        }

        UpdateOperationTip();
    }

    protected override void OnHit(DamageTemplate damageTemplate)
    {
        base.OnHit(damageTemplate);
        GameSceneNodeHolder.HealthBarUi.CurrentHp = CurrentHp;
    }
}