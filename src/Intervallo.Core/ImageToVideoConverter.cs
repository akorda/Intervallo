using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Intervallo
{
    public class ImageToVideoConverter
    {
        private static readonly string PathToFfmpeg = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"ffmpeg\x64\ffmpeg.exe");

        public string WorkingPath { get; set; }
        public string VideoPartPath { get; set; }
        public string ImagePath { get; set; }
        public string Subtitle { get; set; }
        public int VideoWidth { get; set; }
        public int VideoHeight { get; set; }
        public int ImageDuration { get; set; }
        public SubtitleStyle SubtitleStyle { get; set; }

        public ImageToVideoConverter(string workingPath, string videoPartPath, string imagePath, int videoWidth, int videoHeight, int imageDuration, SubtitleStyle subtitleStyle)
        {
            this.WorkingPath = workingPath;
            this.VideoPartPath = videoPartPath;
            this.ImagePath = imagePath;
            this.VideoWidth = videoWidth;
            this.VideoHeight = videoHeight;
            this.ImageDuration = imageDuration;
            this.SubtitleStyle = subtitleStyle;
            this.Subtitle = GetSubtitle(imagePath);
        }

        private static string GetSubtitle(string imagePath)
        {
            return string.Join(" ", Path.GetFileNameWithoutExtension(imagePath).Split(new[] { ' ', '_', '-', '.' })).Trim();
        }

        public void ConvertToVideo()
        {
            var image = Bitmap.FromFile(this.ImagePath);
            var resizedImage = image;
            if (image.Width != this.VideoWidth || image.Height != this.VideoHeight)
            {
                resizedImage = image.GetThumbnailImage(this.VideoWidth, this.VideoHeight, null, IntPtr.Zero);
                image.Dispose();
            }

            using (var g = Graphics.FromImage(resizedImage))
            using (var brush = new SolidBrush(this.SubtitleStyle.Color))
            using (var font = new Font(this.SubtitleStyle.FontFamilyName, this.SubtitleStyle.FontSizeEm))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                var subtitleSize = g.MeasureString(this.Subtitle, font);
                var subtitlePoint = new PointF((this.VideoWidth - subtitleSize.Width) / 2, this.VideoHeight*3/4);
                g.DrawString(this.Subtitle, font, brush, subtitlePoint);
                g.Flush();
            }

            var tempImagePath = Path.Combine(this.WorkingPath, Path.GetRandomFileName().Replace('.', '_') + ".png");
            resizedImage.Save(tempImagePath, ImageFormat.Png);
            resizedImage.Dispose();

            var tempVideoPartPath = Path.Combine(this.WorkingPath, Path.GetRandomFileName().Replace('.', '_') + ".mp4");
            var arguments = string.Join(" ", new[]
            {
                "-loop", "1", "-i", tempImagePath, "-c:v", "libx264",
                "-framerate", "25",
                "-t", this.ImageDuration.ToString(), "-pix_fmt", "yuv420p",
                "-vf", "fade=in:0:25",
                "-y", tempVideoPartPath
            });
            FFMpegUtils.CallFFMpeg(PathToFfmpeg, this.WorkingPath, arguments);
            File.Delete(tempImagePath);

            arguments = string.Join(" ", new[]
            {
                "-i", tempVideoPartPath,
                "-vf", $"fade=out:{this.ImageDuration * 25 - 25}:25",
                "-y", this.VideoPartPath
            });
            FFMpegUtils.CallFFMpeg(PathToFfmpeg, this.WorkingPath, arguments);
            File.Delete(tempVideoPartPath);
        }        
    }
}
