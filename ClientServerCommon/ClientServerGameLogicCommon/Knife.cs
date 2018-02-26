using System;
using Assets.game.model.knife; 

namespace Assets.game.logic.playground.common
{
    public delegate void KnifeThrowFail(float rotationSpeed, float deltaAngle);
    public delegate void KnifeThrowSuccess(int flips, bool isPerfect, SkillType skill);

    [Flags]
    public enum SkillType
    {
        None = 0,
        PerfectDeflection = 1 << 0,
        SuccessDeflection = 1 << 1,
        PerfectGold = 1 << 2,
    }

    public class Knife
    {
        public const float PIXELS_TO_UNITS = 0.01f;

        private float m_position;

        private float m_rotation;
        private float m_rotationAfterThrow;
        private float m_rotationSpeed;
        private KnifeState m_state;
        private KnifeDef m_def; 


        private float m_timeAfterThrow = 0f;
        private float m_simulationSpeed = 1f;

        public KnifePhysicsModel knifePhysics;
        KnifePhysicsModel.KnifeTrajectory knifeTrajectory;

        public KnifePhysicsModel.KnifeTrajectory KnifeTrajectory
        {
            get { return knifeTrajectory; }
        }

        public KnifeState state
        {
            get { return m_state; }
        }

        public KnifeDef def
        {
            get { return m_def; }
            set
            {
                m_def = value;
            }
        }

        public float position
        {
            get { return m_position; }
        }

        public float rotation
        {
            get { return m_rotation; }
        }
           
        /// <summary>
        /// 
        /// </summary>
        public float lastCollisionPoint
        {
            get { return 0; }
        }


        public event Action<KnifeState> stateChanged;
          
        public event Action onReset;
        public event Action throwing;
        public event Action<KnifePhysicsModel.Frame> moved;

        public event KnifeThrowSuccess throwSuccess;
        public event KnifeThrowFail throwFail;
        public event Action onFlip;

        public Knife()
        {
            m_def = new KnifeDef();
            m_state = KnifeState.Freeze; 
        }

        public void Update(float dt)
        {
            dt *= m_simulationSpeed;

            m_timeAfterThrow += dt;

            if ((state == KnifeState.Flying || state == KnifeState.Falling))
            {
                var frame = knifeTrajectory.InterpolateFrame(m_timeAfterThrow);

                m_position = frame.senterOfMassY;

                UpdateRotation(frame.angle);

                if (state == KnifeState.Flying)
                {
                    if (frame.speed <= 0)
                    {
                        Fall();
                    }
                }

                if (moved != null)
                    moved(frame);

                if (m_timeAfterThrow >= knifeTrajectory.lastFrame.timeAfterThrow)
                {
                    Collide();
                }
            }

#if PHYSICS_DEBUG
            //if (state == State.Falling || state == State.Flying)
            //    WriteLog();
#endif
        }




        public void Reset()
        {
            m_timeAfterThrow = 0;
            m_flipRotation = 180f;
            m_rotationAfterThrow = 0f;
            m_rotation = 0f;
            m_rotationSpeed = 0f;

            m_position = m_def.size / 2f; // VladV: как-то можно получить начальные координаты ножа?

            SetState(KnifeState.Freeze);

            if (onReset != null)
                onReset();
        }

        public void Throw(float force, float deltaX)
        {
            if (state != KnifeState.Freeze)
                return;
             
            knifePhysics = new KnifePhysicsModel();
            knifeTrajectory = knifePhysics.Throw(def, force, m_rotation);

            SetState(KnifeState.Flying);

            force *= PIXELS_TO_UNITS;

            m_simulationSpeed = def.timeScale;

            m_rotationAfterThrow = 0f;
            m_timeAfterThrow = 0;
            m_rotationSpeed = def.rotationSpeed;

            if (throwing != null)
                throwing();
        }



        private void Collide()
        {
            if (m_state != KnifeState.Falling)
                return;
             
            m_simulationSpeed = 1f;

            int sidesAngle = 360 / def.sides;
            int halfSidesAngle = sidesAngle / 2;
            int flipCount = (int)(Math.Abs(m_rotationAfterThrow) + halfSidesAngle) / sidesAngle;

            //m_rotation = MathUtils.NormalizeAngle180(m_cc.rigidbody2D.rotation);
            m_rotation = MathUtils.NormalizeAngle180(m_rotation);

            float minAngleLeft = 180f;
            float minAngleRight = 180f;
            for (int i = 0; i < def.sides; i++)
            {
                float testAngle = MathUtils.NormalizeAngle180(m_rotation + sidesAngle * i);

                if (testAngle < 0)
                    minAngleLeft = Math.Min(-testAngle, minAngleLeft);
                else
                    minAngleRight = Math.Min(testAngle, minAngleRight);
            }

            SkillType skillUsed = SkillType.None;

            bool isSuccess = (minAngleLeft < def.successDeflectionLeft) || (minAngleRight < def.successDeflectionRight);
            if (!isSuccess && def.hasSkillSuccessDeflection)
            {
                var isSkillSuccess = (minAngleLeft < def.skillSuccessDeflectionLeft) || (minAngleRight < def.skillSuccessDeflectionRight);
                if (isSkillSuccess)
                {
                    skillUsed |= SkillType.SuccessDeflection;
                    isSuccess = true;
                }
            }

            bool isPerfect = (minAngleLeft < def.perfectDeflectionLeft) || (minAngleRight < def.perfectDeflectionRight);
            if (!isPerfect && def.hasSkillPerfectDeflection)
            {
                var isSkillPerfect = (minAngleLeft < def.skillPerfectDeflectionLeft) || (minAngleRight < def.skillPerfectDeflectionRight);
                if (isSkillPerfect)
                {
                    skillUsed |= SkillType.PerfectDeflection;
                    isPerfect = true;
                }
            }



            if (isSuccess)
            {
                Freeze(isPerfect);

                if (throwSuccess != null)
                {
                    throwSuccess(flipCount, isPerfect, skillUsed);
                }
            }
            else
            {
                Free();

                if (throwFail != null)
                {
                    float deltaLeft = Math.Abs(minAngleLeft - def.successDeflectionLeft);
                    float deltaRight = Math.Abs(minAngleRight - def.successDeflectionRight);
                    float deltaAngle = Math.Min(deltaLeft, deltaRight);

                    float rotationSpeed = knifeTrajectory.InterpolateFrame(m_timeAfterThrow).rotationSpeed;

                    throwFail(rotationSpeed, deltaAngle);
                }
            }
        }

        private void Fall()
        {
            SetState(KnifeState.Falling);
        }

        private void Free()
        {
            SetState(KnifeState.Free);
        }

        private void Freeze(bool isPerfect)
        {
            SetState(KnifeState.Freeze);

            m_rotationSpeed = 0f;
        }

        private float m_flipRotation;

        private void UpdateRotation(float newRotation)
        {
            float dr = newRotation - m_rotation;
            m_rotation = newRotation;

            m_rotationAfterThrow += dr;
            m_flipRotation += dr;

            if (m_flipRotation >= 360f)
            {
                m_flipRotation -= 360f;

                if (onFlip != null)
                    onFlip();
            }
        }

        private void SetState(KnifeState state)
        {
            if (m_state == state)
                return;

            m_state = state;

            if (stateChanged != null)
                stateChanged(state);
        }
    }
}