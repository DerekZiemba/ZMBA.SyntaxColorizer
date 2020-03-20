using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace ZMBA.SyntaxColorizer.Clazzifier {
  internal ref struct DocumentWorkspace {
    internal Workspace Workspace;
    internal Document Document;
    internal SnapshotSpan Snapspan;

    internal ITextSnapshot Snapshot => Snapspan.Snapshot;

    internal static bool TryGetDocumentWorkspace(ref SnapshotSpan snapspan, ref DocumentWorkspace docws) {
      SourceText st = snapspan.Snapshot.AsText();
      if (Workspace.TryGetWorkspace(st.Container, out Workspace ws)) {
        DocumentId docid = ws.GetDocumentIdInCurrentContext(st.Container);
        //ws.WorkspaceChanged;
        //ws.DocumentActiveContextChanged
        if (docid != null && ws.CurrentSolution.ContainsDocument(docid)) {
          Solution sol = ws.CurrentSolution.WithDocumentText(docid, st, PreservationMode.PreserveIdentity);
          Project proj = sol.GetProject(docid.ProjectId);
          if (proj != null) {
            Document doc = proj.GetDocument(docid);
            if (doc != null && doc.SupportsSyntaxTree && doc.SupportsSemanticModel) {
              docws.Workspace = ws;
              docws.Document = doc;
              docws.Snapspan = snapspan;
              return true;
            }
          }
        }
      }
      return false;
    }
  }

  internal struct SemanticModelRootNodeSpans {
    internal SemanticModel SemanticModel;
    internal SyntaxNode RootNode;
    internal List<ClassifiedSpan> ClassifiedSpans;

    internal SemanticModelRootNodeSpans(SemanticModel sm, SyntaxNode node, List<ClassifiedSpan> ls) {
      this.SemanticModel = sm;
      this.RootNode = node;
      this.ClassifiedSpans = ls;
    }
  }


  internal ref partial struct TaggingContext {
    internal DocumentWorkspace DocWs;
    internal SemanticModel SemanticModel;
    internal SyntaxNode RootNode;

    internal List<ClassifiedSpan> ClassifiedSpans;
    internal EnumerateOnceCachedArray<TagSpan<ClassificationTag>> eoca_ts_ct;

    internal ITextSnapshot Snapshot => DocWs.Snapshot;
    internal Workspace Workspace => DocWs.Workspace;
    internal Document Document => DocWs.Document;
    internal SyntaxTree SyntaxTree => SemanticModel.SyntaxTree;
    internal TextSpan TextSpan => DocWs.Snapspan.TextSpan();
    internal Definitions.ClassificationTags Tags => Definitions.ClassificationTags.Instance;

    internal void LoadSemanticModelRootNodeSpans(SemanticModelRootNodeSpans sm_rns) {
      this.SemanticModel = sm_rns.SemanticModel;
      this.RootNode = sm_rns.RootNode;
      this.ClassifiedSpans = sm_rns.ClassifiedSpans;
    }

    internal static bool TryGetContext(ref DocumentWorkspace docws, ref TaggingContext ctx) {
      ctx.DocWs = docws;
      if (ctx.Document.TryGetSemanticModel(out ctx.SemanticModel)) {
        if (ctx.SyntaxTree.TryGetRoot(out ctx.RootNode)) {
          ctx.ClassifiedSpans = (List<ClassifiedSpan>)Classifier.GetClassifiedSpans(ctx.SemanticModel, ctx.TextSpan, ctx.Workspace);
          return ctx.ClassifiedSpans.Count > 0;
        }
      }
      return false;
    }

  }



  //internal class DocumentCache {
  //  private static readonly ConditionalWeakTable<ITextSnapshot, DocumentCache> _weakTable = new ConditionalWeakTable<ITextSnapshot, DocumentCache>();

  //  internal Workspace WS;
  //  internal Document Doc;
  //  internal DocumentCache(Workspace work_space, Document document) {
  //    this.WS = work_space;
  //    this.Doc = document;
  //  }

  //  [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
  //  internal static DocumentCache GetCachedDocument(ITextSnapshot snapshot) {
  //    if (_weakTable.TryGetValue(snapshot, out DocumentCache ctx) && ctx != null) { return ctx; }

  //    SourceText st = snapshot.AsText();
  //    if (!Workspace.TryGetWorkspace(st.Container, out var ws)) { return null; }

  //    DocumentId docid = ws.GetDocumentIdInCurrentContext(st.Container);
  //    if (docid == null || !ws.CurrentSolution.ContainsDocument(docid)) { return null; }

  //    Solution sol = ws.CurrentSolution.WithDocumentText(docid, st, PreservationMode.PreserveIdentity);
  //    Project proj = sol.GetProject(docid.ProjectId);
  //    if (proj == null) { return null; }

  //    Document doc = proj.GetDocument(docid);
  //    if (doc == null) { return null; }

  //    ctx = new DocumentCache(ws, doc);
  //    _weakTable.Add(snapshot, ctx);
  //    return ctx;
  //  }
  //}

}
