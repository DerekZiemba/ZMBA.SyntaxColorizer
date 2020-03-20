using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;


namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {

  //TODO: Pull format settings from a config file eventually. 
  [Order(After = ClassificationTypeNames.Identifier)]
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Key)]
  [Name(Key)]
  [UserVisible(false)]
  internal abstract class ZMBAFormatDefinition : ClassificationFormatDefinition {
    public const string Key = "ZMBA";

    public bool IsUnderlined {
      get {
        return TextDecorations == System.Windows.TextDecorations.Underline;
      }
      set {
        TextDecorations = value ? System.Windows.TextDecorations.Underline : (IsUnderlined ? null : TextDecorations);
      }
    }

    public bool IsOverlined {
      get {
        return TextDecorations == System.Windows.TextDecorations.OverLine;
      }
      set {
        TextDecorations = value ? System.Windows.TextDecorations.OverLine : (IsOverlined ? null : TextDecorations);
      }
    }

    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Key)]
    [BaseDefinition(ClassificationTypeNames.Identifier)]
    public static ClassificationTypeDefinition TypeDef;

    public ZMBAFormatDefinition(string identifier) {
      this.DisplayName = identifier;
    }
  }
}