using System;
using Assets.game.model.knife; 

namespace Assets.game.logic.playground.common
{
    public delegate void KnifeThrowFail(float deltaAngle);
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

        private float m_rotation;
        private float m_rotationAfterThrow;
        private float m_rotationSpeed;
        private State m_state;
        private KnifeDef m_def; 


        private float m_timeAfterThrow = 0f;
        private float m_simulationSpeed = 1f;

        public KnifePhysicsModel knifePhysics;
        KnifePhysicsModel.KnifeTrajectory knifeTrajectory;

        public KnifePhysicsModel.KnifeTrajectory KnifeTrajectory
        {
            get { return knifeTrajectory; }
        }


        public enum State
        {
            Freeze,
            Flying,
            Falling,
            Free,
        }

        public State state
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


        public event Action<State> stateChanged;
          
        public event Action onReset;
        public event Action throwing;
        public event Action<KnifePhysicsModel.Frame> moved;

        public event KnifeThrowSuccess throwSuccess;
        public event KnifeThrowFail throwFail;
        public event Action onFlip;

        public Knife()
        {
            m_def = new KnifeDef();
            m_state = State.Freeze; 
        }

        public void Update(float dt)
        {
            dt *= m_simulationSpeed;

            m_timeAfterThrow += dt;

            if ((state == State.Flying || state == State.Falling))
            {
                var frame = knifeTrajectory.InterpolateFrame(m_timeAfterThrow);

                UpdateRotation(frame.angle);

                if (state == State.Flying)
                {
                    //TODO передалть через проверку высоты
                    if (knifeTrajectory.Velocity(m_timeAfterThrow) <= 0)
                    {
                        Fall();
                    }
                }

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

            SetState(State.Freeze);

            if (onReset != null)
                onReset();
        }

        public void Throw(float force, float deltaX)
        {
            if (state != State.Freeze)
                return;
             
            knifePhysics = new KnifePhysicsModel(def);
            knifeTrajectory = knifePhysics.Throw(force, m_rotation);

            SetState(State.Flying);

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
            if (m_state != State.Falling)
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

                float deltaLeft = Math.Abs(minAngleLeft - def.successDeflectionLeft);
                float deltaRight = Math.Abs(minAngleRight - def.successDeflectionRight);
                float delta = Math.Min(deltaLeft, deltaRight);

                if (throwFail != null)
                {
                    throwFail(delta);
                }
            }
        }

        private void Fall()
        {
            SetState(State.Falling);
        }

        private void Free()
        {
            SetState(State.Free);
        }

        private void Freeze(bool isPerfect)
        {
            SetState(State.Freeze);

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

        private void SetState(State state)
        {
            if (m_state == state)
                return;

            m_state = state;

            if (stateChanged != null)
                stateChanged(state);
        }
    }
}