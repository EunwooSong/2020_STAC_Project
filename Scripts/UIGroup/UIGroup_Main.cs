using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommunicationFormat;
using Account;
using Account.Team;

namespace UIGroup
{
    public class UIGroup_Main : UIGroupBase
    {
        [Header("Values")]
        [SerializeField] private SpriteRenderer char_1;
        [SerializeField] private SpriteRenderer char_2;

        override public void UpdateEachData()
        {
            CharacterData[] datas = UserData.GetPlayerData().GetCharactersData();

            char_1.sprite = null;
            char_2.sprite = null;

            foreach (CharacterData data in datas)
            {
                if (data.GetSelected() == 0)
                {
                    char_1.sprite = Resources.Load<Sprite>("Character_Info/" + data.GetCharacterType().ToString());
                }
                else if (data.GetSelected() == 1)
                {
                    char_2.sprite = Resources.Load<Sprite>("Character_Info/" + data.GetCharacterType().ToString());
                }
                else
                {
                    continue;
                }
            }
        }

        public override void ChangeGroupState(UIGroupState state)
        {
            base.ChangeGroupState(state);


            if (FindObjectOfType<PlayerTeamManager>().GetComponent<PlayerTeamManager>().beforeState != TeamManagerShowState.CHANGE_CHARACTER)
                return;

            Vector3 tmp = char_1.transform.position;
            tmp.x = -20.0f;
            char_1.transform.position = tmp;

            tmp = char_2.transform.position;
            tmp.x = 20.0f;
            char_2.transform.position = tmp;
        }
    }
}