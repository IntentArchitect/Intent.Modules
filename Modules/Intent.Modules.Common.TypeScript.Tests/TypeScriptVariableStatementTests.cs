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
  //@IntentMerge('test')
  {
    path: 'test',
    loadChildren: './users/users.module#UsersModule'
  }
];", outputContent: @"//@IntentMerge
const routes: Routes = [
  //@IntentMerge('test')
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
  //@IntentMerge('test')
  {
    path: 'users-changed',
    loadChildren: './users/users.module#UsersChangedModule'
  }
];", result);
        }

        [Fact]
        public void OverwritesManagedObjectLiteralUsingIdentifier()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"//@IntentMerge
const routes: Routes = [
  {
    path: 'users',
    loadChildren: './users/users.module#UsersModule'
  },
  //@IntentManage('test')
  {
    path: 'test',
    loadChildren: './users/users.module#UsersModule'
  }
];", outputContent: @"//@IntentMerge
const routes: Routes = [
  //@IntentManage('test')
  {
    path: 'users-changed',
    load: './users/users.module#UsersChangedModule'
  }
];");
            Assert.Equal(@"//@IntentMerge
const routes: Routes = [
  {
    path: 'users',
    loadChildren: './users/users.module#UsersModule'
  },
  //@IntentManage('test')
  {
    path: 'users-changed',
    load: './users/users.module#UsersChangedModule'
  }
];", result);
        }

        [Fact]
        public void AddsPropertyWithIntentCanAdd()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"
//@IntentCanAdd()
export const environment = {
  production: false
};", outputContent: @"
export const environment = {
  production: true,
  api_url: 'https://localhost:{port}/api'
};
");
            Assert.Equal(@"
//@IntentCanAdd()
export const environment = {
  production: false,
  api_url: 'https://localhost:{port}/api'
};", result);
        }

        [Fact]
        public void IgnoresExistingPropertyWithIntentCanAdd()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"
//@IntentCanAdd()
export const environment = {
  production: false,
  api_url: 'https://localhost:44336/api'
};", outputContent: @"
//@IntentCanAdd()
export const environment = {
  production: true,
  api_url: 'https://localhost:{port}/api'
};");
            Assert.Equal(@"
//@IntentCanAdd()
export const environment = {
  production: false,
  api_url: 'https://localhost:44336/api'
};", result);
        }
    }
}