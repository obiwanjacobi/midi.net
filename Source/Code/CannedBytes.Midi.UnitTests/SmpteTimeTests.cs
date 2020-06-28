using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CannedBytes.Midi.UnitTests
{
    [TestClass]
    public class SmpteTimeTests
    {
        SmpteTimeBase smpteTimeBase = new SmpteTimeBase(SmpteFrameRate.Smpte25, 40);

        [TestMethod]
        public void ConstructFrames_ConvertToMicros_TimeIsEqual()
        {
            long expected = SmpteTime.MicrosecondsInSecond;
            var frameRate = SmpteFrameRate.Smpte25;
            var smpte = new SmpteTime(0, 0, 0, 25, frameRate);
            var actual = smpte.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructSeconds_ConvertToMicros_TimeIsEqual()
        {
            var seconds = 20;
            long expected = seconds * SmpteTime.MicrosecondsInSecond;
            var frameRate = SmpteFrameRate.Smpte25;
            var smpte = new SmpteTime(0, 0, seconds, 0, frameRate);
            var actual = smpte.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructMinutes_ConvertToMicros_TimeIsEqual()
        {
            var minutes = 20;
            long expected = minutes * SmpteTime.MicrosecondsInMinute;
            var frameRate = SmpteFrameRate.Smpte25;
            var smpte = new SmpteTime(0, minutes, 0, 0, frameRate);
            var actual = smpte.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructHours_ConvertToMicros_TimeIsEqual()
        {
            var hours = 20;
            long expected = hours * SmpteTime.MicrosecondsInHour;
            var frameRate = SmpteFrameRate.Smpte25;
            var smpte = new SmpteTime(hours, 0, 0, 0, frameRate);
            var actual = smpte.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConvertToMicros_ConvertBack_TimeIsEqual()
        {
            var frameRate = SmpteFrameRate.Smpte25;
            var expected = new SmpteTime(1, 2, 3, 4, frameRate);
            var micros = expected.ToMicroseconds();
            var actual = SmpteTime.FromMicroseconds(micros, frameRate, 0);

            Assert.AreEqual(expected.Hour, actual.Hour, "Hour");
            Assert.AreEqual(expected.Minute, actual.Minute, "Minute");
            Assert.AreEqual(expected.Second, actual.Second, "Second");
            Assert.AreEqual(expected.Frame, actual.Frame, "Frame");
        }

        [TestMethod]
        public void ConstructFromMicros_CheckFrames_TimeIsEqual()
        {
            var expected = 13;

            long micros = (long)(expected * (SmpteTime.MicrosecondsInSecond / 25.0));
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(0, actual.Hour, "Hour");
            Assert.AreEqual(0, actual.Minute, "Minute");
            Assert.AreEqual(0, actual.Second, "Second");
            Assert.AreEqual(expected, actual.Frame, "Frame");
        }

        [TestMethod]
        public void ConstructFromMicros_CheckSeconds_TimeIsEqual()
        {
            var expected = 13;
            long micros = expected * SmpteTime.MicrosecondsInSecond;
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(0, actual.Hour, "Hour");
            Assert.AreEqual(0, actual.Minute, "Minute");
            Assert.AreEqual(expected, actual.Second, "Second");
            Assert.AreEqual(0, actual.Frame, "Frame");
        }

        [TestMethod]
        public void ConstructFromMicros_CheckMinutes_TimeIsEqual()
        {
            var expected = 13;
            long micros = expected * SmpteTime.MicrosecondsInMinute;
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(0, actual.Hour, "Hour");
            Assert.AreEqual(expected, actual.Minute, "Minute");
            Assert.AreEqual(0, actual.Second, "Second");
            Assert.AreEqual(0, actual.Frame, "Frame");
        }

        [TestMethod]
        public void ConstructFromMicros_CheckHours_TimeIsEqual()
        {
            var expected = 13;
            long micros = expected * SmpteTime.MicrosecondsInHour;
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(expected, actual.Hour, "Hour");
            Assert.AreEqual(0, actual.Minute, "Minute");
            Assert.AreEqual(0, actual.Second, "Second");
            Assert.AreEqual(0, actual.Frame, "Frame");
        }

        [TestCategory("LocalOnly")]
        [TestMethod]
        [Ignore]
        public void ConstructFromMicros_ConvertFrameRate_TimeIsEqual()
        {
            var expected = 167821672872L;
            var smpte = SmpteTime.FromMicroseconds(expected, SmpteFrameRate.Smpte25, 0);
            var converted = smpte.ConvertTo(SmpteFrameRate.Smpte30, 0);
            var actual = converted.ToMicroseconds();

            // TODO: looks like a rounding error...
            Assert.AreEqual(expected, actual);
        }
    }
}