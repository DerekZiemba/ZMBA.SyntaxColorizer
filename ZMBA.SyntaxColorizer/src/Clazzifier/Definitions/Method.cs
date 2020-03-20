using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;


namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {


  [Order(After = Member.Key)][Order(After = ClassificationTypeNames.Identifier)][Order(After = ClassificationTypeNames.MethodName)]
  [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
  internal class Method : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".method";

    [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
    [BaseDefinition(Member.Key)][BaseDefinition(ClassificationTypeNames.MethodName)]
    public new static ClassificationTypeDefinition TypeDef;

    public Method() : this(Key) { }

    public Method(string id) : base(id) {
      ForegroundColor = Color.FromRgb(236, 231, 196);
      ForegroundColor = Color.FromRgb(255, 242, 198);
    }

    [Order(After = Method.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Constructor : Method {
      public new const string Key = Method.Key + ".constructor";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Method.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Constructor() : this(Key) { }

      public Constructor(string id) : base(id) {
        IsBold = true;
      }
    } // End Constructor


    [Order(After = Method.Key)][Order(After = ClassificationTypeNames.StaticSymbol)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Static : Method {
      public new const string Key = Method.Key + ".static";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Method.Key)][BaseDefinition(ClassificationTypeNames.StaticSymbol)]
      public new static ClassificationTypeDefinition TypeDef;

      public Static() : this(Key) { }

      public Static(string id) : base(id) {
        IsUnderlined = true;
      }

      [Order(After = Static.Key)][Order(After = ClassificationTypeNames.ExtensionMethodName)]
      [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
      internal class Extension : Static {
        public new const string Key = Static.Key + ".extension";

        [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
        [BaseDefinition(Method.Key)][BaseDefinition(ClassificationTypeNames.ExtensionMethodName)]
        public new static ClassificationTypeDefinition TypeDef;

        public Extension() : this(Key) { }

        public Extension(string id) : base(id) {
          IsUnderlined = false;
          IsItalic = true;
          ForegroundColor = Color.FromArgb(255, 148, 255, 255);
        }
      } // End Static
    } // End Static


    [Order(After = Method.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Virtual : Method {
      public new const string Key = Method.Key + ".virtual";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Method.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Virtual() : this(Key) { }

      public Virtual(string id) : base(id) {

      }
    } // End Static

  } // End Method

} // End Namespace
