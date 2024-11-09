using UnityEngine;
using System.Collections.Generic;

public class PFXManager : MonoBehaviour
{
    public static PFXManager Instance;

    [SerializeField] private PFXDatabase pfxDatabase;
    private Dictionary<PFXDatabase.PFXType, Queue<ParticleSystem>> pfxPools;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePools()
    {
        pfxPools = new Dictionary<PFXDatabase.PFXType, Queue<ParticleSystem>>();
        foreach (var entry in pfxDatabase.effects)
        {
            Queue<ParticleSystem> pool = new Queue<ParticleSystem>();

            for (int i = 0; i < entry.poolSize; i++)
            {
                ParticleSystem instance = Instantiate(entry.prefab, transform);
                instance.gameObject.SetActive(false);
                pool.Enqueue(instance);
            }

            pfxPools[entry.effectType] = pool;
        }
    }

    public void PlayPFX(PFXDatabase.PFXType effectType, Vector3 position)
    {
        ParticleSystem pfx = GetPFX(effectType);
        if (pfx != null)
        {
            pfx.transform.position = position;
            pfx.gameObject.SetActive(true);
            pfx.Play();

            StartCoroutine(ReturnPFXToPool(pfx, effectType, pfx.main.duration));
        }
        else
        {
            Debug.LogWarning($"PFX with name {effectType} not found in the pool!");
        }
    }

    private System.Collections.IEnumerator ReturnPFXToPool(ParticleSystem pfx, PFXDatabase.PFXType effectType, float delay)
    {
        yield return new WaitForSeconds(delay);
        pfx.gameObject.SetActive(false);
        pfxPools[effectType].Enqueue(pfx);
    }

    public ParticleSystem GetPFX(PFXDatabase.PFXType effectType)
    {
        if (pfxPools.ContainsKey(effectType) && pfxPools[effectType].Count > 0)
        {
            ParticleSystem pfx = pfxPools[effectType].Dequeue();
            pfx.gameObject.SetActive(true);
            return pfx;
        }
        else
        {
            Debug.LogWarning($"No available instances of PFX with name {effectType}.");
            return null;
        }
    }

    public void ReturnPFX(PFXDatabase.PFXType effectType, ParticleSystem pfx)
    {
        if (pfxPools.ContainsKey(effectType))
        {
            pfx.gameObject.SetActive(false);
            pfxPools[effectType].Enqueue(pfx);
        }
        else
        {
            Debug.LogWarning($"PFX pool with name {effectType} does not exist.");
        }
    }
}
