using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeScrollView.Views
{
    public class CodeScrollViewViewController : UIViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public CodeScrollViewViewController()
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
			
            // Perform any additional setup after loading the view, typically from a nib.
            var scrollView = new UIScrollView(
                new RectangleF(0, 0, View.Frame.Width, View.Frame.Height)
            );
            scrollView.BackgroundColor = UIColor.Gray;
            scrollView.AutoresizingMask = UIViewAutoresizing.All;
            scrollView.ScrollEnabled = true;

            View.Add(scrollView);

            var image = UIImage.FromFile("Images/sampleImage.jpg");

            var imageView = new UIImageView(
                new RectangleF(0,0, image.Size.Width, image.Size.Height)
            );
            imageView.Image = image;
            scrollView.ContentSize = image.Size;
            scrollView.Add(imageView);

        }

    }
}

