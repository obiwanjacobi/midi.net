using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CannedBytes.Midi.Samples.MidiMapper
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _documents.Add(null);
            _documents.Add(null);
            _documents.Add(null);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI.MainForm());
        }

        private static readonly List<MidiMapperDocument> _documents =
            new List<MidiMapperDocument>();

        public static int CurrentPresetIndex { get; set; }

        public static MidiMapperDocument Document
        {
            get { return _documents[CurrentPresetIndex]; }
            set { _documents[CurrentPresetIndex] = value; }
        }
    }
}