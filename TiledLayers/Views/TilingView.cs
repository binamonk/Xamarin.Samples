using System;
using System.Drawing;

using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace TiledLayers.Views
{
    public class TilingView : UIView
    {
        [Export ("layerClass")]
        public static Class LayerClass ()
        {    
            return new Class (typeof (CATiledLayer));
        }

        string ImageName { get; set; }

        public TilingView (string name, SizeF size) : 
            base (new RectangleF (PointF.Empty, size))
        {
            ImageName = name;
            var tiledLayer = (CATiledLayer) this.Layer; 
            // This means that we have 7 zoom levels
            tiledLayer.LevelsOfDetail = 7;
            // This means that the level 4 is the 1x level (1000) and it has 3 zoom levels x2(2000), x4(4000) and x8(8000)
            // you can set LevelsOfDetail to 4 and you will loose the zoom out.
            tiledLayer.LevelsOfDetailBias = 3;
            // This sets the tile size
            tiledLayer.TileSize = new SizeF(256, 256);


        }

        // to handle the interaction between CATiledLayer and high resolution screens, we need to always keep the
        // tiling view's contentScaleFactor at 1.0. UIKit will try to set it back to 2.0 on retina displays, which is the
        // right call in most cases, but since we're backed by a CATiledLayer it will actually cause us to load the
        // wrong sized tiles.

        public override float ContentScaleFactor {
            set {
                base.ContentScaleFactor = 1f;
            }
        }   

        public override void Draw (RectangleF rect)
        {
            var context = UIGraphics.GetCurrentContext ();
            // get the scale from the context by getting the current transform matrix, then asking for
            // its "a" component, which is one of the two scale components. We could also ask for "d".
            // This assumes (safely) that the view is being scaled equally in both dimensions.
            var scale = (float)Math.Round(context.GetCTM ().xx * 1000)/1000;
            CATiledLayer tiledLayer = (CATiledLayer) this.Layer; 
            var tileSize = tiledLayer.TileSize;

            // Even at scales lower than 100%, we are drawing into a rect in the coordinate system of the full
            // image. One tile at 50% covers the width (in original image coordinates) of two tiles at 100%. 
            // So at 50% we need to stretch our tiles to double the width and height; at 25% we need to stretch 
            // them to quadruple the width and height; and so on.
            // (Note that this means that we are drawing very blurry images as the scale gets low. At 12.5%, 
            // our lowest scale, we are stretching about 6 small tiles to fill the entire original image area. 
            // But this is okay, because the big blurry image we're drawing here will be scaled way down before 
            // it is displayed.)
            tileSize.Width /= scale;
            tileSize.Height /= scale;

            // calculate the rows and columns of tiles that intersect the rect we have been asked to draw
            int firstCol = (int) Math.Floor(rect.GetMinX () / tileSize.Width);
            int lastCol = (int) Math.Floor((rect.GetMaxX () - 1) / tileSize.Width);
            int firstRow = (int) Math.Floor(rect.GetMinY () / tileSize.Height);
            int lastRow = (int) Math.Floor((rect.GetMaxY () - 1) / tileSize.Height);


            for (int row = firstRow; row <= lastRow; row++) {
                for (int col = firstCol; col <= lastCol; col++) {

                    UIImage tile = TileForScale (scale, row, col);
                    if (tile != null)
                    {
                        var tileRect = new RectangleF(tileSize.Width * col, tileSize.Height * row, tileSize.Width, tileSize.Height);
                        // if the tile would stick outside of our bounds, we need to truncate it so as to avoid
                        // stretching out the partial tiles at the right and bottom edges
                        tileRect.Intersect(this.Bounds);
                        tile.Draw(tileRect);
                    }
                    else
                    {
                        // We requested an unexistent tile
                        Console.WriteLine("Tile outside of bounds");
                    }
                }
            }
        }

        public UIImage TileForScale (float scale, int row, int col)
        {
            // we use "FromFile" instead of "FromBundle" here because we don't want UIImage to cache our tiles
            string path = String.Format ("Image/ImageTiles/{0}_{1}_{2}_{3}.jpg", ImageName, (int)(Math.Round(scale * 1000)), (int)col, (int)row);
            return UIImage.FromFile (path);
        }
    }
}