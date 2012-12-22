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
            long expected = 1000L;
            var frameRate = SmpteFrameRate.Smpte25;
            var smpte = new SmpteTime(0, 0, 0, 40, frameRate);
            var actual = smpte.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructSeconds_ConvertToMicros_TimeIsEqual()
        {
            var seconds = 20;
            long expected = seconds * 1000L * 1000L;
            var frameRate = SmpteFrameRate.Smpte25;
            var smpte = new SmpteTime(0, 0, seconds, 0, frameRate);
            var actual = smpte.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructMinutes_ConvertToMicros_TimeIsEqual()
        {
            var minutes = 20;
            long expected = minutes * 60L * 1000L * 1000L;
            var frameRate = SmpteFrameRate.Smpte25;
            var smpte = new SmpteTime(0, minutes, 0, 0, frameRate);
            var actual = smpte.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ConstructHours_ConvertToMicros_TimeIsEqual()
        {
            var hours = 20;
            long expected = hours * 60L * 60L * 1000L * 1000L;
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

            Assert.AreEqual(expected.Hour, actual.Hour);
            Assert.AreEqual(expected.Minute, actual.Minute);
            Assert.AreEqual(expected.Second, actual.Second);
            Assert.AreEqual(expected.Frame, actual.Frame);
        }

        [TestMethod]
        public void ConstructFromMicros_CheckFrames_TimeIsEqual()
        {
            var expected = 13;

            long micros = (long)(expected * ((1000L * 1000L) / 25.0));
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(0, actual.Hour);
            Assert.AreEqual(0, actual.Minute);
            Assert.AreEqual(0, actual.Second);
            Assert.AreEqual(expected, actual.Frame);
        }

        [TestMethod]
        public void ConstructFromMicros_CheckSeconds_TimeIsEqual()
        {
            var expected = 13;
            long micros = expected * 1000L * 1000L;
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(0, actual.Hour);
            Assert.AreEqual(0, actual.Minute);
            Assert.AreEqual(expected, actual.Second);
            Assert.AreEqual(0, actual.Frame);
        }

        [TestMethod]
        public void ConstructFromMicros_CheckMinutes_TimeIsEqual()
        {
            var expected = 13;
            long micros = expected * 60L * 1000L * 1000L;
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(0, actual.Hour);
            Assert.AreEqual(expected, actual.Minute);
            Assert.AreEqual(0, actual.Second);
            Assert.AreEqual(0, actual.Frame);
        }

        [TestMethod]
        public void ConstructFromMicros_CheckHours_TimeIsEqual()
        {
            var expected = 13;
            long micros = expected * 60L * 60L * 1000 * 1000;
            var actual = SmpteTime.FromMicroseconds(micros, SmpteFrameRate.Smpte25, 0);

            Assert.AreEqual(expected, actual.Hour);
            Assert.AreEqual(0, actual.Minute);
            Assert.AreEqual(0, actual.Second);
            Assert.AreEqual(0, actual.Frame);
        }

        [TestMethod]
        public void ConstructFromMicros_ConvertFrameRate_TimeIsEqual()
        {
            var expected = 167821672872L;
            var smpte = SmpteTime.FromMicroseconds(expected, SmpteFrameRate.Smpte25, 0);
            var converted = smpte.ConvertTo(SmpteFrameRate.Smpte30, 0);
            var actual = converted.ToMicroseconds();

            Assert.AreEqual(expected, actual);
        }
    }
}