using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace CannedBytes.Midi.Samples.MidiMapper
{
    [Serializable]
    [XmlRoot("mapperDocument", Namespace = "http://schemas.cannedbytes.com/Midi/MidiMapper/12/2012")]
    public class MidiMapperDocument
    {
        private static XmlSerializer _xmlSerializer =
            new XmlSerializer(typeof(MidiMapperDocument));

        public MidiMapperDocument()
        {
            MidiNoteMap = new MidiNoteMap();
        }

        [XmlAttribute("channel-in")]
        public int InChannel { get; set; }

        [XmlAttribute("channel-out")]
        public int OutChannel { get; set; }

        [XmlAttribute("midi-thru")]
        public bool MidiThru { get; set; }

        [XmlAttribute("velocity-offset")]
        public byte VelocityOffset { get; set; }

        [XmlElement("mapItem")]
        public MidiNoteMap MidiNoteMap { get; set; }

        [NonSerialized]
        private string _path;

        [XmlIgnore]
        public string FilePath
        {
            get { return _path; }
            set { _path = value; }
        }

        [NonSerialized]
        private bool _isDirty;

        [XmlIgnore]
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        public bool Save()
        {
            if (String.IsNullOrEmpty(FilePath) && !IsDirty)
            {
                return false;
            }

            SaveAs(FilePath);

            return true;
        }

        public void SaveAs(string filePath)
        {
            using (var stream = File.Open(filePath,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                if (IsXmlFormat(filePath))
                {
                    SaveXml(stream);
                }
                else
                {
                    SaveBinary(stream);
                }

                FilePath = filePath;
                IsDirty = false;
            }
        }

        public void SaveXml(Stream stream)
        {
            _xmlSerializer.Serialize(stream, this);
        }

        public void SaveBinary(Stream stream)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);
        }

        public static MidiMapperDocument Load(string filePath)
        {
            MidiMapperDocument doc = null;

            using (var stream = File.Open(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (IsXmlFormat(filePath))
                {
                    doc = LoadXml(stream);
                }
                else
                {
                    doc = LoadBinary(stream);
                }

                doc.FilePath = filePath;
                doc.IsDirty = false;
            }

            return doc;
        }

        public static MidiMapperDocument LoadXml(Stream stream)
        {
            return (MidiMapperDocument)_xmlSerializer.Deserialize(stream);
        }

        public static MidiMapperDocument LoadBinary(Stream stream)
        {
            var formatter = new BinaryFormatter();
            return (MidiMapperDocument)formatter.Deserialize(stream);
        }

        public static bool IsXmlFormat(string filePath)
        {
            return (Path.GetExtension(filePath).ToLower() == ".xmap");
        }
    }
}