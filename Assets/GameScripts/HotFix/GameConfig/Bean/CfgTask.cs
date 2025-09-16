using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameConfig
{
    ///<summary>
    /// 说明: 任务表 ID:20 字段数:12 有效数据行数:8
    /// Created on 2025-09-16 11:34
    ///</summary>
    public class CfgTask : ConfigBase<CfgTask>
    {

        public const string TableName = "task";

        [JsonConstructor]
        private CfgTask([JsonProperty(nameof(Id))] int id,
        [JsonProperty(nameof(Chapter))] int chapter,
        [JsonProperty(nameof(Type))] int type,
        [JsonProperty(nameof(Desc))] string desc,
        [JsonProperty(nameof(GroupType))] int groupType,
        [JsonProperty(nameof(SourceType))] int sourceType,
        [JsonProperty(nameof(GoalList))] IList<int> goalList,
        [JsonProperty(nameof(Condition))] int condition,
        [JsonProperty(nameof(Reward))] IList<Int2IntVal> reward,
        [JsonProperty(nameof(LimitTime))] int limitTime,
        [JsonProperty(nameof(RewardLimitTime))] int rewardLimitTime,
        [JsonProperty(nameof(Visible))] int visible)
        {
            Id = id;
            Chapter = chapter;
            Type = type;
            Desc = desc;
            GroupType = groupType;
            SourceType = sourceType;
            GoalList = goalList;
            Condition = condition;
            Reward = reward;
            LimitTime = limitTime;
            RewardLimitTime = rewardLimitTime;
            Visible = visible;
        }

        /// <summary>
        /// 章节
        /// </summary>
        private int Chapter { get; }
        /// <summary>
        /// 任务大类型
        /// </summary>
        private int Type { get; }
        /// <summary>
        /// 任务描述
        /// </summary>
        private string Desc { get; }
        /// <summary>
        /// 组任务类型
        /// </summary>
        private int GroupType { get; }
        /// <summary>
        /// 任务数据类型
        /// </summary>
        private int SourceType { get; }
        /// <summary>
        /// 任务目标列表
        /// </summary>
        [JsonConverter(typeof(ImmutableListConverter))]
        private IList<int> GoalList { get; }
        /// <summary>
        /// 解锁条件
        /// </summary>
        private int Condition { get; }
        /// <summary>
        /// 奖励道具
        /// </summary>
        [JsonConverter(typeof(ImmutableListConverter))]
        private IList<Int2IntVal> Reward { get; }
        /// <summary>
        /// 限时完成时间
        /// </summary>
        private int LimitTime { get; }
        /// <summary>
        /// 限时完成时间
        /// </summary>
        private int RewardLimitTime { get; }
        /// <summary>
        /// 是否可见
        /// </summary>
        private int Visible { get; }

    }
}