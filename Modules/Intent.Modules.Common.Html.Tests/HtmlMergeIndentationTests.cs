using Intent.Modules.Common.Html.Weaving;
using Xunit;

namespace Intent.Modules.Common.Html.Tests
{
    public class HtmlMergeIndentationTests
    {
        [Fact]
        public void UsesExistingNodeIndentation()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(@"
<div>
  <div intent-manage intent-id=""div"">
    <span intent-id=""1"">Text 1</span>
  </div>
</div>", @"
<div intent-manage intent-id=""div"">
  <span intent-id=""1"">Text 1</span>
  <span intent-id=""2"">Text 2</span>
</div>");
            Assert.Equal(@"
<div>
  <div intent-manage="""" intent-id=""div"">
    <span intent-id=""1"">Text 1</span>
    <span intent-id=""2"">Text 2</span>
  </div>
</div>", result);
        }
    }
}