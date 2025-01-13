using Fasciculus.CodeAnalysis.Support;
using Fasciculus.Support;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace Fasciculus.CodeAnalysis.Compilers
{
    public partial class Compiler : CSharpSyntaxWalker
    {
        private readonly Stack<SyntaxNode> ancestors = [];

        private List<string> identifierNames = [];
        private string qualifiedName = string.Empty;

        public Compiler()
            : base(SyntaxWalkerDepth.StructuredTrivia)
        {
        }

        public void Compile(CompilationUnitSyntax compilationUnit)
        {
            // compilation_unit
            //   : extern_alias_directive* using_directive* global_attributes? namespace_member_declaration*
            //
            // CompilationUnit: UsingDirective* AttributeList* NamespaceDeclaration*

            Productions.Instance.Add(compilationUnit);

            DefaultVisit(compilationUnit);
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            base.DefaultVisit(node);
        }

        public override void Visit(SyntaxNode? node)
        {
            if (node is not null)
            {
                ancestors.Push(node);
            }

            base.Visit(node);

            if (node is not null)
            {
                ancestors.Pop();
            }
        }

        public T? FindAncestor<T>()
            where T : SyntaxNode
        {
            if (ancestors.Count == 0)
            {
                return null;
            }

            return (T?)ancestors.Skip(1).FirstOrDefault(t => t is T);
        }

        public override void VisitAccessorDeclaration(AccessorDeclarationSyntax node)
        {
            base.VisitAccessorDeclaration(node);
        }

        public override void VisitAccessorList(AccessorListSyntax node)
        {
            base.VisitAccessorList(node);
        }

        public override void VisitAliasQualifiedName(AliasQualifiedNameSyntax node)
        {
            // AliasQualifiedName
            // : IdentifierName IdentifierName

            Productions.Instance.Add(node);

            base.VisitAliasQualifiedName(node);

            if (identifierNames.Count == 0 || identifierNames[0] != "global")
            {
                throw Ex.InvalidOperation();
            }

            identifierNames.RemoveAt(0);
        }

        public override void VisitAllowsConstraintClause(AllowsConstraintClauseSyntax node)
        {
            base.VisitAllowsConstraintClause(node);
        }

        public override void VisitAnonymousMethodExpression(AnonymousMethodExpressionSyntax node)
        {
            base.VisitAnonymousMethodExpression(node);
        }

        public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
        {
            base.VisitAnonymousObjectCreationExpression(node);
        }

        public override void VisitAnonymousObjectMemberDeclarator(AnonymousObjectMemberDeclaratorSyntax node)
        {
            base.VisitAnonymousObjectMemberDeclarator(node);
        }

        public override void VisitArgument(ArgumentSyntax node)
        {
            base.VisitArgument(node);
        }

        public override void VisitArgumentList(ArgumentListSyntax node)
        {
            base.VisitArgumentList(node);
        }

        public override void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
        {
            base.VisitArrayCreationExpression(node);
        }

        public override void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
        {
            // ArrayRankSpecifier
            // : IdentifierName
            // | InvocationExpression
            // | OmittedArraySizeExpression
            // | AddExpression
            // | NumericLiteralExpression
            //
            // only OmittedArraySizeExpression occurs on fields.

            Productions.Instance.Add(node);

            base.VisitArrayRankSpecifier(node);
        }

        public override void VisitArrayType(ArrayTypeSyntax node)
        {
            // ArrayType
            // : (IdentifierName | GenericName | PredefinedType) ArrayRankSpecifier

            Productions.Instance.Add(node);

            base.VisitArrayType(node);
        }

        public override void VisitArrowExpressionClause(ArrowExpressionClauseSyntax node)
        {
            //base.VisitArrowExpressionClause(node);
        }

        public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
        {
            base.VisitAssignmentExpression(node);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            // Attribute
            // : IdentifierName
            // | IdentifierName AttributeArgumentList
            // | QualifiedName AttributeArgumentList

            //Productions.Instance.Add(node);

            //base.VisitAttribute(node);
        }

        public override void VisitAttributeArgument(AttributeArgumentSyntax node)
        {
            base.VisitAttributeArgument(node);
        }

        public override void VisitAttributeArgumentList(AttributeArgumentListSyntax node)
        {
            base.VisitAttributeArgumentList(node);
        }

        public override void VisitAttributeList(AttributeListSyntax node)
        {
            // HasTrivia
            // AttributeList
            // : AttributeTargetSpecifier Attribute
            // | Attribute

            Productions.Instance.Add(node);

            base.VisitAttributeList(node);
        }

        public override void VisitAttributeTargetSpecifier(AttributeTargetSpecifierSyntax node)
        {
            // Leaf

            //attributeTargetSpecifier = node.Identifier.ValueText;
        }

        public override void VisitAwaitExpression(AwaitExpressionSyntax node)
        {
            base.VisitAwaitExpression(node);
        }

        public override void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
        {
            base.VisitBadDirectiveTrivia(node);
        }

        public override void VisitBaseExpression(BaseExpressionSyntax node)
        {
            base.VisitBaseExpression(node);
        }

        public override void VisitBaseList(BaseListSyntax node)
        {
            // BaseList
            // : SimpleBaseType+

            Productions.Instance.Add(node);

            base.VisitBaseList(node);
        }

        public override void VisitBinaryExpression(BinaryExpressionSyntax node)
        {
            base.VisitBinaryExpression(node);
        }

        public override void VisitBinaryPattern(BinaryPatternSyntax node)
        {
            base.VisitBinaryPattern(node);
        }

        public override void VisitBlock(BlockSyntax node)
        {
            // base.VisitBlock(node);
        }

        public override void VisitBracketedArgumentList(BracketedArgumentListSyntax node)
        {
            base.VisitBracketedArgumentList(node);
        }

        public override void VisitBracketedParameterList(BracketedParameterListSyntax node)
        {
            base.VisitBracketedParameterList(node);
        }

        public override void VisitBreakStatement(BreakStatementSyntax node)
        {
            base.VisitBreakStatement(node);
        }

        public override void VisitCasePatternSwitchLabel(CasePatternSwitchLabelSyntax node)
        {
            base.VisitCasePatternSwitchLabel(node);
        }

        public override void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node)
        {
            base.VisitCaseSwitchLabel(node);
        }

        public override void VisitCastExpression(CastExpressionSyntax node)
        {
            base.VisitCastExpression(node);
        }

        public override void VisitCatchClause(CatchClauseSyntax node)
        {
            base.VisitCatchClause(node);
        }

        public override void VisitCatchDeclaration(CatchDeclarationSyntax node)
        {
            base.VisitCatchDeclaration(node);
        }

        public override void VisitCatchFilterClause(CatchFilterClauseSyntax node)
        {
            base.VisitCatchFilterClause(node);
        }

        public override void VisitCheckedExpression(CheckedExpressionSyntax node)
        {
            base.VisitCheckedExpression(node);
        }

        public override void VisitCheckedStatement(CheckedStatementSyntax node)
        {
            base.VisitCheckedStatement(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            // HasTrivia: True
            // ClassDeclaration
            // : AttributeList? TypeParameterList? BaseList? TypeParameterConstraintClause?
            //   (
            //       FieldDeclaration
            //     | PropertyDeclaration
            //     | IndexerDeclaration
            //     | EventFieldDeclaration
            //     | ConstructorDeclaration
            //     | DestructorDeclaration
            //     | MethodDeclaration
            //     | OperatorDeclaration
            //     | ConversionOperatorDeclaration
            //     | ClassDeclaration
            //   )*
            //
            // TypeParameterConstraintClause only when TypeParameterList 

            Productions.Instance.Add(node);

            base.VisitClassDeclaration(node);
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            base.VisitClassOrStructConstraint(node);
        }

        public override void VisitCollectionExpression(CollectionExpressionSyntax node)
        {
            base.VisitCollectionExpression(node);
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            base.VisitCompilationUnit(node);
        }

        public override void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
        {
            base.VisitConditionalAccessExpression(node);
        }

        public override void VisitConditionalExpression(ConditionalExpressionSyntax node)
        {
            base.VisitConditionalExpression(node);
        }

        public override void VisitConstantPattern(ConstantPatternSyntax node)
        {
            base.VisitConstantPattern(node);
        }

        public override void VisitConstructorConstraint(ConstructorConstraintSyntax node)
        {
            base.VisitConstructorConstraint(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            base.VisitConstructorDeclaration(node);
        }

        public override void VisitConstructorInitializer(ConstructorInitializerSyntax node)
        {
            base.VisitConstructorInitializer(node);
        }

        public override void VisitContinueStatement(ContinueStatementSyntax node)
        {
            base.VisitContinueStatement(node);
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            base.VisitConversionOperatorDeclaration(node);
        }

        public override void VisitConversionOperatorMemberCref(ConversionOperatorMemberCrefSyntax node)
        {
            base.VisitConversionOperatorMemberCref(node);
        }

        public override void VisitCrefBracketedParameterList(CrefBracketedParameterListSyntax node)
        {
            base.VisitCrefBracketedParameterList(node);
        }

        public override void VisitCrefParameter(CrefParameterSyntax node)
        {
            base.VisitCrefParameter(node);
        }

        public override void VisitCrefParameterList(CrefParameterListSyntax node)
        {
            base.VisitCrefParameterList(node);
        }

        public override void VisitDeclarationExpression(DeclarationExpressionSyntax node)
        {
            base.VisitDeclarationExpression(node);
        }

        public override void VisitDeclarationPattern(DeclarationPatternSyntax node)
        {
            base.VisitDeclarationPattern(node);
        }

        public override void VisitDefaultConstraint(DefaultConstraintSyntax node)
        {
            base.VisitDefaultConstraint(node);
        }

        public override void VisitDefaultExpression(DefaultExpressionSyntax node)
        {
            base.VisitDefaultExpression(node);
        }

        public override void VisitDefaultSwitchLabel(DefaultSwitchLabelSyntax node)
        {
            base.VisitDefaultSwitchLabel(node);
        }

        public override void VisitDefineDirectiveTrivia(DefineDirectiveTriviaSyntax node)
        {
            base.VisitDefineDirectiveTrivia(node);
        }

        public override void VisitDelegateDeclaration(DelegateDeclarationSyntax node)
        {
            base.VisitDelegateDeclaration(node);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            base.VisitDestructorDeclaration(node);
        }

        public override void VisitDiscardDesignation(DiscardDesignationSyntax node)
        {
            base.VisitDiscardDesignation(node);
        }

        public override void VisitDiscardPattern(DiscardPatternSyntax node)
        {
            base.VisitDiscardPattern(node);
        }

        public override void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
        {
            base.VisitDocumentationCommentTrivia(node);
        }

        public override void VisitDoStatement(DoStatementSyntax node)
        {
            base.VisitDoStatement(node);
        }

        public override void VisitElementAccessExpression(ElementAccessExpressionSyntax node)
        {
            base.VisitElementAccessExpression(node);
        }

        public override void VisitElementBindingExpression(ElementBindingExpressionSyntax node)
        {
            base.VisitElementBindingExpression(node);
        }

        public override void VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node)
        {
            base.VisitElifDirectiveTrivia(node);
        }

        public override void VisitElseClause(ElseClauseSyntax node)
        {
            base.VisitElseClause(node);
        }

        public override void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
        {
            base.VisitElseDirectiveTrivia(node);
        }

        public override void VisitEmptyStatement(EmptyStatementSyntax node)
        {
            base.VisitEmptyStatement(node);
        }

        public override void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
        {
            base.VisitEndIfDirectiveTrivia(node);
        }

        public override void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
        {
            base.VisitEndRegionDirectiveTrivia(node);
        }

        public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
        {
            // HasTrivia: True
            // EnumDeclaration
            // : EnumMemberDeclaration*

            Productions.Instance.Add(node);

            base.VisitEnumDeclaration(node);
        }

        public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
        {
            // HasTrivia: True
            // Leaf

            Productions.Instance.Add(node);

            base.VisitEnumMemberDeclaration(node);
        }

        public override void VisitEqualsValueClause(EqualsValueClauseSyntax node)
        {
            base.VisitEqualsValueClause(node);
        }

        public override void VisitErrorDirectiveTrivia(ErrorDirectiveTriviaSyntax node)
        {
            base.VisitErrorDirectiveTrivia(node);
        }

        public override void VisitEventDeclaration(EventDeclarationSyntax node)
        {
            base.VisitEventDeclaration(node);
        }

        public override void VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            base.VisitEventFieldDeclaration(node);
        }

        public override void VisitExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax node)
        {
            // ExplicitInterfaceSpecifier
            // : IdentifierName
            // | GenericName

            Productions.Instance.Add(node);

            base.VisitExplicitInterfaceSpecifier(node);
        }

        public override void VisitExpressionColon(ExpressionColonSyntax node)
        {
            base.VisitExpressionColon(node);
        }

        public override void VisitExpressionElement(ExpressionElementSyntax node)
        {
            base.VisitExpressionElement(node);
        }

        public override void VisitExpressionStatement(ExpressionStatementSyntax node)
        {
            base.VisitExpressionStatement(node);
        }

        public override void VisitExternAliasDirective(ExternAliasDirectiveSyntax node)
        {
            base.VisitExternAliasDirective(node);
        }

        public override void VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            // HasTrivia: True
            // FieldDeclaration: VariableDeclaration

            Productions.Instance.Add(node);

            base.VisitFieldDeclaration(node);
        }

        public override void VisitFieldExpression(FieldExpressionSyntax node)
        {
            base.VisitFieldExpression(node);
        }

        public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            base.VisitFileScopedNamespaceDeclaration(node);
        }

        public override void VisitFinallyClause(FinallyClauseSyntax node)
        {
            base.VisitFinallyClause(node);
        }

        public override void VisitFixedStatement(FixedStatementSyntax node)
        {
            base.VisitFixedStatement(node);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            base.VisitForEachStatement(node);
        }

        public override void VisitForEachVariableStatement(ForEachVariableStatementSyntax node)
        {
            base.VisitForEachVariableStatement(node);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            base.VisitForStatement(node);
        }

        public override void VisitFromClause(FromClauseSyntax node)
        {
            base.VisitFromClause(node);
        }

        public override void VisitFunctionPointerCallingConvention(FunctionPointerCallingConventionSyntax node)
        {
            base.VisitFunctionPointerCallingConvention(node);
        }

        public override void VisitFunctionPointerParameter(FunctionPointerParameterSyntax node)
        {
            base.VisitFunctionPointerParameter(node);
        }

        public override void VisitFunctionPointerParameterList(FunctionPointerParameterListSyntax node)
        {
            base.VisitFunctionPointerParameterList(node);
        }

        public override void VisitFunctionPointerType(FunctionPointerTypeSyntax node)
        {
            base.VisitFunctionPointerType(node);
        }

        public override void VisitFunctionPointerUnmanagedCallingConvention(FunctionPointerUnmanagedCallingConventionSyntax node)
        {
            base.VisitFunctionPointerUnmanagedCallingConvention(node);
        }

        public override void VisitFunctionPointerUnmanagedCallingConventionList(FunctionPointerUnmanagedCallingConventionListSyntax node)
        {
            base.VisitFunctionPointerUnmanagedCallingConventionList(node);
        }

        public override void VisitGenericName(GenericNameSyntax node)
        {
            // GenericName
            // : TypeArgumentList

            Productions.Instance.Add(node);

            base.VisitGenericName(node);
        }

        public override void VisitGlobalStatement(GlobalStatementSyntax node)
        {
            base.VisitGlobalStatement(node);
        }

        public override void VisitGotoStatement(GotoStatementSyntax node)
        {
            base.VisitGotoStatement(node);
        }

        public override void VisitGroupClause(GroupClauseSyntax node)
        {
            base.VisitGroupClause(node);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            //Productions.Instance.Add(node);

            //base.VisitIdentifierName(node);

            identifierNames.Add(node.Identifier.ValueText);
        }

        public override void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
        {
            base.VisitIfDirectiveTrivia(node);
        }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            base.VisitIfStatement(node);
        }

        public override void VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            base.VisitImplicitArrayCreationExpression(node);
        }

        public override void VisitImplicitElementAccess(ImplicitElementAccessSyntax node)
        {
            base.VisitImplicitElementAccess(node);
        }

        public override void VisitImplicitObjectCreationExpression(ImplicitObjectCreationExpressionSyntax node)
        {
            base.VisitImplicitObjectCreationExpression(node);
        }

        public override void VisitImplicitStackAllocArrayCreationExpression(ImplicitStackAllocArrayCreationExpressionSyntax node)
        {
            base.VisitImplicitStackAllocArrayCreationExpression(node);
        }

        public override void VisitIncompleteMember(IncompleteMemberSyntax node)
        {
            base.VisitIncompleteMember(node);
        }

        public override void VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            base.VisitIndexerDeclaration(node);
        }

        public override void VisitIndexerMemberCref(IndexerMemberCrefSyntax node)
        {
            base.VisitIndexerMemberCref(node);
        }

        public override void VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            base.VisitInitializerExpression(node);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            // HasTrivia: True
            // InterfaceDeclaration
            // : TypeParameterList? BaseList? TypeParameterConstraintClause? MethodDeclaration*

            Productions.Instance.Add(node);

            base.VisitInterfaceDeclaration(node);
        }

        public override void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
        {
            base.VisitInterpolatedStringExpression(node);
        }

        public override void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
        {
            base.VisitInterpolatedStringText(node);
        }

        public override void VisitInterpolation(InterpolationSyntax node)
        {
            base.VisitInterpolation(node);
        }

        public override void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
        {
            base.VisitInterpolationAlignmentClause(node);
        }

        public override void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
        {
            base.VisitInterpolationFormatClause(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            base.VisitInvocationExpression(node);
        }

        public override void VisitIsPatternExpression(IsPatternExpressionSyntax node)
        {
            base.VisitIsPatternExpression(node);
        }

        public override void VisitJoinClause(JoinClauseSyntax node)
        {
            base.VisitJoinClause(node);
        }

        public override void VisitJoinIntoClause(JoinIntoClauseSyntax node)
        {
            base.VisitJoinIntoClause(node);
        }

        public override void VisitLabeledStatement(LabeledStatementSyntax node)
        {
            base.VisitLabeledStatement(node);
        }

        public override void VisitLeadingTrivia(SyntaxToken token)
        {
            base.VisitLeadingTrivia(token);
        }

        public override void VisitLetClause(LetClauseSyntax node)
        {
            base.VisitLetClause(node);
        }

        public override void VisitLineDirectivePosition(LineDirectivePositionSyntax node)
        {
            base.VisitLineDirectivePosition(node);
        }

        public override void VisitLineDirectiveTrivia(LineDirectiveTriviaSyntax node)
        {
            base.VisitLineDirectiveTrivia(node);
        }

        public override void VisitLineSpanDirectiveTrivia(LineSpanDirectiveTriviaSyntax node)
        {
            base.VisitLineSpanDirectiveTrivia(node);
        }

        public override void VisitListPattern(ListPatternSyntax node)
        {
            base.VisitListPattern(node);
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            base.VisitLiteralExpression(node);
        }

        public override void VisitLoadDirectiveTrivia(LoadDirectiveTriviaSyntax node)
        {
            base.VisitLoadDirectiveTrivia(node);
        }

        public override void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
        {
            base.VisitLocalDeclarationStatement(node);
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            base.VisitLocalFunctionStatement(node);
        }

        public override void VisitLockStatement(LockStatementSyntax node)
        {
            base.VisitLockStatement(node);
        }

        public override void VisitMakeRefExpression(MakeRefExpressionSyntax node)
        {
            base.VisitMakeRefExpression(node);
        }

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            base.VisitMemberAccessExpression(node);
        }

        public override void VisitMemberBindingExpression(MemberBindingExpressionSyntax node)
        {
            base.VisitMemberBindingExpression(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            // HasTrivia: True
            // MethodDeclaration
            // : AttributeList? <return-type> ExplicitInterfaceSpecifier? ParameterList TypeParameterConstraintClause? (ArrowExpressionClause | Block)
            //
            // return-type
            // : IdentifierName TypeParameterList?
            // | GenericName TypeParameterList?
            // | PredefinedType TypeParameterList?
            // | ArrayType
            // | NullableType

            Productions.Instance.Add(node);

            base.VisitMethodDeclaration(node);
        }

        public override void VisitNameColon(NameColonSyntax node)
        {
            base.VisitNameColon(node);
        }

        public override void VisitNameEquals(NameEqualsSyntax node)
        {
            base.VisitNameEquals(node);
        }

        public override void VisitNameMemberCref(NameMemberCrefSyntax node)
        {
            base.VisitNameMemberCref(node);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            // HasTrivia: True
            // NamespaceDeclaration
            // : QualifiedName ClassDeclaration* InterfaceDeclaration* EnumDeclaration*

            Productions.Instance.Add(node);

            base.VisitNamespaceDeclaration(node);

            string name = string.IsNullOrEmpty(qualifiedName) ? string.Join(".", identifierNames) : qualifiedName;
        }

        public override void VisitNullableDirectiveTrivia(NullableDirectiveTriviaSyntax node)
        {
            base.VisitNullableDirectiveTrivia(node);
        }

        public override void VisitNullableType(NullableTypeSyntax node)
        {
            // NullableType
            // : IdentifierName
            // | GenericName
            // | PredefinedType

            Productions.Instance.Add(node);

            base.VisitNullableType(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            base.VisitObjectCreationExpression(node);
        }

        public override void VisitOmittedArraySizeExpression(OmittedArraySizeExpressionSyntax node)
        {
            // Leaf

            //Productions.Instance.Add(node);

            //base.VisitOmittedArraySizeExpression(node);
        }

        public override void VisitOmittedTypeArgument(OmittedTypeArgumentSyntax node)
        {
            base.VisitOmittedTypeArgument(node);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            base.VisitOperatorDeclaration(node);
        }

        public override void VisitOperatorMemberCref(OperatorMemberCrefSyntax node)
        {
            base.VisitOperatorMemberCref(node);
        }

        public override void VisitOrderByClause(OrderByClauseSyntax node)
        {
            base.VisitOrderByClause(node);
        }

        public override void VisitOrdering(OrderingSyntax node)
        {
            base.VisitOrdering(node);
        }

        public override void VisitParameter(ParameterSyntax node)
        {
            // Parameter
            // : IdentifierName EqualsValueClause?
            // | GenericName
            // | PredefinedType EqualsValueClause?
            // | ArrayType
            // | PointerType
            // | NullableType EqualsValueClause?

            Productions.Instance.Add(node);

            base.VisitParameter(node);
        }

        public override void VisitParameterList(ParameterListSyntax node)
        {
            // ParameterList: Parameter*

            Productions.Instance.Add(node);

            base.VisitParameterList(node);
        }

        public override void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
        {
            base.VisitParenthesizedExpression(node);
        }

        public override void VisitParenthesizedLambdaExpression(ParenthesizedLambdaExpressionSyntax node)
        {
            base.VisitParenthesizedLambdaExpression(node);
        }

        public override void VisitParenthesizedPattern(ParenthesizedPatternSyntax node)
        {
            base.VisitParenthesizedPattern(node);
        }

        public override void VisitParenthesizedVariableDesignation(ParenthesizedVariableDesignationSyntax node)
        {
            base.VisitParenthesizedVariableDesignation(node);
        }

        public override void VisitPointerType(PointerTypeSyntax node)
        {
            base.VisitPointerType(node);
        }

        public override void VisitPositionalPatternClause(PositionalPatternClauseSyntax node)
        {
            base.VisitPositionalPatternClause(node);
        }

        public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
        {
            base.VisitPostfixUnaryExpression(node);
        }

        public override void VisitPragmaChecksumDirectiveTrivia(PragmaChecksumDirectiveTriviaSyntax node)
        {
            base.VisitPragmaChecksumDirectiveTrivia(node);
        }

        public override void VisitPragmaWarningDirectiveTrivia(PragmaWarningDirectiveTriviaSyntax node)
        {
            base.VisitPragmaWarningDirectiveTrivia(node);
        }

        public override void VisitPredefinedType(PredefinedTypeSyntax node)
        {
            // Leaf

            Productions.Instance.Add(node);

            base.VisitPredefinedType(node);
        }

        public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
        {
            base.VisitPrefixUnaryExpression(node);
        }

        public override void VisitPrimaryConstructorBaseType(PrimaryConstructorBaseTypeSyntax node)
        {
            base.VisitPrimaryConstructorBaseType(node);
        }

        public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            base.VisitPropertyDeclaration(node);
        }

        public override void VisitPropertyPatternClause(PropertyPatternClauseSyntax node)
        {
            base.VisitPropertyPatternClause(node);
        }

        public override void VisitQualifiedCref(QualifiedCrefSyntax node)
        {
            base.VisitQualifiedCref(node);
        }

        public override void VisitQualifiedName(QualifiedNameSyntax node)
        {
            // QualifiedName
            // : IdentifierName IdentifierName
            // | QualifiedName IdentifierName
            // | AliasQualifiedName IdentifierName

            Productions.Instance.Add(node);

            identifierNames.Clear();
            qualifiedName = string.Empty;

            base.VisitQualifiedName(node);

            qualifiedName = string.Join(".", identifierNames);
        }

        public override void VisitQueryBody(QueryBodySyntax node)
        {
            base.VisitQueryBody(node);
        }

        public override void VisitQueryContinuation(QueryContinuationSyntax node)
        {
            base.VisitQueryContinuation(node);
        }

        public override void VisitQueryExpression(QueryExpressionSyntax node)
        {
            base.VisitQueryExpression(node);
        }

        public override void VisitRangeExpression(RangeExpressionSyntax node)
        {
            base.VisitRangeExpression(node);
        }

        public override void VisitRecordDeclaration(RecordDeclarationSyntax node)
        {
            base.VisitRecordDeclaration(node);
        }

        public override void VisitRecursivePattern(RecursivePatternSyntax node)
        {
            base.VisitRecursivePattern(node);
        }

        public override void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
        {
            base.VisitReferenceDirectiveTrivia(node);
        }

        public override void VisitRefExpression(RefExpressionSyntax node)
        {
            base.VisitRefExpression(node);
        }

        public override void VisitRefStructConstraint(RefStructConstraintSyntax node)
        {
            base.VisitRefStructConstraint(node);
        }

        public override void VisitRefType(RefTypeSyntax node)
        {
            base.VisitRefType(node);
        }

        public override void VisitRefTypeExpression(RefTypeExpressionSyntax node)
        {
            base.VisitRefTypeExpression(node);
        }

        public override void VisitRefValueExpression(RefValueExpressionSyntax node)
        {
            base.VisitRefValueExpression(node);
        }

        public override void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
        {
            base.VisitRegionDirectiveTrivia(node);
        }

        public override void VisitRelationalPattern(RelationalPatternSyntax node)
        {
            base.VisitRelationalPattern(node);
        }

        public override void VisitReturnStatement(ReturnStatementSyntax node)
        {
            base.VisitReturnStatement(node);
        }

        public override void VisitScopedType(ScopedTypeSyntax node)
        {
            base.VisitScopedType(node);
        }

        public override void VisitSelectClause(SelectClauseSyntax node)
        {
            base.VisitSelectClause(node);
        }

        public override void VisitShebangDirectiveTrivia(ShebangDirectiveTriviaSyntax node)
        {
            base.VisitShebangDirectiveTrivia(node);
        }

        public override void VisitSimpleBaseType(SimpleBaseTypeSyntax node)
        {
            // SimpleBaseType
            // : IdentifierName
            // | GenericName

            Productions.Instance.Add(node);

            base.VisitSimpleBaseType(node);
        }

        public override void VisitSimpleLambdaExpression(SimpleLambdaExpressionSyntax node)
        {
            base.VisitSimpleLambdaExpression(node);
        }

        public override void VisitSingleVariableDesignation(SingleVariableDesignationSyntax node)
        {
            base.VisitSingleVariableDesignation(node);
        }

        public override void VisitSizeOfExpression(SizeOfExpressionSyntax node)
        {
            base.VisitSizeOfExpression(node);
        }

        public override void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
        {
            base.VisitSkippedTokensTrivia(node);
        }

        public override void VisitSlicePattern(SlicePatternSyntax node)
        {
            base.VisitSlicePattern(node);
        }

        public override void VisitSpreadElement(SpreadElementSyntax node)
        {
            base.VisitSpreadElement(node);
        }

        public override void VisitStackAllocArrayCreationExpression(StackAllocArrayCreationExpressionSyntax node)
        {
            base.VisitStackAllocArrayCreationExpression(node);
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            base.VisitStructDeclaration(node);
        }

        public override void VisitSubpattern(SubpatternSyntax node)
        {
            base.VisitSubpattern(node);
        }

        public override void VisitSwitchExpression(SwitchExpressionSyntax node)
        {
            base.VisitSwitchExpression(node);
        }

        public override void VisitSwitchExpressionArm(SwitchExpressionArmSyntax node)
        {
            base.VisitSwitchExpressionArm(node);
        }

        public override void VisitSwitchSection(SwitchSectionSyntax node)
        {
            base.VisitSwitchSection(node);
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            base.VisitSwitchStatement(node);
        }

        public override void VisitThisExpression(ThisExpressionSyntax node)
        {
            base.VisitThisExpression(node);
        }

        public override void VisitThrowExpression(ThrowExpressionSyntax node)
        {
            base.VisitThrowExpression(node);
        }

        public override void VisitThrowStatement(ThrowStatementSyntax node)
        {
            base.VisitThrowStatement(node);
        }

        public override void VisitToken(SyntaxToken token)
        {
            base.VisitToken(token);
        }

        public override void VisitTrailingTrivia(SyntaxToken token)
        {
            base.VisitTrailingTrivia(token);
        }

        public override void VisitTrivia(SyntaxTrivia trivia)
        {
            base.VisitTrivia(trivia);
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            base.VisitTryStatement(node);
        }

        public override void VisitTupleElement(TupleElementSyntax node)
        {
            base.VisitTupleElement(node);
        }

        public override void VisitTupleExpression(TupleExpressionSyntax node)
        {
            base.VisitTupleExpression(node);
        }

        public override void VisitTupleType(TupleTypeSyntax node)
        {
            base.VisitTupleType(node);
        }

        public override void VisitTypeArgumentList(TypeArgumentListSyntax node)
        {
            // TypeArgumentList
            // : (IdentifierName | PredefinedType)+
            // | GenericName
            // | NullableType

            Productions.Instance.Add(node);

            base.VisitTypeArgumentList(node);
        }

        public override void VisitTypeConstraint(TypeConstraintSyntax node)
        {
            base.VisitTypeConstraint(node);
        }

        public override void VisitTypeCref(TypeCrefSyntax node)
        {
            base.VisitTypeCref(node);
        }

        public override void VisitTypeOfExpression(TypeOfExpressionSyntax node)
        {
            base.VisitTypeOfExpression(node);
        }

        public override void VisitTypeParameter(TypeParameterSyntax node)
        {
            base.VisitTypeParameter(node);
        }

        public override void VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
        {
            base.VisitTypeParameterConstraintClause(node);
        }

        public override void VisitTypeParameterList(TypeParameterListSyntax node)
        {
            // TypeParameterList: TypeParameter+

            Productions.Instance.Add(node);

            base.VisitTypeParameterList(node);
        }

        public override void VisitTypePattern(TypePatternSyntax node)
        {
            base.VisitTypePattern(node);
        }

        public override void VisitUnaryPattern(UnaryPatternSyntax node)
        {
            base.VisitUnaryPattern(node);
        }

        public override void VisitUndefDirectiveTrivia(UndefDirectiveTriviaSyntax node)
        {
            base.VisitUndefDirectiveTrivia(node);
        }

        public override void VisitUnsafeStatement(UnsafeStatementSyntax node)
        {
            base.VisitUnsafeStatement(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            // UsingDirective
            // : IdentifierName
            // | QualifiedName

            Productions.Instance.Add(node);

            base.VisitUsingDirective(node);

            string name = string.IsNullOrEmpty(qualifiedName) ? string.Join(".", identifierNames) : qualifiedName;
        }

        public override void VisitUsingStatement(UsingStatementSyntax node)
        {
            base.VisitUsingStatement(node);
        }

        public override void VisitVariableDeclaration(VariableDeclarationSyntax node)
        {
            // VariableDeclaration
            // : (IdentifierName | GenericName | PredefinedType | ArrayType | NullableType) VariableDeclarator

            Productions.Instance.Add(node);

            base.VisitVariableDeclaration(node);
        }

        public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
        {
            // VariableDeclarator: EqualsValueClause?

            //Productions.Instance.Add(node);

            //base.VisitVariableDeclarator(node);
        }

        public override void VisitVarPattern(VarPatternSyntax node)
        {
            base.VisitVarPattern(node);
        }

        public override void VisitWarningDirectiveTrivia(WarningDirectiveTriviaSyntax node)
        {
            base.VisitWarningDirectiveTrivia(node);
        }

        public override void VisitWhenClause(WhenClauseSyntax node)
        {
            base.VisitWhenClause(node);
        }

        public override void VisitWhereClause(WhereClauseSyntax node)
        {
            base.VisitWhereClause(node);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            base.VisitWhileStatement(node);
        }

        public override void VisitWithExpression(WithExpressionSyntax node)
        {
            base.VisitWithExpression(node);
        }

        public override void VisitXmlCDataSection(XmlCDataSectionSyntax node)
        {
            base.VisitXmlCDataSection(node);
        }

        public override void VisitXmlComment(XmlCommentSyntax node)
        {
            base.VisitXmlComment(node);
        }

        public override void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
        {
            base.VisitXmlCrefAttribute(node);
        }

        public override void VisitXmlElement(XmlElementSyntax node)
        {
            base.VisitXmlElement(node);
        }

        public override void VisitXmlElementEndTag(XmlElementEndTagSyntax node)
        {
            base.VisitXmlElementEndTag(node);
        }

        public override void VisitXmlElementStartTag(XmlElementStartTagSyntax node)
        {
            base.VisitXmlElementStartTag(node);
        }

        public override void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
        {
            base.VisitXmlEmptyElement(node);
        }

        public override void VisitXmlName(XmlNameSyntax node)
        {
            base.VisitXmlName(node);
        }

        public override void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
        {
            base.VisitXmlNameAttribute(node);
        }

        public override void VisitXmlPrefix(XmlPrefixSyntax node)
        {
            base.VisitXmlPrefix(node);
        }

        public override void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
        {
            base.VisitXmlProcessingInstruction(node);
        }

        public override void VisitXmlText(XmlTextSyntax node)
        {
            base.VisitXmlText(node);
        }

        public override void VisitXmlTextAttribute(XmlTextAttributeSyntax node)
        {
            base.VisitXmlTextAttribute(node);
        }

        public override void VisitYieldStatement(YieldStatementSyntax node)
        {
            base.VisitYieldStatement(node);
        }
    }
}
