using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Dungeonator;
using Gungeon;

namespace Knives
{
    class ChaoticComedy : PassiveItem
    {

        public static void Register()
        {

            string itemName = "Manic Theatre";


            string resourceName = "Knives/Resources/Chaotic Comedy";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<ChaoticComedy>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Put On A SHOW!!";
            string longDesc = "Enemies dies in one hit, but so do you! Keep your audience on the edge of their seat while having the rolling on the floor at the same time! \n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
           
            item.CanBeDropped = false;

            item.IgnoredByRat = true;

            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.EnemyProjectileSpeedMultiplier, .1f);

            item.quality = PickupObject.ItemQuality.EXCLUDED;

            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnDamaged += OnDamaged;
            base.Pickup(player);
        }


        public override void  Update()
        {

            if (this.Owner != null && GameManager.Instance.IsLoadingLevel == false)
            {
                if (Owner.CurrentRoom != null)
                {

                    RoomHandler currentRoom = this.Owner.CurrentRoom;
                    if (currentRoom != null)
                    {
                        if (currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                        {
                            foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                            {
                                if (aiactor.healthHaver != null && aiactor.CompanionOwner != this.Owner)
                                {

                                    if (aiactor.healthHaver.IsBoss)//floor boss
                                    {

                                        aiactor.healthHaver.AllDamageMultiplier = 5;

                                    }
                                    else
                                    {

                                        aiactor.healthHaver.AllDamageMultiplier = 25;
                                        aiactor.LocalTimeScale = 1.25f;
                                    }



                                }

                                if (aiactor.EnemyGuid == "6b7ef9e5d05b4f96b04f05ef4a0d1b18")
                                {
                                    aiactor.Transmogrify(EnemyDatabase.GetOrLoadByGuid("98fdf153a4dd4d51bf0bafe43f3c77ff"), null);
                                }
                            }

                        }
                    }

                    if(knownArmor != this.Owner.healthHaver.Armor)
                    {
                        knownArmor = this.Owner.healthHaver.Armor;
                    }
                  
                }

            }
            base.Update();
        }
        public float knownArmor;

        public override DebrisObject Drop(PlayerController player)
        {
            player.healthHaver.OnDamaged -= this.OnDamaged;
            return base.Drop(player);
        }


        private void OnDamaged(float resultValue, float maxValue, CoreDamageTypes damageTypes, DamageCategory damageCategory, Vector2 damageDirection)
        {
            

            if(damageCategory != DamageCategory.Collision && this.Owner.healthHaver.Armor <= knownArmor && resultValue > 0)
            {
                this.Owner.healthHaver.Armor = 0;
                this.Owner.healthHaver.ForceSetCurrentHealth(0);
                this.Owner.healthHaver.Die(Vector2.zero);
                GameManager.Instance.MainCameraController.StartTrackingPlayer();
                GameManager.Instance.MainCameraController.SetManualControl(false, true);
                
            }

            if (this.Owner.characterIdentity == PlayableCharacters.Robot && damageCategory != DamageCategory.Collision && this.Owner.healthHaver.Armor < knownArmor)
            {
                this.Owner.healthHaver.Armor = 0;
                this.Owner.healthHaver.ForceSetCurrentHealth(0);
                this.Owner.healthHaver.Die(Vector2.zero);
                GameManager.Instance.MainCameraController.StartTrackingPlayer();
                GameManager.Instance.MainCameraController.SetManualControl(false, true);
            }

        }


    }
}
