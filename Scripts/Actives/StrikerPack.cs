using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using ItemAPI;
using System.Reflection;

namespace Knives
{
    class Striker_Pack : PlayerItem
    {
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "Knives/Resources/Striker_pack",
            "Knives/Resources/Striker_pack_fire",
            "Knives/Resources/Striker_pack_Burnout",
        };
        public static void Register()
        {

            string itemName = "Striker Pack";

            string resourceName = "Knives/Resources/Striker_pack";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<Striker_Pack>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Hold -Use- To Boost";
            string longDesc = "A particle acceleration pack. Those particles are typically of the user strapped into the harness. Can be used any time there is fuel in the active meter. Running out of fuel will require a full reset." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;

            // item.AddToSubShop(ItemBuilder.ShopType.Flynt, .01f);
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed,10);
            item.UsesNumberOfUsesBeforeCooldown = false;
            item.consumableHandlesOwnDuration = true;
            item.consumable = false;
            item.quality = PickupObject.ItemQuality.B;
            Striker_Pack.spriteIDs = new int[Striker_Pack.spritePaths.Length];
            Striker_Pack.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(Striker_Pack.spritePaths[0], item.sprite.Collection);
            Striker_Pack.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(Striker_Pack.spritePaths[1], item.sprite.Collection);
            Striker_Pack.spriteIDs[2] = SpriteBuilder.AddSpriteToCollection(Striker_Pack.spritePaths[2], item.sprite.Collection);
            item.sprite.SetSprite(spriteIDs[0]);

        }
        public Projectile projectile1 = null;

        public override void Pickup(PlayerController player)
        {

            m_player = player;
            base.Pickup(player);
        }
        public static bool locked = false;
        public static bool UseHeld = false;
        public bool cooldownlocked = false;

        public override void Update()
        {
            if (this.LastOwner != null)
            {
                if (this.IsCurrentlyActive)
                {
                    if (this.LastOwner.CurrentItem == this)
                    {

                        if (Key(GungeonActions.GungeonActionType.UseItem) && !locked)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_jetpack_start_01", base.gameObject);
                            AkSoundEngine.PostEvent("Play_OBJ_jetpack_loop_01", base.gameObject);
                            this.sprite.SetSprite(spriteIDs[1]);
                            UseHeld = true;
                            locked = true;


                        }

                        if (!Key(GungeonActions.GungeonActionType.UseItem) && locked == true)
                        {
                            AkSoundEngine.PostEvent("Play_OBJ_jetpack_end_01", base.gameObject);
                            AkSoundEngine.PostEvent("Stop_OBJ_jetpack_loop_01", base.gameObject);
                            this.sprite.SetSprite(spriteIDs[0]);
                            UseHeld = false;
                            locked = false;



                        }


                        if (UseHeld == true)
                        {

                            StartCoroutine(HandleDash(this.LastOwner));
                           
                            if (beepbeep && beeprunning == false)
                            {
                                StartCoroutine(doWarningBeeps());
                            }
                        }
                        else
                        {
                           
                        }


                    }
                    else
                    {
                        if (IsOnCooldown == true)
                        {
                            this.sprite.SetSprite(spriteIDs[2]);
                            if (cooldownlocked == false)
                            {
                                AkSoundEngine.PostEvent("Play_SteamDown", base.gameObject);
                                AkSoundEngine.PostEvent("Stop_OBJ_jetpack_loop_01", base.gameObject);
                                cooldownlocked = true;
                            }

                        }
                        else
                        {
                            this.cooldownlocked = false;
                            this.sprite.SetSprite(spriteIDs[0]);

                        }
                    }
                }
            }

            base.Update();
        }

        private IEnumerator doWarningBeeps()
        {
            beeprunning = true;
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.gameObject);
            yield return new WaitForSeconds(.1f);
            AkSoundEngine.PostEvent("Play_OBJ_mine_beep_01", base.gameObject);
            beeprunning = false;
        }

        public override void DoEffect(PlayerController user)
        {
            StartCoroutine(HandleMeterUse(this, 1.5f, user, EndEffect));
            AkSoundEngine.PostEvent("Play_OBJ_jetpack_start_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_jetpack_loop_01", base.gameObject);
        }

       

        public IEnumerator HandleDash(PlayerController user)
        {

            DoBurner(user);

            float duration = .10f;
            float adjSpeed = 20;
            float elapsed = -BraveTime.DeltaTime;
            float angle = user.CurrentGun.CurrentAngle;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                this.LastOwner.specRigidbody.Velocity = BraveMathCollege.DegreesToVector(angle).normalized * adjSpeed;
                yield return null;
            }

        }

        private void DoBurner(PlayerController player)
        {
            Projectile projectile1 = ((PickupObjectDatabase.GetById(15) as Gun)).DefaultModule.projectiles[0];
            GameObject gameObject1 = SpawnManager.SpawnProjectile(projectile1.gameObject, player.sprite.WorldCenter - new Vector2(0,.25f), Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle + 180), true);
            Projectile component1 = gameObject1.GetComponent<Projectile>();
            ProjectileSlashingBehaviour slash = component1.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slash.SlashDimensions = 0;
            slash.SlashRange = 0;
            slash.playerKnockback = 0;
            slash.SlashDamage = 0;
            slash.SlashVFX = ((PickupObjectDatabase.GetById(17) as Gun)).muzzleFlashEffects;
            slash.DestroyBaseAfterFirstSlash = true;
            slash.soundToPlay = null;
            component1.Owner = player;

        }

        public static float KnownElaps = 0;
        public static bool beepbeep = false;
        public static IEnumerator HandleMeterUse(PlayerItem item, float duration, PlayerController user, Action<PlayerController> OnFinish)
        {
            if (item.IsCurrentlyActive)
            {
                yield break;
            }
            UseHeld = true;
            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", true);
            SetPrivateType<PlayerItem>(item, "m_activeElapsed", 0f);
            SetPrivateType<PlayerItem>(item, "m_activeDuration", duration);
            item.OnActivationStatusChanged?.Invoke(item);

            float elapsed = GetPrivateType<PlayerItem, float>(item, "m_activeElapsed");
            float dur = GetPrivateType<PlayerItem, float>(item, "m_activeDuration");
            float scale = GetPrivateType<PlayerItem, float>(item, "m_adjustedTimeScale");

            while (GetPrivateType<PlayerItem, float>(item, "m_activeElapsed") < GetPrivateType<PlayerItem, float>(item, "m_activeDuration") && item.IsCurrentlyActive)
            {
                if (UseHeld == true)// decay normaly
                {

                    elapsed += BraveTime.DeltaTime * scale;
                    KnownElaps = elapsed;



                    if ((KnownElaps / dur) >= .8) // 80% used up or above
                    {
                        beepbeep = true;
                    }
                    else
                    {
                        beepbeep = false;
                    }

                }
                if(UseHeld == false)// regen fuel
                {
                    if (KnownElaps > 0)
                    {
                        elapsed = KnownElaps;
                        KnownElaps -= BraveTime.DeltaTime / 5 * scale;
                        
                    }
                    else
                    {

                        SetPrivateType<PlayerItem>(item, "m_activeElapsed", dur);
                        SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", false);
                        item.CurrentTimeCooldown = 0;
                        item.DidDamage(item.LastOwner, 1);
                        item.LastOwner.stats.RecalculateStats(item.LastOwner, true);
                    }
                }
                SetPrivateType<PlayerItem>(item, "m_activeElapsed", elapsed);
                
                yield return null;
            }
            SetPrivateType<PlayerItem>(item, "m_isCurrentlyActive", false);
            item.OnActivationStatusChanged?.Invoke(item);
            OnFinish?.Invoke(user);
            beepbeep = false;
            yield break;
        }
        private void EndEffect(PlayerController obj)
        {

        }
        private static void SetPrivateType<T>(T obj, string field, bool value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
        public PlayerController m_player;
        private bool beeprunning;

        public float KeyTime(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).PressedDuration;
            }
            else
            {
                return 0;
            }

        }

        public bool KeyDown(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).WasPressed;
            }
            else
            {
                return false;
            }

        }

        public bool Key(GungeonActions.GungeonActionType action)
        {
            if (!GameManager.Instance.IsLoadingLevel)
            {
                return BraveInput.GetInstanceForPlayer(m_player.PlayerIDX).ActiveActions.GetActionFromType(action).IsPressed;
            }
            else
            {
                return false;
            }

        }
    }




}