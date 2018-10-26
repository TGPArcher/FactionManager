using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FactionManager
{
    public class ColoniesManagerTabDef : ManagerTabDef
    {
        public override void DrawManagerRect(Rect outRect, ref Vector2 scrollPosition, ref float scrollViewHeight)
        {
            base.DrawManagerRect(outRect, ref scrollPosition, ref scrollViewHeight);

            List<Settlement> settlements = Find.World.worldObjects.Settlements.FindAll(settlement => settlement.Faction.IsPlayer);

            GUI.BeginGroup(outRect);
            Rect listOutRect = new Rect(0f, 0f, outRect.width, outRect.height);
            Rect listRect = new Rect(0f, 0f, outRect.width, scrollViewHeight);

            float num = 0f;
            Widgets.BeginScrollView(listOutRect, ref scrollPosition, listRect);
            foreach (var item in settlements)
            {
                GUI.color = new Color(1f, 1f, 1f, 0.2f);
                Widgets.DrawLineHorizontal(0f, num + RowHeight, listRect.width);
                DrawColonyRow(item, num, listRect);
                num += RowHeight;
            }

            if (Event.current.type == EventType.Layout)
            {
                scrollViewHeight = num;
            }
            Widgets.EndScrollView();
            GUI.EndGroup();
        }

        private static float DrawColonyRow(Settlement settlement, float rowY, Rect fillRect)
        {
            GUI.color = Color.white;
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleLeft;

            Rect settlementRect = new Rect(0f, rowY, fillRect.width * NameWidthProportion, RowHeight);
            Rect settlementNameRect = new Rect(10f, rowY, settlementRect.width - 10, RowHeight);
            string settlementName = settlement.Name;
            Widgets.Label(settlementNameRect, settlementName);

            float buttonPadding = (fillRect.width - settlementRect.xMax) * ButtonPaddingProportion;
            float buttonWidth = (fillRect.width - buttonPadding) - (settlementRect.width + buttonPadding);
            Rect actionButtonRect = new Rect(settlementRect.width + buttonPadding, rowY + (RowHeight - ButtonHeight) / 2, buttonWidth, ButtonHeight);
            string buttonText = settlement.HasMap == true ? "FM.Unload" : "FM.Load";
            string translatedButtonText = buttonText.Translate();

            if (Widgets.ButtonText(actionButtonRect, translatedButtonText))
            {
                Action action = delegate
                {
                    if (buttonText == "FM.Load")
                    {
                        LongEventHandler.QueueLongEvent(delegate
                        {
                            PersistenceUtility.LoadMap(settlement.Name);
                        }, "LoadingLongEvent", doAsynchronously: true, exceptionHandler: null);
                    }
                    else
                    {
                        LongEventHandler.QueueLongEvent(delegate
                        {
                            PersistenceUtility.UnloadMap(settlement.Map, settlement.Name);
                        }, "SavingLongEvent", doAsynchronously: false, exceptionHandler: null);
                    }
                    Find.WindowStack.TryRemove(typeof(MainTabWindow_Colonies));
                };

                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("FM.actionConfirm".Translate() + buttonText.Translate().ToLower() + "?", action));
            }

            Rect rowRect = new Rect(0f, rowY, fillRect.width, RowHeight);
            if (Mouse.IsOver(rowRect))
            {
                GUI.DrawTexture(rowRect, TexUI.HighlightTex);
            }
            if (Widgets.ButtonInvisible(rowRect))
            {
                if (!Mouse.IsOver(actionButtonRect))
                {
                    Find.WindowStack.TryRemove(typeof(MainTabWindow_Colonies));
                    CameraJumper.TryJumpAndSelect(settlement);
                }
            }

            Text.Anchor = TextAnchor.UpperLeft;
            return rowY;
        }
    }
}
