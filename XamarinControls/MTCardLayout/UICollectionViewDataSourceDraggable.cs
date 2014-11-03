using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using TimeInterval = System.Double;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace XamarinControls
{
	//public abstract class UICollectionViewDataSourceDraggable : UICollectionViewDataSource
	public interface UICollectionViewDataSourceDraggable : IUICollectionViewDataSource
	{
		void CollectionViewMoveItemAtIndexPathToIndexPath(UICollectionView collectionView, NSIndexPath fromIndexPath, NSIndexPath toIndexPath);

		bool CollectionViewCanMoveItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath);

		void CollectionViewDeleteItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath);

		bool CollectionViewCanDeleteItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath);

		bool CollectionViewCanMoveItemAtIndexPathToIndexPath(UICollectionView collectionView, NSIndexPath indexPath, NSIndexPath toIndexPath);

		void CollectionViewDidMoveItemAtIndexPathToIndexPath(UICollectionView collectionView, NSIndexPath indexPath, NSIndexPath toIndexPath);

		void CollectionViewAlterTranslation(UICollectionView collectionView, PointF translation);

		CGAffineTransform CollectionViewTransformForDraggingItemAtIndexPathDuration(UICollectionView collectionView, NSIndexPath indexPath, TimeInterval duration);

		UIImage CollectionViewImageForDraggingItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath);

		void CollectionViewDidDeleteItemAtIndexPath(UICollectionView collectionView, NSIndexPath indexPath);

	}
}
