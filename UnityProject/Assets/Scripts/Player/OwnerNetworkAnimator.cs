using Unity.Netcode.Components;
using UnityEngine;

// 这个组件替代官方的 NetworkAnimator，允许客机同步自己的动画
public class OwnerNetworkAnimator : NetworkAnimator
{
    // 重写权限检查：只要我是这个物体的拥有者（Owner），我就有权同步动画！
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}