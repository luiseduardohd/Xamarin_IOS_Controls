using System;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using System.Runtime.InteropServices;
using System.Drawing;

namespace XamarinControls
{
	public class UICollectionViewDraggable :UICollectionView
	{
		enum OBJC_ASSOCIATION {
			ASSIGN = 0,
			RETAIN_NONATOMIC = 1,
			COPY_NONATOMIC = 3,
			RETAIN = 01401,
			COPY = 01403,
		}
		public UICollectionViewDraggable(RectangleF rectangleF,UICollectionViewLayout layout):base(rectangleF, layout)
		{
		}
		public UICollectionViewDraggable(NSCoder coder):base(coder)
		{
		}

		public UICollectionViewDraggable(NSObjectFlag  t):base(t)
		{
		}

		public UICollectionViewDraggable(IntPtr handle):base(handle)
		{
		}


		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern void objc_setAssociatedObject (IntPtr obj, IntPtr key, IntPtr value, OBJC_ASSOCIATION policy);
		[DllImport ("/usr/lib/libobjc.dylib")]
		static extern IntPtr objc_getAssociatedObject (IntPtr obj, IntPtr key);
//		[unsafe]
		//char [] LSCollectionViewHelperObjectKey = "LSCollectionViewHelper".ToCharArray();
		string LSCollectionViewHelperObjectKey = "LSCollectionViewHelper";
//		LSCollectionViewHelper objc_getAssociatedObject (UICollectionViewDraggable uICollectionViewDraggable, object lSCollectionViewHelperObjectKey)
//		{
//			throw new NotImplementedException ();
//		}

//		public UICollectionViewDraggable ()
//		{
//		}
		/*unsafe*/ void DraggableCleanup()
		{
			LSCollectionViewHelper helper = (LSCollectionViewHelper) MonoTouch.ObjCRuntime.Runtime.GetNSObject (objc_getAssociatedObject(this.Handle, Marshal.StringToCoTaskMemUni( LSCollectionViewHelperObjectKey)));
			if (helper != null)
			{
				helper.UnbindFromCollectionView(this);
				objc_setAssociatedObject(this.Handle, Marshal.StringToCoTaskMemUni(LSCollectionViewHelperObjectKey), IntPtr.Zero, OBJC_ASSOCIATION.RETAIN_NONATOMIC);
			}

		}

		LSCollectionViewHelper GetHelper()
		{
			LSCollectionViewHelper helper = (LSCollectionViewHelper) MonoTouch.ObjCRuntime.Runtime.GetNSObject (objc_getAssociatedObject(this.Handle, Marshal.StringToCoTaskMemUni( LSCollectionViewHelperObjectKey)));
			if (helper == null)
			{
				helper = new LSCollectionViewHelper(this);
				objc_setAssociatedObject(this.Handle, Marshal.StringToCoTaskMemUni(LSCollectionViewHelperObjectKey), helper.Handle, OBJC_ASSOCIATION.RETAIN_NONATOMIC);
			}

			return helper;
		}

		bool Draggable()
		{
			return this.GetHelper().Enabled;
		}

		void SetDraggable(bool draggable)
		{
			this.GetHelper().Enabled = draggable;
		}

		UIEdgeInsets ScrollingEdgeInsets()
		{
			return this.GetHelper().ScrollingEdgeInsets;
		}

		void SetScrollingEdgeInsets(UIEdgeInsets scrollingEdgeInsets)
		{
			this.GetHelper().ScrollingEdgeInsets = scrollingEdgeInsets;
		}

		float ScrollingSpeed()
		{
			return this.GetHelper().ScrollingSpeed;
		}

		void SetScrollingSpeed(float scrollingSpeed)
		{
			this.GetHelper().ScrollingSpeed = scrollingSpeed;
		}

		NSIndexPath IndexPathForMovingItem()
		{
			return this.GetHelper().IndexPathForMovingItem;
		}

		UIImageView DropOnToDeleteView()
		{
			return this.GetHelper().DropOnToDeleteView;
		}

		void SetDropOnToDeleteView(UIImageView dropOnToDeleteView)
		{
			this.GetHelper().DropOnToDeleteView = dropOnToDeleteView;
		}


	}
}

