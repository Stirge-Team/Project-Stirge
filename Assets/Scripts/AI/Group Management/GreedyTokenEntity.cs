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

        #region overrides
        public override void ApplyRootMotion()
        {
            throw new System.NotImplementedException();
        }


        public override void EnterAirJuggle(float strength, Vector3 direction, float airStallLength, float stunLength)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterKnockback(float strength, Vector3 direction, float height, float stunLength)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterStun(float stunLength)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsGrounded()
        {
            throw new System.NotImplementedException();
        }

        protected override void BeginGoToPosition(Vector3 newPosition)
        {
            throw new System.NotImplementedException();
        }

        protected override float GetMovementSpeed()
        {
            throw new System.NotImplementedException();
        }

        protected override Vector3 GetPosition()
        {
            throw new System.NotImplementedException();
        }

        protected override Quaternion GetRotation()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDamageTaken(int damage)
        {
            throw new System.NotImplementedException();
        }

        protected override void ResetMovementSpeed()
        {
            throw new System.NotImplementedException();
        }

        protected override void SetMovementSpeed(float speed)
        {
            throw new System.NotImplementedException();
        }

        protected override void SetPosition(Vector3 position)
        {
            throw new System.NotImplementedException();
        }

        protected override void SetRotation(Quaternion rotation)
        {
            throw new System.NotImplementedException();
        }

        protected override void SetRotation(Vector3 eulerRotation)
        {
            throw new System.NotImplementedException();
        }

        protected override void StopGoToPosition()
        {
            throw new System.NotImplementedException();
        }

        public override Vector3 GetForward()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}