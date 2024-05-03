using Dungeonator;
using ItemAPI;
using System;

using UnityEngine;

namespace Knives
{
    class tinyAmmo : PickupObject, IPlayerInteractable
    {
        public static void Register()
        {
            string name = "Tiny Ammo Pack";
            string resourcePath = "Knives/Resources/tiny_ammo";
            GameObject gameObject = new GameObject(name);
            tinyAmmo item = gameObject.AddComponent<tinyAmmo>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "A little something extra";
            string longDesc = "A very small pack of univeral ammo. Its such a small amount of ammo the rat doesn't even bother to steal it.";
            // why am i writing this description no one will see it 
            item.SetupItem(shortDesc, longDesc,"ski");
            item.UsesCustomCost = true;
            item.CustomCost = 10;
            item.quality = ItemQuality.COMMON;
            item.IgnoredByRat = true;
            ID = item.PickupObjectId;


           
            /*
            WeightedGameObject weightedObject = new WeightedGameObject();
            weightedObject.SetGameObject(gameObject);
            weightedObject.weight = .1f;
            weightedObject.rawGameObject = gameObject;
            weightedObject.pickupId = ID;
            weightedObject.forceDuplicatesPossible = true;
            weightedObject.additionalPrerequisites = new DungeonPrerequisite[0];


            GenericLootTable thanksbotluvya = ResourceManager.LoadAssetBundle("shared_auto_001").LoadAsset<GenericLootTable>("Shop_Gungeon_Cheap_Items_01");
            thanksbotluvya.defaultItemDrops.Add(weightedObject);
            */

        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {
           
            if (m_hasBeenPickedUp)
                return;
            
            m_hasBeenPickedUp = true;
            int ammo_to_give = player.CurrentGun.ClipCapacity + 9;
            if (ammo_to_give >= 20)
            {
                player.CurrentGun.GainAmmo(20);
                player.CurrentGun.MoveBulletsIntoClip(player.CurrentGun.ClipCapacity - player.CurrentGun.ClipShotsRemaining);
                AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", base.gameObject);
                //player.RemovePassiveItem(PickupObjectDatabase.GetByName("Tiny Ammo Pack").PickupObjectId);
            }
            else
            {
                player.CurrentGun.GainAmmo(ammo_to_give);
                player.CurrentGun.MoveBulletsIntoClip(player.CurrentGun.ClipCapacity - player.CurrentGun.ClipShotsRemaining);
                AkSoundEngine.PostEvent("Play_OBJ_ammo_pickup_01", base.gameObject);
                //player.RemovePassiveItem(PickupObjectDatabase.GetByName("Tiny Ammo Pack").PickupObjectId);
            }

            player.BloopItemAboveHead(base.sprite, "Knives/Resources/tiny_ammo");
            
            UnityEngine.Object.Destroy(base.gameObject);
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
                ETGModConsole.Log("tiny ammo error start()" + er.ToString(), true);
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


                if (interactor.CurrentGun.InfiniteAmmo != true)
                {
                    this.Pickup(interactor);
                }

            }
            catch (Exception err)
            {
                ETGModConsole.Log("tiny ammo error start()" + err.ToString(), true);
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