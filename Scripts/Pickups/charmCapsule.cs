
using Dungeonator;

using ItemAPI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knives
{
    class CharmBauble : PickupObject, IPlayerInteractable
    {
        public static void Register()
        {
            string name = "Charm PrizeBall";
            string resourcePath = "Knives/Resources/Charm/CharmCapsule";
            GameObject gameObject = new GameObject(name);
            CharmBauble item = gameObject.AddComponent<CharmBauble>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Charmed I'm sure.";
            string longDesc = "A small plastic capsule with a random weapon charm inside.";
            // why am i writing this description no one will see it 
            item.SetupItem(shortDesc, longDesc, "ski");
            item.UsesCustomCost = true;
            item.CustomCost = 25;

            item.quality = ItemQuality.COMMON;
            item.IgnoredByRat = true;


            WeightedGameObject weightedObject = new WeightedGameObject();
            weightedObject.SetGameObject(gameObject);
            weightedObject.weight = .5f;
            weightedObject.rawGameObject = gameObject;
            weightedObject.pickupId = ID;
            weightedObject.forceDuplicatesPossible = true;
            weightedObject.additionalPrerequisites = new DungeonPrerequisite[0];
            

            GenericLootTable thanksbotluvya = ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<GenericLootTable>("Shop_Gungeon_Cheap_Items_01");
            thanksbotluvya.defaultItemDrops.Add(weightedObject);
            List<string> resourcesList = new List<string>()
            {
                "Knives/Resources/Charm/CommonCharmCapsule.png",  // white
                "Knives/Resources/Charm/UnCommonCharmCapsule.png",  // blue
                "Knives/Resources/Charm/RareCharmCapsule.png",  // Purple
                "Knives/Resources/Charm/LegendaryCharmCapsule.png",  // Golden
                
            };
            List<string> namesList = new List<string>()
            {
                "Common",
                "Uncommon",
                "Rare",
                "Legendary",
            };
            overheads = MiscToolMethods.GenerateOverheadFromResourceList(resourcesList, namesList);
            ID = item.PickupObjectId;

        }
        public static List<GameObject> overheads = new List<GameObject>();
        public static int ID;


        public override void Pickup(PlayerController player)
        {

            if (m_hasBeenPickedUp)
                return;

            
            var item = GetRandomCharmID();
            if((int)CurrentRarity == 0) // Common celebration
            {
                player.BloopItemAboveHead(overheads[0].GetComponent<tk2dSprite>(), "");
            }
            if ((int)CurrentRarity == 1) // UnCommon celebration
            {
                player.BloopItemAboveHead(overheads[1].GetComponent<tk2dSprite>(), "");
                
            }
            if ((int)CurrentRarity == 2) // Rare celebration
            {
                player.BloopItemAboveHead(overheads[2].GetComponent<tk2dSprite>(), "");
                DoConfetti(player.CenterPosition, (int)CurrentRarity);
                AkSoundEngine.PostEvent("Play_OBJ_prize_won_01", player.gameObject);
            }
            if ((int)CurrentRarity == 3) // Legendary celebration
            {
                player.BloopItemAboveHead(overheads[3].GetComponent<tk2dSprite>(), "");
                DoConfetti(player.CenterPosition, (int)CurrentRarity);
                AkSoundEngine.PostEvent("Play_OBJ_prize_won_01", player.gameObject);
                AkSoundEngine.PostEvent("Play_hallelujah", base.gameObject);
            }
            StatModifier stat = StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, 1);
            player.ownerlessStatModifiers.Add(stat);
            player.stats.RecalculateStats(player);
            //ETGModConsole.Log(PickupObjectDatabase.GetById(item).name);
            string callname = PickupObjectDatabase.GetById(item).name;
            callname = callname.Replace(" ", "_");
            callname = callname.ToLower();
            callname = "ski:" + callname;
            player.GiveItem(callname);
            
            player.ownerlessStatModifiers.Remove(stat);
            player.stats.RecalculateStats(player);

            UnityEngine.Object.Destroy(base.gameObject);
            m_hasBeenPickedUp = true;
        }
        
        public void DoConfetti(Vector2 targetCenter, int CurrentRarity)
        {
            
            int num = 8;
            
            string[] array = new string[]
            {
                "Global VFX/Confetti_Blue_001",
                "Global VFX/Confetti_Yellow_001",
                "Global VFX/Confetti_Green_001"
            };
            if (CurrentRarity == 3)
            {
                num = 15;
                array = new string[] { "Global VFX/Confetti_Yellow_001"};
            }

            for (int i = 0; i < num; i++)
            {
                GameObject gameObject = (GameObject)BraveResources.Load(array[UnityEngine.Random.Range(0, array.Length)], ".prefab");
                WaftingDebrisObject component = UnityEngine.Object.Instantiate<GameObject>(gameObject).GetComponent<WaftingDebrisObject>();
                component.sprite.PlaceAtPositionByAnchor(targetCenter.ToVector3ZUp(0f) + new Vector3(0.5f, 0.5f, 0f), tk2dBaseSprite.Anchor.MiddleCenter);
                Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
                insideUnitCircle.y = -Mathf.Abs(insideUnitCircle.y);
                component.Trigger(insideUnitCircle.ToVector3ZUp(1.5f) * UnityEngine.Random.Range(0.5f, 2f), 0.5f, 0f);
            }
        }


        private static int GetRandomCharmID()
        {

            float rarity = UnityEngine.Random.Range(0, 1f);
            CurrentRarity = CharmRarity.Common;
            if (rarity <= .40f) // Common   40%
            {
                CurrentRarity = CharmRarity.Common;
                int rng = UnityEngine.Random.Range(0, GlobalCharmList.CommonCharmslist.Count);
                return GlobalCharmList.CommonCharmslist[rng];
               
            }
            if(rarity > .40 && rarity <= .70f) // uncommon  30%
            {
                CurrentRarity = CharmRarity.Uncommon;
                int rng = UnityEngine.Random.Range(0, GlobalCharmList.UncommonCharmslist.Count);
                return GlobalCharmList.UncommonCharmslist[rng];
                
            }
            if (rarity >= .70f && rarity < .92f) // rare  22%
            {
                CurrentRarity = CharmRarity.Rare;
                int rng = UnityEngine.Random.Range(0, GlobalCharmList.RareCharmslist.Count);
                return GlobalCharmList.RareCharmslist[rng];
               
            }
            if(rarity >= .92f) // Legendary   8%
            {
                CurrentRarity = CharmRarity.Legendary;
                int rng = UnityEngine.Random.Range(0, GlobalCharmList.LegendaryCharmslist.Count);
                return GlobalCharmList.LegendaryCharmslist[rng];
            }

            return GlobalCharmList.TotalCharmslist[0]; // default to gnome In emergency
        }

        public static CharmBauble.CharmRarity CurrentRarity = CharmRarity.Common;
        public enum CharmRarity
        {
            Common,
            Uncommon,
            Rare,
            Legendary
        }


        protected void Start()
        {
            try
            {
                GameManager.Instance.PrimaryPlayer.CurrentRoom.RegisterInteractable(this);
                SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f, 0f, SpriteOutlineManager.OutlineType.NORMAL);
            }
            catch (Exception er)
            {
                ETGModConsole.Log(" error start()" + er.ToString(), true);
            }
        }

        public float GetDistanceToPoint(Vector2 point)
        {
            if (!base.sprite)
            {
                return 1000f;
            }
            Bounds bounds = base.sprite.GetBounds();
            bounds.SetMinMax(bounds.min + base.transform.position, bounds.max + base.transform.position);
            float num = Mathf.Max(Mathf.Min(point.x, bounds.max.x), bounds.min.x);
            float num2 = Mathf.Max(Mathf.Min(point.y, bounds.max.y), bounds.min.y);
            return Mathf.Sqrt((point.x - num) * (point.x - num) + (point.y - num2) * (point.y - num2)) / 1.5f;
        }

        public float GetOverrideMaxDistance()
        {
            return 1f;
        }

        public void OnEnteredRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            if (!interactor.CurrentRoom.IsRegistered(this) && !RoomHandler.unassignedInteractableObjects.Contains(this))
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, false);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.white, 0.1f);
            base.sprite.UpdateZDepth();
        }

        public void OnExitRange(PlayerController interactor)
        {
            if (!this)
            {
                return;
            }
            SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);
            SpriteOutlineManager.AddOutlineToSprite(base.sprite, Color.black, 0.1f);
            base.sprite.UpdateZDepth();
        }



        public void Interact(PlayerController interactor)
        {
            try
            {

                if (!this)
                {
                    return;
                }

                if (RoomHandler.unassignedInteractableObjects.Contains(this))
                {
                    RoomHandler.unassignedInteractableObjects.Remove(this);
                }
                SpriteOutlineManager.RemoveOutlineFromSprite(base.sprite, true);

              
                this.Pickup(interactor);
                
            }
            catch (Exception err)
            {
                ETGModConsole.Log(" error start()" + err.ToString(), true);
            }
        }

        public string GetAnimationState(PlayerController interactor, out bool shouldBeFlipped)
        {
            shouldBeFlipped = false;

            return string.Empty;
        }

        private bool m_hasBeenPickedUp;
    }
}