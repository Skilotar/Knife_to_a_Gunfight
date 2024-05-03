using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gungeon;
using UnityEngine;
using ItemAPI;
using HutongGames.PlayMaker.Actions;
using MultiplayerBasicExample;
using MonoMod.Utils;
using Dungeonator;
using MonoMod;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Knives
{
    public class vodoo_kit : PassiveItem
    {
        //Call this method from the Start() method of your ETGModule extension
        public static void Register()
        {
            //The name of the item
            string itemName = "Grateful dead";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/reanimation_kit";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<vodoo_kit>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Your Job Here Isn't Quite Over";
            string longDesc =
                "Some enemies can be revived.\n\n"+

                "A simple reanimation kit, created by a couple of failure medical students that could only \"Heal\" things by smashing a fairy in a bottle over the patients head.\n" +
                "They used this to cheat on operation exams as most of their patients would die. This kit can be used to reanimate some corpses left on an open battlefield into a much ghastlier version. " +
                "These regundead are grateful for your aid and will attack enemies of the room, however they are not entirely harmless." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
           

            item.quality = PickupObject.ItemQuality.B;
        }
        
        public override void Pickup(PlayerController player)
        {
            player.OnKilledEnemyContext += Player_OnKilledEnemyContext; ;
            
            base.Pickup(player);
        }

        private void Player_OnKilledEnemyContext(PlayerController arg1, HealthHaver arg2)
        {
            if(arg2.aiActor != null)
            {
                if(arg2.aiActor.gameObject.GetOrAddComponent<AiactorSpecialStates>().Rez == false)
                {
                    if (UnityEngine.Random.Range(0, 7) < 1)
                    {
                        if (!arg2.aiActor.IsBlackPhantom)
                        {
                            GameObject Soul = arg2.aiActor.PlayEffectOnActor(EasyVFXDatabase.Good, new Vector3(0f, -.4f), false);
                            Soul.GetComponent<tk2dSpriteAnimator>().Play();

                            tk2dBaseSprite DustyDust = Soul.GetComponent<tk2dBaseSprite>();
                            DustyDust.HeightOffGround = -1f;
                            DustyDust.UpdateZDepth();

                            SoulController Controller = Soul.gameObject.GetOrAddComponent<SoulController>();
                            Controller.Evil = false;

                        }
                        else
                        {
                            GameObject Soul = arg2.aiActor.PlayEffectOnActor(EasyVFXDatabase.Evil, new Vector3(0f, -.4f), false);
                            Soul.GetComponent<tk2dSpriteAnimator>().Play();

                            tk2dBaseSprite DustyDust = Soul.GetComponent<tk2dBaseSprite>();
                            DustyDust.HeightOffGround = -1f;
                            DustyDust.UpdateZDepth();

                            SoulController Controller = Soul.gameObject.GetOrAddComponent<SoulController>();
                            Controller.Evil = true;

                        }
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnKilledEnemyContext -= Player_OnKilledEnemyContext; ;

            return base.Drop(player);
        }
      

       
        private System.Random rng = new System.Random();
       
       

    }
}
