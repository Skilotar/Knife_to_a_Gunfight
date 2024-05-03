using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using System.Reflection;
using Dungeonator;

namespace Knives
{
    public class AutonomusDefenseDrive : PassiveItem
    {

        public static void Register()
        {

            string itemName = "Autonomus Defense Drive";


            string resourceName = "Knives/Resources/A.D.D._001";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<AutonomusDefenseDrive>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Back Off!";
            string longDesc = "An asset protection neural drive created to be placed into the mind of the Great Queen Elizabeth the LXI's royal Guard.\n" +
                "If the drive detects an attack on the owner or her majesty it will cause the host body to spring into action involuntarily." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            // item.AddToSubShop(ItemBuilder.ShopType.Trorc, .01f);
            item.quality = PickupObject.ItemQuality.D;

           
            ID = item.PickupObjectId;
        }
        
        public static int ID;

        public override void Pickup(PlayerController player)
        {
           
            base.Pickup(player);
        }

        public float internalCooldown;
        public override void Update()
        {
            if (this.Owner)
            {
                RoomHandler room = this.Owner.CurrentRoom;
                if(room != null && internalCooldown <= 0 && !this.Owner.IsDodgeRolling)
                {
                    float dist = 2.8f;
                    AIActor ai = room.GetNearestEnemy(this.Owner.CenterPosition, out dist, false, true);
                    if(ai != null)
                    {
                        if (Vector2.Distance(this.Owner.CenterPosition, ai.CenterPosition) < 2.8f)
                        {
                            Vector2 dir = ai.CenterPosition - this.Owner.CenterPosition;
                            attackKick(dir);
                            if(this.Owner.PlayerHasActiveSynergy("Autonomus Ultra Instinct"))
                            {
                                internalCooldown = .15f;
                            }
                            else
                            {
                                internalCooldown = 1f;
                            }
                            
                            room = null;
                        }
                    }

                    if (this.Owner.PlayerHasActiveSynergy("Autonomus Ultra Instinct"))
                    {
                        foreach (var projectile in GetBullets())
                        {
                            if (Vector2.Distance(this.Owner.CenterPosition, projectile.sprite.WorldCenter) < 2f)
                            {
                                Vector2 dir = (Vector2)projectile.sprite.WorldCenter - this.Owner.CenterPosition;
                                attackKick(dir);
                                internalCooldown += .15f;
                                if(internalCooldown < 1) internalCooldown = 1;
                            }
                        }
                    }

                }

                

                if ( internalCooldown > 0)
                {
                    internalCooldown -= Time.deltaTime;
                }
                else
                {
                    if(this.Owner.PlayerHasActiveSynergy("Autonomus Ultra Instinct"))
                    {
                        GlobalSparksDoer.EmitFromRegion(GlobalSparksDoer.EmitRegionStyle.RADIAL, 4, .5f, this.Owner.sprite.WorldBottomLeft, this.Owner.sprite.WorldTopRight, Vector3.up, 0, 0, null, null, ExtendedColours.silvedBulletsSilver, GlobalSparksDoer.SparksType.SOLID_SPARKLES);
                    }
                }
            }

            base.Update();
        }

        public void attackKick(Vector2 dir)
        {
            
            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, this.Owner.CenterPosition, this.Owner, dir.ToAngle());
            ProjectileSlashingBehaviour stabby = proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            stabby.SlashDimensions = 30;
            stabby.SlashRange = 3.2f;
            stabby.SlashDamageUsesBaseProjectileDamage = false;
            stabby.SlashDamage = 5;
            stabby.slashKnockback = 40;
            stabby.InteractMode = SlashDoer.ProjInteractMode.IGNORE;
            if (this.Owner.PlayerHasActiveSynergy("Autonomus Ultra Instinct")) stabby.InteractMode = SlashDoer.ProjInteractMode.DESTROY;
            stabby.soundToPlay = "";
            stabby.SlashVFX = ((Gun)PickupObjectDatabase.GetById(335)).muzzleFlashEffects;

            FieldInfo leEnabler = typeof(PlayerController).GetField("m_handlingQueuedAnimation", BindingFlags.Instance | BindingFlags.NonPublic);
            leEnabler.SetValue(this.Owner, true);
            string text = GetAnimationName(this.Owner);
            string str = (!(this.Owner.CurrentGun == null) || this.Owner.ForceHandless) ? "_hand" : "_twohands";
            string text2 = (!this.Owner.UseArmorlessAnim) ? string.Empty : "_armorless";
            bool renderBodyhand = !this.Owner.ForceHandless && this.Owner.CurrentSecondaryGun == null && (this.Owner.CurrentGun == null || this.Owner.CurrentGun.Handedness != GunHandedness.TwoHanded);
            if (renderBodyhand && this.Owner.spriteAnimator.GetClipByName(text + str + text2) != null)
            {
                this.Owner.spriteAnimator.Play(text + str + text2);
            }
            else if (this.Owner.spriteAnimator.GetClipByName(text + text2) != null)
            {
                this.Owner.spriteAnimator.Play(text + text2);
            }
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
                    if (projectile.Owner != null)
                    {
                        if (projectile.collidesWithEnemies = false || projectile.isFakeBullet || projectile.Owner is AIActor || (projectile.Shooter != null && projectile.Shooter.aiActor != null) || projectile.ForcePlayerBlankable || projectile.IsBulletScript)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
        }


        public override DebrisObject Drop(PlayerController player)
        {
            
            return base.Drop(player);
        }

        public static string GetAnimationName(PlayerController player)
        {
            
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
