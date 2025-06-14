using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using System.Reflection;
using HutongGames.PlayMaker.Actions;

namespace Knives
{
    class Long_roll_boots : PassiveItem
    {

        public static void Register()
        {
            string itemName = "Long Roll Boots";

            string resourceName = "Knives/Resources/long_roll_boots";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Long_roll_boots>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Asset Protection";
            string longDesc = "An apeture science prototype for the long fall boots. This early model attempted to negate falling injuries by slowing the timestream of the person jumping. \n\n Turns out slowing time doesnt slow momentum. The subjects still splatted... just really slowly.\n\n" +
                "Oh well all in the name of science I guess. This specific version of the boots is equiped with temporally repositioning airbreaks which let you control your movement mid roll. Good luck and happy rolling." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollDistanceMultiplier, 1.05f, StatModifier.ModifyMethod.MULTIPLICATIVE);

            item.quality = PickupObject.ItemQuality.C;
            item.RespawnsIfPitfall = true;
            item.IgnoredByRat = true;
            ID = item.PickupObjectId;
        }

        public static int ID;
        bool toggleknockbackeffect = false;
        public override void Pickup(PlayerController player)
        {
          
            player.OnPreDodgeRoll += this.OnPreDodgeRoll;
            
            base.Pickup(player);
        }

        private void OnPreDodgeRoll(PlayerController player)
        {
            toggleknockbackeffect = true;
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreDodgeRoll -= this.OnPreDodgeRoll;
            return base.Drop(player);
        }
        public bool doingRollLengthen = false;
        public float ClipFPS = 0;
        
        public override void  Update()
        {
            if (this.Owner != null)
            {
                if (Time.timeScale <= 0f)
                {

                }
                else
                {

                    PlayerController player = (PlayerController)this.Owner;
                    if (toggleknockbackeffect && player.IsDodgeRolling && player.CurrentRollState == PlayerController.DodgeRollState.InAir)
                    {
                        Vector2 LockedDir = GetPrivateType<PlayerController, Vector2>(player, "lockedDodgeRollDirection");
                        float DotProd = Vector2.Dot(LockedDir, player.NonZeroLastCommandedDirection);
                        if(DotProd <= .70f)
                        {
                            player.knockbackDoer.ApplyKnockback(player.NonZeroLastCommandedDirection, .65f * (player.stats.MovementSpeed / 7), true);
                        }
                        
                    }
                    else
                    {
                        if (toggleknockbackeffect == true)
                        {
                            //player.knockbackDoer.ApplyKnockback(player.NonZeroLastCommandedDirection * -1, .35f);
                            toggleknockbackeffect = false;
                        }

                    }

                    if (player.IsOverPitAtAll && player.IsDodgeRolling && !player.IsSlidingOverSurface && !player.IsFlying && AnimationIsPlaying(GetPrivateType<PlayerController, Vector2>(player, "lockedDodgeRollDirection"))) // lengthen roll until player is no longer over a pit or animation ends
                    {
                        float DogRollTime = GetPrivateType<PlayerController, float>(player, "m_dodgeRollTimer");
                        DogRollTime -= BraveTime.DeltaTime;
                        SetPrivateType<PlayerController>(player, "m_dodgeRollTimer", DogRollTime);
                        player.spriteAnimator.OverrideTimeScale = .45f;

                        doingRollLengthen = true;
                    }
                    else
                    {
                        if(doingRollLengthen == true)
                        {
                            player.spriteAnimator.OverrideTimeScale = -1;
                            doingRollLengthen = false;
                        }
                    }
                }

            }
               
         


            base.Update();
        }

        private bool AnimationIsPlaying(Vector2 direction)
        {
            if(direction != null)
            {
                tk2dSpriteAnimationClip tk2dSpriteAnimationClip = null;
                PlayerController player = this.Owner;
                if (Mathf.Abs(direction.x) < 0.1f)
                {
                    tk2dSpriteAnimationClip = player.spriteAnimator.GetClipByName(((direction.y <= 0.1f) ? "dodge" : "dodge_bw") + ((!player.UseArmorlessAnim) ? string.Empty : "_armorless"));
                }
                else
                {
                    tk2dSpriteAnimationClip = player.spriteAnimator.GetClipByName(((direction.y <= 0.1f) ? "dodge_left" : "dodge_left_bw") + ((!player.UseArmorlessAnim) ? string.Empty : "_armorless"));
                }

                if (player.spriteAnimator.IsPlaying(tk2dSpriteAnimationClip))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;

        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }
    }
}
