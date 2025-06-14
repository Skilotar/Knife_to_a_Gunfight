using Alexandria.ItemAPI;
using Alexandria.VisualAPI;
using Alexandria;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Knives
{
    public class BannerPassive : PassiveItem
    {
        // Call setup sprites first then setup banner sprites.
        public void SetSpriteNames(string playerUpSprite, string playerDownSprite, string playerLeftSprite, string playerRightSprite, string playerUpLeftSprite, string playerUpRightSprite)
        {
            
            playerUpSpriteName = playerUpSprite;
            playerDownSpriteName = playerDownSprite;
            playerLeftSpriteName = playerLeftSprite;
            playerRightSpriteName = playerRightSprite;
            playerUpLeftSpriteName = playerUpLeftSprite;
            playerUpRightSpriteName = playerUpRightSprite;
        }

        public void SetupBannerSprites(string name, string upResourcePath, string downResourcePath, string leftResourcePath, string rightResourcePath, string upLeftResourcePath, string upRightResourcePath)
        {
            backpackVfx =
                VFXBuilder.CreateVFX(name, new List<string> { upResourcePath }, 10, new IntVector2(16, 20), tk2dBaseSprite.Anchor.MiddleCenter,
                false, 0, -1, null, tk2dSpriteAnimationClip.WrapMode.Once, persist: true);

            tk2dSprite sprite = backpackVfx.GetComponent<tk2dSprite>();
            SpriteBuilder.AddSpriteToCollection(upResourcePath, sprite.Collection, playerUpSpriteName);
            SpriteBuilder.AddSpriteToCollection(downResourcePath, sprite.Collection, playerDownSpriteName);
            SpriteBuilder.AddSpriteToCollection(upLeftResourcePath, sprite.Collection, playerUpLeftSpriteName);
            SpriteBuilder.AddSpriteToCollection(upRightResourcePath, sprite.Collection, playerUpRightSpriteName);
            SpriteBuilder.AddSpriteToCollection(leftResourcePath, sprite.Collection, playerLeftSpriteName);
            SpriteBuilder.AddSpriteToCollection(rightResourcePath, sprite.Collection, playerRightSpriteName);
        }
        public string playerUpSpriteName, playerDownSpriteName, playerLeftSpriteName, playerRightSpriteName, playerUpLeftSpriteName, playerUpRightSpriteName;
        public GameObject backpackVfx;
        public GameObject backpackInstance;
        public tk2dSprite sprite_instance;
        bool currentlyHidden = false;
        public bool visibleDuringRoll = false;

        public Dictionary<string, Vector2> offsetMapping;
        public override void Update()
        {

            if (Owner == null || sprite_instance == null)
            {
                base.Update();
                return;
            }
            bool shouldBeHidden = Owner.IsFalling || Owner.IsDodgeRolling;
            if (visibleDuringRoll == false && Owner.IsDodgeRolling) shouldBeHidden = false;

            if (!currentlyHidden && shouldBeHidden)
            {
                currentlyHidden = true;
                sprite_instance.renderer.enabled = false;
            }
            else if (currentlyHidden && !shouldBeHidden)
            {
                currentlyHidden = false;
                sprite_instance.renderer.enabled = true;
            }

            if (!currentlyHidden)
            {
                sprite_instance.HeightOffGround = Owner.IsBackfacing() ? 0.2f : -0.6f;
                
                string spriteName = GetSpriteNameFromPlayerDirection();
                if(spriteName != null)
                {
                    sprite_instance.SetSprite(spriteName);
                    Vector2 offset = Offset;
                    if (offset != null)
                    {
                        
                        sprite_instance.PlaceAtLocalPositionByAnchor(offset, tk2dBaseSprite.Anchor.LowerLeft);
                        sprite_instance.UpdateZDepth();
                    }
                    else
                    {
                        ETGModConsole.Log("Offset Null");
                    }
                }
                else
                {
                    ETGModConsole.Log("SpriteName Null");
                }
                sprite_instance.UpdateZDepth();
            }
            
            base.Update();
        }
        public Vector2 Offset = new Vector2(10 / 16f, 2 / 16f); // Default Left
        string GetSpriteNameFromPlayerDirection()
        {
            float currentAngle = Owner.FacingDirection;
            if (currentAngle < -120f)
            {
                Offset = leftOffset;
                return playerLeftSpriteName;
            }
            else if (currentAngle < -60f)
            {
                Offset = downOffset;
                return playerDownSpriteName;
            }
            else if (currentAngle < 25f)
            {
                Offset = rightOffset;
                return playerRightSpriteName;
            }
            else if (currentAngle < 60f)
            {
                Offset = uprightOffset;
                return playerUpRightSpriteName;
            }
            else if (currentAngle < 120f)
            {
                Offset = upOffset;
                return playerUpSpriteName;
            }
            else if (currentAngle < 155f)
            {
                Offset = upLeftOffset;
                return playerUpLeftSpriteName;
            }

            // either angle above 155 (therefore should be facing left) or is out of range (somehow)
            Offset = leftOffset;
            return playerLeftSpriteName;
        }

        public Vector2 leftOffset = new Vector2(10 / 16f, 2 / 16f);
        public Vector2 downOffset = new Vector2(5 / 16f, 4 / 16f);
        public Vector2 rightOffset = new Vector2(3 / 16f, 5 / 16f);
        public Vector2 uprightOffset = new Vector2(7 / 16f, 2 / 16f);
        public Vector2 upOffset = new Vector2(8 / 16f, 2 / 16f);
        public Vector2 upLeftOffset = new Vector2(10 / 16f, 2 / 16f);

        void EnableVisuals()
        {
            //backpackInstance = LastOwner.RegisterAttachedObject(backbackVfx, "jetpack", -0.1f);
            backpackInstance = GameObject.Instantiate(backpackVfx, Owner.CenterPosition, Quaternion.identity, Owner.transform);

            sprite_instance = backpackInstance.GetComponent<tk2dSprite>();
            Vector3 currentPosition = sprite_instance.transform.localPosition;
            //sprite_instance.transform.localPosition = currentPosition + new Vector3(currentPosition.x - 4f / 16f, currentPosition.y - 8f/16f, 0);
            //sprite_instance.PlaceAtPositionByAnchor(LastOwner.CenterPosition + new Vector2(currentPosition.x + 7f / 16f, currentPosition.y + 14f / 16f), tk2dSprite.Anchor.MiddleCenter);
            sprite_instance.PlaceAtLocalPositionByAnchor(new Vector2(8 / 16f, 4 / 16f), tk2dSprite.Anchor.LowerLeft);
            sprite_instance.HeightOffGround = 0.2f;
            sprite_instance.UpdateZDepth();

        }
        void DisableVisuals()
        {
            if (backpackInstance == null)
                return;

            Destroy(backpackInstance);
        }
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            EnableVisuals();
        }
        public override DebrisObject Drop(PlayerController player)
        {
            DisableVisuals();
            return base.Drop(player);
        }
        
        public override void OnDestroy()
        {
            DisableVisuals();
            base.OnDestroy();
        }
    }
}
