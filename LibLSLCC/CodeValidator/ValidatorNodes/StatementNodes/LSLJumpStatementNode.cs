﻿#region FileInfo
// 
// File: LSLJumpStatementNode.cs
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
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.CodeValidator.ValidatorNodes.Interfaces;
using LibLSLCC.CodeValidator.ValidatorNodeVisitor;

#endregion

namespace LibLSLCC.CodeValidator.ValidatorNodes.StatementNodes
{
    public class LSLJumpStatementNode : ILSLJumpStatementNode, ILSLCodeStatement
    {
// ReSharper disable UnusedParameter.Local
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "err")]
        protected LSLJumpStatementNode(LSLSourceCodeRange sourceRange, Err err)
// ReSharper restore UnusedParameter.Local
        {
            SourceCodeRange = sourceRange;
            HasErrors = true;
        }

        internal LSLJumpStatementNode(LSLParser.JumpStatementContext context, LSLLabelStatementNode jumpTarget,
            bool isSingleBlockStatement)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (jumpTarget == null)
            {
                throw new ArgumentNullException("jumpTarget");
            }

            IsSingleBlockStatement = isSingleBlockStatement;
            ParserContext = context;
            JumpTarget = jumpTarget;
            jumpTarget.AddJumpToHere(this);
            SourceCodeRange = new LSLSourceCodeRange(context);

            LabelNameSourceCodeRange = new LSLSourceCodeRange(context.jump_target);
            JumpKeywordSourceCodeRange = new LSLSourceCodeRange(context.jump_keyword);
            SemiColonSourceCodeRange = new LSLSourceCodeRange(context.semi_colon);
        }

        internal LSLParser.JumpStatementContext ParserContext { get; private set; }
        public LSLLabelStatementNode JumpTarget { get; set; }

        #region ILSLCodeStatement Members

        public bool IsSingleBlockStatement { get; private set; }
        public ILSLSyntaxTreeNode Parent { get; set; }
        public bool ConstantJump { get; set; }

        public bool IsLastStatementInScope { get; set; }

        public bool IsDeadCode { get; set; }

        ILSLReadOnlyCodeStatement ILSLReadOnlyCodeStatement.ReturnPath
        {
            get { return ReturnPath; }
        }

        public int StatementIndex { get; set; }

        public bool HasReturnPath
        {
            get { return false; }
        }

// ReSharper disable UnusedParameter.Local


        public bool HasErrors { get; set; }

        public LSLSourceCodeRange SourceCodeRange { get; private set; }


        public T AcceptVisitor<T>(ILSLValidatorNodeVisitor<T> visitor)
        {
            return visitor.VisitJumpStatement(this);
        }

        #endregion

        #region Nested type: Err

        protected enum Err
        {
            Err
        }

        #endregion

        public ILSLCodeStatement ReturnPath { get; set; }


        ILSLReadOnlySyntaxTreeNode ILSLReadOnlySyntaxTreeNode.Parent
        {
            get { return Parent; }
        }

        public LSLDeadCodeType DeadCodeType { get; set; }


        public string LabelName
        {
            get { return ParserContext.jump_target.Text; }
        }

        ILSLLabelStatementNode ILSLJumpStatementNode.JumpTarget
        {
            get { return JumpTarget; }
        }


        public ulong ScopeId { get; set; }


        public static
            LSLJumpStatementNode GetError(LSLSourceCodeRange sourceRange)
        {
            return new LSLJumpStatementNode(sourceRange, Err.Err);
        }


        public LSLSourceCodeRange JumpKeywordSourceCodeRange { get; private set; }

        public LSLSourceCodeRange LabelNameSourceCodeRange { get; private set; }

        public LSLSourceCodeRange SemiColonSourceCodeRange { get; private set; }
    }
}