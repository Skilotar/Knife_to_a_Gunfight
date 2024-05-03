using Dungeonator;

using ItemAPI;
using System;
using System.Collections;
using UnityEngine;

namespace Knives
{
    class MeatPickup : PickupObject, IPlayerInteractable
    {
        public static void Register()
        {
            string name = "Meat";
            string resourcePath = "Knives/Resources/Charm/icons/Meat_pickup";
            GameObject gameObject = new GameObject(name);
            MeatPickup item = gameObject.AddComponent<MeatPickup>();
            ItemBuilder.AddSpriteToObject(name, resourcePath, gameObject);
            string shortDesc = "Food of Warriors";
            string longDesc = "Its meat.. Best not to think where its been.";
            // why am i writing this description no one will see it 
            item.SetupItem(shortDesc, longDesc, "ski");

            
            item.quality = ItemQuality.EXCLUDED;

            ID = item.PickupObjectId;

        }
        public static int ID;
        public GameObject Vfx;
        public GameObject instanceVFX;

        public override void Pickup(PlayerController player)
        {
            Vfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);
            if (m_hasBeenPickedUp)
                return;

            player.StartCoroutine(this.HandleRage(player));

            UnityEngine.Object.Destroy(base.gameObject);
            m_hasBeenPickedUp = true;
        }

        public IEnumerator HandleRage(PlayerController player)
        {
            StatModifier stat = StatModifier.Create(PlayerStats.StatType.Damage, StatModifier.ModifyMethod.ADDITIVE, 1);
            player.ownerlessStatModifiers.Add(stat);
            player.stats.RecalculateStats(player);
            GameObject Vfx = (PickupObjectDatabase.GetById(353).gameObject.GetComponent<RagePassiveItem>().OverheadVFX);
            if (!this.instanceVFX)
            {
                this.instanceVFX = player.PlayEffectOnActor(Vfx, new Vector3(0f, 1.375f, 0f), true, true, false);
            }

            yield return new WaitForSeconds(4);

            this.instanceVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("rage_face_vfx_out", null);
            this.instanceVFX = null;
            player.ownerlessStatModifiers.Remove(stat);
            player.stats.RecalculateStats(player);
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