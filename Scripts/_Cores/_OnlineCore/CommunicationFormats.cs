
using System;

namespace CommunicationFormat{
    public enum CharacterType
    {
        NONE = -1, MARK = 0, LEE_DR, WOLF, VAMPIRE, SM_124, MAROS, MIYA, MR_MA
    }

    public enum CharacterMovement
    {
        ATTACK = 0, DEFENSE, EVADE, SKILL01, SKILL02, SKILL03, FAINT
    }

    public enum ItemType
    {
        NONE
    }

    [System.Serializable]
    public class PlayerData
    {
        private string nickname;                    //User Nickname
        private MoneyData moneyData;                //User Money Data
        private RankData rankData;                  //User Rank Data
        private CharacterData[] characterDatas;     //User Each Characters Data;
        private ItemData[] itemDatas;               //User Item Data 

        public void SetNickname(string data)
        {
            nickname = data;
        }
        public string GetNickname()
        {
            return nickname;
        }

        //Rank Data
        public void SetRankData(RankData data)
        {
            rankData = data;
        }
        public RankData GetRankData()
        {
            return rankData;
        }


        //Money Data
        public void SetMoneyData(MoneyData data)
        {
            moneyData = data;
        }
        public MoneyData GetMoneyData()
        {
            return moneyData;
        }

        public CharacterData[] GetCharactersData()
        {
            return characterDatas;
        }

        public void SetCharactersData(CharacterData[] data)
        {
            characterDatas = data;
        }
    }

    [System.Serializable]
    public class TeamData
    {
        public CharacterData player1;
        public CharacterData player2;
    }

    [System.Serializable]
    public class RankData
    {
        private int currentRank;
        private int currentRankScore;

        public RankData(int currentR, int currentRS)
        {
            this.currentRank = currentR;
            this.currentRankScore = currentRS;
        }

        public void SetRankData(int currentRank)
        {
            this.currentRank = currentRank;
        }

        public int GetRankData()
        {
            return currentRank;
        }

        public void SetRankScoreData(int currentRankScore)
        {
            this.currentRankScore = currentRankScore;
        }

        public int GetRankScoreData()
        {
            return currentRankScore;
        }
    }

    [System.Serializable]
    public class MoneyData
    {
        private int currentCoin;
        private int currentGem;

        public int GetCurrentCoin()
        {
            return currentCoin;
        }
        public void SetCurrentCoin(int currentCoin)
        {
            this.currentCoin = currentCoin;
        }


        public int GetCurrentGem()
        {
            return currentGem;
        }

        public void SetCurrentGem(int currentGem)
        {
            this.currentGem = currentGem; 
        }
    }

    [System.Serializable]
    public class CharacterData {
        private CharacterType type;
        private string rank;
        private string name;
        private int level;
        private int selected;
        private ItemData itemData;
        private int currentXp;
        private int maximumHp;
        private int currentSpeed;
        private string characterInfo;
        private SkillData[] skill_Info;

        public void SetRank(string data)
        {
            this.rank = data;
        }

        public string GetRank()
        {
            return rank;
        }

        public CharacterType GetCharacterType()
        {
            return type;
        }

        public void SetCharacterType(CharacterType value)
        {
            type = value;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string value)
        {
            name = value;
        }

        public int GetLevel()
        {
            return level;
        }

        public void SetLevel(int value)
        {
            level = value;
        }

        public int GetSelected()
        {
            return selected;
        }

        public void SetSelected(int value)
        {
            selected = value;
        }

        public ItemData GetItemData()
        {
            return itemData;
        }

        public void SetItemData(ItemData value)
        {
            itemData = value;
        }

        //Current Xp
        public int GetCurrentXp()
        {
            return currentXp;
        }
       
        public void SetCurrentXp(int value)
        {
            currentXp = value;
        }

        //Maximum Hp
        public void SetMaximumHp(int value)
        {
            maximumHp = value;
        }
        public int GetMaximumHp()
        {
            return maximumHp;
        }

        //Current Speed
        public void SetCurrentSpeed(int value)
        {
            currentSpeed = value;
        }

        public int GetCurrentSpeed()
        {
            return currentSpeed;
        }

        public string GetCharacterInfo()
        {
            return characterInfo;
        }

        public void SetCharacterInfo(string value)
        {
            characterInfo = value;
        }

        public SkillData[] GetSkill_Info()
        {
            return skill_Info;
        }

        public void SetSkill_Info(SkillData[] value)
        {
            skill_Info = value;
        }
    }

    [System.Serializable]
    public class SkillData
    {
        private string name;
        private int damage;
        private int mana;
        private string action;

        public string GetAction()
        {
            return action;
        }

        public void SetAction(string value)
        {
            action = value;
        }

        public int GetMana()
        {
            return mana;
        }

        public void SetMana(int value)
        {
            mana = value;
        }

        public int GetDamage()
        {
            return damage;
        }

        public void SetDamage(int value)
        {
            damage = value;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string value)
        {
            name = value;
        }
    }

    [System.Serializable]
    public class ItemData
    {
        private ItemType type;
        private string name;
        private string info;
        private string effect;
        private bool isUsed;
    }

    [System.Serializable]
    public class RoundData
    {
        private int currentRound;
        private int currentTime;
        private int[] player1Data;
        private int[] player2Data;
    }

    [System.Serializable]
    public class GameTimer
    {
        private int currentRound;
        private int timer;
    }
}