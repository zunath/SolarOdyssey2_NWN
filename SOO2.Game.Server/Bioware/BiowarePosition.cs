using System;
using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.GameObject;
using NWN;

namespace Freescape.Game.Server.Bioware
{
    public class BiowarePosition : IBiowarePosition
    {
        private readonly INWScript _;

        public BiowarePosition(INWScript script)
        {
            _ = script;
        }

        public void TurnToFaceObject(NWObject oObjectToFace, NWObject oTarget)
        {
            oTarget.AssignCommand(() =>
            {
                _.SetFacingPoint(oObjectToFace.Position);
            });
        }

        public float GetChangeInX(float fDistance, float fAngle)
        {
            return (float)(fDistance * Math.Cos(fAngle));
        }

        public float GetChangeInY(float fDistance, float fAngle)
        {
            return (float)(fDistance * Math.Sin(fAngle));
        }

        public Vector GetChangedPosition(Vector vOriginal, float fDistance, float fAngle)
        {
            float changedZ = vOriginal.m_Z;

            var changedX = vOriginal.m_X + GetChangeInX(fDistance, fAngle);
            if (changedX < 0.0)
                changedX = -changedX;
            var changedY = vOriginal.m_Y + GetChangeInY(fDistance, fAngle);
            if (changedY < 0.0)
                changedY = -changedY;

            return _.Vector(changedX, changedY, changedZ);
        }

    }
}
