using UnityEngine;

#nullable disable
[CreateAssetMenu(fileName = "NewCropData", menuName = "Item/Crop Data")]
public class CropData : ItemDataSO
{
  [Header("作物基础信息，只读不改，全体共享")]
  public string cropName;
  public int growthDate;
  public Sprite cropIcon;
  public GameObject cropPrefab;
  public int nutrientNeed;

  public enum GrowthStage
  {
    seed,
    germination,
    ripeness,
  }
}