//The MIT License (MIT)

//Copyright (c) 2015 Roberto Pepato

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
//the Software, and to permit persons to whom the Software is furnished to do so,
//subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.TypeSystem;
using NRefactoryCUBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using demchecker.extension_methods;

namespace demchecker.Parsers
{
    class LoDEParser : LoDParser
    {
        IList<string> _projectNamespaces;

        public LoDEParser()
        {
            base.OnProjectIdentified += LoDEParser_OnProjectIdentified;
        }

        void LoDEParser_OnProjectIdentified(Project project)
        {
            _projectNamespaces = project.GetNamespaceDeclarations();    
        }

        protected override List<string> CollectClassMemberTypes(ICSharpCode.NRefactory.CSharp.TypeDeclaration typeDeclaration)
        {
            var classMemberTypes = new List<string>();
            Action<string> action = (typeName) => { classMemberTypes.Add(typeName); };
            foreach (var variable in typeDeclaration.Descendants.OfType<FieldDeclaration>())
            {
                PerformActionForAllTypesAndSubTypesOfParameterizedTypes(Resolver.Resolve(variable.ReturnType).Type, action);
            }
            return classMemberTypes;
        }

        protected override List<string> CollectConstructorParametersTypes(ConstructorDeclaration constructorDeclaration)
        {
            var constructorParameters = new List<string>();
            Action<string> action = (typeName) => { constructorParameters.Add(typeName); };
            foreach (var parameter in constructorDeclaration.Parameters)
            {
                PerformActionForAllTypesAndSubTypesOfParameterizedTypes(Resolver.Resolve(parameter).Type, action);
            }
            return constructorParameters;
        }

        protected override List<string> CollectMethodParametersTypes(MethodDeclaration methodDeclaration)
        {
            var methodParameters = new List<string>();
            Action<string> action = (typeName) => { methodParameters.Add(typeName); };
            foreach(var parameter in methodDeclaration.Parameters)
            {
                PerformActionForAllTypesAndSubTypesOfParameterizedTypes(Resolver.Resolve(parameter).Type, action);
            }
            return methodParameters;
        }

        protected override List<string> CollectMethodScopedVariableTypes(EntityDeclaration entityDeclaration)
        {
            var localScopedVariables = new List<string>();
            Action<string> action = (typeName) => { localScopedVariables.Add(typeName); };
            foreach(var variable in entityDeclaration.Descendants.OfType<VariableInitializer>())
            {
                PerformActionForAllTypesAndSubTypesOfParameterizedTypes(Resolver.Resolve(variable).Type, action);
            }
            return localScopedVariables;
        }

        protected override List<string> CollectClassProperties(TypeDeclaration typeDeclaration)
        {
            var classProperties = new List<string>();
            Action<string> action = (typeName) => { classProperties.Add(typeName); };
            foreach(var property in typeDeclaration.Descendants.OfType<PropertyDeclaration>())
            {
                PerformActionForAllTypesAndSubTypesOfParameterizedTypes(Resolver.Resolve(property).Type, action);
            }
            return classProperties;
        }

        protected override bool IsPreferredSupplier(Expression expression)
        {
            if (IsInLambdaExpression(expression))
            {
                if (IsLambdaIdentifier(expression))
                {
                    return true;
                }
            }
            return base.IsPreferredSupplier(expression);
        }

        protected override bool IsViolation(MemberReferenceExpression expression)
        {
            return base.IsViolation(expression) &&
                   IsInKnownNamespace(expression.Target);
        }

        private bool IsInKnownNamespace(Expression expression)
        {
            return _projectNamespaces.Contains(Resolver.Resolve(expression).Type.Namespace.ToUpper());
        }

        private void PerformActionForAllTypesAndSubTypesOfParameterizedTypes(IType type, Action<string> action)
        {
            var parameterizedType = type as ParameterizedType;
            if (parameterizedType != null)
            {
                foreach (var param in parameterizedType.TypeArguments)
                {
                    PerformActionForAllTypesAndSubTypesOfParameterizedTypes(param, action);
                }
            }
            action(type.FullName);
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
            var res = Resolver.Resolve(expression);
            return identifiers.Any(i => i.Name == identifierExpression.Identifier && Resolver.Resolve(i).Type.Name == res.Type.Name);
        }
    }
}
