using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

using VB = Microsoft.CodeAnalysis.VisualBasic;
using CS = Microsoft.CodeAnalysis.CSharp;
using VBKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind;
using CSKind = Microsoft.CodeAnalysis.CSharp.SyntaxKind;

namespace ZMBA.SyntaxColorizer {

	internal static class Extensions {


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


	}

}
