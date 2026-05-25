using System;
using System.Linq;
using Stirge.Enemy;
using UnityEngine;

namespace Stirge.Enemy
{
    public class AttackTokenDispenser : MonoBehaviour
    {
        [SerializeField]
        private int m_maxTokens = 3;
        private float m_raffleTime = 1;
        private int m_entryLimit = 10;
        public class EntryData
        {
            public Enemy _enemy;
            public float _score;
        }
        private EntryData[] m_entrantList;
        private Transform m_targetReference;
        public void Awake()
        {
            ResetList();
        }

        //enemies can ping the board for a token - done
        //the enemy is given a score
        //the scores are then checked against various variables to find the enemy most eligable to attack.

        private void ResetList()
        {
            m_entrantList = new EntryData[m_entryLimit];
        }
        public bool EnterAttackRaffle(Enemy enterant)
        {
            for (int i = 0; i < m_entrantList.Length; i++)
            {
                if (m_entrantList[i]._enemy == null)
                {
                    m_entrantList[i]._enemy = enterant;
                    return true; //entrant is added to the list
                }
            }
            return false; //entrant not added to the list
        }

        private void ScoreList()
        {
            for (int i = 0; i < m_entrantList.Length; i++)
            {
                m_entrantList[i]._score = Vector3.Distance(m_entrantList[i]._enemy.transform.position, m_targetReference.position);
            }
        }
        public EntryData[] getWinner(bool golfScoring = false)
        {
            EntryData[] winners = new EntryData[m_maxTokens];

            for (int y = 1; y < m_entrantList.Length; y++)
            {
                for (int x = 0; x < m_maxTokens; x++)
                {
                    winners[x] = CompScores(winners[x], m_entrantList[y], golfScoring);
                }
            }
            return winners;
        }
        private EntryData CompScores(EntryData entryA, EntryData entryB, bool golfScoring = false)
        {
            if (entryA == null)
            {
                return entryB;
            }
            if (entryA._score < entryB._score && !golfScoring)
            {
                return entryB;
            }
            else if (entryA._score > entryB._score && golfScoring)
            {
                return entryB;
            }
            return entryA;
        }
    }
}