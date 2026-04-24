using UnityEngine;
using Unity.Netcode;
using System;

[Serializable]
public struct ItemStack:INetworkSerializable,IEquatable<ItemStack>
{
    public ushort ItemID;
    public int Amount;
    public bool IsEmpty => ItemID == 0 || Amount <= 0;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ItemID);
        serializer.SerializeValue(ref Amount);
    }
    public bool Equals(ItemStack other)
    {
        return ItemID == other.ItemID &&   Amount == other.Amount;
    }
    public override string ToString()
    {
        return IsEmpty ? "Empty Slot" : $"[ItemID:{ItemID},Amount:{Amount}]";
    }
}
