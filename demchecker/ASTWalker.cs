using demchecker.analysis_content;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.Semantics;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker
{
    public class ASTWalker : DepthFirstAstVisitor
    {
        private CSharpAstResolver _resolver;

        //TODO: CSharpInvocationResult, Testar Arrays, etc

        public ASTWalker(string solutionPath)
        {
            DemeterAnalysis.Reset();
            DemeterAnalysis.Current.AddSolution(new Solution(solutionPath));
        }

        public void Parse()
        {
            DemeterAnalysis.Current.CurrentSolution.CreateCompilationUnitsForAllPojects();
            foreach (var project in DemeterAnalysis.Current.CurrentSolution.Projects)
            {
                DemeterAnalysis.Current.AddProject(project);
                foreach (var file in project.Files)
                {
                    DemeterAnalysis.Current.AddFiles(file);
                    _resolver = new CSharpAstResolver(DemeterAnalysis.Current.CurrentProject.Compilation,
                                 DemeterAnalysis.Current.CurrentFile.SyntaxTree,
                                 DemeterAnalysis.Current.CurrentFile.UnresolvedTypeSystemForFile);

                    file.SyntaxTree.AcceptVisitor(this);
                    WalkOnAST(file.SyntaxTree);
                }
            }
        }

        private void WalkOnAST(AstNode node)
        {
            foreach (var child in node.Children)
            {
                WalkOnAST(child);
            }
        }


        public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            var className = typeDeclaration.Name;
            var fullQualifiedName = _resolver.Resolve(typeDeclaration).Type.FullName;

            if (DemeterAnalysis.Current.Classes.FirstOrDefault(c => c.FullQualifiedName == fullQualifiedName) == null)
            {

                DemeterAnalysis.Current.AddClass(new Class()
                {
                    Name = className,
                    FullQualifiedName = fullQualifiedName
                });

                foreach (var variable in typeDeclaration.Descendants.OfType<FieldDeclaration>())
                {
                    Action<string> action = (typeName) => { DemeterAnalysis.Current.CurrentClass.AddDeclaredType(typeName); };
                    checkType(_resolver.Resolve(variable.ReturnType).Type, action);
                }
            }
            base.VisitTypeDeclaration(typeDeclaration);
        }

        private void checkType(IType type, Action<string> action)
        {
            var parameterizedType = type as ParameterizedType;
            if (parameterizedType != null)
            {
                foreach(var param in parameterizedType.TypeArguments)
                {
                    checkType(param, action);
                }
            }
            action(type.FullName);
        }

        public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            var method = new Method(DemeterAnalysis.Current.CurrentClass, methodDeclaration.Name);
            foreach(var parameter in methodDeclaration.Parameters)
            {
                Action<string> action = (fullQualifiedParameterName) => { method.AddParameterType(fullQualifiedParameterName); };
                checkType(_resolver.Resolve(parameter).Type, action);
            }

            foreach (var variable in methodDeclaration.Descendants.OfType<VariableInitializer>())
            {
                Action<string> action = (fullQualifiedVariableName) => { method.AddLocalVariable(fullQualifiedVariableName); };
                checkType(_resolver.Resolve(variable).Type, action);
            }
            DemeterAnalysis.Current.AddMethod(method);
            base.VisitMethodDeclaration(methodDeclaration);
        }

        public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
        {
            base.VisitMemberReferenceExpression(memberReferenceExpression);
            if (IsLoDViolation(memberReferenceExpression))
            {
                var inAMethodBody = IsInAMethodBody(memberReferenceExpression);

                var violation = new Violation(DemeterAnalysis.Current.CurrentSolution,
                                              DemeterAnalysis.Current.CurrentProject,
                                              DemeterAnalysis.Current.CurrentFile,
                                              memberReferenceExpression.GetText(),
                                              memberReferenceExpression.StartLocation.Line, 
                                              DemeterAnalysis.Current.CurrentClass,
                                              DemeterAnalysis.Current.CurrentMethod);
                DemeterAnalysis.Current.AddViolation(violation);
            }
        }

        private bool IsLoDViolation(MemberReferenceExpression expression)
        {
            return !(IsPreferredSupplierClass(expression.Target) ||
                   IsPreferedAcquaitanceClass(expression.Target));
        }
        
        private bool IsPreferredSupplierClass(Expression expression)
        {
            var typeName = _resolver.Resolve(expression).Type.FullName;

            if (IsInAMethodBody(expression))
            {
                return DemeterAnalysis.Current.CurrentClass.DeclaredTypes.Contains(typeName) ||
                       DemeterAnalysis.Current.CurrentMethod.ParameterTypes.Contains(typeName);
            }
            else
            {
                return DemeterAnalysis.Current.CurrentClass.DeclaredTypes.Contains(typeName);
            }
        }

        private bool IsPreferedAcquaitanceClass(Expression expression)
        {
            var type = _resolver.Resolve(expression);
            var resolvedType = type.Type;

            if (IsInAMethodBody(expression))
            {
                return IsGlobal(resolvedType) ||
                        DemeterAnalysis.Current.CurrentMethod.LocalVariables.Contains(type.Type.FullName);
                
            }
            else
            {
                return IsGlobal(resolvedType);
            }
        }

        private bool IsGlobal(IType type)
        {
            if (type is DefaultResolvedTypeDefinition)
            {
                return type.GetDefinition().IsStatic && (type.GetDefinition().Accessibility == ICSharpCode.NRefactory.TypeSystem.Accessibility.Public);
            }
            return false;
        }

        private bool IsInAMethodBody(AstNode node)
        {
            return node.Ancestors.OfType<MethodDeclaration>().Any();
        }
    }
}
