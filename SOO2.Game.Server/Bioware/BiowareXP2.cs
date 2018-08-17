﻿using Freescape.Game.Server.Bioware.Contracts;
using Freescape.Game.Server.Enumeration;
using Freescape.Game.Server.GameObject;
using NWN;
using static NWN.NWScript;

namespace Freescape.Game.Server.Bioware
{
    public class BiowareXP2 : IBiowareXP2
    {
        private readonly INWScript _;

        public BiowareXP2(INWScript script)
        {
            _ = script;
        }

        // ----------------------------------------------------------------------------
        // Add an item property in a safe fashion, preventing unwanted stacking
        // Parameters:
        //   oItem     - the item to add the property to
        //   ip        - the itemproperty to add
        //   fDuration - set 0.0f to add the property permanent, anything else is temporary
        //   nAddItemPropertyPolicy - How to handle existing properties. Valid values are:
        //	     X2_IP_ADDPROP_POLICY_REPLACE_EXISTING - remove any property of the same type, subtype, durationtype before adding;
        //	     X2_IP_ADDPROP_POLICY_KEEP_EXISTING - do not add if any property with same type, subtype and durationtype already exists;
        //	     X2_IP_ADDPROP_POLICY_IGNORE_EXISTING - add itemproperty in any case - Do not Use with OnHit or OnHitSpellCast props!
        //   bIgnoreDurationType  - If set to TRUE, an item property will be considered identical even if the DurationType is different. Be careful when using this
        //	                          with X2_IP_ADDPROP_POLICY_REPLACE_EXISTING, as this could lead to a temporary item property removing a permanent one
        //   bIgnoreSubType       - If set to TRUE an item property will be considered identical even if the SubType is different.
        //
        // * WARNING: This function is used all over the game. Touch it and break it and the wrath
        //	            of the gods will come down on you faster than you can saz "I didn't do it"
        // ----------------------------------------------------------------------------
        public void IPSafeAddItemProperty(NWItem oItem, ItemProperty ip, float fDuration, AddItemPropertyPolicy nAddItemPropertyPolicy, bool bIgnoreDurationType, bool bIgnoreSubType)
        {
            int nType = _.GetItemPropertyType(ip);
            int nSubType = _.GetItemPropertySubType(ip);
            int nDuration;
            // if duration is 0.0f, make the item property permanent
            if (fDuration == 0.0f)
            {

                nDuration = DURATION_TYPE_PERMANENT;
            }
            else
            {

                nDuration = DURATION_TYPE_TEMPORARY;
            }

            int nDurationCompare = nDuration;
            if (bIgnoreDurationType)
            {
                nDurationCompare = -1;
            }

            if (nAddItemPropertyPolicy == AddItemPropertyPolicy.ReplaceExisting)
            {

                // remove any matching properties
                if (bIgnoreSubType)
                {
                    nSubType = -1;
                }
                IPRemoveMatchingItemProperties(oItem, nType, nDurationCompare, nSubType);
            }
            else if (nAddItemPropertyPolicy == AddItemPropertyPolicy.KeepExisting)
            {
                // do not replace existing properties
                if (IPGetItemHasProperty(oItem, ip, nDurationCompare, bIgnoreSubType))
                {
                    return; // item already has property, return
                }
            }
            else //X2_IP_ADDPROP_POLICY_IGNORE_EXISTING
            {

            }

            if (nDuration == DURATION_TYPE_PERMANENT)
            {
                _.AddItemProperty(nDuration, ip, oItem.Object);
            }
            else
            {
                _.AddItemProperty(nDuration, ip, oItem.Object, fDuration);
            }
        }



        // ----------------------------------------------------------------------------
        // Removes all itemproperties with matching nItemPropertyType and
        // nItemPropertyDuration (a DURATION_TYPE_* constant)
        // ----------------------------------------------------------------------------
        public void IPRemoveMatchingItemProperties(NWItem oItem, int nItemPropertyType, int nItemPropertyDuration, int nItemPropertySubType)
        {
            var props = oItem.ItemProperties;

            for (int current = 0; current < props.Count; current++)
            {
                ItemProperty prop = props[current];

                // same property type?
                if (_.GetItemPropertyType(prop) == nItemPropertyType)
                {
                    // same duration or duration ignored?
                    if (_.GetItemPropertyDurationType(prop) == nItemPropertyDuration || nItemPropertyDuration == -1)
                    {
                        // same subtype or subtype ignored
                        if (_.GetItemPropertySubType(prop) == nItemPropertySubType || nItemPropertySubType == -1)
                        {
                            // Put a warning into the logfile if someone tries to remove a permanent ip with a temporary one!
                            /*if (nItemPropertyDuration == DURATION_TYPE_TEMPORARY &&  GetItemPropertyDurationType(ip) == DURATION_TYPE_PERMANENT)
                            {
                               WriteTimestampedLogEntry("x2_inc_itemprop:: IPRemoveMatchingItemProperties() - WARNING: Permanent item property removed by temporary on "+GetTag(oItem));
                            }
                            */
                            _.RemoveItemProperty(oItem.Object, prop);
                        }
                    }
                }

            }
        }


        public bool IPGetItemHasProperty(NWItem oItem, ItemProperty ipCompareTo, int nDurationCompare, bool bIgnoreSubType)
        {
            var props = oItem.ItemProperties;

            foreach (ItemProperty ip in props)
            {
                if ((_.GetItemPropertyType(ip) == _.GetItemPropertyType(ipCompareTo)))
                {
                    //PrintString ("**Testing - S: " + IntToString(GetItemPropertySubType(ip)));
                    if (_.GetItemPropertySubType(ip) == _.GetItemPropertySubType(ipCompareTo) || bIgnoreSubType)
                    {
                        // PrintString ("***Testing - d: " + IntToString(GetItemPropertyDurationType(ip)));
                        if (_.GetItemPropertyDurationType(ip) == nDurationCompare || nDurationCompare == -1)
                        {
                            //PrintString ("***FOUND");
                            return true; // if duration is not ignored and durationtypes are equal, true
                        }
                    }
                }
            }

            return false;
        }

        public void IPRemoveAllItemProperties(NWItem oItem, int nItemPropertyDuration)
        {
            var props = oItem.ItemProperties;
            for (int current = 0; current < props.Count; current++)
            {
                ItemProperty prop = props[current];

                _.GetItemPropertyDurationType(prop);
                if (_.GetItemPropertyDurationType(prop) == nItemPropertyDuration)
                {
                    _.RemoveItemProperty(oItem.Object, prop);
                }
            }
        }

    }
}
