using System;
using Server.Engines.Craft;
using Server.Gumps;

namespace Server.Items
{
    [Flipable(0x153d, 0x153e)]
    public class CraftsmansApron : BaseShirt
    {
        [Constructable]
        public CraftsmansApron()
            : this(0)
        {
        }

        [Constructable]
        public CraftsmansApron(int hue)
            : base(0x153d, hue)
        {
            this.Name = "Craftsman's Apron";
            this.Weight = 1.0;
        }

        public CraftsmansApron(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.SendGump(new CraftsmansApronGump(from, this));
            }
            else
            {
                from.SendMessage("This must be in your backpack to do that.");
            }
        }

        public override bool CanBeWornByGargoyles
        {
            get
            {
                return true;
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
