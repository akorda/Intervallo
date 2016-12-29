using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

//based on the work of spanezz -> intervallo
//https://github.com/spanezz/intervallo
//

namespace Intervallo
{
    class Program
    {
        static int Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<CommandLineOptions>(args).MapResult(options =>
            {
                return Run(options);
            },
            errors => -1);
            return result;
        }

        private static int Run(CommandLineOptions options)
        {
            try
            {
                string workingPath;
                var finalVideoPath = options.OutputFile;
                if (string.IsNullOrEmpty(finalVideoPath) || !Path.IsPathRooted(finalVideoPath))
                {
                    workingPath = Directory.GetCurrentDirectory();
                    finalVideoPath = Path.Combine(workingPath, "intervallo.mp4");
                }
                else
                {
                    workingPath = Path.GetDirectoryName(finalVideoPath);
                }
                var audioFile = options.AudioFile;
                var videoWidth = options.VideoWidth;
                var videoHeight = options.VideoHeight;
                var imageDuration = options.ImageDuration;
                var subtitleStyle = new SubtitleStyle
                {
                    FontFamilyName = options.FontFamilyName,
                    FontSizeEm = options.FontSizeEm,
                    Color = Color.FromArgb(options.FontColor)
                };

                //var imagePaths = Directory.GetFiles(@"C:\Projects\Temp\Intervallo\Intervallo.CLI\bin\Debug\video", "*.jpg");
                var imagePaths = new List<string>();
                foreach (var file in options.ImageFiles)
                {
                    if (!file.Contains("*"))
                    {
                        if (File.Exists(file)) imagePaths.Add(file);
                    }
                    else
                    {
                        var dirName = Path.GetDirectoryName(file);
                        string pattern;
                        if (string.IsNullOrEmpty(dirName))
                        {
                            dirName = workingPath;
                            pattern = file;
                        }
                        else
                        {
                            pattern = file.Substring(dirName.Length);
                            if (pattern.StartsWith(Path.DirectorySeparatorChar.ToString())) pattern = pattern.Substring(1);                            
                        }

                        imagePaths.AddRange(Directory.GetFiles(dirName, pattern));
                    }
                }
                var creator = new IntervalloCreator(finalVideoPath, workingPath, audioFile, videoWidth, videoHeight, imageDuration, subtitleStyle);
                creator.Create(imagePaths);

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        static void Main1(string[] args)
        {
            var workingPath = @"C:\Projects\Temp\Intervallo\Intervallo.CLI\bin\Debug\video";
            var finalVideoPath = @"C:\Projects\Temp\Intervallo\Intervallo.CLI\bin\Debug\video\final.mp4";
            var audioPath = @"C:\Projects\Temp\Intervallo\Intervallo.CLI\bin\Debug\video\audio.mp3";
            var videoWidth = 300;
            var videoHeight = 200;
            var imageDuration = 5;
            var subtitleStyle = new SubtitleStyle();

            var imagePaths = Directory.GetFiles(@"C:\Projects\Temp\Intervallo\Intervallo.CLI\bin\Debug\video", "*.jpg");
            var creator = new IntervalloCreator(finalVideoPath, workingPath, audioPath, videoWidth, videoHeight, imageDuration, subtitleStyle);
            creator.Create(imagePaths);
        }
    }
}
