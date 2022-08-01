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

  [Export(typeof(ITaggerProvider))]
  [ContentType("Basic")]
  [ContentType("CSharp")]
  [TagType(typeof(IClassificationTag))]
  internal sealed class VBCSClassificationTagProvider : ITaggerProvider {
    [Import] internal IClassificationTypeRegistryService ClassificationRegistry; // Set via MEF

    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      //Don't need to pass the buffer because we aren't doing anything too complicated. 
      //Would need it if we had to do some intense tagging on a background thread.
      return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(() => new VBCSTagClassifier(ClassificationRegistry, buffer));
    }
  }


  internal partial class VBCSTagClassifier : ITagger<ClassificationTag> {
    public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    private readonly FormattingTags Tags;
    private readonly ITextBuffer Buffer;
    //private readonly Dictionary<SnapshotSpan, Context> Completed;

    internal VBCSTagClassifier(IClassificationTypeRegistryService registry, ITextBuffer buffer) {
      this.Tags = new FormattingTags(registry);
      this.Buffer = buffer;
      //this.Completed = new Dictionary<SnapshotSpan, Context>(32);
    }

    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection snapshots) {
      for (var i = 0; i < snapshots.Count; ++i) {
        SnapshotSpan snapshotSpan = snapshots[i];
        ITextSnapshot txtSnapshot = snapshotSpan.Snapshot;
        ITextBuffer txtBuffer = txtSnapshot.TextBuffer;
        SourceTextContainer srcTxtContainer = txtBuffer.AsTextContainer();

        Workspace workspace = null;
        if (!Workspace.TryGetWorkspace(srcTxtContainer, out workspace)) {
          workspace = Workspace.GetWorkspaceRegistration(srcTxtContainer).Workspace;
          if (workspace == null) { break; }
        }

        DocumentId docId = workspace.GetDocumentIdInCurrentContext(srcTxtContainer);
        if (docId == null) { break; }

        Solution solutionCurrent = workspace.CurrentSolution;
        Solution solution = solutionCurrent.WithDocumentText(docId, srcTxtContainer.CurrentText, PreservationMode.PreserveIdentity);
        Document document = solution.GetDocument(docId);
        if (document == null) { break; }
        if (!document.SupportsSemanticModel) { break; }
        if (!document.SupportsSyntaxTree) { break; }

        SemanticModel semanticModel;
        SyntaxNode rootNode;
        if (!document.TryGetSemanticModel(out semanticModel) || !semanticModel.SyntaxTree.TryGetRoot(out rootNode)) {
          Task.Run(() => CompleteInBackgroundThenRaiseTagsChange(snapshotSpan, document));
          break;
        }

        TextSpan txtSpan = new TextSpan(snapshotSpan.Start.Position, snapshotSpan.Length);
        var spans = (List<ClassifiedSpan>)Classifier.GetClassifiedSpans(semanticModel, txtSpan, workspace);

        for (var j = 0; j < spans.Count; ++j) {
          ClassifiedSpan span = spans[j];
          TextSpan txtspan = span.TextSpan;
          ClassificationTag tag = null;
#if DEBUG
          string text = txtSnapshot.GetText(txtspan.Start, txtspan.Length);
#endif
          switch (span.ClassificationType) {
            case "string":
              tag = ClassifyString(Tags, txtspan, rootNode);
              break;
            case "method name":
            case "property name":
            case "identifier":
              tag = ClassifyIdentifier(Tags, txtspan, semanticModel, rootNode);
              break;
            case "keyword - control":
              tag = ClassifyKeywordControl(Tags, txtspan, rootNode);
              break;
            case "operator":
              tag = ClassifyOperator(Tags, txtspan, semanticModel, rootNode);
              break;

            case "operator - overloaded":
              tag = ClassifyOperatorOverloaded(Tags, txtspan, semanticModel, rootNode);
              break;
            case "keyword": tag = Tags.SyntaxKeyword; break;
            case "event name": tag = Tags.IdentifierEvent; break;
            case "field name": tag = Tags.IdentifierField; break;
            case "constant name": tag = Tags.IdentifierConst; break;
            case "local name":
              //Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.LocalSymbol;
              //SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
              //var info = model.GetSymbolInfo(node);
              //var rawsymbol = info.Symbol;
              //var declared = model.GetDeclaredSymbol(node);
              tag = Tags.Variable; break;

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
              break;
          }

          if (tag != null) {
            yield return new TagSpan<ClassificationTag>(new SnapshotSpan(txtSnapshot, txtspan.Start, txtspan.Length), tag);
          }
        }
      }
    }

    private async Task CompleteInBackgroundThenRaiseTagsChange(SnapshotSpan snapshotSpan, Document document) {
      var semanticModel = await document.GetSemanticModelAsync().ConfigureAwait(false);
      var rootNode = await semanticModel.SyntaxTree.GetRootAsync().ConfigureAwait(false);
      if (rootNode != null) {
        TagsChanged(this, new SnapshotSpanEventArgs(snapshotSpan));
      }
    }

    //private class Context {
    //  public FormattingTags Tags;
    //  public ITextBuffer txtBuffer;
    //  public SourceTextContainer srcTxtContainer;
    //  public Workspace workspace;
    //  public DocumentId docId;
    //  public Solution solution;
    //  public Document document;
    //  public SemanticModel semanticModel;
    //  public SyntaxNode rootNode;

    //  public List<TagSpan<ClassificationTag>> GetClassifiedTags(ITextSnapshot txtSnapshot, List<ClassifiedSpan> spans) {
    //    var classifiedSpans = new List<TagSpan<ClassificationTag>>(spans.Count);

    //    return classifiedSpans;
    //  }



      
    //}






    //private static async Task<(SemanticModel, SyntaxNode)> GetSemanticModelAndRootNode(Document document) {
    //  SemanticModel model = null;
    //  SyntaxNode rootNode = null;
    //  if (!document.TryGetSemanticModel(out model)) {
    //    model = await document.GetSemanticModelAsync().ConfigureAwait(false);
    //    if (model != null && document.SupportsSyntaxTree && !model.SyntaxTree.TryGetRoot(out rootNode)) {
    //      rootNode = await model.SyntaxTree.GetRootAsync().ConfigureAwait(false);
    //    }
    //  }
    //  return (model, rootNode);
    //}

    //private static async Task<List<ClassifiedSpan>> GetClassifiedSpansAsync(Document document, SnapshotSpan snapshotSpan) {
    //  TextSpan txtSpan = new TextSpan(snapshotSpan.Start.Position, snapshotSpan.Length);
    //  var classifiedSpans = await Classifier.GetClassifiedSpansAsync(document, txtSpan).ConfigureAwait(false);
    //  return (List<ClassifiedSpan>)classifiedSpans;
    //}


//    [MethodImpl(AGGRESSIVE_OPTIMIZATION)]
//    private async Task LoadTags(SnapshotSpan snapshotSpan) {
//      ITextSnapshot txtSnapshot = snapshotSpan.Snapshot;
//      txtSnapshot.Version
//      ITextBuffer txtBuffer = txtSnapshot.TextBuffer;
//      SourceTextContainer srcTxtContainer = txtBuffer.AsTextContainer();

//      Workspace workspace = null;
//      if (!Workspace.TryGetWorkspace(srcTxtContainer, out workspace)) { 
//        workspace = Workspace.GetWorkspaceRegistration(srcTxtContainer).Workspace;
//        if (workspace == null) { return; }
//      }

//      DocumentId docId = workspace.GetDocumentIdInCurrentContext(srcTxtContainer);
//      if (docId == null) { return; }

//      Solution solutionCurrent = workspace.CurrentSolution;
//      Solution solution = solutionCurrent.WithDocumentText(docId, srcTxtContainer.CurrentText, PreservationMode.PreserveIdentity);
//      Document document = solution.GetDocument(docId);
//      if (document == null) { return; }
//      if (!document.SupportsSemanticModel) { return; }


//      var getClassifiedSpans = GetClassifiedSpansAsync(document, snapshotSpan);
//      var getModelAndRoot = GetSemanticModelAndRootNode(document);

//      List<ClassifiedSpan> spans = await getClassifiedSpans.ConfigureAwait(false);
//      var (model, rootNode) = await getModelAndRoot.ConfigureAwait(false);

//      List<TagSpan<ClassificationTag>> classifiedSpans = new List<TagSpan<ClassificationTag>>(spans.Count);
//      for (var j = 0; j < spans.Count; ++j) {
//        ClassifiedSpan span = spans[j];
//        TextSpan txtspan = span.TextSpan;
//        ClassificationTag tag = null;
//#if DEBUG
//        string text = txtSnapshot.GetText(txtspan.Start, txtspan.Length);
//#endif
//        switch (span.ClassificationType) {
//          case "string":
//            tag = ClassifyString(Tags, txtspan, rootNode);
//            break;
//          case "method name":
//          case "property name":
//          case "identifier":
//            tag = ClassifyIdentifier(Tags, txtspan, model, rootNode);
//            break;
//          case "keyword - control":
//            tag = ClassifyKeywordControl(Tags, txtspan, rootNode);
//            break;
//          case "operator":
//            tag = ClassifyOperator(Tags, txtspan, model, rootNode);
//            break;

//          case "operator - overloaded":
//            tag = ClassifyOperatorOverloaded(Tags, txtspan, model, rootNode);
//            break;
//          case "keyword": tag = Tags.SyntaxKeyword; break;
//          case "event name": tag = Tags.IdentifierEvent; break;
//          case "field name": tag = Tags.IdentifierField; break;
//          case "constant name": tag = Tags.IdentifierConst; break;
//          case "local name":
//            //Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.LocalSymbol;
//            //SyntaxNode node = rootNode?.FindOuterMostNode(txtspan, true);
//            //var info = model.GetSymbolInfo(node);
//            //var rawsymbol = info.Symbol;
//            //var declared = model.GetDeclaredSymbol(node);
//            tag = Tags.Variable; break;

//          case "parameter name": tag = Tags.Param; break;
//          case "enum member name": tag = Tags.EnumMember; break;
//          case "extension method name": tag = Tags.MethodExtension; break;
//          case "number": tag = Tags.SyntaxNumber; break;
//          case "punctuation": tag = Tags.SyntaxPunctuation; break;
//          case "comment": tag = Tags.SyntaxComment; break;
//          case "namespace name": tag = Tags.IdentifierNamespace; break;
//          case "class name": tag = Tags.TypeClass; break;
//          case "module name": tag = Tags.TypeModule; break;
//          case "struct name": tag = Tags.TypeStructure; break;
//          case "interface name": tag = Tags.TypeInterface; break;
//          case "type parameter name": tag = Tags.TypeGeneric; break;
//          case "delegate name": tag = Tags.TypeDelegate; break;
//          case "enum name": tag = Tags.Enum; break;
//          case "preprocessor keyword": tag = Tags.Preprocessor; break;
//          case "preprocessor text": tag = Tags.PreprocessorText; break;
//          case "static symbol":
//          //case "xml doc comment": break;
//          //case "xml doc comment - text": break;
//          //case "xml doc comment - name": break;
//          //case "xml doc comment - delimiter": break;
//          //case "xml doc comment - attribute name": break;
//          //case "xml doc comment - attribute value": break;
//          //case "xml doc comment - attribute quotes": break;
//          //case "xml doc comment - entity reference": break;
//          //case "excluded code": break;
//          default:
//            break;
//        }

//        if (tag != null) {
//          classifiedSpans.Add(new TagSpan<ClassificationTag>(new SnapshotSpan(txtSnapshot, txtspan.Start, txtspan.Length), tag));
//        }
//      }
//      NotifyWorkCompleted(snapshotSpan, classifiedSpans);
//    }





    //protected virtual void HandleBufferChanged(TextContentChangedEventArgs args) {
    //  if (args.Changes.Count == 0)
    //    return;

    //  var temp = TagsChanged;
    //  if (temp == null)
    //    return;

    //  // Combine all changes into a single span so that
    //  // the ITagger<>.TagsChanged event can be raised just once for a compound edit
    //  // with many parts.

    //  ITextSnapshot snapshot = args.After;

    //  int start = args.Changes[0].NewPosition;
    //  int end = args.Changes[args.Changes.Count - 1].NewEnd;

    //  SnapshotSpan totalAffectedSpan = new SnapshotSpan(
    //      snapshot.GetLineFromPosition(start).Start,
    //      snapshot.GetLineFromPosition(end).End);

    //  TagsChanged(this, new SnapshotSpanEventArgs(totalAffectedSpan));
    //}


  } // END Class VBCSTagClassifier

} // END Namespace
