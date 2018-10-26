using UnityEngine;
using Verse;

namespace FactionManager
{
    public class ManagerTabDef : Def
    {
        protected const float NameWidthProportion = 0.7f;
        protected const float RowHeight = 50f;
        protected const float ButtonPaddingProportion = 0.2f;
        protected const float ButtonHeight = 40f;

        public virtual void DrawManagerRect(Rect outRect, ref Vector2 scrollPosition, ref float scrollViewHeight)
        {
        }
    }
}
