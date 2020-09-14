using System;
using Intent.Modules.Common.Html.Weaving;
using Xunit;

namespace Intent.Modules.Common.Html.Tests
{
    public class HtmlMergeTests
    {
        [Fact]
        public void Test1()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(HtmlFileWithManagedButtons, HtmlFileWithManagedDifferentButtons);
            Assert.Equal(HtmlFileWithManagedDifferentButtons, result);
        }

        [Fact]
        public void AddsNewNodeInManagedContainer()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(HtmlFileWithMergedContainerOnly, HtmlFileWithMergedContainer);
            Assert.Equal(HtmlFileWithMergedContainer, result);
        }

        [Fact]
        public void Test2()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(HtmlFileWithMergedContainerOnly, HtmlFileWithManagedButtons);
            Assert.Equal(HtmlFileWithManagedButtons, result);
        }

        [Fact]
        public void OverwritesEmptyFile()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge("", HtmlFileWithManagedButtons);
            Assert.Equal(HtmlFileWithManagedButtons, result);
        }

        [Fact]
        public void AddsElementsToBasicDeepMergedNode()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(HtmlFileWithDeepMergedElement, HtmlFileWithDeepMergedElementWithTwoButtons);
            Assert.Equal(HtmlFileWithDeepMergedElementWithTwoButtons, result);
        }

        [Fact]
        public void AddsElementsToDeepMergedNode()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(@"
<nav class=""navbar navbar-expand-lg navbar-light bg-light"">
  <div class=""collapse navbar-collapse"" [collapse]=""isCollapsed"">
    <ul class=""navbar-nav"" intent-merge=""navbar"">
    </ul>
  </div>
</nav>", @"
<nav class=""navbar navbar-expand-lg navbar-light bg-light"">
  <div class=""collapse navbar-collapse"" [collapse]=""isCollapsed"">
    <ul class=""navbar-nav"" intent-merge=""navbar"">
      <li class=""nav-item active"">
        <a class=""nav-link"" routerLink=""/customers"">Customers</a>
      </li>
      <li class=""nav-item active"">
        <a class=""nav-link"" routerLink=""/users"">Users</a>
      </li>
    </ul>
  </div>
</nav>");
            Assert.Equal(@"
<nav class=""navbar navbar-expand-lg navbar-light bg-light"">
  <div class=""collapse navbar-collapse"" [collapse]=""isCollapsed"">
    <ul class=""navbar-nav"" intent-merge=""navbar"">
      <li class=""nav-item active"">
        <a class=""nav-link"" routerLink=""/customers"">Customers</a>
      </li>
      <li class=""nav-item active"">
        <a class=""nav-link"" routerLink=""/users"">Users</a>
      </li>
    </ul>
  </div>
</nav>", result);
            
        }

        public static string HtmlFileWithMergedContainerOnly = @"
  <div class=""container-fluid"" intent-merge=""container""></div>";

        public static string HtmlFileWithManagedButtons = @"
  <div class=""container-fluid"" intent-merge=""container"">
    <div class=""row"" intent-manage=""buttons-identifier"">
      <div class=""col"">
        <div>
          <button type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add User</button>
        </div>
      </div>
    </div>
  </div>";
        public static string HtmlFileWithManagedDifferentButtons = @"
  <div class=""container-fluid"" intent-merge=""container"">
    <div class=""row"" intent-manage=""buttons-identifier"">
      <div class=""col"">
        <div>
          <button type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
        </div>
      </div>
    </div>
  </div>";
        public static string HtmlFileWithMergedContainer = @"
  <div class=""container-fluid"" intent-merge=""container"">
    <div class=""row"">
      <div class=""col"">
        <div>
          <button type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add User</button>
        </div>
      </div>
    </div>
  </div>";

        public static string HtmlFileWithDeepMergedElement = @"
  <div class=""container-fluid"">
    <div class=""row"">
      <div class=""col"">
        <div intent-merge=""container"">
          <button type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
        </div>
      </div>
    </div>
  </div>";

        public static string HtmlFileWithDeepMergedElementWithTwoButtons = @"
  <div class=""container-fluid"">
    <div class=""row"">
      <div class=""col"">
        <div intent-merge=""container"">
          <button type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
          <button type=""button"" class=""btn btn-warning"" (click)=""delete()"">Delete Customer</button>
        </div>
      </div>
    </div>
  </div>";
    }
}
