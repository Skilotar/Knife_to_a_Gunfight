using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using GungeonAPI;

namespace Knives
{
    class CurtainTest : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Curtain";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/tur_collection/tur_nonmounted_right";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<CurtainTest>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Oo pretty object";
            string longDesc = "You're not supposed to have this..." +
                "";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 1f);
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1, StatModifier.ModifyMethod.ADDITIVE);

            //Set the rarity of the item



            item.quality = PickupObject.ItemQuality.EXCLUDED;

           

        }
       
        public override void Pickup(PlayerController player)
        {
            CurtainTest.BuildPrefab();
            user = player;
            base.Pickup(player);
        }

        public override void  DoEffect(PlayerController user)
        {

          
                //Destroy last instance of object if availible
                if (m_Gun != null)
                {
                    UnityEngine.GameObject.DestroyObject(m_Gun);

                }

           // Material mat = new Material(PickupObjectDatabase.GetByEncounterName("GuNNER").sprite.renderer.material);
          //  GameManager.Instance.MainCameraController.renderer.material = mat;

            //make new gun
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(CurtainTest.Gun, this.LastOwner.transform.position + new Vector3(0.6f, 5f, 6f), Quaternion.identity);
                gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor(this.LastOwner.specRigidbody.UnitCenter, tk2dBaseSprite.Anchor.MiddleCenter);
                
                

                m_Gun_position = this.LastOwner.transform.position + new Vector3(0.6f, 1.05f, -6f);
                m_Gun = gameObject;
                

        }

        public static void BuildPrefab()
        {
            GameObject gameObject = SpriteBuilder.SpriteFromResource("Knives/Resources/better_kasa_obake_idle_001", null);
            gameObject.SetActive(false);
            ItemAPI.FakePrefab.MarkAsFakePrefab(gameObject);
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
           
            CurtainTest.Gun = gameObject;

        }
        public bool startup = false;
        public PlayerController user;
       
        public bool dismount = false;

        //update is used as the mount state controller so the code can interact with player item code and interact code at the same time
        //the isOnTurret bool is the interact press state.
        public override void Update()
        {
           

            base.Update();
        }

        public static GameObject Gun;
        public static GameObject m_Gun = null;
        public Vector3 m_Gun_position;
        
    }


}


