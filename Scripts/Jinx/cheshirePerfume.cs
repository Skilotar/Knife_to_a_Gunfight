using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    class Cheshire_purfume : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Cheshire Purrfume";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Cheshire_purfume";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Cheshire_purfume>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Smells Smelly";
            string longDesc =

                "Buffs or Debuffs Nearby enemies. \n The smells of this purfume seem to grin an impossibly large mischievous smile." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;



            //Set the rarity of the item

            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.quality = PickupObject.ItemQuality.B;
            itemID = item.PickupObjectId;
            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Buffs or Debuffs Nearby enemies.";

        }
        public static int itemID;

        public override void Pickup(PlayerController player)
        {
          

            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
           
            return base.Drop(player);
        }

        public override void  Update()
        {
            try
            {
                if (this.Owner != null)
                {
                    RoomHandler currentRoom = this.Owner.CurrentRoom;
                    foreach (AIActor aiactor in currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                    {
                        if (aiactor.isActiveAndEnabled)
                        {
                            if(aiactor.gameObject.GetOrAddComponent<AiactorSpecialStates>().smelledChechPerf != true)
                            {
                                
                                if (Vector2.Distance(aiactor.sprite.WorldCenter, this.Owner.sprite.WorldCenter) <= 5)//within 4 tiles
                                {
                                    aiactor.gameObject.GetOrAddComponent<AiactorSpecialStates>().smelledChechPerf = true;
                                    if ((int)UnityEngine.Random.Range(1, 11) <= 6)
                                    {
                                        int Debuffselect = UnityEngine.Random.Range(1, 7);
                                        switch (Debuffselect)
                                        {
                                            case 1:
                                                aiactor.ApplyEffect(StaticStatusEffects.charmingRoundsEffect);

                                                break;

                                            case 2:
                                                aiactor.ApplyEffect(StaticStatusEffects.hotLeadEffect);
                                                break;

                                            case 3:
                                                aiactor.ApplyEffect(StaticStatusEffects.tripleCrossbowSlowEffect);
                                                break;

                                            case 4:
                                                aiactor.ApplyEffect(StaticStatusEffects.irradiatedLeadEffect);
                                                break;

                                            case 5:
                                                HexStatusEffectController hexen = aiactor.gameObject.GetOrAddComponent<HexStatusEffectController>();
                                                hexen.statused = true;
                                                break;

                                            case 6:

                                                BlastBlightedStatusController boom = aiactor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                                                boom.statused = true;
                                                break;



                                        }

                                        GlobalSparksDoer.DoRadialParticleBurst(15, aiactor.sprite.WorldBottomLeft, aiactor.sprite.WorldBottomRight, 25, 3, 2, null, null, null, GlobalSparksDoer.SparksType.DARK_MAGICKS);
                                        
                                    }
                                    else
                                    {
                                        if (aiactor.IsBlackPhantom == false)
                                        {
                                            aiactor.BecomeBlackPhantom();
                                            GlobalSparksDoer.DoRadialParticleBurst(15, aiactor.sprite.WorldBottomLeft, aiactor.sprite.WorldBottomRight, 25, 3, 2, null, null, null, GlobalSparksDoer.SparksType.BLACK_PHANTOM_SMOKE);
                                            StartCoroutine(UndoTempBlackPhantom(aiactor));
                                            
                                        }
                                      
                                    }
                                }
                                
                            }
                           
                        }
                    }
                }

            }
            catch
            {

            }

            base.Update();
        }

        private IEnumerator UndoTempBlackPhantom(AIActor aiactor)
        {
            yield return new WaitForSeconds(5);
            aiactor.UnbecomeBlackPhantom();
        }


        public HeatIndicatorController m_indicator;
    }
}

