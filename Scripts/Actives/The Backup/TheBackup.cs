using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using MultiplayerBasicExample;
using System.Collections;

namespace Knives
{
    class The_Backup : PlayerItem
    {
        public static void Register()
        {
            string itemName = "The Backup";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Backup";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<The_Backup>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Support Inbound";
            string longDesc = "An emergency radio. Due to rain-damage it can only be tuned to channel 2. Use to call in a supply of offensive support drones." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item
            //PlayerController owner = item.LastOwner as PlayerController;
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 60f);


            item.quality = PickupObject.ItemQuality.B;


            item.numberOfUses = 1;
            item.UsesNumberOfUsesBeforeCooldown = true;


            ID = item.PickupObjectId;
        }

        public static int ID;

        public bool toggle = false;
        public List<GameObject> m_drones = new List<GameObject>();
        public override void DoEffect(PlayerController user)
        {
            AkSoundEngine.PostEvent("Play_OBJ_turret_set_01", base.gameObject);
            float dura = 20f;
            int num = 4;
            if (user.PlayerHasActiveSynergy("Calling All Units")) num = 6;
            for(int i = 1; i<= num; i++)
            {
                Vector3 offset = new Vector3(0,-2f,1);
                if (i == 1) offset = new Vector3(0, 2f, 1);
                if (i == 2) offset = new Vector3(2f, 0, 1);
                if (i == 3) offset = new Vector3(-2f, 0, 1);
                if (i >= 4) offset = new Vector3(0, -2f, 1);
                StartCoroutine(doDropAndSpawn(user,offset));
               
            }

            StartCoroutine(ItemBuilder.HandleDuration(this, dura, user, EndEffect));

            toggle = true;
        }
        

        private IEnumerator doDropAndSpawn(PlayerController user,Vector3 offset)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(BackupDrone.Drone.OrbitalPrefab.gameObject, this.LastOwner.transform.position + offset, Quaternion.identity);
            
            gameObject.transform.position += new Vector3(0, 15, 0);
            
            float StartY = gameObject.transform.position.y;
            

            m_drones.Add(gameObject);
            float t = 1f;
            while (t > 0)
            {

                float actualY = this.LastOwner.transform.position.y + offset.y;
                float currY = Mathf.Lerp(actualY, StartY, t);
                gameObject.transform.position = new Vector3(this.LastOwner.transform.position.x + offset.x, currY, this.LastOwner.transform.position.z);
                t -= Time.deltaTime;
                yield return null;

            }

            yield return new WaitForSeconds(.1f);
            gameObject.transform.position = this.LastOwner.transform.position + offset;
            BackupShootComp backup = gameObject.GetOrAddComponent<BackupShootComp>();
            backup.Orbital = gameObject;
            backup.parentOwner = user;
            
            PlayerOrbital component = gameObject.GetComponent<PlayerOrbital>();
            component.Initialize(this.LastOwner);

        }

        protected void EndEffect(PlayerController user)
        {
           
            breakThis();
            toggle = false;
        }

       
        private void breakThis()
        {
            foreach (GameObject item in m_drones)
            {
                if (item != null)
                {
                    if (item.gameObject != null)
                    {
                        GlobalSparksDoer.DoRandomParticleBurst(20, item.transform.position, item.transform.position, new Vector3(1f, 1f, 0f), 120f, 0.75f, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
                        UnityEngine.Object.Destroy(item.gameObject);
                        AkSoundEngine.PostEvent("Play_OBJ_turret_fade_01", base.gameObject);
                    }
                }
            }
            m_drones.Clear();

        }

        public override void OnPreDrop(PlayerController user)
        {
            if (toggle)
            {
                breakThis();
                
                toggle = false;
            }

            base.OnPreDrop(user);
        }
    }
}
