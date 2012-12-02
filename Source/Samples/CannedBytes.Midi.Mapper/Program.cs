using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CannedBytes.Midi.Mapper
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

        private static List<MidiMapperDocument> _documents =
            new List<MidiMapperDocument>();

        private static int _index;

        public static int CurrentPresetIndex
        {
            get { return _index; }
            set { _index = value; }
        }

        public static MidiMapperDocument Document
        {
            get { return _documents[_index]; }
            set { _documents[_index] = value; }
        }
    }
}