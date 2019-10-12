using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;

namespace ZMBA.SyntaxColorizer {
  internal class ClassifierContext : IDisposable {
    internal SnapshotSpan SnapSpan;
    internal ITextSnapshot SnapShot;
    internal Workspace Workspace;
    internal Document Document;
    internal SemanticModel SemanticModel;
    internal SyntaxNode RootNode;
    internal List<ClassifiedSpan> ClassifiedSpans;

    internal SyntaxTree SyntaxTree { get => SemanticModel.SyntaxTree; }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    internal static ClassifierContext GetContext(ref SnapshotSpan snapspan) {
      ITextSnapshot snapshot = snapspan.Snapshot;
      CachedContext cached = CachedContext.GetCachedContext(snapshot);
      if (cached == null) { return null; }
      if (!cached.Doc.SupportsSyntaxTree || !cached.Doc.SupportsSemanticModel) { return null; }
      ClassifierContext ctx = new ClassifierContext(ref snapspan, cached.WS, cached.Doc);
      if (ctx.Document.TryGetSemanticModel(out ctx.SemanticModel)) {
        if (ctx.SyntaxTree.TryGetRoot(out ctx.RootNode)) {
          ctx.ClassifiedSpans = ctx.GetClassifiedSpans();
          return ctx;
        }
      }
      return InitAsync(ctx).ConfigureAwait(false).GetAwaiter().GetResult();
    }
    private ClassifierContext(ref SnapshotSpan snapspan, Workspace ws, Document doc) {
      this.SnapSpan = snapspan;
      this.SnapShot = snapspan.Snapshot;
      this.Workspace = ws;
      this.Document = doc;
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private List<ClassifiedSpan> GetClassifiedSpans() {
       return (List<ClassifiedSpan>)Classifier.GetClassifiedSpans(this.SemanticModel, new TextSpan(SnapSpan.Start, SnapSpan.Length), this.Workspace);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private static async Task<ClassifierContext> InitAsync(ClassifierContext ctx) {
      if (ctx.SemanticModel == null) { ctx.SemanticModel = await ctx.Document.GetSemanticModelAsync().ConfigureAwait(false); }
      if (ctx.RootNode == null) { ctx.RootNode = await ctx.SyntaxTree.GetRootAsync().ConfigureAwait(false); }
      ctx.ClassifiedSpans = ctx.GetClassifiedSpans();
      return ctx;
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls
    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          this.SnapSpan = default;
          this.SnapShot = null;
          this.Workspace = null;
          this.Document = null;
          this.SemanticModel = null;
          this.RootNode = null;
        }
        disposedValue = true;
      }
    }
    // This code added to correctly implement the disposable pattern.
    public void Dispose() {
      Dispose(true);
    }
    #endregion

    private class CachedContext {
      private static readonly ConditionalWeakTable<ITextSnapshot, CachedContext> _weakTable = new ConditionalWeakTable<ITextSnapshot, CachedContext>();

      internal Workspace WS;
      internal Document Doc;
      internal CachedContext(Workspace work_space, Document document) {
        this.WS = work_space;
        this.Doc = document;
      }

      [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
      internal static CachedContext GetCachedContext(ITextSnapshot snapshot) {
        if (_weakTable.TryGetValue(snapshot, out CachedContext ctx) && ctx != null) { return ctx; }

        SourceTextContainer stc = snapshot.TextBuffer.AsTextContainer();
        Workspace ws = Workspace.GetWorkspaceRegistration(stc).Workspace;
        if (ws == null) { return null; }
        DocumentId docid = ws.GetDocumentIdInCurrentContext(stc);
        if (docid == null) { return null; }
        Document doc = ws.CurrentSolution.WithDocumentText(docid, stc.CurrentText, PreservationMode.PreserveIdentity).GetDocument(docid);
        if (doc == null) { return null; }

        ctx = new CachedContext(ws, doc);
        _weakTable.Add(snapshot, ctx);
        return ctx;
      }
    }
  }
}
