using System;
using MonoTouch.UIKit;

namespace XamarinControls
{
	public interface IUICollectionViewLayoutWarpable
	{
		LSCollectionViewLayoutHelper LayoutHelper {get;  set;}

		UICollectionViewScrollDirection ScrollDirection();

		UICollectionViewScrollDirection DragDirection();

	}
}
