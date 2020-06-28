using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.Message.UnitTests
{
    [TestClass]
    public class MidiNoteNameTests
    {
        private const string TestNoteName1 = "E";
        private const string TestFullNoteName1 = "E5";
        private const int TestNoteNumber1 = 64;

        [TestMethod]
        public void Construct_GivenANoteNumber_FormatsProperties()
        {
            var nn = new MidiNoteName(TestNoteNumber1);

            Assert.AreEqual(TestNoteNumber1, nn.NoteNumber);
            Assert.AreEqual(5, nn.Octave);
            Assert.AreEqual(TestNoteName1, nn.NoteName);
            Assert.AreEqual(TestFullNoteName1, nn.FullNoteName);
        }

        [TestMethod]
        public void Construct_GivenANoteName_FormatsProperties()
        {
            var nn = new MidiNoteName(TestFullNoteName1);

            Assert.AreEqual(TestNoteNumber1, nn.NoteNumber);
            Assert.AreEqual(5, nn.Octave);
            Assert.AreEqual(TestNoteName1, nn.NoteName);
            Assert.AreEqual(TestFullNoteName1, nn.FullNoteName);
        }

        [TestMethod]
        public void NoteNumberProp_GivenValue_FormatsProperties()
        {
            var nn = new MidiNoteName();

            nn.NoteNumber = TestNoteNumber1;

            Assert.AreEqual(TestNoteNumber1, nn.NoteNumber);
            Assert.AreEqual(5, nn.Octave);
            Assert.AreEqual(TestNoteName1, nn.NoteName);
            Assert.AreEqual(TestFullNoteName1, nn.FullNoteName);
        }

        [TestMethod]
        public void FullNoteNameProp_GivenValue_FormatsProperties()
        {
            var nn = new MidiNoteName();

            nn.FullNoteName = TestFullNoteName1;

            Assert.AreEqual(TestNoteNumber1, nn.NoteNumber);
            Assert.AreEqual(5, nn.Octave);
            Assert.AreEqual(TestNoteName1, nn.NoteName);
            Assert.AreEqual(TestFullNoteName1, nn.FullNoteName);
        }
    }
}