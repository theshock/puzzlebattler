using Match;
using Match.Gem;
using UnityEngine;

namespace Config {
	public class CColor : MonoBehaviour {
		public Sprite imageBlue;
		public Sprite imageGreen;
		public Sprite imagePurple;
		public Sprite imageRed;
		public Sprite imageYellow;

		public Sprite GetCorrectSprite (EColor type) {
			switch (type) {
				case EColor.Blue  : return imageBlue;
				case EColor.Green : return imageGreen;
				case EColor.Purple: return imagePurple;
				case EColor.Red   : return imageRed;
				case EColor.Yellow: return imageYellow;
			}

			return null;
		}
	}
}