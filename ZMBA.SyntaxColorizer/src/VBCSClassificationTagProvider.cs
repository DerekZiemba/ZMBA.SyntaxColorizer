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
}
