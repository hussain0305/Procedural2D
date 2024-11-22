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
            Vector3 relativePosition = transform.position - GameManager.Instance.player.transform.position;
            bool isHorizontal = Mathf.Abs(relativePosition.x) > Mathf.Abs(relativePosition.y);
            PFXDatabase.PFXType pfxToPlay;

            if (isHorizontal)
            {
                pfxToPlay = relativePosition.x > 0 
                    ? PFXDatabase.PFXType.blockBreakFromLeft 
                    : PFXDatabase.PFXType.blockBreakFromRight;
            }
            else
            {
                pfxToPlay = relativePosition.y > 0 
                    ? PFXDatabase.PFXType.blockBreakFromBelow 
                    : PFXDatabase.PFXType.blockBreakFromAbove;
            }

            PFXManager.Instance.PlayPFX(pfxToPlay, transform.position);
            TilemapManager.Instance.RemovePlatformileFrom(tilePosition);
            Destroy(this.gameObject);
        }
    }
}
