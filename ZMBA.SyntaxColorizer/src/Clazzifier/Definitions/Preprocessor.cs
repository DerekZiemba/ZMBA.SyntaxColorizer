using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {

  [Order(After = ClassificationTypeNames.PreprocessorKeyword)][Order(After = ClassificationTypeNames.PreprocessorText)]
  [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
  internal class Preprocessor : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".preprocessor";

    [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
    public new static ClassificationTypeDefinition TypeDef;

    public Preprocessor() : this(Key) { }

    public Preprocessor(string id) : base(id) {
      ForegroundColor = Color.FromRgb(160, 130, 189);
    }

    [Order(After = Preprocessor.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Keyword : ZMBAFormatDefinition {
      public new const string Key = Preprocessor.Key + ".keyword";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Keyword() : this(Key) { }

      public Keyword(string id) : base(id) {

      }
    } // End PreprocessorKeyword


    [Order(After = Preprocessor.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Text : ZMBAFormatDefinition {
      public new const string Key = Preprocessor.Key + ".text";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Text() : this(Key) { }

      public Text(string id) : base(id) {
        ForegroundColor = Color.FromRgb(198, 224, 194);
      }
    } // End PreprocessorText

  } // End Preprocessor

} // End Namespace
