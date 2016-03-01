﻿#region FileInfo

// 
// File: LSLPrefixOperationNode.cs
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

using System;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.AntlrParser;

#endregion

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    ///     Default <see cref="ILSLPrefixOperationNode" /> implementation used by <see cref="LSLCodeValidator" />
    /// </summary>
    public sealed class LSLPrefixOperationNode : ILSLPrefixOperationNode, ILSLExprNode
    {
        private ILSLSyntaxTreeNode _parent;
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        private LSLPrefixOperationNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceRange = sourceRange;
            HasErrors = true;
        }


        /// <exception cref="ArgumentNullException">
        ///     <paramref name="context" /> or <paramref name="rightExpression" /> is
        ///     <c>null</c>.
        /// </exception>
        internal LSLPrefixOperationNode(LSLParser.Expr_PrefixOperationContext context, LSLType resultType,
            ILSLExprNode rightExpression)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }


            Type = resultType;
            RightExpression = rightExpression;
            RightExpression.Parent = this;

            ParseAndSetOperation(context.operation.Text);

            SourceRange = new LSLSourceCodeRange(context);

            SourceRangeOperation = new LSLSourceCodeRange(context.operation);

            SourceRangesAvailable = true;
        }


        /// <summary>
        ///     Construct an <see cref="LSLPostfixOperationNode" /> from a given <see cref="ILSLExprNode" /> and
        ///     <see cref="LSLPostfixOperationType" />.
        /// </summary>
        /// <param name="resultType">The return type of the postfix operation on the given expression.</param>
        /// <param name="rightExpression">The expression the postfix operation occurs on.</param>
        /// <param name="operationType">The postfix operation type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="rightExpression" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="operationType" /> is <see cref="LSLPrefixOperationType.Error" />.</exception>
        public LSLPrefixOperationNode(LSLType resultType, LSLPrefixOperationType operationType,
            ILSLExprNode rightExpression)
        {
            if (rightExpression == null)
            {
                throw new ArgumentNullException("rightExpression");
            }

            if (operationType == LSLPrefixOperationType.Error)
            {
                throw new ArgumentException("operationType cannot be LSLPrefixOperationType.Error.");
            }

            Type = resultType;
            RightExpression = rightExpression;
            RightExpression.Parent = this;

            Operation = operationType;
            OperationString = Operation.ToOperatorString();
        }


        /// <summary>
        ///     Create an <see cref="LSLPrefixOperationNode" /> by cloning from another.
        /// </summary>
        /// <param name="other">The other node to clone from.</param>
        /// <exception cref="ArgumentNullException"><paramref name="other" /> is <c>null</c>.</exception>
        private LSLPrefixOperationNode(LSLPrefixOperationNode other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }


            SourceRangesAvailable = other.SourceRangesAvailable;

            if (SourceRangesAvailable)
            {
                SourceRange = other.SourceRange;
                SourceRangeOperation = other.SourceRangeOperation;
            }

            Type = other.Type;

            RightExpression = other.RightExpression.Clone();
            RightExpression.Parent = this;

            Operation = other.Operation;


            HasErrors = other.HasErrors;
        }


        /// <summary>
        ///     The expression that is right of the prefix operator, this should never be null.
        /// </summary>
        public ILSLExprNode RightExpression { get; private set; }

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        /// <summary>
        ///     The prefix operation type preformed on the expression.
        ///     <see cref="LSLPrefixOperationType" />
        /// </summary>
        public LSLPrefixOperationType Operation { get; private set; }

        /// <summary>
        ///     The prefix operation string taken from the source code.
        /// </summary>
        public string OperationString { get; private set; }

        ILSLReadOnlyExprNode ILSLPrefixOperationNode.RightExpression
        {
            get { return RightExpression; }
        }


        /// <summary>
        ///     Returns a version of this node type that represents its error state;  in case of a syntax error
        ///     in the node that prevents the node from being even partially built.
        /// </summary>
        /// <param name="sourceRange">The source code range of the error.</param>
        /// <returns>A version of this node type in its undefined/error state.</returns>
        public static
            LSLPrefixOperationNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLPrefixOperationNode(sourceRange, Err.Err);
        }


        private void ParseAndSetOperation(string operationString)
        {
            OperationString = operationString;
            Operation = LSLPrefixOperationTypeTools.ParseFromOperator(OperationString);
        }

        #region Nested type: Err

        private enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        ///     Deep clones the expression node.  It should clone the node and all of its children and cloneable properties, except
        ///     the parent.
        ///     When cloned, the parent node reference should be left <c>null</c>.
        /// </summary>
        /// <returns>A deep clone of this expression tree node.</returns>
        public LSLPrefixOperationNode Clone()
        {
            return HasErrors ? GetError(SourceRange) : new LSLPrefixOperationNode(this);
        }


        ILSLExprNode ILSLExprNode.Clone()
        {
            return Clone();
        }


        /// <summary>
        ///     The parent node of this syntax tree node.
        /// </summary>
        /// <exception cref="InvalidOperationException" accessor="set">If Parent has already been set.</exception>
        /// <exception cref="ArgumentNullException" accessor="set"><paramref name="value" /> is <c>null</c>.</exception>
        public ILSLSyntaxTreeNode Parent
        {
            get { return _parent; }
            set
            {
                if (_parent != null)
                {
                    throw new InvalidOperationException(GetType().Name +
                                                        ": Parent node already set, it can only be set once.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value", GetType().Name + ": Parent cannot be set to null.");
                }

                _parent = value;
            }
        }


        /// <summary>
        ///     True if this syntax tree node contains syntax errors. <para/>
        ///     <see cref="SourceRange"/> should point to a more specific error location when this is <c>true</c>. <para/>
        ///     Other source ranges will not be available.
        /// </summary>
        public bool HasErrors { get; internal set; }


        /// <summary>
        ///     The source code range that this syntax tree node occupies.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRange { get; private set; }

        /// <summary>
        ///     Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceRangesAvailable { get; private set; }


        /// <summary>
        ///     The source code range the prefix operator occupies.
        /// </summary>
        /// <remarks>
        ///     If <see cref="ILSLReadOnlySyntaxTreeNode.SourceRangesAvailable" /> is <c>false</c> this property will be
        ///     <c>null</c>.
        /// </remarks>
        public LSLSourceCodeRange SourceRangeOperation { get; private set; }


        /// <summary>
        ///     Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}" />
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="visitor"/> is <c>null</c>.</exception>
        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");

            return visitor.VisitPrefixOperation(this);
        }


        /// <summary>
        ///     The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type { get; private set; }


        /// <summary>
        ///     The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        ///     The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.PrefixExpression; }
        }

        /// <summary>
        ///     True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return RightExpression != null && RightExpression.IsConstant; }
        }

        /// <summary>
        ///     True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a
        ///     function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects
        {
            get
            {
                if (RightExpression == null) return false;

                var modifiesState = Operation.IsModifying() && RightExpression.IsModifiableLeftValue();

                return (RightExpression.HasPossibleSideEffects || modifiesState);
            }
        }


        /// <summary>
        ///     Should produce a user friendly description of the expressions return type.
        ///     This is used in some syntax error messages, Ideally you should enclose your description in
        ///     parenthesis or something that will make it stand out in a string.
        /// </summary>
        /// <returns></returns>
        public string DescribeType()
        {
            return "(" + Type + (this.IsLiteral() ? " Literal)" : ")");
        }


        ILSLReadOnlyExprNode ILSLReadOnlyExprNode.Clone()
        {
            return Clone();
        }

        #endregion
    }
}