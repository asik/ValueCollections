using Microsoft.VisualStudio.TestTools.UnitTesting;
using ValueCollections;

namespace NetFrameworkTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Sanity()
        {
            Assert.AreEqual(Block.Create(1, 2, 3), Block.Create(1, 2, 3));
        }
    }
}
