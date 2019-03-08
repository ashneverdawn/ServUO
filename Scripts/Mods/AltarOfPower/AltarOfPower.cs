using System;
using Server.Gumps;
using Server.Targeting;

namespace Server.Items
{
    public class AltarOfPower : Item
    {
        [Constructable]
        public AltarOfPower()
            : base(0x40BC)
        {
            this.Name = "Altar of Power";
        }
        public AltarOfPower(Serial serial) : base(serial)
        {
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

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                from.SendMessage("Select a power scroll: (Need 2 identical)");
                from.Target = new InternalTarget();
            }
            else
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(2, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PowerScroll)
                {
                    PowerScroll ps = targeted as PowerScroll;
                    int count = 0;
                    foreach (PowerScroll item in from.Backpack.FindItemsByType<PowerScroll>())
                    {
                        if(item.Skill == ps.Skill && item.Value == ps.Value)
                        {
                            count++;
                        }
                    }

                    if(count > 1)
                    {
                        from.SendGump(new AltarOfPowerGump(from, ps));
                    }
                    else
                    {
                        from.SendMessage("You must have 2 identical copies of the power scroll.");
                    }

                }
                else
                {
                    from.SendMessage("That is not a power scroll!");
                }
            }
        }
    }
}
