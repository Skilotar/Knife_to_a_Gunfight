using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class power_bracer : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Power Bracer";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/power_bracer";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<power_bracer>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Sweet Revenge";
            string longDesc = "Upon taking a hit the kinetic force is stored in the gem of this bracer. When activated this force is released.  \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 20f);
            
            //Set the rarity of the item
            
           

            item.quality = PickupObject.ItemQuality.B;
           
        }

        public override void Update()
        {

            base.Update();
            if(this.LastOwner != null)
            {
                healthchecker();

            }

        }

        public float lastknownhealth;
        public float energy;
        public void healthchecker()
        {
            float health = this.LastOwner.healthHaver.GetCurrentHealth();

            if (health < lastknownhealth)
            {
                energy++;
                AkSoundEngine.PostEvent("Play_OBJ_metronome_jingle_01", base.gameObject);
            }
            lastknownhealth = health;
        }
        public override void  DoEffect(PlayerController user)
        {
            energy = (energy * 20) + 10;
            AkSoundEngine.PostEvent("Play_ENM_hammer_smash_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_hammer_smash_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_ENM_hammer_smash_01", base.gameObject);
            RoomHandler room = user.CurrentRoom;
            if (!room.HasActiveEnemies(RoomHandler.ActiveEnemyType.All)) return;
            foreach (var enemy in room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
            {
                enemy.healthHaver.ApplyDamage(energy, Vector2.zero, "bracer", CoreDamageTypes.Magic, DamageCategory.Unstoppable,true, null,true);
                
            }
            energy = 0f;
        }
    }

    
}
