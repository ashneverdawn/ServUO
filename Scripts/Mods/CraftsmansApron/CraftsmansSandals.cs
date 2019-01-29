using System;
using Server.Engines.Craft;
using Server.Gumps;

namespace Server.Items
{
    public class CraftsmansSandals : BaseClothing
    {
        [Constructable]
        public CraftsmansSandals()
            : this(0)
        {
        }

        [Constructable]
        public CraftsmansSandals(int hue)
            : base(0x170D, Layer.Shoes, hue)
        {
            this.Name = "Craftsman's Sandals";
            this.Weight = 1.0;
        }

        public CraftsmansSandals(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from) || IsChildOf(from.Backpack))
            {
                from.SendGump(new CraftsmansGump(from, this));
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
