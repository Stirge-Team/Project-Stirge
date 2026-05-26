using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Stirge.Combat
{
    public class AttackTokenDispenser : MonoBehaviour
    {
        static public AttackTokenDispenser instance = null;

        [SerializeField, Min(1), Tooltip("The maximum number of tokens that can be given out at once.")]
        private int m_maxTokens = 3;
        [SerializeField, Min(0), Tooltip("How long a token will last before expiring")]
        private float m_tokenLifetime = 1;
        [SerializeField, Min(0), Tooltip("How long this script should wait after getting an entry before automatically drawing token winners\nSet to 0 for instant draws!")]
        private float m_raffleTime = 1;
        private float m_countdown;
        [SerializeField, Min(1), Tooltip("The number of requests allows before triggering a draw event.")]
        private int m_entryLimit = 10;
        [SerializeField, Tooltip("Should the raffle be drawn if all the possible entry slot have been filled.")]
        private bool m_drawOnLimitReached = true;
        [SerializeField, Tooltip("Can Combat Entities submit multiple requests for tokens?")]
        private bool m_allowRepeatEntries = false;
        public class EntryData
        {
            private CombatEntity m_contestant = null;
            public CombatEntity Contentant => m_contestant;
            private float m_score = 0;
            public float Score => m_score;
            private bool m_winner = false;
            public bool Winner => m_winner;

            public EntryData(CombatEntity enemy)
            {
                m_contestant = enemy;
                m_score = 0;
            }
            public void SetScore(float value)
            {
                m_score = value;
            }
            public void SetWinner(bool value)
            {
                m_winner = value;
            }
        }
        private EntryData[] m_entrantList;
        private int m_submittedEntries = 0;
        [SerializeField, Tooltip("The target to base the scoring around (this is to be replaced)")]
        private Transform m_targetReference;
        public void Awake()
        {
            //singleton setup
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.LogError("There are multiple Attack Token Dispensers in this scene, please remove them");
                enabled = false;
            }

            if (m_entryLimit < m_maxTokens)
            {
                Debug.LogWarning("There are more tokens to give then possible entries. The maximum number of entries should ALWAYS be greater then or equal to the maximum number of tokens! (An automatic adjustment has been applied)", this);
                m_entryLimit = m_maxTokens;
            }
            ResetList();
            m_countdown = m_raffleTime;
        }
        private void ResetList()
        {
            m_entrantList = new EntryData[m_entryLimit];
            m_submittedEntries = 0;
        }
        public bool EnterAttackRaffle(CombatEntity enterant)
        {
            //checks for any free slots in the list and returns true is there is and the enemy is added.
            for (int i = 0; i < m_entrantList.Length; i++)
            {
                if (m_entrantList[i] == null)
                {
                    m_entrantList[i] = new(enterant);
                    m_submittedEntries++;

                    if (m_drawOnLimitReached && m_submittedEntries == m_entryLimit)
                    {
                        DrawRaffle();
                    }
                    return true; //entrant is added to the list
                }
                else if (!m_allowRepeatEntries && m_entrantList[i].Contentant == enterant)
                {
                    return false;
                }
            }
            return false; //entrant not added to the list
        }

        private void ScoreEntries()
        {
            for (int i = 0; i < m_entrantList.Length; i++)
            {
                if (m_entrantList[i] != null)
                    //distance check is a placeholder, a more complex check is to be added when the Utility AI system is implemented.
                    m_entrantList[i].SetScore(Vector3.Distance(m_entrantList[i].Contentant.transform.position, m_targetReference.position));
            }
        }
        private EntryData[] SortEntrantsByScore(bool golfScoring = false, bool setAsMainArray = true)
        {
            EntryData[] sortedArray = new EntryData[m_entrantList.Length];

            //go through each slot in the new array
            for (int x = 0; x < sortedArray.Length; x++)
            {
                //compare the score to each of the entrants.
                for (int y = 0; y < m_entrantList.Length; y++)
                {
                    //if either are null skip
                    if (m_entrantList[y] == null)
                    {
                        continue;
                    }
                    else if (sortedArray[x] == null || (m_entrantList[y].Score > sortedArray[x].Score && !golfScoring) || (m_entrantList[y].Score < sortedArray[x].Score && golfScoring))
                    {
                        sortedArray[x] = m_entrantList[y];
                    }
                }
            }
            if(setAsMainArray) m_entrantList = sortedArray;
            return sortedArray;
        }
        public void CheckWinners(bool golfScoring = false)
        {
            EntryData[] winners = new EntryData[m_maxTokens];
            //fill the list of winners (if only 2 entities submitted entries and we're drawing 2 winners, they automatically win)
            for (int i = 0; i < winners.Length; i++)
            {
                winners[i] = m_entrantList[i];
            }
            //goes through the list of winners and compares their scores to the current indexed entry.
            for (int y = m_maxTokens; y < m_entrantList.Length; y++)
            {
                //skip if no entry at index
                if (m_entrantList[y] == null) continue;

                if (m_entrantList[y].Score > m_entrantList[SortEntrantsByScore(golfScoring)].Score)
                {

                }
                //check all the winners against the current indexed entry.
                for (int x = 0; x < winners.Length; x++)
                {
                    //so currently its only comparing the first score, when it should be comparing the highest score.
                    winners[x] = CompScores(winners[x], m_entrantList[y], golfScoring);
                    //if the current winner is the same as the current indexed entry, they have won, so we can stop comparing.
                    if (winners[x] == m_entrantList[y]) break;
                }

            }
            //mark the entries as winners
            foreach (var winner in winners)
            {
                winner.SetWinner(true);
            }
        }
        private EntryData CompScores(EntryData entryA, EntryData entryB, bool golfScoring = false)
        {
            //if there is no value in the first entry.
            if (entryA == null)
            {
                return entryB;
            }
            //incoming greater score
            if (entryA.Score < entryB.Score && !golfScoring)
            {
                return entryB;
            }
            //incoming lesser score
            else if (entryA.Score > entryB.Score && golfScoring)
            {
                return entryB;
            }
            return entryA;
        }
        private void DrawRaffle()
        {
            ScoreEntries();
            CheckWinners(true);
            foreach (var entry in m_entrantList)
            {
                if (entry == null) continue;

                if (entry.Winner)
                    entry.Contentant.GiveToken(m_tokenLifetime);
                else
                    entry.Contentant.LostRaffle();
            }
            ResetList();
        }
        private bool ContainsEntry(EntryData[] array, EntryData data)
        {
            foreach (var item in array)
            {
                if (item == data) return true;
            }
            return false;
        }
        public void Update()
        {
            if (m_submittedEntries == 0)
                return; //no reason to update if there are no entries!

            //simple timer loop
            if (m_countdown > 0)
            {
                m_countdown -= Time.deltaTime;
            }
            else
            {
                DrawRaffle();
                m_countdown = m_raffleTime;
            }
        }
    }
}