//using System;
//using System.Drawing;
//
//namespace XamarinControls
//{
//	//http://fossies.org/linux/misc/mono-sources/monodevelop/monodevelop-4.0.13-38.tar.gz/monodevelop-4.0.13/external/maccore/src/CoreGraphics/CGGeometry.cs
//	public enum CGRectEdge {
//		 		MinXEdge,
//		 		MinYEdge,
//		 		MaxXEdge,
//		 		MaxYEdge,
//		 	}
//	//http://nshipster.com/cggeometry/
//	public class RectangleFExtensions
//	{
//		public RectangleFExtensions ()
//		{
//		}
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern float CGRectGetMinX (RectangleF rect);
//		public static float GetMinX (this RectangleF self)
//		{
////			return CGRectGetMinX (self);
//			return self.X;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern float CGRectGetMidX (RectangleF rect);
//		public static float GetMidX (this RectangleF self)
//		{
////			return CGRectGetMidX (self);
//			return (self.X + self.Width)/2;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern float CGRectGetMaxX (RectangleF rect);
//		public static float GetMaxX (this RectangleF self)
//		{
////			return CGRectGetMaxX (self);
//			return self.X + self.Width;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern float CGRectGetMinY (RectangleF rect);
//		public static float GetMinY (this RectangleF self)
//		{
////			return CGRectGetMinY (self);
//			return self.Y;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern float CGRectGetMidY (RectangleF rect);
//		public static float GetMidY (this RectangleF self)
//		{
////			return CGRectGetMidY (self);
//			return (self.Y + self.Height)/2;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern float CGRectGetMaxY (RectangleF rect);
//		public static float GetMaxY (this RectangleF self)
//		{
////			return CGRectGetMaxY (self);
//			return self.Y + self.Height;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern RectangleF CGRectStandardize (RectangleF rect);
//		public static RectangleF Standardize (this RectangleF self)
//		{
////			return CGRectStandardize (self);
//			return self;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern bool CGRectIsNull (RectangleF rect);
//		public static bool IsNull (this RectangleF self)
//		{
////			return CGRectIsNull (self);
//			return self;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern bool CGRectIsInfinite (RectangleF rect);
//		public static bool IsInfinite (this RectangleF self)
//		{
////			return CGRectIsNull (self);
//			return self;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern RectangleF CGRectInset (RectangleF rect, float dx, float dy);
//		public static RectangleF Inset (this RectangleF self, float dx, float dy)
//		{
////			return CGRectInset (self, dx, dy);
//			return self;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern RectangleF CGRectIntegral (RectangleF rect);
//		public static RectangleF Integral (this RectangleF self)
//		{
////			return CGRectIntegral (self);
//			return self;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern RectangleF CGRectUnion (RectangleF r1, RectangleF r2);
//		public static RectangleF UnionWith (this RectangleF self, RectangleF other)
//		{
////			return CGRectUnion (self, other);
//			return self;
//		}
//
////		[DllImport (Constants.CoreGraphicsLibrary)]
////		static extern void CGRectDivide (RectangleF rect, out RectangleF slice, out RectangleF remainder, float amount, CGRectEdge edge);
//		public static void Divide (this RectangleF self, float amount, CGRectEdge edge, out RectangleF slice, out RectangleF remainder)
//		{
////			CGRectDivide (self, out slice, out remainder, amount, edge);
//		}
//	}
//
//}
//
