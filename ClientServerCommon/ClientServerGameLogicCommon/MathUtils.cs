using System; 

namespace Assets.game.logic.playground
{
    public static class MathUtils
    {
        //return -180..180 angle
        public static float NormalizeAngle180(float angle)
        {
            angle = angle - (float)Math.Truncate(angle / 360f) * 360f;

            if (angle > 180)
                angle -= 360f;
            else if (angle <= -180)
                angle += 360f;
             

            return angle;
        }
    }
}
