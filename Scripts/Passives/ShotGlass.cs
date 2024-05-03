using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using Gungeon;
using MonoMod.RuntimeDetour;
using System.Reflection;
using Dungeonator;

namespace Knives
{
    class ShotGlass : PassiveItem
    {
        
        public static void Register()
        {
            //The name of the item
            string itemName = "ShotGlass";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/ShotGlass";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<ShotGlass>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Shots Shots Shots!";
            string longDesc = "Glass Guons shoot enemies.\n\n" +
                "A small alcohol delivery round used for long range delievery of wartime booze rations.\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight";


            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            

            item.quality = PickupObject.ItemQuality.B;
            ID = item.PickupObjectId;
        }


        public static int ID;

       
       
        public override void Pickup(PlayerController player)
        {
            if (startup == false)
            {
                player.GiveItem("glass_guon_stone");
                player.GiveItem("glass_guon_stone");

                

                startup = true;
            }
            base.Pickup(player);

    
        }
        public bool startup = false;
        private float cooldown;


        public override void Update()
        {
            if (this.Owner != null)
            {
                PlayerController player = this.Owner;
                if (player.IsInCombat)
                {
                    if (player.orbitals != null)
                    {
                        if (cooldown <= 0)
                        {
                            for (int i = player.orbitals.Count - 1; i >= 0; i--)
                            {
                                if (player.orbitals[i] != null)
                                {
                                    PlayerOrbital guon = (PlayerOrbital)player.orbitals[i];
                                    AIActor posibleTarget = GetEnemyTarget(guon);
                                    if (posibleTarget != null)
                                    {
                                        if (guon.SourceItem.PickupObjectId == 565)
                                        {
                                            Vector2 angprep = (Vector2)posibleTarget.specRigidbody.UnitCenter - (Vector2)guon.transform.position;
                                            float ang = angprep.ToAngle();
                                            Projectile proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, guon.transform.position, player, ang, 3);
                                            proj.AdditionalScaleMultiplier = .5f;
                                            proj.baseData.speed *= .7f;
                                            proj.baseData.damage = 4;
                                            BlockletsModifier stop = proj.gameObject.GetOrAddComponent<BlockletsModifier>();
                                            stop.DieWithShield = false;
                                            
                                        }
                                    }
                                    
                                }
                            }
                            cooldown = 1.5f;
                        }
                        else
                        {
                            cooldown -= Time.deltaTime;
                        }
                    }
                }
              

            }
            base.Update();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);

            
            return debrisObject;
        }
        private List<AIActor> roomEnemies = new List<AIActor>();

        private AIActor GetEnemyTarget(PlayerOrbital guon)
        {
            
            if (this.Owner != null)
            {
                if (this.Owner.CurrentRoom != null)
                {
                    this.Owner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear, ref this.roomEnemies);

                    if (this.roomEnemies.Count > 0)
                    {
                        for (int i = 0; i < this.roomEnemies.Count; i++)
                        {
                            AIActor aiactor = this.roomEnemies[i];

                            if (aiactor.IsHarmlessEnemy || !aiactor.IsNormalEnemy || aiactor.healthHaver.IsDead || aiactor == guon || aiactor.EnemyGuid == "ba928393c8ed47819c2c5f593100a5bc" || aiactor.gameObject.GetComponent<CompanionController>() != null || aiactor.CompanionOwner != null)
                            { this.roomEnemies.Remove(aiactor); }
                        }
                        if (this.roomEnemies.Count == 0) { return null; }
                        else
                        {
                            AIActor aiactor2 = this.roomEnemies[UnityEngine.Random.Range(0, this.roomEnemies.Count)];
                            return aiactor2;
                        }
                    }

                }


            }

            


            return null;

        }

    }
}