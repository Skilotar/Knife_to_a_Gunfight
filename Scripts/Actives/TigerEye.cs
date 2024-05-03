using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class Eye_of_the_tiger : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Tiger Eye";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Eye_of_the_tiger";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Eye_of_the_tiger>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Blind Rage";
            string longDesc = "It's the eye of that tiger\n" +
                "It's the thrill of the fright\n" +
                "Running away from the rage of that tiger\n" +
                "And the last known survivor is fleeing the fight\n" +
                "It's all from stealing the eyeeeeeeeee of that tiger!!!!\n\n______________________________\n"+
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 100f);
            //Adds the actual passive effect to the item

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.D;

            ID = item.PickupObjectId;
        }
        public static int ID;
        
        public override void  DoEffect(PlayerController user)
        {
            Vector2 targetPosition = Vector2.zero;
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.MovementSpeed, 4f, StatModifier.ModifyMethod.ADDITIVE);
            Vector2 vector = targetPosition - base.transform.position.XY();
            float z = Mathf.Atan2(vector.y, vector.x) * 57.29578f;
            user.healthHaver.IsVulnerable = false;


            for(int i = 0; i< 5; i++)
            {
                Projectile projectile = ((Gun)ETGMod.Databases.Items[369]).DefaultModule.projectiles[0];
                GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile.gameObject, base.transform.position.XY(), Quaternion.Euler(0f, 0f, z), true);
                Projectile component1 = gameObject1.GetComponent<Projectile>();
                component1.AppliesKnockbackToPlayer = false;
                component1.AdditionalScaleMultiplier = .5f;
                component1.baseData.damage = 0;

            }


            float dura = 10f;
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
            StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));
            StartCoroutine(iframewearoff());
        }

        private IEnumerator iframewearoff()
        {
            yield return new WaitForSeconds(.1f);
            this.LastOwner.healthHaver.IsVulnerable = true;
        }

        protected void EndEffect(PlayerController user)
        {
           
            ItemBuilder.AddPassiveStatModifier(this, PlayerStats.StatType.MovementSpeed, -4f, StatModifier.ModifyMethod.ADDITIVE);
            this.LastOwner.stats.RecalculateStats(LastOwner, true);
        }
    }
}

