using System.Linq;
using Intent.Modules.Angular.Editor;
using Xunit;

namespace Intent.Modules.Common.Tests
{
    public class TypescriptArrayLiteralTests
    {
        [Fact]
        public void DetectsArray()
        {
            var file = new TypescriptFile(@"
const a: Array[] = [];
");
            var array = file.VariableDeclarations().First().GetAssignedValue<TypescriptArrayLiteralExpression>();

            Assert.NotNull(array);
        }

        [Fact]
        public void DetectsArrayIsEmpty()
        {
            var file = new TypescriptFile(@"
const a: Array[] = [];
");
            var variable = file.VariableDeclarations().First();
            var array = variable.GetAssignedValue<TypescriptArrayLiteralExpression>();

            Assert.Empty(array.GetValues<TypescriptNode>());
        }

        [Fact]
        public void DetectsArrayObjects()
        {
            var file = new TypescriptFile(@"
const routes: Routes = [
  {
    path: 'user-search',
    component: UserSearchComponent
  },
  {
    path: 'user-edit/:id',
    component: UserEditComponent,
    resolve: {
      user: UserResolver
    }
  },
];
");
            var array = file.VariableDeclarations().First().GetAssignedValue<TypescriptArrayLiteralExpression>();

            Assert.NotEmpty(array.GetValues<TypescriptObjectLiteralExpression>());
            Assert.Collection(array.GetValues<TypescriptObjectLiteralExpression>(), x => { }, x => { });
        }

        [Fact]
        public void AddsLiteralValueToEmptyArray()
        {
            var file = new TypescriptFile(@"
const routes: Routes = [];
");
            var array = file.VariableDeclarations().First().GetAssignedValue<TypescriptArrayLiteralExpression>();
            array.AddValue("0");
            file.UpdateChanges();

            array = file.VariableDeclarations().First().GetAssignedValue<TypescriptArrayLiteralExpression>();
            Assert.NotEmpty(array.GetValues<TypescriptNode>());
            Assert.Collection(array.GetValues<TypescriptLiteral>(), i =>
            {
                Assert.NotNull(i);
                Assert.Equal("0", i.Value);
            });
        }

        [Fact]
        public void AddsLiteralValueToExistingArray()
        {
            var file = new TypescriptFile(@"
const routes: Routes = [
    'existing-value1',
    'existing-value2'
];
");
            var array = file.VariableDeclarations().First().GetAssignedValue<TypescriptArrayLiteralExpression>();
            array.AddValue("'new-value'");
            file.UpdateChanges();

            array = file.VariableDeclarations().First().GetAssignedValue<TypescriptArrayLiteralExpression>();
            Assert.NotEmpty(array.GetValues<TypescriptNode>());
            Assert.Collection(array.GetValues<TypescriptLiteral>(), s =>
            {
                Assert.NotNull(s);
                Assert.Equal("'existing-value1'", s.Value);
            }, s =>
            {
                Assert.NotNull(s);
                Assert.Equal("'existing-value2'", s.Value);
            }, s =>
            {
                Assert.NotNull(s);
                Assert.Equal("'new-value'", s.Value);
            });
        }
    }

    public class VariableSource
    {
        public const string EmptyArray = @"
const routes: Routes = [];
";
        public const string RoutesArray = @"
const routes: Routes = [
  {
    path: 'user-search',
    component: UserSearchComponent
  },
  {
    path: 'user-edit/:id',
    component: UserEditComponent,
    resolve: {
      user: UserResolver
    }
  },
];
";
    }
}