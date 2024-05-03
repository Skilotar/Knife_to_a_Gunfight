
using Dungeonator;

using ItemAPI;
using System;

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

            ID = item.PickupObjectId;

            WeightedGameObject weightedObject = new WeightedGameObject();
            weightedObject.SetGameObject(gameObject);
            weightedObject.weight = .55f;
            weightedObject.rawGameObject = gameObject;
            weightedObject.pickupId = ID;
            weightedObject.forceDuplicatesPossible = true;
            weightedObject.additionalPrerequisites = new DungeonPrerequisite[0];
            

            GenericLootTable thanksbotluvya = ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<GenericLootTable>("Shop_Gungeon_Cheap_Items_01");
            thanksbotluvya.defaultItemDrops.Add(weightedObject);

            
            
        }
        public static int ID;


        public override void Pickup(PlayerController player)
        {

            if (m_hasBeenPickedUp)
                return;

            
            var item = GetRandomCharmID();
            StatModifier stat = StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, 1);
            player.ownerlessStatModifiers.Add(stat);
            player.stats.RecalculateStats(player);
            //ETGModConsole.Log(PickupObjectDatabase.GetById(item).name);
            string callname = PickupObjectDatabase.GetById(item).name;
            callname = callname.Replace(" ", "_");
            callname = callname.ToLower();
            callname = "ski:" + callname;
            player.GiveItem(callname);
            player.BloopItemAboveHead(base.sprite, "Knives/Resources/Charm/CharmCapsule");
            player.ownerlessStatModifiers.Remove(stat);
            player.stats.RecalculateStats(player);

            UnityEngine.Object.Destroy(base.gameObject);
            m_hasBeenPickedUp = true;
        }

        private static int GetRandomCharmID()
        {
            int rng = UnityEngine.Random.Range(0, GlobalCharmList.Charmslist.Count);
            return GlobalCharmList.Charmslist[rng];
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