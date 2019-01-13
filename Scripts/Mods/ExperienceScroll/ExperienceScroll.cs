using System;
using Server.Gumps;

namespace Server.Items
{
    public class ExperienceScroll : Item, ICommodity
    {
        public static void HandleKill(Mobile victim, Mobile killer)
        {
            if(victim.Fame < 2000)
            {
                double chance = (1.0 - (victim.Fame / 2000.0)) * 0.75;
                if (victim.Fame > 1000)
                    chance *= 0.5;

                if (chance > Utility.RandomDouble())
                {
                    if(killer.Account.Young)
                        killer.AddToBackpack(new ExperienceScroll(4, 8));
                    else
                        killer.AddToBackpack(new ExperienceScroll(1, 5));

                    killer.SendMessage("You have been awarded experience.");
                }

            }
        }
        [Constructable]
        public ExperienceScroll()
            : this(1)
        {
        }

        [Constructable]
        public ExperienceScroll(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public ExperienceScroll(int amount)
            : base(0x226A)
        {
            this.Name = "Experience Scroll";
            this.Stackable = true;
            this.Amount = amount;
            this.Hue = 0x481;
        }

        public ExperienceScroll(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 0.2;
            }
        }

        TextDefinition ICommodity.Description
        {
            get
            {
                return this.LabelNumber;
            }
        }
        bool ICommodity.IsDeedable
        {
            get
            {
                return true;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendGump(new ExperienceGump(from));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}