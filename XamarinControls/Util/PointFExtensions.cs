using System;
using System.Drawing;

namespace XamarinControls
{
	static class PointFExtensions
	{
		public static PointF Add(this PointF point, PointF other)
		{
			return new PointF(point.X + other.X, point.Y + other.Y);
		}

		public static PointF Add(this PointF point, float value)
		{
			return new PointF(point.X + value, point.Y + value);
		}

		public static PointF Multiply(this PointF point, PointF other)
		{
			return new PointF(point.X * other.X, point.Y * other.Y);
		}

		public static PointF Multiply(this PointF point, float value)
		{
			return new PointF(point.X * value, point.Y * value);
		}
	}
}

