using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Intervallo
{
    public static class FFMpegUtils
    {
        public static void CallFFMpeg(string pathToFfmpeg, string workingDir, string arguments)
        {
            var ffmpeg = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    FileName = pathToFfmpeg,
                    WorkingDirectory = workingDir,
                    Arguments = arguments
                }
            };

            try
            {
                if (!ffmpeg.Start())
                {
                    //Console.WriteLine("Error starting");
                    return;
                }
                var reader = ffmpeg.StandardError;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
            catch// (Exception exception)
            {
                //Console.WriteLine(exception.ToString());
                return;
            }

            ffmpeg.Close();
        }
    }
}
