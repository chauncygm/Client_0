using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameConfig
{
    ///<summary>
    /// 说明: 条件表 ID:21 字段数:5 有效数据行数:6
    /// Created on 2025-09-16 11:34
    ///</summary>
    public class CfgCondition : ConfigBase<CfgCondition>
    {

        public const string TableName = "condition";

        [JsonConstructor]
        private CfgCondition([JsonProperty(nameof(Id))] int id,
        [JsonProperty(nameof(ConditionType))] int conditionType,
        [JsonProperty(nameof(SourceType))] int sourceType,
        [JsonProperty(nameof(ConditionParams))] IList<Int2IntVal> conditionParams,
        [JsonProperty(nameof(Target))] int target)
        {
            Id = id;
            ConditionType = conditionType;
            SourceType = sourceType;
            ConditionParams = conditionParams;
            Target = target;
        }

        /// <summary>
        /// 条件类型
        /// </summary>
        private int ConditionType { get; }
        /// <summary>
        /// 任务数据类型
        /// </summary>
        private int SourceType { get; }
        /// <summary>
        /// 任务目标参数
        /// </summary>
        [JsonConverter(typeof(ImmutableListConverter))]
        private IList<Int2IntVal> ConditionParams { get; }
        /// <summary>
        /// 目标值
        /// </summary>
        private int Target { get; }

    }
}