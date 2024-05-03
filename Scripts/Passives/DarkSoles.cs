using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using System.Collections;

namespace Knives
{
    class DarkSoles : PassiveItem
    {
        public static void Register()
        {

            string itemName = "Dark Soles";


            string resourceName = "Knives/Resources/Dark_Soles";


            GameObject obj = new GameObject(itemName);


            var item = obj.AddComponent<DarkSoles>();


            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "A Dark Step";
            string longDesc = "With the thrill of the fight gain the ability to roll unhindered.\n" +
                "Every 5 kills earn a dark roll. Dark rolls are fully invincible. Max of 5 stored dark rolls." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 1);


            //Adds the actual passive effect to the item

            item.quality = PickupObject.ItemQuality.C;


        }
        private ImprovedAfterImage zoomy;
        public override void Pickup(PlayerController player)
        {
            player.OnPreDodgeRoll += Player_OnPreDodgeRoll;
            player.OnKilledEnemy += Player_OnKilledEnemy;



            base.Pickup(player);
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.OnPreDodgeRoll -= Player_OnPreDodgeRoll;
            player.OnKilledEnemy -= Player_OnKilledEnemy;

            return base.Drop(player);
        }
        public int amt2Roll = 5;
        private void Player_OnKilledEnemy(PlayerController obj)
        {
            if (obj != null)
            {

               
                if (tokens != (5*amt2Roll))
                {
                    tokens++;
                    if (tokens % amt2Roll == 0)
                    {
                        AkSoundEngine.PostEvent("Play_ENM_bullet_dash_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_ENM_bullet_dash_01", base.gameObject);
                        AkSoundEngine.PostEvent("Play_ENM_bullet_dash_01", base.gameObject);
                    }
                }
                if (tokens > (5 * amt2Roll))
                {
                    tokens = (5 * amt2Roll);
                }

                

            }
        }
        public int tokens;
        public int Extratime = 30;
        private void Player_OnPreDodgeRoll(PlayerController obj)
        {
            if(tokens >= amt2Roll)
            {
                tokens = tokens - amt2Roll;
                StartCoroutine(DoDarkRoll(obj));
                
                zoomy = obj.gameObject.GetOrAddComponent<ImprovedAfterImage>();
                zoomy.dashColor = new Color(0, 0, 0);
                zoomy.spawnShadows = true;
                zoomy.shadowTimeDelay = .1f;
                zoomy.shadowLifetime = 1f;
                zoomy.minTranslation = 0.1f;
            }
        }
        private IEnumerator DoDarkRoll(PlayerController obj)
        {
            if(obj != null)
            {
                while(obj.IsDodgeRolling || obj.CurrentRollState == PlayerController.DodgeRollState.InAir)
                {
                    obj.healthHaver.IsVulnerable = false;
                }
                StartCoroutine(DestroyEnemyBulletsInCircleForDuration());
                yield return new WaitForSeconds(1.5f);
                obj.healthHaver.IsVulnerable = true;
                StopShadows();
                
                yield return null;
            }

            yield return null;
        }

        public IEnumerator DestroyEnemyBulletsInCircleForDuration()
        {
            float duration = 1.5f;
            float radius = 2f;
           
            float ela = 0f;
            while (ela < duration)
            {
                Vector2 center = this.Owner.transform.localPosition + new Vector3(.5f, .5f);
                ela += BraveTime.DeltaTime;
                SilencerInstance.DestroyBulletsInRange(center, radius, true, false, null, false, null, false, null);
                yield return null;
            }
            yield break;
        }

        private void StopShadows()
        {
            zoomy.spawnShadows = false;
        }
        public bool RunningPoof = false;
        public override void Update()
        {

            if (this.Owner)
            {
                if(tokens >= amt2Roll)
                {
                    if (RunningPoof != true && this.Owner.Velocity.magnitude > 0)
                    {
                        StartCoroutine(doPoof());
                    }
                    
                }
               

                base.Update();
            }
        }

        private IEnumerator doPoof()
        {
            RunningPoof = true;
            yield return new WaitForSeconds(.25f);
            GlobalSparksDoer.DoRandomParticleBurst(4, this.Owner.sprite.WorldBottomLeft + new Vector2(.25f, 0), this.Owner.sprite.WorldBottomRight + new Vector2(-.25f, 0), Vector3.zero, .1f, .15f, null, null, null, GlobalSparksDoer.SparksType.DARK_MAGICKS);
            RunningPoof = false;

        }
    }
}

