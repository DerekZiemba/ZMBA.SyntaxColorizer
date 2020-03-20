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
  [UserVisible(false)]
  internal class Syntax : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".syntax";

    [Export(typeof(ClassificationTypeDefinition))]
    [Name(Key)]
    [BaseDefinition(ClassificationTypeNames.Identifier)]
    public new static ClassificationTypeDefinition TypeDef;

    public Syntax() : this(Key) { }

    public Syntax(string id) : base(id) {
      ForegroundColor = Color.FromRgb(253, 252, 247);
    }


    [Order(After = Syntax.Key)]
    [Order(After = ClassificationTypeNames.Comment)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Comment : Syntax {
      public new const string Key = Syntax.Key + ".comment";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Syntax.Key)]
      [BaseDefinition(ClassificationTypeNames.Comment)]
      public new static ClassificationTypeDefinition TypeDef;

      public Comment() : this(Key) { }

      public Comment(string id) : base(id) {
        ForegroundColor = Color.FromRgb(102, 116, 123);
      }
    } // End Comment


    [Order(After = Syntax.Key)]
    [Order(After = ClassificationTypeNames.NumericLiteral)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Number : Syntax {
      public new const string Key = Syntax.Key + ".number";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Syntax.Key)]
      [BaseDefinition(ClassificationTypeNames.NumericLiteral)]
      public new static ClassificationTypeDefinition TypeDef;

      public Number() : this(Key) { }

      public Number(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 205, 34);
      }
    } // End Number


    [Order(After = Syntax.Key)]
    [Order(After = ClassificationTypeNames.Keyword)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Keyword : Syntax {
      public new const string Key = Syntax.Key + ".keyword";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Syntax.Key)]
      [BaseDefinition(ClassificationTypeNames.Keyword)]
      public new static ClassificationTypeDefinition TypeDef;

      public Keyword() : this(Key) { }

      public Keyword(string id) : base(id) {
        ForegroundColor = Color.FromRgb(147, 199, 99);
      }


      [Order(After = Keyword.Key)]
      [Order(After = ClassificationTypeNames.ControlKeyword)]
      [Export(typeof(EditorFormatDefinition))]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      [UserVisible(true)]
      internal class Control : Keyword {
        public new const string Key = Keyword.Key + ".control";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        [BaseDefinition(Keyword.Key)]
        [BaseDefinition(ClassificationTypeNames.ControlKeyword)]
        public new static ClassificationTypeDefinition TypeDef;

        public Control() : this(Key) { }

        public Control(string id) : base(id) {

        }


        [Order(After = Control.Key)]
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = Key)]
        [Name(Key)]
        [UserVisible(true)]
        internal class Branch : Control {
          public new const string Key = Control.Key + ".branch";

          [Export(typeof(ClassificationTypeDefinition))]
          [Name(Key)]
          [BaseDefinition(Control.Key)]
          public new static ClassificationTypeDefinition TypeDef;

          public Branch() : this(Key) { }

          public Branch(string id) : base(id) {
            ForegroundColor = Color.FromRgb(216, 160, 223);
          }
        } // End Branch
      } // End Control
    } // End Keyword


    [Order(After = Syntax.Key)]
    [Order(After = ClassificationTypeNames.Operator)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Operator : Syntax {
      public new const string Key = Syntax.Key + ".operator";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Syntax.Key)]
      [BaseDefinition(ClassificationTypeNames.Operator)]
      public new static ClassificationTypeDefinition TypeDef;

      public Operator() : this(Key) { }

      public Operator(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 255, 128);
      }

      [Order(After = Operator.Key)]
      [Order(After = Overloaded.Key)]
      [Export(typeof(EditorFormatDefinition))]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      [UserVisible(true)]
      internal class UserDefined : Operator {
        public new const string Key = Operator.Key + "-userdefined";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        [BaseDefinition(Operator.Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public UserDefined() : this(Key) { }

        public UserDefined(string id) : base(id) {
          IsItalic = true;
          ForegroundColor = Color.FromArgb(255, 0, 240, 240);
        }
      } // End UserDefined Operator

      [Order(After = Operator.Key)]
      [Order(After = ClassificationTypeNames.OperatorOverloaded)]
      [Export(typeof(EditorFormatDefinition))]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      [UserVisible(true)]
      internal class Overloaded : Operator {
        public new const string Key = Operator.Key + ".overloaded";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        [BaseDefinition(Operator.Key)]
        [BaseDefinition(ClassificationTypeNames.OperatorOverloaded)]
        public new static ClassificationTypeDefinition TypeDef;

        public Overloaded() : this(Key) { }

        public Overloaded(string id) : base(id) {
          IsItalic = true;
          IsUnderlined = true;
        }
      } // End Overloaded Operator


    } // End Operator


    [Order(After = Syntax.Key)]
    [Order(After = ClassificationTypeNames.Punctuation)]
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    [UserVisible(true)]
    internal class Punctuation : Syntax {
      public new const string Key = Syntax.Key + ".punctuation";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      [BaseDefinition(Syntax.Key)]
      [BaseDefinition(ClassificationTypeNames.Punctuation)]
      public new static ClassificationTypeDefinition TypeDef;

      public Punctuation() : this(Key) { }

      public Punctuation(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 128, 128);
      }

      [Order(After = Punctuation.Key)]
      [Export(typeof(EditorFormatDefinition))]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      [UserVisible(true)]
      internal class Braces : Punctuation {
        public new const string Key = Punctuation.Key + ".braces";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        [BaseDefinition(Punctuation.Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Braces() : this(Key) { }

        public Braces(string id) : base(id) {

        }
      } // End Bracket

      [Order(After = Punctuation.Key)]
      [Export(typeof(EditorFormatDefinition))]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      [UserVisible(true)]
      internal class Brackets : Punctuation {
        public new const string Key = Punctuation.Key + ".bracket";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        [BaseDefinition(Punctuation.Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Brackets() : this(Key) { }

        public Brackets(string id) : base(id) {

        }
      } // End Bracket

      [Order(After = Punctuation.Key)]
      [Export(typeof(EditorFormatDefinition))]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      [UserVisible(true)]
      internal class Parenthesis : Punctuation {
        public new const string Key = Punctuation.Key + ".parenthesis";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        [BaseDefinition(Punctuation.Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Parenthesis() : this(Key) { }

        public Parenthesis(string id) : base(id) {

        }
      } // End Parenthesis
    } // End Punctuation

  } // End Syntax
} // End Namespace
