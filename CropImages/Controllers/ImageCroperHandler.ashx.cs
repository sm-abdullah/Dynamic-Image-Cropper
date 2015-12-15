using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace CropImages.Controllers
{
    /// <summary>
    /// Create a Generic handler which will process all the CropeImage Reuqests
    /// </summary>
    public class ImageCroperRoutHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            ImageCroperHttpHandler imageCroperHandler = new ImageCroperHttpHandler();
            return imageCroperHandler;
        }
        /// <summary>
        /// Summary description for ImageCroperHandler
        /// </summary>
        private class ImageCroperHttpHandler : IHttpHandler
        {

           
            public bool IsReusable
            {
                get
                {
                    return false;
                }
            }
            public void ProcessRequest(HttpContext context)
            {
                
                try
                {
                    //Get Rout Parameters from Request
                    var routeValues = context.Request.RequestContext.RouteData.Values;

                    //First of All get the Image Name from the URL
                    string imagePath = context.Request.RequestContext.RouteData.Values["imgName"].ToString();

                    //get the physicall path of the images which are stored in Images Directory located at the root of the application
                    imagePath = Path.Combine(HttpContext.Current.Server.MapPath("~/Images"), imagePath);
                 
                    //Get the Width Parameter from URL
                    var temp = context.Request.RequestContext.RouteData.Values["w"].ToString();
                    //parse Width of image 
                    int w = int.Parse(temp);

                    //Get the Height Parameter from URL
                    temp = context.Request.RequestContext.RouteData.Values["h"].ToString();
                    int h = int.Parse(temp);

                    temp = context.Request.RequestContext.RouteData.Values["x"].ToString();
                    int x = int.Parse(temp);

                    temp = context.Request.RequestContext.RouteData.Values["y"].ToString();
                    int y = int.Parse(temp);

                    //Now Crop the image and get Bytes Array
                    byte[] CropImage = CropImages(imagePath, w, h, x, y);
                  
                    //set the Content Type to Images
                    context.Response.ContentType = "image/jpeg";

                    //Now Write the Bytes in Response
                    context.Response.BinaryWrite(CropImage);
                    //End the response
                    context.Response.End();
                }
                catch (Exception exception)
                {
                    //if there is an error it will return you image with exception message
                    //you can handle it by your way
                    context.Response.ContentType = "image/jpeg";
                    Font font = new Font("Arial", 12);
                    byte[] img = DrawText(exception.Message, font, Color.White, Color.Black);
                    context.Response.BinaryWrite(img);
                    context.Response.End();
                }
            }
            /// <summary>
            /// this function will take the ImagePath and crop it returns the bytes array
            /// </summary>
            /// <param name="imagePath">Physicall Image Path</param>
            /// <param name="Width">Widht of the Image</param>
            /// <param name="Height">Height of the Image</param>
            /// <param name="X">Start Croping from Left</param>
            /// <param name="Y">Start Croping from Top</param>
            /// <returns></returns>
            private  byte[] CropImages(string imagePath, int Width, int Height, int X, int Y)
            {

               
                    //get Image from Path
                    using (Image orignalImage = Image.FromFile(imagePath))
                    {
                        //Create a bitmap as well.
                        using (Bitmap bitmap = new Bitmap(Width, Height))
                        {
                            //set the resolution of the bitmap
                            bitmap.SetResolution(orignalImage.HorizontalResolution, orignalImage.VerticalResolution);

                            //get Graphics
                            using (Graphics graphics = Graphics.FromImage(bitmap))
                            {

                                graphics.SmoothingMode = SmoothingMode.AntiAlias;

                                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                //Draw image from orignalImage to Bitmap
                                graphics.DrawImage(orignalImage, new Rectangle(0, 0, Width, Height), X, Y, Width, Height, GraphicsUnit.Pixel);

                                MemoryStream imageStream = new MemoryStream();
                                //save bitmap into memory stream
                                bitmap.Save(imageStream, orignalImage.RawFormat);
                                //return byte buffer
                                return imageStream.GetBuffer();
                            }

                        }

                    }

               

            }
            private  byte[] DrawText(String text, Font font, Color textColor, Color backColor)
            {
                //first, create a dummy bitmap just to get a graphics object
                Image image = new Bitmap(1, 1);
                Graphics drawing = Graphics.FromImage(image);

                //measure the string to see how big the image needs to be
                SizeF textSize = drawing.MeasureString(text, font);

                //free up the dummy image and old graphics object
                image.Dispose();
                drawing.Dispose();

                //create a new image of the right size
                image = new Bitmap((int)textSize.Width, (int)textSize.Height);

                drawing = Graphics.FromImage(image);

                //paint the background
                drawing.Clear(backColor);

                //create a brush for the text
                Brush textBrush = new SolidBrush(textColor);

                drawing.DrawString(text, font, textBrush, 0, 0);

                drawing.Save();

                textBrush.Dispose();
                drawing.Dispose();

                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Jpeg);

                return ms.GetBuffer();


            }

        }


    }
}