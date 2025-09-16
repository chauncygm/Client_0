namespace GameConfig
{
    public interface CfgDefine {

        protected void InitLoad(string tableName);

        public void Init()
        {
            // ID:20 字段数:12 有效数据行数:8 说明:任务
            InitLoad(CfgTask.TableName);
            // ID:21 字段数:5 有效数据行数:6 说明:条件
            InitLoad(CfgCondition.TableName);
            // ID:11 字段数:8 有效数据行数:18 说明:背包
            InitLoad(CfgBag.TableName);
            // ID:12 字段数:8 有效数据行数:18 说明:道具
            InitLoad(CfgItem.TableName);
        }


        public int ReloadCfg(string tableName, string data)
        {
            return tableName switch
            {
                // ID:20 字段数:12 有效数据行数:8 说明:任务
                CfgTask.TableName => CfgTask.Reload(data),
                // ID:21 字段数:5 有效数据行数:6 说明:条件
                CfgCondition.TableName => CfgCondition.Reload(data),
                // ID:11 字段数:8 有效数据行数:18 说明:背包
                CfgBag.TableName => CfgBag.Reload(data),
                // ID:12 字段数:8 有效数据行数:18 说明:道具
                CfgItem.TableName => CfgItem.Reload(data),
                _ => -1
            };
        }
    }
}