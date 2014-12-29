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
using demchecker.extension_methods;
using ICSharpCode.NRefactory;

namespace demchecker
{
    public class ASTWalker : DepthFirstAstVisitor
    {
        private CSharpAstResolver _resolver;
        private IList<string> projectNamespaces;

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
                projectNamespaces = project.GetNamespaceDeclarations();
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
            if (typeDeclaration.ClassType != ClassType.Class)
            {
                return;
            }
            var className = typeDeclaration.Name;
            var fullQualifiedName = _resolver.Resolve(typeDeclaration).Type.FullName;

            if (DemeterAnalysis.Current.Classes.FirstOrDefault(c => c.FullQualifiedName == fullQualifiedName) == null)
            {
                DemeterAnalysis.Current.AddClass(new Class()
                {
                    Name = className,
                    FullQualifiedName = fullQualifiedName
                });

                DemeterAnalysis.Current.CurrentClass.AddDeclaredType(fullQualifiedName);

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

        public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        {
            base.VisitConstructorDeclaration(constructorDeclaration);

            var constructor = new Method(DemeterAnalysis.Current.CurrentClass, constructorDeclaration.Name);
            foreach (var parameter in constructorDeclaration.Parameters)
            {
                Action<string> action = (fullQualifiedParameterName) => { constructor.AddParameterType(fullQualifiedParameterName); };
                checkType(_resolver.Resolve(parameter).Type, action);
            }

            foreach (var variable in constructorDeclaration.Descendants.OfType<VariableInitializer>())
            {
                Action<string> action = (fullQualifiedVariableName) => { constructor.AddLocalVariable(fullQualifiedVariableName); };
                checkType(_resolver.Resolve(variable).Type, action);
            }
            DemeterAnalysis.Current.AddMethod(constructor); 
        }

        public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
        {
            base.VisitMemberReferenceExpression(memberReferenceExpression);

            DemeterAnalysis.Current.IncrementInspectedInstruction();

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
            var violation =  !(IsPreferredSupplierClass(expression.Target) ||
                               IsPreferedAcquaitanceClass(expression.Target));
                               // || IsStaticCall(expression));  --> generating errors

            return violation && IsInKnownNamespace(expression.Target);
        }

        private bool IsInKnownNamespace(Expression expression)
        {
            var nameSpace = _resolver.Resolve(expression).Type.Namespace;
            return projectNamespaces.Contains(nameSpace.ToUpper());
        }

        private bool IsInLambdaExpression(Expression expression)
        {
            return expression.Ancestors.OfType<LambdaExpression>().Any();
        }

        private bool IsLambdaIdentifier(Expression expression)
        {
            var identifierExpression = expression as IdentifierExpression;
            if (identifierExpression == null)
            {
                return false;
            }
            var lambdaExpression = expression.Ancestors.OfType<LambdaExpression>().First();
            var identifiers = lambdaExpression.Parameters;
            if (identifiers.Count == 0)
            {
                return false;
            }
            var res = _resolver.Resolve(expression);
            return identifiers.Any(i => i.Name == identifierExpression.Identifier && _resolver.Resolve(i).Type.Name == res.Type.Name);
        }

        private bool IsPreferredSupplierClass(Expression expression)
        {
            if (expression is ThisReferenceExpression || expression is BaseReferenceExpression)
            {
                return true;
            }

            if (IsInLambdaExpression(expression))
            {
                if (IsLambdaIdentifier(expression))
                {
                    return true;
                }
            }

            var resolved = _resolver.Resolve(expression).Type;

            if ((resolved.IsReferenceType.HasValue ? !resolved.IsReferenceType.Value : false))
            {
                return true;
            }
            
            var typeName = resolved.FullName;

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
                return type.GetDefinition().IsStatic;  //&& (type.GetDefinition().Accessibility == ICSharpCode.NRefactory.TypeSystem.Accessibility.Public);
            }
            return true;
        }

        private bool IsStaticCall(Expression expression)
        {
            var resolvedType = _resolver.Resolve(expression).Type as DefaultResolvedTypeDefinition;
            var memberExpression = expression as MemberReferenceExpression;
            if (resolvedType != null && memberExpression != null)
            {
                return resolvedType.IsStatic ||
                            resolvedType.Methods.Any(m => m.IsStatic && m.Name == memberExpression.MemberName ||
                            resolvedType.Properties.Any(p => p.IsStatic || p.Name == memberExpression.MemberName) ||
                            resolvedType.Fields.Any(f => f.IsConst || f.Name == memberExpression.MemberName));
            }
            return false;
        }

        private bool IsInAMethodBody(AstNode node)
        {
            return (node.Ancestors.OfType<MethodDeclaration>().Any() ||
                    node.Ancestors.OfType<ConstructorDeclaration>().Any());
        }
    }
}
