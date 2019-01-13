using System;
using Server.Network;
using Server.Items;
using Server.Misc;

namespace Server.Gumps
{
    class ExperienceGump : Gump
    {
        private readonly Mobile m_From;
        public ExperienceGump(Mobile from)
            : base(100, 100)
        {
            this.m_From = from;

            from.CloseGump(typeof(ExperienceGump));

            this.AddPage(0);
            this.AddBackground(0, 0, 650, 600, 0xA3C);

            int experiencePoints = GetExperience(from);

            int[] cols = { 20, 50, 80, 235, 265, 295, 450, 480, 510 };

            this.AddLabel(cols[0], 10, 965, "Select a Stat or Skill to Increase");
            this.AddLabel(cols[0], 35, 965, "Experience Scrolls Available: " + experiencePoints);


            int gumpY = 70;
            this.AddLabel(cols[0]+13, gumpY, 965, "1");
            this.AddLabel(cols[1], gumpY, 965, "Max");
            this.AddLabel(cols[3]+13, gumpY, 965, "1");
            this.AddLabel(cols[4], gumpY, 965, "Max");
            this.AddLabel(cols[6]+13, gumpY, 965, "1");
            this.AddLabel(cols[7], gumpY, 965, "Max");

            gumpY = 90;
            this.AddButton(cols[0], gumpY, 4014, 4016, 1, GumpButtonType.Reply, 1);
            this.AddButton(cols[1], gumpY, 4014, 4016, 1000 + 1, GumpButtonType.Reply, 1);
            this.AddLabel(cols[2], gumpY, 965, "Strength");
            this.AddButton(cols[3], gumpY, 4014, 4016, 2, GumpButtonType.Reply, 1);
            this.AddButton(cols[4], gumpY, 4014, 4016, 1000 + 2, GumpButtonType.Reply, 1);
            this.AddLabel(cols[5], gumpY, 965, "Dexterity");
            this.AddButton(cols[6], gumpY, 4014, 4016, 3, GumpButtonType.Reply, 1);
            this.AddButton(cols[7], gumpY, 4014, 4016, 1000 + 3, GumpButtonType.Reply, 1);
            this.AddLabel(cols[8], gumpY, 965, "Intelligence");

            gumpY = 120;
            int gumpOffsetY = 20;
            int rows = 20;
            int i = 0;
            foreach (Skill skill in from.Skills)
            {
                this.AddButton(cols[0 + i/rows*3], gumpY + (i-i/rows*rows) * gumpOffsetY, 4014, 4016, i + 4, GumpButtonType.Reply, 1);
                this.AddButton(cols[1 + i/rows*3], gumpY + (i-i/rows*rows) * gumpOffsetY, 4014, 4016, 1000 + i + 4, GumpButtonType.Reply, 1);
                this.AddLabel (cols[2 + i/rows*3], gumpY + (i-i/rows*rows) * gumpOffsetY, 965, skill.Name);
                i++;

            }

            gumpY = 550;
            this.AddButton(cols[0], gumpY, 4014, 4016, 999, GumpButtonType.Reply, 1);
            this.AddLabel(cols[1], gumpY, 965, "Buy a Craftsman's Apron (500 scrolls)");

        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            int id = info.ButtonID;

            int scrollsToSpend = 1;
            if (id >= 1000)
            {
                id -= 1000;
                scrollsToSpend = GetExperience(from);
            }

            bool success = false;
            switch (id)
            {
                case 0:
                    return;
                case 1:
                    success = Purchase(from, SkillCheck.Stat.Str, scrollsToSpend);
                    break;
                case 2:
                    success = Purchase(from, SkillCheck.Stat.Dex, scrollsToSpend);
                    break;
                case 3:
                    success = Purchase(from, SkillCheck.Stat.Int, scrollsToSpend);
                    break;
                case 999:
                    scrollsToSpend = 500;
                    if(GetExperience(from) < scrollsToSpend)
                    {
                        from.SendMessage("You do not have enough experience scrolls in your backpack.");
                        return;
                    }
                    if (from.Backpack.ConsumeTotal(new Type[] { typeof(ExperienceScroll) }, new int[] { scrollsToSpend }) != -1)
                        return;
                    from.AddToBackpack(new CraftsmansApron());
                    return;
                default:
                    Skill skill = GetSkillByIndex(from, id - 4);
                    success = Purchase(from, skill, scrollsToSpend);
                    break;
            }

            if(!success)
                from.SendMessage("You cannot do that. Make sure you have experience scrolls in your backpack and that your locks are appropriately set.");

            if (GetExperience(from) > 0)
            {
                from.SendGump(new ExperienceGump(from));
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
        Skill GetSkillByIndex(Mobile from, int index)
        {
            int i = 0;
            foreach (Skill skill in from.Skills)
            {
                if (i == index)
                    return skill;
                i++;
            }

            return null;
        }

        bool Purchase(Mobile from, SkillCheck.Stat stat, int scrollsToSpend)
        {
            if (scrollsToSpend <= 0)
                return false;

            int gainAmount = scrollsToSpend;

            int canGainAmount = CanGainAmount(from, stat);
            if (canGainAmount <= 0)
                return false;

            if (canGainAmount < gainAmount)
                gainAmount = canGainAmount;

            if (from.Backpack.ConsumeTotal(new Type[] { typeof(ExperienceScroll) }, new int[] { gainAmount }) != -1)
                return false;

            int lossAmount = gainAmount - (from.StatCap - from.RawStatTotal);
            if (lossAmount > 0)
            {
                if (from.StrLock == StatLockType.Down)
                {
                    if (from.RawStr - 10 >= lossAmount)
                    {
                        from.RawStr -= lossAmount;
                        lossAmount = 0;
                    }
                    else
                    {
                        lossAmount -= from.RawStr - 10;
                        from.RawStr = 10;

                    }
                }
            }
            if (lossAmount > 0)
            {
                if (from.DexLock == StatLockType.Down)
                {
                    if (from.RawDex - 10 >= lossAmount)
                    {
                        from.RawDex -= lossAmount;
                        lossAmount = 0;
                    }
                    else
                    {
                        lossAmount -= from.RawDex - 10;
                        from.RawDex = 10;

                    }
                }
            }
            if (lossAmount > 0)
            {
                if (from.IntLock == StatLockType.Down)
                {
                    if (from.RawInt - 10 >= lossAmount)
                    {
                        from.RawInt -= lossAmount;
                        lossAmount = 0;
                    }
                    else
                    {
                        lossAmount -= from.RawInt - 10;
                        from.RawInt = 10;

                    }
                }
            }

            switch(stat)
            {
                case SkillCheck.Stat.Str:
                    from.RawStr += gainAmount;
                    break;
                case SkillCheck.Stat.Dex:
                    from.RawDex += gainAmount;
                    break;
                case SkillCheck.Stat.Int:
                    from.RawInt += gainAmount;
                    break;
            }
            return true;

        }
        bool Purchase(Mobile from, Skill skill, int scrollsToSpend)
        {
            if (scrollsToSpend <= 0)
                return false;

            int scrollsToConsume;
            int gainAmount = GetSkillGainAmountFromScrollsToSpend(skill, scrollsToSpend, out scrollsToConsume);
            if (gainAmount <= 0)
                return false;

            int canGainAmount = CanGainAmount(from, skill);
            if (canGainAmount <= 0)
                return false;

            if (canGainAmount < gainAmount)
            {
                gainAmount = canGainAmount;
                scrollsToConsume = GetScrollsToConsumeFromSkillGainAmount(skill, gainAmount);
            }

            if (from.Backpack.ConsumeTotal(new Type[] { typeof(ExperienceScroll) }, new int[] { scrollsToConsume }) != -1)
                return false;


            int lossAmount = gainAmount - (from.SkillsCap - from.SkillsTotal);
            if(lossAmount > 0)
            {
                foreach(Skill s in from.Skills)
                {
                    if(s.Lock == SkillLock.Down)
                    {
                        if(s.BaseFixedPoint >= lossAmount)
                        {
                            s.BaseFixedPoint -= lossAmount;
                            lossAmount = 0;
                            break;
                        }
                        else
                        {
                            lossAmount -= s.BaseFixedPoint;
                            s.BaseFixedPoint = 0;

                        }
                    }
                }
            }
            skill.BaseFixedPoint += gainAmount;
            return true;
        }
        int GetScrollsToConsumeFromSkillGainAmount(Skill skill, int gainAmount)
        {
            int amount = 0;
            int scrollsToConsume = 0;
            while (amount < gainAmount)
            {
                int gain = GetSkillGainIncrement(skill.BaseFixedPoint + amount);
                amount += gain;
                if (gain <= 0)
                    break;
                scrollsToConsume++;
            }
            return scrollsToConsume;
        }
        int GetSkillGainAmountFromScrollsToSpend(Skill skill, int scrollsToSpend, out int scrollsToConsume)
        {
            int amount = 0;
            int i = 0;
            for (; i < scrollsToSpend; i++)
            {
                int gain = GetSkillGainIncrement(skill.BaseFixedPoint + amount);
                amount += gain;
                if (gain <= 0)
                    break;
            }
            scrollsToConsume = i;
            return amount;
        }
        int GetSkillGainIncrement(int skillLevel)
        {
            if (skillLevel <= 600)
                return 10;
            else if (skillLevel <= 700)
                return 8;
            else if (skillLevel <= 800)
                return 6;
            else if (skillLevel <= 900)
                return 4;
            else if (skillLevel <= 1000)
                return 3;
            else if (skillLevel <= 1100)
                return 2;
            else if (skillLevel <= 1200)
                return 1;
            else
                return 0;
        }

        int CanGainAmount(Mobile from, SkillCheck.Stat stat)
        {
            int canRaiseBy = CanRaiseBy(from, stat);
            if (canRaiseBy <= 0)
                return 0;
            int canLowerBy = from.StatCap - from.RawStatTotal;

            switch (stat)
            {
                case SkillCheck.Stat.Str:
                    canLowerBy += CanLowerBy(from, SkillCheck.Stat.Dex);
                    canLowerBy += CanLowerBy(from, SkillCheck.Stat.Int);
                    break;
                case SkillCheck.Stat.Dex:
                    canLowerBy += CanLowerBy(from, SkillCheck.Stat.Str);
                    canLowerBy += CanLowerBy(from, SkillCheck.Stat.Int);
                    break;
                case SkillCheck.Stat.Int:
                    canLowerBy += CanLowerBy(from, SkillCheck.Stat.Str);
                    canLowerBy += CanLowerBy(from, SkillCheck.Stat.Dex);
                    break;
            }
            return (canRaiseBy <= canLowerBy) ? canRaiseBy : canLowerBy;
        }

        int CanGainAmount(Mobile from, Skill skill)
        {
            int canRaiseBy = CanRaiseBy(from, skill);
            if (canRaiseBy <= 0)
                return 0;

            int canLowerBy = from.SkillsCap - from.SkillsTotal;
            foreach(Skill s in from.Skills)
                canLowerBy += CanLowerBy(from, s);

            return (canRaiseBy <= canLowerBy) ? canRaiseBy : canLowerBy;
        }

        int CanLowerBy(Mobile from, SkillCheck.Stat stat)
        {
            switch (stat)
            {
                case SkillCheck.Stat.Str:
                    return (from.StrLock == StatLockType.Down) ? from.RawStr - 10 : 0;
                case SkillCheck.Stat.Dex:
                    return (from.DexLock == StatLockType.Down) ? from.RawDex - 10 : 0;
                case SkillCheck.Stat.Int:
                    return (from.IntLock == StatLockType.Down) ? from.RawInt - 10 : 0;
            }
            return 0;
        }

        int CanLowerBy(Mobile from, Skill skill)
        {
            return (from.Skills[skill.SkillID].Lock == SkillLock.Down) ? from.Skills[skill.SkillID].BaseFixedPoint : 0;
        }

        int CanRaiseBy(Mobile from, SkillCheck.Stat stat)
        {
            switch (stat)
            {
                case SkillCheck.Stat.Str:
                    return (from.StrLock == StatLockType.Up) ? from.StrCap - from.RawStr : 0;
                case SkillCheck.Stat.Dex:
                    return (from.DexLock == StatLockType.Up) ? from.DexCap - from.RawDex : 0;
                case SkillCheck.Stat.Int:
                    return (from.IntLock == StatLockType.Up) ? from.IntCap - from.RawInt : 0;
            }
            return 0;
        }

        int CanRaiseBy(Mobile from, Skill skill)
        {
            return (from.Skills[skill.SkillID].Lock == SkillLock.Up) ? from.Skills[skill.SkillID].CapFixedPoint - from.Skills[skill.SkillID].BaseFixedPoint : 0;
        }

    }
}

