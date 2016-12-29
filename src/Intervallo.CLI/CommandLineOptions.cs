using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Intervallo
{
    public class CommandLineOptions
    {
        [Option(shortName: 'o', longName: "output", HelpText = "Output filename")]
        public string OutputFile { get; set; }

        [Option(shortName: 'a', longName: "audio", HelpText = "Audio filename")]
        public string AudioFile { get; set; }

        [Option(shortName: 'w', longName: "width", HelpText = "Video width", Default = 704)]
        public int VideoWidth { get; set; }

        [Option(shortName: 'h', longName: "height", HelpText = "Video height", Default = 576)]
        public int VideoHeight { get; set; }

        [Option(shortName: 'd', longName: "duration", HelpText = "Time for each image in seconds", Default = 5)]
        public int ImageDuration { get; set; }

        [Option(shortName: 'f', longName: "font", HelpText = "Font to use for subtitles", Default = "DejaVuSerif")]
        public string FontFamilyName { get; set; }

        [Option(shortName: 's', longName: "fontsize", HelpText = "Font size in em", Default = 9)]
        public int FontSizeEm { get; set; }

        [Option(shortName: 'c', longName: "color", HelpText = "Font color", Default = 0xFFFFFF)]
        public int FontColor { get; set; }

        [Value(0, Required = true, HelpText = "Image files (absolute, relative, or search pattern)")]
        public IEnumerable<string> ImageFiles { get; set; }
    }
}
