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

	internal class ClassifierContext {
		internal ITextSnapshot SnapShot;
		internal Workspace Workspace;
		internal SyntaxTree SyntaxTree;
		internal SyntaxNode RootNode;
		internal SemanticModel SemanticModel;
		internal List<TagSpan<ClassificationTag>> Classified = new List<TagSpan<ClassificationTag>>(64);

		internal static ClassifierContext GetContext(ref ClassifierContext cached, SnapshotSpan snapspan) {
			ITextSnapshot txtsnapshot = snapspan.Snapshot;
			if(txtsnapshot != null) {
				ClassifierContext ctx = cached;
				if(ctx != null && ctx.SnapShot == txtsnapshot) {
					ctx.Classified.Clear();
					return ctx;
				} else { 
					SourceTextContainer srcTextContainer = txtsnapshot.TextBuffer.AsTextContainer();
					Workspace workspace = Workspace.GetWorkspaceRegistration(srcTextContainer).Workspace;
					if(workspace == null) { return null; }
					DocumentId docid = workspace.GetDocumentIdInCurrentContext(srcTextContainer);
					Document doc = workspace.CurrentSolution.WithDocumentText(docid, srcTextContainer.CurrentText, PreservationMode.PreserveIdentity).GetDocument(docid);

					if(doc.SupportsSyntaxTree && doc.SupportsSemanticModel) {
						ctx = new ClassifierContext();
						ctx.SnapShot = txtsnapshot;
						ctx.Workspace = workspace;
						Task<SyntaxTree> syntaxTreeTask = null;
						Task<SyntaxNode> rootNodeTask = null;
						Task<SemanticModel> semanticModelTask = null;

						if(!doc.TryGetSyntaxTree(out ctx.SyntaxTree)) {
							syntaxTreeTask = doc.GetSyntaxTreeAsync();
						}
						if(syntaxTreeTask != null) {
							ctx.SyntaxTree = syntaxTreeTask.ConfigureAwait(false).GetAwaiter().GetResult();
						}
						if(!ctx.SyntaxTree.TryGetRoot(out ctx.RootNode)) {
							rootNodeTask = ctx.SyntaxTree.GetRootAsync();
						}
						if(!doc.TryGetSemanticModel(out ctx.SemanticModel)) {
							semanticModelTask = doc.GetSemanticModelAsync();
						}
						if(rootNodeTask != null) {
							ctx.RootNode = rootNodeTask.ConfigureAwait(false).GetAwaiter().GetResult();
						}
						if(semanticModelTask != null) {
							ctx.SemanticModel = semanticModelTask.ConfigureAwait(false).GetAwaiter().GetResult();
						}
						Interlocked.Exchange(ref cached, ctx);
						return ctx;
					}
				}
			}
			return null;
		}

		internal List<ClassifiedSpan> GetClassifiedSpans(SnapshotSpan snapspan) {
			return (List<ClassifiedSpan>)Classifier.GetClassifiedSpans(this.SemanticModel, new TextSpan(snapspan.Start, snapspan.Length), this.Workspace);
		}

		/** In the future might want to tag a several sub spans, so this can be modified to a enqueue spans. **/
		internal void AssociateTagWithText(ClassificationTag tag, TextSpan text) {
			Classified.Add(new TagSpan<ClassificationTag>(new SnapshotSpan(this.SnapShot, text.Start, text.Length), tag));
		}

	}

}
