using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using Shouldly;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class MethodModifierTests
{
    [Fact]
    public async Task ValidModifierCombinationsTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("BaseClass", @class =>
            {
                @class.Abstract();
                @class.AddMethod("void", "AbstractOverride", m => m.Virtual());
                @class.AddMethod("void", "NewVirtual");
                @class.AddMethod("void", "NewAbstract");
                @class.AddMethod("void", "StaticNew", m => m.Static());
                @class.AddMethod("void", "OverridePartial", m => m.Virtual());
                @class.AddMethod("void", "SealedOverride", m => m.Virtual());
                @class.AddMethod("void", "OverrideThenSealed", m => m.Virtual());
                @class.AddMethod("void", "SealedViaInterface", m => m.Virtual());
            })
            .AddClass("DerivedClass", @class =>
            {
                @class.Abstract().Partial().WithBaseType("BaseClass");
                @class.AddMethod("void", "SealedOverride", m => m.Sealed());
                @class.AddMethod("void", "OverrideThenSealed", m => m.Override().Sealed());
                @class.AddMethod("void", "SealedViaInterface", m => ((ICSharpClassMethodDeclaration)m).Sealed());
                @class.AddMethod("void", "AbstractOverride", m => m.Override().Abstract());
                @class.AddMethod("void", "NewVirtual", m => m.New().Virtual());
                @class.AddMethod("void", "NewAbstract", m => m.Abstract().New());
                @class.AddMethod("void", "StaticNew", m => m.Static().New());
                @class.AddMethod("void", "StaticPartial", m => m.Static().Partial());
                @class.AddMethod("void", "StaticPartial", m => m.Static().Partial().AddStatement("// Implementation"));
                @class.AddMethod("void", "OverridePartial", m => m.Override().Partial());
                @class.AddMethod("void", "OverridePartial", m => m.Override().Partial().AddStatement("// Implementation"));
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task ConflictingModifiersLastCallWinsTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddClass("ConflictingModifiers", @class =>
            {
                @class.Abstract();
                @class.AddMethod("void", "StaticThenVirtual", m => m.Static().Virtual());
                @class.AddMethod("void", "VirtualThenStatic", m => m.Virtual().Static());
                @class.AddMethod("void", "AbstractThenVirtual", m => m.Abstract().Virtual());
                @class.AddMethod("void", "VirtualThenOverride", m => m.Virtual().Override());
                @class.AddMethod("void", "OverrideThenNew", m => m.Override().New());
                @class.AddMethod("void", "NewThenOverride", m => m.New().Override());
                @class.AddMethod("void", "OverrideThenStatic", m => m.Override().Static());
                @class.AddMethod("void", "CombinedThenWithoutMethodModifier", m => m.New().Virtual().WithoutMethodModifier());
                @class.AddMethod("void", "SealedThenVirtual", m => m.Sealed().Virtual());
                @class.AddMethod("void", "SealedThenNew", m => m.Sealed().New());
                @class.AddMethod("void", "SealedThenAbstract", m => m.Sealed().Abstract());
                @class.AddMethod("void", "AbstractThenSealed", m => m.Abstract().Sealed());
            })
            .CompleteBuild();

        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public void ModifierFlagsTrackCombinations()
    {
        var @class = new CSharpClass("Flags");

        @class.AddMethod("void", "StaticNew", m => m.Static().New());
        @class.AddMethod("void", "AbstractOverride", m => m.Override().Abstract());
        @class.AddMethod("void", "Sealed", m => ((ICSharpClassMethodDeclaration)m).Sealed());
        @class.AddMethod("void", "StaticThenSealed", m => m.Static().Sealed());

        var staticNew = (ICSharpClassMethodDeclaration)@class.FindMethod("StaticNew")!;
        staticNew.IsStatic.ShouldBeTrue();
        staticNew.IsAbstract.ShouldBeFalse();
        staticNew.IsSealed.ShouldBeFalse();

        var abstractOverride = (ICSharpClassMethodDeclaration)@class.FindMethod("AbstractOverride")!;
        abstractOverride.IsAbstract.ShouldBeTrue();
        abstractOverride.IsStatic.ShouldBeFalse();

        var @sealed = (ICSharpClassMethodDeclaration)@class.FindMethod("Sealed")!;
        @sealed.IsSealed.ShouldBeTrue();

        var staticThenSealed = (ICSharpClassMethodDeclaration)@class.FindMethod("StaticThenSealed")!;
        staticThenSealed.IsSealed.ShouldBeTrue();
        staticThenSealed.IsStatic.ShouldBeFalse();
    }

    [Fact]
    public void SealedHasNoEffectOnInterfaceMethods()
    {
        var @interface = new CSharpInterface("IFlags");
        @interface.AddMethod("void", "Method", m => ((ICSharpInterfaceMethodDeclaration)m).Sealed());

        var method = (ICSharpInterfaceMethodDeclaration)@interface.Methods[0];
        method.IsSealed.ShouldBeFalse();
        @interface.Methods[0].ToString().ShouldNotContain("sealed");
    }
}
