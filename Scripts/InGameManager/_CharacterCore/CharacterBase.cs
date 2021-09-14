using CommunicationFormat;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEditor;
using UnityEngine;

namespace CharacterCore
{
    public enum CharacterState
    {
        NONE = 0, ATTACK = 1, SKILL1, SKILL2, SKILL3, SHIELD, AVOID, ADNOMAL = -1
    }

    public class CharacterBase : MonoBehaviour
    {
        protected List<CharacterState> m_CharacterActions;

        [Header("Mana, Hp")]
        [SerializeField] public int currentMana;
        [SerializeField] public int currentHp;
        [SerializeField] public int maximumHp;
        [SerializeField] public int currentSpeed;

        [Header("Skill Info")]
        [SerializeField] public SkillData[] skills_Data;

        [Header("Current Action Index")]
        [SerializeField] public int currentIndex = 0;

        [Header("Animator")]
        [SerializeField] private Animator anim;

        //[Header("Character State")]
        //[SerializeField] public int currentState = 0;

        public CharacterBase(CharacterData data)
        {
            currentHp = data.GetMaximumHp();
            maximumHp = data.GetMaximumHp();
            skills_Data = data.GetSkill_Info();
            currentSpeed = data.GetCurrentSpeed();

        }

        private void Awake()
        {
            m_CharacterActions = new List<CharacterState>();
            anim = GetComponent<Animator>();
        }

        public void UpdateStatesArray(CharacterState[] actions, int enemyMaximum)
        {
            if (enemyMaximum > actions.Length)
            {
                m_CharacterActions.Clear();
                int index = 0;
                float counter = 0;
                int differ = enemyMaximum - actions.Length;
                float noneTrigger = (float)actions.Length / differ;


                for(int i = 0; i < enemyMaximum; i++)
                {
                    if(differ > 0)
                    {
                        if(counter > noneTrigger)
                        {
                            counter -= noneTrigger;
                            differ--;

                            m_CharacterActions.Add(CharacterState.NONE);
                        }
                    }
                    else
                    {
                        m_CharacterActions.Add(actions[index++]);
                        counter++;
                    }
                }

                Debug.Log(m_CharacterActions.Count + " " + enemyMaximum);
            }
            else
            {
                m_CharacterActions.Clear();

                foreach (CharacterState action in actions) {
                    m_CharacterActions.Add(action);
                };
            }
        }

        // If My Character Attack User -> CheckIsCanHit
        public virtual bool CheckIsCanHit(CharacterState user_state)
        {
            if(ActionIsAttack())
            if (user_state != CharacterState.SHIELD || user_state != CharacterState.AVOID)
                return true;

            return false;
        }

        public virtual bool ActionIsAttack()
        {
             return 1 <= (int)m_CharacterActions[currentIndex]
                && (int)m_CharacterActions[currentIndex] <= 4;
        }

        public virtual void ChangeEnemyAction(CharacterBase obj) { }


        //------------------------------------------------
        // Value Manager
        //------------------------------------------------
        public int GetCurrentActionIndex()
        {
            return currentIndex;
        }

        public void SetCurrentActionIndex(int index)
        {
            currentIndex = index;
        }

        public CharacterState GetCharacterState()
        {
            return m_CharacterActions[currentIndex];
        }

        public int GetCharacterStateLength()
        {
            return m_CharacterActions.Count;
        }

        public void PlayCurrentAction()
        {
            switch (m_CharacterActions[currentIndex])
            {
                case CharacterState.NONE:
                    break;
                case CharacterState.ATTACK:
                    Attack();
                    break;
                case CharacterState.SKILL1:
                    SKill_1();
                    break;
                case CharacterState.SKILL2:
                    SKill_2();
                    break;
                case CharacterState.SKILL3:
                    SKill_3();
                    break;
                case CharacterState.SHIELD:
                    Shield();
                    break;
                case CharacterState.AVOID:
                    Avoid();
                    break;
                case CharacterState.ADNOMAL:
                    
                    break;
            }

            Debug.Log(m_CharacterActions[currentIndex].ToString());
        }

        //------------------------------------------------
        // Player Method
        //------------------------------------------------
        public virtual void Attack() { anim.SetTrigger("ATTACk"); }
        public virtual void Shield() { anim.SetTrigger("SHIELD"); }
        public virtual void Avoid() {  }
        public virtual void SKill_1() { anim.SetTrigger("SKILL1"); }
        public virtual void SKill_2() { anim.SetTrigger("SKILL2"); }
        public virtual void SKill_3() { anim.SetTrigger("SKILL3"); }
        public virtual void Hit() { anim.SetTrigger("HIT"); }
    }
}


