using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using Gungeon;
using Dungeonator;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class PaladinsPrayerbook : PlayerItem
    {

        public static void Register()
        {
            //The name of the item
            string itemName = "Paladin's Prayerbook";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/PaladinsPrayerBook";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<PaladinsPrayerbook>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Deus Salvabit Eos";
            string longDesc = "Converts blanks to armor. Activates itself when needed most.\n" +
                "A well worn book bound with a firm leather strap and armored buckle. It radiates with an aura of goodwill." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.None,0);
            
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.A;

            ID = item.PickupObjectId;
        }
        public static int ID;

        public override void Pickup(PlayerController player)
        {
            player.healthHaver.OnPreDeath += HealthHaver_OnPreDeath;
            base.Pickup(player);
        }

        private void HealthHaver_OnPreDeath(Vector2 obj)
        {
            if(this.LastOwner.Blanks > 0)
            {
                PlayerController user = this.LastOwner;
                user.Blanks--;
                bool FullHealth = false;
                if (user.healthHaver.GetMaxHealth() == user.healthHaver.GetCurrentHealth()) FullHealth = true;
                if (FullHealth)
                {
                    user.healthHaver.Armor++;
                    AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);
                }
                else
                {
                    AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", gameObject);
                    user.healthHaver.ApplyHealing(.5f);
                }
                AkSoundEngine.PostEvent("Play_OBJ_compass_point_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_tutorialknight_appear_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_compass_point_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_tutorialknight_appear_01", base.gameObject);
                GameManager.Instance.MainCameraController.SetManualControl(false, false);
                user.StartCoroutine(DoSaveVisuals(user));
                user.StartCoroutine(HandleShield(user));
                this.LastOwner.BloopItemAboveHead(base.sprite, "Knives/Resources/PaladinsPrayerBook");
                StartCoroutine(spawnsparkles(user));
            }
        }

        private IEnumerator DoSaveVisuals(PlayerController user)
        {
            user.spriteAnimator.ForceClearCurrentClip();
            user.spriteAnimator.Play("death");
            user.CurrentInputState = PlayerInputState.NoInput;
            yield return new WaitForSeconds(2f);

            StartCoroutine(this.DestroyEnemyBulletsInCircleForDuration(user.specRigidbody.UnitCenter, 5, .15f));
            Exploder.DoDistortionWave(user.CenterPosition, .5f, 0.04f, 5, .4f);
            user.ToggleGunRenderers(true, "non-death");
            user.ToggleHandRenderers(true, "non-death");
            user.CurrentInputState = PlayerInputState.AllInput;
            user.spriteAnimator.ForceClearCurrentClip();
        }

        public IEnumerator DestroyEnemyBulletsInCircleForDuration(Vector2 center, float radius, float duration)
        {
            float ela = 0f;
            while (ela < duration)
            {
                ela += BraveTime.DeltaTime;
                SilencerInstance.DestroyBulletsInRange(center, radius, true, false, null, false, null, false, null);
                yield return null;
            }
            yield break;
        }


        private IEnumerator HandleShield(PlayerController user)
        {


            user.healthHaver.IsVulnerable = false;
            float elapsed = 0f;
            while (elapsed < 3)
            {

                user.healthHaver.IsVulnerable = false;
                elapsed += BraveTime.DeltaTime;
                yield return null;
            }

            user.healthHaver.IsVulnerable = true;
            yield break;
        }

        public override void DoEffect(PlayerController user)
        {
            if(this.LastOwner.Blanks > 0)
            {
                user.healthHaver.Armor++;
                user.Blanks--;
                
                AkSoundEngine.PostEvent("Play_OBJ_compass_point_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_tutorialknight_appear_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_OBJ_compass_point_01", base.gameObject);
                AkSoundEngine.PostEvent("Play_ENM_tutorialknight_appear_01", base.gameObject);
                this.LastOwner.BloopItemAboveHead(base.sprite, "Knives/Resources/PaladinsPrayerBook");
                StartCoroutine(spawnsparkles(user));
            }
        }
        private IEnumerator spawnsparkles(PlayerController player)
        {
            sparkle(player);
            sparkle(player);
            yield return new WaitForSeconds(.1f);
           
            sparkle(player);
            sparkle(player);
            sparkle(player);
            sparkle(player);
        }

        public override void OnPreDrop(PlayerController user)
        {
            user.healthHaver.OnPreDeath -= HealthHaver_OnPreDeath;
            base.OnPreDrop(user);
        }

        public void sparkle(PlayerController player)
        {
            GameObject Snap = SpawnManager.SpawnVFX(EasyVFXDatabase.Sparkle, (player.CenterPosition), Quaternion.identity);
            tk2dBaseSprite bang = Snap.GetComponent<tk2dBaseSprite>();

            Snap.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor((player.CenterPosition + new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1))), tk2dBaseSprite.Anchor.LowerCenter);
            Snap.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
            bang.HeightOffGround = 1f;
            bang.UpdateZDepth();

        }

        public override void Update()
        {
            if (this.LastOwner != null)
            {
                
            }
            base.Update();
        }


    }
}