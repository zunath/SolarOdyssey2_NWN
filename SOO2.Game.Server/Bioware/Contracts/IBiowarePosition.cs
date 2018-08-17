﻿using SOO2.Game.Server.GameObject;
using NWN;

namespace SOO2.Game.Server.Bioware.Contracts
{
    public interface IBiowarePosition
    {
        void TurnToFaceObject(NWObject oObjectToFace, NWObject oTarget);
        float GetChangeInX(float fDistance, float fAngle);
        float GetChangeInY(float fDistance, float fAngle);
        Vector GetChangedPosition(Vector vOriginal, float fDistance, float fAngle);
    }
}