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

  internal class VBCSTagClassifier : ITagger<ClassificationTag> {
#pragma warning disable CS0067 // The event is never used
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore CS0067 // The event is never used

    private readonly FormattingTags Tags;
    private readonly ITextBuffer Buffer;

    internal VBCSTagClassifier(IClassificationTypeRegistryService registry, ITextBuffer buffer) {
      this.Tags = new FormattingTags(registry);
      this.Buffer = buffer;
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection snapshots) {
      for (var i = 0; i < snapshots.Count; i++) {
        SnapshotSpan snapspan = snapshots[i];
        ClassifierContext ctx = ClassifierContext.GetContext(ref snapspan);
        if (ctx != null && ctx.ClassifiedSpans != null) {
          for (var idx = 0; idx < ctx.ClassifiedSpans.Count; idx++) {
            ClassifiedSpan span = ctx.ClassifiedSpans[idx];
            TagSpan<ClassificationTag> tag = Classify(ctx, ref span);
            if (tag != null) { yield return tag; }
            //if (tag == null) { System.Diagnostics.Debugger.Break(); }            
          }
          ctx.Dispose();
        }
      }
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private TagSpan<ClassificationTag> Classify(ClassifierContext context, ref ClassifiedSpan span) {
      switch (span.ClassificationType) {
        case "string": return ClassifyString(context, ref span);
        case "method name": return ClassifyIdentifier(context, ref span);
        case "property name": return ClassifyIdentifier(context, ref span);
        case "identifier": return ClassifyIdentifier(context, ref span);
        case "event name": return Tags.IdentifierEvent.Associate(context.SnapShot, span.TextSpan);
        case "field name": return Tags.IdentifierField.Associate(context.SnapShot, span.TextSpan);
        case "constant name": return Tags.IdentifierConst.Associate(context.SnapShot, span.TextSpan);
        case "local name": return Tags.Variable.Associate(context.SnapShot, span.TextSpan);
        case "parameter name": return Tags.Param.Associate(context.SnapShot, span.TextSpan);
        case "enum member name": return Tags.EnumMember.Associate(context.SnapShot, span.TextSpan);
        case "extension method name": return Tags.MethodExtension.Associate(context.SnapShot, span.TextSpan);
        case "keyword": return Tags.SyntaxKeyword.Associate(context.SnapShot, span.TextSpan);
        case "keyword - control": return ClassifyKeywordControl(context, ref span);
        case "operator": return ClassifyOperator(context, ref span);
        case "operator - overloaded": return ClassifyOperatorOverloaded(context, ref span);
        case "number": return Tags.SyntaxNumber.Associate(context.SnapShot, span.TextSpan);
        case "punctuation": return Tags.SyntaxPunctuation.Associate(context.SnapShot, span.TextSpan);
        case "comment": return Tags.SyntaxComment.Associate(context.SnapShot, span.TextSpan);
        case "class name": return Tags.TypeClass.Associate(context.SnapShot, span.TextSpan);
        case "module name": return Tags.TypeModule.Associate(context.SnapShot, span.TextSpan);
        case "struct name": return Tags.TypeStructure.Associate(context.SnapShot, span.TextSpan);
        case "interface name": return Tags.TypeInterface.Associate(context.SnapShot, span.TextSpan);
        case "type parameter name": return Tags.TypeGeneric.Associate(context.SnapShot, span.TextSpan);
        case "delegate name": return Tags.TypeDelegate.Associate(context.SnapShot, span.TextSpan);
        case "enum name": return Tags.Enum.Associate(context.SnapShot, span.TextSpan);
        case "preprocessor keyword": return Tags.Preprocessor.Associate(context.SnapShot, span.TextSpan);
        case "preprocessor text": return Tags.PreprocessorText.Associate(context.SnapShot, span.TextSpan);
        case "namespace name": return Tags.IdentifierNamespace.Associate(context.SnapShot, span.TextSpan);
        case "static symbol":

          return null;
        //case "xml doc comment": break;
        //case "xml doc comment - text": break;
        //case "xml doc comment - name": break;
        //case "xml doc comment - delimiter": break;
        //case "xml doc comment - attribute name": break;
        //case "xml doc comment - attribute value": break;
        //case "xml doc comment - attribute quotes": break;
        //case "xml doc comment - entity reference": break;
        //case "excluded code": break;
        default:
          System.Diagnostics.Debugger.Break();
          break;
      }
      return null;
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    public TagSpan<ClassificationTag> ClassifyKeywordControl(ClassifierContext ctx, ref ClassifiedSpan span) {
      SyntaxNode node = ctx.RootNode.FindOuterMostNode(span.TextSpan, false);
      if (node != null) {
        switch (node.RawKind) {
          case (int)CSKind.ReturnKeyword:
          case (int)CSKind.YieldReturnStatement:
          case (int)CSKind.ReturnStatement:
          case (int)VBKind.ReturnKeyword:
          case (int)VBKind.ReturnStatement:
            return Tags.SyntaxKeyword.Associate(ctx.SnapShot, span.TextSpan);
        }
      }
      return Tags.SyntaxKeywordControl.Associate(ctx.SnapShot, span.TextSpan);
    }


    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    public TagSpan<ClassificationTag> ClassifyString(ClassifierContext ctx, ref ClassifiedSpan span) {
      SyntaxNode node = ctx.RootNode.FindOuterMostNode(span.TextSpan, false);
      if (node != null) {
        switch (node.RawKind) {
          case (int)VBKind.InterpolatedStringExpression:
          case (int)CSKind.InterpolatedVerbatimStringStartToken:
          case (int)CSKind.InterpolatedStringStartToken:
          case (int)CSKind.InterpolatedStringEndToken:
          case (int)CSKind.InterpolatedStringExpression:
            return Tags.StringToken.Associate(ctx.SnapShot, span.TextSpan);
          case (int)VBKind.CharacterLiteralToken:
          case (int)VBKind.CharacterLiteralExpression:
          case (int)VBKind.SingleQuoteToken:
          case (int)CSKind.CharacterLiteralToken:
          case (int)CSKind.CharacterLiteralExpression:
          case (int)CSKind.SingleQuoteToken:
            return Tags.StringSingleQuote.Associate(ctx.SnapShot, span.TextSpan);
          case (int)VBKind.InterpolatedStringTextToken:
          case (int)VBKind.InterpolatedStringText:
          case (int)CSKind.InterpolatedStringTextToken:
          case (int)CSKind.InterpolatedStringText:
          case (int)CSKind.InterpolatedStringToken:
            return Tags.StringInterpolated.Associate(ctx.SnapShot, span.TextSpan);
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
      return Tags.String.Associate(ctx.SnapShot, span.TextSpan);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private TagSpan<ClassificationTag> ClassifyOperator(ClassifierContext ctx, ref ClassifiedSpan span) {
      SyntaxNode node = ctx.RootNode.FindOuterMostNode(span.TextSpan, true);
      if (node != null) {
        ISymbol rawsymbol = ctx.SemanticModel.GetRawSymbol(node);
        if (rawsymbol != null) {
          switch (rawsymbol.Kind) {
            case SymbolKind.Method: return ClassifyIdentifier_Method(ctx, ref span, node, (IMethodSymbol)rawsymbol);
          }
        }
      }
      return Tags.SyntaxOperator.Associate(ctx.SnapShot, span.TextSpan);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private TagSpan<ClassificationTag> ClassifyOperatorOverloaded(ClassifierContext ctx, ref ClassifiedSpan span) {
      SyntaxNode node = ctx.RootNode.FindOuterMostNode(span.TextSpan, true);
      if (node != null) {
        ISymbol rawsymbol = ctx.SemanticModel.GetRawSymbol(node);
        if (rawsymbol != null) {
          switch (rawsymbol.Kind) {
            case SymbolKind.Method: return ClassifyIdentifier_Method(ctx, ref span, node, (IMethodSymbol)rawsymbol);
          }
        }
      }
      return Tags.SyntaxOperatorOverloaded.Associate(ctx.SnapShot, span.TextSpan);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private TagSpan<ClassificationTag> ClassifyIdentifier(ClassifierContext ctx, ref ClassifiedSpan span) {
      SyntaxNode node = ctx.RootNode.FindOuterMostNode(span.TextSpan, true);
      if (node != null) {
        switch (node.RawKind) {
          case (int)VBKind.Attribute:
          case (int)CSKind.Attribute:
            return Tags.IdentifierAttribute.Associate(ctx.SnapShot, span.TextSpan);
          default:
            ISymbol rawsymbol = ctx.SemanticModel.GetRawSymbol(node);
            if (rawsymbol != null) {
              switch (rawsymbol.Kind) {
                case SymbolKind.Field: return ClassifyIdentifier_Field(ctx, ref span, (IFieldSymbol)rawsymbol);
                case SymbolKind.Property: return ClassifyIdentifier_Property(ctx, ref span, (IPropertySymbol)rawsymbol);
                case SymbolKind.Local: return ClassifyIdentifier_Local(ctx, ref span, (ILocalSymbol)rawsymbol);
                case SymbolKind.NamedType: return ClassifyIdentifier_NamedType(ctx, ref span, (INamedTypeSymbol)rawsymbol);
                case SymbolKind.Method: return ClassifyIdentifier_Method(ctx, ref span, node, (IMethodSymbol)rawsymbol);
                case SymbolKind.Parameter: return Tags.Param.Associate(ctx.SnapShot, span.TextSpan);
                case SymbolKind.TypeParameter: return Tags.TypeGeneric.Associate(ctx.SnapShot, span.TextSpan);
                case SymbolKind.DynamicType: return Tags.TypeDynamic.Associate(ctx.SnapShot, span.TextSpan);
                case SymbolKind.Namespace: return Tags.IdentifierNamespace.Associate(ctx.SnapShot, span.TextSpan);
                case SymbolKind.Event: return Tags.IdentifierEvent.Associate(ctx.SnapShot, span.TextSpan);
                case SymbolKind.Label: return Tags.IdentifierLabel.Associate(ctx.SnapShot, span.TextSpan);
                case SymbolKind.Preprocessing: return Tags.PreprocessorText.Associate(ctx.SnapShot, span.TextSpan);
              }
            }
            break;
        }
      }
      return Tags.Identifier.Associate(ctx.SnapShot, span.TextSpan);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private TagSpan<ClassificationTag> ClassifyIdentifier_Field<T>(ClassifierContext ctx, ref ClassifiedSpan span, T symbol) where T : IFieldSymbol {
      if (symbol.ContainingType.TypeKind == TypeKind.Enum) {
        return Tags.EnumMember.Associate(ctx.SnapShot, span.TextSpan);
      }
      else if (symbol.IsConst) {
        return Tags.IdentifierConst.Associate(ctx.SnapShot, span.TextSpan);
      }
      return Tags.IdentifierField.Associate(ctx.SnapShot, span.TextSpan);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private TagSpan<ClassificationTag> ClassifyIdentifier_Property<T>(ClassifierContext ctx, ref ClassifiedSpan span, T symbol) where T : IPropertySymbol {
      if (symbol.PropertyImplementsInterface()) {
        return Tags.IdentifierPropertyInterfaceImplementation.Associate(ctx.SnapShot, span.TextSpan);
      }
      return Tags.IdentifierProperty.Associate(ctx.SnapShot, span.TextSpan);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private TagSpan<ClassificationTag> ClassifyIdentifier_Local<T>(ClassifierContext ctx, ref ClassifiedSpan span, T symbol) where T : ILocalSymbol {
      if (symbol.IsConst) {
        return Tags.IdentifierConst.Associate(ctx.SnapShot, span.TextSpan);
      }
      return Tags.Variable.Associate(ctx.SnapShot, span.TextSpan);
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private TagSpan<ClassificationTag> ClassifyIdentifier_NamedType<T>(ClassifierContext ctx, ref ClassifiedSpan span, T symbol) where T : INamedTypeSymbol {
      if (symbol.SpecialType != SpecialType.None) {
        return Tags.Type.Associate(ctx.SnapShot, span.TextSpan);
      }
      return Tags.TypeClass.Associate(ctx.SnapShot, span.TextSpan);
    }


    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private TagSpan<ClassificationTag> ClassifyIdentifier_Method<T>(ClassifierContext ctx, ref ClassifiedSpan span, SyntaxNode node, T symbol) where T : IMethodSymbol {
      switch (node.Parent.RawKind) {
        case (int)VBKind.Attribute:
        case (int)CSKind.Attribute:
        case (int)VBKind.QualifiedName when node.Parent.Parent.RawKind == (int)VBKind.Attribute:
        case (int)CSKind.QualifiedName when node.Parent.Parent.RawKind == (int)CSKind.Attribute:
          return Tags.IdentifierAttribute.Associate(ctx.SnapShot, span.TextSpan);
        default:
          switch (symbol.MethodKind) {
            case MethodKind.Constructor:
            case MethodKind.StaticConstructor:
            case MethodKind.Destructor:
              return Tags.MethodConstructor.Associate(ctx.SnapShot, span.TextSpan);
            case MethodKind.UserDefinedOperator:
              return Tags.MethodUserDefinedOperator.Associate(ctx.SnapShot, span.TextSpan);
            case MethodKind.LocalFunction:
              return Tags.Method.Associate(ctx.SnapShot, span.TextSpan); // TODO:
            default:
              if (symbol.IsExtensionMethod) {
                return Tags.MethodExtension.Associate(ctx.SnapShot, span.TextSpan);
              }
              else if (symbol.IsStatic) {
                return Tags.MethodStatic.Associate(ctx.SnapShot, span.TextSpan);
              }
              else if (symbol.IsVirtual || symbol.IsOverride) {
                return Tags.MethodVirtual.Associate(ctx.SnapShot, span.TextSpan);
              }
              else if (symbol.MethodImplementsInterface()) {
                return Tags.MethodInterfaceImplementation.Associate(ctx.SnapShot, span.TextSpan);
              }
              else {
                return Tags.Method.Associate(ctx.SnapShot, span.TextSpan);
              }

          }
      }
    }
  }


}
