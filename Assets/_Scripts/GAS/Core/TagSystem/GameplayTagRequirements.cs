namespace GAS.TagSystem
{
    /// <summary>
    /// 标签需求 用于进行条件判断
    /// </summary>
    public class GameplayTagRequirements
    {
        public GameplayTagContainer NeedTags;//需要的标签(合集)
        public GameplayTagContainer BanTags; //禁止的标签(合集)

        public GameplayTagRequirements()
        {
            NeedTags = new GameplayTagContainer();
            BanTags = new GameplayTagContainer();
        }

        /// <summary>
        /// 是否满足条件 包含所有需要的标签 不包含禁止的标签
        /// </summary>
        /// <param name="tags">要检查的标签容器</param>
        /// <returns>是否满足条件</returns>
        public bool IsSatisfied(GameplayTagContainer tags)
        {
            if (!tags.ContainsAll(NeedTags.Tags)) return false;
                
            if (tags.ContainsAny(BanTags.Tags)) return false;
                
            return true;
        }
    }
}