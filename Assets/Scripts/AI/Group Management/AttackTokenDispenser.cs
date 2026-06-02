using System.Collections.Generic;
using UnityEngine;
using Stirge.ScoringMethods;

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
            private Enemy.Enemy m_contestant = null;
            public Enemy.Enemy Contentant => m_contestant;
            private float m_score = 0;
            public float Score => m_score;
            private ScoringMethod m_scoreMethod;
            //HERE IS MY REASONING FOR THIS
            //
            // You could make a class of functions, that can all be different scoring methods
            //

            private bool m_winner = false;
            public bool Winner => m_winner;

            public EntryData(Enemy.Enemy enemy, ScoringMethod scoringMethod)
            {
                m_contestant = enemy;
                m_scoreMethod = scoringMethod;
                m_score = 0;
            }
            public void SetScore()//float value)
            {
                m_score = m_scoreMethod.ScoreFunction();
                //m_score = value;
            }
            public void SetWinner(bool value)
            {
                m_winner = value;
            }
        }
        private List<EntryData> m_entrantList;
        public enum ScoreComparisionMethod
        {
            [InspectorName(null)]
            Flip = -2,
            [InspectorName(null)]
            Reset = -1,
            Standard = 0,
            Golf = 1,
        };
        [SerializeField, Tooltip("The default way to compare entry scores")]
        private ScoreComparisionMethod m_defaultComparisionMethod;
        private ScoreComparisionMethod m_comparisionMethod;

        public void Awake()
        {
            //singleton setup
            if (instance == null)
                instance = this;
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
            m_countdown = m_raffleTime;
            ChangeComparisionMethod(ScoreComparisionMethod.Reset);
            ResetList();
        }
        private void ResetList()
        {
            m_entrantList = new();
            enabled = false;
        }
        public bool EnterAttackRaffle(Enemy.Enemy enterant, ScoringMethod scoreMethod)
        {
            //bounce if the list is full
            if (m_entrantList.Count >= m_entryLimit) return false;

            //check if the incoming request has already entered the raffle
            if (!m_allowRepeatEntries)
                foreach (var entry in m_entrantList)
                    if (entry.Contentant == enterant) return false;

            //if this entry is to be our first, enable the update function
            if (m_entrantList.Count == 0) enabled = true;
            m_entrantList.Add(new(enterant, scoreMethod));

            //check if we've reached the limit on entries to draw immediatly.
            if (m_drawOnLimitReached && m_entrantList.Count == m_entryLimit) DrawRaffle();

            return true; //entrant is added to the list
        }
        /// <summary>
        /// Checks the current raffle to see if the given enterant has already entered.
        /// </summary>
        /// <param name="enterant">The enemy attempting to check</param>
        /// <returns>TRUE if the given enemy is already in the raffle.</returns>
        public bool HaveIEnteredAlready(Enemy.Enemy enterant)
        {
            foreach(var entry in m_entrantList) //check the list
            {
                if(entry.Contentant == enterant) //return true if they are already in
                    return true;
            }
            return false; //otherwise return false.
        }
        /// <summary>
        /// Checks the current raffle to see if the given enterant has already entered, and attempts to enter them if they aren't.
        /// </summary>
        /// <param name="enterant">The enemy attempting to check.</param>
        /// <param name="scoreMethod">The scoring method to use if they are not in the raffle yet.</param>
        /// <returns>TRUE if they are already in the raffle.<br></br>
        /// TRUE if they have been entered into the raffle.<br></br>
        /// FALSE if they weren't able to be added into the raffle.
        /// </returns>
        public bool HaveIEnteredAlready(Enemy.Enemy enterant, ScoringMethod scoreMethod)
        {
            if(HaveIEnteredAlready(enterant)) //return true if already entered
            {
                return true;
            }
            else
            {
                return EnterAttackRaffle(enterant, scoreMethod); //else enter the raffle 
            }
        }
        private void ScoreEntries()
        {
            for (int i = 0; i < m_entrantList.Count; i++)
                if (m_entrantList[i] != null)
                    m_entrantList[i].SetScore();
        }
        private List<EntryData> SortEntrantsByScore()
        {
            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                for (int i = 1; i < m_entrantList.Count; i++)
                    //if golf scoring
                    // current less then previous
                    //else
                    // current greater then previous
                    //suprised inline if statements work here.
                    if ((m_entrantList[i].Score < m_entrantList[i - 1].Score && m_comparisionMethod == ScoreComparisionMethod.Golf) || (m_entrantList[i].Score > m_entrantList[i - 1].Score && m_comparisionMethod == ScoreComparisionMethod.Standard))
                    {
                        EntryData tmp = m_entrantList[i - 1];
                        m_entrantList[i - 1] = m_entrantList[i];
                        m_entrantList[i] = tmp;
                        sorted = false;
                    }
            }

            return m_entrantList;
        }
        public void CheckWinners()
        {
            for (int i = 0; i < m_entrantList.Count; i++)
                if (i < m_maxTokens)
                    m_entrantList[i].Contentant.GiveToken(m_tokenLifetime);
                else
                    m_entrantList[i].Contentant.LostRaffle();
        }

        private void DrawRaffle()
        {
            ScoreEntries();
            SortEntrantsByScore();
            CheckWinners();
            ResetList();
        }
        public void Update()
        {
            //simple timer loop
            if (m_countdown > 0)
                m_countdown -= Time.deltaTime;
            else
            {
                DrawRaffle();
                m_countdown = m_raffleTime;
            }
        }
        public ScoreComparisionMethod ChangeComparisionMethod(ScoreComparisionMethod newMethod = ScoreComparisionMethod.Flip)
        {
            //Alert devs to special enum case being sent!
            if (newMethod < 0)
            {
                Debug.Log("Applying dynamic change to comparision method");
            }

            switch (newMethod)
            {
                case ScoreComparisionMethod.Flip: //-2, Swaps between standard and golf scoring methods
                    if (m_comparisionMethod == ScoreComparisionMethod.Standard)
                        m_comparisionMethod = ScoreComparisionMethod.Golf;
                    else if (m_defaultComparisionMethod == ScoreComparisionMethod.Golf)
                        m_comparisionMethod = ScoreComparisionMethod.Standard;
                    break;
                case ScoreComparisionMethod.Reset: //-1, Resets the current method back to the default
                    m_comparisionMethod = m_defaultComparisionMethod;
                    break;
                case ScoreComparisionMethod.Standard: //0, Larger scores will win
                case ScoreComparisionMethod.Golf: //1, Smaller scores will win
                    m_comparisionMethod = newMethod;
                    break;
                default: //Error in call
                    Debug.Log($"{newMethod} is not a vaild method value! Please try something else");
                    break;
            }

            if(m_comparisionMethod < 0) //THE BIG BAD ERROR. IF YOU GET TO THIS YOU DONE FUCKED UP!
            {
                Debug.LogError("Invalid comparision method set! Reseting to default", this);
                m_comparisionMethod = m_defaultComparisionMethod;
            }

            return m_comparisionMethod;
        }
    }
}