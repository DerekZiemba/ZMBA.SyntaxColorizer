using System;
using System.Linq;
using System.Collections.Generic;
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

      [Flags]
      public enum SubstrOptions {
         Default = 0,
         /// <summary> Whether the sequence is included in the returned substring </summary>
         IncludeSeq = 1 << 0,
         /// <summary> OrdinalIgnoreCase </summary>
         IgnoreCase = 1 << 1,
         /// <summary> If operation fails, return the original input string. </summary>
         RetInput = 1 << 2
      }

      public static string SubstrAfterLast(this string input, string seq, SubstrOptions opts = SubstrOptions.Default) {
         if (input?.Length > 0 && seq?.Length > 0) {
            int index = input.LastIndexOf(seq, (opts & SubstrOptions.IgnoreCase) > 0 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            if (index >= 0) {
               if ((opts & SubstrOptions.IncludeSeq) == 0) { index += seq.Length; }
               return input.Substring(index);
            }
         }
         return (opts & SubstrOptions.RetInput) > 0 ? input : null;
      }


      internal static SyntaxNode FindOuterMostNode(this SyntaxNode rootNode, TextSpan span, bool bTrivia) {
			SyntaxNode node = rootNode.FindToken(span.Start, bTrivia).Parent;
			SyntaxNode parent;
			while (node != null && (parent = node.Parent) != null) {
				TextSpan nodespan = node.FullSpan;
				if (span.Start < nodespan.Start || nodespan.End < span.End || parent != rootNode && nodespan.Length == parent.FullSpan.Length) {
					node = parent;
				}	else {
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

      internal static bool ImplementsInterface(this IMethodSymbol symbol) {
         if (symbol.ExplicitInterfaceImplementations.Length > 0) {
            return true;
         }
         var type = symbol.ContainingType;
         var interfaces = type.Interfaces;        
         for (var i = 0; i < interfaces.Length; i++) {
            var mems = interfaces[i].GetMembers(symbol.Name.SubstrAfterLast(".", SubstrOptions.RetInput));
            for (var j = 0; j < mems.Length; j++) {
               IMethodSymbol member = mems[j] as IMethodSymbol;            
               if (member != null && symbol.ReturnType == member.ReturnType) {
                  if (symbol.Parameters.Length == member.Parameters.Length) {
                     var impl = type.FindImplementationForInterfaceMember(member);
                     if (impl != null && impl.Equals(symbol)) {
                        return true;
                     }
                  }
               }
            }
         }
         return false;
      }


      internal static bool ImplementsInterface(this IPropertySymbol symbol) {
         if (symbol.ExplicitInterfaceImplementations.Length > 0) {
            return true;
         }
         var type = symbol.ContainingType;
         var interfaces = type.Interfaces;
         for (var i = 0; i < interfaces.Length; i++) {
            var mems = interfaces[i].GetMembers(symbol.Name.SubstrAfterLast(".", SubstrOptions.RetInput));
            for (var j = 0; j < mems.Length; j++) {
               IPropertySymbol member = mems[j] as IPropertySymbol;
               if (member != null && symbol.Type == member.Type) {
                  if (symbol.Parameters.Length == member.Parameters.Length) {
                     var impl = type.FindImplementationForInterfaceMember(member);
                     if (impl != null && impl.Equals(symbol)) {
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
