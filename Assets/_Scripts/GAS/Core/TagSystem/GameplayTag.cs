using System;

namespace GAS.TagSystem
{
    //标签系统 以String表示标签 在后续GAS之中进行条件判断 
    [System.Serializable]
    public struct GameplayTag : IEquatable<GameplayTag>
    {
        public readonly string TagName; //标签名字

        // 构造函数 赋值Tag
        public GameplayTag(string tagName)
        {
            if (string.IsNullOrEmpty(tagName))
                throw new ArgumentException("Tag name cannot be null!", nameof(tagName));
            TagName = tagName;
        }

        //以"."分割标签层级 如果要求 Poison，你有 Ailment.Poison → 满足 
        // 如果要求 Ailment，你有 Ailment.Poison → 满足 
        public string[] TagNameSplit => TagName.Split('.');
        public int Depth => TagNameSplit.Length;//层级深度

        public bool Equals(GameplayTag other) =>TagName == other.TagName;

        public override bool Equals(object obj) =>obj is GameplayTag tag && Equals(tag);

        public override int GetHashCode() =>TagName.GetHashCode();

        public override string ToString() =>TagName;


        /// <summary>
        /// 检查此标签是否是目标标签的父标签
        /// 比如 Ailment (this) 是 Ailment.Poison (other)的父标签
        /// </summary>
        public bool IsParentOf(GameplayTag otherTag)
        {
            var thisSplitArray = TagNameSplit;
            var otherSplitArray = otherTag.TagNameSplit;

            //如果此标签的层级深度大于等于目标标签的层级深度，则返回false 
            //this = 2 other=1 那肯定是other才是父信息
            if (thisSplitArray.Length >= otherSplitArray.Length) return false;

            for (int i = 0; i < thisSplitArray.Length; i++)
            {
                if (thisSplitArray[i] != otherSplitArray[i]) return false;
            }
            return true;
        }

        //私有构造函数绕过检查普通空字符串的检查 因为公共构造函数不允许设置为空字符串
        private GameplayTag(string tagName, bool _)
        {
            TagName = tagName;
        }
        //空标签
        public static readonly GameplayTag EmptyTag = new GameplayTag("", true);
    }
}