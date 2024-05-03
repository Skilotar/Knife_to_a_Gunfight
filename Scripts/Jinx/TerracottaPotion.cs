using System;
using System.Collections;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using Dungeonator;


namespace Knives
{
    public class StonePotion : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Terracotta Potion";

            string resourceName = "Knives/Resources/Terracotta_potion";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<StonePotion>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "A Hardened Heart";
            string longDesc = "A potion made of dirt and anger that leaves a bitter taste in your mouth forever and hardens your heart to war. \n\n" +
                "Gain temporary shields each room, can no longer heal." +
            
                "\n\n\n -Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            

            item.quality = PickupObject.ItemQuality.C;

            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.ArmorToGainOnInitialPickup = 1;

            StonePotion.NoHeartPrefab = SpriteBuilder.SpriteFromResource(StonePotion.NoHeartVFX, null);
            StonePotion.NoHeartPrefab.name = StonePotion.vfxName;
            UnityEngine.Object.DontDestroyOnLoad(StonePotion.NoHeartPrefab);
            FakePrefab.MarkAsFakePrefab(StonePotion.NoHeartPrefab);
            StonePotion.NoHeartPrefab.SetActive(false);
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            itemID = item.PickupObjectId;

            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Can no longer heal, Gain temporary shields each room.";
        }

        public static int itemID;
        public override void Pickup(PlayerController player)
        {
            player.OnEnteredCombat = (Action)Delegate.Combine(player.OnEnteredCombat, new Action(this.SpawnShields));
            player.healthHaver.ModifyHealing = (Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>)Delegate.Combine(player.healthHaver.ModifyHealing, new Action<HealthHaver,HealthHaver.ModifyHealingEventArgs>(this.stopheal));
            base.Pickup(player);


        }
       
        private void stopheal(HealthHaver player, HealthHaver.ModifyHealingEventArgs args)
        {
            args.ModifiedHealing = 0;
            StartCoroutine(GiveCase());
            GameObject original = StonePotion.NoHeartPrefab;
            tk2dSprite OhGeez = original.GetComponent<tk2dSprite>();
            this.Owner.BloopItemAboveHead(OhGeez, "");


        }

        private IEnumerator GiveCase()
        {
            yield return new WaitForSeconds(.25f);
            for (int i = 0; i <= 5; i++)
            {
                this.Owner.GiveItem("casing");
            }
        }

        public int GuonCounter;
        private void SpawnShields()
        {
            GuonCounter = 0;
            foreach(PlayerOrbital orbit in this.Owner.orbitals)
            {
                if(orbit.name == "ClayShield")
                {
                    GuonCounter++;
                    
                }
                
            }
            if(GuonCounter < 10)
            {
                if(GuonCounter <= 8)
                {
                    for (int i = 0; i < 2; i++)
                    {

                        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(terracottaIoun.Clay.OrbitalPrefab.gameObject, this.Owner.transform.position, Quaternion.identity);
                        gameObject.GetOrAddComponent<FragileGuonController>();
                        PlayerOrbital component = gameObject.GetComponent<PlayerOrbital>();
                        component.Initialize(this.Owner);


                    }

                }
                if(GuonCounter == 9)
                {

                    GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(terracottaIoun.Clay.OrbitalPrefab.gameObject, this.Owner.transform.position, Quaternion.identity);
                    gameObject.GetOrAddComponent<FragileGuonController>();
                    PlayerOrbital component = gameObject.GetComponent<PlayerOrbital>();
                    component.Initialize(this.Owner);

                }

            }


        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.ModifyHealing = (Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>)Delegate.Remove(player.healthHaver.ModifyHealing, new Action<HealthHaver, HealthHaver.ModifyHealingEventArgs>(this.stopheal));
            player.OnEnteredCombat = (Action)Delegate.Remove(player.OnEnteredCombat, new Action(this.SpawnShields));
            return base.Drop(player);
        }


        private static string NoHeartVFX = "Knives/Resources/NoHeal";
        private static GameObject NoHeartPrefab;
        private static string vfxName = "NoHealingAllowed";
    }
}
