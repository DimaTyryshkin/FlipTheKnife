using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToonKnife.Physics
{
    public class KnifePhysicsModel
    {
        public float InputSensitivity = 8f;

        public void Input(float timePress)
        {
            Jump(timePress * InputSensitivity);
        }

        public void Jump(float startSpeed)
        {

        }

        public void Start(KnifeStats knife, float sartAngle, float startSpeed)
        {

        }
    }
}