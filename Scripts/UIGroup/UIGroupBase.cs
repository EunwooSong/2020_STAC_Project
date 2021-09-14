using Account;
using CommunicationFormat;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UIAnimation;
using UnityEngine;

namespace UIGroup
{
    public enum UIGroupState
    {
        ENABLED = 0, DISABLED = 1
    }

    public class UIGroupBase : MonoBehaviour
    {
        [Header("UIGroup")]
        [SerializeField] protected SmoothMoveAnimation[] animations;
        [SerializeField] protected UIGroupState groupState;

        virtual public void UpdateEachData() { }
        virtual public void ChangeGroupState(UIGroupState state) {
            groupState = state;
            foreach (SmoothMoveAnimation obj in animations)
            {
                obj.GetComponent<SmoothMoveAnimation>().ChangeIndex((int)state);
            }
        }
    }
}

