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

using static ZMBA.SyntaxColorizer.Extensions;
using System.Collections.Immutable;

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


    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private static bool LoadSemantics<TS>(TS snapshot, out Workspace ws, out Document doc, out SemanticModel model, out SyntaxNode rootNode) where TS: ITextSnapshot {
      doc = null;
      model = null;
      rootNode = null;
      SourceTextContainer stc = snapshot.TextBuffer.AsTextContainer();
      
      if (!Workspace.TryGetWorkspace(stc, out ws)) { ws = Workspace.GetWorkspaceRegistration(stc).Workspace; }
      if (ws != null) {
        DocumentId docid = ws.GetDocumentIdInCurrentContext(stc);
        if (docid != null) {
          doc = ws.CurrentSolution.WithDocumentText(docid, stc.CurrentText, PreservationMode.PreserveIdentity).GetDocument(docid);
          if (doc != null && doc.SupportsSemanticModel) {
            if (!doc.TryGetSemanticModel(out model)) { model = doc.GetSemanticModelAsync().ConfigureAwait(false).GetAwaiter().GetResult(); }
            if (doc.SupportsSyntaxTree) {
              if (!model.SyntaxTree.TryGetRoot(out rootNode)) { rootNode = model.SyntaxTree.GetRoot(); }
            }
            return model != null;
          }
        }
      }
      return false;
    }

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection snapshots) {
      for (var i = 0; i < snapshots.Count; i++) {   
        SnapshotSpan snapspan = snapshots[i];
        ITextSnapshot snapshot = snapspan.Snapshot;
        if (LoadSemantics(snapshot, out Workspace ws, out Document doc, out SemanticModel model, out SyntaxNode rootNode)) {
          List<ClassifiedSpan> spans = (List<ClassifiedSpan>)Classifier.GetClassifiedSpans(model, snapspan.ToTextSpan(), ws);
          for (var j = 0; j < spans.Count; j++) {
            ClassifiedSpan span = spans[j];
            TextSpan txtspan = span.TextSpan;
            string type = span.ClassificationType;
            ClassificationTag tag = null;
            switch (type) {
              case "string": 
                tag = ClassifyString(txtspan, rootNode); 
                break;
              case "method name": 
              case "property name":
              case "identifier": 
                tag = ClassifyIdentifier(txtspan, model, rootNode);
                break;
              case "keyword - control": 
                tag = ClassifyKeywordControl(txtspan, rootNode);
                break;
              case "operator": 
                tag = ClassifyOperator(txtspan, model, rootNode);
                break;
              case "operator - overloaded": 
                tag = ClassifyOperatorOverloaded(txtspan, model, rootNode);
                break;
              case "keyword": tag = Tags.SyntaxKeyword; break;
              case "event name": tag = Tags.IdentifierEvent; break;
              case "field name": tag = Tags.IdentifierField; break;
              case "constant name": tag = Tags.IdentifierConst; break;
              case "local name": tag = Tags.Variable; break;
              case "parameter name": tag = Tags.Param; break;
              case "enum member name": tag = Tags.EnumMember; break;
              case "extension method name": tag = Tags.MethodExtension; break;
              case "number": tag = Tags.SyntaxNumber; break;
              case "punctuation": tag = Tags.SyntaxPunctuation; break;
              case "comment": tag = Tags.SyntaxComment; break;
              case "namespace name": tag = Tags.IdentifierNamespace; break;
              case "class name": tag = Tags.TypeClass; break;
              case "module name": tag = Tags.TypeModule; break;
              case "struct name": tag = Tags.TypeStructure; break;
              case "interface name": tag = Tags.TypeInterface; break;
              case "type parameter name": tag = Tags.TypeGeneric; break;
              case "delegate name": tag = Tags.TypeDelegate; break;
              case "enum name": tag = Tags.Enum; break;
              case "preprocessor keyword": tag = Tags.Preprocessor; break;
              case "preprocessor text": tag = Tags.PreprocessorText; break;
              case "static symbol":
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
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
                break;
            }

            if (tag != null) {
              yield return new TagSpan<ClassificationTag>(new SnapshotSpan(snapshot, txtspan.Start, txtspan.Length), tag);
            }
          }
        }
      }
    }



    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private ClassificationTag ClassifyKeywordControl(TextSpan txtspan, SyntaxNode rootNode) {
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
    private ClassificationTag ClassifyString(TextSpan txtspan, SyntaxNode rootNode) {
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
      return tag;
    }

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private ClassificationTag ClassifyOperator(TextSpan txtspan, SemanticModel model, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.SyntaxOperator;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
      if (node != null) {
        ISymbol rawsymbol = model.GetRawSymbol(node);
        if (rawsymbol != null && rawsymbol.Kind == SymbolKind.Method) {
          tag = ClassifyIdentifier_Method((IMethodSymbol)rawsymbol, node);
        }
      }
      return tag;
    }

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private ClassificationTag ClassifyOperatorOverloaded(TextSpan txtspan, SemanticModel model, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.SyntaxOperatorOverloaded;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
      if (node != null) {
        ISymbol rawsymbol = model.GetRawSymbol(node);
        if (rawsymbol != null && rawsymbol.Kind == SymbolKind.Method) {
          tag = ClassifyIdentifier_Method((IMethodSymbol)rawsymbol, node);
        }
      }
      return tag;
    }

    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private ClassificationTag ClassifyIdentifier(TextSpan txtspan, SemanticModel model, SyntaxNode rootNode) {
      ClassificationTag tag = Tags.Identifier;
      SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
      if (node != null) {
        switch (node.RawKind) {
          case (int)VBKind.Attribute:
          case (int)CSKind.Attribute:
            tag = Tags.IdentifierAttribute;
            break;
          default:
            ISymbol rawsymbol = model.GetRawSymbol(node);
            if (rawsymbol != null) {
              switch (rawsymbol.Kind) {
                case SymbolKind.Field: 
                  tag = ClassifyIdentifier_Field((IFieldSymbol)rawsymbol);
                  break;
                case SymbolKind.Property: 
                  tag = PropertyImplementsInterface((IPropertySymbol)rawsymbol) ? Tags.IdentifierPropertyInterfaceImplementation : Tags.IdentifierProperty;
                  break;
                case SymbolKind.Local: 
                  tag = ClassifyIdentifier_Local((ILocalSymbol)rawsymbol);
                  break;
                case SymbolKind.NamedType: 
                  tag = ClassifyIdentifier_NamedType((INamedTypeSymbol)rawsymbol);
                  break;
                case SymbolKind.Method: 
                  tag = ClassifyIdentifier_Method((IMethodSymbol)rawsymbol, node);
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

      [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
      ClassificationTag ClassifyIdentifier_Field<T>(T symbol) where T : IFieldSymbol => symbol.ContainingType.TypeKind == TypeKind.Enum ? Tags.EnumMember : (symbol.IsConst ? Tags.IdentifierConst : Tags.IdentifierField);

      [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
      ClassificationTag ClassifyIdentifier_Local<T>(T symbol) where T: ILocalSymbol => symbol.IsConst ? Tags.IdentifierConst : Tags.Variable;

      [MethodImpl(AGGRESSIVE_INLINE | AGGRESSIVE_OPTIMIZATION)]
      ClassificationTag ClassifyIdentifier_NamedType<T>(T symbol) where T : INamedTypeSymbol => symbol.SpecialType != SpecialType.None ? Tags.Type : Tags.TypeClass;

      [MethodImpl(AGGRESSIVE_OPTIMIZATION)] static bool PropertyImplementsInterface<T>(T symbol) where T : IPropertySymbol {
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
    }




    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
    private ClassificationTag ClassifyIdentifier_Method<T>(T symbol, SyntaxNode node) where T: IMethodSymbol {
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
            case MethodKind.LocalFunction:
              tag = Tags.Method; // TODO:
              break;
            default:
              if (symbol.IsExtensionMethod) {
                tag = Tags.MethodExtension;
              }
              else if (symbol.IsStatic) {
                tag = Tags.MethodStatic;
              }
              else if (symbol.IsVirtual || symbol.IsOverride) {
                tag = Tags.MethodVirtual;
              }
              else {
                tag = symbol.ExplicitInterfaceImplementations.Length > 0 || MethodImplementsInterface(symbol) ? Tags.MethodInterfaceImplementation : Tags.Method;
              }
              break;
          }
          break;
      }
      return tag;

    
      [MethodImpl(AGGRESSIVE_OPTIMIZATION)] static bool MethodImplementsInterface<T>(T symbol) where T : IMethodSymbol {
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
      }
    } 
  }


}
