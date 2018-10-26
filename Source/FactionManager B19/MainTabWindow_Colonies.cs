using UnityEngine;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace FactionManager
{
    public class MainTabWindow_Colonies : MainTabWindow
    {
        private const float NameWidthProportion = 0.7f;
        private const float RowHeight = 50f;
        private const float ButtonPaddingProportion = 0.2f;
        private const float ButtonHeight = 40f;

        private Vector2 scrollPosition = Vector2.zero;
        private float scrollViewHeight;
        private ManagerTabDef curTabInt;

        private ManagerTabDef CurTab
        {
            get
            {
                return curTabInt;
            }
            set
            {
                if(value != curTabInt)
                {
                    curTabInt = value;
                    scrollPosition = Vector2.zero;
                }
            }
        }

        public override void PreOpen()
        {
            base.PreOpen();

            if(CurTab == null)
            {
                CurTab = ManagerTabDefOf.Colonies;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            base.DoWindowContents(inRect);

            Rect generalInfoRect = new Rect(0f, 0f, inRect.width, RowHeight);
            // General Info group
            GUI.BeginGroup(generalInfoRect);
            GUI.color = Color.yellow;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            Rect factionNameRect = new Rect(0f, 0f, generalInfoRect.width / 2 - 10, RowHeight);
            Widgets.Label(factionNameRect, "FM.Faction".Translate() + ": " + GetFactionName());
            Text.Anchor = TextAnchor.UpperRight;
            Rect worldNameRect = new Rect(factionNameRect.xMax + 10, 0f, generalInfoRect.width / 2 - 10, RowHeight);
            Widgets.Label(worldNameRect, "FM.World".Translate() + ": " + GetWorldName());
            // Reset setting to default
            Text.Anchor = TextAnchor.UpperLeft;
            GUI.color = Color.white;
            GUI.EndGroup();

            // List rect
            Rect menuRect = new Rect(0f, RowHeight + 5, inRect.width, inRect.height - RowHeight - 5);
            Widgets.DrawMenuSection(menuRect);
            List<TabRecord> list = new List<TabRecord>();
            foreach (ManagerTabDef allDef in DefDatabase<ManagerTabDef>.AllDefs)
            {
                ManagerTabDef localTabDef = allDef;
                list.Add(new TabRecord(localTabDef.LabelCap, delegate
                {
                    CurTab = localTabDef;
                }, localTabDef == CurTab));
            }
            TabDrawer.DrawTabs(menuRect, list);

            CurTab?.DrawManagerRect(menuRect, ref scrollPosition, ref scrollViewHeight);
        }

        private string GetWorldName()
        {
            return Find.World.info.name;
        }

        private string GetFactionName()
        {
            return Find.FactionManager.OfPlayer.Name;
        }
    }
}
