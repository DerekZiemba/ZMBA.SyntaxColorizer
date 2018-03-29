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
		internal ITextSnapshot SnapShot { get; set; }
		internal Workspace Workspace { get; set; }
		internal SyntaxTree SyntaxTree { get; set; }
		internal SyntaxNode RootNode { get; set; }
		internal SemanticModel SemanticModel { get; set; }
		internal TagSpan<ClassificationTag> TaggedText { get; set; }

		internal static ClassifierContext GetContext(ref ClassifierContext cached, SnapshotSpan snapspan) {
			ITextSnapshot txtsnapshot = snapspan.Snapshot;
			if (txtsnapshot == null) { return null; }
			if (cached != null && cached.SnapShot == txtsnapshot) { return cached; }

			ITextBuffer buffer = txtsnapshot.TextBuffer;
			SourceTextContainer srcTextContainer = buffer.AsTextContainer();
			WorkspaceRegistration workRegistration = Workspace.GetWorkspaceRegistration(srcTextContainer);
			Workspace workspace = workRegistration.Workspace;
			if(workspace == null) { return null; }
			DocumentId docid = workspace.GetDocumentIdInCurrentContext(srcTextContainer);
			Solution sol = workspace.CurrentSolution.WithDocumentText(docid, srcTextContainer.CurrentText, PreservationMode.PreserveIdentity);
			Document doc = sol.GetDocument(docid);

			if (doc.SupportsSyntaxTree && doc.SupportsSemanticModel) {
				ClassifierContext ctx = AsyncGetNewContextHelper().ConfigureAwait(false).GetAwaiter().GetResult();
				Interlocked.Exchange(ref cached, ctx);
				return ctx;
			} else {
				return null;
			}
			async Task<ClassifierContext> AsyncGetNewContextHelper() {
				ClassifierContext ctx = new ClassifierContext();
				ctx.SnapShot = txtsnapshot;
				ctx.Workspace = workspace;
				ctx.SyntaxTree = await doc.GetSyntaxTreeAsync().ConfigureAwait(false);
				ConfiguredTaskAwaitable<SyntaxNode> rootNodeTask = ctx.SyntaxTree.GetRootAsync().ConfigureAwait(false);
				ConfiguredTaskAwaitable<SemanticModel> semanticModelTask = doc.GetSemanticModelAsync().ConfigureAwait(false);
				ctx.RootNode = await rootNodeTask;
				ctx.SemanticModel = await semanticModelTask;
				return ctx;
			}
		}

		internal List<ClassifiedSpan> GetClassifiedSpans(SnapshotSpan snapspan) {
			IEnumerable<ClassifiedSpan> result = Classifier.GetClassifiedSpans(this.SemanticModel, new TextSpan(snapspan.Start, snapspan.Length), this.Workspace);
			return (List<ClassifiedSpan>)result;
		}

		/** In the future might want to tag a several sub spans, so this can be modified to a enqueue spans. **/
		internal void AssociateTagWithText(ClassificationTag tag, TextSpan text) {
			TaggedText = new TagSpan<ClassificationTag>(new SnapshotSpan(this.SnapShot, text.Start, text.Length), tag);
		}

	}

}
