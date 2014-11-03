using System;
using TimeInterval = System.Double;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.ObjCRuntime;
//using MonoTouch.CoreGraphics.RectangleFExtensions;
using MonoTouch.CoreGraphics;
using System.Collections;

namespace XamarinControls
{
	public enum ScrollingDirection {
		Unknown = 0,
		Up,
		Down,
		Left,
		Right
	};
	public class LSCollectionViewHelper : UIGestureRecognizerDelegate
	{
//		public LSCollectionViewHelper ()
//		{
//		}
		protected NSIndexPath lastIndexPath;
		protected UIImageView mockCell;
		protected PointF mockCenter;
		protected PointF fingerTranslation;
		protected CADisplayLink timer;
		protected ScrollingDirection scrollingDirection;
		protected bool canWarp;
		protected bool canScroll;
		protected bool _hasShouldAlterTranslationDelegateMethod;
		protected PointF _dropOnToDeleteViewCenter;
		static int kObservingCollectionViewLayoutContext;
		static int kObservingCollectionViewOffset;
		public UICollectionView CollectionView {get; private set;}

		public UIGestureRecognizer LongPressGestureRecognizer {get; private set;}

		public UIGestureRecognizer PanPressGestureRecognizer {get; private set;}

		public UIEdgeInsets ScrollingEdgeInsets {get; set;}

		public float ScrollingSpeed {get; set;}

		bool _enabled;
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
//				this.LongPressGestureRecognizer.Value = canWarp && value;
//				this.PanPressGestureRecognizer.Value = canWarp && value;
			}
		}

		public NSIndexPath IndexPathForMovingItem
		{
			get
			{
				if (!(mockCell != null)) return null;

				return this.LayoutHelper.FromIndexPath;
			}
		}

		UIImageView _dropOnToDeleteView;
		public UIImageView DropOnToDeleteView
		{
			get
			{
				return _dropOnToDeleteView;
			}
			set
			{
				_dropOnToDeleteView = value;
				_dropOnToDeleteViewCenter = value.Center;
			}
		}

		public LSCollectionViewLayoutHelper LayoutHelper
		{
			get
			{
				return ((IUICollectionViewLayoutWarpable)this.CollectionView.CollectionViewLayout).LayoutHelper;
			}
		}

		public LSCollectionViewHelper(UICollectionView collectionView)
		{
			this.CollectionView = collectionView;
			this.CollectionView.AddObserver(this, "collectionViewLayout", 0,new IntPtr(kObservingCollectionViewLayoutContext));
			this.CollectionView.AddObserver(this, "contentOffset", 0, new IntPtr(kObservingCollectionViewOffset));
			this.ScrollingEdgeInsets = new UIEdgeInsets(50.0f, 50.0f, 50.0f, 50.0f);
			this.ScrollingSpeed = 300f;
			this.LongPressGestureRecognizer = new UILongPressGestureRecognizer(this, new Selector ("handleLongPressGesture:"));
			this.CollectionView.AddGestureRecognizer(this.LongPressGestureRecognizer);
			this.PanPressGestureRecognizer = new UIPanGestureRecognizer(this, new Selector ("handlePanGesture:"));
			this.PanPressGestureRecognizer.Delegate = this;
			this.CollectionView.AddGestureRecognizer(this.PanPressGestureRecognizer);
			this.LayoutChanged();
		}

		public void UnbindFromCollectionView(UICollectionView collectionView)
		{
			collectionView.RemoveObserver(this, "collectionViewLayout");
			collectionView.RemoveObserver(this, "contentOffset");
		}

		void LayoutChanged()
		{
			canWarp = this.CollectionView.CollectionViewLayout.ConformsToProtocol(Runtime.GetProtocol ("UICollectionViewLayout"));
			canScroll = this.CollectionView.CollectionViewLayout.RespondsToSelector(new Selector("scrollDirection"));
			this.LongPressGestureRecognizer.Enabled = this.PanPressGestureRecognizer.Enabled = canWarp && this.Enabled;
		}

		void ObserveValueForKeyPathOfObjectChangeContext(string keyPath, NSObject theObject, NSDictionary change, IntPtr context)
		{
			if (context == new IntPtr(kObservingCollectionViewOffset))
			{
				if (this.DropOnToDeleteView.Superview != null)
				{
					RectangleF bounds = this.CollectionView.Bounds;
					this.DropOnToDeleteView.Center = new PointF(bounds.GetMinX() + _dropOnToDeleteViewCenter.X, bounds.GetMinY() + _dropOnToDeleteViewCenter.Y);
				}

			}
			else if (context == new IntPtr(kObservingCollectionViewLayoutContext))
			{
				this.LayoutChanged();
			}
			else
			{
				base.ObserveValue(new NSString(keyPath), theObject, change, context);
			}

		}

		UIImage ImageFromCell(UICollectionViewCell cell)
		{
			UIGraphics.BeginImageContextWithOptions(cell.Bounds.Size, false, 0);
			cell.Layer.RenderInContext(UIGraphics.GetCurrentContext());
			UIImage image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return image;
		}

		void InvalidatesScrollTimer()
		{
			if (timer != null)
			{
				timer.Invalidate();
				timer = null;
			}

			scrollingDirection = ScrollingDirection.Unknown;
		}

		void SetupScrollTimerInDirection(ScrollingDirection direction)
		{
			scrollingDirection = direction;
			if (timer == null)
			{
				timer = CADisplayLink.Create(this, new Selector ("handleScroll:"));
				timer.AddToRunLoop(NSRunLoop.Main, NSRunLoop.NSDefaultRunLoopMode);
			}

		}

		bool GestureRecognizerShouldBegin(UIGestureRecognizer gestureRecognizer)
		{
			if (gestureRecognizer.Equals(this.PanPressGestureRecognizer))
			{
				return this.LayoutHelper.FromIndexPath != null;
			}

			return true;
		}

		bool GestureRecognizerShouldRecognizeSimultaneouslyWithGestureRecognizer(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
		{
			if (gestureRecognizer.Equals(this.LongPressGestureRecognizer))
			{
				return otherGestureRecognizer.Equals(this.PanPressGestureRecognizer);
			}

			if (gestureRecognizer.Equals(this.PanPressGestureRecognizer))
			{
				return otherGestureRecognizer.Equals(this.LongPressGestureRecognizer);
			}

			return false;
		}

		NSIndexPath IndexPathForItemClosestToPoint(PointF point)
		{
			ArrayList layoutAttrsInRect;
			int closestDist = int.MaxValue;
			NSIndexPath indexPath= new NSIndexPath();
			NSIndexPath toIndexPath = this.LayoutHelper.ToIndexPath;
			this.LayoutHelper.ToIndexPath = null;
			layoutAttrsInRect = new ArrayList( this.CollectionView.CollectionViewLayout.LayoutAttributesForElementsInRect(this.CollectionView.Bounds));
			this.LayoutHelper.ToIndexPath = toIndexPath;
			foreach (UICollectionViewLayoutAttributes layoutAttr in layoutAttrsInRect)
			{
				if (layoutAttr.RepresentedElementCategory == UICollectionElementCategory.Cell)
				{
					if (layoutAttr.Frame.Contains( point))
					{
						closestDist = 0;
						indexPath = layoutAttr.IndexPath;
					}
					else
					{
						float xd = layoutAttr.Center.X - point.X;
						float yd = layoutAttr.Center.Y - point.Y;
						float dist = (float)Math.Sqrt(xd * xd + yd * yd);
						if (dist < closestDist)
						{
							closestDist = (int)dist;
							indexPath = layoutAttr.IndexPath;
						}

					}

				}

			}
			int sections = this.CollectionView.NumberOfSections();
			for (int i = 0; i < sections; ++i)
			{
				if (i == this.LayoutHelper.FromIndexPath.Section)
				{
					continue;
				}

				int items = this.CollectionView.NumberOfItemsInSection(i);
				NSIndexPath nextIndexPath = NSIndexPath.FromItemSection(items - 1, i);
				UICollectionViewLayoutAttributes layoutAttr;
				float xd, yd;
				if (items > 0)
				{
					layoutAttr = this.CollectionView.CollectionViewLayout.LayoutAttributesForItem(nextIndexPath);
					xd = layoutAttr.Center.X - point.X;
					yd = layoutAttr.Center.Y - point.Y;
				}
				else
				{
					layoutAttr = this.CollectionView.CollectionViewLayout.LayoutAttributesForSupplementaryView(UICollectionElementKindSection.Header, nextIndexPath);
					xd = layoutAttr.Frame.X - point.X;
					yd = layoutAttr.Frame.Y - point.Y;
				}

				float dist = (float)Math.Sqrt(xd * xd + yd * yd);
				if (dist < closestDist)
				{
					closestDist = (int)dist;
					indexPath = layoutAttr.IndexPath;
				}

			}

			return indexPath;
		}

		void HandleLongPressGesture(UILongPressGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Changed)
			{
				return;
			}

			if (!this.CollectionView.DataSource.ConformsToProtocol(Runtime.GetProtocol ("UICollectionViewDataSource")))
			{
				return;
			}

			_hasShouldAlterTranslationDelegateMethod = this.CollectionView.DataSource.RespondsToSelector(new Selector ("collectionView:alterTranslation:"));
			PointF point = sender.LocationInView(this.CollectionView);
			NSIndexPath indexPath = this.IndexPathForItemClosestToPoint(point);
			switch (sender.State)
			{
			case UIGestureRecognizerState.Began :

				{
					if (indexPath == null)
					{
						return;
					}

					if (!((UICollectionViewDataSourceDraggable)this.CollectionView.DataSource).CollectionViewCanMoveItemAtIndexPath(this.CollectionView, indexPath))
					{
						return;
					}

					UICollectionViewCell cell = this.CollectionView.CellForItem(indexPath);
					cell.Highlighted = false;
					mockCell.RemoveFromSuperview();
					mockCell = new UIImageView(cell.Frame);
					mockCell.Alpha = 0.8f;
					if (this.CollectionView.DataSource.RespondsToSelector(new Selector ("collectionView:imageForDraggingItemAtIndexPath:")))
					{
						mockCell.Image = ((UICollectionViewDataSourceDraggable)this.CollectionView.DataSource).CollectionViewImageForDraggingItemAtIndexPath(this.CollectionView, indexPath);
					}

					if (mockCell.Image == null)
					{
						mockCell.Image = this.ImageFromCell(cell);
					}

					mockCell.SizeToFit();
					mockCell.Layer.MasksToBounds = false;
					mockCell.Layer.ShadowColor = UIColor.Black.CGColor;
					mockCell.Layer.ShadowOpacity = (0.65f);
					mockCell.Layer.ShadowRadius = (10.0f);
					mockCell.Layer.ShadowOffset = (new SizeF(0, 0));
					mockCell.Layer.ShadowPath = (UIBezierPath.FromRect(mockCell.Bounds).CGPath);
					mockCenter = mockCell.Center;
					this.CollectionView.AddSubview(mockCell);
					if (this.DropOnToDeleteView != null)
					{
						bool canDelete = false;
						if (this.CollectionView.DataSource.RespondsToSelector(new Selector ("collectionView:canDeleteItemAtIndexPath:")))
						{
							canDelete = ((UICollectionViewDataSourceDraggable)this.CollectionView.DataSource).CollectionViewCanDeleteItemAtIndexPath(this.CollectionView, indexPath);
						}

						if (canDelete)
						{
							RectangleF bounds = this.CollectionView.Bounds;
							this.DropOnToDeleteView.Center = new PointF( bounds.GetMinX() + _dropOnToDeleteViewCenter.X, bounds.GetMinY() + _dropOnToDeleteViewCenter.Y);
							this.DropOnToDeleteView.Highlighted = false;
							this.CollectionView.AddSubview(this.DropOnToDeleteView);
							this.DropOnToDeleteView.Alpha = 0.2f;
						}

					}

					if (this.CollectionView.DataSource.RespondsToSelector( new Selector ("collectionView:transformForDraggingItemAtIndexPath:duration:")))
					{
						TimeInterval duration = 0.3;
						CGAffineTransform transform = ((UICollectionViewDataSourceDraggable)this.CollectionView.DataSource).CollectionViewTransformForDraggingItemAtIndexPathDuration(this.CollectionView, indexPath, duration);
						UIView.Animate(duration, delegate()
							{
								mockCell.Transform = transform;
							}, null);
					}

					lastIndexPath = indexPath;
					this.LayoutHelper.FromIndexPath = indexPath;
					this.LayoutHelper.HideIndexPath = indexPath;
					this.LayoutHelper.ToIndexPath = indexPath;
					this.CollectionView.CollectionViewLayout.InvalidateLayout();
				}
				break;
			case UIGestureRecognizerState.Ended :
			case UIGestureRecognizerState.Cancelled :

				{
					if (this.LayoutHelper.FromIndexPath == null)
					{
						return;
					}

					NSIndexPath fromIndexPath = this.LayoutHelper.FromIndexPath;
					NSIndexPath toIndexPath = this.LayoutHelper.ToIndexPath;
					UICollectionViewDataSourceDraggable dataSource = (UICollectionViewDataSourceDraggable)this.CollectionView.DataSource;
					bool toDelete = sender.State == UIGestureRecognizerState.Ended && this.DropOnToDeleteView.Highlighted;
					if (toDelete)
					{
						this.CollectionView.PerformBatchUpdates(delegate()
							{
								dataSource.CollectionViewDeleteItemAtIndexPath(this.CollectionView, fromIndexPath);
								this.CollectionView.DeleteItems(new NSIndexPath[]{fromIndexPath});
								this.LayoutHelper.FromIndexPath = null;
								this.LayoutHelper.ToIndexPath = null;
							}, delegate(bool finished)
							{
								if (((NSObject)dataSource).RespondsToSelector(new Selector ("collectionView:didDeleteItemAtIndexPath:")))
								{
									dataSource.CollectionViewDidDeleteItemAtIndexPath(this.CollectionView, fromIndexPath);
								}

							});
						UIView.Animate(0.3, delegate()
							{
								mockCell.Center = this.DropOnToDeleteView.Center;
								mockCell.Transform = CGAffineTransform.Scale(CGAffineTransform.MakeRotation(10), 0.01f, 0.01f);
								this.DropOnToDeleteView.Alpha = 0.0f;
							}, //delegate(bool finished)
							delegate()
							{
								mockCell.RemoveFromSuperview();
								mockCell = null;
								this.DropOnToDeleteView.RemoveFromSuperview();
								this.DropOnToDeleteView.Alpha = 1.0f;
								this.LayoutHelper.HideIndexPath = null;
								this.CollectionView.CollectionViewLayout.InvalidateLayout();
							});
					}
					else
					{
						this.CollectionView.PerformBatchUpdates(delegate()
							{
								dataSource.CollectionViewMoveItemAtIndexPathToIndexPath(this.CollectionView, fromIndexPath, toIndexPath);
								this.CollectionView.MoveItem(fromIndexPath, toIndexPath);
								this.LayoutHelper.FromIndexPath = null;
								this.LayoutHelper.ToIndexPath = null;
							}, delegate(bool finished)
							{
								if (((NSObject)dataSource).RespondsToSelector(new Selector ("collectionView:didMoveItemAtIndexPath:toIndexPath:")))
								{
									dataSource.CollectionViewDidMoveItemAtIndexPathToIndexPath(this.CollectionView, fromIndexPath, toIndexPath);
								}

							});
						UICollectionViewLayoutAttributes layoutAttributes = this.CollectionView.GetLayoutAttributesForItem(this.LayoutHelper.HideIndexPath);
						UIView.Animate(0.3, delegate()
							{
								RectangleF frame = layoutAttributes.Frame;
								frame.Size = mockCell.Frame.Size;
								mockCell.Frame = frame;
								mockCell.Transform = CGAffineTransform.MakeIdentity();
							}, //delegate(bool finished)
							delegate()
							{
								mockCell.RemoveFromSuperview();
								mockCell = null;
								this.DropOnToDeleteView.RemoveFromSuperview();
								this.LayoutHelper.HideIndexPath = null;
								this.CollectionView.CollectionViewLayout.InvalidateLayout();
							});
					}

					this.InvalidatesScrollTimer();
					lastIndexPath = null;
				}
				break;
			default :
				break;
			}

		}

		void WarpToIndexPath(NSIndexPath indexPath)
		{
			if (indexPath == null || lastIndexPath.Equals(indexPath))
			{
				return;
			}

			lastIndexPath = indexPath;
			if (this.CollectionView.DataSource.RespondsToSelector(new Selector ("collectionView:canMoveItemAtIndexPath:toIndexPath:")) == true && ((UICollectionViewDataSourceDraggable)this.CollectionView.DataSource).CollectionViewCanMoveItemAtIndexPathToIndexPath(this.CollectionView, this.LayoutHelper.FromIndexPath, indexPath) == false)
			{
				return;
			}

			this.CollectionView.PerformBatchUpdates(delegate()
				{
					this.LayoutHelper.HideIndexPath = indexPath;
					this.LayoutHelper.ToIndexPath = indexPath;
				}, null);
		}

		void HandlePanGesture(UIPanGestureRecognizer sender)
		{
			if (sender.State == UIGestureRecognizerState.Changed)
			{
				fingerTranslation = sender.TranslationInView(this.CollectionView);
				if (_hasShouldAlterTranslationDelegateMethod)
				{
					((UICollectionViewDataSourceDraggable)this.CollectionView.DataSource).CollectionViewAlterTranslation(this.CollectionView, fingerTranslation);
				}

				IUICollectionViewLayoutWarpable/*UICollectionViewLayout*/ /*<UICollectionViewLayout_Warpable>*/ layout = (IUICollectionViewLayoutWarpable/*UICollectionViewLayout*/ /*<UICollectionViewLayout_Warpable>*/)this.CollectionView.CollectionViewLayout;
//				if (layout.RespondsToSelector(new Selector ("dragDirection")))
				{
					UICollectionViewScrollDirection direction = (UICollectionViewScrollDirection)layout.DragDirection();
					if (direction == UICollectionViewScrollDirection.Horizontal) fingerTranslation.Y = 0;
					else if (direction == UICollectionViewScrollDirection.Vertical) fingerTranslation.X = 0;

				}

				mockCell.Center = mockCenter.Add( fingerTranslation);
				if (canScroll)
				{
					IUICollectionViewLayoutWarpable/*UICollectionViewLayout*/ /*<UICollectionViewLayout_Warpable>*/ scrollLayout = (IUICollectionViewLayoutWarpable/*UICollectionViewLayout*/ /*<UICollectionViewLayout_Warpable>*/)this.CollectionView.CollectionViewLayout;

					if (scrollLayout.ScrollDirection() == UICollectionViewScrollDirection.Vertical)
					{
						if (mockCell.Center.Y < (this.CollectionView.Bounds.GetMinY() + this.ScrollingEdgeInsets.Top))
						{
							this.SetupScrollTimerInDirection(ScrollingDirection.Up);
						}
						else
						{
							if (mockCell.Center.Y > ( this.CollectionView.Bounds.GetMaxY() - this.ScrollingEdgeInsets.Bottom))
							{
								this.SetupScrollTimerInDirection(ScrollingDirection.Down);
							}
							else
							{
								this.InvalidatesScrollTimer();
							}

						}

					}
					else
					{
						if (mockCell.Center.X < ( this.CollectionView.Bounds.GetMinX() + this.ScrollingEdgeInsets.Left))
						{
							this.SetupScrollTimerInDirection(ScrollingDirection.Left);
						}
						else
						{
							if (mockCell.Center.X > ( this.CollectionView.Bounds.GetMaxX() - this.ScrollingEdgeInsets.Right))
							{
								this.SetupScrollTimerInDirection(ScrollingDirection.Right);
							}
							else
							{
								this.InvalidatesScrollTimer();
							}

						}

					}

				}

				if (scrollingDirection > ScrollingDirection.Unknown)
				{
					return;
				}

				PointF point = sender.LocationInView(this.CollectionView);
				NSIndexPath indexPath = this.IndexPathForItemClosestToPoint(point);
				this.WarpToIndexPath(indexPath);
				if (this.DropOnToDeleteView != null && this.DropOnToDeleteView.Superview != null)
				{
					bool highlighted = this.DropOnToDeleteView.Frame.Contains( point);
					this.DropOnToDeleteView.Highlighted = highlighted;
					if (!highlighted)
					{
						PointF center = this.DropOnToDeleteView.Center;
						float distance = (center.X - point.X) * (center.X - point.X) + (center.Y - point.Y) * (center.Y - point.Y);
						this.DropOnToDeleteView.Alpha = (float)(2f - Math.Atan(distance / 10000.0f)) / 2f;
					}
					else
					{
						this.DropOnToDeleteView.Alpha = 1.0f;
					}

				}

			}

		}

		void HandleScroll(NSTimer timer)
		{
			if (scrollingDirection == ScrollingDirection.Unknown)
			{
				return;
			}

			SizeF frameSize = this.CollectionView.Bounds.Size;
			SizeF contentSize = this.CollectionView.ContentSize;
			PointF contentOffset = this.CollectionView.ContentOffset;
			float distance = this.ScrollingSpeed / 60.0f;
			PointF translation = new PointF( 0 , 0 );
			switch (scrollingDirection)
			{
			case ScrollingDirection.Up :

				{
					distance = -distance;
					if ((contentOffset.Y + distance) <= 0.0f)
					{
						distance = -contentOffset.Y;
					}

					translation = new PointF(0.0f, distance);
				}
				break;
			case ScrollingDirection.Down :

				{
					float maxY = Math.Max(contentSize.Height, frameSize.Height) - frameSize.Height;
					if ((contentOffset.Y + distance) >= maxY)
					{
						distance = maxY - contentOffset.Y;
					}

					translation = new PointF(0.0f, distance);
				}
				break;
			case ScrollingDirection.Left :

				{
					distance = -distance;
					if ((contentOffset.X + distance) <= 0.0f)
					{
						distance = -contentOffset.X;
					}

					translation = new PointF(distance, 0.0f);
				}
				break;
			case ScrollingDirection.Right :

				{
					float maxX = Math.Max(contentSize.Width, frameSize.Width) - frameSize.Width;
					if ((contentOffset.X + distance) >= maxX)
					{
						distance = maxX - contentOffset.X;
					}

					translation = new PointF(distance, 0.0f);
				}
				break;
			default :
				break;
			}

			mockCenter = mockCenter.Add( translation);
			mockCell.Center = mockCenter.Add( fingerTranslation);
			this.CollectionView.ContentOffset = contentOffset.Add( translation);
			NSIndexPath indexPath = this.IndexPathForItemClosestToPoint(mockCell.Center);
			this.WarpToIndexPath(indexPath);
		}

	}
}

