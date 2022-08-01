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

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Immutable;

using static ZMBA.SyntaxColorizer.Extensions;

namespace ZMBA.SyntaxColorizer {



  internal partial class VBCSTagClassifier : ITagger<ClassificationTag> {

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private static ClassificationTag ClassifyString(FormattingTags Tags, TextSpan txtspan, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.String;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, false);
      if (node != null) {
        switch (node.RawKind) {
          case (int)VBKind.InterpolatedStringExpression:
          case (int)CSKind.InterpolatedVerbatimStringStartToken:
          case (int)CSKind.InterpolatedStringStartToken:
          case (int)CSKind.InterpolatedStringEndToken:
          case (int)CSKind.InterpolatedStringExpression:
            tag = Tags.StringToken;
            break;
          case (int)VBKind.CharacterLiteralToken:
          case (int)VBKind.CharacterLiteralExpression:
          case (int)VBKind.SingleQuoteToken:
          case (int)CSKind.CharacterLiteralToken:
          case (int)CSKind.CharacterLiteralExpression:
          case (int)CSKind.SingleQuoteToken:
            tag = Tags.StringSingleQuote;
            break;
          case (int)VBKind.InterpolatedStringTextToken:
          case (int)VBKind.InterpolatedStringText:
          case (int)CSKind.InterpolatedStringTextToken:
          case (int)CSKind.InterpolatedStringText:
          case (int)CSKind.InterpolatedStringToken:
            tag = Tags.StringInterpolated;
            break;
            //case (int)VBKind.StringLiteralToken:
            //case (int)VBKind.StringLiteralExpression:
            //case (int)VBKind.XmlEntityLiteralToken:
            //case (int)VBKind.XmlString:
            //case (int)CSKind.StringLiteralToken:
            //case (int)CSKind.StringLiteralExpression:
            //case (int)CSKind.XmlEntityLiteralToken:
            //default:
            //  break;
        }
      }
      return tag;
    } // END Nested Method ClassifyString


    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
  private static ClassificationTag ClassifyIdentifier(FormattingTags Tags, TextSpan txtspan, SemanticModel model, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.Identifier;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
      if (node != null) {
        switch (node.RawKind) {
          case (int)VBKind.Attribute:
          case (int)CSKind.Attribute:
            tag = Tags.IdentifierAttribute;
            break;
          default:
            ISymbol rawsymbol = model.GetSymbolInfo(node).Symbol ?? model.GetDeclaredSymbol(node);
            if (rawsymbol != null) {
              switch (rawsymbol.Kind) {
                case SymbolKind.Field:
                  tag = ClassifyIdentifier_Field(Tags, (IFieldSymbol)rawsymbol);
                  break;
                case SymbolKind.Property:
                  tag = PropertyImplementsInterface((IPropertySymbol)rawsymbol) ? Tags.IdentifierPropertyInterfaceImplementation : Tags.IdentifierProperty;
                  break;
                case SymbolKind.Local:
                  tag = ClassifyIdentifier_Local(Tags, (ILocalSymbol)rawsymbol);
                  break;
                case SymbolKind.NamedType:
                  tag = ClassifyIdentifier_NamedType(Tags, (INamedTypeSymbol)rawsymbol);
                  break;
                case SymbolKind.Method:
                  tag = ClassifyIdentifier_Method(Tags, (IMethodSymbol)rawsymbol, node);
                  break;
                case SymbolKind.Parameter:
                  tag = Tags.Param;
                  break;
                case SymbolKind.TypeParameter:
                  tag = Tags.TypeGeneric;
                  break;
                case SymbolKind.DynamicType:
                  tag = Tags.TypeDynamic;
                  break;
                case SymbolKind.Namespace:
                  tag = Tags.IdentifierNamespace;
                  break;
                case SymbolKind.Event:
                  tag = Tags.IdentifierEvent;
                  break;
                case SymbolKind.Label:
                  tag = Tags.IdentifierLabel;
                  break;
                case SymbolKind.Preprocessing:
                  tag = Tags.PreprocessorText;
                  break;
              }
            }
            break;
        }
      }
      return tag;


    } // END Nested Method ClassifyIdentifier

    [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
    private static ClassificationTag ClassifyIdentifier_Field<T>(FormattingTags Tags, T symbol) where T : IFieldSymbol {
      if (symbol.ContainingType.TypeKind == TypeKind.Enum) {
        return Tags.EnumMember;
      } else if (symbol.IsConst) {
        return Tags.IdentifierConst;
      } else {
        return Tags.IdentifierField;
      }
    }

    [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
    private static ClassificationTag ClassifyIdentifier_Local<T>(FormattingTags Tags, T symbol) where T : ILocalSymbol {
      if (symbol.IsConst) {
        return Tags.IdentifierConst;
      } else {
        return Tags.Variable;
      }
    }

    [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
    private static ClassificationTag ClassifyIdentifier_NamedType<T>(FormattingTags Tags, T symbol) where T : INamedTypeSymbol {
      if (symbol.SpecialType != SpecialType.None) {
        return Tags.Type;
      } else {
        return Tags.TypeClass;
      }
    }

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private static ClassificationTag ClassifyKeywordControl(FormattingTags Tags, TextSpan txtspan, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.SyntaxKeywordControl;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, false);
      if (node != null) {
        switch (node.RawKind) {
          case (int)CSKind.ReturnKeyword:
          case (int)CSKind.YieldReturnStatement:
          case (int)CSKind.ReturnStatement:
          case (int)VBKind.ReturnKeyword:
          case (int)VBKind.ReturnStatement:
            tag = Tags.SyntaxKeyword;
            break;
        }
      }
      return tag;
    }

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private static ClassificationTag ClassifyOperator(FormattingTags Tags, TextSpan txtspan, SemanticModel model, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.SyntaxOperator;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
      if (node != null) {
        ISymbol rawsymbol = model.GetSymbolInfo(node).Symbol ?? model.GetDeclaredSymbol(node);
        if (rawsymbol != null && rawsymbol.Kind == SymbolKind.Method) {
          tag = ClassifyIdentifier_Method(Tags, (IMethodSymbol)rawsymbol, node);
        }
      }
      return tag;
    } // END Nested Method

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private static ClassificationTag ClassifyOperatorOverloaded(FormattingTags Tags, TextSpan txtspan, SemanticModel model, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.SyntaxOperatorOverloaded;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
      if (node != null) {
        ISymbol rawsymbol = model.GetSymbolInfo(node).Symbol ?? model.GetDeclaredSymbol(node);
        if (rawsymbol != null && rawsymbol.Kind == SymbolKind.Method) {
          tag = ClassifyIdentifier_Method(Tags, (IMethodSymbol)rawsymbol, node);
        }
      }
      return tag;
    } // END Nested Method


    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private static bool PropertyImplementsInterface<T>(T symbol) where T : IPropertySymbol {
      if (symbol.ExplicitInterfaceImplementations.Length > 0) { return true; }

      ImmutableArray<INamedTypeSymbol> interfaces = symbol.ContainingType.Interfaces;
      for (var i = 0; i < interfaces.Length; i++) {
        string fullname = symbol.Name;
        int period = fullname.LastIndexOf('.');
        if (period >= 0) { fullname = fullname.Substring(period); }
        ImmutableArray<ISymbol> mems = interfaces[i].GetMembers(fullname);
        for (var j = 0; j < mems.Length; j++) if (mems[j] is T member) {
            if (ReferenceEquals(symbol.Type, member.Type) && symbol.Parameters.Length == member.Parameters.Length) {
              var impl = symbol.ContainingType.FindImplementationForInterfaceMember(member);
              if (impl != null && symbol.Equals(impl)) {
                return true;
              }
            }
          }
      }
      return false;
    } 


    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private static bool MethodImplementsInterface<T>(T symbol) where T : IMethodSymbol {
      ImmutableArray<INamedTypeSymbol> interfaces = symbol.ContainingType.Interfaces;
      for (var i = 0; i < interfaces.Length; i++) {
        string fullname = symbol.Name;
        int period = fullname.LastIndexOf('.');
        if (period >= 0) { fullname = fullname.Substring(period); }
        ImmutableArray<ISymbol> mems = interfaces[i].GetMembers(fullname);
        for (var j = 0; j < mems.Length; j++) if (mems[j] is T member) {
            if (ReferenceEquals(symbol.ReturnType, member.ReturnType) && symbol.Parameters.Length == member.Parameters.Length) {
              var impl = symbol.ContainingType.FindImplementationForInterfaceMember(member);
              if (impl != null && symbol.Equals(impl)) {
                return true;
              }
            }
          }
      }
      return false;
    } // END Method MethodImplementsInterface


    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    static ClassificationTag ClassifyIdentifier_Method<T>(FormattingTags Tags, T symbol, SyntaxNode node) where T : IMethodSymbol {
      ClassificationTag tag;
      switch (node.Parent.RawKind) {
        case (int)VBKind.Attribute:
        case (int)CSKind.Attribute:
        case (int)VBKind.QualifiedName when node.Parent.Parent.RawKind == (int)VBKind.Attribute:
        case (int)CSKind.QualifiedName when node.Parent.Parent.RawKind == (int)CSKind.Attribute:
          tag = Tags.IdentifierAttribute;
          break;
        default:
          switch (symbol.MethodKind) {
            case MethodKind.Constructor:
            case MethodKind.StaticConstructor:
            case MethodKind.Destructor:
              tag = Tags.MethodConstructor;
              break;
            case MethodKind.UserDefinedOperator:
              tag = Tags.MethodUserDefinedOperator;
              break;
            case MethodKind.BuiltinOperator:
              tag = Tags.SyntaxOperator;
              break;
            case MethodKind.LocalFunction:
              tag = Tags.Method; // TODO:
              break;
            default:
              if (symbol.IsExtensionMethod) {
                tag = Tags.MethodExtension;
              } else if (symbol.IsStatic) {
                tag = Tags.MethodStatic;
              } else if (symbol.IsVirtual || symbol.IsOverride) {
                tag = Tags.MethodVirtual;
              } else {
                tag = symbol.ExplicitInterfaceImplementations.Length > 0 || MethodImplementsInterface(symbol) ? Tags.MethodInterfaceImplementation : Tags.Method;
              }
              break;
          }
          break;
      }
      return tag;


    } // END Method ClassifyIdentifier_Method

  } // END Class VBCSTagClassifier

} // END Namespace
