namespace SOO2.Game.Server.NWNX.Contracts
{
    public interface INWNXDamage
    {
        DamageData GetDamageEventData();
        void SetDamageEventData(DamageData data);
        void SetDamageEventScript(string script);
    }
}