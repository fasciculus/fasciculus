using Fasciculus.Support;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Fasciculus.CodeAnalysis.Compilers
{
    internal partial class CompilationUnitCompiler
    {
        //public override void DefaultVisit(SyntaxNode node)
        //{
        //    base.DefaultVisit(node);
        //}

        //public override void Visit(SyntaxNode? node)
        //{
        //    base.Visit(node);
        //}

        public override void VisitAllowsConstraintClause(AllowsConstraintClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitAllowsConstraintClause");
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitAnonymousMethodExpression");
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitAnonymousObjectCreationExpression");
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            throw Ex.InvalidOperation("VisitAnonymousObjectMemberDeclarator");
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            // Argument
            // : IdentifierName
            // | SimpleMemberAccessExpression
            // | NumericLiteralExpression
            // | NullLiteralExpression
            // | CollectionExpression
            // | InvocationExpression
            // | ObjectCreationExpression
            // plus maybe more

            throw Ex.InvalidOperation("VisitArgument");
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            // ArgumentList: Argument*

            throw Ex.InvalidOperation("VisitArgumentList");
        }

        public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitArrayCreationExpression");
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitAssignmentExpression");
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            // Attribute
            // : IdentifierName
            // | IdentifierName AttributeArgumentList
            // | QualifiedName AttributeArgumentList

            throw Ex.InvalidOperation("VisitAttribute");
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            throw Ex.InvalidOperation("VisitAttributeArgument");
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            throw Ex.InvalidOperation("VisitAttributeArgumentList");
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            throw Ex.InvalidOperation("VisitAttributeTargetSpecifier");
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitAwaitExpression");
        }

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            throw Ex.InvalidOperation("VisitBadDirectiveTrivia");
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitBaseExpression");
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitBinaryExpression");
        }

        public override void VisitBinaryPattern(BinaryPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitBinaryPattern");
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            throw Ex.InvalidOperation("VisitBracketedArgumentList");
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitBreakStatement");
        }

        public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
            throw Ex.InvalidOperation("VisitCasePatternSwitchLabel");
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            throw Ex.InvalidOperation("VisitCaseSwitchLabel");
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitCastExpression");
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitCatchClause");
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            throw Ex.InvalidOperation("VisitCatchDeclaration");
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitCatchFilterClause");
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitCheckedExpression");
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitCheckedStatement");
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            // ClassConstraint or StructConstraint
            // Leaf

            throw Ex.InvalidOperation("VisitClassOrStructConstraint");
        }

        public override void VisitCollectionExpression(CollectionExpressionSyntax node)
        {
            // CollectionExpression:

            throw Ex.InvalidOperation("VisitCollectionExpression");
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitConditionalAccessExpression");
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitConditionalExpression");
        }

        public override void VisitConstantPattern(ConstantPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitConstantPattern");
        }

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            throw Ex.InvalidOperation("VisitConstructorConstraint");
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitContinueStatement");
        }

        public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            throw Ex.InvalidOperation("VisitConversionOperatorMemberCref");
        }

        public override void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            throw Ex.InvalidOperation("VisitCrefBracketedParameterList");
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
            throw Ex.InvalidOperation("VisitCrefParameter");
        }

        public override void VisitCrefParameterList(CrefParameterListSyntax node)
        {
            throw Ex.InvalidOperation("VisitCrefParameterList");
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitDeclarationExpression");
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitDeclarationPattern");
        }

        public override void VisitDefaultConstraint(DefaultConstraintSyntax node)
        {
            throw Ex.InvalidOperation("VisitDefaultConstraint");
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitDefaultExpression");
        }

        public override void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            throw Ex.InvalidOperation("VisitDefaultSwitchLabel");
        }

        public override void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            throw Ex.InvalidOperation("VisitDefineDirectiveTrivia");
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            throw Ex.InvalidOperation("VisitDelegateDeclaration");
        }

        public override void VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
            throw Ex.InvalidOperation("VisitDiscardDesignation");
        }

        public override void VisitDiscardPattern(DiscardPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitDiscardPattern");
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitDoStatement");
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitElementAccessExpression");
        }

        public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitElementBindingExpression");
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitElseClause");
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            throw Ex.InvalidOperation("VisitEventDeclaration");
        }

        public override void VisitExpressionColon(ExpressionColonSyntax node)
        {
            throw Ex.InvalidOperation("VisitExpressionColon");
        }

        public override void VisitExpressionElement(ExpressionElementSyntax node)
        {
            throw Ex.InvalidOperation("VisitExpressionElement");
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitExpressionStatement");
        }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            throw Ex.InvalidOperation("VisitExternAliasDirective");
        }

        public override void VisitFieldExpression(FieldExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitFieldExpression");
        }

        public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            throw Ex.InvalidOperation("VisitFileScopedNamespaceDeclaration");
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitFinallyClause");
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitFixedStatement");
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitForEachStatement");
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitForEachVariableStatement");
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitForStatement");
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitFromClause");
        }

        public override void VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            throw Ex.InvalidOperation("VisitFunctionPointerCallingConvention");
        }

        public override void VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
        {
            throw Ex.InvalidOperation("VisitFunctionPointerParameter");
        }

        public override void VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            throw Ex.InvalidOperation("VisitFunctionPointerParameterList");
        }

        public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            throw Ex.InvalidOperation("VisitFunctionPointerType");
        }

        public override void VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            throw Ex.InvalidOperation("VisitFunctionPointerUnmanagedCallingConvention");
        }

        public override void VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            throw Ex.InvalidOperation("VisitFunctionPointerUnmanagedCallingConventionList");
        }

        public override void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitGlobalStatement");
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitGotoStatement");
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitGroupClause");
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitIfStatement");
        }

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitImplicitArrayCreationExpression");
        }

        public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            throw Ex.InvalidOperation("VisitImplicitElementAccess");
        }

        public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            // ImplicitObjectCreationExpression: ArgumentList

            throw Ex.InvalidOperation("VisitImplicitObjectCreationExpression");
        }

        public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitImplicitElementAccess");
        }

        public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            throw Ex.InvalidOperation("VisitIncompleteMember");
        }

        public override void VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            throw Ex.InvalidOperation("VisitIndexerMemberCref");
        }

        public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitInitializerExpression");
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitInterpolatedStringExpression");
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            throw Ex.InvalidOperation("VisitInterpolatedStringText");
        }

        public override void VisitInterpolation(InterpolationSyntax node)
        {
            throw Ex.InvalidOperation("VisitInterpolation");
        }

        public override void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitInterpolationAlignmentClause");
        }

        public override void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitInterpolationFormatClause");
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            // InvocationExpression: SimpleMemberAccessExpression ArgumentList

            throw Ex.InvalidOperation("VisitInvocationExpression");
        }

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitIsPatternExpression");
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitJoinClause");
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitJoinIntoClause");
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitLabeledStatement");
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitLetClause");
        }

        public override void VisitListPattern(ListPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitListPattern");
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            // covers ArgListExpression, NumericLiteralExpression, StringLiteralExpression, Utf8StringLiteralExpression
            //  CharacterLiteralExpression, TrueLiteralExpression, FalseLiteralExpression, NullLiteralExpression, DefaultLiteralExpression
            //
            // DefaultLiteralExpression:
            // FalseLiteralExpression:
            // NullLiteralExpression:

            throw Ex.InvalidOperation("VisitLiteralExpression");
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitLocalDeclarationStatement");
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitLocalFunctionStatement");
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitLockStatement");
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitMakeRefExpression");
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            // covers SimpleMemberAccessExpression and PointerMemberAccessExpression
            // SimpleMemberAccessExpression: (IdentifierName | GenericName) IdentifierName

            throw Ex.InvalidOperation("VisitMemberAccessExpression");
        }

        public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitMemberBindingExpression");
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
            throw Ex.InvalidOperation("VisitNameColon");
        }

        public override void VisitNameEquals(NameEqualsSyntax node)
        {
            throw Ex.InvalidOperation("VisitNameEquals");
        }

        public override void VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            throw Ex.InvalidOperation("VisitNameMemberCref");
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            // ObjectCreationExpression: GenericName ArgumentList

            throw Ex.InvalidOperation("VisitObjectCreationExpression");
        }

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            throw Ex.InvalidOperation("VisitOmittedTypeArgument");
        }

        public override void VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            throw Ex.InvalidOperation("VisitOperatorMemberCref");
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitOrderByClause");
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
            throw Ex.InvalidOperation("VisitOrdering");
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitParenthesizedExpression");
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitParenthesizedLambdaExpression");
        }

        public override void VisitParenthesizedPattern(ParenthesizedPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitParenthesizedPattern");
        }

        public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
            throw Ex.InvalidOperation("VisitParenthesizedVariableDesignation");
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            throw Ex.InvalidOperation("VisitPointerType");
        }

        public override void VisitPositionalPatternClause(PositionalPatternClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitPositionalPatternClause");
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitPostfixUnaryExpression");
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitPrefixUnaryExpression");
        }

        public override void VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
        {
            throw Ex.InvalidOperation("VisitPrimaryConstructorBaseType");
        }

        public override void VisitPropertyPatternClause(PropertyPatternClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitPropertyPatternClause");
        }

        public override void VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            throw Ex.InvalidOperation("VisitQualifiedCref");
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            throw Ex.InvalidOperation("VisitQueryBody");
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            throw Ex.InvalidOperation("VisitQueryContinuation");
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitQueryExpression");
        }

        public override void VisitRangeExpression(RangeExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitRangeExpression");
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            throw Ex.InvalidOperation("VisitRecordDeclaration");
        }

        public override void VisitRecursivePattern(RecursivePatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitRecursivePattern");
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitRefExpression");
        }

        public override void VisitRefStructConstraint(RefStructConstraintSyntax node)
        {
            throw Ex.InvalidOperation("VisitRefStructConstraint");
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
            throw Ex.InvalidOperation("VisitRefType");
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitRefTypeExpression");
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitRefValueExpression");
        }

        public override void VisitRelationalPattern(RelationalPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitRelationalPattern");
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitReturnStatement");
        }

        public override void VisitScopedType(ScopedTypeSyntax node)
        {
            throw Ex.InvalidOperation("VisitScopedType");
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitSelectClause");
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitSimpleLambdaExpression");
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            throw Ex.InvalidOperation("VisitSingleVariableDesignation");
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitSizeOfExpression");
        }

        public override void VisitSlicePattern(SlicePatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitSlicePattern");
        }

        public override void VisitSpreadElement(SpreadElementSyntax node)
        {
            throw Ex.InvalidOperation("VisitSpreadElement");
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitStackAllocArrayCreationExpression");
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            throw Ex.InvalidOperation("VisitStructDeclaration");
        }

        public override void VisitSubpattern(SubpatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitSubpattern");
        }

        public override void VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitSwitchExpression");
        }

        public override void VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            throw Ex.InvalidOperation("VisitSwitchExpressionArm");
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            throw Ex.InvalidOperation("VisitSwitchSection");
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitSwitchStatement");
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitThisExpression");
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitThrowExpression");
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitThrowStatement");
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitTryStatement");
        }

        public override void VisitTupleElement(TupleElementSyntax node)
        {
            throw Ex.InvalidOperation("VisitTupleElement");
        }

        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitTupleExpression");
        }

        public override void VisitTupleType(TupleTypeSyntax node)
        {
            throw Ex.InvalidOperation("VisitTupleType");
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
            throw Ex.InvalidOperation("VisitTypeCref");
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitTypeOfExpression");
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            throw Ex.InvalidOperation("VisitTypeParameter");
        }

        public override void VisitTypePattern(TypePatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitTypePattern");
        }

        public override void VisitUnaryPattern(UnaryPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitUnaryPattern");
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitUnsafeStatement");
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitUsingStatement");
        }

        public override void VisitVarPattern(VarPatternSyntax node)
        {
            throw Ex.InvalidOperation("VisitVarPattern");
        }

        public override void VisitWhenClause(WhenClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitWhenClause");
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            throw Ex.InvalidOperation("VisitWhereClause");
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitWhileStatement");
        }

        public override void VisitWithExpression(WithExpressionSyntax node)
        {
            throw Ex.InvalidOperation("VisitWithExpression");
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            throw Ex.InvalidOperation("VisitYieldStatement");
        }
    }
}