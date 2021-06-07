using Code.Core.KeyStretching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;

namespace TEST
{
    [TestClass]
    public class KeyStretchingTest
    {
        [TestMethod]
        public void Input_And_Output_Not_Same()
        {
            byte[] mdp = DataPath.Mdp;
            IKeyStretching stretch = new ArgonKeyStretching();
            byte[] stretchMdp = stretch.Stretching(mdp);

            Assert.IsNotNull(stretchMdp);
            Assert.IsTrue(mdp.Length != stretchMdp.Length);
            Assert.IsFalse(mdp.SequenceEqual(stretchMdp));
        }

        [TestMethod]
        public void Test_Default_OutputSize()
        {
            byte[] mdp = DataPath.Mdp;
            IKeyStretching stretch = new ArgonKeyStretching();
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
            IKeyStretching stretch = new ArgonKeyStretching();
            byte[] stretchMdp = stretch.Stretching(mdp, expectedOutputSize);

            Assert.IsTrue(stretchMdp.Length == expectedOutputSize);
        }

        [TestMethod]
        [DataRow(262144, 1, 1)] 
        [DataRow(524288, 1, 1)]
        [DataRow(524288, 2, 1)]
        [DataRow(524288, 1, 2)]
        [DataRow(1048576, 2, 1)] 
        public void Duration_must_be_greater_than_1_seconde(int memoryPressure, int parallelizationPressure,int iteration)
        {
            byte[] mdp = DataPath.Mdp;
            IKeyStretching stretch = new ArgonKeyStretching(memoryPressure,parallelizationPressure, iteration);
            Stopwatch sw = Stopwatch.StartNew();
            byte[] stretchMdp = stretch.Stretching(mdp);
            sw.Stop();

            Assert.IsTrue(sw.Elapsed.TotalSeconds > 1d);
        }
    }
}
