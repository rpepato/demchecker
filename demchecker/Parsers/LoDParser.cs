using demchecker.analysis_content;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.CSharp.Resolver;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demchecker.Parsers
{
    class LoDParser : DepthFirstAstVisitor, IParser
    {
        private CSharpAstResolver _resolver;

        protected delegate void ProjecIdentifiedEventHandler(Project project);
        protected delegate void FileIdentifiedEventHandler(File file);

        protected event ProjecIdentifiedEventHandler OnProjectIdentified;
        protected event FileIdentifiedEventHandler OnFileIdentified;

        public virtual void Parse(string solutionPath)
        {
            DemeterAnalysis.Reset();
            DemeterAnalysis.Current.AddSolution(new Solution(solutionPath));
            DemeterAnalysis.Current.CurrentSolution.CreateCompilationUnitsForAllPojects();
            foreach(var project in DemeterAnalysis.Current.CurrentSolution.Projects)
            {
                EmitProjecIdentifiedEvent(project);
                DemeterAnalysis.Current.AddProject(project);

                foreach(var file in project.Files)
                {
                    EmitFileIdentifiedEvent(file);
                    DemeterAnalysis.Current.AddFiles(file);
                    _resolver = new CSharpAstResolver(DemeterAnalysis.Current.CurrentProject.Compilation,
                                                      DemeterAnalysis.Current.CurrentFile.SyntaxTree,
                                                      DemeterAnalysis.Current.CurrentFile.UnresolvedTypeSystemForFile);
                    file.SyntaxTree.AcceptVisitor(this);
                    ParseAST(file.SyntaxTree);
                }
            }
        }

        public override void VisitTypeDeclaration(TypeDeclaration typeDeclaration)
        {
            // We need to fetch the types os members declared on classes, so we
            // are interested in class declarions only
            if (typeDeclaration.ClassType != ClassType.Class)
            {
                return;
            }

            var className = typeDeclaration.Name;
            // We need to obtain the full qualified name, so the unresolved type must
            // be resolved before we get it
            var fullQualifiedClassName = Resolver.Resolve(typeDeclaration).Type.FullName;

            if (!IsClassAlreadyParsed(fullQualifiedClassName))
            {
                DemeterAnalysis.Current.AddClass(new Class()
                {
                    Name = className,
                    FullQualifiedName = fullQualifiedClassName
                });

                // The type of the declaring type should be contained on the list
                // of valid types for invocations
                DemeterAnalysis.Current.CurrentClass.AddDeclaredType(fullQualifiedClassName);
                
                var typeMembers = CollectClassMemberTypes(typeDeclaration);
                typeMembers.ForEach(member => DemeterAnalysis.Current.CurrentClass.AddDeclaredType(member));

            }
            base.VisitTypeDeclaration(typeDeclaration);
        }

        public override void VisitMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            var method = new Method(DemeterAnalysis.Current.CurrentClass, methodDeclaration.Name);
            DemeterAnalysis.Current.AddMethod(method);
            CollectMethodParametersTypes(methodDeclaration).ForEach(parameter => DemeterAnalysis.Current.CurrentMethod.AddParameterType(parameter));
            CollectMethodScopedVariableTypes(methodDeclaration).ForEach(var => DemeterAnalysis.Current.CurrentMethod.AddLocalVariable(var));
 	        base.VisitMethodDeclaration(methodDeclaration);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration)
        {
            base.VisitConstructorDeclaration(constructorDeclaration);
            var constructor = new Method(DemeterAnalysis.Current.CurrentClass, constructorDeclaration.Name);
 	        CollectConstructorParametersTypes(constructorDeclaration).ForEach(parameter => DemeterAnalysis.Current.CurrentMethod.AddParameterType(parameter));
            CollectMethodScopedVariableTypes(constructorDeclaration).ForEach(var => DemeterAnalysis.Current.CurrentMethod.AddLocalVariable(var));
        }

        public override void VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression)
        {
            base.VisitMemberReferenceExpression(memberReferenceExpression);

            DemeterAnalysis.Current.IncrementInspectedInstruction();

            if (IsViolation(memberReferenceExpression))
            {
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

        protected virtual bool IsViolation(MemberReferenceExpression expression)
        {
            return !(IsPreferredSupplier(expression.Target) ||
                     IsPreferredAcquaitanceClass(expression.Target));
        }

        protected virtual bool IsPreferredSupplier(Expression expression)
        {
            if (expression is ThisReferenceExpression || expression is BaseReferenceExpression)
            {
                return true;
            }

            var resolved = Resolver.Resolve(expression).Type;

            // All value types are autommatically accepted
            if (resolved.IsReferenceType.HasValue && !resolved.IsReferenceType.Value)
            {
                return true;
            }

            var typeName = resolved.FullName;

            AstNode node = (AstNode)expression;

            if (IsInAMethodBody(node))
            {
                return DemeterAnalysis.Current.CurrentClass.DeclaredTypes.Contains(typeName) ||
                       DemeterAnalysis.Current.CurrentMethod.ParameterTypes.Contains(typeName);
            }
            else
            {
                return DemeterAnalysis.Current.CurrentClass.DeclaredTypes.Contains(typeName);
            }
            
        }

        public virtual bool IsPreferredAcquaitanceClass(Expression expression)
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

        protected virtual bool IsInAMethodBody(AstNode node)
        {
            return (node.Ancestors.OfType<MethodDeclaration>().Any() ||
                    node.Ancestors.OfType<ConstructorDeclaration>().Any());
        }

        protected virtual List<string> CollectClassMemberTypes(TypeDeclaration typeDeclaration)
        {
            return typeDeclaration.Descendants.OfType<FieldDeclaration>().
                Select(member => Resolver.Resolve(member.ReturnType).Type.FullName).
                ToList();
        }

        protected virtual List<string> CollectMethodParametersTypes(MethodDeclaration methodDeclaration)
        {
            return methodDeclaration.Parameters.Select(parameter => Resolver.Resolve(parameter).Type.FullName).ToList();
        }

        protected virtual List<string> CollectMethodScopedVariableTypes(EntityDeclaration entityDeclaration)
        {
            return entityDeclaration.Descendants.OfType<VariableInitializer>().Select(var => Resolver.Resolve(var).Type.FullName).ToList();
        }

        protected virtual List<string> CollectConstructorParametersTypes(ConstructorDeclaration constructorDeclaration)
        {
            return constructorDeclaration.Parameters.Select(parameter => Resolver.Resolve(parameter).Type.FullName).ToList();
        }

        private bool IsClassAlreadyParsed(string fullQualifiedName)
        {
            return DemeterAnalysis.Current.Classes.Any(c => c.FullQualifiedName == fullQualifiedName);
        }

        private void ParseAST(AstNode targetNode)
        {
            foreach(var child in targetNode.Children)
            {
                ParseAST(child);
            }
        }

        private void EmitProjecIdentifiedEvent(Project project)
        {
            if (OnProjectIdentified != null)
            {
                OnProjectIdentified(project);
            }
        }

        private void EmitFileIdentifiedEvent(File file)
        {
            if (OnFileIdentified != null)
            {
                OnFileIdentified(file);
            }
        }

        protected CSharpAstResolver Resolver
        {
            get
            {
                return _resolver;
            }
        }
    }
}
