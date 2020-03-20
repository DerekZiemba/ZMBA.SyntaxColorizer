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
  internal class Type : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".type";

    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Key)]
    [BaseDefinition(ClassificationTypeNames.Identifier)]
    public new static ClassificationTypeDefinition TypeDef;

    public Type() : this(Key) { }

    public Type(string id) : base(id) {
      ForegroundColor = Color.FromArgb(255, 78, 201, 176);
    }

    [Order(After = Type.Key)]
    [Order(After = ClassificationTypeNames.ClassName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Class : Type {
      public new const string Key = Type.Key + ".class";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      [BaseDefinition(ClassificationTypeNames.ClassName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Class() : this(Key) { }

      public Class(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 125, 157, 189);
      }
    } // End Class


    [Order(After = Type.Key)]
    [Order(After = ClassificationTypeNames.ModuleName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Module : Type {
      public new const string Key = Type.Key + ".module";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      [BaseDefinition(ClassificationTypeNames.ModuleName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Module() : this(Key) { }

      public Module(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 138, 219, 201);
      }
    } // End Module



    [Order(After = Type.Key)]
    [Order(After = ClassificationTypeNames.StructName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Structure : Type {
      public new const string Key = Type.Key + ".structure";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      [BaseDefinition(ClassificationTypeNames.StructName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Structure() : this(Key) { }

      public Structure(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 157, 208, 225);
      }
    } // End Structure


    [Order(After = Type.Key)]
    [Order(After = ClassificationTypeNames.InterfaceName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Interface : Type {
      public new const string Key = Type.Key + ".interface";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      [BaseDefinition(ClassificationTypeNames.InterfaceName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Interface() : this(Key) { }

      public Interface(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 186, 245, 222);
      }
    } // End Interface


    [Order(After = Type.Key)]
    [Order(After = ClassificationTypeNames.DelegateName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Delegate : Type {
      public new const string Key = Type.Key + ".delegate";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      [BaseDefinition(ClassificationTypeNames.DelegateName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Delegate() : this(Key) { }

      public Delegate(string id) : base(id) {
        ForegroundColor = Color.FromRgb(173, 216, 230);
      }
    } // End Delegate;

    [Order(After = Type.Key)]
    [Order(After = ClassificationTypeNames.TypeParameterName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Generic : Type {
      public new const string Key = Type.Key + ".generic";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      [BaseDefinition(ClassificationTypeNames.TypeParameterName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Generic() : this(Key) { }

      public Generic(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 0, 255, 255);
      }
    } // End Generic;


    [Order(After = Type.Key)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Dynamic : Type {
      public new const string Key = Type.Key + ".dynamic";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Dynamic() : this(Key) { }

      public Dynamic(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 180, 180, 180);
      }
    } // End Dynamic;


    [Order(After = Type.Key)]
    [Order(After = ClassificationTypeNames.EnumName)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Enum : Type {
      public new const string Key = Type.Key + ".enum";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Type.Key)]
      [BaseDefinition(ClassificationTypeNames.EnumName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Enum() : this(Key) { }

      public Enum(string id) : base(id) {
        ForegroundColor = Color.FromRgb(222, 210, 227);
      }


      [Order(After = Enum.Key)]
      [Order(After = ClassificationTypeNames.EnumMemberName)]
      [Export(typeof(EditorFormatDefinition))]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      [UserVisible(true)]
      internal class Member : Enum {
        public new const string Key = Enum.Key + ".member";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        [BaseDefinition(Enum.Key)]
        [BaseDefinition(ClassificationTypeNames.EnumMemberName)]
        public new static ClassificationTypeDefinition TypeDef;

        public Member() : this(Key) { }

        public Member(string id) : base(id) {
          IsItalic = true;
          ForegroundColor = Color.FromRgb(215, 186, 216);
        }
      }
    } // End Enum;
  } // End Type
} // End Namespace
