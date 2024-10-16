using System.Collections.Generic;
using UnityEngine;

public enum NPCCharacter
{
    CyrusTheMadScribe,
    SisterEliora,
    RolandTheLostExplorer,
    LilahTheBlindSeer,
    DerrickTheForsakenKnight,
    MaraTheWhisperer,
    AldenTheGravekeeper
}

[System.Serializable]
public struct NPCData
{
    public NPCCharacter npcCharacter;
    public NPC npc;
}

[CreateAssetMenu(fileName = "NPCList", menuName = "ScriptableObjects/NPCList", order = 1)]
public class NPCList : ScriptableObject
{
    public List<NPCData> npcList;

    public NPC GetNPC(NPCCharacter character)
    {
        foreach (NPCData data in npcList)
        {
            if (data.npcCharacter == character)
            {
                return data.npc;
            }
        }
        Debug.LogWarning("NPC not found for character: " + character);
        return null;
    }
}