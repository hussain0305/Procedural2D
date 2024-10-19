using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PurchasableItem : MonoBehaviour
{
    public PurchasableItemInfo itemInfo;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PopupManager.Instance.ShowPurchasableItemPopup(transform.position + new Vector3(0, 1, 0), itemInfo);
    }

    private void OnTriggerExit(Collider other)
    {
        PopupManager.Instance.HideCurrentPopup();
    }

    public virtual void WasPurchased()
    {
        
    }

    public void PostPurchase()
    {
        Destroy(this.gameObject);
    }
}
