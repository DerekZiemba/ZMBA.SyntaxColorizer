using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;


namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {

  [Order(After = ClassificationTypeNames.Identifier)]
  [Export(typeof(EditorFormatDefinition))]
  [ClassificationType(ClassificationTypeNames = Key)]
  [Name(Key)]
  [UserVisible(true)]
  internal class Identifier : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".identifier";

    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Key)]
    [BaseDefinition(ClassificationTypeNames.Identifier)]
    public new static ClassificationTypeDefinition TypeDef;

    public Identifier() : this(Key) { }

    public Identifier(string id) : base(id) {
      ForegroundColor = Color.FromArgb(200, 240, 240, 240);
    }



    [Order(After = Identifier.Key)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Attribute : Identifier {
      public new const string Key = Identifier.Key + ".attribute";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Identifier.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Attribute() : this(Key) { }

      public Attribute(string id) : base(id) {
        IsItalic = true;
      }
    } // End Attribute

    [Order(After = Identifier.Key)]
    [Order(After = ClassificationTypeNames.EventName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Event : Identifier {
      public new const string Key = Identifier.Key + ".event";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Identifier.Key)]
      [BaseDefinition(ClassificationTypeNames.EventName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Event() : this(Key) { }

      public Event(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 168, 232);
        IsBold = false;
      }
    } // End Event

    [Order(After = Identifier.Key)]
    [Order(After = ClassificationTypeNames.LabelName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Label : Identifier {
      public new const string Key = Identifier.Key + ".label";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Identifier.Key)]
      [BaseDefinition(ClassificationTypeNames.LabelName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Label() : this(Key) { }

      public Label(string id) : base(id) {
        IsBold = true;
      }
    } // End Label



    [Order(After = Identifier.Key)]
    [Order(After = ClassificationTypeNames.NamespaceName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Namespace : Identifier {
      public new const string Key = Identifier.Key + ".namespace";
      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Identifier.Key)]
      [BaseDefinition(ClassificationTypeNames.NamespaceName)]
      public new static ClassificationTypeDefinition TypeDef;
      public Namespace() : this(Key) { }
      public Namespace(string id) : base(id) {
        ForegroundColor = Color.FromRgb(135, 150, 194);
      }
    } // End Namespace

  } // End Identifier
} // End Namespace

