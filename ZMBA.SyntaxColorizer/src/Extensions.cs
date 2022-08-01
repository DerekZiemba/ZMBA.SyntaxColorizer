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
    internal const MethodImplOptions AGGRESSIVE_OPTIMIZATION = (MethodImplOptions)512;
    internal const MethodImplOptions AGGRESSIVE_INLINE = MethodImplOptions.AggressiveInlining | AGGRESSIVE_OPTIMIZATION;


    //private static class ListUnderExt<T, E> where E : List<T> {
    //  public delegate T[] DelGetUnderlyingArray(E target);
    //  public static DelGetUnderlyingArray GetUnderlyingArray = DelegateFactory<DelGetUnderlyingArray>.CompileFieldGetter<E>("_items");
    //}

    //public static Span<T> GetUnderlyingArrayAsSpan<T>(this List<T> ls) {
    //  return ListUnderExt<T, List<T>>.GetUnderlyingArray(ls).AsSpan(0, ls.Count);
    //}

    [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
    internal static L AddUnique<T, L>(this L list, T item) where L: IList<T> {
      if (!list.Contains(item)) {
        list.Add(item);
      }
      return list;
    }



    [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
    internal static SyntaxNode FindOuterMostNode(this SyntaxNode rootNode, TextSpan span, bool bTrivia) {
      SyntaxNode node = rootNode.FindToken(span.Start, bTrivia).Parent;
      SyntaxNode parent;
      while (node != null && (parent = node.Parent) != null) {
        TextSpan nodespan = node.FullSpan;
        if (span.Start < nodespan.Start || nodespan.End < span.End || parent != rootNode && nodespan.Length == parent.FullSpan.Length) {
          node = parent;
        } else {
          break;
        }
      }
      switch (node.RawKind) {
        case (int)VBKind.SimpleArgument: node = ((VB.Syntax.SimpleArgumentSyntax)node).Expression; break;
        case (int)CSKind.Argument: node = ((CS.Syntax.ArgumentSyntax)node).Expression; break;
        case (int)CSKind.AttributeArgument: node = ((CS.Syntax.AttributeArgumentSyntax)node).Expression; break;
      }
      return node;
    }

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    internal static SyntaxNode FindOuterMostCSNode(this SyntaxNode rootNode, TextSpan span, bool bTrivia) {
      SyntaxNode node = rootNode;
      if (node != null) {
        SyntaxToken token = node.FindToken(span.Start, bTrivia);
        node = token.Parent;
        SyntaxNode parent;
        TextSpan nodespan;    
        while(node != null && (parent = node.Parent) != null && (span.Start < (nodespan = node.FullSpan).Start || nodespan.End < span.End || parent != rootNode && nodespan.Length == parent.FullSpan.Length)) {
          node = parent;
        }
        switch(node.csKind()) {
          case CSKind.Argument: 
            node = ((CS.Syntax.ArgumentSyntax)node).Expression; 
            break;
          case CSKind.AttributeArgument: 
            node = ((CS.Syntax.AttributeArgumentSyntax)node).Expression;
            break;
        }
      }
      return node;
    }



    [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
    internal static CSKind csKind(this SyntaxNode node) => node != null ? (CSKind)node.RawKind : CSKind.None;


    [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
    internal static string substrAfterLast(this string str, char ch) {
      int index = str.LastIndexOf(ch);
      return index >= 0 ? str.Substring(index) : str;
    }

  }

}
