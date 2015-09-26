#region FileInfo
// 
// File: LSLElseStatementNode.cs
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodes.ScopeNodes;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLElseStatementNode : ILSLElseStatementNode, ILSLBranchStatementNode
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLElseStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLElseStatementNode(LSLParser.ElseStatementContext context, LSLCodeScopeNode code,
            bool isConstantBranch)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (code == null)
            {
                throw new ArgumentNullException("code");
            }


            ParserContext = context;
            IsConstantBranch = isConstantBranch;
            Code = code;

            code.Parent = this;

            SourceCodeRange = new LSLSourceCodeRange(context);

            ElseKeywordSourceCodeRange = new LSLSourceCodeRange(context.else_keyword);
        }

        public IEnumerable<LSLConstantJumpDescription> ConstantJumps
        {
            get { return Code.ConstantJumps; }
        }

        public LSLCodeScopeNode Code { get; }
        internal LSLParser.ElseStatementContext ParserContext { get; private set; }
        public LSLSourceCodeRange ElseKeywordSourceCodeRange { get; }

        #region ILSLBranchStatementNode Members

        public bool IsConstantBranch { get; }

        #endregion

        #region ILSLReturnPathNode Members

        public bool HasReturnPath
        {
            get { return Code.HasReturnPath; }
        }

        #endregion

        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        ILSLCodeScopeNode ILSLElseStatementNode.Code
        {
            get { return Code; }
        }

        public static
            LSLElseStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLElseStatementNode(sourceRange, Err.Err);
        }

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        #region ILSLTreeNode Members

        public ILSLSyntaxTreeNode Parent { get; set; }
        public LSLSourceCodeRange SourceCodeRange { get; }


        public bool HasErrors { get; set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitElseStatement(this);
        }

        #endregion
    }
}