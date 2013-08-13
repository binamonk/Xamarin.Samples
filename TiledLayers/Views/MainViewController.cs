using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace TiledLayers.Views
{

    public class MainViewController : UIViewController
    {
        UIScrollView scrollView;
        TilingView tilingView;
        UIImageView imageView;

        public MainViewController() 
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // scrollView setup
            scrollView = new UIScrollView(new RectangleF(0,0,View.Frame.Width, View.Frame.Height));
            scrollView.BackgroundColor = UIColor.Gray;
            View.AddSubview(scrollView);

            // Autoresize the scroll view to the device orientation
            scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

            // imageView Setup
            var tmpImage = UIImage.FromFile("Image/PlaceholderImages/SLC.jpg");

            imageView = new UIImageView(new RectangleF(0,0,
                                                       1250, 1017));
            imageView.Image = tmpImage;

            // tilingView Setup
            tilingView = new TilingView("SLC",imageView.Bounds.Size);
            imageView.AddSubview(tilingView);
            scrollView.AddSubview(imageView);
            scrollView.SetNeedsLayout();

            scrollView.ViewForZoomingInScrollView += (UIScrollView sv) => { 
                return imageView; 
            };


            // set zooming max min and scale, 
            // IMPORTANT: The tiling will only work within the max and min zoom levels
            // if you set a max of 2 and a min of 1 you will only get x1 and x2 zoom 
            // and the others will not be used
            scrollView.MaximumZoomScale = 5f;
            scrollView.MinimumZoomScale = 0.1f;               
            scrollView.SetZoomScale(1f, false);

            // configure a double-tap gesture recognizer
            UITapGestureRecognizer doubletap = new UITapGestureRecognizer();
            doubletap.NumberOfTapsRequired = 2;
            doubletap.AddTarget (this, new MonoTouch.ObjCRuntime.Selector("DoubleTapSelector"));
            scrollView.AddGestureRecognizer(doubletap);

        }

        // implement the double-tap handler
        [MonoTouch.Foundation.Export("DoubleTapSelector")]
        public void OnDoubleTap (UIGestureRecognizer sender) {
            if (scrollView.ZoomScale >= 1.5)
            {
                scrollView.SetZoomScale(1f, true);
            }
            else
            {
                scrollView.SetZoomScale(2f, true);
            }
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return true;
        }
    }
}

