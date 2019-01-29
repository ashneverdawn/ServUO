using System;
using Server.Network;
using Server.Items;
using Server.Misc;

namespace Server.Gumps
{
    public class CraftsmansGump : Gump
    {
        private readonly SkillName[] itemSkills = {
                SkillName.Alchemy,
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
        private BaseClothing m_Item;
        public CraftsmansGump(Mobile from, BaseClothing item)
            : base(100, 100)
        {
            this.m_From = from;
            this.m_Item = item;

            from.CloseGump(typeof(CraftsmansGump));

            this.AddPage(0);
            this.AddBackground(0, 0, 450, 500, 0xA3C);

            int experience = GetExperience(from);

            int[] cols = { 20, 50, 240, 270, 300, 330 };

            this.AddLabel(cols[0], 10, 965, "Enhance your Item");
            this.AddLabel(cols[0], 35, 965, "Experience Scrolls Available: " + experience);

            int gumpY = 70;
            this.AddLabel(cols[2], gumpY, 965, "Skill");
            this.AddLabel(cols[3] + 12, gumpY, 965, "10");
            this.AddLabel(cols[4], gumpY, 965, "Max");

            gumpY = 90;
            int gumpOffsetY = 20;
            int i = 0;
            foreach(SkillName skillName in itemSkills)
            {
                AddRadio(cols[0], gumpY + i * gumpOffsetY, 9727, 9730, false, i+1);
                this.AddLabel(cols[1] , gumpY + i * gumpOffsetY, 965, itemSkills[i].ToString());
                i++;
            }
            for(i = 0; i < 5; i++)
            {
                this.AddButton(cols[2], gumpY + i * gumpOffsetY, 4014, 4016, i + 1, GumpButtonType.Reply, 1);
                this.AddButton(cols[3], gumpY + i * gumpOffsetY, 4014, 4016, i + 6, GumpButtonType.Reply, 1);
                this.AddButton(cols[4], gumpY + i * gumpOffsetY, 4014, 4016, i + 11, GumpButtonType.Reply, 1);
                SkillName sn;
                double value;
                item.SkillBonuses.GetValues(i, out sn, out value);
                this.AddLabel(cols[5], gumpY + i * gumpOffsetY, 965, (value > 0) ? sn.ToString() + "(+" + value + ")" : "Skill " + (i+1));
            }

            gumpY = 240;
            this.AddButton(cols[2], gumpY, 4014, 4016, 16, GumpButtonType.Reply, 1);
            this.AddLabel(cols[3], gumpY, 965, "Convert to Fancy Shirt");

            this.AddButton(cols[2], gumpY + 20, 4014, 4016, 17, GumpButtonType.Reply, 1);
            this.AddLabel(cols[3], gumpY + 20, 965, "Convert to Apron");

            this.AddButton(cols[2], gumpY + 40, 4014, 4016, 18, GumpButtonType.Reply, 1);
            this.AddLabel(cols[3], gumpY + 40, 965, "Convert to Talisman");

            this.AddButton(cols[2], gumpY + 60, 4014, 4016, 19, GumpButtonType.Reply, 1);
            this.AddLabel(cols[3], gumpY + 60, 965, "Convert to Earrings");

            this.AddButton(cols[2], gumpY + 80, 4014, 4016, 20, GumpButtonType.Reply, 1);
            this.AddLabel(cols[3], gumpY + 80, 965, "Convert to Sandals");
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
            bool skillChange = false;
            switch (id)
            {
                case 0:
                    return;
                case 1: case 2: case 3: case 4: case 5:
                    if(radioId > 0)
                    {
                        SkillName sn;
                        double value;
                        m_Item.SkillBonuses.GetValues(id - 1, out sn, out value);
                        m_Item.SkillBonuses.SetValues(id - 1, itemSkills[radioId - 1], (value == 0) ? 1.0 : value);
                        skillChange = true;

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
                        m_Item.SkillBonuses.GetValues(id - 1, out sn, out value);
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
                        m_Item.SkillBonuses.SetValues(id - 1, sn, newValue);
                    }
                    break;
                case 16:
                    if (m_Item.Layer != Layer.Shirt)
                        SwitchItemTo(new CraftsmansFancyShirt());
                    break;
                case 17:
                    if(m_Item.Layer != Layer.MiddleTorso)
                        SwitchItemTo(new CraftsmansApron());
                    break;
                case 18:
                    if (m_Item.Layer != Layer.Talisman)
                        SwitchItemTo(new CraftsmansTalisman());
                    break;
                case 19:
                    if (m_Item.Layer != Layer.Earrings)
                        SwitchItemTo(new CraftsmansEarrings());
                    break;
                case 20:
                    if (m_Item.Layer != Layer.Shoes)
                        SwitchItemTo(new CraftsmansSandals());
                    break;
            }

            if (skillChange || GetExperience(from) > 0)
            {
                from.SendGump(new CraftsmansGump(from, m_Item));
            }

        }
        void SwitchItemTo(BaseClothing newItem)
        {
            newItem.SkillBonuses = m_Item.SkillBonuses;
            m_Item.Delete();
            m_From.Backpack.AddItem(newItem);
            m_Item = newItem;
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
