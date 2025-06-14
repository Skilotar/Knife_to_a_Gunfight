using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using System.Reflection;
using Gungeon;

namespace Knives
{
    class DecompilerItem : PlayerItem
    {
        private static int[] spriteIDs;
        private static readonly string[] spritePaths = new string[]
        {
            "Knives/Resources/Decompiler",
            "Knives/Resources/Decompiler_Bad"

        };
        public static void Register()
        {
            string itemName = "Decomplier";

            string resourceName = "Knives/Resources/Decompiler";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<DecompilerItem>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "I Will Unmake You";
            string longDesc =
                "Use on items to reform them into ammo. \n\n" +
                "This device was originally made for synthesizing culinary commodities, " +
                "however since the robot uprising it has been stuck on ammo production. " +
                
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 1500f);
            

            item.quality = PickupObject.ItemQuality.B;
            item.numberOfUses = 3;
            item.UsesNumberOfUsesBeforeCooldown = true;

            DecompilerItem.spriteIDs = new int[DecompilerItem.spritePaths.Length];
            DecompilerItem.spriteIDs[0] = SpriteBuilder.AddSpriteToCollection(DecompilerItem.spritePaths[0], item.sprite.Collection);
            DecompilerItem.spriteIDs[1] = SpriteBuilder.AddSpriteToCollection(DecompilerItem.spritePaths[1], item.sprite.Collection);

            ID = item.PickupObjectId;
        }

        public static int ID;

        public override void DoEffect(PlayerController user)
        {
            if (user)
            {
                
                IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.sprite.WorldCenter, 1.5f, user);
                if(nearestInteractable != null)
                {
                    PickupObject pick = (nearestInteractable as PickupObject);
                    AkSoundEngine.PostEvent("Play_BOSS_RatMech_Target_01", base.gameObject);
                    StartCoroutine(HandleItemSuck(pick));
                    

                }
                else
                {
                    if (user.PlayerHasActiveSynergy("Immoral Material Collection"))
                    {
                        Func<AIActor, bool> isValid = (AIActor a) => a && a.HasBeenEngaged && a.healthHaver && a.healthHaver.IsVulnerable && a.healthHaver.IsAlive && !a.IsGone && !a.healthHaver.IsBoss && a.CompanionOwner == null && a.gameObject.GetComponent<CompanionController>() == null;
                        AIActor closestToPosition = BraveUtility.GetClosestToPosition<AIActor>(GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(user.CenterPosition.ToIntVector2()).GetActiveEnemies(Dungeonator.RoomHandler.ActiveEnemyType.RoomClear), user.CenterPosition, isValid, new AIActor[] { });
                        AkSoundEngine.PostEvent("Play_BOSS_RatMech_Target_01", base.gameObject);
                        StartCoroutine(HandleEnemySuck(closestToPosition));
                        closestToPosition.EraseFromExistenceWithRewards();
                    }
                }

            }
        }

        private IEnumerator HandleItemSuck(PickupObject target)
        {
            //ETGModConsole.Log("StartSuck");
            this.CanBeDropped = false;
            this.LastOwner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, 1000));
            Transform copySprite = this.CreateEmptyItemSprite(target);
            Vector3 startPosition = copySprite.transform.position;
            target.gameObject.transform.position = target.gameObject.transform.position + new Vector3(100, 0, 0); // need it away from the player to keep bug with item tips from occuring.
            UnityEngine.GameObject.Destroy(target.gameObject);
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool flag = this.LastOwner && copySprite;
                yield return new WaitForEndOfFrame();
                if (flag)
                {
                    Vector3 position = this.LastOwner.CenterPosition;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    position = default(Vector3);
                }
                yield return null;
            }
            bool flag2 = copySprite;
            if (flag2)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }

            yield return new WaitForSeconds(.25f);
            DebrisObject ammo = LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, this.LastOwner.sprite.WorldCenter, new Vector3(0, 0, 1), 10f, false, false, false);
            ammo.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/GlitchUnlit");
            ammo.sprite.usesOverrideMaterial = true;
            ammo.sprite.renderer.material.SetFloat("_GlitchInterval", 0.16f);
            ammo.sprite.renderer.material.SetFloat("_DispProbability", 0.2f);
            ammo.sprite.renderer.material.SetFloat("_DispIntensity", 0.015f);
            ammo.sprite.renderer.material.SetFloat("_ColorProbability", 0.2f);
            ammo.sprite.renderer.material.SetFloat("_ColorIntensity", 0.02f);
            PickupObject ammoPickup = ammo.gameObject.GetComponent<PickupObject>();
            ammoPickup.IgnoredByRat = true;

            this.LastOwner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, -1000));
            this.CanBeDropped = true;
            yield break;
        }

        private Transform CreateEmptyItemSprite(PickupObject target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
           
            return gameObject2.transform;
        }

        private IEnumerator HandleEnemySuck(AIActor target)
        {
            //ETGModConsole.Log("StartSuck");
            this.CanBeDropped = false;
            this.LastOwner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, 1000));
            Transform copySprite = this.CreateEmptySprite(target);
            Vector3 startPosition = copySprite.transform.position;
            float elapsed = 0f;
            float duration = 0.5f;
            while (elapsed < duration)
            {
                elapsed += BraveTime.DeltaTime;
                bool flag = this.LastOwner && copySprite;
                if (flag)
                {
                    Vector3 position = this.LastOwner.CenterPosition;
                    float t = elapsed / duration * (elapsed / duration);
                    copySprite.position = Vector3.Lerp(startPosition, position, t);
                    copySprite.rotation = Quaternion.Euler(0f, 0f, 360f * BraveTime.DeltaTime) * copySprite.rotation;
                    copySprite.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), t);
                    position = default(Vector3);
                }
                yield return null;
            }
            bool flag2 = copySprite;
            if (flag2)
            {
                UnityEngine.Object.Destroy(copySprite.gameObject);
            }

            yield return new WaitForSeconds(.25f);
            DebrisObject ammo = LootEngine.SpawnItem(PickupObjectDatabase.GetById(78).gameObject, this.LastOwner.sprite.WorldCenter, new Vector3(0, 0, 1), 10f, false, false, false);
            ammo.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/Internal/GlitchUnlit");
            ammo.sprite.usesOverrideMaterial = true;
            ammo.sprite.renderer.material.SetFloat("_GlitchInterval", 0.16f);
            ammo.sprite.renderer.material.SetFloat("_DispProbability", 0.2f);
            ammo.sprite.renderer.material.SetFloat("_DispIntensity", 0.015f);
            ammo.sprite.renderer.material.SetFloat("_ColorProbability", 0.2f);
            ammo.sprite.renderer.material.SetFloat("_ColorIntensity", 0.02f);
            PickupObject ammoPickup = ammo.gameObject.GetComponent<PickupObject>();
            ammoPickup.IgnoredByRat = true;

            this.LastOwner.ownerlessStatModifiers.Add(StatModifier.Create(PlayerStats.StatType.AdditionalItemCapacity, StatModifier.ModifyMethod.ADDITIVE, -1000));
            this.CanBeDropped = true;

            //ETGModConsole.Log("EndSuck");
            yield break;
        }


        private Transform CreateEmptySprite(AIActor target)
        {
            GameObject gameObject = new GameObject("suck image");
            gameObject.layer = target.gameObject.layer;
            tk2dSprite tk2dSprite = gameObject.AddComponent<tk2dSprite>();
            gameObject.transform.parent = SpawnManager.Instance.VFX;
            tk2dSprite.SetSprite(target.sprite.Collection, target.sprite.spriteId);
            tk2dSprite.transform.position = target.sprite.transform.position;
            GameObject gameObject2 = new GameObject("image parent");
            gameObject2.transform.position = tk2dSprite.WorldCenter;
            tk2dSprite.transform.parent = gameObject2.transform;
            bool flag = target.optionalPalette != null;
            if (flag)
            {
                tk2dSprite.renderer.material.SetTexture("_PaletteTex", target.optionalPalette);
            }
            return gameObject2.transform;
        }


        protected void EndEffect(PlayerController user)
        {

        }

        public bool swapped = false;
        public override void Update()
        {

            if (this.LastOwner != null)
            {
                this.CanBeUsed(this.LastOwner);

                if (this.LastOwner.PlayerHasActiveSynergy("Immoral Material Collection"))
                {
                    if(swapped == false)
                    {
                        this.sprite.SetSprite(spriteIDs[1]);
                        swapped = true;
                    }
                }
                else
                {
                    if (swapped == true)
                    {
                        this.sprite.SetSprite(spriteIDs[0]);
                        swapped = false;
                    }
                }
            }
            base.Update();
        }

        public override bool CanBeUsed(PlayerController user)
        {

            bool Cando = false;

            if (checkStuff(user) == false) Cando = true; // Items take priority over enemies

            if (user.PlayerHasActiveSynergy("Immoral Material Collection"))
            {
                if (Vector2.Distance(OMITBMathsAndLogicExtensions.GetPositionOfNearestEnemy(user.CenterPosition, true,false,null,false), user.CenterPosition) <= 3.2f) Cando = true;
            }

            return Cando;
        }

        private bool checkStuff(PlayerController user)
        {
            bool failChecks = false;

            IPlayerInteractable nearestInteractable = user.CurrentRoom.GetNearestInteractable(user.sprite.WorldCenter, 1.5f, user);
            PickupObject pick = (nearestInteractable as PickupObject);
            if (nearestInteractable != null) // Anything Nearby? 
            {
                if (nearestInteractable is PickupObject == false) failChecks = true;
                if (pick != null)
                {
                    if (pick.gameObject.GetComponent<tinyAmmo>() != null) failChecks = true;
                    if (pick.gameObject.GetComponent<AmmoPickup>() != null) failChecks = true;
                }

            }
            else
            {
                failChecks = true;
            }

            return failChecks;
        }
    }
}