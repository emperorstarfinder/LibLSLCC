﻿#region FileInfo
// 
// File: LSLConstantLiteralNode.cs
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

using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Nodes.Interfaces;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;
using LibLSLCC.Parser;

#endregion

namespace LibLSLCC.CodeValidator.Nodes
{
    public abstract class LSLConstantLiteralNode : ILSLExprNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLConstantLiteralNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }


        public LSLConstantLiteralNode(LSLConstantLiteralNode other)
        {
            RawText = other.RawText;
            Type = other.Type;

            SourceCodeRangesAvailable = other.SourceCodeRangesAvailable;

            if (SourceCodeRangesAvailable)
            {
                SourceCodeRange = other.SourceCodeRange.Clone();
            }

            Parent = other.Parent;
            HasErrors = other.HasErrors;
        }

        protected internal LSLConstantLiteralNode(LSLParser.Expr_AtomContext context, LSLType type)
        {
            RawText = context.children[0].GetText();
            Type = type;
            SourceCodeRange = new LSLSourceCodeRange(context);
            SourceCodeRangesAvailable = true;
        }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLExprNode Members

        /// <summary>
        /// The raw text of the literal taken from the source code.
        /// </summary>
        public string RawText { get; private set; }


        /// <summary>
        /// True if this syntax tree node contains syntax errors.
        /// </summary>
        public bool HasErrors { get; private set; }


        /// <summary>
        /// The source code range that this syntax tree node occupies.
        /// </summary>
        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        /// <summary>
        /// Should return true if source code ranges are available/set to meaningful values for this node.
        /// </summary>
        public bool SourceCodeRangesAvailable { get; private set; }


        /// <summary>
        /// Accept a visit from an implementor of <see cref="ILSLValidatorNodeVisitor{T}"/>
        /// </summary>
        /// <typeparam name="T">The visitors return type.</typeparam>
        /// <param name="visitor">The visitor instance.</param>
        /// <returns>The value returned from this method in the visitor used to visit this node.</returns>
        public abstract T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor);


        /// <summary>
        /// The parent node of this syntax tree node.
        /// </summary>
        public ILSLSyntaxTreeNode Parent { get; set; }



        /// <summary>
        /// The return type of the expression. see: <see cref="LSLType" />
        /// </summary>
        public LSLType Type { get; private set; }


        /// <summary>
        /// The expression type/classification of the expression. see: <see cref="LSLExpressionType" />
        /// </summary>
        /// <value>
        /// The type of the expression.
        /// </value>
        public LSLExpressionType ExpressionType
        {
            get { return LSLExpressionType.Literal; }
        }


        /// <summary>
        /// True if the expression is constant and can be calculated at compile time.
        /// </summary>
        public bool IsConstant
        {
            get { return true; }
        }

        /// <summary>
        /// True if the expression statement has some modifying effect on a local parameter or global/local variable;  or is a function call.  False otherwise.
        /// </summary>
        public bool HasPossibleSideEffects {
            get
            {
                //a literal expression node can never change the state of the surrounding program
                return false;
            }
        }


        /// <summary>
        /// Deep clones the expression node.  It should clone the node and also clone all of its children.
        /// </summary>
        /// <returns>A deep clone of this expression node.</returns>
        public abstract ILSLExprNode Clone();


        /// <summary>
        /// Should produce a user friendly description of the expressions return type.
        /// This is used in some syntax error messages, Ideally you should enclose your description in
        /// parenthesis or something that will make it stand out in a string.
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