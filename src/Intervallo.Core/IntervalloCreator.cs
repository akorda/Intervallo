using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Intervallo
{
    public class IntervalloCreator
    {
        private static readonly string PathToFfmpeg = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"ffmpeg\x64\ffmpeg.exe");

        public string FinalVideoPath { get; set; }
        public string WorkingPath { get; set; }
        public string AudioFile { get; set; }
        public int VideoWidth { get; set; }
        public int VideoHeight { get; set; }
        public int ImageDuration { get; set; }
        public SubtitleStyle SubtitleStyle { get; set; }

        public IntervalloCreator(string finalVideoPath, string workingPath, string audioFile, int videoWidth, int videoHeight, int imageDuration, SubtitleStyle subtitleStyle)
        {
            this.FinalVideoPath = finalVideoPath;
            this.WorkingPath = workingPath;
            this.AudioFile = audioFile;
            this.VideoWidth = videoWidth;
            this.VideoHeight = videoHeight;
            this.ImageDuration = imageDuration;
            this.SubtitleStyle = subtitleStyle;
        }

        public void Create(IEnumerable<string> imagePaths)
        {
            var videoPaths = new List<string>();
            foreach (var imagePath in imagePaths)
            {
                var videoPartPath = Path.Combine(this.WorkingPath, Path.GetRandomFileName().Replace('.', '_') + ".mp4");
                videoPaths.Add(videoPartPath);
                var converter = new ImageToVideoConverter(this.WorkingPath, videoPartPath, imagePath, this.VideoWidth, this.VideoHeight, this.ImageDuration, this.SubtitleStyle);
                converter.ConvertToVideo();
            }

            var tempVideoPathsFilePath = Path.GetRandomFileName().Replace('.', '_') + ".txt";
            var tempVideoPathsFileFullPath = Path.Combine(this.WorkingPath, tempVideoPathsFilePath);
            File.WriteAllLines(tempVideoPathsFileFullPath, videoPaths.Select(p => p.Substring(this.WorkingPath.Length + 1)).Select(p => $"file '{p}'"));

            var tempMutedVideoPartPath = Path.GetRandomFileName().Replace('.', '_') + ".mp4";
            var tempMutedVideoPartFullPath = Path.Combine(this.WorkingPath, tempMutedVideoPartPath);
            var arguments = string.Join(" ", new[]
            {
                "-y", "-f", "concat", "-i", tempVideoPathsFilePath, "-c", "copy", tempMutedVideoPartPath
            });
            FFMpegUtils.CallFFMpeg(PathToFfmpeg, this.WorkingPath, arguments);
            File.Delete(tempVideoPathsFileFullPath);

            if (!string.IsNullOrEmpty(this.AudioFile))
            {
                arguments = string.Join(" ", new[]
                {
                    "-y", "-i", tempMutedVideoPartPath, "-i", this.AudioFile, "-codec", "copy", "-shortest", this.FinalVideoPath
                });
                FFMpegUtils.CallFFMpeg(PathToFfmpeg, this.WorkingPath, arguments);
                File.Delete(tempMutedVideoPartFullPath);
            }
            else
            {
                File.Move(tempMutedVideoPartFullPath, this.FinalVideoPath);
            }
            
            foreach (var vp in videoPaths)
                File.Delete(vp);
            
        }
    }
}
