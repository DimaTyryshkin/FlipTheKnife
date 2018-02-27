using System;
using Assets.game.model.knife;
using CodeWriter.Logging;
using OEPFramework; 

namespace Assets.game.logic.playground.common
{
    public struct FailInfo
    {
        public float rotationSpeed;
        public float deltaAngles;

        public override string ToString()
        {
            return string.Format("[rotationSpeed={0}, deltAngles={1}]", rotationSpeed, deltaAngles);
        }
    }

    public struct ThrowInfo
    {
        public int flips;
        public int score;
        public int perfectRow;
        public SkillType skill;
        public int extraStep;

        public int flipsGold;
        public int perfectGold;
        public int extraStepGold;
        public int skillGold;

        public override string ToString()
        {
            return string.Format("[score={0}, flips={1}, perfect={2}, skill={3}, extraStepAward={4}]",
                score, flips, perfectRow, skill, extraStep);
        }
    }

    public class Game
    {
        private static readonly ILog m_log = LogManager.GetLogger(typeof(Game));

        private readonly RawNode m_settings;
        private readonly RawNode m_knivesSettings;

        private float m_time;

        private Random m_rnd;
        private int m_score;
        private int m_perfectFlipCounter;

        private StepAwardCalculator m_stepAwardCalculator;

        private KnifeMode m_knifeMode;

        private Knife m_knife;

        public float time
        {
            get { return m_time; }
        }

        public Knife knife
        {
            get { return m_knife; }
        }

        public int score
        {
            get { return m_score; }
            set { m_score = value; }
        }

        public KnifeDef knifeDef
        {
            get { return m_knife.def; }
        }

        public KnifeMode knifeMode
        {
            get { return m_knifeMode; }
        }

        public event Action<int> scoreChanged;
        public event Action<KnifeMode> knifeModeChanged;
        public event GameThrowSuccess onThrowSuccess;

        public delegate void GameThrowSuccess(ThrowInfo info);

#if PHYSICS_DEBUG
        TestKnifePhysicsModel test;
#endif

        public Game(RawNode knivesSettings, RawNode settings)
        {
            m_rnd = new Random();
            m_settings = settings;
            m_knivesSettings = knivesSettings;

            m_knife = new Knife();
            m_knife.throwSuccess += OnKnifeThrowSuccess;

            m_score = 0;
            m_perfectFlipCounter = 0;
            m_knifeMode = KnifeMode.Medium;

            m_stepAwardCalculator = new StepAwardCalculator(
                requiredScore: m_settings.GetIntArray("flips_bonus_flips"),
                awards: m_settings.GetIntArray("flips_bonus_gold")
            );
        }

        public void Restart()
        {
            m_perfectFlipCounter = 0;

            m_stepAwardCalculator.Restart();
            UpdateScore(0);

            m_knife.Reset();
        }

        public void Update(float dt)
        {
            m_time += dt;

            m_knife.Update(dt);
        }

        public void Throw(float force)
        {
            m_log.DebugFormat("Throw: force={0}", force);

            m_knife.Throw(force, 0);

#if PHYSICS_DEBUG
            test.Start(force);
#endif
        }

        public void SetKnifeMode(string knife, KnifeMode mode)
        {
            m_log.DebugFormat("SetKnifeMode: {0}", mode);

            m_knifeMode = mode;
            m_knife.def = GetKnifeFullDef(knife, m_knifeMode);
#if PHYSICS_DEBUG
            test = new TestKnifePhysicsModel(m_knife.def);
#endif
            if (knifeModeChanged != null)
                knifeModeChanged(mode);
        }

        private KnifeDef GetKnifeFullDef(string knife, KnifeMode mode)
        {
            RawNode node = null;

            string modeKey = KnifeUtils.GetKnifeModeKey(mode);

            var knifeNode = m_knivesSettings.GetNode(knife);
            var modesNode = knifeNode.GetNode("modes");
            if (modesNode.CheckKey(modeKey))
            {
                var modeNode = modesNode.GetNode(modeKey);
                node = modeNode.WithFallback(knifeNode);
            }
            else
            {
                throw new InvalidOperationException("Knife " + knife + " not support " + mode + " mode");
            }

            var modesSettings = m_settings.GetNode("modes");

            if (modesSettings.CheckKey(modeKey))
                node = node.WithFallback(modesSettings.GetNode(modeKey));

            if (modesSettings.CheckKey("default"))
                node = node.WithFallback(modesSettings.GetNode("default"));

            return new KnifeDef(knife, mode, node);
        }

        private void OnKnifeThrowSuccess(int flips, bool perfect, SkillType skill)
        {
#if PHYSICS_DEBUG
            var r = test.Resilt();
            test.SaveToFile(); 
#endif

            m_perfectFlipCounter = perfect ? (m_perfectFlipCounter + 1) : 0;

            var info = new ThrowInfo();
            info.score = flips + m_perfectFlipCounter;

            info.flips = flips;
            info.flipsGold = flips;

            info.perfectRow = m_perfectFlipCounter;
            info.perfectGold = m_perfectFlipCounter;

            info.skill = skill;

            int award;
            if (perfect && CalcSkillPerfectFlipGoldAward(out award))
            {
                info.skill |= SkillType.PerfectGold;
                info.skillGold += award;
            }

            if (m_stepAwardCalculator.CalcAward(info.score, out info.extraStep, out info.extraStepGold))
            {
                // Extra step
            }

            UpdateScore(m_score + info.score);

            m_log.DebugFormat("ThrowSuccess: {0}", info);

            if (onThrowSuccess != null)
                onThrowSuccess(info);
        }

        private bool CalcSkillPerfectFlipGoldAward(out int award)
        {
            award = 0;
            if (!m_knife.def.skillPefectFlipGoldProbability.HasValue)
                return false;

            if (m_rnd.NextDouble() > m_knife.def.skillPefectFlipGoldProbability.Value)
                return false;

            var min = m_knife.def.skillPefectFlipGoldMin;
            var max = m_knife.def.skillPefectFlipGoldMax;
            award = m_rnd.Next(min, max + 1);
            return true;
        }

        private void UpdateScore(int score)
        {
            if (m_score == score)
                return;

            m_score = score;

            if (scoreChanged != null)
                scoreChanged(score);
        }
    }

    class StepAwardCalculator
    {
        private int m_lastAwardIndex;
        private int[] m_requiredScore;
        private int[] m_awards;

        public StepAwardCalculator(int[] requiredScore, int[] awards)
        {
            m_requiredScore = requiredScore;
            m_awards = awards;
            m_lastAwardIndex = -1;
        }

        public void Restart()
        {
            m_lastAwardIndex = -1;
        }

        public bool CalcAward(int score, out int index, out int award)
        {
            for (int i = m_lastAwardIndex + 1; i < m_requiredScore.Length; i++)
            {
                if (score >= m_requiredScore[i])
                {
                    m_lastAwardIndex = index = i;
                    award = m_awards[i];
                    return true;
                }
            }

            index = -1;
            award = 0;
            return false;
        }
    }
}