using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using ItemAPI;

namespace Knives
{
    class dog :PlayerItem
    {
        public static void Register()
        {
            string itemName = "Dog Problem";

            string resourceName = "Knives/Resources/dog";

            GameObject obj = new GameObject(itemName);

            var item = obj.AddComponent<dog>();

            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "You Are Carrying Too Many Dogs";
            string longDesc = "yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip" +
                " yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip" +
                " yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip" +
                " yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip" +
                " yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip" +
                " yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip" +
                " yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip yip" +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.PerRoom, 3f);



            item.consumable = false;
            item.quality = PickupObject.ItemQuality.S;

            ID = item.PickupObjectId;
        }
        public int DogNumber;

        public static int ID;
        public override void Pickup(PlayerController player)
        {
            player.OnNewFloorLoaded += this.OnLoadedFloor;
            base.Pickup(player);
        }
        public override void OnPreDrop(PlayerController user)
        {
            user.OnNewFloorLoaded -= this.OnLoadedFloor;
            base.OnPreDrop(user);
        }
        public override void DoEffect(PlayerController owner)
        {

            string dogGuid = "c07ef60ae32b404f99e294a6f9acba75";
            string turtleGuid = PickupObjectDatabase.GetById(645).GetComponent<MulticompanionItem>().CompanionGuid;

            if (owner.PlayerHasActiveSynergy("A really big problem"))
            {

                for (int i = 0; i < 4; i++)
                {
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(dogGuid);
                    Vector3 vector = owner.transform.position;
                    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                    {
                        vector += new Vector3(1.125f, -0.3125f, 0f);
                    }
                    GameObject extantCompanion = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                    this.m_extantCompanion = extantCompanion;
                    CompanionController orAddComponent = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
                    orAddComponent.Initialize(owner);
                    if (orAddComponent.specRigidbody)
                    {
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
                        DogNumber++;
                    }

                   
                }
                for (int i2 = 0; i2 < 2; i2++)
                {

                    AIActor orLoadByGuid2 = EnemyDatabase.GetOrLoadByGuid(turtleGuid);
                    Vector3 vector2 = owner.transform.position;
                    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                    {
                        vector2 += new Vector3(1.125f, -0.3125f, 0f);
                    }
                    GameObject extantCompanion2 = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid2.gameObject, vector2, Quaternion.identity);
                    this.m_extantCompanion = extantCompanion2;
                    CompanionController orAddComponent2 = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
                    orAddComponent2.Initialize(owner);
                    if (orAddComponent2.specRigidbody)
                    {
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent2.specRigidbody, null, false);

                    }
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(dogGuid);
                    Vector3 vector = owner.transform.position;
                    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                    {
                        vector += new Vector3(1.125f, -0.3125f, 0f);
                    }
                    GameObject extantCompanion = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                    this.m_extantCompanion = extantCompanion;
                    CompanionController orAddComponent = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
                    orAddComponent.Initialize(owner);
                    if (orAddComponent.specRigidbody)
                    {
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
                        DogNumber++;
                    }
                }
            }
            

          

         
           
            
        }

        bool half_toggle = false;
        public void OnLoadedFloor(PlayerController owner)
        {

            if (half_toggle)
            {

                string dogGuid = "c07ef60ae32b404f99e294a6f9acba75";

                for (int i = 0; i < DogNumber; i++)
                {
                    AIActor orLoadByGuid = EnemyDatabase.GetOrLoadByGuid(dogGuid);
                    Vector3 vector = owner.transform.position;
                    if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
                    {
                        vector += new Vector3(1.125f, -0.3125f, 0f);
                    }
                    GameObject extantCompanion = UnityEngine.Object.Instantiate<GameObject>(orLoadByGuid.gameObject, vector, Quaternion.identity);
                    this.m_extantCompanion = extantCompanion;
                    CompanionController orAddComponent = this.m_extantCompanion.GetOrAddComponent<CompanionController>();
                    orAddComponent.Initialize(owner);
                    if (orAddComponent.specRigidbody)
                    {
                        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(orAddComponent.specRigidbody, null, false);
                        
                    }
                }


                half_toggle = false;
            }
            else
            {
                half_toggle = true;
            }


        }

        protected void EndEffect(PlayerController user)
        {
           


        }
        
        private bool hasSynergy = false;
        public override void Update()
        {
            bool flag = this.LastOwner;
            if (flag)
            {
                bool flag2 = this.LastOwner.HasPickupID(PickupObjectDatabase.GetById(301).PickupObjectId);
                if (flag2)
                {
                    this.hasSynergy = true;
                }
                else
                {
                    this.hasSynergy = false;
                }
                

                base.Update();
            }
        }
        private GameObject m_extantCompanion;
    }
}
