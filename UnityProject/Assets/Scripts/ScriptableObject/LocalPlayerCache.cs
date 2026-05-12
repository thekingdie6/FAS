public static class LocalPlayerCache
{
    public static string PlayerName = "像素流浪者";

    // 【模块化设计】提前为纸娃娃系统预留部位 ID
    // 即使我们现在只测试“整体替换”，也统一使用 BodyID 或 HeadID 代表当前形象
    public static int HeadID = 0;   // 头部/发型
    public static int BodyID = 0;   // 身体/衣服 (前期测试我们就用这个代表整体形象)
    public static int WeaponID = 0; // 武器 (预留)
}