using UnityEngine;

namespace Libraries {
	public class Point {

		public static Point zero {
			get { return new Point(0, 0); }
		}

		public int x, y;

		public Point (int x, int y) {
			this.x = x;
			this.y = y;
		}

		public void Set (int newX, int newY) {
			this.x = newX;
			this.y = newY;
		}

		public void Set (Point newPoint) {
			this.x = newPoint.x;
			this.y = newPoint.y;
		}

		public override string ToString () {
			return "Point(" + x + ", " + y + ")";
		}

		public bool Equals(Point obj) {
			return x == obj.x && y == obj.y;
		}

		public static implicit operator Point (Vector2 vector) {
			return new Point((int) vector.x, (int) vector.y);
		}

		public static implicit operator Vector2 (Point point) {
			return new Vector2((float) point.x, (float) point.y);
		}

		public static implicit operator Point (Vector3 vector) {
			return new Point((int) vector.x, (int) vector.y);
		}

		public static implicit operator Vector3 (Point point) {
			return new Vector3((float) point.x, (float) point.y, 0);
		}

	}
}