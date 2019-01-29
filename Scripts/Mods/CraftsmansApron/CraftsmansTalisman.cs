using System;
using Server.Engines.Craft;
using Server.Gumps;

namespace Server.Items
{
    public class CraftsmansTalisman : BaseClothing
    {
        [Constructable]
        public CraftsmansTalisman()
            : this(0)
        {
        }

        [Constructable]
        public CraftsmansTalisman(int hue)
            : base(0x2F5B, Layer.Talisman, hue)
        {
            this.Name = "Craftsman's Talisman";
            this.Weight = 1.0;
        }

        public CraftsmansTalisman(Serial serial)
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
