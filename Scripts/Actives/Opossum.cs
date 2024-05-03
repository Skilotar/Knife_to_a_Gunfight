using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using System.Reflection;
using Gungeon;
using System.Collections.Generic;

namespace Knives
{
    class Opossum_ring : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Opossum Ring";

            string resourceName = "Knives/Resources/Opossum_ring";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Opossum_ring>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Agens Mortem";
            string longDesc = "Fake your death near bullets to gain stealth for sneak attacks.\n\n" +
                "A ring that allows its wearer acting skill at the peak of their physical ability. A gift from two brothers of the planet thespos where the opossum is a protected planetary species." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 120);
            

            item.quality = PickupObject.ItemQuality.C;
        }
        public static bool shouldBeFlipped = false;

        public override void Pickup(PlayerController player)
        { 
            base.Pickup(player);
        }

        
        public override void  OnPreDrop(PlayerController player)
        {
            base.OnPreDrop(player);
        }
        public override void Update()
        {

            if (this.LastOwner)
            {
                this.CanBeUsed(this.LastOwner);
                proximity(this.LastOwner);
            }


            base.Update();
        }
        bool near_bullet = false;
        public void proximity(PlayerController player)
        {

            near_bullet = false;
            Vector2 standing = player.CenterPosition;
            foreach (var projectile in GetBullets())
            {
                if (player.healthHaver.IsAlive)
                {
                    Vector2 bullet = projectile.sprite.WorldCenter;

                    float radius = 2f;
                    if (Vector2.Distance(bullet, standing) < radius )
                    {
                        near_bullet = true;
                    }
                }
            }
            

        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && near_bullet && !base.IsOnCooldown;
        }


        private static List<Projectile> GetBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    
                    if (projectile.collidesWithEnemies = false || projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable || projectile.IsBulletScript)
                    {
                        list.Add(projectile);
                       
                    }
                    
                }
            }
            return list;
        }

        public override void  DoEffect(PlayerController player)
        {   //Duration
            StartCoroutine(ItemBuilder.HandleDuration(this, 5f, player, EndEffect));
            player.healthHaver.IsVulnerable = false;
            //plays death
            StartCoroutine(doAnimation(player));

            //stealthy bits
            StartCoroutine(DoStealthyBits(player));
            player.SetIsStealthed(true, "Looks Dead to me");
            //no move
            player.MovementModifiers += this.NoMotionModifier;
            player.IsStationary = true;

            //sound
            if(UnityEngine.Random.Range(1,2) == 1)
            {
                AkSoundEngine.PostEvent("Play_CHR_general_death_01", base.gameObject);
            }
            else
            {
                AkSoundEngine.PostEvent("Play_CHR_general_death_02", base.gameObject);
            }
        }

        private IEnumerator DoStealthyBits(PlayerController player)
        {
            yield return new WaitForSeconds(1.2f);
            player.IsStationary = false;
            player.MovementModifiers -= this.NoMotionModifier;
            player.healthHaver.IsVulnerable = true;
            player.ChangeSpecialShaderFlag(1, 1f);
            player.OnDidUnstealthyAction += this.BreakStealth;
            player.healthHaver.OnDamaged += this.OnDamaged;
            
            player.SetCapableOfStealing(true, "Dead", null);
        }
        private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
        {
            voluntaryVel = Vector2.zero;
        }

        private void BreakStealth(PlayerController obj)
        {
            obj.ChangeSpecialShaderFlag(1, 0f);
            obj.OnDidUnstealthyAction -= this.BreakStealth;
            obj.healthHaver.OnDamaged -= this.OnDamaged;
            obj.SetIsStealthed(false, "Looks Dead to me");
            obj.SetCapableOfStealing(false, "Dead", null);

        }
        private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            this.BreakStealth(this.LastOwner as PlayerController);
        }
       
        private IEnumerator doAnimation(PlayerController player)
        {
            yield return new WaitForSeconds(.01f);
            FieldInfo leEnabler = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(player, true);
            string text = "death";
            string deathshot = "deathshot";
            string str = (!(player.CurrentGun == null) || player.ForceHandless) ? "_hand" : "_twohands";
            string text2 = (!player.UseArmorlessAnim) ? string.Empty : "_armorless";
            bool renderBodyhand = !player.ForceHandless && player.CurrentSecondaryGun == null && (player.CurrentGun == null || player.CurrentGun.Handedness != GunHandedness.TwoHanded);
            if (renderBodyhand && player.spriteAnimator.GetClipByName(text + str + text2) != null)
            {

                player.spriteAnimator.Play(text + str + text2);
                yield return new WaitForSeconds(1);
                player.spriteAnimator.Play(deathshot + str + text2);



            }
            else if (player.spriteAnimator.GetClipByName(text + text2) != null)
            {
                player.spriteAnimator.Play(text + text2);
               
            }

        }

        protected void EndEffect(PlayerController user)
        {
            this.BreakStealth(user);
        }
       
       
    }
}
