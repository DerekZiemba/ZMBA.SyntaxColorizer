using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Shared;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using Roslyn.Utilities;
using VB = Microsoft.CodeAnalysis.VisualBasic;
using CS = Microsoft.CodeAnalysis.CSharp;
using VBKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;
using CSKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;


namespace ZMBA.SyntaxColorizer {

  internal static class Extensions {

    internal static TagSpan<ClassificationTag> Associate(this ClassificationTag tag, ITextSnapshot snapshot, TextSpan txt) {
      return new TagSpan<ClassificationTag>(new SnapshotSpan(snapshot, txt.Start, txt.Length), tag);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private static ImmutableArray<ISymbol> GetMembersByName<T> (this T symbol, string name) where T: INamedTypeSymbol {
      if(name != null && name.Length > 1) {
        int index = name.LastIndexOf('.');
        if(index >= 0) {
          name = name.Substring(index);
        }
        return symbol.GetMembers(name);
      }
      return ImmutableArray<ISymbol>.Empty;
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    internal static SyntaxNode FindOuterMostNode (this SyntaxNode rootNode, TextSpan span, bool bTrivia) {
      SyntaxNode node = rootNode.FindToken(span.Start, bTrivia).Parent;
      SyntaxNode parent;
      while(node != null && (parent = node.Parent) != null) {
        TextSpan nodespan = node.FullSpan;
        if(span.Start < nodespan.Start || nodespan.End < span.End || parent != rootNode && nodespan.Length == parent.FullSpan.Length) {
          node = parent;
        } else {
          break;
        }
      }
      switch(node.RawKind) {
        case (int)VBKind.SimpleArgument: node = ((VB.Syntax.SimpleArgumentSyntax)node).Expression; break;
        case (int)CSKind.Argument: node = ((CS.Syntax.ArgumentSyntax)node).Expression; break;
        case (int)CSKind.AttributeArgument: node = ((CS.Syntax.AttributeArgumentSyntax)node).Expression; break;
      }
      return node;
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    internal static bool MethodImplementsInterface<T> (this T symbol) where T: IMethodSymbol {
      if(symbol.ExplicitInterfaceImplementations.Length > 0) {  return true; }

      ImmutableArray<INamedTypeSymbol> interfaces = symbol.ContainingType.Interfaces;
      for(var i = 0; i < interfaces.Length; i++) {
        ImmutableArray<ISymbol> mems = interfaces[i].GetMembersByName(symbol.Name);
        for (var j = 0; j < mems.Length; j++) {
          if(mems[j] is IMethodSymbol member && Equals(symbol.ReturnType, member.ReturnType)) {
            if(symbol.Parameters.Length == member.Parameters.Length) {
              var impl = symbol.ContainingType.FindImplementationForInterfaceMember(member);
              if(impl != null && impl.Equals(symbol)) {
                return true;
              }
            }
          }
        }
      }
      return false;
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    internal static bool PropertyImplementsInterface<T> (this T symbol) where T: IPropertySymbol {
      if(symbol.ExplicitInterfaceImplementations.Length > 0) {  return true; }

      ImmutableArray<INamedTypeSymbol> interfaces = symbol.ContainingType.Interfaces;
      for(var i = 0; i < interfaces.Length; i++) {
        ImmutableArray<ISymbol> mems = interfaces[i].GetMembersByName(symbol.Name);
        for(var j = 0; j < mems.Length; j++) {
          if(mems[j] is IPropertySymbol member && Equals(symbol.Type, member.Type)) {
            if(symbol.Parameters.Length == member.Parameters.Length) {
              var impl = symbol.ContainingType.FindImplementationForInterfaceMember(member);
              if(impl != null && impl.Equals(symbol)) {
                return true;
              }
            }
          }
        }
      }

      return false;
    }
  }

}
