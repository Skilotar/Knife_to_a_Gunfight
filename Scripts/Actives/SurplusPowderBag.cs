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
    class Surplus_Powder_bag : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Imported Powder Bag";

            string resourceName = "Knives/Resources/PowderBag_idle_001";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Surplus_Powder_bag>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Surplus";
            string longDesc = 
                "One of many bags of gunpowder purchased from off planet by the gungeoneers. \n\nAfflicts enemies with[BlastBlight].Afflicted enemies will explode after 3 instances of damage or on death. " +

                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 300f);
            item.usableDuringDodgeRoll = true;

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void DoEffect(PlayerController player)
        {
            player.DidUnstealthyAction();
            for (int i = 0; i < 12; i++)
            {
                int vari = UnityEngine.Random.Range(-20, 20);
                Projectile projectile = ((Gun)ETGMod.Databases.Items[15]).DefaultModule.projectiles[0];
                GameObject gameObject = SpawnManager.SpawnProjectile(projectile.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + vari), true);
                Projectile component = gameObject.GetComponent<Projectile>();
                bool flag2 = component != null;
                if (flag2)
                {
                    component.Owner = player;
                    component.Shooter = player.specRigidbody;
                    component.Speed *= 1.3f;
                    component.baseData.damage = 0f;
                    component.OnHitEnemy = (Action<Projectile, SpeculativeRigidbody, bool>)Delegate.Combine(projectile.OnHitEnemy, new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy));
                    component.HasDefaultTint = true;
                    component.DefaultTintColor = new Color(.70f, .40f, .24f);
                    component.CurseSparks = true;

                    component.gameObject.GetOrAddComponent<SlowingBulletsEffect>();

                }

            }

        }
        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if (arg2 && arg2.aiActor)
            {
                AIActor aiActor = arg2.aiActor;
                if (aiActor.IsNormalEnemy && !aiActor.IsHarmlessEnemy)
                {

                    BlastBlightedStatusController boom = aiActor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                    boom.statused = true;

                }
            }
        }


        protected void EndEffect(PlayerController user)
        {

        }
  
        public override void Update()
        {

            if (this.LastOwner != null)
            {

                this.CanBeUsed(this.LastOwner);



            }
            base.Update();
        }


    }
}