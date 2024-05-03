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
    class Slide_Shatter : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Slide Tech Shatter";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/slide_Shatter";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<Slide_Shatter>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Break The Rules";
            string longDesc = "A scroll hidden away deep in the cult of the slide's library. It is looked down upon to use a technique of such force but sometimes rules are made to be broken. \n\n" +
                "Sliding over a table will shatter it causing a small blank." +
                "\n\n\n - Knife_to_a_Gunfight"
                ;

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            //Set the rarity of the item

            item.quality = PickupObject.ItemQuality.B;
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);

        }
        bool doingreloadroutine = false;
        public override void  Update()
        {
            base.Update();
            if (this.Owner != null)
            {
                if (this.Owner.IsSlidingOverSurface)
                {
                    if (doingreloadroutine != true && onepertable == false)
                    {
                        StartCoroutine(shatterroutine(this.Owner));
                    }

                }
                else
                {
                    onepertable = false;
                }

                
            }
        }
        public bool onepertable = false;
        private IEnumerator shatterroutine(PlayerController player)
        {

            doingreloadroutine = true;
            onepertable = true;
            yield return new WaitForSeconds(.1f);
            player.ForceStartDodgeRoll(Vector2.up);
            AkSoundEngine.PostEvent("Play_OBJ_silenceblank_small_01", base.gameObject);
            player.ForceBlank(5, .8f, true, true,null,true,10);
            player.DoDustUps = true;
            SpawnManager.SpawnVFX(EasyVFXDatabase.MachoBraceDustUpVFX, player.specRigidbody.UnitCenter + new Vector2(-1.5f,-2f), Quaternion.identity);
            yield return new WaitForSeconds(3f);
            doingreloadroutine = false;
        }
    }
}
