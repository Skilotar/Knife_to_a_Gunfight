using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;


namespace Knives
{
    class Pig_Whistle : PlayerItem

    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Pig whistle";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/pig_whistle";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Pig_Whistle>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "SEEWWW WEEE!";
            string longDesc = "Here! pig pig pig pig pig pig pig!  \n\n" +
                "A cursed relic created by the sages that sealed the mighty piglord.\n\n" +
                "Upon playing this relic summons lord cannon from his eternal prison to proform his required duties" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500f);


            //Set the rarity of the item
            //item.AddToSubShop(ItemBuilder.ShopType.Cursula, .01f);


            item.quality = PickupObject.ItemQuality.C;

        }
        AIActor buddy;
        //applies damage on last use
        public override void  DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_ENM_smiley_whistle_02", base.gameObject);

            string guid;
            if (user)
            {
                guid = "86237c6482754cd29819c239403a2de7";
            }
            else
            {
                guid = "86237c6482754cd29819c239403a2de7";
            }

            AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(guid);
            IntVector2? intVector = new IntVector2?(user.CurrentRoom.GetRandomVisibleClearSpot(2, 2));
            AIActor aiactor = AIActor.Spawn(orLoadByGuid.aiActor, intVector.Value, GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(intVector.Value), true, AIActor.AwakenAnimationType.Default, true);
            aiactor.CanTargetEnemies = true;
            aiactor.CanTargetPlayers = false;
            PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(aiactor.specRigidbody, null, false);
            aiactor.gameObject.AddComponent<KillOnRoomClear>();
            
            aiactor.IgnoreForRoomClear = true;
            aiactor.LocalTimeScale = 3f;
            
            aiactor.reinforceType = AIActor.ReinforceType.Instant;
            aiactor.HandleReinforcementFallIntoRoom(0f);
            aiactor.CompanionOwner = user;
            aiactor.IsHarmlessEnemy = true;
            aiactor.isPassable = true;
           
            aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(
                        CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox, CollisionLayer.EnemyCollider, CollisionLayer.PlayerHitBox,
                        CollisionLayer.Projectile, CollisionLayer.PlayerCollider, CollisionLayer.PlayerBlocker, CollisionLayer.BeamBlocker)
                        );
            aiactor.specRigidbody.AddCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.BulletBreakable, CollisionLayer.Trap));

            CompanionController orAddComponent = aiactor.gameObject.GetOrAddComponent<CompanionController>();
            orAddComponent.companionID = CompanionController.CompanionIdentifier.NONE;
            orAddComponent.Initialize(user);



            buddy = aiactor;



        }

        public List<Projectile> GetbuddyBullets()
        {
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];

                if (projectile.Owner == buddy)
                {
                    list.Add(projectile);
                }

            }
            return list;
        }
        public override void Update()
        {
            if(this.LastOwner != null)
            {
                safebullets();
                safebullets();

                this.CanBeUsed(this.LastOwner);
            }



            base.Update();
        }

        public override bool CanBeUsed(PlayerController user)
        {
            return base.CanBeUsed(user) && user.IsInCombat;
        }


        public void safebullets()
        {
            foreach (var projectile in GetbuddyBullets())
            {
                projectile.collidesWithPlayer = false;
                projectile.UpdateCollisionMask();

            }
        }
    }  
}

