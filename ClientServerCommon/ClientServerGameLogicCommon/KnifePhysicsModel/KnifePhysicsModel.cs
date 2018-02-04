using System; 
using System.Collections.Generic; 
using Assets.game.model.knife;
using CodeWriter.Logging;

namespace Assets.game.logic.playground.common 
{
    public class KnifePhysicsModel
    {
        public class KnifeTrajectory
        {
            int lastLeftFrame;
            ILog log;

            public List<Frame> frames;

            public Frame lastFrame { get { return frames[frames.Count - 1]; } }



            public KnifeTrajectory(List<Frame> frames)
            {
                log = LogManager.GetLogger(typeof(KnifeTrajectory));
                this.frames = frames;
            }

            public Frame InterpolateFrame(float time)
            {
                //крайние положения

                if (time > frames[frames.Count - 1].timeAfterThrow)
                    return frames[frames.Count - 1];

                if (time < frames[0].timeAfterThrow)
                    return frames[0];

                int leftIndex = FindLeftFrame(time);
                
                Frame left = frames[leftIndex];
                Frame right = frames[leftIndex + 1];

                float t = (time - left.timeAfterThrow) / (right.timeAfterThrow - left.timeAfterThrow);

                return Frame.LerpUnclamped(left, right, t);
            }
            
            int FindLeftFrame(float time)
            {
                // сначала ищем начиная с того места где в последжний раз нашли.(скорее всего следующиу - нужный нам)
                int index = FindLeftFrame(lastLeftFrame, time);

                if (index == -1)
                {
                    // если не нашли, то полный посик
                    index = FindLeftFrame(0, time);
                }

                if (index != -1)
                    lastLeftFrame = index;


                return index;
            }

            int FindLeftFrame(int start, float time)
            {
                for (int n = start; n < frames.Count; n++)
                {
                    if (frames[n].timeAfterThrow > time)
                        return Math.Max(n - 1, 0);
                }


                return -1;
            }
        }

        public struct Frame
        {
            public static Frame LerpUnclamped(Frame a, Frame b, float t)
            {
                Frame r = new Frame();

                r.timeAfterThrow = LerpUnclamped(a.timeAfterThrow, b.timeAfterThrow, t);
                r.senterOfMassY = LerpUnclamped(a.senterOfMassY, b.senterOfMassY, t);
                r.angle = LerpUnclamped(a.angle, b.angle, t);
                r.rotationSpeed = LerpUnclamped(a.rotationSpeed, b.rotationSpeed, t);
                r.speed = LerpUnclamped(a.speed, b.speed, t);
                r.bladeX = LerpUnclamped(a.bladeX, b.bladeX, t);
                r.bladeY = LerpUnclamped(a.bladeY, b.bladeY, t);
                r.handleX = LerpUnclamped(a.handleX, b.handleX, t);
                r.handleY = LerpUnclamped(a.handleY, b.handleY, t);


                return r;
            }

            private static float LerpUnclamped(float a, float b, float t)
            {
                return a + (b - a) * t;
            }


            public float timeAfterThrow;
            public float senterOfMassY;
            public float angle;
            public float rotationSpeed;
            public float speed;

            public float bladeX;
            public float bladeY;

            public float handleX;
            public float handleY;

            public override string ToString()
            {
                return string.Join(", ", new string[]{
                  timeAfterThrow.ToString(),
                    senterOfMassY.ToString() ,
                   angle.ToString(),
                    bladeX.ToString(),
                    bladeY.ToString(),
                    handleX .ToString(),
                    handleY.ToString() }
                         );
            }
        }

        //float k = 1f / 388.14991f; origin
        //TestOptions testOptions;

        float timeStep = 0.1f;

        KnifeDef knifeDef;
        float knifeMass;
        float knifeLength;
        float startY;


        float PI { get { return (float)Math.PI; } }

        /// <summary>
        /// Фазовый сдвиг вращения лезвия 
        /// </summary>
        float BladePhaseRotation
        {
            get { return -PI * 0.5f; }
        }

        /// <summary>
        /// Фазовый сдвиг вращения ручки
        /// </summary>
        float HandlePhaseRotation
        {
            get { return BladePhaseRotation + PI; }
        }

        public float StartY
        {
            get
            {
                return startY;
            }
        }

        public KnifePhysicsModel( )
        { 
            timeStep = 0.01f;
            knifeMass = 1f; 
        }


        public KnifeTrajectory Throw(KnifeDef knifeDef, float impulse, float startAngle)
        {
            if (knifeDef.gravity > 0) throw new ArgumentException("knifeDef.gravity > 0");
            if (impulse <= 0) throw new ArgumentOutOfRangeException("impulse <= 0");

            this.knifeDef = knifeDef;

            knifeLength = knifeDef.size;

            impulse *= Knife.PIXELS_TO_UNITS;
            impulse *= 1f;//testOptions.impulseMultipler;


            startAngle = AngleToRad(startAngle);//переводим в радианы

            List<Frame> frames = new List<Frame>();

            float oldY = -10000;
            bool flight = true;

            float startSpeed = impulse / knifeMass;
            startY = (knifeLength / 2f) * Cos(startAngle);

            float rorationSpeed = AngleToRad(knifeDef.rotationSpeed);
            float angle = startAngle;
            float time = 0;
            float y = startY;

            frames.Add(new Frame()
            {
                angle = angle,
                senterOfMassY = y,
                timeAfterThrow = time
            });

            while (flight)
            {
                time += timeStep;

                oldY = y;
                y = startY + startSpeed * time + (Knife.PIXELS_TO_UNITS * knifeDef.gravity) * (float)Math.Pow(time, 2) * 0.5f;
                float speed = (y - oldY) / timeStep;

                // Поворот
                rorationSpeed -= AngleToRad(knifeDef.rotationDecrease) * timeStep;
                if (rorationSpeed < AngleToRad(knifeDef.rotationMinSpeed))
                    rorationSpeed = AngleToRad(knifeDef.rotationMinSpeed);

                angle += rorationSpeed * timeStep;


                var frame = new Frame()
                {
                    angle = RadToAngle(angle),//обратно в углы переводим
                    senterOfMassY = y,
                    timeAfterThrow = time,
                    rotationSpeed = RadToAngle(rorationSpeed),
                    speed = speed
                };



                // проверка на удар о землю

                // острие
                float bladeX;
                float bladeY;

                if (CheckGroundCollision(y, angle, BladePhaseRotation, out bladeX, out bladeY))
                {
                    if (oldY > y)
                        flight = false;
                }

                frame.bladeX = bladeX;
                frame.bladeY = bladeY;

                // ручка
                float handleX;
                float handleY;

                if (CheckGroundCollision(y, angle, HandlePhaseRotation, out handleX, out handleY))
                    if (oldY > y)
                        flight = false;

                frame.handleX = handleX;
                frame.handleY = handleY;


                frames.Add(frame);
            }

            return new KnifeTrajectory(frames);
        }

        bool CheckGroundCollision(float centerMassY, float angle, float phaseRotation, out float x, out float y)
        {
            y = centerMassY + Sin(angle + phaseRotation) * knifeLength * 0.5f;
            x = Cos(angle + phaseRotation) * knifeLength * 0.5f;

            return y < 0;
        }

        float RadToAngle(float rad)
        {
            return (rad * 180f) / PI;
        }

        float AngleToRad(float angle)
        {
            return (angle / 180f) * PI;
        }

        float Sin(float x)
        {
            return (float)Math.Sin(x);
        }

        float Cos(float x)
        {
            return (float)Math.Cos(x);
        }
    }
}