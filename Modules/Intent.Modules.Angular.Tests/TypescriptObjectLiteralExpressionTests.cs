using System.Linq;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Common.TypeScript.Editor;
using Xunit;

namespace Intent.Modules.Common.Tests
{
    public class TypescriptObjectLiteralExpressionTests
    {
        [Fact]
        public void FindsProperty()
        {
            var file = new TypeScriptFile(@"var v = {
    path: 'user-search',
    component: UserSearchComponent
  }");
            var variable = file.VariableDeclarations().First();
            var objectLiteral = variable.GetAssignedValue<TypeScriptObjectLiteralExpression>();

            Assert.NotNull(objectLiteral);
            Assert.True(objectLiteral.PropertyAssignmentExists("path"));
            Assert.True(objectLiteral.PropertyAssignmentExists("path", "'user-search'"));
            Assert.True(objectLiteral.PropertyAssignmentExists("component", "UserSearchComponent"));
        }

        [Fact]
        public void AddsProperty()
        {
            var file = new TypeScriptFile(@"var v = {
    path: 'user-search',
    component: UserSearchComponent
  }");
            var variable = file.VariableDeclarations().First();
            var objectLiteral = variable.GetAssignedValue<TypeScriptObjectLiteralExpression>();

            objectLiteral.AddPropertyAssignment("test: 'test-value'");
            //file.UpdateChanges();

            //objectLiteral = file.VariableDeclarations().First().GetAssignedValue<TypescriptObjectLiteralExpression>();
            Assert.True(objectLiteral.PropertyAssignmentExists("test", "'test-value'"));
        }
    }
}