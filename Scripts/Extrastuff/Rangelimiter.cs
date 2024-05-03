using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Knives
{

    public class UnchangeableRangeController : BraveBehaviour
    {
        public void Awake()
        {
            if (base.projectile != null)
            {
                this.m_origRange = base.projectile.baseData.range;
            }
        }

        public void Update()
        {
            if (base.projectile != null && base.projectile.baseData.range != this.m_origRange)
            {
                base.projectile.baseData.range = this.m_origRange;
            }
        }

        private float m_origRange;

    }
}
