namespace Stirge.AI
{
    [System.Serializable]
    public class AirJuggleCondition : Condition
    {        
        protected override bool _IsTrue(Agent agent)
        {
            return agent.ContainsMemory("AirStallLength");
        }
    }
}
