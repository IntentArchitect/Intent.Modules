using System;
using Intent.Modules.Common.Html.Weaving;
using Xunit;

namespace Intent.Modules.Common.Html.Tests
{
    public class HtmlMergeTests
    {
        [Fact]
        public void OverwritesManagedElements()
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
        public void AddsNewElementsToEmptyContainer()
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
        public void RemovesElementsFromBasicDeepMergedNode()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(HtmlFileWithDeepManagedElementWithThreeButtons, HtmlFileWithDeepManagedElement);
            Assert.Equal(HtmlFileWithDeepManagedElement, result);
        }

        [Fact]
        public void InsertElementInCorrectOrder()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(HtmlFileWithDeepManagedElementWithTwoButtons, HtmlFileWithDeepManagedElementWithThreeButtons);
            Assert.Equal(HtmlFileWithDeepManagedElementWithThreeButtons, result);
        }

        [Fact]
        public void InsertElementInCorrectOrderSimple()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(@"
<div intent-manage="""">
  <span id=""1"">1</span>
  <span id=""4"">4</span>
</div>", @"
<div intent-manage="""">
  <span id=""1"">1</span>
  <span id=""2"">2</span>
  <span id=""3"">3</span>
  <span id=""4"">4</span>
</div>");
            Assert.Equal(@"
<div intent-manage="""">
  <span id=""1"">1</span>
  <span id=""2"">2</span>
  <span id=""3"">3</span>
  <span id=""4"">4</span>
</div>", result);
        }

        [Fact]
        public void RemovesCorrectElementsBasedOnId()
        {
            var merger = new HtmlWeavingMerger();
            var result = merger.Merge(@"
<div intent-manage="""">
  <span id=""1"">1</span>
  <span id=""2"">2</span>
  <span id=""3"">3</span>
  <span id=""4"">4</span>
</div>", @"
<div intent-manage="""">
  <span id=""1"">1</span>
  <span id=""4"">4</span>
</div>");
            Assert.Equal(@"
<div intent-manage="""">
  <span id=""1"">1</span>
  <span id=""4"">4</span>
</div>", result);
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
          <button id=""add-customer"" type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
        </div>
      </div>
    </div>
  </div>";
        public static string HtmlFileWithMergedContainer = @"
  <div class=""container-fluid"" intent-merge=""container"">
    <div class=""row"">
      <div class=""col"">
        <div>
          <button id=""add-customer"" type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
        </div>
      </div>
    </div>
  </div>";

        public static string HtmlFileWithDeepMergedElement = @"
  <div class=""container-fluid"">
    <div class=""row"">
      <div class=""col"">
        <div intent-merge=""container"">
          <button id=""add-customer"" type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
        </div>
      </div>
    </div>
  </div>";

        public static string HtmlFileWithDeepManagedElement = @"
  <div class=""container-fluid"">
    <div class=""row"">
      <div class=""col"">
        <div intent-manage=""container"">
          <button id=""add-customer"" type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
        </div>
      </div>
    </div>
  </div>";

        public static string HtmlFileWithDeepMergedElementWithTwoButtons = @"
  <div class=""container-fluid"">
    <div class=""row"">
      <div class=""col"">
        <div intent-merge=""container"">
          <button id=""add-customer"" type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
          <button id=""delete-customer"" type=""button"" class=""btn btn-warning"" (click)=""delete()"">Delete Customer</button>
        </div>
      </div>
    </div>
  </div>";

        public static string HtmlFileWithDeepManagedElementWithTwoButtons = @"
  <div class=""container-fluid"">
    <div class=""row"">
      <div class=""col"">
        <div intent-manage=""container"">
          <button id=""add-customer"" type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add Customer</button>
          <button id=""delete-customer"" type=""button"" class=""btn btn-warning"" (click)=""delete()"">Delete Customer</button>
        </div>
      </div>
    </div>
  </div>";

        public static string HtmlFileWithDeepManagedElementWithThreeButtons = @"
  <div class=""container-fluid"">
    <div class=""row"">
      <div class=""col"">
        <div intent-manage=""container"">
          <button id=""add-customer"" type=""button"" class=""btn btn-primary"" (click)=""navigateToCreate()"">Add</button>
          <button id=""cancel"" type=""button"" class=""btn btn-warning"" (click)=""doSomeSpecial()"">Something Special?</button>
          <button id=""delete-customer"" type=""button"" class=""btn btn-warning"" (click)=""delete()"">Delete Customer</button>
        </div>
      </div>
    </div>
  </div>";
    }
}
