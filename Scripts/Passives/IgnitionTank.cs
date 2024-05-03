using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;
using System.Reflection;
using Dungeonator;

namespace Knives
{
    class IgnitionTank : PassiveItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Ignition Tank";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Ignition";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<IgnitionTank>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Arsonist Cocktail";
            string longDesc = "Bullets gain a small chance to ignite, Burning enemies contract [Blastblight].\n" +
                "Afflicted enemies will explode after 3 instances of damage or on death. \n\n" +
                "" +
                "Contains a mixture of the most flammable substances we know. A single spark would be more than enough to set this baby off!" +
                "These canisters are extreamly expensive as they have been known to spontaneously combust durring transit. Fire insurance comes free with this purchase." +
                "\n\n\n - Knife_to_a_Gunfight";


            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            item.quality = PickupObject.ItemQuality.A;

        }

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += this.PostProcessProjectile;
            base.Pickup(player);

        }

        private System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile source, float chance)
        {
            chance = rng.Next(1, 20);
            if (chance == 1)
            {
                source.FireApplyChance = 1;
                source.AppliesFire = true;
                source.fireEffect = StaticStatusEffects.hotLeadEffect;
                source.HasDefaultTint = true;
                source.DefaultTintColor = UnityEngine.Color.red;
            }
        }

        public override void Update()
        {
            if (this.Owner != null)
            {
                PlayerController player = this.Owner;
                if (player.CurrentRoom != null)
                {
                    //ETGModConsole.Log("room good");
                    if (player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All) != null)
                    {
                        foreach (AIActor actor in player.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All))
                        {
                            //ETGModConsole.Log("found enemeies");
                            if (actor != null)
                            {
                                if (CheckIfBurning(actor.specRigidbody))
                                {
                                    if (actor.gameObject.GetComponent<BlastBlightedStatusController>() == null)
                                    {
                                        
                                        BlastBlightedStatusController boom = actor.gameObject.GetOrAddComponent<BlastBlightedStatusController>();
                                        boom.statused = true;
                                        
                                    }
                                }
                            }

                        }

                    }


                }


            }
            base.Update();
        }

        private bool CheckIfBurning(SpeculativeRigidbody arg2)
        {
            AIActor boi = arg2.aiActor;
            
            GameActorEffect effect = boi.GetEffect(StaticStatusEffects.hotLeadEffect.effectIdentifier); // uses effect Identifier    // Hey Future me. DONT use the resistnaceType on the get effect method. it don't work fam.
            GameActorEffect effect2 = boi.GetEffect(StaticStatusEffects.greenFireEffect.effectIdentifier);
            if (effect != null || effect2 != null)
            {
                //if (effect != null) ETGModConsole.Log(effect.effectIdentifier.ToString());
                //if (effect2 != null) ETGModConsole.Log(effect2.effectIdentifier.ToString());
                return true;
            }
            else
            {
                return false;
            }


        }

        public override DebrisObject Drop(PlayerController player)
        {
            DebrisObject debrisObject = base.Drop(player);
            player.PostProcessProjectile -= this.PostProcessProjectile;

            return debrisObject;
        }


    }
}
