using UnityEngine;
using UnityEngine.Serialization;

public class PlatformBlock : Damageable
{
    [FormerlySerializedAs("gridIndex")] [HideInInspector]
    public Vector3Int tilePosition;

    [HideInInspector]
    public bool isDestructible = true;

    public override void TakeDamage(int damageAmount)
    {
        if (isDestructible)
        {
            PFXManager.Instance.PlayPFX(PFXDatabase.PFXType.PlatformBlockBreak, transform.position);
            TilemapManager.Instance.RemovePlatformileFrom(tilePosition);
            Destroy(this.gameObject);
        }
    }
}
