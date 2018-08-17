using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using Freescape.Game.Server.NWNX;
using Freescape.Game.Server.NWNX.Contracts;
using Freescape.Game.Server.Service.Contracts;
using NWN;

namespace Freescape.Game.Server.Service
{
    public class LocalVariableService: ILocalVariableService
    {
        private readonly INWScript _;
        private readonly INWNXObject _nwnxObject;

        public LocalVariableService(
            INWScript script,
            INWNXObject nwnxObject)
        {
            _ = script;
            _nwnxObject = nwnxObject;
        }

        public void CopyVariables(NWObject oSource, NWObject oCopy)
        {
            int variableCount = _nwnxObject.GetLocalVariableCount(oSource);
            for (int variableIndex = 0; variableIndex < variableCount - 1; variableIndex++)
            {
                LocalVariable stCurVar = _nwnxObject.GetLocalVariable(oSource, variableIndex);

                switch (stCurVar.Type)
                {
                    case LocalVariableType.Int:
                        oCopy.SetLocalInt(stCurVar.Key, oSource.GetLocalInt(stCurVar.Key));
                        break;
                    case LocalVariableType.Float:
                        oCopy.SetLocalFloat(stCurVar.Key, oSource.GetLocalFloat(stCurVar.Key));
                        break;
                    case LocalVariableType.String:
                        oCopy.SetLocalString(stCurVar.Key, oSource.GetLocalString(stCurVar.Key));
                        break;
                    case LocalVariableType.Object:
                        oCopy.SetLocalObject(stCurVar.Key, oSource.GetLocalObject(stCurVar.Key));
                        break;
                    case LocalVariableType.Location:
                        oCopy.SetLocalLocation(stCurVar.Key, oSource.GetLocalLocation(stCurVar.Key));
                        break;
                }
            }
        }
    }
}
