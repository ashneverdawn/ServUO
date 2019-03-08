using System;
using Server.Network;
using Server.Items;
using System.Collections.Generic;

namespace Server.Gumps
{
    class AltarOfPowerGump : Gump
    {
        private readonly Mobile m_From;
        private readonly PowerScroll m_ps;

        public AltarOfPowerGump(Mobile from, PowerScroll ps)
            : base(100, 100)
        {
            this.m_From = from;
            this.m_ps = ps;

            from.CloseGump(typeof(AltarOfPowerGump));

            this.AddPage(0);
            this.AddBackground(0, 0, 650, 400, 0xA3C);

            int[] cols = { 20, 50, 80, 235, 265, 295, 450, 480, 510 };

            this.AddLabel(cols[0], 10, 965, "Merge your 2 identical PowerScrolls and convert into this skill:");

            int gumpY = 70;
            int gumpOffsetY = 20;
            int rows = 12;
            int i = 0;
            foreach (SkillName skill in PowerScroll.Skills)
            {
                if(skill != SkillName.Blacksmith && skill != SkillName.Tailoring && skill != SkillName.Imbuing)
                {
                    this.AddButton(cols[1 + i / rows * 3], gumpY + (i - i / rows * rows) * gumpOffsetY, 4014, 4016, i + 1, GumpButtonType.Reply, 1);
                    this.AddLabel(cols[2 + i / rows * 3], gumpY + (i - i / rows * rows) * gumpOffsetY, 965, skill.ToString());
                }
                i++;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if(info.ButtonID == 0)
                return;

            Mobile from = sender.Mobile;

            List<PowerScroll> psList = new List<PowerScroll>();
            foreach (PowerScroll item in from.Backpack.FindItemsByType<PowerScroll>())
            {
                if (item.Skill == m_ps.Skill && item.Value == m_ps.Value)
                {
                    psList.Add(item);
                    if(psList.Count == 2)
                    {
                        break;
                    }
                }
            }

            if (psList.Count >= 2)
            {
                psList[1].Delete();
                psList[0].Delete();
                PowerScroll powerScroll = new PowerScroll(GetSkillByIndex(from, info.ButtonID - 1), m_ps.Value);
                from.AddToBackpack(powerScroll);
            }
            else
            {
                from.SendMessage("You don't have the required power scrolls in your backpack.");
            }

        }
        SkillName GetSkillByIndex(Mobile from, int index)
        {
            int i = 0;
            foreach (SkillName skillName in PowerScroll.Skills)
            {
                if (i == index)
                    return skillName;
                i++;
            }

            return 0;
        }
    }
}
