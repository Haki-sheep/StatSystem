namespace GAS.Targeting
{
    /// <summary>
    /// 目标类型
    /// </summary>
    public enum TargetType
    {
        None,           // 无目标
        Self,           // 自身
        
        // 单体
        SingleEnemy,    // 单体敌人
        SingleAlly,     // 单体友军
        
        // 圆形范围
        AreaEnemy,      // 圆形范围敌人
        AreaAlly,       // 圆形范围友军
        AreaAll,        // 圆形范围所有
        
        // 扇形范围
        ConeEnemy,      // 扇形敌人
        ConeAlly,       // 扇形友军
        ConeAll,        // 扇形所有
        
        // 矩形范围
        BoxEnemy,       // 矩形敌人
        BoxAlly,        // 矩形友军
        BoxAll,         // 矩形所有
        
        // 特殊
        Cursor,         // 光标位置
    }
}