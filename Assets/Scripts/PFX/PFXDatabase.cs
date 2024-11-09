using UnityEngine;

[CreateAssetMenu(fileName = "PFXDatabase", menuName = "ScriptableObjects/PFXDatabase", order = 1)]
public class PFXDatabase : ScriptableObject
{
    public enum PFXType{PlatformBlockBreak}
    
    [System.Serializable]
    public class PFXEntry
    {
        public PFXType effectType;
        public ParticleSystem prefab;
        public int poolSize = 10;
    }

    public PFXEntry[] effects;
}