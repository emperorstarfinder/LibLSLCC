#region FileInfo
// 
// File: LSLLibraryEventSignature.cs
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
using System.Security;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using LibLSLCC.CodeValidator.Enums;
using LibLSLCC.CodeValidator.Primitives;
using LibLSLCC.Collections;

#endregion

namespace LibLSLCC.CodeValidator.Components
{
    [XmlRoot("EventHandler")]
    public sealed class LSLLibraryEventSignature : LSLEventSignature, IXmlSerializable
    {
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private HashSet<string> _subsets = new HashSet<string>();

        private LSLLibraryDataSubsetsAttributeRegex _subsetsRegex = new
            LSLLibraryDataSubsetsAttributeRegex();

        private LSLLibraryEventSignature()
        {
        }

        public LSLLibraryEventSignature(LSLEventSignature sig)
            : base(sig)
        {
            DocumentationString = "";
        }

        public LSLLibraryEventSignature(LSLLibraryEventSignature sig)
            : base(sig)
        {
            DocumentationString = sig.DocumentationString;
            _subsets = new HashSet<string>(sig._subsets);
        }

        public LSLLibraryEventSignature(string name, IEnumerable<LSLParameter> parameters)
            : base(name, parameters)
        {
            DocumentationString = "";
        }

        public LSLLibraryEventSignature(string name)
            : base(name)
        {
            DocumentationString = "";
        }

        public IReadOnlySet<string> Subsets
        {
            get { return new ReadOnlyHashSet<string>(_subsets); }
        }

        public IReadOnlyDictionary<string, string> Properties
        {
            get { return _properties; }
        }

        public string DocumentationString { get; set; }

        public string SignatureAndDocumentation
        {
            get
            {
                if (string.IsNullOrWhiteSpace(DocumentationString))
                {
                    return SignatureString + ";";
                }
                return SignatureString + ";" +
                       Environment.NewLine +
                       DocumentationString;
            }
        }

        /// <summary>
        ///     This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return
        ///     null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the
        ///     <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is
        ///     produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method
        ///     and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" />
        ///     method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        ///     Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized. </param>
        public void ReadXml(XmlReader reader)
        {
            var parameterNames = new HashSet<string>();


            reader.MoveToContent();

            var hasSubsets = false;
            var hasName = false;

            var lineNumberInfo = (IXmlLineInfo) reader;

            while (reader.MoveToNextAttribute())
            {
                if (reader.Name == "Subsets")
                {
                    SetSubsets(reader.Value);
                    hasSubsets = true;
                }
                else if (reader.Name == "Name")
                {
                    if (string.IsNullOrWhiteSpace(reader.Value))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Name attribute invalid");
                    }
                    hasName = true;
                    Name = reader.Value;
                }
                else
                {
                    throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                        GetType().Name + ", unknown attribute " + reader.Name);
                }
            }


            if (!hasName)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing Name attribute");
            }


            if (!hasSubsets)
            {
                throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                    "Missing Subsets attribute");
            }


            var canRead = reader.Read();
            while (canRead)
            {
                if ((reader.Name == "Parameter") && reader.IsStartElement())
                {
                    LSLType pType;

                    if (!Enum.TryParse(reader.GetAttribute("Type"), out pType))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Parameter Type attribute invalid");
                    }

                    var pName = reader.GetAttribute("Name");
                    if (string.IsNullOrWhiteSpace(pName))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Parameter Name attribute invalid");
                    }

                    if (parameterNames.Contains(pName))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            GetType().Name + ", Parameter Name already used");
                    }

                    parameterNames.Add(pName);

                    AddParameter(new LSLParameter(pType, pName, false));

                    canRead = reader.Read();
                }
                else if ((reader.Name == "DocumentationString") && reader.IsStartElement())
                {
                    DocumentationString = reader.ReadElementContentAsString();
                    canRead = reader.Read();
                }
                else if ((reader.Name == "Property") && reader.IsStartElement())
                {
                    var name = reader.GetAttribute("Name");

                    if (string.IsNullOrWhiteSpace(name))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Event {1}: Property element's Name attribute cannot be empty",
                                GetType().Name, Name));
                    }

                    var value = reader.GetAttribute("Value");

                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Event {1}: Property element's Value attribute cannot be empty",
                                GetType().Name, Name));
                    }

                    if (_properties.ContainsKey(name))
                    {
                        throw new XmlSyntaxException(lineNumberInfo.LineNumber,
                            string.Format(
                                "{0}, Event {1}: Property name {2} has already been used",
                                GetType().Name, Name, name));
                    }

                    _properties.Add(name, value);

                    canRead = reader.Read();
                }
                else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "EventHandler")
                {
                    break;
                }
                else
                {
                    canRead = reader.Read();
                }
            }
        }

        /// <summary>
        ///     Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized. </param>
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", Name);
            writer.WriteAttributeString("Subsets", string.Join(",", _subsets));

            foreach (var param in Parameters)
            {
                writer.WriteStartElement("Parameter");
                writer.WriteAttributeString("Name", param.Name);
                writer.WriteAttributeString("Type", param.Type.ToString());
                writer.WriteEndElement();
            }

            writer.WriteStartElement("DocumentationString");
            writer.WriteString(DocumentationString);
            writer.WriteEndElement();
        }

        public new static LSLLibraryEventSignature Parse(string str)
        {
            return new LSLLibraryEventSignature(LSLEventSignature.Parse(str));
        }

        public void AddSubsets(string subsets)
        {
            _subsets.UnionWith(_subsetsRegex.ParseSubsets(subsets));
        }

        public void AddSubsets(IEnumerable<string> subsets)
        {
            _subsets.UnionWith(subsets);
        }

        public void SetSubsets(IEnumerable<string> subsets)
        {
            _subsets = new HashSet<string>(subsets);
        }

        public void SetSubsets(string subsets)
        {
            _subsets = new HashSet<string>(_subsetsRegex.ParseSubsets(subsets));
        }

        public static LSLLibraryEventSignature FromXmlFragment(XmlReader fragment)
        {
            var x = new LSLLibraryEventSignature();
            x.ReadXml(fragment);
            return x;
        }
    }
}