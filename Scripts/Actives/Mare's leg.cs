using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using System.Reflection;
using Gungeon;

namespace Knives
{
    class Mares_Leg : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Mares Leg";

            string resourceName = "Knives/Resources/Shotgun_Leg";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Mares_Leg>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Kickback";
            string longDesc = "Roll to reload \nHold two shots" +
                "\n\n" +
                
                "Mary Jane Mare, the notorious outlaw of the wild gundrominian region was knocked down in her prime by an accident involving getting kicked in the leg by a horse. " +
                "Mary's leg suffered major internal damage and the bandit doctor thought it best to amputate. " +
                "Mary replaced her missing leg with a levering shotgun and the rest is history. " +
                 
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, .5f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 20);
            

            item.UsesNumberOfUsesBeforeCooldown = true;
            item.numberOfUses = 2;
            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }
        public static bool shouldBeFlipped = false;
        public static int ID;
        public override void Pickup(PlayerController player)
        {

            player.OnPreDodgeRoll += Player_OnPreDodgeRoll;
            base.Pickup(player);
        }

        private void Player_OnPreDodgeRoll(PlayerController player)
        {

            if(player.PlayerHasActiveSynergy("Bandit Extraordinaire"))
            {
                FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
                remainingTimeCooldown.SetValue(this, 0);
                this.numberOfUses = 2;
            }
            else
            {
                if (numberOfUses < 2)
                {
                    this.numberOfUses++;

                }
                if (this.IsOnCooldown)
                {
                    FieldInfo remainingTimeCooldown = typeof(PlayerItem).GetField("remainingTimeCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
                    remainingTimeCooldown.SetValue(this, 0);
                    this.numberOfUses = 1;
                }
            }
           
        }

        public override void  OnPreDrop(PlayerController player)
        {

            player.OnPreDodgeRoll -= Player_OnPreDodgeRoll;
            base.OnPreDrop(player);
        }
        public override void  DoEffect(PlayerController player)
        {
            player.DidUnstealthyAction();
            StartCoroutine(ItemBuilder.HandleDuration(this, .5f, player, EndEffect));
            if(player.PlayerHasActiveSynergy("Scuffed Soles"))
            {
                Vector2 vector = player.unadjustedAimPoint.XY() - player.CenterPosition;
                player.knockbackDoer.ApplyKnockback(vector, -40, false);
               
                StartCoroutine(NoFall());
            }
            //Does Shotgun
            StartCoroutine(Shotkick(player));
            AkSoundEngine.PostEvent("Play_half_gauge_fire", base.gameObject);

            //plays Kick
            FieldInfo leEnabler = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(player, true);
            string text = GetAnimationName(player);
            string str = (!(player.CurrentGun == null) || player.ForceHandless) ? "_hand" : "_twohands";
            string text2 = (!player.UseArmorlessAnim) ? string.Empty : "_armorless";
            bool renderBodyhand = !player.ForceHandless && player.CurrentSecondaryGun == null && (player.CurrentGun == null || player.CurrentGun.Handedness != GunHandedness.TwoHanded);
            if (renderBodyhand && player.spriteAnimator.GetClipByName(text + str + text2) != null)
            {
                player.spriteAnimator.Play(text + str + text2);
            }
            else if (player.spriteAnimator.GetClipByName(text + text2) != null)
            {
                player.spriteAnimator.Play(text + text2);
            }
            
        }

        private IEnumerator NoFall()
        {
            this.LastOwner.SetIsFlying(true, "kickback");
            yield return new WaitForSeconds(.3f);
            this.LastOwner.SetIsFlying(false, "kickback");
        }

        protected void EndEffect(PlayerController user)
        {

        }
        public IEnumerator Shotkick(PlayerController player)
        {
            yield return new WaitForSeconds(.1f);
            
            int angle = -8;

            for (int i = 0; i < 9; i++)
            {
                float angleadjusted = angle;
                if (angle == -6)// set top side pattern
                {
                    angleadjusted = -7.5f;
                }
                
                if (angle == 6)// set bottom side pattern
                {
                    angleadjusted = 7.5f;
                }
                Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + angleadjusted), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.baseData.damage = 3f;
                    component.baseData.speed *= (1f - (Math.Abs(angle) / 40));
                    component.UpdateSpeed();
                    if (UnityEngine.Random.Range(1, 11) == 1)
                    {
                        player.DoPostProcessProjectile(component);
                    }
                }
                angle++;
                angle++;
            }

            Projectile projectile2 = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
            GameObject gameObject2 = SpawnManager.SpawnProjectile(projectile2.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component2 = gameObject2.GetComponent<Projectile>();
            bool flag = component2 != null;
            if (flag)
            {
                component2.Owner = player;
                component2.Shooter = player.specRigidbody;
                component2.baseData.damage = 10f;
                ProjectileSlashingBehaviour slashy = component2.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                Gun swipeFlash = (Gun)PickupObjectDatabase.GetById(335);
                slashy.SlashVFX = swipeFlash.muzzleFlashEffects;
                slashy.slashKnockback = -4f;
            }

        }

        public static string GetAnimationName(PlayerController player)
        {
            shouldBeFlipped = false;
           
            if (player.CurrentGun.CurrentAngle <= 45 && player.CurrentGun.CurrentAngle >= -45)// right
            {
                return "tablekick_right";
            }
            if (player.CurrentGun.CurrentAngle > 45 && player.CurrentGun.CurrentAngle <= 135)// up
            {
                return "tablekick_up";
            }
            if (player.CurrentGun.CurrentAngle > 135 || player.CurrentGun.CurrentAngle <= -135)// left
            {
                shouldBeFlipped = true;
                return "tablekick_right";
            }
            if (player.CurrentGun.CurrentAngle < -45 && player.CurrentGun.CurrentAngle >= -135)// down
            {
                return "tablekick_down";
            }
            return "error";
      
        }

       
    }
}

