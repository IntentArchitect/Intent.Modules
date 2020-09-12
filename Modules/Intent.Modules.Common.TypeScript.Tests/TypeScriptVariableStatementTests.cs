using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptVariableStatementTests
    {
        [Fact]
        public void ArrayObjectsMergedAccordingToIndex()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"//@IntentMerge
const routes: Routes = [
  {
    path: 'users',
    loadChildren: './users/users.module#UsersModule'
  },
  {
    path: 'test',
    loadChildren: './users/users.module#UsersModule'
  }
];", outputContent: @"//@IntentMerge
const routes: Routes = [
  {
    path: 'users-changed',
    loadChildren: './users/users.module#UsersChangedModule'
  }
];");
            Assert.Equal(@"//@IntentMerge
const routes: Routes = [
  {
    path: 'users-changed',
    loadChildren: './users/users.module#UsersChangedModule'
  },
  {
    path: 'test',
    loadChildren: './users/users.module#UsersModule'
  }
];", result);
            
        }

        [Fact]
        public void SpecialIdentifiersGetMatched()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"//@IntentMerge
const routes: Routes = [
  {
    path: 'users',
    loadChildren: './users/users.module#UsersModule'
  },
  //@IntentIdentifier('test')
  {
    path: 'test',
    loadChildren: './users/users.module#UsersModule'
  }
];", outputContent: @"//@IntentMerge
const routes: Routes = [
  //@IntentIdentifier('test')
  {
    path: 'users-changed',
    loadChildren: './users/users.module#UsersChangedModule'
  }
];");
            Assert.Equal(@"//@IntentMerge
const routes: Routes = [
  {
    path: 'users',
    loadChildren: './users/users.module#UsersModule'
  },
  //@IntentIdentifier('test')
  {
    path: 'users-changed',
    loadChildren: './users/users.module#UsersChangedModule'
  }
];", result);

        }
    }
}