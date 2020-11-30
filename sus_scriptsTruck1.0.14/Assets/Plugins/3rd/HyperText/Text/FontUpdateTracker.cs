using UnityEngine;
using System.Collections.Generic;

namespace Candlelight.UI
{
	public static class FontUpdateTracker
	{
		private static Dictionary<Font, List<HyperText>> s_Tracked = new Dictionary<Font, List<HyperText>>();
		public static void TrackHyperText(HyperText hyperText)
		{
			if (hyperText.FontToUse == null)
			{
				return;
			}
			List<HyperText> exists;
			s_Tracked.TryGetValue(hyperText.FontToUse, out exists);
			if (exists == null)
			{
				exists = new List<HyperText>();
				s_Tracked.Add(hyperText.FontToUse, exists);
#if UNITY_4_6
				hyperText.FontToUse.textureRebuildCallback += RebuildForFont(hyperText.FontToUse);
#else
				Font.textureRebuilt += font => RebuildForFont(hyperText.FontToUse);
#endif
			}
			exists.Add(hyperText);
		}

        public static void UntrackHyperText(HyperText hyperText)
        {
            if (hyperText.FontToUse == null)
            {
                return;
            }
            List<HyperText> texts;
            s_Tracked.TryGetValue(hyperText.FontToUse, out texts);
            if (texts == null)
            {
                return;
            }
            texts.Remove(hyperText);
        }

#if UNITY_4_6
		private static Font.FontTextureRebuildCallback RebuildForFont(Font font)
		{
			return () =>
#else
        private static void RebuildForFont(Font font)
		{
#endif
			{
				if (font == null)
				{
					return;
				}
				List<HyperText> texts;
				s_Tracked.TryGetValue(font, out texts);
				if (texts == null)
				{
					return;
				}
				for (int i = 0; i < texts.Count; ++i)
				{
					texts[i].FontTextureChanged();
				}
			};
		}
        
	}
}