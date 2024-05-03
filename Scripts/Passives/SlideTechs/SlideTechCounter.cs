using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using Dungeonator;
using Gungeon;
using ItemAPI;
using MultiplayerBasicExample;
using UnityEngine;
namespace Knives
{
    class Slide_counter : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Slide Tech Counter";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/slide_counter";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Slide_counter>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Your Faith Will Protect You";
            string longDesc = "A staple page of the cult of the slide's library. Praised for its ability to counter attacks better than dodge rolling.\n\n" +
                "" +
                "\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.C;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

        }
        bool m_usedOverrideMaterial;
        bool doingCounter = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if (this.Owner.IsSlidingOverSurface && doingCounter == false)
                {
                    
                    StartCoroutine(Slidecounter());
                   

                }
                

            }
        }

        private IEnumerator Slidecounter()
        {
            doingCounter = true;
            if (this.Owner.healthHaver.IsVulnerable == false)//already invincible
            {
                this.Owner.healthHaver.IsVulnerable = false;
            }
            
            m_usedOverrideMaterial = this.Owner.sprite.usesOverrideMaterial;
            this.Owner.sprite.usesOverrideMaterial = true;
            this.Owner.SetOverrideShader(ShaderCache.Acquire("Brave/ItemSpecific/MetalSkinShader"));
            
            SpeculativeRigidbody specRigidbody = this.Owner.specRigidbody;
            specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));

            yield return new WaitForSeconds(.75f);

            if (this.Owner.healthHaver.IsVulnerable == false)//already invincible
            {
                this.Owner.healthHaver.IsVulnerable = true;
            }
            this.Owner.ClearOverrideShader();
            this.Owner.sprite.usesOverrideMaterial = this.m_usedOverrideMaterial;
            specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollision));
            doingCounter = false;
        }

        private void OnPreCollision(SpeculativeRigidbody myRigidbody, PixelCollider myCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherCollider)
        {
            Projectile component = otherRigidbody.GetComponent<Projectile>();
            if (component != null && !(component.Owner is PlayerController))
            {
                PassiveReflectItem.ReflectBullet(component, true, Owner.specRigidbody.gameActor, 10f, 1f, 1f, 0f);
                PhysicsEngine.SkipCollision = true;
            }
        }

    }
}