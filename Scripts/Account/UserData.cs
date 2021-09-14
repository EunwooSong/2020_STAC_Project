using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommunicationFormat;

namespace Account
{
    public static class UserData
    {
        //계정에 관련된 모든 데이터를 가지고 있음
        private static string _TokenID = null;
        private static PlayerData playerData = null;
        
        public static void SetAccessToken(string accessToken)
        {
            if(_TokenID == null)
                _TokenID = accessToken;
        }
        public static string GetAccessToken()
        {
            return _TokenID;
        }

        public static void SetPlayerData(PlayerData data)
        {
            if (playerData == null)
            {
                playerData = data;
                playerData.SetRankData(new RankData(0, 0));
                playerData.SetMoneyData(new MoneyData());
            }
        }

        public static PlayerData GetPlayerData()
        {
            return playerData;
        }
    }
}