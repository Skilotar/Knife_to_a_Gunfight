using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Knives
{
    class GlobalCharmList
    {
        public GlobalCharmList()
        {

        }

        public static void ConcatCharmLists()
        {
            TotalCharmslist.AddRange(CommonCharmslist);     // D
            TotalCharmslist.AddRange(UncommonCharmslist);   // C
            TotalCharmslist.AddRange(RareCharmslist);       // B
            TotalCharmslist.AddRange(LegendaryCharmslist);  // A-S     

            if (debugging)
            {
                ETGModConsole.Log("-Common charms- :" + CommonCharmslist.Count);
                
                for(int i = CommonCharmslist.Count - 1; i >= 0; i--)
                {
                    ETGModConsole.Log(PickupObjectDatabase.GetById(CommonCharmslist[i]).name);
                }
                ETGModConsole.Log("-UnCommon charms- :" + UncommonCharmslist.Count);
                for (int i = UncommonCharmslist.Count - 1; i >= 0; i--)
                {
                    ETGModConsole.Log(PickupObjectDatabase.GetById(UncommonCharmslist[i]).name);
                }
                ETGModConsole.Log("-Rare charms- :" + RareCharmslist.Count);
                for (int i = RareCharmslist.Count - 1; i >= 0; i--)
                {
                    ETGModConsole.Log(PickupObjectDatabase.GetById(RareCharmslist[i]).name);
                }
                ETGModConsole.Log("-Legendary charms- :" + LegendaryCharmslist.Count);
                for (int i = LegendaryCharmslist.Count - 1; i >= 0; i--)
                {
                    ETGModConsole.Log(PickupObjectDatabase.GetById(LegendaryCharmslist[i]).name);
                }
            }
        }
        public static bool debugging = false;
        public static List<int> TotalCharmslist = new List<int>();
        public static List<int> CommonCharmslist = new List<int>();
        public static List<int> UncommonCharmslist = new List<int>();
        public static List<int> RareCharmslist = new List<int>();
        public static List<int> LegendaryCharmslist = new List<int>();
    }
}
