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

namespace ZMBA.SyntaxColorizer.Clazzifier {

  internal class VBCSTagClassifier {
    internal SnapshotSpan Snapspan;
    internal Workspace Ws;
    internal Document Doc;
    internal SemanticModel Model;
    internal SyntaxNode Root;
    internal List<ClassifiedSpan> ClassifiedSpans;
    internal List<TagSpan<ClassificationTag>> TaggedSpans;


    internal ITextSnapshot Snapshot => Snapspan.Snapshot;
    internal SyntaxTree SyntaxTree => Model.SyntaxTree;
    internal TextSpan TextSpan => new TextSpan(Snapspan.Start, Snapspan.Length);
    internal Definitions.ClassificationTags Tags => Definitions.ClassificationTags.Instance;


    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void Associate(TextSpan txt, ClassificationTag tag) {
      TaggedSpans.Add(tag.Associate(Snapshot, txt));
    }

    [MethodImpl(512)]
    private void Associate(ref ClassifiedSpan span, ClassificationTag tag) {
      Associate(span.TextSpan, tag);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private void Classify(ref ClassifiedSpan span) {
      switch (span.ClassificationType) {
        case ClassificationTypeNames.StringLiteral:
        case ClassificationTypeNames.VerbatimStringLiteral:
          ClassifyString(ref span);
          break;

        case ClassificationTypeNames.MethodName:
        case ClassificationTypeNames.PropertyName:
        case ClassificationTypeNames.Identifier:
          ClassifyIdentifier(ref span);
          break;

        case ClassificationTypeNames.Keyword:
          Associate(ref span, Tags.SyntaxKeyword);
          break;

        case ClassificationTypeNames.ControlKeyword:
          ClassifyKeyword(ref span);
          break;

        case ClassificationTypeNames.Operator:
        case ClassificationTypeNames.OperatorOverloaded:
          ClassifyOperator(ref span);
          break;

        case ClassificationTypeNames.Punctuation: Associate(ref span, Tags.SyntaxPunctuation); break;
        case ClassificationTypeNames.ExtensionMethodName: Associate(ref span, Tags.MethodStaticExtension); break;
        case ClassificationTypeNames.Comment: Associate(ref span, Tags.SyntaxComment); break;
        case ClassificationTypeNames.ConstantName: Associate(ref span, Tags.Constant); break;
        case ClassificationTypeNames.ClassName: Associate(ref span, Tags.TypeClass); break;
        case ClassificationTypeNames.DelegateName: Associate(ref span, Tags.TypeDelegate); break;
        case ClassificationTypeNames.EnumName: Associate(ref span, Tags.TypeEnum); break;
        case ClassificationTypeNames.EnumMemberName: Associate(ref span, Tags.TypeEnumMember); break;
        case ClassificationTypeNames.EventName: Associate(ref span, Tags.IdentifierEvent); break;
        case ClassificationTypeNames.FieldName: Associate(ref span, Tags.Field); break;
        case ClassificationTypeNames.InterfaceName: Associate(ref span, Tags.TypeInterface); break;
        case ClassificationTypeNames.LabelName: Associate(ref span, Tags.IdentifierLabel); break;
        case ClassificationTypeNames.LocalName: Associate(ref span, Tags.Variable); break;
        case ClassificationTypeNames.ModuleName: Associate(ref span, Tags.TypeModule); break;
        case ClassificationTypeNames.NamespaceName: Associate(ref span, Tags.IdentifierNamespace); break;
        case ClassificationTypeNames.NumericLiteral: Associate(ref span, Tags.SyntaxNumber); break;
        case ClassificationTypeNames.ParameterName: Associate(ref span, Tags.Parameter); break;
        case ClassificationTypeNames.PreprocessorKeyword: Associate(ref span, Tags.Preprocessor); break;
        case ClassificationTypeNames.PreprocessorText: Associate(ref span, Tags.PreprocessorText); break;
        case ClassificationTypeNames.StringEscapeCharacter: Associate(ref span, Tags.StringToken); break;
        case ClassificationTypeNames.StructName: Associate(ref span, Tags.TypeStructure); break;
        case ClassificationTypeNames.TypeParameterName: Associate(ref span, Tags.TypeGeneric); break;
        default:
          //System.Diagnostics.Debugger.Break();
          break;
      }
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    public void ClassifyKeyword(ref ClassifiedSpan span) {
      SyntaxNode node = Root.FindOuterMostNode(span.TextSpan, false);
      if (node != null) {
        switch (node.RawKind) {
          case (int)CSKind.ReturnKeyword:
          case (int)CSKind.YieldReturnStatement:
          case (int)CSKind.ReturnStatement:
          case (int)VBKind.ReturnKeyword:
          case (int)VBKind.ReturnStatement:
            Associate(ref span, Tags.SyntaxKeyword);
            return;
        }
      }
      Associate(ref span, Tags.SyntaxKeywordControl);
    }


    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    public void ClassifyString(ref ClassifiedSpan span) {
      SyntaxNode node = Root.FindOuterMostNode(span.TextSpan, false);
      if (node != null) {
        switch (node.RawKind) {
          case (int)VBKind.InterpolatedStringExpression:
          case (int)CSKind.InterpolatedVerbatimStringStartToken:
          case (int)CSKind.InterpolatedStringStartToken:
          case (int)CSKind.InterpolatedStringEndToken:
          case (int)CSKind.InterpolatedStringExpression:
            Associate(ref span, Tags.StringToken);
            return;
          case (int)VBKind.CharacterLiteralToken:
          case (int)VBKind.CharacterLiteralExpression:
          case (int)VBKind.SingleQuoteToken:
          case (int)CSKind.CharacterLiteralToken:
          case (int)CSKind.CharacterLiteralExpression:
          case (int)CSKind.SingleQuoteToken:
            Associate(ref span, Tags.StringSingleQuote);
            return;
          case (int)VBKind.InterpolatedStringTextToken:
          case (int)VBKind.InterpolatedStringText:
          case (int)CSKind.InterpolatedStringTextToken:
          case (int)CSKind.InterpolatedStringText:
          case (int)CSKind.InterpolatedStringToken:
            Associate(ref span, Tags.StringInterpolated);
            return;
          case (int)VBKind.StringLiteralToken:
          case (int)VBKind.StringLiteralExpression:
          case (int)VBKind.XmlEntityLiteralToken:
          case (int)VBKind.XmlString:
          case (int)CSKind.StringLiteralToken:
          case (int)CSKind.StringLiteralExpression:
          case (int)CSKind.XmlEntityLiteralToken:
          default:
            break;
        }
      }
      Associate(ref span, Tags.String);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private void ClassifyOperator(ref ClassifiedSpan span) {
      SyntaxNode node = Root.FindOuterMostNode(span.TextSpan, true);
      if (node != null) {
        ISymbol rawsymbol = Model.GetRawSymbol(node);
        if (rawsymbol != null) {
          switch (rawsymbol.Kind) {
            case SymbolKind.Method:
              ClassifyIdentifier_Method(ref span, node, (IMethodSymbol)rawsymbol);
              return;
          }
        }
      }
      Associate(ref span, Tags.SyntaxOperator);
      if (span.ClassificationType == ClassificationTypeNames.OperatorOverloaded) { Associate(ref span, Tags.SyntaxOperatorOverloaded); }
    }


    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private void ClassifyIdentifier(ref ClassifiedSpan span) {
      SyntaxNode node = Root.FindOuterMostNode(span.TextSpan, true);
      if (node != null) {
        switch (node.RawKind) {
          case (int)VBKind.Attribute:
          case (int)CSKind.Attribute:
            Associate(ref span, Tags.IdentifierAttribute); break;
          default:
            ISymbol rawsymbol = Model.GetRawSymbol(node);
            if (rawsymbol != null) {
              switch (rawsymbol.Kind) {
                case SymbolKind.Field: ClassifyIdentifier_Field(ref span, (IFieldSymbol)rawsymbol); return;
                case SymbolKind.Property: ClassifyIdentifier_Property(ref span, (IPropertySymbol)rawsymbol); return;
                case SymbolKind.Local: ClassifyIdentifier_Local(ref span, (ILocalSymbol)rawsymbol); return;
                case SymbolKind.NamedType: ClassifyIdentifier_NamedType(ref span, (INamedTypeSymbol)rawsymbol); return;
                case SymbolKind.Method: ClassifyIdentifier_Method(ref span, node, (IMethodSymbol)rawsymbol); return;
                case SymbolKind.Parameter: Associate(ref span, Tags.Parameter); return;
                case SymbolKind.TypeParameter: Associate(ref span, Tags.TypeGeneric); return;
                case SymbolKind.DynamicType: Associate(ref span, Tags.TypeDynamic); return;
                case SymbolKind.Namespace: Associate(ref span, Tags.IdentifierNamespace); return;
                case SymbolKind.Event: Associate(ref span, Tags.IdentifierEvent); return;
                case SymbolKind.Label: Associate(ref span, Tags.IdentifierLabel); return;
                case SymbolKind.Preprocessing: Associate(ref span, Tags.PreprocessorText); return;
              }
            }
            break;
        }
      }
      Associate(ref span, Tags.Identifier);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void ClassifyIdentifier_Field<T>(ref ClassifiedSpan span, T symbol) where T : IFieldSymbol {
      if (symbol.ContainingType.TypeKind == TypeKind.Enum) {
        Associate(ref span, Tags.TypeEnumMember);
        return;
      }
      else if (symbol.IsConst) {
        Associate(ref span, Tags.Constant);
        return;
      }
      Associate(ref span, Tags.Field);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void ClassifyIdentifier_Property<T>(ref ClassifiedSpan span, T symbol) where T : IPropertySymbol {
      if (symbol.PropertyImplementsInterface()) {
        Associate(ref span, Tags.MemberInterfaceImpl);
        return;
      }
      Associate(ref span, Tags.Property);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void ClassifyIdentifier_Local<T>(ref ClassifiedSpan span, T symbol) where T : ILocalSymbol {
      if (symbol.IsConst) {
        Associate(ref span, Tags.Constant);
        return;
      }
      Associate(ref span, Tags.Variable);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void ClassifyIdentifier_NamedType<T>(ref ClassifiedSpan span, T symbol) where T : INamedTypeSymbol {
      if (symbol.SpecialType != SpecialType.None) {
        Associate(ref span, Tags.Type);
        return;
      }
      Associate(ref span, Tags.TypeClass);
    }


    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private void ClassifyIdentifier_Method<T>(ref ClassifiedSpan span, SyntaxNode node, T symbol) where T : IMethodSymbol {
      switch (node.Parent.RawKind) {
        case (int)VBKind.Attribute:
        case (int)CSKind.Attribute:
          Associate(ref span, Tags.IdentifierAttribute);
          return;
        case (int)VBKind.QualifiedName:
        case (int)CSKind.QualifiedName:
          switch (node.Parent.Parent.RawKind) {
            case (int)VBKind.Attribute:
            case (int)CSKind.Attribute:
              Associate(ref span, Tags.IdentifierAttribute);
              return;
          }
          break;
      }

      switch (symbol.MethodKind) {
        case MethodKind.Constructor:
        case MethodKind.StaticConstructor:
        case MethodKind.Destructor:
          Associate(ref span, Tags.MethodCtor);
          return;
        case MethodKind.UserDefinedOperator:
          Associate(ref span, Tags.SyntaxOperatorUserDefined);
          return;
        case MethodKind.LocalFunction:
          Associate(ref span, Tags.Method);
          return; // TODO:
      }

      if (symbol.IsExtensionMethod) {
        Associate(ref span, Tags.MethodStaticExtension);
        return;
      }
      else if (symbol.IsStatic) {
        Associate(ref span, Tags.MethodStatic);
        return;
      }
      else if (symbol.IsVirtual || symbol.IsOverride) {
        Associate(ref span, Tags.MethodVirtual);
        return;
      }
      else {
        if (symbol.MethodImplementsInterface()) {
          Associate(ref span, Tags.MemberInterfaceImpl);
          return;
        }
      }
      Associate(ref span, Tags.Method);
    }


    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects).
        }
        disposedValue = true;
      }
    }

    public void Dispose() {
      Dispose(true);
    }
    #endregion
  }
}
