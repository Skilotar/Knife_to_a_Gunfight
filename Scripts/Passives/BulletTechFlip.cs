using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;

namespace Knives
{
    class Fliplets : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Bullet Tech Flip";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/BulletTech_Flipper";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Fliplets>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Flip Wizard";
            string longDesc = "Shooting tables Flips them.\n\n" +
                "A single page manuscript describing a specific angle that can be used to have bullets flip tables from a distance. " +
                "This Golden Angle is so mathmatically complex and precise that few can aim well enough to hit it. \n\n" +
                "The Gunslinger who wrote this must have missed his table as the signature is only half signed." +
                "\n\n - Knife_to_a_Gu... ";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            item.quality = PickupObject.ItemQuality.D;
            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);
        }
       
        private void PostProcessProjectile(Projectile proj, float chance)
        {
            proj.gameObject.GetOrAddComponent<FlipTablesComp>();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= this.PostProcessProjectile;
            return base.Drop(player);

        }
    }

	public class FlipTablesComp : MonoBehaviour
	{

		public FlipTablesComp()
		{

		}

		private void Start()
		{
			this.Projectile = base.GetComponent<Projectile>();
            room = Projectile.LastPosition.GetAbsoluteRoom();
            owner = Projectile.Owner as PlayerController;
            
        }
        public VFXPool pool = new VFXPool();
        
        private void FixedUpdate()
        {
            if(Projectile.LastPosition != null)
            {
                IPlayerInteractable nearestInteractable = owner.CurrentRoom.GetNearestInteractable(Projectile.LastPosition, 1f, owner);
                if(nearestInteractable != null)
                {
                    
                    if (nearestInteractable is FlippableCover)
                    {
                        FlippableCover Table = nearestInteractable as FlippableCover;
                        if (!Table.IsFlipped)
                        {
                            Table.Flip(owner.specRigidbody);
                            BounceProjModifier bnc = Projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
                            bnc.numberOfBounces++;
                            Projectile.baseData.damage *= 1.5f;

                            if (owner.PlayerHasActiveSynergy("Flip-A-Dip-Dip!"))
                            {
                                
                                Projectile Proj = MiscToolMethods.SpawnProjAtPosi(MiscToolMethods.standardproj, Table.specRigidbody.UnitCenter, owner);
                                ProjectileSlashingBehaviour stabby = Proj.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
                                
                                stabby.SlashDimensions = 180;
                                stabby.SlashDamageUsesBaseProjectileDamage = false;
                                stabby.SlashRange = 8f;
                                stabby.SlashDamage = 15f;
                                stabby.SlashVFX.type = VFXPoolType.None;
                                stabby.soundToPlay = "Play_TrickFlip_001";
                                SpawnManager.SpawnVFX(EasyVFXDatabase.MachoBraceDustUpVFX, Table.specRigidbody.UnitCenter + new Vector2(-1.5f, -2f), Quaternion.identity);

                            }
                        }
                    }
                    
                }
            }

        }

       


        private Projectile Projectile;
        public RoomHandler room;
        public PlayerController owner;

	}

}
