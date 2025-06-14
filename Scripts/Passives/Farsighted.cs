using System;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;
using MonoMod.Utils;
using MonoMod;
using System.Runtime.CompilerServices;

namespace Knives
{
    public class Farsighted : PassiveItem
    {
        

        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Farsighted bullets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/farsighted_bullets";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Farsighted>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Vision Test";
            string longDesc =

                "This New bullet type spontaniously generated one day when Homing bullets lost its glasses." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Accuracy, -4, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.RangeMultiplier, 3, StatModifier.ModifyMethod.ADDITIVE);

            //Set the rarity of the item


            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }
        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
           
            base.Pickup(player);
        }

        private void PostProcessProjectile(Projectile SourceProjectile, float chance)
        {
            if(this.Owner.PlayerHasActiveSynergy("Full Spectrum Eye Exam"))
            {
                SourceProjectile.OnHitEnemy += this.HitEnemy;
            }
            StartCoroutine(direction(SourceProjectile));
        }

        private void HitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2.aiActor != null)
            {
                if(arg3 == false)
                {
                    if(Vector2.Distance(this.Owner.CenterPosition, arg2.aiActor.Position) >= 6)
                    {
                        arg2.healthHaver.ApplyDamage(arg1.baseData.damage * .5f, Vector2.zero, "I can see you");
                    }
                }
            }
        }

        public IEnumerator direction(Projectile bullet)
        {
            if (bullet != null)
            {


                BraveInput instanceForPlayer = BraveInput.GetInstanceForPlayer(this.Owner.PlayerIDX);
                if (instanceForPlayer != null)
                {
                    if (instanceForPlayer.IsKeyboardAndMouse() == false)
                    {
                        Projectile projectile2 = bullet;
                        GameObject gameObject = SpawnManager.SpawnProjectile(projectile2.gameObject, this.Owner.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (this.Owner.CurrentGun == null) ? 0f : this.Owner.CurrentGun.CurrentAngle), true);
                        Projectile component = gameObject.GetComponent<Projectile>();
                        PierceProjModifier peirce = gameObject.GetOrAddComponent<PierceProjModifier>();
                        peirce.penetration = 90;
                        bool flag = component != null;
                        if (flag)
                        {
                            component.Owner = this.Owner;
                            component.Shooter = this.Owner.specRigidbody;
                            component.baseData.damage = 0f;
                            component.baseData.speed *= 1.8f;
                            component.AdditionalScaleMultiplier = .0001f;
                        }

                        yield return new WaitForSecondsRealtime(.25f);
                        if (bullet.gameObject != null)
                        {
                            Vector2 vector = bullet.sprite.WorldCenter;
                            Vector2 Aim = component.sprite.WorldCenter;
                            bullet.SendInDirection(Aim - vector, false, true);
                        }

                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(.25f);
                        if (bullet != null)
                        {
                            Vector2 vector = bullet.sprite.WorldCenter;
                            Vector2 Aim = this.Owner.unadjustedAimPoint;
                            bullet.SendInDirection(Aim - vector, false, true);
                        }
                    }
                }
            }
        }
       
       
        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);
        }
        
    }
}
