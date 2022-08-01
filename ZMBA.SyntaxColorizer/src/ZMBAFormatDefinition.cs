using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

using static ZMBA.SyntaxColorizer.Extensions;

namespace ZMBA.SyntaxColorizer {

  //TODO: Pull format settings from a config file eventually. 
  internal abstract class ZMBAFormatDefinition : ClassificationFormatDefinition {
    private static readonly TextDecoration _tdUnderline = System.Windows.TextDecorations.Underline[0];
    private static readonly TextDecoration _tdOverLine = System.Windows.TextDecorations.OverLine[0];
    private static readonly TextDecoration _tdBaseline = System.Windows.TextDecorations.Baseline[0];
    private static readonly TextDecoration _tdStrikeThrough = System.Windows.TextDecorations.Strikethrough[0];

    public bool IsUnderlined {
      get {
        return TextDecorations == null ? false : TextDecorations.Contains(_tdUnderline);
      }
      set {
        if (value) {
          (TextDecorations ??= new TextDecorationCollection()).AddUnique(_tdUnderline);
        } else if (IsUnderlined) {
          TextDecorations.Remove(_tdUnderline);
        }
      }
    }

    public bool IsOverlined {
      get {
        return TextDecorations == null ? false : TextDecorations.Contains(_tdOverLine);
      }
      set {
        if (value) {
          (TextDecorations ??= new TextDecorationCollection()).AddUnique(_tdOverLine);
        } else if (IsOverlined) {
          TextDecorations.Remove(_tdOverLine);
        }
      }
    }

    public bool IsBaselined {
      get {
        return TextDecorations == null ? false : TextDecorations.Contains(_tdBaseline);
      }
      set {
        if (value) {
          (TextDecorations ??= new TextDecorationCollection()).AddUnique(_tdBaseline);
        } else if (IsBaselined) {
          TextDecorations.Remove(_tdBaseline);
        }
      }
    }

    public bool IsStrikeThrough {
      get {
        return TextDecorations == null ? false : TextDecorations.Contains(_tdStrikeThrough);
      }
      set {
        if (value) {
          (TextDecorations ??= new TextDecorationCollection()).AddUnique(_tdStrikeThrough);
        } else if (IsStrikeThrough) {
          TextDecorations.Remove(_tdStrikeThrough);
        }
      }
    }

    public ZMBAFormatDefinition(string identifier) {
      this.DisplayName = identifier;
    }
  }
}
