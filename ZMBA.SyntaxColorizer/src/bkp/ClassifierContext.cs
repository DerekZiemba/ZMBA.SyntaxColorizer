using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace ZMBA.SyntaxColorizer.Clazzifier {
  internal class ClassifierContext : IDisposable {
    internal SnapshotSpan SnapSpan;
    internal ITextSnapshot SnapShot;
    internal Workspace Workspace;
    internal Document Document;
    internal SemanticModel SemanticModel;
    internal SyntaxNode RootNode;
    internal List<ClassifiedSpan> ClassifiedSpans;
    internal List<TagSpan<ClassificationTag>> TaggedSpans;

    internal SyntaxTree SyntaxTree { get => SemanticModel.SyntaxTree; }

    private ClassifierContext(ref SnapshotSpan snapspan, Workspace ws, Document doc) {
      this.SnapSpan = snapspan;
      this.SnapShot = snapspan.Snapshot;
      this.Workspace = ws;
      this.Document = doc;
    }

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
          ctx.TagTheSpans();
          return ctx;
        }
      }

      return InitAsync(ctx).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private static async Task<ClassifierContext> InitAsync(ClassifierContext ctx) {
      if (ctx.SemanticModel == null) { ctx.SemanticModel = await ctx.Document.GetSemanticModelAsync().ConfigureAwait(false); }
      if (ctx.RootNode == null) { ctx.RootNode = await ctx.SyntaxTree.GetRootAsync().ConfigureAwait(false); }
      ctx.ClassifiedSpans = ctx.GetClassifiedSpans();
      ctx.TagTheSpans();
      return ctx;
    }

    [MethodImpl(256 | 512)] //[MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private List<ClassifiedSpan> GetClassifiedSpans(CancellationToken token = default) {
       return (List<ClassifiedSpan>)Classifier.GetClassifiedSpans(this.SemanticModel, new TextSpan(SnapSpan.Start, SnapSpan.Length), this.Workspace, token);
    }

    private void TagTheSpans() {
      if (this.ClassifiedSpans == null) { return; }
      this.TaggedSpans = new List<TagSpan<ClassificationTag>>(this.ClassifiedSpans.Count + 8);
      for (var i = 0; i < this.ClassifiedSpans.Count; i++) {

      }
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
          this.ClassifiedSpans = null;
          this.TaggedSpans = null;
        }
        disposedValue = true;
      }
    }
    // This code added to correctly implement the disposable pattern.
    public void Dispose() {
      Dispose(true);
    }
    #endregion


  }
}
