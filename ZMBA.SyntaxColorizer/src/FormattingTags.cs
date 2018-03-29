using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;


namespace ZMBA.SyntaxColorizer {

	internal class FormattingTags {
		internal ClassificationTag SyntaxKeyword{ get; }
		internal ClassificationTag SyntaxOperator{ get; }
		internal ClassificationTag SyntaxNumber{ get; }
		internal ClassificationTag SyntaxPunctuation{ get; }
		internal ClassificationTag SyntaxComment{ get; }

		internal ClassificationTag Preprocessor{ get; }
		internal ClassificationTag PreprocessorText{ get; }

		internal ClassificationTag Type{ get; }
		internal ClassificationTag TypeModule{ get; }
		internal ClassificationTag TypeClass{ get; }
		//internal ClassificationTag TypeClassAttribute { get; }
		internal ClassificationTag TypeStructure{ get; }
		internal ClassificationTag TypeInterface{ get; }
		internal ClassificationTag TypeDelegate{ get; }
		internal ClassificationTag TypeGeneric{ get; }
		internal ClassificationTag TypeDynamic { get; }

		internal ClassificationTag Enum{ get; }
		internal ClassificationTag EnumMember{ get; }

		internal ClassificationTag String{ get; }
		internal ClassificationTag StringToken { get; }
		internal ClassificationTag StringSingleQuote{ get; }
		internal ClassificationTag StringInterpolated{ get; }

		internal ClassificationTag Variable{ get; }
		//internal ClassificationTag VariableMutated { get; }
		internal ClassificationTag Param{ get; }
		//internal ClassificationTag ParamMutated{ get; }
		

		internal ClassificationTag Method{ get; }
		internal ClassificationTag MethodConstructor{ get; }
		internal ClassificationTag MethodStatic{ get; }
      internal ClassificationTag MethodVirtual { get; }
      internal ClassificationTag MethodExtension{ get; }
      internal ClassificationTag MethodUserDefinedOperator { get; }
      internal ClassificationTag MethodInterfaceImplementation { get; }

      internal ClassificationTag Identifier{ get; }
		internal ClassificationTag IdentifierNamespace{ get; }
		internal ClassificationTag IdentifierAttribute{ get; }
		internal ClassificationTag IdentifierConst{ get; }
		internal ClassificationTag IdentifierField{ get; }
		internal ClassificationTag IdentifierProperty{ get; }
      internal ClassificationTag IdentifierPropertyInterfaceImplementation { get; }
      internal ClassificationTag IdentifierEvent { get; }
		internal ClassificationTag IdentifierLabel { get; }

		internal FormattingTags(IClassificationTypeRegistryService registry) {
			this.SyntaxKeyword = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Syntax.KeyWord.Key));
			this.SyntaxOperator = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Syntax.Operator.Key));
			this.SyntaxNumber = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Syntax.Number.Key));
			this.SyntaxPunctuation = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Syntax.Punctuation.Key));
			this.SyntaxComment = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Syntax.Comment.Key));

			this.Preprocessor = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Preprocessor.Key));
			this.PreprocessorText = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Preprocessor.Text.Key));

			this.Type = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Key));
			this.TypeModule = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Module.Key));
			this.TypeClass = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Class.Key));
			//this.TypeClassAttribute = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Class.Attribute.Key));
			this.TypeStructure = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Structure.Key));
			this.TypeInterface = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Interface.Key));
			this.TypeDelegate = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Delegate.Key));
			this.TypeGeneric = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Generic.Key));
			this.TypeDynamic = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Type.Dynamic.Key));

			this.Enum = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Enum.Key));
			this.EnumMember = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Enum.Member.Key));

			this.String = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.String.Key));
			this.StringToken = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.String.Token.Key));
			this.StringSingleQuote = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.String.SingleQuotes.Key));
			this.StringInterpolated = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.String.Interpolated.Key));

			this.Variable = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Variable.Key));
			//this.VariableMutated = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Variable.Mutated.Key));
			this.Param = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Param.Key));
			//this.ParamMutated = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Param.Mutated.Key));

			this.Method = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Method.Key));
			this.MethodConstructor = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Method.Constructor.Key));
			this.MethodStatic = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Method.Static.Key));
         this.MethodVirtual = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Method.Virtual.Key));
         this.MethodExtension = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Method.Extension.Key));
         this.MethodUserDefinedOperator = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Method.UserDefinedOperator.Key));
         this.MethodInterfaceImplementation = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Method.InterfaceImplementation.Key));

         this.Identifier = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Key));
			this.IdentifierNamespace = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Namespace.Key));
			this.IdentifierAttribute = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Attribute.Key));
			this.IdentifierConst = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Constant.Key));
			this.IdentifierField = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Field.Key));
			this.IdentifierProperty = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Property.Key));
         this.IdentifierPropertyInterfaceImplementation = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Property.InterfaceImplementation.Key));
         this.IdentifierEvent = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Event.Key));
			this.IdentifierLabel = new ClassificationTag(registry.GetClassificationType(FormatDefinitions.Identifier.Label.Key));
		}
	}
}