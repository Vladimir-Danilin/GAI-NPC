using UnityEngine;

public class HierarcyAI : NPCAI
{
    
    public AgentsList[] agents;


}


[System.Serializable]
public class AgentsList
{
    bool predator;
    public GameObject[] agentList;

}
