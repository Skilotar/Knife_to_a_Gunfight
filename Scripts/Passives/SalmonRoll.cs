using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Gungeon;



namespace Knives
{
    class Salmon_roll : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Salmon Roll";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Salmon_roll";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Salmon_roll>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Jumpy";
            string longDesc = "This tightly packed sushi roll is fairly common in the gungeon. Which is strange as no fish actually inhabit the gungeons grimey water. \n\n" +
            "The meat in this roll taste creamy and fishy, it gives you energy enough to jump about all over the place. You now understand how to jump into a roll." +
            "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.DodgeRollDistanceMultiplier, .25f, StatModifier.ModifyMethod.ADDITIVE);
            

            //Set the rarity of the item
            item.quality = PickupObject.ItemQuality.C;





        }

        public override void Pickup(PlayerController player)
        {
            PassiveItem.IncrementFlag(player, typeof(LiveAmmoItem));
            PassiveItem.IncrementFlag(player, typeof(PegasusBootsItem));
            player.OnIsRolling += Player_OnIsRolling;
            player.OnRolledIntoEnemy += Player_OnRolledIntoEnemy;
            base.Pickup(player);
        }

        private void Player_OnRolledIntoEnemy(PlayerController arg1, AIActor arg2)
        {
            arg2.ApplyEffect(StaticStatusEffects.tripleCrossbowSlowEffect);
        }

        private void Player_OnIsRolling(PlayerController obj)
        {
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            string text = "assets/data/goops/water goop.asset";
            GoopDefinition goopDefinition;
            try
            {
                GameObject gameObject = assetBundle.LoadAsset(text) as GameObject;
                goopDefinition = gameObject.GetComponent<GoopDefinition>();
            }
            catch
            {
                goopDefinition = (assetBundle.LoadAsset(text) as GoopDefinition);
            }
            goopDefinition.name = text.Replace("assets/data/goops/", "").Replace(".asset", "");
            DeadlyDeadlyGoopManager goopManagerForGoopType = DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(goopDefinition);
            goopManagerForGoopType.TimedAddGoopCircle(this.Owner.sprite.WorldCenter + new Vector2(0,-.5f), 1.5f, .5f, false);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnIsRolling -= Player_OnIsRolling;
            player.OnRolledIntoEnemy -= Player_OnRolledIntoEnemy;
            PassiveItem.DecrementFlag(player, typeof(PegasusBootsItem));
            PassiveItem.DecrementFlag(player, typeof(LiveAmmoItem));
            return base.Drop(player);
        }
        public override void  Update()
        {
            //extra damage while standing on goop
            base.Update();
        }
    }
}
