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
using Microsoft.VisualStudio.Threading;

namespace ZMBA.SyntaxColorizer.Clazzifier {

  [ContentType("Basic")]
  [ContentType("CSharp")]
  [Export(typeof(ITaggerProvider))]
  [TagType(typeof(IClassificationTag))]
  internal sealed class VBCSClassificationTagProvider : ITaggerProvider {
    public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
      //Don't need to pass the buffer because we aren't doing anything too complicated. 
      //Would need it if we had to do some intense tagging on a background thread.
      return (ITagger<T>)buffer.Properties.GetOrCreateSingletonProperty(() => new VBCSTagClassifierService(buffer));
    }
  }

  internal class VBCSTagClassifierService : VBCSEventListener, ITagger<ClassificationTag>, IAccurateTagger<ClassificationTag>, IDisposable {
    private static readonly List<TagSpan<ClassificationTag>> EmptyTagList = new List<TagSpan<ClassificationTag>>(0);
    private readonly ITextBuffer _buffer;
    private readonly Workspace _ws;
       

    internal VBCSTagClassifierService(ITextBuffer buffer) {
      this._buffer = buffer;
      this._ws = buffer.GetWorkspace();
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection snapshots) {
      if (snapshots.Count == 0) { return EmptyTagList; }

      EnumerateOnceCachedArray<TagSpan<ClassificationTag>> eoca_ts_ct = null;
      for (var i = 0; i < snapshots.Count; i++) {
        ClassifyTagsForSnapshot(snapshots[i], ref eoca_ts_ct);
      }
      if (eoca_ts_ct == null) { 
        return EmptyTagList; 
      }
      if (eoca_ts_ct.Count == 0) {
        EnumerateOnceCachedArray<TagSpan<ClassificationTag>>.Return(ref eoca_ts_ct);
        return EmptyTagList;
      }
      return eoca_ts_ct;
    }

    public IEnumerable<ITagSpan<ClassificationTag>> GetAllTags(NormalizedSnapshotSpanCollection spans, CancellationToken cancel) {
      return EmptyTagList;
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private void ClassifyTagsForSnapshot(SnapshotSpan snapspan, ref EnumerateOnceCachedArray<TagSpan<ClassificationTag>> eoca_ts_ct) {
      DocumentWorkspace docws = default;
      if (DocumentWorkspace.TryGetDocumentWorkspace(ref snapspan, ref docws)) {
        if (eoca_ts_ct == null) { eoca_ts_ct = EnumerateOnceCachedArray<TagSpan<ClassificationTag>>.Take(); }
        ClassifyTagsForDocumentWorkspace(ref docws, eoca_ts_ct);
      }
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private void ClassifyTagsForDocumentWorkspace(ref DocumentWorkspace docws, EnumerateOnceCachedArray<TagSpan<ClassificationTag>> eoca_ts_ct) {
      TaggingContext ctx = default;
      ctx.eoca_ts_ct = eoca_ts_ct;
      if (TaggingContext.TryGetContext(ref docws, ref ctx)) {
        PerformClassifications(in ctx);
      } else {
        ctx.LoadSemanticModelRootNodeSpans(GetSemanticModelRootNodeSpansAsync(ctx.DocWs.Snapspan, ctx.Document, ctx.Workspace).ConfigureAwait(false).GetAwaiter().GetResult());
        PerformClassifications(in ctx);
      }
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private async Task<SemanticModelRootNodeSpans> GetSemanticModelRootNodeSpansAsync(SnapshotSpan spanspan, Document doc, Workspace ws) {
      SemanticModel model = await doc.GetSemanticModelAsync().ConfigureAwait(false);
      SyntaxNode node = await model.SyntaxTree.GetRootAsync().ConfigureAwait(false);
      List<ClassifiedSpan> ls_cs = (List<ClassifiedSpan>)Classifier.GetClassifiedSpans(model, spanspan.TextSpan(), ws);
      return new SemanticModelRootNodeSpans(model, node, ls_cs);
    }

    [MethodImpl(512)] // MethodImplOptions.AggressiveOptimization
    private void PerformClassifications(in TaggingContext ctx) {
      List<ClassifiedSpan> ls_cs = ctx.ClassifiedSpans;
      int count = ls_cs.Count;
      for (var i = 0; i < count; i++) {

      }
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

