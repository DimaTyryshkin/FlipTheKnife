using Assets.game.model.knife;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClientServerGameLogicCommon
{
    [Serializable]
    public class EnemyDef
    {
        public string EnemyUserName { get; }
        public string EnemyKnifeName { get; }
        public KnifeMode EnemyKnifeMode { get; }

        public EnemyDef(string enemyUserName, string enemyKnifeName, KnifeMode enemyKnifeMode)
        {
            EnemyUserName = enemyUserName ?? throw new ArgumentNullException(nameof(enemyUserName));
            EnemyKnifeName = enemyKnifeName ?? throw new ArgumentNullException(nameof(enemyKnifeName));
            EnemyKnifeMode = enemyKnifeMode;
        }
    }
}