using UnityEngine;
namespace Knives
{
    public class EasyTrailComponent : BraveBehaviour //----------------------------------------------------------------------------------------------
    {
        public EasyTrailComponent()
        {
            //=====
            this.TrailPos = new Vector3(0, 0, 0);
            //======
            this.BaseColor = Color.red;
            this.StartColor = Color.red;
            this.EndColor = Color.white;
            //======
            this.LifeTime = 1f;
            //======
            this.StartWidth = 1;
            this.EndWidth = 0;

        }
        /// <summary>
        /// Lets you add a trail to your projectile.    
        /// </summary>
        /// <param name="TrailPos">Where the trail attaches its center-point to. You can input a custom Vector3 but its best to use the base preset. (Namely"projectile.transform.position;").</param>
        /// <param name="BaseColor">The Base Color of your trail.</param>
        /// <param name="StartColor">The Starting color of your trail.</param>
        /// <param name="EndColor">The End color of your trail. Having it different to the StartColor will make it transition from the Starting/Base Color to its End Color during its lifetime.</param>
        /// <param name="LifeTime">How long your trail lives for.</param>
        /// <param name="StartWidth">The Starting Width of your Trail.</param>
        /// <param name="EndWidth">The Ending Width of your Trail. Not sure why youd want it to be something other than 0, but the options there.</param>
        public void Start()
        {
            obj = base.gameObject;
            {
                TrailRenderer tr;
                var tro = obj.AddChild("trail object");
                tro.transform.position = obj.transform.position;
                tro.transform.localPosition = TrailPos;

                tr = tro.AddComponent<TrailRenderer>();
                tr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                tr.receiveShadows = false;
                var mat = material ?? new Material(Shader.Find("Sprites/Default"));
                tr.material = mat;
                tr.minVertexDistance = 0.1f;
                //======
                mat.SetColor("_Color", BaseColor);
                tr.startColor = StartColor;
                tr.endColor = EndColor;
                //======
                tr.time = LifeTime;
                //======
                tr.startWidth = StartWidth;
                tr.endWidth = EndWidth;
                tr.autodestruct = false;
                trail = tr;
            }
        }

        public TrailRenderer ReturnTrailRenderer()
        {
            return trail;
        }

        private GameObject obj;

        public Vector2 TrailPos;
        public Color BaseColor;
        public Color StartColor;
        public Color EndColor;
        public float LifeTime;
        public float StartWidth;
        public float EndWidth;
        public Material material;
        public TrailRenderer trail;


    }


}
