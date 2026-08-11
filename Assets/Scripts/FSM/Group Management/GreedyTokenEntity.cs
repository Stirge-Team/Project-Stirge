using Stirge.Combat;
using UnityEngine;
using Stirge.ScoringMethods;
using UnityEngine.UI;
using System.Collections;

namespace Stirge
{

    public class GreedyTokenEntity : Enemy.Enemy
    {
        protected override void AwakeThis()
        {
            
        }
        protected override void OnEnable()
        {
            
        }
        protected override void OnDisable()
        {
            
        }
        public void Update()
        {
            if (!m_hasAttackToken)
            {
                if(AttackTokenDispenser.instance.EnterAttackRaffle(this, new DistanceScore(transform, AttackTokenDispenser.instance.transform)))
                    GetComponent<Image>().color = Color.yellow; //enters
            }
        }
        public override bool GiveToken(float time)
        {
            base.GiveToken(time);
            Debug.Log($"[{name}]: Yippie! I won the raffle!");
            GetComponent<Image>().color = Color.green; //wins
            if (time == 0)
            {
                Debug.Log($"[{name}]: Oh the token doesn't expire? nom!");
                RemoveToken();
            }
            return m_hasAttackToken;
        }
        public override bool RemoveToken()
        {
            Debug.Log($"[{name}]: And the token is gone...");
            GetComponent<Image>().color = Color.orange; //uses
            return base.RemoveToken();
        }
        public override void LostRaffle()
        {
            GetComponent<Image>().color = Color.red; //loses
            StartCoroutine(GiveUpForABit());
            base.LostRaffle();
        }
        private IEnumerator GiveUpForABit()
        {
            enabled = false;
            yield return new WaitForSeconds(1);
            enabled = true;
        }
    }
}