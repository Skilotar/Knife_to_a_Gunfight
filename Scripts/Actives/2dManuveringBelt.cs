using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeonator;
using Alexandria.ItemAPI;
using SaveAPI;
using UnityEngine;
using System.Collections;
using System.Reflection;
using Alexandria.Misc;

namespace Knives
{
    class AOTBelt : PlayerItem
    {
        //Call this method from the Start() method of your ETGModule extension class
        public static void Register()
        {
            //The name of the item
            string itemName = "2d Maneuvering Belt";

            //Refers to an embedded png in the project. Make sure to embed your resources! Google it.
            string resourceName = "Knives/Resources/2d-manuver";

            //Create new GameObject
            GameObject obj = new GameObject(itemName);

            //Add a ActiveItem component to the object
            var item = obj.AddComponent<AOTBelt>();

            //Adds a tk2dSprite component to the object and adds your texture to the item sprite collection
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            //Ammonomicon entry variables
            string shortDesc = "Technically 3D";
            string longDesc = "A set of grapping hook launchers attached to the users hips via a reinforced belt. \n" +
                "These were used in the frontier wars by specialized titan combat units. They were used to grapple to the backs of the titans and remove their vulnerable reactor cores." +
                "\n\n\n - Knife_to_a_Gunfight";

            
            ItemBuilder.SetupItem(item, shortDesc, longDesc, "ski");

            //Set the cooldown type and duration of the cooldown
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, .25f);

            //Set some other fields
            item.consumable = false;
            item.quality = ItemQuality.C;
           


            Gun gun = (Gun)ETGMod.Databases.Items[15];
            Projectile projectile2 = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0].projectile);
            projectile2.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile2.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile2);
            projectile2.shouldRotate = true;
            projectile2.baseData.damage = 5f;
            projectile2.baseData.speed = 90f;
            projectile2.baseData.range = 99f;
            projectile2.hitEffects.suppressMidairDeathVfx = true;
            projectile2.pierceMinorBreakables = true;
            
            /*
            ImprovedAfterImage trail = projectile2.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            trail.shadowLifetime = .2f;
            trail.shadowTimeDelay = .001f;
            trail.dashColor = new Color(1.0f, .45f, .0f);
            trail.spawnShadows = true;
            */
            projectile2.SetProjectileSpriteRight("Stinger", 19, 5, false, tk2dBaseSprite.Anchor.LowerLeft, 19, 5);

            proj = projectile2;


            ID = item.PickupObjectId;
        }
        public static int ID;
        public static Projectile proj;
        
        public override void Pickup(PlayerController player)
        {
           
            base.Pickup(player);
        }

        

        public override void OnPreDrop(PlayerController player)
        {

            base.OnPreDrop(player);
        }
        public override void DoEffect(PlayerController player)
        {
            AkSoundEngine.PostEvent("Play_WPN_crossbowshotgun_shot_01", base.gameObject);
            AkSoundEngine.PostEvent("Play_OBJ_hook_shot_01", base.gameObject);
            GameObject gameObject = SpawnManager.SpawnProjectile(proj.gameObject, player.CurrentGun.sprite.WorldCenter, Quaternion.Euler(0f, 0f, (player.CurrentGun == null) ? 0f : player.CurrentGun.CurrentAngle), true);
            Projectile component = gameObject.GetComponent<Projectile>();
            bool flag2 = component != null;
            if (flag2)
            {
                component.Owner = player;
                component.Shooter = player.specRigidbody;
                ProjSticksInWall drill = component.gameObject.GetOrAddComponent<ProjSticksInWall>();
                drill.based = this;

                
            }

            Gun gun = (Gun)PickupObjectDatabase.GetById(126);
            Projectile fake = MiscToolMethods.SpawnProjAtPosi(gun.DefaultModule.projectiles[0].projectile, player.CurrentGun.barrelOffset.transform.position, player,player.CurrentGun.CurrentAngle,0,1,false);
            ProjectileSlashingBehaviour slashy = fake.gameObject.GetOrAddComponent<ProjectileSlashingBehaviour>();
            slashy.SlashVFX = gun.muzzleFlashEffects;
            slashy.SlashDamage = 0;

            GlobalSparksDoer.DoRadialParticleBurst(10, player.sprite.WorldBottomLeft, player.sprite.WorldBottomRight, 90, 5, 2, null, null, null, GlobalSparksDoer.SparksType.FLOATY_CHAFF);
        }

        public static List<Projectile> hooks = new List<Projectile>
        {

        };

        public bool toggle = false;
        public override void Update()
        {
            if (this.LastOwner != null)
            {
                if(hooks.Count != 0 )
                {
                    if (toggle == false)
                    {
                        this.LastOwner.AdditionalCanDodgeRollWhileFlying.AddOverride("AOTHook", null);
                        this.LastOwner.SetIsFlying(true, "AOTHook", false);
                        toggle = true;
                        
                    }
                   
                }
                else
                {
                    if(toggle == true)
                    {
                        this.LastOwner.AdditionalCanDodgeRollWhileFlying.RemoveOverride("AOTHook");
                        this.LastOwner.SetIsFlying(false, "AOTHook", false);
                        toggle = false;
                        
                    }
                   
                }
            }
            base.Update();
        }

        private static void SetPrivateType<T>(T obj, string field, float value)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(obj, value);
        }

        private static T2 GetPrivateType<T, T2>(T obj, string field)
        {
            FieldInfo f = typeof(T).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T2)f.GetValue(obj);
        }

    }




    public class ProjSticksInWall : MonoBehaviour
    {
        public ProjSticksInWall()
        {

        }
        public Projectile Drill;
        public PlayerItem based;
        private void Start()
        {
            m_projectile = base.GetComponent<Projectile>();
            if (m_projectile.Owner is PlayerController)
            {
                player = m_projectile.Owner as PlayerController;
            }
            
            player.OnUsedPlayerItem += Player_OnUsedPlayerItem;
            SpeculativeRigidbody specRigidBody = m_projectile.specRigidbody;
            m_projectile.BulletScriptSettings.surviveTileCollisions = true;
            m_projectile.BulletScriptSettings.surviveRigidbodyCollisions = true;
            m_projectile.pierceMinorBreakables = true;
            m_projectile.specRigidbody.OnPreTileCollision = (SpeculativeRigidbody.OnPreTileCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreTileCollision, new SpeculativeRigidbody.OnPreTileCollisionDelegate(this.OnPreCollisontile));
            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Combine(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));
            m_projectile.OnDestruction += M_projectile_OnDestruction;

            attach(m_projectile);

            ptrail = player.gameObject.GetOrAddComponent<ImprovedAfterImage>();
            ptrail.shadowLifetime = .1f;
            ptrail.shadowTimeDelay = .01f;
            ptrail.dashColor = new Color(220f / 255f, 220f / 255f, 220f / 255f);
            ptrail.spawnShadows = false;
            ptrail.OverrideImageShader = ShaderCache.Acquire("Brave/Internal/DownwellAfterImage");

            

        }
        public void attach(Projectile arg1)
        {

            this.cable = arg1.gameObject.AddComponent<ArbitraryCableDrawer>();
            this.cable.Attach2Offset = new Vector2(0, 0);

            this.cable.Attach1Offset = arg1.sprite.WorldCenter - arg1.transform.position.XY();
           
            this.cable.Initialize(arg1.transform, player.CurrentGun.PrimaryHandAttachPoint);


        }
        private ArbitraryCableDrawer cable;
        

        private void Player_OnUsedPlayerItem(PlayerController arg1, PlayerItem arg2)
        {
            if (arg2.PickupObjectId == based.PickupObjectId && Stuck == true)
            {
                DoBreak();
            }
        }

        private void M_projectile_OnDestruction(Projectile obj)
        {
            EmergencyCleanup(); // OH GEEZ IT GOT DELETED FIX IT NOW!!!
        }
       

        private void OnPreCollisontile(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, PhysicsEngine.Tile tile, PixelCollider tilePixelCollider)
        {
            if(returntrip == false)
            {
                DoStop();
            }
            else
            {
                PhysicsEngine.SkipCollision = true;
            }
        }

        private void OnPreCollison(SpeculativeRigidbody myRigidbody, PixelCollider myPixelCollider, SpeculativeRigidbody otherRigidbody, PixelCollider otherPixelCollider)//Non Wall Collision
        {
            if (otherRigidbody.aiActor)
            {

            }
            else
            {
                if (!otherRigidbody.minorBreakable)
                {
                    DoStop();
                }
            }

        }
        public bool returntrip = false;
        public bool Stuck = false;
        public void DoStop()
        {
             
            Stuck = true;
            AkSoundEngine.PostEvent("Play_WPN_saw_impact_01", base.gameObject);
            m_projectile.baseData.speed *= 0f;
            m_projectile.UpdateSpeed();

            m_projectile.specRigidbody.OnPreRigidbodyCollision = (SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate)Delegate.Remove(m_projectile.specRigidbody.OnPreRigidbodyCollision, new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreCollison));

            StartCoroutine(DeathTimer());
            StartCoroutine(PullPlayer());
            if(Vector2.Distance(m_projectile.sprite.WorldCenter, player.sprite.WorldCenter) > 2f)
            {
                ptrail.spawnShadows = true;
                AOTBelt.hooks.Add(m_projectile);
            }
            
        }

        public float distanceTension;
       
        private IEnumerator PullPlayer()
        {

            distanceTension = Vector2.Distance(m_projectile.sprite.WorldCenter, player.sprite.WorldCenter);
            
            while (m_projectile != null && Stuck == true && player.IsDodgeRolling == false && Vector2.Distance(m_projectile.sprite.WorldCenter , player.sprite.WorldCenter) > 2f)
            {

                Vector2 vector = m_projectile.sprite.WorldCenter - player.sprite.WorldCenter;
                PullDirection = new Vector2(vector.x, vector.y);

                player.MovementModifiers += MovementMod;
                yield return new WaitForSeconds(.1f);
                player.MovementModifiers -= MovementMod;

                if (distanceTension >= Vector2.Distance(m_projectile.sprite.WorldCenter, player.sprite.WorldCenter))
                {
                   
                    distanceTension = Vector2.Distance(m_projectile.sprite.WorldCenter, player.sprite.WorldCenter);
                    
                }
                else
                {
                    StartCoroutine(DoBreak());
                }
                
            }
            player.MovementModifiers -= MovementMod;
            StartCoroutine(DoBreak());

        }
        public void EmergencyCleanup()
        {
            player.MovementModifiers -= MovementMod;
            ptrail.spawnShadows = false;
            AOTBelt.hooks.Remove(m_projectile);
            m_projectile.DieInAir();

        }
        private IEnumerator DoBreak(bool skip = false)
        {
            
            player.MovementModifiers -= MovementMod;
            ptrail.spawnShadows = false;

            AOTBelt.hooks.Remove(m_projectile);
            if (Stuck == true)
            {
                
                returntrip = true;
                Stuck = false;
                while (m_projectile != null && Vector2.Distance(m_projectile.sprite.WorldCenter, player.sprite.WorldCenter) > 2f)
                {
                    m_projectile.baseData.speed = -40f;
                    m_projectile.UpdateSpeed();
                    m_projectile.SendInDirection(m_projectile.sprite.WorldCenter - player.sprite.WorldCenter, true, true);
                    yield return null;
                }

                m_projectile.DieInAir();
            }


            
        }

       public void MovementMod(ref Vector2 voluntaryVal, ref Vector2 involuntaryVal)
        {
            //ETGModConsole.Log(voluntaryVal);
            float involuntaryGradient = involuntaryVal.y / involuntaryVal.x;
            float voluntaryGradient = voluntaryVal.y / voluntaryVal.x;
            involuntaryVal += PullDirection * (25 / distanceTension);
            if (voluntaryGradient.IsBetweenRange(involuntaryGradient - (involuntaryGradient / 4), involuntaryGradient + (involuntaryGradient / 4)))
            {
                voluntaryVal /= .5f;
            }
            //ETGModConsole.Log($"expl grad: {involuntaryGradient}, move grad: {voluntaryGradient}, move: {voluntaryVal}");
        }

        private IEnumerator DeathTimer()
        {
            yield return new WaitForSeconds(8f);
            GameManager.Instance.StartCoroutine(DoBreak());
            if(returntrip != true)
            {
                ptrail.spawnShadows = false;
                m_projectile.DieInAir();
            }
            
        }

        public static ImprovedAfterImage ptrail;
        private  Projectile m_projectile;
        private  PlayerController player;
        public  Vector2 PullDirection;
    }
}
