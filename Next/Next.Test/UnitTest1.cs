using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Next.Test
{
    [TestClass]
    public class UnitTest1
    {
        private Compressor Compressor { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            Compressor = new Compressor();
        }

        [TestMethod]
        public void ShouldFindDifferences()
        {
            string one = "The brown fox jumped over the fence.";
            string two = "The brown fox and I jumped over the fence.";

            var source = Encoding.UTF8.GetBytes(one);
            var target = Encoding.UTF8.GetBytes(two);

            var delta = Compressor.Compress(source, target);

            byte[] output = Compressor.Uncompress(source, delta);

            Assert.AreEqual(output.Length, target.Length);
            Assert.AreEqual(two, Encoding.UTF8.GetString(output));
        }

        [TestMethod]
        public void ShouldNotFailIfTargetsLengthIsLessThanSource()
        {
            string one = "the brown fox jumped over the fence and it was awesome.";
            string two = "the brown fox jumped over the fence.";

            byte[] source = Encoding.UTF8.GetBytes(one);

            var delta = Compressor.Compress(source, Encoding.UTF8.GetBytes(two));
            
            byte[] output = Compressor.Uncompress(source, delta);

            Assert.AreEqual(two, Encoding.UTF8.GetString(output));
        }
    }
}
