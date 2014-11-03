using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using System.Collections;

namespace XamarinControls
{
	public class LSCollectionViewLayoutHelper
	{
		public LSCollectionViewLayoutHelper ()
		{
		}
		public UICollectionViewLayout /*<UICollectionViewLayout_Warpable>*/ CollectionViewLayout {get; private set;}

		public NSIndexPath FromIndexPath {get; set;}

		public NSIndexPath ToIndexPath {get; set;}

		public NSIndexPath HideIndexPath {get; set;}

		public LSCollectionViewLayoutHelper(UICollectionViewLayout /*<UICollectionViewLayout_Warpable>*/ collectionViewLayout)
		{
			this.CollectionViewLayout = collectionViewLayout;
		}

		public ArrayList ModifiedLayoutAttributesForElements(ArrayList elements)
		{
			UICollectionView collectionView = this.CollectionViewLayout.CollectionView;
			NSIndexPath fromIndexPath = this.FromIndexPath;
			NSIndexPath toIndexPath = this.ToIndexPath;
			NSIndexPath hideIndexPath = this.HideIndexPath;
			NSIndexPath indexPathToRemove = null;
			if (toIndexPath == null)
			{
				if (hideIndexPath == null)
				{
					return elements;
				}

				foreach (UICollectionViewLayoutAttributes layoutAttributes in elements)
				{
					if (layoutAttributes.RepresentedElementCategory != UICollectionElementCategory.Cell)
					{
						continue;
					}

					if (layoutAttributes.IndexPath.Equals(hideIndexPath))
					{
						layoutAttributes.Hidden = true;
					}

				}
				return elements;
			}

			if (fromIndexPath.Section != toIndexPath.Section)
			{
				indexPathToRemove = NSIndexPath.FromItemSection(collectionView.NumberOfItemsInSection(fromIndexPath.Section) - 1, fromIndexPath.Section);
			}

			foreach (UICollectionViewLayoutAttributes layoutAttributes in elements)
			{
				if (layoutAttributes.RepresentedElementCategory != UICollectionElementCategory.Cell)
				{
					continue;
				}

				if (layoutAttributes.IndexPath.Equals(indexPathToRemove))
				{
					layoutAttributes.IndexPath = NSIndexPath.FromItemSection(collectionView.NumberOfItemsInSection(toIndexPath.Section), toIndexPath.Section);
					if (layoutAttributes.IndexPath.Item != 0)
					{
						layoutAttributes.Center = this.CollectionViewLayout.LayoutAttributesForItem(layoutAttributes.IndexPath).Center;
					}

				}

				NSIndexPath indexPath = layoutAttributes.IndexPath;
				if (indexPath.Equals(hideIndexPath))
				{
					layoutAttributes.Hidden = true;
				}

				if (indexPath.Equals(toIndexPath))
				{
					layoutAttributes.IndexPath = fromIndexPath;
				}
				else if (fromIndexPath.Section != toIndexPath.Section)
				{
					if (indexPath.Section == fromIndexPath.Section && indexPath.Item >= fromIndexPath.Item)
					{
						layoutAttributes.IndexPath = NSIndexPath.FromItemSection(indexPath.Item + 1, indexPath.Section);
					}
					else if (indexPath.Section == toIndexPath.Section && indexPath.Item >= toIndexPath.Item)
					{
						layoutAttributes.IndexPath = NSIndexPath.FromItemSection(indexPath.Item - 1, indexPath.Section);
					}

				}
				else if (indexPath.Section == fromIndexPath.Section)
				{
					if (indexPath.Item <= fromIndexPath.Item && indexPath.Item > toIndexPath.Item)
					{
						layoutAttributes.IndexPath = NSIndexPath.FromItemSection(indexPath.Item - 1, indexPath.Section);
					}
					else if (indexPath.Item >= fromIndexPath.Item && indexPath.Item < toIndexPath.Item)
					{
						layoutAttributes.IndexPath = NSIndexPath.FromItemSection(indexPath.Item + 1, indexPath.Section);
					}

				}

			}
			return elements;
		}

	}
}
