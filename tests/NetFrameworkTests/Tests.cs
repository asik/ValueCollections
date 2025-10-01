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
            Assert.AreEqual(ValueArray.Create(1, 2, 3), ValueArray.Create(1, 2, 3));
        }
    }
}
