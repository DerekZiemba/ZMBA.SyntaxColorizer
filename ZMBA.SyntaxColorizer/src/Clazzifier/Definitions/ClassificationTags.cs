using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;


namespace ZMBA.SyntaxColorizer.Clazzifier.Definitions {
  using CFD = ZMBA.SyntaxColorizer.Clazzifier.Definitions;

  internal class ClassificationTags {
    internal static ClassificationTags Instance { get; private set; }

    internal static void Initialize(IClassificationTypeRegistryService registry) {
      Instance = new ClassificationTags(registry);
    }

    internal ClassificationTag Identifier                   => this._Identifier;
    internal ClassificationTag IdentifierNamespace          => this._IdentifierNamespace;
    internal ClassificationTag IdentifierAttribute          => this._IdentifierAttribute;
    internal ClassificationTag IdentifierEvent              => this._IdentifierEvent;
    internal ClassificationTag IdentifierLabel              => this._IdentifierLabel;
    internal ClassificationTag Member                       => this._Member;
    internal ClassificationTag Constant                     => this._MemberConstant;
    internal ClassificationTag Field                        => this._MemberField;
    internal ClassificationTag Property                     => this._MemberProperty;
    internal ClassificationTag MemberInterfaceImpl          => this._MemberInterfaceImpl;
    internal ClassificationTag Method                       => this._Method;
    internal ClassificationTag MethodCtor                   => this._MethodCtor;
    internal ClassificationTag MethodStatic                 => this._MethodStatic;
    internal ClassificationTag MethodStaticExtension        => this._MethodStaticExtension;
    internal ClassificationTag MethodVirtual                => this._MethodVirtual;
    internal ClassificationTag Preprocessor                 => this._Preprocessor;
    internal ClassificationTag PreprocessorKeyword          => this._PreprocessorKeyword;
    internal ClassificationTag PreprocessorText             => this._PreprocessorText;
    internal ClassificationTag String                       => this._String;
    internal ClassificationTag StringSingleQuote            => this._StringSinglequote;
    internal ClassificationTag StringVerbatim               => this._StringVerbatim;
    internal ClassificationTag StringInterpolated           => this._StringInterpolated;
    internal ClassificationTag StringToken                  => this._StringToken;
    internal ClassificationTag Syntax                       => this._Syntax;
    internal ClassificationTag SyntaxNumber                 => this._SyntaxNumber;
    internal ClassificationTag SyntaxComment                => this._SyntaxComment;
    internal ClassificationTag SyntaxKeyword                => this._SyntaxKeyword;
    internal ClassificationTag SyntaxKeywordControl         => this._SyntaxKeywordControl;
    internal ClassificationTag SyntaxKeywordControlBranch   => this._SyntaxKeywordControlBranch;
    internal ClassificationTag SyntaxOperator               => this._SyntaxOperator;
    internal ClassificationTag SyntaxOperatorOverloaded     => this._SyntaxOperatorOverloaded;
    internal ClassificationTag SyntaxOperatorUserDefined    => this._SyntaxOperatorUserDefined;
    internal ClassificationTag SyntaxPunctuation            => this._SyntaxPunctuation;
    internal ClassificationTag SyntaxPunctuationBraces      => this._SyntaxPunctuationBraces;
    internal ClassificationTag SyntaxPunctuationBrackets    => this._SyntaxPunctuationBrackets;
    internal ClassificationTag SyntaxPunctuationParenthesis => this._SyntaxPunctuationParenthesis;
    internal ClassificationTag Type                         => this._Type;
    internal ClassificationTag TypeClass                    => this._TypeClass;
    internal ClassificationTag TypeDelegate                 => this._TypeDelegate;
    internal ClassificationTag TypeDynamic                  => this._TypeDynamic;
    internal ClassificationTag TypeEnum                     => this._TypeEnum;
    internal ClassificationTag TypeEnumMember               => this._TypeEnumMember;
    internal ClassificationTag TypeGeneric                  => this._TypeGeneric;
    internal ClassificationTag TypeInterface                => this._TypeInterface;
    internal ClassificationTag TypeModule                   => this._TypeModule;
    internal ClassificationTag TypeStructure                => this._TypeStructure;
    internal ClassificationTag Variable                     => this._Variable;
    internal ClassificationTag Parameter                    => this._Parameter;
    internal ClassificationTag VariableMutated              => this._VariableMutated;

    private ClassificationTags(IClassificationTypeRegistryService registry) {
      this._Identifier                   = registry.LoadTag(CFD.Identifier.Key);
      this._IdentifierNamespace          = registry.LoadTag(CFD.Identifier.Namespace.Key);
      this._IdentifierAttribute          = registry.LoadTag(CFD.Identifier.Attribute.Key);
      this._IdentifierEvent              = registry.LoadTag(CFD.Identifier.Event.Key);
      this._IdentifierLabel              = registry.LoadTag(CFD.Identifier.Label.Key);

      this._Member                       = registry.LoadTag(CFD.Member.Key);
      this._MemberConstant               = registry.LoadTag(CFD.Member.Constant.Key);
      this._MemberField                  = registry.LoadTag(CFD.Member.Field.Key);
      this._MemberProperty               = registry.LoadTag(CFD.Member.Property.Key);
      this._MemberInterfaceImpl          = registry.LoadTag(CFD.Member.InterfaceImplementation.Key);

      this._Method                       = registry.LoadTag(CFD.Method.Key);
      this._MethodCtor                   = registry.LoadTag(CFD.Method.Constructor.Key);
      this._MethodStatic                 = registry.LoadTag(CFD.Method.Static.Key);
      this._MethodStaticExtension        = registry.LoadTag(CFD.Method.Static.Extension.Key);
      this._MethodVirtual                = registry.LoadTag(CFD.Method.Virtual.Key);

      this._Preprocessor                 = registry.LoadTag(CFD.Preprocessor.Key);
      this._PreprocessorKeyword          = registry.LoadTag(CFD.Preprocessor.Keyword.Key);
      this._PreprocessorText             = registry.LoadTag(CFD.Preprocessor.Text.Key);

      this._String                       = registry.LoadTag(CFD.String.Key);
      this._StringSinglequote            = registry.LoadTag(CFD.String.Singlequote.Key);
      this._StringVerbatim               = registry.LoadTag(CFD.String.Verbatim.Key);
      this._StringInterpolated           = registry.LoadTag(CFD.String.Interpolated.Key);
      this._StringToken                  = registry.LoadTag(CFD.String.Token.Key);

      this._Syntax                       = registry.LoadTag(CFD.Syntax.Key);
      this._SyntaxComment                = registry.LoadTag(CFD.Syntax.Comment.Key);
      this._SyntaxNumber                 = registry.LoadTag(CFD.Syntax.Number.Key);
      this._SyntaxKeyword                = registry.LoadTag(CFD.Syntax.Keyword.Key);
      this._SyntaxKeywordControl         = registry.LoadTag(CFD.Syntax.Keyword.Control.Key);
      this._SyntaxKeywordControlBranch   = registry.LoadTag(CFD.Syntax.Keyword.Control.Branch.Key);

      this._SyntaxOperator               = registry.LoadTag(CFD.Syntax.Operator.Key);
      this._SyntaxOperatorOverloaded     = registry.LoadTag(CFD.Syntax.Operator.Overloaded.Key);
      this._SyntaxOperatorUserDefined    = registry.LoadTag(CFD.Syntax.Operator.UserDefined.Key);

      this._SyntaxPunctuation            = registry.LoadTag(CFD.Syntax.Punctuation.Key);
      this._SyntaxPunctuationBraces      = registry.LoadTag(CFD.Syntax.Punctuation.Braces.Key);
      this._SyntaxPunctuationBrackets    = registry.LoadTag(CFD.Syntax.Punctuation.Brackets.Key);
      this._SyntaxPunctuationParenthesis = registry.LoadTag(CFD.Syntax.Punctuation.Parenthesis.Key);

      this._Type                         = registry.LoadTag(CFD.Type.Key);
      this._TypeClass                    = registry.LoadTag(CFD.Type.Class.Key);
      this._TypeDelegate                 = registry.LoadTag(CFD.Type.Delegate.Key);
      this._TypeDynamic                  = registry.LoadTag(CFD.Type.Dynamic.Key);
      this._TypeEnum                     = registry.LoadTag(CFD.Type.Enum.Key);
      this._TypeEnumMember               = registry.LoadTag(CFD.Type.Enum.Member.Key);
      this._TypeGeneric                  = registry.LoadTag(CFD.Type.Generic.Key);
      this._TypeInterface                = registry.LoadTag(CFD.Type.Interface.Key);
      this._TypeModule                   = registry.LoadTag(CFD.Type.Module.Key);
      this._TypeStructure                = registry.LoadTag(CFD.Type.Structure.Key);

      this._Variable                     = registry.LoadTag(CFD.Variable.Key);
      this._Parameter                    = registry.LoadTag(CFD.Variable.Parameter.Key);
      this._VariableMutated              = registry.LoadTag(CFD.Variable.Mutated.Key);
    }

    private readonly ClassificationTag _Identifier;
    private readonly ClassificationTag _IdentifierNamespace;
    private readonly ClassificationTag _IdentifierAttribute;
    private readonly ClassificationTag _IdentifierEvent;
    private readonly ClassificationTag _IdentifierLabel;

    private readonly ClassificationTag _Member;
    private readonly ClassificationTag _MemberConstant;
    private readonly ClassificationTag _MemberField;
    private readonly ClassificationTag _MemberProperty;
    private readonly ClassificationTag _MemberInterfaceImpl;

    private readonly ClassificationTag _Method;
    private readonly ClassificationTag _MethodCtor;
    private readonly ClassificationTag _MethodStatic;
    private readonly ClassificationTag _MethodStaticExtension;
    private readonly ClassificationTag _MethodVirtual;

    private readonly ClassificationTag _Preprocessor;
    private readonly ClassificationTag _PreprocessorKeyword;
    private readonly ClassificationTag _PreprocessorText;

    private readonly ClassificationTag _String;
    private readonly ClassificationTag _StringSinglequote;
    private readonly ClassificationTag _StringVerbatim;
    private readonly ClassificationTag _StringInterpolated;
    private readonly ClassificationTag _StringToken;

    private readonly ClassificationTag _Syntax;
    private readonly ClassificationTag _SyntaxNumber;
    private readonly ClassificationTag _SyntaxComment;
    private readonly ClassificationTag _SyntaxKeyword;
    private readonly ClassificationTag _SyntaxKeywordControl;
    private readonly ClassificationTag _SyntaxKeywordControlBranch;

    private readonly ClassificationTag _SyntaxOperator;
    private readonly ClassificationTag _SyntaxOperatorOverloaded;
    private readonly ClassificationTag _SyntaxOperatorUserDefined;

    private readonly ClassificationTag _SyntaxPunctuation;
    private readonly ClassificationTag _SyntaxPunctuationBraces;
    private readonly ClassificationTag _SyntaxPunctuationBrackets;
    private readonly ClassificationTag _SyntaxPunctuationParenthesis;

    private readonly ClassificationTag _Type;
    private readonly ClassificationTag _TypeClass;
    private readonly ClassificationTag _TypeDelegate;
    private readonly ClassificationTag _TypeDynamic;
    private readonly ClassificationTag _TypeEnum;
    private readonly ClassificationTag _TypeEnumMember;
    private readonly ClassificationTag _TypeGeneric;
    private readonly ClassificationTag _TypeInterface;
    private readonly ClassificationTag _TypeModule;
    private readonly ClassificationTag _TypeStructure;

    private readonly ClassificationTag _Variable;
    private readonly ClassificationTag _Parameter;
    private readonly ClassificationTag _VariableMutated;
  }
}