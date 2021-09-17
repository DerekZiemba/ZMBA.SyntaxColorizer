using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value null
namespace ZMBA.SyntaxColorizer {

  internal static class FormatDefinitions {

    internal static class Syntax {


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal class KeyWord : ZMBAFormatDefinition {
        public const string Key = "ZMBA.syntax.keyword";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public static ClassificationTypeDefinition TypeDef;


        public KeyWord() : base(Key) {

          ForegroundColor = Color.FromRgb(147, 199, 99);
          ForegroundColor = Color.FromRgb(86, 156, 214);
        }
        private KeyWord(string id) : base(id) {

        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = Key)]
        [Name(Key)]
        internal sealed class Control : KeyWord {
          public new const string Key = "ZMBA.syntax.keyword-control";

          [Export(typeof(ClassificationTypeDefinition))]
          [Name(Key)]
          public new static ClassificationTypeDefinition TypeDef;

          public Control() : base(Key) {

            ForegroundColor = Color.FromRgb(255, 127, 127);
          }

        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal class Operator : ZMBAFormatDefinition {
        public const string Key = "ZMBA.syntax.operator";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public static ClassificationTypeDefinition TypeDef;


        public Operator() : this(Key) {
          ForegroundColor = Color.FromRgb(255, 255, 128);
          ForegroundColor = Color.FromRgb(255, 127, 127);
        }

        public Operator(string id) : base(id) {

        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = Key)]
        [Name(Key)]
        internal class Overloaded : ZMBAFormatDefinition {
          public const string Key = "ZMBA.syntax.operator-overloaded";

          [Export(typeof(ClassificationTypeDefinition))]
          [Name(Key)]
          public static ClassificationTypeDefinition TypeDef;


          public Overloaded() : base(Key) {
            ForegroundColor = Color.FromRgb(220, 220, 170);
            ForegroundColor = Color.FromRgb(255, 255, 128);
            IsItalic = true;
          }
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Number : ZMBAFormatDefinition {
        public const string Key = "ZMBA.syntax.number";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public static ClassificationTypeDefinition TypeDef;


        public Number() : base(Key) {
          ForegroundColor = Color.FromRgb(255, 205, 34);
          ForegroundColor = Color.FromRgb(255, 205, 22);
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Punctuation : ZMBAFormatDefinition {
        public const string Key = "ZMBA.syntax.punctuation";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public static ClassificationTypeDefinition TypeDef;


        public Punctuation() : base(Key) {

          ForegroundColor = Color.FromRgb(255, 128, 128);
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Comment : ZMBAFormatDefinition {
        public const string Key = "ZMBA.syntax.comment";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public static ClassificationTypeDefinition TypeDef;


        public Comment() : base(Key) {
          ForegroundColor = Color.FromRgb(102, 116, 123);
        }
      }
    }




    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class Preprocessor : ZMBAFormatDefinition {
      public const string Key = "ZMBA.preprocessor";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private Preprocessor(string id) : base(id) {
        ForegroundColor = Color.FromRgb(160, 130, 189);
      }
      public Preprocessor() : base(Key) {

      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Text : Preprocessor {
        public new const string Key = "ZMBA.preprocessor-text";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Text() : base(Key) {
          ForegroundColor = Color.FromRgb(198, 224, 194);
        }
      }
    }



    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class Type : ZMBAFormatDefinition {
      public const string Key = "ZMBA.type";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private Type(string id) : base(id) {
        ForegroundColor = Color.FromArgb(255, 78, 201, 176);
      }
      public Type() : this(Key) {

      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Module : Type {
        public new const string Key = "ZMBA.type-module";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Module() : base(Key) {

          ForegroundColor = Color.FromArgb(255, 138, 219, 201);
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal class Class : Type {
        public new const string Key = "ZMBA.type-class";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        private Class(string id) : base(id) {
          ForegroundColor = Color.FromArgb(255, 125, 157, 189);
        }
        public Class() : this(Key) {

        }


        //[Export(typeof(EditorFormatDefinition))]
        //[UserVisible(false)]
        //[Order(After = Priority.High)]
        //[ClassificationType(ClassificationTypeNames = Key)]
        //[Name(Key)]
        //internal sealed class Attribute : Class {
        //	public new const string Key = "ZMBA.type-class-attribute";

        //	[Export(typeof(ClassificationTypeDefinition))]
        //	[Name(Key)]
        //	public new static ClassificationTypeDefinition TypeDef;
        //	
        //	public Attribute() : base(Key) {
        //		
        //		IsItalic = true;
        //	}
        //}
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Structure : Type {
        public new const string Key = "ZMBA.type-structure";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Structure() : base(Key) {

          ForegroundColor = Color.FromArgb(255, 157, 208, 225);
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Interface : Type {
        public new const string Key = "ZMBA.type-interface";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Interface() : base(Key) {

          ForegroundColor = Color.FromArgb(255, 186, 245, 222);
        }
      }


      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Delegate : Type {
        public new const string Key = "ZMBA.type-delegate";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Delegate() : base(Key) {

          ForegroundColor = Color.FromRgb(173, 216, 230);
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Generic : Type {
        public new const string Key = "ZMBA.type-generic";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Generic() : base(Key) {

          ForegroundColor = Color.FromArgb(255, 0, 255, 255);
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Dynamic : Type {
        public new const string Key = "ZMBA.type-dynamic";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Dynamic() : base(Key) {

          ForegroundColor = Color.FromArgb(255, 180, 180, 180);
        }
      }

    }





    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class Enum : ZMBAFormatDefinition {
      public const string Key = "ZMBA.enum";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private Enum(string id) : base(id) {
        ForegroundColor = Color.FromRgb(222, 210, 227);
      }
      public Enum() : this(Key) {

      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Member : Enum {
        public new const string Key = "ZMBA.enum-member";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Member() : base(Key) {

          IsBold = true;
          ForegroundColor = Color.FromRgb(198, 176, 206);
        }
      }
    }





    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class String : ZMBAFormatDefinition {
      public const string Key = "ZMBA.string";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private String(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 164, 72);
      }
      public String() : this(Key) {

      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Token : String {
        public new const string Key = "ZMBA.string-token";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Token() : base(Key) {

          //ForegroundColor = Color.FromRgb(214,157,133);
          //ForegroundColor = Color.FromRgb(255, 143, 32);
          //ForegroundColor = Color.FromRgb(255, 148, 40);
          ForegroundColor = Color.FromRgb(255, 150, 45);
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class SingleQuotes : String {
        public new const string Key = "ZMBA.string-singlequotes";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public SingleQuotes() : base(Key) {

          ForegroundColor = Color.FromRgb(255, 198, 140);
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Interpolated : String {
        public new const string Key = "ZMBA.string-interpolated";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Interpolated() : base(Key) {

          ForegroundColor = Color.FromArgb(255, 255, 180, 120);
        }
      }

    }





    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class Variable : ZMBAFormatDefinition {
      public const string Key = "ZMBA.variable";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private Variable(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 255, 255);
      }
      public Variable() : this(Key) {

      }

      //TODO:  Requires more advanced analysis
      //[Export(typeof(EditorFormatDefinition))]
      //[UserVisible(false)]
      //[Order(After = Priority.High)]
      //[ClassificationType(ClassificationTypeNames = Key)]
      //[Name(Key)]
      //internal sealed class Mutated : Variable {
      //	public new const string Key = "ZMBA.variable-mutated";

      //	[Export(typeof(ClassificationTypeDefinition))]
      //	[Name(Key)]
      //	public new static ClassificationTypeDefinition TypeDef;
      //	

      //	public Mutated() : base(Key) {
      //		
      //		IsItalic = true;
      //		//ForegroundColor = Color.FromRgb(237, 210, 182);
      //		//ForegroundColor = Color.FromRgb(224, 255, 241);
      //	}
      //}

    }


    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class Param : ZMBAFormatDefinition {
      public const string Key = "ZMBA.parameter";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private Param(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 215, 215);
      }
      public Param() : this(Key) {

      }

      //TODO:  Requires more advanced analysis
      //[Export(typeof(EditorFormatDefinition))]
      //[UserVisible(false)]
      //[Order(After = Priority.High)]
      //[ClassificationType(ClassificationTypeNames = Key)]
      //[Name(Key)]
      //internal new sealed class ParamMutated : Param {
      //	public new const string Key = "ZMBA.parameter-mutated";

      //	[Export(typeof(ClassificationTypeDefinition))]
      //	[Name(Key)]
      //	public new static ClassificationTypeDefinition TypeDef;
      //	

      //	public Mutated() : base(Key) {
      //		
      //		IsItalic = true;
      //	}
      //}

    }





    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class Method : ZMBAFormatDefinition {
      public const string Key = "ZMBA.method";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private Method(string id) : base(id) {
        ForegroundColor = Color.FromRgb(236, 231, 196);
      }
      public Method() : this(Key) {

      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Constructor : Method {
        public new const string Key = "ZMBA.method-constructor";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Constructor() : base(Key) {

          IsBold = true;
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Static : Method {
        public new const string Key = "ZMBA.method-static";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Static() : base(Key) {

          IsBold = true;
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Virtual : Method {
        public new const string Key = "ZMBA.method-virtual";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Virtual() : base(Key) {

          IsItalic = true;
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Extension : Method {
        public new const string Key = "ZMBA.method-extension";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Extension() : base(Key) {

          IsItalic = true;
          ForegroundColor = Color.FromArgb(255, 148, 255, 255);
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class UserDefinedOperator : Method {
        public new const string Key = "ZMBA.method-userdefinedoperator";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public UserDefinedOperator() : base(Key) {

          ForegroundColor = Color.FromArgb(255, 0, 240, 240);
        }
      }

      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class InterfaceImplementation : Method {
        public new const string Key = "ZMBA.method-interfaceimplementation";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public InterfaceImplementation() : base(Key) {

          IsUnderlined = true;
        }
      }

    }





    [Export(typeof(EditorFormatDefinition))]
    [UserVisible(true)]
    [Order(After = Priority.High)]
    [ClassificationType(ClassificationTypeNames = Key)]
    [Name(Key)]
    internal class Identifier : ZMBAFormatDefinition {
      public const string Key = "ZMBA.identifier";

      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Key)]
      public static ClassificationTypeDefinition TypeDef;


      private Identifier(string id) : base(id) {
        ForegroundColor = Color.FromArgb(200, 240, 240, 240);
      }
      public Identifier() : this(Key) {

      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Namespace : Identifier {
        public new const string Key = "ZMBA.identifier-namespace";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Namespace() : base(Key) {

          ForegroundColor = Color.FromRgb(135, 150, 194);
        }
      }


      //TODO: Currently unreliable. 
      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(false)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Attribute : Identifier {
        public new const string Key = "ZMBA.identifier-attribute";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Attribute() : base(Key) {

          IsItalic = true;
          //ForegroundColor = Color.FromRgb(136, 202, 244);
        }
      }



      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Constant : Identifier {
        public new const string Key = "ZMBA.identifier-constant";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Constant() : base(Key) {

          IsBold = true;
          ForegroundColor = Color.FromRgb(198, 176, 206);
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Field : Identifier {
        public new const string Key = "ZMBA.identifier-field";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;


        public Field() : base(Key) {

          //IsItalic = true;
          //ForegroundColor = Color.FromRgb(205, 237, 254);
          //ForegroundColor = Color.FromRgb(238, 213, 187);
          ForegroundColor = Color.FromRgb(224, 243, 255);
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal class Property : Identifier {
        public new const string Key = "ZMBA.identifier-property";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        private Property(string id) : base(id) {
          ForegroundColor = Color.FromRgb(205, 237, 254);
        }
        public Property() : this(Key) {

        }

        [Export(typeof(EditorFormatDefinition))]
        [UserVisible(true)]
        [Order(After = Priority.High)]
        [ClassificationType(ClassificationTypeNames = Key)]
        [Name(Key)]
        internal sealed class InterfaceImplementation : Property {
          public new const string Key = "ZMBA.identifier-property-interfaceimplementation";

          [Export(typeof(ClassificationTypeDefinition))]
          [Name(Key)]
          public new static ClassificationTypeDefinition TypeDef;

          public InterfaceImplementation() : base(Key) {

            IsUnderlined = true;
          }
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Event : Identifier {
        public new const string Key = "ZMBA.identifier-event";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Event() : base(Key) {

          ForegroundColor = Color.FromRgb(250, 152, 225);
          IsBold = false;
        }
      }


      [Export(typeof(EditorFormatDefinition))]
      [UserVisible(true)]
      [Order(After = Priority.High)]
      [ClassificationType(ClassificationTypeNames = Key)]
      [Name(Key)]
      internal sealed class Label : Identifier {
        public new const string Key = "ZMBA.identifier-label";

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(Key)]
        public new static ClassificationTypeDefinition TypeDef;

        public Label() : base(Key) {

          IsBold = true;
        }
      }


    }

  }

}
#pragma warning restore CS0649 // Field is never assigned to, and will always have its default value null
