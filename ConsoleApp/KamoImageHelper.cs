using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ConsoleApp
{
    public unsafe class KamoImageHelper
    {
        #region 亮度、对比度调整
        /// <summary>
        /// 图像明暗调整
        /// </summary>
        /// <param name="filePath">原始图</param>
        /// <param name="degree">亮度[-255, 255]</param>
        /// <returns></returns>
        public static Bitmap AdjustLighten(string filePath, int degree)
        {
            var image = new Bitmap(filePath);
            return AdjustLighten(image, degree);
        }
        /// <summary>
        /// 图像明暗调整
        /// </summary>
        /// <param name="b">原始图</param>
        /// <param name="degree">亮度[-255, 255]</param>
        /// <returns></returns>
        public static Bitmap AdjustLighten(Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }
            if (degree < -255) degree = -255;
            if (degree > 255) degree = 255;
            try
            {
                int width = b.Width;
                int height = b.Height;
                int pix = 0;
                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* p = (byte*)data.Scan0;
                    int offset = data.Stride - width * 3;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // 处理指定位置像素的亮度
                            for (int i = 0; i < 3; i++)
                            {
                                pix = p[i] + degree;
                                if (degree < 0) p[i] = (byte)Math.Max(0, pix);
                                if (degree > 0) p[i] = (byte)Math.Min(255, pix);

                            } // i
                            p += 3;
                        } // x
                        p += offset;
                    } // y
                }
                b.UnlockBits(data);
                return b;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 图像对比度调整
        /// </summary>
        /// <param name="filePath">原始图</param>
        /// <param name="degree">对比度[-100, 100]</param>
        /// <returns></returns>
        public static Bitmap AdjustContrast(string filePath, int degree)
        {
            var image = new Bitmap(filePath);
            return AdjustContrast(image, degree);
        }
        /// <summary>
        /// 图像对比度调整
        /// </summary>
        /// <param name="b">原始图</param>
        /// <param name="degree">对比度[-100, 100]</param>
        /// <returns></returns>
        public static Bitmap AdjustContrast(Bitmap b, int degree)
        {
            if (b == null)
            {
                return null;
            }
            if (degree < -100) degree = -100;
            if (degree > 100) degree = 100;
            try
            {
                double pixel = 0;
                double contrast = (100.0 + degree) / 100.0;
                contrast *= contrast;
                int width = b.Width;
                int height = b.Height;
                BitmapData data = b.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                unsafe
                {
                    byte* p = (byte*)data.Scan0;
                    int offset = data.Stride - width * 3;
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            // 处理指定位置像素的对比度
                            for (int i = 0; i < 3; i++)
                            {
                                pixel = ((p[i] / 255.0 - 0.5) * contrast + 0.5) * 255;
                                if (pixel < 0) pixel = 0;
                                if (pixel > 255) pixel = 255;
                                p[i] = (byte)pixel;
                            } // i
                            p += 3;
                        } // x
                        p += offset;
                    } // y
                }
                b.UnlockBits(data);
                return b;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 图片颜色调整
        private static Bitmap CreateGrayBitmap(int Width, int Height, PixelFormat format)
        {
            //创建8位深度的灰度图像
            Bitmap Bmp = new Bitmap(Width, Height, format);
            ColorPalette Pal = Bmp.Palette;
            for (int Y = 0; Y < Pal.Entries.Length; Y++)
            {
                Pal.Entries[Y] = Color.FromArgb(255, Y, Y, Y);//将RGB转化为灰度调色板
            }
            Bmp.Palette = Pal;

            return Bmp;
        }
        private static Bitmap ConvertToGrayBitmap(Bitmap Src)
        {
            Bitmap Dest = CreateGrayBitmap(Src.Width, Src.Height, PixelFormat.Format24bppRgb);
            BitmapData SrcData = Src.LockBits(new Rectangle(0, 0, Src.Width, Src.Height), ImageLockMode.ReadWrite, Src.PixelFormat);//这个我还没搞懂具体的作用
            BitmapData DestData = Dest.LockBits(new Rectangle(0, 0, Dest.Width, Dest.Height), ImageLockMode.ReadWrite, Dest.PixelFormat);
            int Width = SrcData.Width, Height = SrcData.Height;
            int SrcStride = SrcData.Stride, DestStride = DestData.Stride;
            byte* srcP, destP;//c#使用指针的时候必须开启unsafe功能，下面有开启unsafe功能的方法
            for (int Y = 0; Y < Height; Y++)
            {
                srcP = (byte*)SrcData.Scan0 + Y * SrcStride; // 必须在某个地方开启unsafe功能，其实C#中的unsafe很safe，搞的好吓人。
                destP = (byte*)DestData.Scan0 + Y * DestStride;
                for (int X = 0; X < Width; X++)
                {
                    destP = (byte*)((*srcP + (*(srcP + 1) << 1) + *(srcP + 2)) >> 2);//将彩色图像转化为灰度图像的变换公式
                    srcP += 3;
                    destP++;
                }
            }

            Src.UnlockBits(SrcData);
            Dest.UnlockBits(DestData);//解锁

            return Dest;
        }
        public static Bitmap MakeBrownScale(string filePath)
        {
            var image = new Bitmap(filePath);
            ColorMatrix brownMatrix = new ColorMatrix(
               new float[][]
                  {
                     new float[] { 1.67f, .3f, .3f, 0, 0},
                     new float[] {.2f, .76f, .59f, 0, 0},
                     new float[] {.11f, .11f, .01f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });
            return MakeScale(image, brownMatrix);
        }
        public static Bitmap MakeBrownScale(Image image)
        {
            ColorMatrix brownMatrix = new ColorMatrix(
               new float[][]
                  {
                     new float[] { 1.67f, .3f, .3f, 0, 0},
                     new float[] {.2f, .76f, .59f, 0, 0},
                     new float[] {.11f, .11f, .01f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });
            return MakeScale(image, brownMatrix);
        }

        public static Bitmap MakeGrayScale(string filePath)
        {
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                  {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });
            return MakeScale(filePath, colorMatrix);
        }
        public static Bitmap MakeGrayScale(Image image)
        {
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
                  {
                     new float[] {.3f, .3f, .3f, 0, 0},
                     new float[] {.59f, .59f, .59f, 0, 0},
                     new float[] {.11f, .11f, .11f, 0, 0},
                     new float[] {0, 0, 0, 1, 0},
                     new float[] {0, 0, 0, 0, 1}
                  });
            return MakeScale(image, colorMatrix);
        }

        private static Bitmap MakeScale(string filePath, ColorMatrix colorMatrix)
        {
            var image = new Bitmap(filePath);
            return MakeScale(image, colorMatrix);
        }
        private static Bitmap MakeScale(Image image, ColorMatrix colorMatrix)
        {
            // source: http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale

            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(image.Width, image.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
               0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            attributes.Dispose();

            return newBitmap;
        }
        #endregion

        #region 生成缩略图
        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <returns></returns>
        public static string MakeThumbnail(string originalImagePath, int width = 100)
        {
            return MakeThumbnail(originalImagePath, width, width, ThumbnailMode.Width);
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">源图路径（物理路径）</param>
        /// <param name="width">缩略图宽度</param>
        /// <param name="height">缩略图高度</param>
        /// <param name="mode">生成缩略图的方式</param> 
        /// <returns></returns>
        public static string MakeThumbnail(string originalImagePath, int width, int height, ThumbnailMode mode)
        {
            var dir = Path.GetDirectoryName(originalImagePath);
            var fileName = Path.GetFileNameWithoutExtension(originalImagePath);
            var savePath = Path.Combine(dir, $"{fileName}_{width}_{height}.png").Replace(@"\", "//");

            Image originalImage = Image.FromFile(originalImagePath);

            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ThumbnailMode.WidthHeight:
                    break;
                case ThumbnailMode.Width:
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ThumbnailMode.Height:
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ThumbnailMode.Cut:
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Image bitmap = new Bitmap(towidth, toheight);

            //新建一个画板
            Graphics g = Graphics.FromImage(bitmap);

            //设置高质量插值法
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量,低速度呈现平滑程度
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充
            g.Clear(Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分
            g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight),
                new Rectangle(x, y, ow, oh),
                GraphicsUnit.Pixel);

            try
            {
                //以 png 格式保存缩略图
                bitmap.Save(savePath, ImageFormat.Png);
                return savePath;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        public enum ThumbnailMode
        {
            /// <summary>
            /// 指定高宽缩放（可能变形）
            /// </summary>
            WidthHeight = 0,

            /// <summary>
            /// 指定宽，高按比例
            /// </summary>
            Width = 1,

            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            Height = 2,

            /// <summary>
            /// 指定高宽裁减（不变形）
            /// </summary>
            Cut = 3,
        }
        #endregion
    }
}
/*
        public static ICollection<string> SaveBinaryFile(string convertFilePath, string taskCode, out int newFileSize)
        {
            var result = new List<string>();
            var fsReader = new FileStream(convertFilePath, FileMode.Open, FileAccess.Read);
            var bReader = new BinaryReader(fsReader);
            try
            {
                var fileSize = fsReader.Length;
                newFileSize = (int)fileSize;
                int pkgNum = 1;
                var buffer = new byte[LhpConsts.EveryImageCacheSize];
                while (true)
                {
                    var readCount = bReader.Read(buffer, 0, buffer.Length);
                    var fileName = taskCode + LhpConsts.ImageFileDelimiter + pkgNum;
                    var fileNamePath = Path.Combine(Path.GetDirectoryName(convertFilePath), fileName);
                    using (var filWriter = new FileStream(fileNamePath, FileMode.CreateNew))
                    {
                        using (var w = new BinaryWriter(filWriter))
                        {
                            w.Write(buffer, 0, readCount);
                        }
                    }
                    result.Add(fileNamePath);
                    if (readCount < buffer.Length)
                    {
                        break;  //结束循环
                    }
                    pkgNum++;
                }
            }
            finally
            {
                bReader.Close();
                fsReader.Close();
            }
            return result;
        }

        public static string HanderImagePixelFormat(string filePath)
        {
            var image = Image.FromFile(filePath);

            if (image.PixelFormat == PixelFormat.Format24bppRgb || image.PixelFormat == PixelFormat.Format32bppRgb)
            {
                return filePath;
            }

            var convertBitMap = ConvertTo24bpp(image, PixelFormat.Format24bppRgb);

            var fileDir = Path.GetDirectoryName(filePath);
            var fileNewName = Path.GetFileNameWithoutExtension(filePath) + "_24bpp.png";
            var newFilePath = Path.Combine(fileDir, fileNewName);
            convertBitMap.Save(newFilePath);
            convertBitMap.Dispose();
            image.Dispose();
            return newFilePath;
        }

        public static Bitmap ConvertTo24bpp(Image img, PixelFormat format)
        {
            var bmp = new Bitmap(img.Width, img.Height, format);
            using (var gr = Graphics.FromImage(bmp))
                gr.DrawImage(img, new Rectangle(0, 0, img.Width, img.Height));
            return bmp;
        }
 */
