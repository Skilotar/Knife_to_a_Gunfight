using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;

namespace Knives
{
    public class Overkill : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Overkill Capacitor";

            string resourceName = "Knives/Resources/Overkill";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Overkill>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Fatal Capacity";
            string longDesc = "When Killing an enemy, half of the final damage instance is transfered over to the nearest enemy. A capacitor that was accidentally made by a necromancer trying to fix his computer." +
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            

            item.quality = PickupObject.ItemQuality.B;


        }
        public override void Pickup(PlayerController player)
        {
            player.OnDealtDamageContext += this.OnDealtDamageContext;
            
            base.Pickup(player);

        }


        public float raduis = 30f;

        public void OnDealtDamageContext(PlayerController player, float damage, bool fatal, HealthHaver hitenemy)
        {
            if (hitenemy != null)
            {
                if (fatal)
                {
                    // on fatal hit will spawn two connecting projectiles that will form a chain lightning and disipate quickly

                    RoomHandler room = player.CurrentRoom;
                    AIActor enemy = room.GetNearestEnemy(hitenemy.sprite.WorldCenter, out raduis, true, true);
                    if(enemy != null)
                    {
                        enemy.healthHaver.ApplyDamage(damage/2, Vector2.zero, "overkill", CoreDamageTypes.Electric, DamageCategory.Unstoppable);
                        GlobalSparksDoer.DoRandomParticleBurst(40, enemy.sprite.WorldBottomLeft, enemy.sprite.WorldTopRight, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.SOLID_SPARKLES);

                    }

                }
            }
        }

      

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnDealtDamageContext -= this.OnDealtDamageContext;
            return base.Drop(player);
        }
    }
}