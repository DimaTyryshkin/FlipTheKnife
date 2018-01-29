using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToonKnife.Physics
{
    public struct KnifeStats
    {
        public float length;
        public float startRotationSpeed;
        public float minRotationSpeed;

        /// <summary>
        /// Максимальный угол при котором нож втыкается в землю
        /// </summary>
        public float maxHitAngle;

        public float rotationSpeedDecreaseFactor;
    }
}
