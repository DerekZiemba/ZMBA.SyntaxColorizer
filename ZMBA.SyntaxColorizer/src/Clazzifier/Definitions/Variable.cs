using System;
using System.ComponentModel.Composition;
using System.Windows.Media;

using Microsoft.CodeAnalysis.Classification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {


  [Order(After = ClassificationTypeNames.LocalName)]
  [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
  internal class Variable : ZMBAFormatDefinition {
    public new const string Key = ZMBAFormatDefinition.Key + ".variable";

    [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
    [BaseDefinition(ClassificationTypeNames.LocalName)]
    public new static ClassificationTypeDefinition TypeDef;

    public Variable() : this(Key) { }

    public Variable(string id) : base(id) {
      ForegroundColor = Color.FromRgb(255, 255, 255);
    }

    [Order(After = Variable.Key)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Mutated : Variable {
      public new const string Key = Variable.Key + "-mutated";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Variable.Key)]
      public new static ClassificationTypeDefinition TypeDef;

      public Mutated() : this(Key) { }

      public Mutated(string id) : base(id) {
        IsItalic = true;
      }
    } // End Mutated


    [Order(After = Variable.Key)][Order(After = ClassificationTypeNames.ParameterName)]
    [Export(typeof(EditorFormatDefinition))][ClassificationType(ClassificationTypeNames = Key)][Name(Key)][UserVisible(true)]
    internal class Parameter : Variable {
      public new const string Key = Variable.Key + ".parameter";

      [Export(typeof(ClassificationTypeDefinition))][Name(Key)]
      [BaseDefinition(Variable.Key)][BaseDefinition(ClassificationTypeNames.ParameterName)]
      public new static ClassificationTypeDefinition TypeDef;

      public Parameter() : this(Key) { }

      public Parameter(string id) : base(id) {
        ForegroundColor = Color.FromRgb(255, 225, 225);
      }
    } // End Parameter

  } // End Variable
} // End Namespace
