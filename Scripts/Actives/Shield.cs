using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class shield : PlayerItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Hardlight Shield";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Hardlight_Shield";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<shield>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Get Behind My Barrier";
            string longDesc = "This sheild made from smart bullet hardlight technology is specifically designed to amplify your bullets and nullify enemy bullets." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Adds the actual passive effect to the item


            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Damage, 500f);


            //Set the rarity of the item



            item.quality = PickupObject.ItemQuality.C;
            ID = item.PickupObjectId;
        }
        public static int ID;

        public GameObject barrier;
        public override void  DoEffect(PlayerController user)
        {
            vangaurd = true;
            Gun owner = PickupObjectDatabase.GetById(380) as Gun;
            Gun currentGun = this.LastOwner.CurrentGun;
            GameObject gameObject = owner.ObjectToInstantiateOnReload.gameObject;
            GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, owner.sprite.WorldCenter, Quaternion.identity);
            SingleSpawnableGunPlacedObject @interface = gameObject2.GetInterface<SingleSpawnableGunPlacedObject>();
            BreakableShieldController component = gameObject2.GetComponent<BreakableShieldController>();
            bool flag3 = gameObject2;
            if (flag3)
            {

                component.maxDuration = 20;

                @interface.Initialize(currentGun);
                component.Initialize(currentGun);

                StartCoroutine(ItemBuilder.HandleDuration(this, 19.5f, user, EndEffect));
            }
            barrier = gameObject2;
        }
        public Vector2 barrier_center;
        public Vector2 bullet;
        public bool vangaurd = false;
        public override void Update()
        {

            if (this.LastOwner && vangaurd)
            {
                proximity(this.LastOwner);
            }





            base.Update();
        }
        protected void EndEffect(PlayerController user)
        {
            vangaurd = false;
        }

        public void proximity(PlayerController player)
        {   // gets and compares distance to every bullet checking id they are witin 1.55 meters and awards stacks
            if (barrier != null)
            {
                barrier_center = barrier.transform.position;
                foreach (var projectile in GetBullets())
                {

                    bullet = (Vector2)projectile.LastPosition;

                    float radius = 1.55f;
                    if (Vector2.Distance(bullet, barrier_center) < radius)
                    {
                        projectileStates buffed = projectile.gameObject.GetOrAddComponent<projectileStates>();
                        if (buffed.boostedbyHarlight == false)
                        {
                            float damage = projectile.baseData.damage;
                            projectile.baseData.damage = damage * 1.5f;

                            projectile.baseData.speed *= 1.2f;


                            AkSoundEngine.PostEvent("Play_WPN_energy_accent_01", base.gameObject);
                            projectile.AdjustPlayerProjectileTint(UnityEngine.Color.blue, 1, 0);
                            buffed.boostedbyHarlight = true;
                        }

                    }

                }
            }
           
        }
        private List<Projectile> GetBullets()
        {
            PlayerController player = this.LastOwner;
            List<Projectile> list = new List<Projectile>();
            var allProjectiles = StaticReferenceManager.AllProjectiles;
            for (int i = 0; i < allProjectiles.Count; i++)
            {
                Projectile projectile = allProjectiles[i];
                if (projectile && projectile.sprite && !projectile.ImmuneToBlanks && !projectile.ImmuneToSustainedBlanks)
                {
                    if (projectile.Owner != null && projectile.Owner != aiActor)
                    {
                        if (player != null)
                        {
                            list.Add(projectile);
                        }
                    }
                }
            }
            return list;
        }

        
    }

}
