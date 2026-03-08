using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GAS.TagSystem
{
    /// <summary>
    /// 自定义标签容器 用于存储和管理GameplayTag
    /// 精确查找增删查改等等
    /// </summary>
    public class GameplayTagContainer
    {
        //标签列表
        private readonly HashSet<GameplayTag> mTagHashSet = new HashSet<GameplayTag>();

        private ReadOnlyCollection<GameplayTag>? cachedTags;
        //只读
        public ReadOnlyCollection<GameplayTag> Tags
        {
            get
            {
                // 仅当缓存失效时重新创建
                cachedTags ??= new List<GameplayTag>(mTagHashSet).AsReadOnly();
                return cachedTags;
            }
        }
        public int Count => mTagHashSet.Count;//当前持有标签数量
        public bool IsEmpty => mTagHashSet.Count == 0;//是否为空

        //添加标签
        public void AddTag(GameplayTag tag)
        {
            //如果标签不为空 且 添加成功，则清空缓存
            if (!tag.Equals(GameplayTag.EmptyTag))
            {
                if (mTagHashSet.Add(tag))
                {
                    cachedTags = null;
                }
            }
        }

        //批量加
        public void AddTags(params GameplayTag[] tags)
        {
            bool changed = false;
            foreach (var tag in tags)
            {
                if (!tag.Equals(GameplayTag.EmptyTag) && mTagHashSet.Add(tag))
                    changed = true;
            }
            if (changed) cachedTags = null;
        }
        //移除标签
        public void RemoveTag(GameplayTag tag)
        {
            if (mTagHashSet.Remove(tag))
            {
                cachedTags = null;
            }
        }

        //清空
        public void Clear()
        {
            mTagHashSet.Clear();
            cachedTags = null;
        }

        //是有持有该标签
        public bool Contains(GameplayTag tag)
        {
            return mTagHashSet.Contains(tag);
        }

        /// <summary>
        /// 查询包含匹配 支持精确匹配或父子匹配
        /// </summary>
        /// <param name="tag">查询的标签</param>
        /// <returns>是否包含匹配</returns>
        public bool ContainsTag(GameplayTag tag)
        {
            foreach (var existingTag in mTagHashSet)
            {
                // 检查查询标签是否是已存在标签的父标签（即容器中的标签是否匹配查询标签或查询标签的子标签）
                if (existingTag.Equals(tag) || tag.IsParentOf(existingTag))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 外界传递的标签合集 本容器是否任意包含一个
        /// </summary>
        /// <param name="newTags"></param>
        /// <returns></returns>
        public bool ContainsAny(IEnumerable<GameplayTag> newTags)
        {
            foreach (var tag in newTags)
            {
                if (this.ContainsTag(tag)) return true;
            }
            return false;
        }

        /// <summary>
        /// 外界传递的标签合集 本容器是否全部包含
        /// </summary>
        /// <param name="newTags">外界传递的标签合集</param>
        /// <returns>是否全部包含</returns>
        /// <returns></returns>
        public bool ContainsAll(IEnumerable<GameplayTag> newTags)
        {
            foreach (var tag in newTags)
            {
                if (!this.ContainsTag(tag)) return false;
            }
            return true;
        }

    }
}