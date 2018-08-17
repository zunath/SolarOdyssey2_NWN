﻿using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;

namespace Freescape.Game.Server.NWNX.Contracts
{
    public interface INWNXPlayerQuickBarSlot
    {
        QuickBarSlot Empty(QuickBarSlotType type);
        QuickBarSlot UseItem(NWItem oItem, int nPropertyID);
        QuickBarSlot EquipItem(NWItem oItem, NWItem oSecondaryItem);
        QuickBarSlot CastSpell(int nSpell, int nClassIndex, int nMetamagic, int nDomainLevel);
        QuickBarSlot UseSkill(int nSkill);
        QuickBarSlot UseFeat(int nFeat);
        QuickBarSlot StartDialog();
        QuickBarSlot Attack();
        QuickBarSlot Emote(int nEmote);
        QuickBarSlot ToggleMode(int nMode);
        QuickBarSlot Examine();
        QuickBarSlot Barter();
        QuickBarSlot QuickChat(int nCommand);
        QuickBarSlot PossessFamiliar();
        QuickBarSlot UseSpecialAbility(int nSpell, int nCasterLevel);
    }
}