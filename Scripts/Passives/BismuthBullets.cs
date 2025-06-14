using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;

namespace Knives
{
    class BismuthBullets : PassiveItem
    {
        public static void Register()
        {
            //The name of the item
            string itemName = "Bismuth Bullets";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it
            string resourceName = "Knives/Resources/Bismuth_bullets_01";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a PassiveItem component to the object
            var item = obj.AddComponent<BismuthBullets>();

            //Adds a sprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "ManMade SuperNova";
            string longDesc =

                "An alloy round created on accident as the Hegemony of man attempted to alchemize gold into lead. \n" +
                "Their calculations were off by one leading to a much more expensive and powerful round." +
                "\n\n\n - Knife_to_a_Gunfight";

            //Adds the item to the gungeon item list, the ammonomicon, the loot table, etc.
            //Do this after ItemBuilder.AddSpriteToObject!
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(((Gun)PickupObjectDatabase.GetById(100)).DefaultModule.projectiles[0].projectile);

            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.baseData.damage *= .5f;
            projectile2.baseData.range = 15f;
            projectile2.baseData.force = -10;
            m_beam = projectile2;
            item.quality = PickupObject.ItemQuality.S;
        }

        public static Projectile m_beam;

        public override void Pickup(PlayerController player)
        {

            player.PostProcessProjectile += this.PostProcessProjectile;
            player.GunChanged += this.HandleGunChanged;
            this.Paul = Shader.Find("Brave/Internal/RainbowChestShader");
            this.ProcessGunShader(player.CurrentGun);
            base.Pickup(player);
        }

        private void HandleGunChanged(Gun oldGun, Gun newGun, bool arg3)
        {
            this.RemoveGunShader(oldGun);
            this.ProcessGunShader(newGun);
        }

        private System.Random rng = new System.Random();
        private void PostProcessProjectile(Projectile source, float chance)
        {
            if(source != null)
            {
                PierceProjModifier stabby = source.gameObject.GetOrAddComponent<PierceProjModifier>();
                stabby.penetration = 1;
                BeamBulletsBehaviour beam = source.gameObject.GetOrAddComponent<BeamBulletsBehaviour>();
                beam.firetype = BeamBulletsBehaviour.FireType.BACKWARDS;
                beam.beamToFire = m_beam;
                beam.invokedelay = .1f;
            }

        }

       
        public override DebrisObject Drop(PlayerController player)
        {
            if (player.CurrentGun)
            {
                this.RemoveGunShader(player.CurrentGun);
            }
            player.PostProcessProjectile -= this.PostProcessProjectile;
            player.GunChanged -= this.HandleGunChanged;
            return base.Drop(player);

        }

        private void RemoveGunShader(Gun g)
        {
            if (!g)
            {
                return;
            }
            MeshRenderer component = g.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            List<Material> list = new List<Material>();
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader != this.Paul)
                {
                    list.Add(sharedMaterials[i]);
                }
            }
            component.sharedMaterials = list.ToArray();
        }

        // Token: 0x0600733D RID: 29501 RVA: 0x002CEDAC File Offset: 0x002CCFAC
        private void ProcessGunShader(Gun g)
        {
            MeshRenderer component = g.GetComponent<MeshRenderer>();
            if (!component)
            {
                return;
            }
            Material[] sharedMaterials = component.sharedMaterials;
            for (int i = 0; i < sharedMaterials.Length; i++)
            {
                if (sharedMaterials[i].shader == this.Paul)
                {
                    return;
                }
            }
            Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
            Material material = new Material(this.Paul);
            material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
            sharedMaterials[sharedMaterials.Length - 1] = material;
            component.sharedMaterials = sharedMaterials;
        }
        private Shader Paul;
    }
}
