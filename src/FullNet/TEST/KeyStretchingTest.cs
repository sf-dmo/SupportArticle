using Code.Core.KeyStretching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace TEST
{
    [TestClass]
    public class KeyStretchingTest
    {
        [TestMethod]
        public void Input_And_Output_Not_Same()
        {
            byte[] mdp = DataPath.Mdp;
            IKeyStretching stretch = new PBKDF2KeyStretching();
            byte[] stretchMdp = stretch.Stretching(mdp);

            Assert.IsNotNull(stretchMdp);
            Assert.IsTrue(mdp.Length != stretchMdp.Length);
            Assert.IsFalse(mdp.SequenceEqual(stretchMdp));
        }

        [TestMethod]
        public void Test_Default_OutputSize()
        {
            byte[] mdp = DataPath.Mdp;
            IKeyStretching stretch = new PBKDF2KeyStretching();
            byte[] stretchMdp = stretch.Stretching(mdp);

            Assert.IsTrue(stretchMdp.Length == 32);
        }

        [TestMethod]
        [DataRow(16)]
        [DataRow(32)]
        [DataRow(24)]
        [DataRow(64)]
        [DataRow(55)]
        public void Test_OutputSize(int expectedOutputSize)
        {
            byte[] mdp = DataPath.Mdp;
            IKeyStretching stretch = new PBKDF2KeyStretching();
            byte[] stretchMdp = stretch.Stretching(mdp, expectedOutputSize);

            Assert.IsTrue(stretchMdp.Length == expectedOutputSize);
        }

        [TestMethod]
        //[DataRow(1000)]
        //[DataRow(10000)]
        //[DataRow(50000)]
        //[DataRow(100000)]
        //[DataRow(1000000)]
        [DataRow(1200000)]
        [DataRow(1500000)]
        [DataRow(2000000)]
        //[DataRow(3000000)]
        //[DataRow(5000000)]
        //[DataRow(10000000)]
        public void Duration_must_be_greater_than_1_seconde(int iterationCount)
        {
            byte[] mdp = DataPath.Mdp;
            IKeyStretching stretch = new PBKDF2KeyStretching();
            Stopwatch sw = Stopwatch.StartNew();
            byte[] stretchMdp = stretch.Stretching(mdp, ((PBKDF2KeyStretching)stretch).DEFAULT_SALT, iterationCount);
            sw.Stop();
           
            Assert.IsTrue(sw.Elapsed.TotalSeconds > 1d);
        }
    }
}
