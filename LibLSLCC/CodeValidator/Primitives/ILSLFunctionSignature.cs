#region FileInfo
// 
// File: ILSLFunctionSignature.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2016, Eric A. Blundell
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

using System;
using System.Collections.Generic;
using LibLSLCC.Collections;

namespace LibLSLCC.CodeValidator
{
    /// <summary>
    /// A read only interface for <see cref="LSLFunctionSignature"/>.
    /// </summary>
    public interface ILSLFunctionSignature
    {
        /// <summary>
        ///     Returns the number of parameters the function signature has including variadic parameters.
        /// </summary>
        int ParameterCount { get; }

        /// <summary>
        ///     Returns the number of non variadic parameters the function signature has.
        /// </summary>
        int ConcreteParameterCount { get; }

        /// <summary>
        ///     The functions LSL return type.
        /// </summary>
        LSLType ReturnType { get;  }

        /// <summary>
        ///     The functions name, must follow LSL symbol naming conventions.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Indexable list of objects describing the functions parameters.
        /// </summary>
        IReadOnlyGenericArray<LSLParameterSignature> Parameters { get; }

        /// <summary>
        ///     An enumerable of all non-variadic parameters in the function signature.
        /// </summary>
        IEnumerable<LSLParameterSignature> ConcreteParameters { get; }

        /// <summary>
        ///     Returns a formated signature string for the function signature without a trailing semi-colon. <para/>
        ///     Such as:  float llAbs(float value) Or: modInvokeN(string fname, params any[] parms) <para/>
        ///     The later being a function from OpenSim's modInvoke API to demonstrate variadic parameter formating. <para/>
        ///     If a parameter is variadic and has a type that is not void, the 'any' keyword will be replaced with the corresponding name for the type.
        /// </summary>
        string SignatureString { get; }

        /// <summary>
        ///     Whether or not a variadic parameter has been added to this function signature. <para/>
        ///     There can only be one variadic parameter.
        /// </summary>
        bool HasVariadicParameter { get; }

        /// <summary>
        ///     The index of the variadic parameter in the Parameters list, or -1 if none exists.
        /// </summary>
        int VariadicParameterIndex { get; }


        /// <summary>
        ///     Delegates to <see cref="SignatureString"/>.
        /// </summary>
        /// <returns>
        ///     <see cref="SignatureString"/>
        /// </returns>
        string ToString();


        /// <summary>
        ///     Determines if two function signatures match exactly (including return type), parameter names do not matter but
        ///     parameter types do.
        /// </summary>
        /// <param name="otherSignature">The other function signature to compare to</param>
        /// <returns>True if the two signatures are identical</returns>
        /// <exception cref="ArgumentNullException"><paramref name="otherSignature"/> is <c>null</c>.</exception>
        bool SignatureEquivalent(ILSLFunctionSignature otherSignature);


        /// <summary>
        ///     Determines if a given <see cref="LSLFunctionSignature" /> is a duplicate definition of this function signature. <para/>
        ///     The logic behind this is a bit different than SignatureMatches(). <para/>
        ///     If the given function signature has the same name, a differing return type and both functions have no parameters;
        ///     than this function will return true
        ///     and <see cref="SignatureEquivalent(ILSLFunctionSignature)" /> will not. <para/>
        ///     If the other signature is an overload that is ambiguous in all cases due to variadic parameters, this function
        ///     returns true.
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <param name="otherSignature">The other function signature to compare to</param>
        /// <returns>
        ///     True if the two signatures are duplicate definitions of each other, taking static overloading ambiguities into
        ///     account.
        /// </returns>
        bool DefinitionIsDuplicate(ILSLFunctionSignature otherSignature);


        /// <summary>
        ///     <see cref="GetHashCode"/> uses <see cref="ILSLFunctionSignature.Name" />, <see cref="LSLFunctionSignature.ReturnType" /> and the 
        ///     <see cref="LSLParameterSignature.Type" />/<see cref="LSLParameterSignature.Variadic" /> status of the parameters. <para/>
        ///     this means the Hash Code is linked the Function name, return Type and the Types/Variadic'ness of all its parameters. <para/>
        ///     Inherently, uniqueness is also determined by the number of parameters.
        /// </summary>
        /// <returns>Hash code for this <see cref="LSLFunctionSignature" /></returns>
        int GetHashCode();


        /// <summary>
        ///     Equals(object obj) delegates to <see cref="LSLFunctionSignature.SignatureEquivalent" />
        /// </summary>
        /// <param name="obj">The other function signature</param>
        /// <returns>Equality</returns>
        bool Equals(object obj);
    }
}