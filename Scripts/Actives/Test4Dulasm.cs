
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// using Alexandria.ItemAPI;    -- add me back
using UnityEngine;
using static AkMIDIEvent;
using ItemAPI; // -- remove me
using System.Collections;
using Dungeonator;
using System.Reflection;

namespace Knives
{
    class CallOfTheVoid : PlayerItem
    {
        

        public static void Init()
        {
            string ItemName = "Call Of the Void";

            string SpriteDirectory = "Knives/Resources/Pitlord/Call_of_the_void"; // repair spritepath to original
            GameObject obj = new GameObject(ItemName);

            var item = obj.AddComponent<CallOfTheVoid>();

            ItemBuilder.AddSpriteToObject(ItemName, SpriteDirectory, obj);

            string shortDesc = "The Depths Becon";

            string longDesc = "An ominous stone tablet with glowing glyphs covering it. It has a prayer to the one of the deep, calling him to claim a sacrifice.\n\n";


            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 200);
            PitLordSetup();
            item.quality = PickupObject.ItemQuality.B;
        }

        public PlayerController lastknownowner;
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            
            lastknownowner = player;
        }

        public override void DoEffect(PlayerController user)
        {
            RoomHandler room = user.GetAbsoluteParentRoom();
            AIActor enemy = room.GetRandomActiveEnemy(false);
            
            StartCoroutine(DoFallVisuals(enemy));
            
            base.DoEffect(user);
        }

        public IEnumerator DoFallVisuals(AIActor target)
        {
            GameObject PitGrabby = SpawnManager.SpawnVFX(Pit, target.transform.localPosition, Quaternion.identity);
            PitGrabby.GetComponent<tk2dBaseSprite>().PlaceAtPositionByAnchor(target.GetComponent<tk2dBaseSprite>().WorldCenter, tk2dBaseSprite.Anchor.MiddleCenter);
            PitGrabby.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject("start");
            
            PitGrabby.layer = 0;
            PitGrabby.GetComponent<tk2dBaseSprite>().sprite.UpdateZDepth();

            yield return new WaitForSeconds(.25f);
            if (target.healthHaver.IsBoss == false)
            {
                AkSoundEngine.PostEvent("Play_BOSS_bashellisk_swallow_01", base.gameObject);
                target.FallingProhibited = false;
                target.ForceFall(); 
            }
            else
            {
                target.healthHaver.ApplyDamage(30, Vector2.zero, "PitBite", CoreDamageTypes.Void, DamageCategory.Collision);
            }
            
        }

        public override void  Update()
        {

            if(this.lastknownowner != null)
            {
               
            }
            base.Update();
        }
        public static GameObject Pit;
        public static void PitLordSetup()
        {
            GameObject pit = ItemBuilder.AddSpriteToObject("Pit_VFX", "Knives/Resources/Pitlord/PitLord_swallow_001", null);
            FakePrefab.MarkAsFakePrefab(pit);
            UnityEngine.Object.DontDestroyOnLoad(pit);
            tk2dSpriteAnimator animator = pit.GetOrAddComponent<tk2dSpriteAnimator>();
            tk2dSpriteAnimation animation = pit.AddComponent<tk2dSpriteAnimation>();

            tk2dSpriteCollectionData Babcollection = SpriteBuilder.ConstructCollection(pit, ("pit_Collection"));

            tk2dSpriteAnimationClip idleClip = new tk2dSpriteAnimationClip() { name = "start", frames = new tk2dSpriteAnimationFrame[0], fps = 8 };
            List<tk2dSpriteAnimationFrame> frames = new List<tk2dSpriteAnimationFrame>();

            for (int i = 1; i <= 7; i++)
            {
                tk2dSpriteCollectionData collection = Babcollection;
                int frameSpriteId = SpriteBuilder.AddSpriteToCollection($"Knives/Resources/Pitlord/PitLord_swallow_00{i}", collection);
                tk2dSpriteDefinition frameDef = collection.spriteDefinitions[frameSpriteId];
                frames.Add(new tk2dSpriteAnimationFrame { spriteId = frameSpriteId, spriteCollection = collection });
            }
            idleClip.frames = frames.ToArray();
            idleClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.Library = animation;
            animator.Library.clips = new tk2dSpriteAnimationClip[] { idleClip };
            animator.DefaultClipId = animator.GetClipIdByName("start");
            animator.DefaultClip.wrapMode = tk2dSpriteAnimationClip.WrapMode.Once;
            animator.playAutomatically = true;
            animator.PlayAndDisableObject();


            Pit = pit;

        }
    }
}