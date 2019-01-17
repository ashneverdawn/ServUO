using System;
using Server.Network;
using Server.Items;
using Server.Misc;

namespace Server.Gumps
{
    public class CraftsmansApronGump : Gump
    {
        private readonly SkillName[] apronSkills = {
                SkillName.Alchemy
                SkillName.ArmsLore,
                SkillName.Begging,
                SkillName.Blacksmith,
                SkillName.Fletching,
                SkillName.Carpentry,
                SkillName.Cartography,
                SkillName.Cooking,
                SkillName.Fishing,
                SkillName.Forensics,
                SkillName.Imbuing,
                SkillName.Inscribe,
                SkillName.Lockpicking,
                SkillName.Lumberjacking,
                SkillName.Mining,
                SkillName.Tailoring,
                SkillName.TasteID,
                SkillName.Tinkering};

        private readonly Mobile m_From;
        private readonly CraftsmansApron m_Apron;
        public CraftsmansApronGump(Mobile from, CraftsmansApron apron)
            : base(100, 100)
        {
            this.m_From = from;
            this.m_Apron = apron;

            from.CloseGump(typeof(CraftsmansApronGump));

            this.AddPage(0);
            this.AddBackground(0, 0, 450, 500, 0xA3C);

            int experience = GetExperience(from);

            int[] cols = { 20, 50, 240, 270, 300, 330 };

            this.AddLabel(cols[0], 10, 965, "Enhance your Apron");
            this.AddLabel(cols[0], 35, 965, "Experience Scrolls Available: " + experience);

            int gumpY = 70;
            this.AddLabel(cols[2], gumpY, 965, "Skill");
            this.AddLabel(cols[3] + 12, gumpY, 965, "10");
            this.AddLabel(cols[4], gumpY, 965, "Max");

            gumpY = 90;
            int gumpOffsetY = 20;
            int i = 0;
            foreach(SkillName skillName in apronSkills)
            {
                AddRadio(cols[0], gumpY + i * gumpOffsetY, 9727, 9730, false, i+1);
                this.AddLabel(cols[1] , gumpY + i * gumpOffsetY, 965, apronSkills[i].ToString());
                i++;
            }
            for(i = 0; i < 5; i++)
            {
                this.AddButton(cols[2], gumpY + i * gumpOffsetY, 4014, 4016, i + 1, GumpButtonType.Reply, 1);
                this.AddButton(cols[3], gumpY + i * gumpOffsetY, 4014, 4016, i + 6, GumpButtonType.Reply, 1);
                this.AddButton(cols[4], gumpY + i * gumpOffsetY, 4014, 4016, i + 11, GumpButtonType.Reply, 1);
                SkillName sn;
                double value;
                apron.SkillBonuses.GetValues(i, out sn, out value);
                this.AddLabel(cols[5], gumpY + i * gumpOffsetY, 965, (value > 0) ? sn.ToString() + "(+" + value + ")" : "Skill " + (i+1));
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int id = info.ButtonID;
            int radioId = 0;
            foreach(int s in info.Switches)
            {
                if (info.IsSwitched(s))
                {
                    radioId = s;
                    break;
                }

            }

            switch (id)
            {
                case 0:
                    return;
                case 1: case 2: case 3: case 4: case 5:
                    if(radioId > 0)
                    {
                        SkillName sn;
                        double value;
                        m_Apron.SkillBonuses.GetValues(id - 1, out sn, out value);
                        m_Apron.SkillBonuses.SetValues(id - 1, apronSkills[radioId - 1], (value == 0) ? 1.0 : value);

                    }
                    break;
                case 6: case 7: case 8: case 9: case 10: case 11: case 12: case 13: case 14: case 15:
                    int scrollsToUse = 10;
                    id -= 5;
                    if (id > 5)
                    {
                        id -= 5;
                        scrollsToUse = GetExperience(from);
                    }
                    if (scrollsToUse < 10)
                    {
                        from.SendMessage("You do not have enough experience scrolls in your backpack.");
                        return;
                    }

                    {
                        SkillName sn;
                        double value;
                        m_Apron.SkillBonuses.GetValues(id - 1, out sn, out value);
                        int maxGain = 1200 - (int)(value * 10 + 0.5);
                        if (maxGain < scrollsToUse)
                            scrollsToUse = maxGain;

                        if (from.Backpack.ConsumeTotal(new Type[] { typeof(ExperienceScroll) }, new int[] { scrollsToUse }) != -1)
                        {
                            from.SendMessage("You do not have enough experience scrolls in your backpack.");
                            return;
                        }
                        double newValue = value + (scrollsToUse / 10);
                        if (newValue > 119.0)
                            newValue = 120.0;
                        newValue = (int)(newValue + 0.5);
                        m_Apron.SkillBonuses.SetValues(id - 1, sn, newValue);
                    }
                    break;
            }

            if (GetExperience(from) > 0)
            {
                from.SendGump(new CraftsmansApronGump(from, m_Apron));
            }

        }

        int GetExperience(Mobile from)
        {
            int experiencePoints = 0;
            foreach (Item item in from.Backpack.FindItemsByType<ExperienceScroll>())
            {
                experiencePoints += item.Amount;
            }
            return experiencePoints;
        }
    }
}
