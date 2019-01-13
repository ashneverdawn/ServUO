using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Gumps;
using Server.Network;
using System.Linq;
using System.IO;
using Server.Items;
using Server.Mobiles;
using Server.Accounting;

namespace Server.Commands 
{
    public class AdminCommands
    {
        public static void Initialize() 
        { 
            CommandSystem.Register("ResetSkillForAll", AccessLevel.Administrator, new CommandEventHandler(ResetSkillForAll));
        }

        [Usage("ResetSkillForAll [SkillName]")]
        [Description("Resets the skill on all characters on the server. They are compensated with 1 Experience Scroll for each skill point lost.")]
        private static void ResetSkillForAll(CommandEventArgs e)
        {
            if (e.Arguments.Length != 1)
            {
                e.Mobile.SendMessage("Invalid number of arguments. (Expecting 1)");
                return;
            }

            bool argOk = false;
            var values = Enum.GetValues(typeof(SkillName));
            List<string> skillNameList = new List<string>();
            SkillName skillName = SkillName.Alchemy;
            foreach (SkillName sn in values)
            {
                skillNameList.Add(sn.ToString());
                if (sn.ToString().ToLower() == e.Arguments[0].ToLower())
                {
                    skillName = sn;
                    argOk = true;
                    break;
                }
            }
            if(!argOk)
            {
                e.Mobile.SendMessage("Invalid skill name. Skill Names are as follows: " + String.Join(", ", skillNameList.ToArray()));
                return;
            }

            foreach (var a in Accounts.GetAccounts())
            {
                for (int i = 0; i < a.Length; ++i)
                {
                    Mobile m = a[i];
                    if (m == null)
                        continue;

                    int scrollsToDrop = m.Skills[skillName].BaseFixedPoint / 10;
                    m.Skills[skillName].BaseFixedPoint = 0;
                    m.AddToBackpack(new ExperienceScroll(scrollsToDrop));
                }
            }
            e.Mobile.SendMessage("Serverwide skill reset complete.");

        }
    }
}
