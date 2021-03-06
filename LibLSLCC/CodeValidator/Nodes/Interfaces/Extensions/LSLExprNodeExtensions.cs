#region FileInfo

// 
// File: LSLExprNodeInterfaceExtensions.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Eric A. Blundell
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 

#endregion

#region Imports

#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Various extensions for dealing with syntax tree node interfaces.
    /// </summary>
    public static class LSLExprNodeExtensions
    {
        /// <summary>
        /// Determine if a given expressions parent node is a prefix negate operator.
        /// </summary>
        /// <param name="node">The <see cref="ILSLReadOnlyExprNode"/> to test.</param>
        /// <returns><c>true</c> if <see cref="ILSLReadOnlySyntaxTreeNode.Parent"/> is an <see cref="ILSLPrefixOperationNode"/> where <see cref="ILSLPrefixOperationNode.Operation"/> equals <see cref="LSLPrefixOperationType.Negate"/>.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters")]
        public static bool IsNegated(this ILSLReadOnlyExprNode node)
        {
            var parentAsPrefixOperator = node.Parent as ILSLPrefixOperationNode;
            return parentAsPrefixOperator != null &&
                   parentAsPrefixOperator.Operation == LSLPrefixOperationType.Negate;
        }


        /// <summary>
        ///     Determines if the expression node represents a code literal.  Such as a string, vector, rotation or list literal.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a code literal.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsLiteral(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.Literal;
        }


        /// <summary>
        ///     Determines if the expression node represents a compound expression.
        ///     Compound expressions are:
        ///     Binary Expressions
        ///     Parenthesized Expressions
        ///     Postfix Expressions
        ///     Prefix Expressions
        ///     Typecast Expressions
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a compound expression.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsCompoundExpression(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.BinaryExpression ||
                   node.ExpressionType == LSLExpressionType.ParenthesizedExpression ||
                   node.ExpressionType == LSLExpressionType.PostfixExpression ||
                   node.ExpressionType == LSLExpressionType.PrefixExpression ||
                   node.ExpressionType == LSLExpressionType.TypecastExpression;
        }


        /// <summary>
        ///     Determines if an expression node represents a function call to a user defined or library defined function.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a function call to either a user defined or library defined function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsFunctionCall(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.LibraryFunction ||
                   node.ExpressionType == LSLExpressionType.UserFunction;
        }


        /// <summary>
        ///     Determines if an expression node represents a function call to a library defined function.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a function call to a library defined function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsLibraryFunctionCall(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.LibraryFunction;
        }


        /// <summary>
        ///     Determines if an expression node represents a function call to a user defined function.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a function call to a user defined function.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsUserFunctionCall(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.UserFunction;
        }


        /// <summary>
        ///     Determines if an expression node represents a reference to a global or local variable.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to either a global or local variable.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsVariable(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.GlobalVariable ||
                   node.ExpressionType == LSLExpressionType.LocalVariable;
        }


        /// <summary>
        ///     Determines whether the expression is a reference to a vector or rotation component, using the dot operator.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>
        ///     True if the expression node represents a reference to a vector or rotation variable component via the dot
        ///     operator.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsVectorOrRotationComponent(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.VectorComponentAccess ||
                   node.ExpressionType == LSLExpressionType.RotationComponentAccess;
        }


        /// <summary>
        ///     Determines whether the given expression is a modifiable L-Value.
        ///     This is true if its a global variable, local variable, local parameter or a reference to a vector component via the
        ///     dot operator.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression is a modifiable L-Value.</returns>
        public static bool IsModifiableLeftValue(this ILSLReadOnlyExprNode node)
        {
            return node.IsVariableOrParameter() || node.IsVectorOrRotationComponent();
        }


        /// <summary>
        ///     Determines if an expression node represents a reference to a global/local variable or parameter reference.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to either a global/local variable or parameter reference.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsVariableOrParameter(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return node.ExpressionType == LSLExpressionType.GlobalVariable ||
                   node.ExpressionType == LSLExpressionType.LocalVariable ||
                   node.ExpressionType == LSLExpressionType.ParameterVariable;
        }


        /// <summary>
        ///     Determines if an expression node represents a reference to a local variable.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to a local variable.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsLocalVariable(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return
                node.ExpressionType == LSLExpressionType.LocalVariable;
        }


        /// <summary>
        ///     Determines if an expression node represents a reference to a local parameter.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to a local parameter.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsLocalParameter(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return
                node.ExpressionType == LSLExpressionType.ParameterVariable;
        }


        /// <summary>
        ///     Determines if an expression node represents a reference to a global variable.
        /// </summary>
        /// <param name="node">The expression node to test.</param>
        /// <returns>True if the expression node represents a reference to a global variable.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="node"/> is <c>null</c>.</exception>
        public static bool IsGlobalVariable(this ILSLReadOnlyExprNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            return
                node.ExpressionType == LSLExpressionType.GlobalVariable;
        }
    }
}