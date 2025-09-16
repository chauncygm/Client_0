using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameConfig
{
    ///<summary>
    /// 说明: 道具表 ID:12 字段数:8 有效数据行数:18
    /// Created on 2025-09-16 11:34
    ///</summary>
    public class CfgItem : ConfigBase<CfgItem>
    {

        public const string TableName = "item";

        [JsonConstructor]
        private CfgItem([JsonProperty(nameof(Id))] int id,
        [JsonProperty(nameof(Type))] int type,
        [JsonProperty(nameof(Desc))] string desc,
        [JsonProperty(nameof(BagType))] int bagType,
        [JsonProperty(nameof(Quality))] int quality,
        [JsonProperty(nameof(Bind))] int bind,
        [JsonProperty(nameof(MaxStack))] int maxStack,
        [JsonProperty(nameof(Decompose))] IList<Int2IntVal> decompose)
        {
            Id = id;
            Type = type;
            Desc = desc;
            BagType = bagType;
            Quality = quality;
            Bind = bind;
            MaxStack = maxStack;
            Decompose = decompose;
        }

        /// <summary>
        /// 道具类型
        /// </summary>
        private int Type { get; }
        /// <summary>
        /// 道具描述
        /// </summary>
        private string Desc { get; }
        /// <summary>
        /// 所属背包
        /// </summary>
        private int BagType { get; }
        /// <summary>
        /// 道具品质
        /// </summary>
        private int Quality { get; }
        /// <summary>
        /// 是否绑定
        /// </summary>
        private int Bind { get; }
        /// <summary>
        /// 最大堆叠数量
        /// </summary>
        private int MaxStack { get; }
        /// <summary>
        /// 分解获得
        /// </summary>
        [JsonConverter(typeof(ImmutableListConverter))]
        private IList<Int2IntVal> Decompose { get; }

    }
}