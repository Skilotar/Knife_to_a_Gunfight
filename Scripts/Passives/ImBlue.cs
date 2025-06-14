using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using JetBrains.Annotations;

namespace Knives
{
    class Im_blue :PassiveItem
    {
        public static void Register()
        {
            string itemName = "Im blue";

            string resourceName = "Knives/Resources/Im_blue";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Im_blue>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "da ba de da ba DIE";
            string longDesc = "Slows Player and Some Enemies\n " +
                "Everything you see is blue. This depressing outlook on life makes every second seem to drag.\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.ProjectileSpeed, -.20f, StatModifier.ModifyMethod.ADDITIVE);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.MovementSpeed, -1f, StatModifier.ModifyMethod.ADDITIVE);
            item.quality = PickupObject.ItemQuality.C;
        }

        public override void Pickup(PlayerController player)
        {
            player.OnEnteredCombat += this.EnterCombat;
            base.Pickup(player);
        }

        public List<AIActor> sadBois = new List<AIActor>();
        private void EnterCombat()
        {
            if(this.Owner.CurrentRoom != null)
            {
                sadBois =  MiscToolMethods.RandomNoRepeats(this.Owner.CurrentRoom.GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.All), (int)UnityEngine.Random.Range(0, 5f));
                for(int i = sadBois.Count -1; i >= 0; i--)
                {
                    // SadBoi Status
                    if(sadBois[i] != null)
                    {
                        if (!sadBois[i].healthHaver.IsBoss)
                        {
                            sadBois[i].RegisterOverrideColor(ExtendedColours.freezeBlue, "The Big Sad");
                            sadBois[i].LocalTimeScale *= .5f;
                        }
                    }
                    
                }
            }
            
        }
        public override void Update()
        {
            if (this.Owner != null)
            {
                //RenderSettings.ambientLight = ExtendedColours.freezeBlue;
                this.Owner.sprite.color = ExtendedColours.freezeBlue;
                this.Owner.primaryHand.sprite.color = ExtendedColours.freezeBlue;
                this.Owner.secondaryHand.sprite.color = ExtendedColours.freezeBlue;
                base.Update();
            }
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnEnteredCombat += this.EnterCombat;
            this.Owner.sprite.color = UnityEngine.Color.white;
            this.Owner.primaryHand.sprite.color = UnityEngine.Color.white;
            this.Owner.secondaryHand.sprite.color = UnityEngine.Color.white;
            return base.Drop(player);
        }
    }
}
