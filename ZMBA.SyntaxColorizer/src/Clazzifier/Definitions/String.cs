using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {

  [Order(After = ClassificationTypeNames.StringLiteral)]
  [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
  internal class String : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".string";

    [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
    [BaseDefinition(ClassificationTypeNames.StringLiteral)]
    public new static ClassificationTypeDefinition TypeDef;

    public String() : this(Key) { }

    public String(string id) : base(id) {
      ForegroundColor = Color.FromRgb(255, 164, 72);
    }


    [Order(After = String.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Singlequote : String {
      public new const string Key = String.Key + ".singlequote";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(String.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Singlequote() : this(Key) { }

      public Singlequote(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 198, 140);
      }
    } // End Singlequotes


    [Order(After = String.Key)][Order(After = ClassificationTypeNames.VerbatimStringLiteral)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Verbatim : String {
      public new const string Key = String.Key + ".verbatim";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(String.Key)][BaseDefinition(ClassificationTypeNames.VerbatimStringLiteral)]
      public new static ClassificationTypeDefinition TypeDef;

      public Verbatim() : this(Key) { }

      public Verbatim(string id) : base(id) {
      }
    } // End Verbatim


    [Order(After = String.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Interpolated : String {
      public new const string Key = String.Key + "-interpolated";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(String.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Interpolated() : this(Key) { }

      public Interpolated(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 255, 180, 120);
      }
    } // End Interpolated

    [Order(After = String.Key)][Order(After = ClassificationTypeNames.StringEscapeCharacter)]
    [Order(After = Singlequote.Key)][Order(After = Verbatim.Key)][Order(After = Interpolated.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Token : String {
      public new const string Key = String.Key + "-token";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(String.Key)][BaseDefinition(ClassificationTypeNames.StringEscapeCharacter)]
      public new static ClassificationTypeDefinition TypeDef;

      public Token() : this(Key) { }

      public Token(string id) : base(id) {
        //ForegroundColor = Color.FromRgb(214,157,133);
        //ForegroundColor = Color.FromRgb(255, 143, 32);
        //ForegroundColor = Color.FromRgb(255, 148, 40);
        ForegroundColor = Color.FromRgb(255, 150, 45);
      }
    } // End Token

  } // End String
} // End Namespace
