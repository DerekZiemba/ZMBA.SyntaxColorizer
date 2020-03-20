using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;


namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {

  [Order(After = ClassificationTypeNames.Identifier)][Order(After = Type.Key)][Order(After = Syntax.Key)]
  [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)]
  [UserVisible(false)]
  internal class Member : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".member";

    [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
    [BaseDefinition(ClassificationTypeNames.Identifier)]
    public new static ClassificationTypeDefinition TypeDef;

    public Member() : this(Key) { }

    public Member(string id) : base(id) {
      ForegroundColor = Color.FromRgb(255, 255, 255);
    }

    [Order(After = Member.Key)][Order(After = ClassificationTypeNames.ConstantName)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Constant : Member {
      public new const string Key = Member.Key + ".const";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Member.Key)][BaseDefinition(ClassificationTypeNames.ConstantName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Constant() : this(Key) { }

      public Constant(string id) : base(id) {
        IsBold = true;
        ForegroundColor = Color.FromRgb(210, 176, 225);
      }
    } // End Mutated


    [Order(After = Member.Key)][Order(After = ClassificationTypeNames.FieldName)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Field : Member {
      public new const string Key = Member.Key + ".field";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Member.Key)][BaseDefinition(ClassificationTypeNames.FieldName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Field() : this(Key) { }

      public Field(string id) : base(id) {
        //IsItalic = true;
        //ForegroundColor = Color.FromRgb(205, 237, 254);
        //ForegroundColor = Color.FromRgb(238, 213, 187);
        ForegroundColor = Color.FromRgb(228, 244, 255);
      }
    } // End Field


    [Order(After = Member.Key)][Order(After = ClassificationTypeNames.PropertyName)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Property : Member {
      public new const string Key = Member.Key + ".property";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Member.Key)][BaseDefinition(ClassificationTypeNames.PropertyName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Property() : this(Key) { }

      public Property(string id) : base(id) {
        ForegroundColor = Color.FromRgb(205, 237, 254);
      }
    } // End Property

    [Order(After = Member.Key)][Order(After = Field.Key)][Order(After = Property.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class InterfaceImplementation : Member {
      public new const string Key = Member.Key + "-interfaceimplementation";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Member.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public InterfaceImplementation() : this(Key) { }

      public InterfaceImplementation(string id) : base(id) {
        IsItalic = true;
      }
    } // End InterfaceImplementation

  } // End Member
} // End Namespace
