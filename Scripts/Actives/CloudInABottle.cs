using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MonoMod;
using System.Reflection;
using Gungeon;
using Alexandria.BreakableAPI;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    class CloudInABottle : PlayerItem
    {
        public static void Register()
        {
            string itemName = "Cloud in a Bottle";

            string resourceName = "Knives/Resources/CloudInABottle";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<CloudInABottle>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Chance for Precipitation";
            string longDesc =
                "Toss on the floor for a small cloudy burst, toss in a pit to create a cloudy platform.\n\n" +
                "The process of getting a cloud into a bottle is easier than one might think. First find a cloud, find a bottle, and then spend 60 years and 4 PHD's worth of science to cram that cloud into the bottle.\n" +
                "Clouds are notoriously slippery." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 10f);
            item.usableDuringDodgeRoll = true;
            Cloud = BreakableAPIToolbox.GenerateDebrisObject("Knives/Resources/CloudInABottle", true, 7, 10, 100, 0, null, .2f, null, null, 0);

            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(MiscToolMethods.standardproj);
            projectile.gameObject.SetActive(false);
            projectile.baseData.damage = 2;
            projectile.baseData.speed *= .5f;
            projectile.baseData.range = 20f;
            projectile.SuppressHitEffects = true;
            projectile.sprite.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            projectile.sprite.renderer.material.SetFloat("_VertexColor", 1f);
            projectile.sprite.color = projectile.sprite.color.WithAlpha(0.75f);
            projectile.sprite.usesOverrideMaterial = true;
            projectile.SetProjectileSpriteRight("steam", 12, 12, false, tk2dBaseSprite.Anchor.MiddleCenter, 12, 12);
            projectile.SuppressHitEffects = true;
            projectile.hitEffects.suppressHitEffectsIfOffscreen = true;
            projectile.hitEffects.suppressMidairDeathVfx = true;
            projectile.objectImpactEventName = null;
            PierceProjModifier stabby = projectile.gameObject.GetOrAddComponent<PierceProjModifier>();
            stabby.penetratesBreakables = true;
            stabby.penetration = 5;
            BounceProjModifier bnc = projectile.gameObject.GetOrAddComponent<BounceProjModifier>();
            bnc.numberOfBounces = 1;
            SlowingBulletsEffect slow = projectile.gameObject.GetOrAddComponent<SlowingBulletsEffect>();
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);

           
            cloudPoof = projectile;


            item.quality = PickupObject.ItemQuality.D;
            ID = item.PickupObjectId;
        }

        public static DebrisObject Cloud;
        public static Projectile cloudPoof;
        public static int ID;

        public override void DoEffect(PlayerController player)
        {
            if (player != null)
            {
                StartCoroutine(PoofBomb());
                player.DidUnstealthyAction();
            }
        }

        private IEnumerator PoofBomb()
        {
            Vector2 dir =  this.LastOwner.unadjustedAimPoint - this.LastOwner.CurrentGun.barrelOffset.transform.position;
            GameObject Cloudy = UnityEngine.Object.Instantiate<GameObject>(Cloud.gameObject, this.LastOwner.CenterPosition, Quaternion.identity);
            DebrisObject debris = LootEngine.DropItemWithoutInstantiating(Cloudy, this.LastOwner.CurrentGun.barrelOffset.transform.position, dir, 7, false, false, true, false);

            while (Cloudy.gameObject != null)
            {
                if (GetPrivateType<DebrisObject, bool>(debris, "onGround"))
                {
                    for (int i = 0; i < 12; i++)
                    {
                        if(debris != null)
                        {
                            MiscToolMethods.SpawnProjAtPosi(cloudPoof, debris.transform.position, this.LastOwner, 20 * i);
                        }

                    }
                    AkSoundEngine.PostEvent("Play_obj_glass_break_01", base.gameObject);
                    debris.TriggerDestruction();
                    
                }

                if(GetPrivateType<DebrisObject, bool>(debris, "isFalling"))
                {
                    AkSoundEngine.PostEvent("Play_obj_glass_break_01", base.gameObject);
                    
                    RoomHandler room;
                    room = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(Vector2Extensions.ToIntVector2(debris.transform.position, VectorConversions.Round));
                    CellData cellaim = room.GetNearestCellToPosition(debris.transform.position);
                    
                    if(cellaim != null)
                    {
                        StartCoroutine(Convert(cellaim, room));
                    }
                    UnityEngine.GameObject.Destroy(Cloudy.gameObject);
                }
                yield return null;
            }
            
            yield return null;
        }

        private IEnumerator Convert(CellData cellaim, RoomHandler room)
        {
            if (cellaim != null && room != null)
            {
                for (int i = 0; i <= offsets.Count - 1; i++)
                {
                    CellData targetcell = room.GetNearestCellToPosition((Vector2)(cellaim.position + offsets[i]));
                    if(targetcell != null)
                    {
                        if (targetcell.type == CellType.PIT)
                        {
                            targetcell.type = CellType.FLOOR;
                            MiscToolMethods.GenerateCloudTileAtPosition(GameManager.Instance.Dungeon, room, targetcell.position, 1f, false);
                            MiscToolMethods.SpawnProjAtPosi(cloudPoof, targetcell.position.ToCenterVector2(), this.LastOwner, 20 * i); ;
                            
                        }
                    }
                    
                }

            }

            return null;

        }

        protected void EndEffect(PlayerController user)
        {

        }
       
        public override void Update()
        {

            if (this.LastOwner != null)
            {

                this.CanBeUsed(this.LastOwner);

            }
            base.Update();
        }

        public static Dictionary<int, IntVector2> offsets = new Dictionary<int, IntVector2>
        {
            { 0,new IntVector2(0,0)}, // +
            { 1,new IntVector2(0,1)},
            { 2,new IntVector2(1,0)},
            { 3,new IntVector2(0,-1)},
            { 4,new IntVector2(-1,0)},
            { 5,new IntVector2(0,2)},
            { 6,new IntVector2(2,0)},
            { 7,new IntVector2(0,-2)},
            { 8,new IntVector2(-2,0)},
            { 9,new IntVector2(0,3)},
            { 10,new IntVector2(3,0)},
            { 11,new IntVector2(0,-3)},
            { 12,new IntVector2(-3,0)},

            { 13,new IntVector2(1,3)}, // northeast
            { 14,new IntVector2(1,2)},
            { 15,new IntVector2(1,1)},
            { 16,new IntVector2(2,1)},
            { 17,new IntVector2(3,1)},
            { 18,new IntVector2(2,2)},

            { 19,new IntVector2(1,-3)}, //southeast
            { 20,new IntVector2(1,-2)},
            { 21,new IntVector2(1,-1)},
            { 22,new IntVector2(2,-1)},
            { 23,new IntVector2(3,-1)},
            { 24,new IntVector2(2,-2)},

            { 25,new IntVector2(-1,-3)}, //southwest
            { 26,new IntVector2(-1,-2)},
            { 27,new IntVector2(-1,-1)},
            { 28,new IntVector2(-2,-1)},
            { 29,new IntVector2(-3,-1)},
            { 30,new IntVector2(-2,-2)},

            { 31,new IntVector2(-1,3)}, //northwest
            { 32,new IntVector2(-1,2)},
            { 33,new IntVector2(-1,1)},
            { 34,new IntVector2(-2,1)},
            { 35,new IntVector2(-3,1)},
            { 36,new IntVector2(-2,2)},
        };


        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }
    }
}