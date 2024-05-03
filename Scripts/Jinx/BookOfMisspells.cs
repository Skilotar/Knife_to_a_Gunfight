using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemAPI;
using Dungeonator;
using System.Collections.Generic;

namespace Knives
{
    class Book_of_misspelled_spells : PassiveItem
    {
        public static void Register()
        {
            string itemName = "Book of Mispells";
            string resourceName = "Knives/Resources/Book_o_mispells";
            GameObject obj = new GameObject(itemName);
            var item = obj.AddComponent<Book_of_misspelled_spells>();
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);
            string shortDesc = "Ah Braka Dabrah";
            string longDesc =

                "Wizard Enemies Randomize. Eh buk riten bie eh Whizurd thet ded naut pess literatur scool." +
                "\n\n\n - Knife_to_a_Gunfight";
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            item.CanBeDropped = false;
            item.PersistsOnDeath = true;
            item.quality = PickupObject.ItemQuality.B;
            

            itemID = item.PickupObjectId;


            Remove_from_lootpool.RemovePickupFromLootTables(item);
            JinxItemDisplayStorageClass text = item.gameObject.GetOrAddComponent<JinxItemDisplayStorageClass>();
            text.jinxItemDisplayText = "Chance to transmogrify to strong magic enemy or chicken";
        }
        public static int itemID;

        public override void Pickup(PlayerController player)
        {
            player.PostProcessProjectile += Player_PostProcessProjectile;
            base.Pickup(player);
        }

        private void Player_PostProcessProjectile(Projectile projectile, float arg2)
        {
            int rng = UnityEngine.Random.Range(1, 17);
            if (rng == 1)
            {
                projectile.OnHitEnemy += HandleHitEnemy;
                projectile.sprite.color = UnityEngine.Color.blue;
                projectile.sprite.usesOverrideMaterial = true;
                Material mat = new Material(EnemyDatabase.GetOrLoadByName("GunNut").sprite.renderer.material);
                mat.SetColor("_EmissiveColor", new Color32(30, 30, 197, 255));
                mat.SetFloat("_EmissiveColorPower", 6f);
                mat.SetFloat("_EmissivePower", 4);
            }
        }

        private void HandleHitEnemy(Projectile arg1, SpeculativeRigidbody arg2, bool arg3)
        {
            if(arg2 != null)
            {
                if(arg2.aiActor != null)
                {
                    if (arg2.healthHaver.IsBoss != true || arg2.aiActor.EnemyGuid != "465da2bb086a4a88a803f79fe3a27677")
                    {
                        if (arg2.aiActor.IsTransmogrified == false)
                        {

                            int rng2 = UnityEngine.Random.Range(1, 11);
                            if (rng2 <= 6)
                            {
                                arg2.aiActor.Transmogrify(EnemyDatabase.GetOrLoadByGuid("76bc43539fc24648bff4568c75c686d1"), null); // Chiggen
                            }
                            else
                            {
                                StartCoroutine(delayTransmog(arg2.aiActor));
                            }

                        }
                    }
                }
                
            }
            
        }

        public override DebrisObject Drop(PlayerController player)
        {
            player.PostProcessProjectile -= Player_PostProcessProjectile;
            
            return base.Drop(player);
        }

        public override void Update()
        {
            try
            {
                if (this.Owner != null)
                {
                    
                }

            }
            catch
            {

            }

            base.Update();
        }

        private IEnumerator delayTransmog(AIActor aiactor)
        {
            yield return new WaitForSeconds(.01f);
            int lineitem = UnityEngine.Random.Range(0, this.Wizurds.Count);
            string guid = Wizurds[lineitem];
            aiactor.Transmogrify(EnemyDatabase.GetOrLoadByGuid(guid), null);
            aiactor.healthHaver.SetHealthMaximum(aiactor.healthHaver.GetCurrentHealth() * 1.5f, 0, true);
        }

        public List<string> Wizurds = new List<string>
        {
            "844657ad68894a4facb1b8e1aef1abf9",//confirmed
            "c0ff3744760c4a2eb0bb52ac162056e6",//redbook
            "6f22935656c54ccfb89fca30ad663a64",//bluebook
            "a400523e535f41ac80a43ff6b06dc0bf",//greenbook
            
            "c4fba8def15e47b297865b18e36cbef8",//gunjurer
            "9b2cf2949a894599917d4d391a0b7394",//high gunjurer
            "56fb939a434140308b8f257f0f447829",//lore gunjurer
            
        };



    }
}

