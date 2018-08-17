using Freescape.Game.Server.ValueObject;
using Freescape.Game.Server.ValueObject.Dialog;
using NUnit.Framework;

namespace Freescape.Game.Server.Tests.ValueObject.Dialog
{
    public class DialogPageTests
    {
        [Test]
        public void DialogPage_Ctor_ShouldHaveValidDefaultValues()
        {
            DialogPage page = new DialogPage();

            Assert.AreEqual(string.Empty, page.Header);
            Assert.AreEqual(0, page.Responses.Count);
            Assert.AreEqual(new CustomData(), page.CustomData);
            Assert.AreEqual(0, page.NumberOfResponses);
        }

        [Test]
        public void DialogPage_CtorValues_ShouldHaveValidDefaultValues()
        {
            DialogPage page = new DialogPage(
                "MyHeader",
                "Response 1",
                "Response 2");

            Assert.AreEqual("MyHeader", page.Header);
            Assert.AreEqual(2, page.Responses.Count);
            Assert.AreEqual(new CustomData(), page.CustomData);
            Assert.AreEqual(2, page.NumberOfResponses);
        }
    }
}
