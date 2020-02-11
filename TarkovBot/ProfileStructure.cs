using System;
using System.Collections.Generic;
using System.Text;

namespace TarkovBot
{
    public class Settings
    {
        public string Role { get; set; }
        public string BotDifficulty { get; set; }
        public int? Experience { get; set; }
    }

    public class Info
    {
        public string Nickname { get; set; }
        public string LowerNickname { get; set; }
        public string Side { get; set; }
        public string Voice { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int RegistrationDate { get; set; }
        public string GameVersion { get; set; }
        public int AccountType { get; set; }
        public object MemberCategory { get; set; }
        public bool lockedMoveCommands { get; set; }
        public int SavageLockTime { get; set; }
        public int LastTimePlayedAsSavage { get; set; }
        public Settings Settings { get; set; }
        public bool NeedWipe { get; set; }
        public bool GlobalWipe { get; set; }
        public int NicknameChangeDate { get; set; }
        public List<object> Bans { get; set; }
    }

    public class Customization
    {
        public string Head { get; set; }
        public string Body { get; set; }
        public string Feet { get; set; }
        public string Hands { get; set; }
    }

    public class Hydration
    {
        public int Current { get; set; }
        public int Maximum { get; set; }
    }

    public class Energy
    {
        public int Current { get; set; }
        public int Maximum { get; set; }
    }

    public class Health
    {
        public double Current { get; set; }
        public double Maximum { get; set; }
    }

    public class Head
    {
        public Health Health { get; set; }
    }

    public class Chest
    {
        public Health Health { get; set; }
    }

    public class Stomach
    {
        public Health Health { get; set; }
    }


    public class LeftArm
    {
        public Health Health { get; set; }
    }

    public class RightArm
    {
        public Health Health { get; set; }
    }


    public class LeftLeg
    {
        public Health Health { get; set; }
    }



    public class RightLeg
    {
        public Health Health { get; set; }
    }

    public class BodyParts
    {
        public Head Head { get; set; }
        public Chest Chest { get; set; }
        public Stomach Stomach { get; set; }
        public LeftArm LeftArm { get; set; }
        public RightArm RightArm { get; set; }
        public LeftLeg LeftLeg { get; set; }
        public RightLeg RightLeg { get; set; }
    }

    public class HealthStatus
    {
        public Hydration Hydration { get; set; }
        public Energy Energy { get; set; }
        public BodyParts BodyParts { get; set; }
        public int UpdateTime { get; set; }
    }

    public class MedKit
    {
        public double HpResource { get; set; }
    }

    public class Repairable
    {
        public double MaxDurability { get; set; }
        public double Durability { get; set; }
    }

    public class Key
    {
        public int NumberOfUsages { get; set; }
    }

    public class Sight
    {
        public int SelectedSightMode { get; set; }
    }

    public class Light
    {
        public bool IsActive { get; set; }
        public int SelectedMode { get; set; }
    }

    public class Foldable
    {
        public bool Folded { get; set; }
    }

    public class Upd
    {
        public bool SpawnedInSession { get; set; }
        public int? StackObjectsCount { get; set; }
        public MedKit MedKit { get; set; }
        public Repairable Repairable { get; set; }
        public Key Key { get; set; }
        public Sight Sight { get; set; }
        public Light Light { get; set; }
        public Foldable Foldable { get; set; }
    }

    public class Item
    {
        public string _id { get; set; }
        public string _tpl { get; set; }
        public Upd upd { get; set; }
        public string parentId { get; set; }
        public string slotId { get; set; }
        public object location { get; set; }
    }

    public class FastPanel
    {
    }

    public class Inventory
    {
        public List<Item> items { get; set; }
        public string equipment { get; set; }
        public string stash { get; set; }
        public string questRaidItems { get; set; }
        public string questStashItems { get; set; }
        public FastPanel fastPanel { get; set; }
    }

    public class Common
    {
        public string Id { get; set; }
        public double Progress { get; set; }
        public int PointsEarnedDuringSession { get; set; }
        public int LastAccess { get; set; }
    }

    public class Mastering
    {
        public string Id { get; set; }
        public int Progress { get; set; }
    }

    public class Skills
    {
        public List<Common> Common { get; set; }
        public List<Mastering> Mastering { get; set; }
        public int Points { get; set; }
    }

    public class Item2
    {
        public List<string> Key { get; set; }
        public int Value { get; set; }
    }

    public class SessionCounters
    {
        public List<Item2> Items { get; set; }
    }

    public class Item3
    {
        public List<string> Key { get; set; }
        public int Value { get; set; }
    }

    public class OverallCounters
    {
        public List<Item3> Items { get; set; }
    }

    public class Stats
    {
        public SessionCounters SessionCounters { get; set; }
        public OverallCounters OverallCounters { get; set; }
        public double SessionExperienceMult { get; set; }
        public int ExperienceBonusMult { get; set; }
        public int TotalSessionExperience { get; set; }
        public int LastSessionDate { get; set; }
        public object Aggressor { get; set; }
        public List<object> DroppedItems { get; set; }
        public List<object> FoundInRaidItems { get; set; }
        public List<object> Victims { get; set; }
        public List<object> CarriedQuestItems { get; set; }
        public int TotalInGameTime { get; set; }
        public string SurvivorClass { get; set; }
    }
    public class ConditionCounters
    {
        public List<object> Counters { get; set; }
    }

    public class Area
    {
        public int type { get; set; }
        public int level { get; set; }
        public bool active { get; set; }
        public bool passiveBonusesEnabled { get; set; }
        public int completeTime { get; set; }
        public bool constructing { get; set; }
        public List<object> slots { get; set; }
    }

    public class RagfairInfo
    {
        public double rating { get; set; }
        public bool isRatingGrowing { get; set; }
        public List<object> offers { get; set; }
    }

    public class PlayerData
    {
        public string _id { get; set; }
        public int aid { get; set; }
        public string savage { get; set; }
        public Info Info { get; set; }
        public Customization Customization { get; set; }
        public Health Health { get; set; }
        public Inventory Inventory { get; set; }
        public Skills Skills { get; set; }
        public Stats Stats { get; set; }
        public ConditionCounters ConditionCounters { get; set; }
        public List<object> InsuredItems { get; set; }
        public List<object> Bonuses { get; set; }
        public List<object> Quests { get; set; }
        public RagfairInfo RagfairInfo { get; set; }
        public List<object> WishList { get; set; }
    }

    public class Profile
    {
        public int err { get; set; }
        public object errmsg { get; set; }
        public List<PlayerData> data { get; set; }
    }
}
