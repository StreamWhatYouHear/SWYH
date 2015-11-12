/*   
Copyright 2006 - 2010 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.IO;
using System.Xml;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Net.Sockets;
using OpenSource.Utilities;

namespace OpenSource.UPnP
{
    /// <summary>
    /// An interface that is used by the Code generator to encapsulate an implementation
    /// </summary>
    public interface IUPnPService
    {
        /// <summary>
        /// Gets the underlying UPnPService object
        /// </summary>
        UPnPService GetUPnPService();
    }


    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ReturnArgumentAttribute : System.Attribute
    {
        private string _name;
        public string Name
        {
            get
            {
                return (_name);
            }
        }
        public ReturnArgumentAttribute(string val)
        {
            _name = val;
        }
    }
    /// <summary>
    /// There are times when you wish to do asynchronous processing when dispatched
    /// from the UPnPService. If you call <see cref="DelayInvokeRespose"/> and give it
    /// a Unique handle, then throw this exception, then from any thread you can call 
    /// <see cref="OpenSource.UPnP.UPnPService.DelayedInvokeResponse"/> to complete the transaction.
    /// This is useful if for example you need to query other services in order to complete the action.
    /// </summary>
    public class DelayedResponseException : Exception
    {
        public DelayedResponseException()
            : base("ResponseWillReturnLater")
        {
        }
    }
    public class SCPDException : Exception
    {
        public SCPDException(String msg)
            : base(msg)
        {
        }
    }
    public class InvalidRelatedStateVariableException : SCPDException
    {
        public InvalidRelatedStateVariableException(String msg)
            : base(msg)
        {
        }
    }

    public class XMLParsingException : Exception
    {
        public int line;
        public int position;
        public XMLParsingException(string msg, int ln, int pos)
            : base(msg)
        {
            line = ln-1;
            position = pos;
        }

        public XMLParsingException(string msg, int ln, int pos, Exception ex)
            : base(msg,ex)
        {
            line = ln-1;
            position = pos;
        }

        public override string ToString()
        {
            return base.ToString() + " near line " + line.ToString() + ", position " + position.ToString();
        }


    }

    /// <summary>
    /// This exception gets thrown if you try to subscribe to a service, that you already subscribed to.
    /// </summary>
    public class UPnPAlreadySubscribedException : Exception
    {
        public UPnPAlreadySubscribedException(String msg)
            : base(msg)
        {
        }
    }

    public class UPnPTypeMismatchException : Exception
    {
        public UPnPTypeMismatchException(String msg)
            : base(msg)
        {
        }
    }
    /// <summary>
    /// This gets thrown if an exception was thrown during an Invoke attempt
    /// </summary>
    public class UPnPInvokeException : Exception
    {
        public string MethodName;
        public UPnPArgument[] Arguments;

        protected UPnPCustomException _Inner = null;
        public UPnPCustomException UPNP
        {
            get
            {
                return (_Inner);
            }
        }
        public UPnPInvokeException(string Method_Name, UPnPArgument[] Args, String msg, UPnPCustomException e)
            : base(msg)
        {
            MethodName = Method_Name;
            Arguments = Args;
            _Inner = e;
        }
        public UPnPInvokeException(string Method_Name, UPnPArgument[] Args, String msg)
            : base(msg)
        {
            MethodName = Method_Name;
            Arguments = Args;
        }
    }
    /// <summary>
    /// This gets thrown if a SOAP Fault was returned during an invoke attempt.
    /// </summary>
    public class UPnPCustomException : Exception
    {
        protected int _EC;
        protected String _ED;

        public int ErrorCode
        {
            get
            {
                return (_EC);
            }
        }
        public String ErrorDescription
        {
            get
            {
                return (_ED);
            }
        }
        public UPnPCustomException(int _ErrorCode, String _ErrorDescription, Exception innerException)
            : base(_ErrorDescription, innerException)
        {
            _EC = _ErrorCode;
            _ED = _ErrorDescription;
        }
        public UPnPCustomException(int _ErrorCode, String _ErrorDescription)
            : base(_ErrorDescription)
        {
            _EC = _ErrorCode;
            _ED = _ErrorDescription;
        }
    }

    /// <summary>
    /// A UPnPStateVariable
    /// <para>
    /// This object is the "meat" of UPnP. It is used to event the current state of a 
    /// variable to subscribed control points. This object is also used by control points to determine the 
    /// types of the parameters for a given action.
    /// </para>
    /// </summary>
    /// <exception cref="OpenSource.UPnP.UPnPStateVariable.CannotRemoveException">This gets thrown if an attempt is made to remove
    /// a UPnPState variable when something is dependent on it. Possible reasons include:
    /// <list type="bullet">
    /// <description>An action has a related state variable pointing to this</description>
    /// <description>The association list in this object, is referencing an existing action</description>
    /// </list></exception>
    /// <exception cref="OpenSource.UPnP.UPnPStateVariable.OutOfRangeException">
    /// This gets thrown if a value is passed that is out of the range specified by this object</exception>
    public class UPnPStateVariable : ICloneable
    {
        protected UPnPComplexType _ComplexType = null;
        public UPnPComplexType ComplexType
        {
            get
            {
                return (_ComplexType);
            }
        }
        public UPnPService OwningService
        {
            get
            {
                return (ParentService);
            }
        }
        /// <summary>
        /// This gets thrown if an attempt is made to remove
        /// a UPnPStateVariable when something is dependent on it. Possible reasons include:
        /// <list type="bullet">
        /// <term>An action has a related state variable pointing to this</term>
        /// <term>The association list in this object, is referencing an existing action</term>
        /// </list>
        /// </summary>
        public class CannotRemoveException : Exception
        {
            public CannotRemoveException(String msg)
                : base(msg)
            {
            }
        }

        /// <summary>
        /// This gets thrown if a value is passed that is out of the range specified by this object
        /// </summary>
        public class OutOfRangeException : Exception
        {
            public OutOfRangeException(String msg)
                : base(msg)
            {
            }
        }

        public object Clone()
        {
            return (this.MemberwiseClone());
        }

        public delegate void ModifiedHandler(UPnPStateVariable sender, object NewValue);

        /// <summary>
        /// This Event is triggered whenever the value for this UPnPStateVariable has been modified.
        /// </summary>
        public event ModifiedHandler OnModified;

        internal Object CurrentValue;
        protected Object DefValue;
        protected Object MinVal = null;
        protected Object MaxVal = null;
        protected Object StepVal = null;
        internal UPnPService ParentService = null;
        internal bool DO_VALIDATE = true;

        /// <summary>
        /// Get/Set the Minimum Value
        /// </summary>
        public Object Minimum
        {
            set { MinVal = value; }
            get { return (MinVal); }
        }

        public void Clear()
        {
            CurrentValue = null;
        }

        /// <summary>
        /// Get/Set the Maximum Value
        /// </summary>
        public Object Maximum
        {
            set { MaxVal = value; }
            get { return (MaxVal); }
        }

        /// <summary>
        /// Get/Set the Step
        /// </summary>
        public Object Step
        {
            set { StepVal = value; }
            get { return (StepVal); }
        }

        internal String VarType;
        /// <summary>
        /// Bool indicating if this UPnPStateVariable is evented
        /// </summary>
        public bool SendEvent;
        public bool MulticastEvent;
        /// <summary>
        /// Get/Set the default value for this UPnPStateVariable
        /// </summary>
        public object DefaultValue
        {
            get
            {
                return (DefValue);
            }
            set
            {
                DefValue = value;
            }
        }

        protected ArrayList AssociationList;
        protected String[] Allowed;
        protected String VariableName;
        public struct AssociationNode
        {
            public String ActionName;
            public String ArgName;
            public override bool Equals(object j)
            {
                AssociationNode an = (AssociationNode)j;
                if ((an.ActionName == ActionName) && (an.ArgName == ArgName))
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            public override int GetHashCode()
            {
                return (this.ActionName.GetHashCode());
            }
        }
        /// <summary>
        /// Get the name of this UPnPStateVariable
        /// </summary>
        public String Name
        {
            get
            {
                return (VariableName);
            }
        }

        /// <summary>
        /// Get/Set the AllowedValue list for this UPnPStateVariable as a string array.
        /// </summary>
        public string[] AllowedStringValues
        {
            get { return (Allowed); }
            set { Allowed = value; }
        }

        internal void InitialEvent()
        {
            if (SendEvent && Value != null && OnModified != null) OnModified(this, Value);
        }

        internal UPnPStateVariable(String VarName)
        {
            DefValue = null;
            CurrentValue = null;
            VariableName = VarName;
            Allowed = null;
            SendEvent = false;
            AssociationList = new ArrayList();
        }
        /// <summary>
        /// Constructs a UPnPStateVariable
        /// <para>
        /// This constructor creates a UPnPStateVariable with the given name, and assigns it the given default value.
        /// This UPnPStateVariable will be evented, unless otherwise modified.
        /// </para>
        /// </summary>
        /// <param name="VarName">Name of this StateVariable</param>
        /// <param name="VarValue">Initial Value</param>
        public UPnPStateVariable(String VarName, Object VarValue)
            : this(VarName, VarValue, null)
        {
        }
        /// <summary>
        /// Constructs a UPnPStateVariable
        /// <para>
        /// This constructor creates a UPnPStateVariable with the given name,
        /// and assigns the type, as well as sets the bool indicating if this is evented.
        /// </para>
        /// <param name="VarName">Name of this StateVariable</param>
        /// <param name="VType">Type of this StateVariable</param>
        /// <param name="SendEvents">Evented?</param>
        public UPnPStateVariable(String VarName, Type VType, bool SendEvents)
        {
            SendEvent = SendEvents;
            DefValue = null;
            CurrentValue = null;
            VariableName = VarName;
            Allowed = null;
            SendEvent = SendEvents;
            AssociationList = new ArrayList();

            VarType = ConvertToUPnPType(VType);
        }
        public UPnPStateVariable(String VarName, UPnPComplexType CT)
        {
            SendEvent = false;
            DefValue = null;
            CurrentValue = null;
            VariableName = VarName;
            Allowed = null;
            AssociationList = new ArrayList();
            VarType = "string";
            _ComplexType = CT;
        }
        /// <summary>
        /// Constructs a UPnPStateVariable
        /// <para>
        /// The UPnPStateVariable is created with the given name, assigned the given default value, and given
        /// the given AllowedValueList.
        /// </para>
        /// </summary>
        /// <param name="VarName">Name of this StateVariable</param>
        /// <param name="VarValue">Initial Value</param>
        /// <param name="AllowedValues">Allowed Values</param>
        public UPnPStateVariable(String VarName, Object VarValue, String[] AllowedValues)
        {
            DefValue = VarValue;
            CurrentValue = VarValue;
            VariableName = VarName;
            Allowed = AllowedValues;
            SendEvent = true;

            VarType = ConvertToUPnPType(VarValue.GetType());
            if (VarType == "boolean")
            {
                VarValue = VarValue.ToString().ToLower(); 
            }

            AssociationList = new ArrayList();
        }

        /// <summary>
        /// Sets the Allowed Range for this UPnPStateVariable. And of these values can be null.
        /// </summary>
        /// <param name="Min">Inclusive Minimum Value</param>
        /// <param name="Max">Inclusive Maximum Value</param>
        /// <param name="Step">Step</param>
        public void SetRange(Object Min, Object Max, Object Step)
        {
            // Check whether parameters are of correct types (same as statevariable)
            if (Min != null)
            {
                if (Min.GetType().FullName != GetNetType().FullName)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Type Checking Failed (minimum value): " + ConvertFromUPnPType(VarType).FullName +
                        " expected, not " + Min.GetType().FullName);
                    throw (new UPnPTypeMismatchException("Minimum value: " + ConvertFromUPnPType(VarType).FullName +
                        " expected, not " + Min.GetType().FullName));
                }
            }

            if (Max != null)
            {
                if (Max.GetType().FullName != GetNetType().FullName)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Type Checking Failed (maximum value): " + ConvertFromUPnPType(VarType).FullName +
                        " expected, not " + Max.GetType().FullName);
                    throw (new UPnPTypeMismatchException("Maximum value: " + ConvertFromUPnPType(VarType).FullName +
                        " expected, not " + Max.GetType().FullName));
                }
            }

            if (Step != null)
            {
                if (Step.GetType().FullName != GetNetType().FullName)
                {
                    OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Type Checking Failed (step value): " + ConvertFromUPnPType(VarType).FullName +
                        " expected, not " + Step.GetType().FullName);
                    throw (new UPnPTypeMismatchException("Step value: " + ConvertFromUPnPType(VarType).FullName +
                        " expected, not " + Step.GetType().FullName));
                }
            }

            MinVal = Min;
            MaxVal = Max;
            StepVal = Step;
        }

        internal void Validate(Object NewVal)
        {
            if (DO_VALIDATE == false) return;

            if (NewVal == null)
            {
                return;
            }
            if (NewVal.GetType().FullName != GetNetType().FullName)
            {
                OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Type Checking Failed: " + ConvertFromUPnPType(VarType).FullName +
                    " expected, not " + NewVal.GetType().FullName);
                throw (new UPnPTypeMismatchException(ConvertFromUPnPType(VarType).FullName +
                    " expected, not " + NewVal.GetType().FullName));
            }
            else
            {
                // Check Allowed Values
                if (this.AllowedStringValues != null)
                {
                    bool OK = false;
                    foreach (string x in AllowedStringValues)
                    {
                        if ((string)NewVal == x)
                        {
                            OK = true;
                            break;
                        }
                    }
                    if (OK == false)
                    {
                        OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Type Checking Failed: " + NewVal.ToString() + " NOT in allowed value list");
                        throw (new UPnPTypeMismatchException(NewVal.ToString() + " NOT in allowed value list"));
                    }
                }

                // Correct Type, Check Range
                if ((MinVal == null) && (MaxVal == null))
                {
                    return;
                }
                else
                {
                    // There is a Range Specified
                    if (MinVal != null)
                    {
                        if (((IComparable)NewVal).CompareTo(MinVal) < 0)
                        {
                            OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Type Checking Failed: " + "Specified value: " + NewVal.ToString() + " must be >= " + MinVal.ToString());
                            throw (new OutOfRangeException("Specified value: " + NewVal.ToString() + " must be >= " + MinVal.ToString()));
                        }
                    }
                    if (MaxVal != null)
                    {
                        if (((IComparable)NewVal).CompareTo(MaxVal) > 0)
                        {
                            OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error, "Type Checking Failed: " + "Specified value: " + NewVal.ToString() + " must be <= " + MaxVal.ToString());
                            throw (new OutOfRangeException("Specified value: " + NewVal.ToString() + " must be <= " + MaxVal.ToString()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the CLR type of this UPnPStateVariable
        /// </summary>
        /// <returns>The Type</returns>
        public Type GetNetType()
        {
            return (UPnPStateVariable.ConvertFromUPnPType(this.VarType));
        }
        protected Type GetTypeFromUnknown(String TypeName)
        {
            AppDomain ad = AppDomain.CurrentDomain;
            Assembly[] amy = ad.GetAssemblies();
            Module[] md;
            Type qqt = null;
            try
            {
                return (Type.GetType(TypeName, true));
            }
            catch
            { }

            for (int amyx = 0; amyx < amy.Length; ++amyx)
            {
                md = amy[amyx].GetModules();
                for (int mdx = 0; mdx < md.Length; ++mdx)
                {
                    qqt = md[mdx].GetType(TypeName);
                    if (qqt != null)
                    {
                        break;
                    }
                }
                if (qqt != null)
                {
                    break;
                }

            }
            if (qqt != null)
            {
                return (qqt);
            }
            else
            {
                throw (new Exception("Type: " + TypeName + " could not be loaded"));
            }
        }
        /// <summary>
        /// Converts UPnPTypes to CLRTypes
        /// </summary>
        /// <param name="TheType">UPnPType</param>
        /// <returns>CLRType</returns>
        public static Type ConvertFromUPnPType(string TheType)
        {
            Type RetVal;
            switch (TheType)
            {
                case "string":
                    RetVal = typeof(string);
                    break;
                case "boolean":
                    RetVal = typeof(bool);
                    break;
                case "uri":
                    RetVal = typeof(Uri);
                    break;
                case "ui1":
                    RetVal = typeof(byte);
                    break;
                case "ui2":
                    RetVal = typeof(UInt16);
                    break;
                case "ui4":
                    RetVal = typeof(UInt32);
                    break;
                case "int":
                    RetVal = typeof(Int32);
                    break;
                case "i4":
                    RetVal = typeof(Int32);
                    break;
                case "i2":
                    RetVal = typeof(Int16);
                    break;
                case "i1":
                    RetVal = typeof(SByte);
                    break;
                case "r4":
                    RetVal = typeof(Single);
                    break;
                case "r8":
                    RetVal = typeof(Double);
                    break;
                case "number":
                    RetVal = typeof(Double);
                    break;
                case "float":
                    RetVal = typeof(Single);
                    break;
                case "char":
                    RetVal = typeof(Char);
                    break;
                case "bin.base64":
                    RetVal = typeof(Byte[]);
                    break;
                case "dateTime":
                    RetVal = typeof(DateTime);
                    break;
                default:
                    RetVal = typeof(Object);
                    break;
            }
            return (RetVal);
        }
        /// <summary>
        /// Converts CLRTypes to UPnPTypes
        /// </summary>
        /// <param name="TheType">CLRType</param>
        /// <returns>UPnPType</returns>
        public static string ConvertToUPnPType(Type TheType)
        {
            string vt = "";
            string vtype = TheType.FullName;
            if (vtype.EndsWith("&") == true)
            {
                vtype = vtype.Substring(0, vtype.Length - 1);
            }
            switch (vtype)
            {
                case "System.Char":
                    vt = "char";
                    break;
                case "System.String":
                    vt = "string";
                    break;
                case "System.Boolean":
                    vt = "boolean";
                    break;
                case "System.Uri":
                    vt = "uri";
                    break;
                case "System.Byte":
                    vt = "ui1";
                    break;
                case "System.UInt16":
                    vt = "ui2";
                    break;
                case "System.UInt32":
                    vt = "ui4";
                    break;
                case "System.Int32":
                    vt = "i4";
                    break;
                case "System.Int16":
                    vt = "i2";
                    break;
                case "System.SByte":
                    vt = "ui1";
                    break;
                case "System.Single":
                    vt = "r4";
                    break;
                case "System.Double":
                    vt = "r8";
                    break;
                case "System.Byte[]":
                    vt = "bin.base64";
                    break;
                case "System.DateTime":
                    vt = "dateTime";
                    break;
                default:
                    vt = TheType.FullName;
                    break;
            }
            return (vt);
        }

        internal void BuildProperty(string prefix, string ns, XmlTextWriter XMLDoc)
        {
            if (SendEvent == false)
            {
                return;
            }

            XMLDoc.WriteStartElement(prefix, "property", ns);
            XMLDoc.WriteStartElement(Name);
            XMLDoc.WriteRaw(UPnPService.SerializeObjectInstance(Value));
            XMLDoc.WriteEndElement();
            //XMLDoc.WriteElementString(Name,UPnPService.SerializeObjectInstance(Value));
            XMLDoc.WriteEndElement();
        }
        internal string BuildProperty()
        {
            String XMLPayLoad = "";
            if (SendEvent == false)
            {
                return ("");
            }
            XMLPayLoad += "<e:property>\n";
            XMLPayLoad += "<" + this.Name + ">" + UPnPService.SerializeObjectInstance(this.Value) + "</" + this.Name + ">\n";
            XMLPayLoad += "</e:property>\n";
            return (XMLPayLoad);
        }

        /// <summary>
        /// Associate this UPnPStateVariable to an Action
        /// </summary>
        /// <param name="ActionName">Name of Action to associate with</param>
        /// <param name="ArgumentName">Nme of Argument to associate with</param>
        public void AddAssociation(String ActionName, String ArgumentName)
        {
            AssociationNode temp = new AssociationNode();
            temp.ActionName = ActionName;
            temp.ArgName = ArgumentName;
            AssociationList.Add(temp);
        }

        /// <summary>
        /// Removes an association for this UPnPStateVariable
        /// </summary>
        /// <param name="ActionName">Action to disassociate with</param>
        /// <param name="ArgumentName">Argument to disassociate with</param>
        public void RemoveAssociation(string ActionName, string ArgumentName)
        {
            AssociationNode temp = new AssociationNode();
            temp.ActionName = ActionName;
            temp.ArgName = ArgumentName;

            AssociationList.Remove(temp);
        }


        internal UPnPStateVariable.AssociationNode[] GetAssociations()
        {
            return ((AssociationNode[])AssociationList.ToArray(typeof(AssociationNode)));
        }

        public override string ToString()
        {
            return (Name);
        }
        /// <summary>
        /// Gets/Sets the current value
        /// </summary>
        public virtual Object Value
        {
            get
            {
                return (CurrentValue);
            }
            set
            {
                bool Changed = false;
                Validate(value);
                if (CurrentValue != null)
                {
                    if (CurrentValue.Equals(value) == false) Changed = true;
                }
                else
                {
                    Changed = true;
                }

                CurrentValue = value;
                if ((SendEvent == true) && (ParentService != null))
                {
                    ParentService.SendEvents(this);
                }
                if ((OnModified != null) && (Changed == true))
                {
                    OnModified(this, value);
                }
            }
        }
        /// <summary>
        /// Gets the UPnP Type
        /// </summary>
        public string ValueType
        {
            get { return (VarType); }
        }
        internal void GetStateVariableXML(XmlTextWriter XDoc)
        {
            XDoc.WriteStartElement("stateVariable");
            if (SendEvent == true)
            {
                XDoc.WriteAttributeString("sendEvents", "yes");
            }
            else
            {
                XDoc.WriteAttributeString("sendEvents", "no");
            }
            if (this.MulticastEvent)
            {
                XDoc.WriteAttributeString("multicast", "yes");
            }
            XDoc.WriteElementString("name", VariableName);
            if (this.ComplexType != null)
            {
                // References Complex Type
                XDoc.WriteStartElement("dataType");
                XDoc.WriteAttributeString("type", this.OwningService.ComplexType_NamespacePrefix[this.ComplexType.Name_NAMESPACE].ToString() + ":" + ComplexType.Name_LOCAL);
                XDoc.WriteString("string");
                XDoc.WriteEndElement();
            }
            else
            {
                // Normal Variable
                XDoc.WriteElementString("dataType", ValueType);
            }

            if (Allowed != null)
            {
                XDoc.WriteStartElement("allowedValueList");
                for (int z = 0; z < Allowed.Length; ++z)
                {
                    XDoc.WriteElementString("allowedValue", Allowed[z]);
                }
                XDoc.WriteEndElement();
            }
            if (DefValue != null)
            {
                XDoc.WriteElementString("defaultValue", UPnPService.SerializeObjectInstance(DefValue));
            }
            if ((MinVal != null) && (MaxVal != null))
            {
                XDoc.WriteStartElement("allowedValueRange");
                XDoc.WriteElementString("minimum", MinVal.ToString());
                XDoc.WriteElementString("maximum", MaxVal.ToString());
                if (StepVal != null)
                {
                    XDoc.WriteElementString("step", StepVal.ToString());
                }
                XDoc.WriteEndElement();
            }
            XDoc.WriteEndElement();
        }
    }

    /// <summary>
    /// UPnPArgument
    /// <para>
    /// This object is used to represent parameters in an action.
    /// </para>
    /// </summary>
    public class UPnPArgument : ICloneable
    {
        /// <summary>
        /// Name of the Argument
        /// </summary>
        public String Name;
        /// <summary>
        /// Direction of the Argument
        /// <para>
        /// Can be, "in" or "out"
        /// </para>
        /// </summary>
        public String Direction;
        /// <summary>
        /// Data type for argument.
        /// <para>
        /// This does not need to be set, because it will be set for you, when
        /// you assign a value to the DataValue property.
        /// </para></parA>
        /// </summary>
        public String DataType;
        /// <summary>
        /// Value to be assigned/obtained to/from this argument
        /// </summary>
        public Object DataValue;
        /// <summary>
        /// Bool indicating if this argument is/was a return value.
        /// </summary>
        public bool IsReturnValue;
        internal UPnPStateVariable __StateVariable = null;

        /// <summary>
        /// Get/Set the Related UPnPStateVariable for this argument.
        /// <para>
        /// This tree is automatically built for the Control Point, and must be
        /// assembled if building a device.
        /// </para>
        /// </summary>
        public UPnPStateVariable RelatedStateVar
        {
            set
            {
                if (value == null)
                {
                    StateVarName = null;
                }
                else
                {
                    StateVarName = value.Name;
                    __StateVariable = value;
                }
            }
            get
            {
                if (StateVarName == null)
                {
                    return (null);
                }
                else
                {
                    return (parentAction.ParentService.GetStateVariableObject(StateVarName));
                }
            }
        }
        public object Clone()
        {
            return (this.MemberwiseClone());
        }
        public override string ToString()
        {
            return (this.Name);
        }
        internal string StateVarName = null;
        internal UPnPAction parentAction = null;

        /// <summary>
        /// Returns the UPnPAction that this argument was obtained from
        /// </summary>
        public UPnPAction ParentAction
        {
            get { return parentAction; }
            set { parentAction = value; }
        }

        internal UPnPArgument()
        { }
        /// <summary>
        /// Construct a UPnPArgument
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="val">Value</param>
        public UPnPArgument(String name, Object val)
        {
            Name = name;
            DataValue = val;
            if (val != null)
            {
                DataType = val.GetType().ToString();
            }
            else
            {
                DataType = "System.Void";
            }
            IsReturnValue = false;
        }
    }

    /// <summary>
    /// UPnPAction
    /// <para>
    /// This object is used to represent the Actions represented by a UPnPService.
    /// </para>
    /// </summary>
    public class UPnPAction : ICloneable
    {
        public delegate void SpecialInvokeCase(UPnPAction sender, UPnPArgument[] InArgs, out object RetVal, out UPnPArgument[] OutArgs);
        /// <summary>
        /// This is a special delegate if assigned will override an action dispatch. This is to be used for special cases only.
        /// </summary>
        public SpecialInvokeCase SpecialCase = null;

        public String Name;
        public UPnPService ParentService = null;

        internal MethodInfo MethodPointer = null;
        protected ArrayList ArgList;
        public override String ToString()
        {
            return (this.Name);
        }
        public override bool Equals(object obj)
        {
            UPnPAction Target = (UPnPAction)obj;
            if (this.ParentService != null && Target.ParentService != null)
            {
                if (ParentService.ServiceURN == Target.ParentService.ServiceURN && Target.Name == this.Name)
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            if (this.ParentService != null && this.ParentService.ServiceURN != null &&
                this.Name != null)
            {
                string rv = this.ParentService.ServiceURN + ":" + this.Name;
                return rv.GetHashCode();
            }
            else
            {
                return this.ToString().GetHashCode();
            }
        }

        public object Clone()
        {
            UPnPAction A = (UPnPAction)this.MemberwiseClone();
            A.ArgList = new ArrayList();
            foreach (UPnPArgument arg in this.ArgList)
            {
                A.ArgList.Add(arg.Clone());
            }
            return (A);
        }
        /// <summary>
        /// Validates an argument array. This includes Number, Name, Type,Range, and allowed Value List.
        /// </summary>
        /// <param name="Args">Array of UPnPArguments to validate</param>
        /// <returns>True if OK, detailed exception otherwise</returns>
        public bool ValidateArgs(UPnPArgument[] Args)
        {
            UPnPArgument a;

            int NumShouldHave = ArgList.Count;
            if (this.HasReturnValue == true)
            {
                --NumShouldHave;
            }
            if (Args.Length != NumShouldHave)
            {
                // Incorrect # of arguments
                throw (new UPnPInvokeException(this.Name, Args, "Incorrect number of Args"));
            }

            for (int i = 0; i < Args.Length; ++i)
            {
                a = GetArg(Args[i].Name);
                if (a != null)
                {
                    if (a.Direction == "in")
                    {
                        try
                        {
                            a.RelatedStateVar.Validate(Args[i].DataValue);
                        }
                        catch (Exception sve)
                        {
                            throw (new UPnPInvokeException(this.Name, Args, sve.Message));
                        }
                    }
                }
                else
                {
                    // Arg Not Found
                    throw (new UPnPInvokeException(this.Name, Args, Args[i].Name + " was not found in action: " + this.Name));
                }
            }

            return (true);

        }
        /// <summary>
        /// Returns true if this Action has a return value, false otherwise.
        /// </summary>
        public bool HasReturnValue
        {
            get
            {
                bool RetVal = false;
                int cnt = ArgList.Count;
                for (int x = 0; x < cnt; ++x)
                {
                    if (((UPnPArgument)ArgList[x]).IsReturnValue == true)
                    {
                        RetVal = true;
                        break;
                    }
                }
                return (RetVal);
            }
        }

        /// <summary>
        /// Returns the Return argument for this action.
        /// <para>
        /// null is returned if there in none.
        /// </para>
        /// </summary>
        /// <returns></returns>
        public UPnPArgument GetRetArg()
        {
            IEnumerator en = ArgList.GetEnumerator();
            UPnPArgument RetVal = null;
            while (en.MoveNext())
            {
                if (((UPnPArgument)en.Current).IsReturnValue == true)
                {
                    RetVal = (UPnPArgument)en.Current;
                    break;
                }
            }
            return (RetVal);
        }
        /// <summary>
        /// Returns the UPnPArgument of the given name, for this action.
        /// <para>
        /// null if it doesn't exist
        /// </para>
        /// </summary>
        /// <param name="ArgName"></param>
        /// <returns></returns>
        public UPnPArgument GetArg(string ArgName)
        {
            IEnumerator en = ArgList.GetEnumerator();
            UPnPArgument RetVal = null;
            while (en.MoveNext())
            {
                if (((UPnPArgument)en.Current).Name == ArgName)
                {
                    RetVal = (UPnPArgument)en.Current;
                    break;
                }
            }
            return (RetVal);
        }
        internal void AddArgument(UPnPArgument Arg)
        {
            Arg.ParentAction = this;
            ArgList.Add(Arg);
        }
        /// <summary>
        /// Returns the Argument list for this Action.
        /// </summary>
        public UPnPArgument[] ArgumentList
        {
            get
            {
                UPnPArgument[] RetVal = new UPnPArgument[ArgList.Count];
                for (int x = 0; x < RetVal.Length; ++x)
                {
                    RetVal[x] = (UPnPArgument)ArgList[x];
                }
                return (RetVal);
            }
        }

        public IList Arguments
        {
            set
            {
                ArgList.Clear();
                foreach (UPnPArgument arg in value)
                {
                    ArgList.Add(arg);
                }
            }
            get
            {
                return (IList)ArgList;
            }
        }

        public UPnPAction()
        {
            ArgList = new ArrayList();
        }

        public UPnPAction(UPnPService parent)
        {
            ArgList = new ArrayList();
            ParentService = parent;
        }

        internal void GetXML(XmlTextWriter XDoc)
        {
            ArrayList OutList = new ArrayList();
            ArrayList InList = new ArrayList();
            UPnPArgument ThisArg;

            XDoc.WriteStartElement("action");
            XDoc.WriteElementString("name", Name);
            if ((ArgumentList.Length > 0) || (HasReturnValue == true))
            {
                XDoc.WriteStartElement("argumentList");

                OutList.Clear();
                InList.Clear();

                if (HasReturnValue == true)
                {
                    // Has Return Value
                    OutList.Add(GetRetArg());
                }

                foreach (UPnPArgument ARG in this.ArgumentList)
                {
                    if (ARG.IsReturnValue == false)
                    {
                        if (ARG.Direction == "out")
                        {
                            OutList.Add(ARG);
                        }
                        else
                        {
                            InList.Add(ARG);
                        }
                    }
                }

                // In Args First
                for (int x = 0; x < InList.Count; ++x)
                {
                    ThisArg = (UPnPArgument)InList[x];
                    XDoc.WriteStartElement("argument");
                    XDoc.WriteElementString("name", ThisArg.Name);
                    XDoc.WriteElementString("direction", ThisArg.Direction);
                    if (ThisArg.IsReturnValue == true)
                    {
                        XDoc.WriteElementString("retval", "");
                    }
                    if (ThisArg.RelatedStateVar != null)
                    {
                        XDoc.WriteElementString("relatedStateVariable", ThisArg.RelatedStateVar.Name);
                    }
                    XDoc.WriteEndElement();
                }

                // Out Args Next
                for (int x = 0; x < OutList.Count; ++x)
                {
                    ThisArg = (UPnPArgument)OutList[x];
                    XDoc.WriteStartElement("argument");
                    XDoc.WriteElementString("name", ThisArg.Name);
                    XDoc.WriteElementString("direction", ThisArg.Direction);
                    if (ThisArg.IsReturnValue == true)
                    {
                        XDoc.WriteElementString("retval", "");
                    }
                    if (ThisArg.RelatedStateVar != null)
                    {
                        XDoc.WriteElementString("relatedStateVariable", ThisArg.RelatedStateVar.Name);
                    }
                    XDoc.WriteEndElement();
                }
                XDoc.WriteEndElement();
            }
            XDoc.WriteEndElement();
        }
    }


    /// <summary>
    /// UPnPService.
    /// <para>
    /// This is the object that represents all the consumables for a UPnPDevice.
    /// </para>
    /// </summary>
    public sealed class UPnPService : ICloneable
    {
        public object User = null;
        public object User2 = null;
        private Hashtable ComplexTypeTable = Hashtable.Synchronized(new Hashtable());
        internal Hashtable ComplexType_NamespacePrefix = Hashtable.Synchronized(new Hashtable());
        internal Hashtable ComplexType_NamespaceTables = Hashtable.Synchronized(new Hashtable());
        internal Hashtable SchemaURLS = new Hashtable();

        private int ComplexType_NamespacePrefixIndex = 0;

        public UPnPComplexType GetComplexType(string ns, string localName)
        {
            return ((UPnPComplexType)ComplexTypeTable[localName + ":" + ns]);
        }
        public string[] GetSchemaNamespaces()
        {
            ArrayList a = new ArrayList();
            IDictionaryEnumerator en = ComplexType_NamespaceTables.GetEnumerator();
            while (en.MoveNext())
            {
                a.Add(en.Key.ToString());
            }
            return ((string[])a.ToArray(typeof(string)));
        }

        public void AddComplexType(UPnPComplexType t)
        {
            t.ParentService = this;
            if (!ComplexType_NamespaceTables.Contains(t.Name_NAMESPACE))
            {
                ComplexType_NamespaceTables[t.Name_NAMESPACE] = new ArrayList();
                SchemaURLS[t.Name_NAMESPACE] = "http://www.vendor.org/Schemas/Sample.xsd";
            }
            ((ArrayList)ComplexType_NamespaceTables[t.Name_NAMESPACE]).Add(t);
            ComplexTypeTable[t.Name_LOCAL + ":" + t.Name_NAMESPACE] = t;
            if (!this.ComplexType_NamespacePrefix.ContainsKey(t.Name_NAMESPACE))
            {
                ++ComplexType_NamespacePrefixIndex;
                this.ComplexType_NamespacePrefix[t.Name_NAMESPACE] = "ct" + this.ComplexType_NamespacePrefixIndex.ToString();
            }
        }
        public UPnPComplexType.Group[] GetComplexTypeList_Group()
        {
            UPnPComplexType.Group[] RetVal;
            int i = 0;

            IDictionaryEnumerator en = ComplexTypeTable.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Value.GetType() == typeof(UPnPComplexType.Group))
                {
                    ++i;
                }
            }
            RetVal = new UPnPComplexType.Group[i];
            en.Reset();
            i = 0;
            while (en.MoveNext())
            {
                if (en.Value.GetType() == typeof(UPnPComplexType.Group))
                {
                    RetVal[i] = (UPnPComplexType.Group)en.Value;
                    ++i;
                }
            }
            return (RetVal);
        }
        public UPnPComplexType[] GetComplexTypeList()
        {
            UPnPComplexType[] RetVal;
            int i = 0;

            IDictionaryEnumerator en = ComplexTypeTable.GetEnumerator();
            while (en.MoveNext())
            {
                if (en.Value.GetType() == typeof(UPnPComplexType))
                {
                    ++i;
                }
            }
            RetVal = new UPnPComplexType[i];
            en.Reset();
            i = 0;
            while (en.MoveNext())
            {
                if (en.Value.GetType() == typeof(UPnPComplexType))
                {
                    RetVal[i] = (UPnPComplexType)en.Value;
                    ++i;
                }
            }
            return (RetVal);
        }
        public void RemoveComplexType(UPnPComplexType t)
        {
            ComplexTypeTable.Remove(t.Name_LOCAL + ":" + t.Name_NAMESPACE);
            ((ArrayList)this.ComplexType_NamespaceTables[t.Name_NAMESPACE]).Remove(t);
            t.ParentService = null;
        }
        public string GetComplexSchemaForNamespace(string ns)
        {
            string RetVal;
            MemoryStream m = new MemoryStream();
            XmlTextWriter X = new XmlTextWriter(m, System.Text.Encoding.UTF8);
            ArrayList a = (ArrayList)this.ComplexType_NamespaceTables[ns];

            if (a != null)
            {
                X.WriteStartDocument();
                X.WriteStartElement("xs", "schema", "http://www.w3.org/2001/XMLSchema");
                X.WriteAttributeString("targetNamespace", ns);
                X.WriteAttributeString("xmlns", ns);
                foreach (UPnPComplexType CT in a)
                {
                    CT.GetComplexTypeSchemaPart(X);
                }

                X.WriteEndElement();
                X.WriteEndDocument();
                X.Flush();

                System.Text.UTF8Encoding U = new System.Text.UTF8Encoding();
                RetVal = U.GetString(m.GetBuffer(), 3, (int)m.Length - 3);
                X.Close();
                return (RetVal);
            }
            else
            {
                return (null);
            }
        }




        private WeakEvent PeriodicRenewFailedEvent = new WeakEvent();
        public delegate void PeriodicRenewFailedHandler(UPnPService sender);
        public event PeriodicRenewFailedHandler OnPeriodicRenewFailed
        {
            add
            {
                PeriodicRenewFailedEvent.Register(value);
            }
            remove
            {
                PeriodicRenewFailedEvent.UnRegister(value);
            }
        }

        private WeakEvent SubscriptionAddedEvent = new WeakEvent();
        private WeakEvent SubscriptionRemovedEvent = new WeakEvent();
        public delegate void OnSubscriptionHandler(UPnPService sender);
        public event OnSubscriptionHandler OnSubscriptionAdded
        {
            add
            {
                SubscriptionAddedEvent.Register(value);
            }
            remove
            {
                SubscriptionAddedEvent.UnRegister(value);
            }
        }
        public event OnSubscriptionHandler OnSubscriptionRemoved
        {
            add
            {
                SubscriptionRemovedEvent.Register(value);
            }
            remove
            {
                SubscriptionRemovedEvent.UnRegister(value);
            }
        }

        private Hashtable SubscribeRequestTable = Hashtable.Synchronized(new Hashtable());
        private Hashtable SendEventTable = Hashtable.Synchronized(new Hashtable());
        private LifeTimeMonitor.LifeTimeHandler SubscribeCycleCallback;
        public delegate void ServiceResetHandler(UPnPService sender);
        public event ServiceResetHandler OnServiceReset;
        private HTTPRequest InvocationPipeline = new HTTPRequest();

        internal bool ValidationMode
        {
            set
            {
                foreach (UPnPStateVariable v in this.GetStateVariables())
                {
                    v.DO_VALIDATE = value;
                }
            }
        }

        private Hashtable DelayedResponseTable = new Hashtable();

        public delegate void UPnPEventHandler(UPnPService sender, long SEQ);
        /// <summary>
        /// This event is triggered when a UPnPEvent is received from the device.
        /// </summary>
        public event UPnPEventHandler OnUPnPEvent;

        public delegate void UPnPEventSubscribeHandler(UPnPService sender, bool SubscribeOK);
        /// <summary>
        /// This event is triggered when a subscription request returns.
        /// </summary>
        public event UPnPEventSubscribeHandler OnSubscribe;

        public delegate void UPnPServiceInvokeHandler(UPnPService sender, String MethodName, UPnPArgument[] Args, Object ReturnValue, object Tag);
        public delegate void UPnPServiceInvokeErrorHandler(UPnPService sender, String MethodName, UPnPArgument[] Args, UPnPInvokeException e, object Tag);
        /// <summary>
        /// This event is triggered when an AsyncInvoke request successfully returns.
        /// </summary>
        public event UPnPServiceInvokeHandler OnInvokeResponse;
        /// <summary>
        /// This event is triggered when an AsyncInvoke request failed.
        /// </summary>
        public event UPnPServiceInvokeErrorHandler OnInvokeError;

        private Hashtable EventSessionTable = Hashtable.Synchronized(new Hashtable());
        static private LifeTimeMonitor SubscribeCycle = new LifeTimeMonitor();

        private WeakEvent OnSniffEvent = new WeakEvent();
        private WeakEvent OnSniffPacketEvent = new WeakEvent();
        private int SniffPacketCount = 0;
        private int SniffCount = 0;
        internal delegate void SniffPacketHandler(UPnPService sender, HTTPMessage MSG);
        internal event SniffPacketHandler OnSniffPacket
        {
            add
            {
                OnSniffPacketEvent.Register(value);
                if (Interlocked.Increment(ref SniffPacketCount) == 1)
                {
                    InvocationPipeline.OnSniffPacket += new HTTPRequest.RequestHandler(SniffPacketSink);
                    InvocationPipeline.SetSniffHandlers();
                }
            }
            remove
            {
                OnSniffPacketEvent.UnRegister(value);
                if (Interlocked.Decrement(ref SniffPacketCount) == 0)
                {
                    InvocationPipeline.OnSniffPacket -= new HTTPRequest.RequestHandler(SniffPacketSink);
                    InvocationPipeline.ReleaseSniffHandlers();
                }
            }
        }
        internal delegate void SniffHandler(byte[] Raw, int offset, int length);
        internal event SniffHandler OnSniff
        {
            add
            {
                OnSniffEvent.Register(value);
                if (Interlocked.Increment(ref this.SniffCount) == 1)
                {
                    InvocationPipeline.OnSniff += new HTTPRequest.SniffHandler(SniffSink);
                    InvocationPipeline.SetSniffHandlers();
                }
            }
            remove
            {
                OnSniffEvent.UnRegister(value);
                if (Interlocked.Decrement(ref this.SniffCount) == 0)
                {
                    InvocationPipeline.OnSniff -= new HTTPRequest.SniffHandler(SniffSink);
                    InvocationPipeline.ReleaseSniffHandlers();
                }
            }
        }

        private void SniffPacketSink(HTTPRequest sender, HTTPMessage MSG, object Tag)
        {
            OnSniffPacketEvent.Fire(this, MSG);
        }

        private object SubscribeLock = new object();
        private int SubscribeCounter = 0;

        public int Major;
        public int Minor;
        private String ServiceType;
        private String Service_ID;
        private bool StandardService;

        private void SniffSink(HTTPRequest sender, byte[] raw, int offset, int length)
        {
            OnSniffEvent.Fire(raw, offset, length);
        }

        internal void _Update(UPnPService s)
        {
            this.__controlurl = s.__controlurl;
            this.__eventurl = s.__eventurl;
            this.EventCallbackURL = s.EventCallbackURL;
            this.EventURL = s.EventURL;
            if (this.SubscribeCounter != 0)
            {
                UPnPService.SubscribeCycle.Remove(this.GetHashCode());
                this.SubscribeCounter = 0;
                this.Subscribe(CurrentTimeout, null);
            }
            if (this.OnServiceReset != null) this.OnServiceReset(this);
        }

        public object Clone()
        {
            UPnPService T = (UPnPService)this.MemberwiseClone();
            T.StateVariables = new Hashtable();

            foreach (UPnPStateVariable sv in this.GetStateVariables())
            {
                UPnPStateVariable csv = (UPnPStateVariable)sv.Clone();
                csv.ParentService = T;
                T.StateVariables.Add(sv.Name, csv);
            }

            T.RemoteMethods = new SortedList();
            foreach (UPnPAction A in this.Actions)
            {
                T.RemoteMethods[A.Name] = A.Clone();
            }
            return (T);
        }
        /// <summary>
        /// Get/Set the URN for this Service.
        /// </summary>
        public String ServiceURN
        {
            get
            {
                return (ServiceType);
            }
            set
            {
                if (value.ToUpper().StartsWith("URN:SCHEMAS-UPNP-ORG:SERVICE:") == false)
                {
                    if (value.ToUpper().StartsWith("URN:"))
                    {
                        ServiceType = value;
                        DText p = new DText();
                        p.ATTRMARK = ":";
                        p[0] = value;
                        if (Version != "1" && p[p.DCOUNT()] == "1")
                        {
                            p[p.DCOUNT()] = Version;
                            ServiceType = p[0];
                        }
                        else
                        {
                            this.SetVersion(p[p.DCOUNT()]);
                        }
                    }
                    else
                    {
                        ServiceType = "urn:schemas-upnp-org:service:" + value + ":" + Version;
                    }
                }
                else
                {
                    ServiceType = value;
                    DText p = new DText();
                    p.ATTRMARK = ":";
                    p[0] = value;
                    if (Version != "1" && p[p.DCOUNT()] == "1")
                    {
                        p[p.DCOUNT()] = Version;
                        ServiceType = p[0];
                    }
                    else
                    {
                        this.SetVersion(p[p.DCOUNT()]);
                    }
                    this.SetVersion(p[p.DCOUNT()]);
                }
            }
        }
        public string ServiceURN_Prefix
        {
            get
            {
                int len;
                DText p = new DText();
                p.ATTRMARK = ":";
                p[0] = ServiceType;
                len = p[p.DCOUNT()].Length;
                return (ServiceType.Substring(0, ServiceType.Length - len));
            }
        }
        /// <summary>
        /// Get/Set the ID for this Service.
        /// </summary>
        public String ServiceID
        {
            get
            {
                return (Service_ID);
            }
            set
            {
                if (value.ToUpper().StartsWith("URN:") == false)
                {
                    Service_ID = "urn:upnp-org:serviceId:" + value;
                }
                else
                {
                    Service_ID = value;
                }
            }
        }
        internal String SCPDURL;
        internal String ControlURL
        {
            get
            {
                if (__controlurl == null) return ("");
                int x = __controlurl.LastIndexOf("/");
                if (x == -1)
                {
                    return (__controlurl);
                }
                else
                {
                    return (__controlurl.Substring(x + 1));
                }
            }
            set
            {
                __controlurl = value;
            }
        }
        internal String EventURL
        {
            get
            {
                if (__eventurl == null) return ("");
                int x = __eventurl.LastIndexOf("/");
                if (x == -1)
                {
                    return (__eventurl);
                }
                else
                {
                    return (__eventurl.Substring(x + 1));
                }
            }
            set
            {
                __eventurl = value;
            }
        }
        internal String EventCallbackURL;

        internal string __controlurl;
        internal string __eventurl;
        internal string SCPDFile
        {
            get
            {
                int i = SCPDURL.LastIndexOf("/");
                if (i == -1)
                {
                    return (SCPDURL);
                }
                else
                {
                    return (SCPDURL.Substring(i + 1));
                }
            }
        }

        /// <summary>
        /// The UPnPDevice which contains this UPnPService.
        /// </summary>
        public UPnPDevice ParentDevice;

        private Object ServiceInstance;
        private SortedList RemoteMethods;
        private Hashtable LocalMethodList;
        private Hashtable StateVariables;
        private Hashtable SubscriberTable;
        private int EventSID;
        private object SIDLock;

        private String CurrentSID;
        private int CurrentTimeout;
        //private System.Timers.Timer RenewalTimer;

        private Hashtable VarAssociation;

        struct SubscriberInfo
        {
            public String SID;
            public String CallbackURL;
            public long Expires;
            public long SEQ;
        }
        public struct AsyncInvokeInfo
        {
            public String MethodName;
            public UPnPArgument[] Args;
            public object Tag;
            public HTTPMessage Packet;
            public UPnPService.UPnPServiceInvokeHandler InvokeCB;
            public UPnPService.UPnPServiceInvokeErrorHandler ErrorCB;
        }

        internal UPnPService(double version)
        {
            OpenSource.Utilities.InstanceTracker.Add(this);
            InvocationPipeline.OnResponse += new HTTPRequest.RequestHandler(HandleInvokeRequest);

            this.SubscribeCycleCallback = new LifeTimeMonitor.LifeTimeHandler(SubscribeCycleSink);
            SubscribeCycle.OnExpired += this.SubscribeCycleCallback;
            VarAssociation = new Hashtable();
            LocalMethodList = new Hashtable();
            RemoteMethods = new SortedList();
            SIDLock = new object();
            EventSID = 0;

            StateVariables = Hashtable.Synchronized(new Hashtable());
            SubscriberTable = Hashtable.Synchronized(new Hashtable());
            CurrentSID = "";

            if (version == 0)
            {
                Major = 1;
                Minor = 0;
            }
            else
            {
                DText TempNum = new DText();
                TempNum.ATTRMARK = ".";
                TempNum[0] = version.ToString();

                Major = int.Parse(TempNum[1]);
                Minor = 0;
                if (TempNum.DCOUNT() == 2) int.TryParse(TempNum[2], out Minor);
            }
        }

        /// <summary>
        /// Instantiate a UPnPService
        /// </summary>
        /// <param name="version">Major.Minor</param>
        /// <param name="serviceID">DeviceUnique Identifier</param>
        /// <param name="serviceType">ServiceType</param>
        /// <param name="IsStandardService">Standard Type?</param>
        /// <param name="Instance">Instance of object containing methods to expose</param>
        public UPnPService(double version, String serviceID, String serviceType, bool IsStandardService, Object Instance)
            : this(version)
        {
            StandardService = IsStandardService;
            this.ServiceInstance = Instance;
            if (serviceID == "")
            {
                ServiceID = Guid.NewGuid().ToString();
            }
            else
            {
                ServiceID = serviceID;
            }

            ServiceURN = serviceType;

            SCPDURL = "_" + ServiceID.Replace(":", "-") + "_scpd.xml";
            ControlURL = "_" + ServiceID.Replace(":", "-") + "_control";
            EventURL = "_" + ServiceID.Replace(":", "-") + "_event";
        }

        /// <summary>
        /// Constructs a new Service Object that can expose a service
        /// </summary>
        /// <param name="version">Major.Minor</param>
        /// <param name="InstanceObject">Instance of object that contains methods to expose</param>
        public UPnPService(double version, Object InstanceObject) : this(version)
        {
            ServiceInstance = InstanceObject;
        }

        private void SubscribeCycleSink(LifeTimeMonitor sender, object obj)
        {
            if ((int)obj == this.GetHashCode()) Renew();
        }

        internal void AddVirtualDirectory(string VD)
        {
            SCPDURL = VD + "/" + SCPDURL;
            __controlurl = VD + "/" + __controlurl;
            __eventurl = VD + "/" + __eventurl;
        }

        public void Dispose()
        {
            if (CurrentSID != "") UnSubscribe(null);
        }


        internal bool IsYourEvent(string MySID)
        {
            return (CurrentSID == MySID);
        }

        /// <summary>
        /// Retrieves Actions exposed by this Service
        /// </summary>
        /// <returns>Array of Actions</returns>
        public UPnPAction[] GetActions()
        {
            UPnPAction[] RetVal = new UPnPAction[RemoteMethods.Count];
            IDictionaryEnumerator en = RemoteMethods.GetEnumerator();
            int id = 0;
            while (en.MoveNext())
            {
                RetVal[id] = (UPnPAction)en.Value;
                ++id;
            }
            return (RetVal);
        }

        /// <summary>
        /// Retrieves all StateVariables in this Service
        /// </summary>
        /// <returns>Array of StateVariables</returns>
        public UPnPStateVariable[] GetStateVariables()
        {
            UPnPStateVariable[] RetVal = new UPnPStateVariable[StateVariables.Count];
            if (StateVariables.Count == 0) return RetVal;

            IDictionaryEnumerator en = StateVariables.GetEnumerator();
            int id = 0;
            while (en.MoveNext())
            {
                RetVal[id] = (UPnPStateVariable)en.Value;
                ++id;
            }
            return RetVal;
        }

        /// <summary>
        /// Returns the UPnPAction of the given name.
        /// <para>
        /// null if it doesn't exist
        /// </para>
        /// </summary>
        /// <param name="ActionName"></param>
        /// <returns></returns>
        public UPnPAction GetAction(string ActionName)
        {
            if (RemoteMethods.ContainsKey(ActionName) == false) return null;
            return ((UPnPAction)RemoteMethods[ActionName]);
        }
        private void AddAction(UPnPAction action)
        {
            action.ParentService = this;
            RemoteMethods[action.Name] = action;

            // Check State Variables
            foreach (UPnPArgument arg in action.Arguments)
            {
                if (arg.__StateVariable != null)
                {
                    if (this.GetStateVariableObject(arg.__StateVariable.Name) == null)
                    {
                        this.AddStateVariable(arg.__StateVariable);
                    }
                    arg.__StateVariable = null;
                }
            }
        }

        /// <summary>
        /// Subscribe to UPnPEvents
        /// </summary>
        /// <param name="Timeout">Subscription Cycle</param>
        public void Subscribe(int Timeout, UPnPEventSubscribeHandler CB)
        {
            bool processSubscribe = false;
            lock (SubscribeLock)
            {
                if (SubscribeCounter == 0) processSubscribe = true;
                ++SubscribeCounter;
                //OnUPnPEvent += UPnPEventCallback;
            }

            if (processSubscribe == false)
            {
                // Already subscribed... Just trigger Events
                foreach (UPnPStateVariable V in this.GetStateVariables()) V.InitialEvent();
                if (CB != null) CB(this, true);
                return;
            }

            foreach (UPnPStateVariable V in this.GetStateVariables())
            {
                if (V.SendEvent) V.CurrentValue = null;
            }

            CurrentTimeout = Timeout;

            HTTPMessage request = new HTTPMessage();
            String WebIP;
            int WebPort;
            String Tag;
            String NT;
            if (Timeout == 0)
            {
                NT = "Second-infinite";
            }
            else
            {
                NT = "Second-" + Timeout.ToString();
            }

            SSDP.ParseURL(this.__eventurl, out WebIP, out WebPort, out Tag);
            IPEndPoint dest = new IPEndPoint(IPAddress.Parse(WebIP), WebPort);
            IPAddress addr = IPAddress.Parse(WebIP);

            request.Directive = "SUBSCRIBE";
            request.DirectiveObj = Tag;
            if (addr.AddressFamily == AddressFamily.InterNetwork)
            {
                request.AddTag("Host", WebIP + ":" + WebPort.ToString());
            }
            else if (addr.AddressFamily == AddressFamily.InterNetworkV6)
            {
                request.AddTag("Host", "[" + RemoveIPv6Scope(addr.ToString()) + "]:" + WebPort.ToString());
            }
            request.AddTag("Callback", "<" + this.EventCallbackURL + ">");
            request.AddTag("NT", "upnp:event");
            request.AddTag("Timeout", NT);

            HTTPRequest SR = new HTTPRequest();
            SubscribeRequestTable[SR] = SR;
            SR.OnResponse += new HTTPRequest.RequestHandler(HandleSubscribeResponse);
            SR.PipelineRequest(dest, request, CB);
        }

        private void HandleSubscribeResponse(HTTPRequest sender, HTTPMessage response, object Tag)
        {
            UPnPEventSubscribeHandler CB = (UPnPEventSubscribeHandler)Tag;
            SubscribeRequestTable.Remove(sender);
            sender.Dispose();
            if (response != null)
            {
                if (response.StatusCode != 200)
                {
                    if (CB != null)
                    {
                        CB(this, false);
                    }
                    else
                    {
                        if (OnSubscribe != null) OnSubscribe(this, false);
                    }
                }
                else
                {
                    CurrentSID = response.GetTag("SID");
                    if (CB != null)
                    {
                        CB(this, true);
                    }
                    else
                    {
                        if (OnSubscribe != null) OnSubscribe(this, true);
                    }

                    if (CurrentTimeout != 0)
                    {
                        EventLogger.Log(this, System.Diagnostics.EventLogEntryType.SuccessAudit, "SUBSCRIBE [" + this.CurrentSID + "] Duration: " + CurrentTimeout.ToString() + " <" + DateTime.Now.ToLongTimeString() + ">");
                        SubscribeCycle.Add(this.GetHashCode(), CurrentTimeout / 2);
                    }
                }
            }
            else
            {
                if (CB != null)
                {
                    CB(this, false);
                }
                else
                {
                    if (OnSubscribe != null) OnSubscribe(this, false);
                }
            }
        }

        /// <summary>
        /// Unsubscribe to UPnPEvent
        /// </summary>
        /// <param name="cb"></param>
        public void UnSubscribe(UPnPEventHandler cb)
        {
            bool processUnSubscribe = false;

            lock (SubscribeLock)
            {
                --SubscribeCounter;
                if (SubscribeCounter <= 0)
                {
                    SubscribeCounter = 0;
                    processUnSubscribe = true;
                }
                if (cb == null)
                {
                    processUnSubscribe = true;
                    OnUPnPEvent = null;
                }
                else
                {
                    OnUPnPEvent -= cb;
                }
            }

            if (processUnSubscribe == false) return;

            HTTPMessage request = new HTTPMessage();
            String WebIP;
            int WebPort;
            String Tag;

            SSDP.ParseURL(this.__eventurl, out WebIP, out WebPort, out Tag);
            IPEndPoint dest = new IPEndPoint(IPAddress.Parse(WebIP), WebPort);

            request.Directive = "UNSUBSCRIBE";
            request.DirectiveObj = Tag;
            request.AddTag("Host", WebIP + ":" + WebPort.ToString()); // WebIP is already formatted for IPv6
            request.AddTag("SID", CurrentSID);

            HTTPRequest UR = new HTTPRequest();
            SubscribeRequestTable[UR] = UR;
            UR.OnResponse += new HTTPRequest.RequestHandler(HandleUnsubscribeResponse);
            CurrentSID = "";
            UR.PipelineRequest(dest, request, null);
        }

        private void HandleUnsubscribeResponse(HTTPRequest sender, HTTPMessage response, object Tag)
        {
            SubscribeRequestTable.Remove(sender);
            sender.Dispose();
            SubscribeCycle.Remove(this.GetHashCode());
        }

        private void Renew()
        {
            HTTPMessage request = new HTTPMessage();
            String WebIP;
            int WebPort;
            String Tag;
            String NT;

            NT = "Second-" + CurrentTimeout.ToString();

            SSDP.ParseURL(this.__eventurl, out WebIP, out WebPort, out Tag);
            IPEndPoint dest = new IPEndPoint(IPAddress.Parse(WebIP), WebPort);

            request.Directive = "SUBSCRIBE";
            request.DirectiveObj = Tag;
            request.AddTag("Host", WebIP + ":" + WebPort.ToString());
            request.AddTag("SID", CurrentSID);
            request.AddTag("Timeout", NT);

            HTTPRequest R = new HTTPRequest();
            R.OnResponse += new HTTPRequest.RequestHandler(RenewSink);
            this.SendEventTable[R] = R;
            R.PipelineRequest(dest, request, null);
        }

        private void RenewSink(HTTPRequest sender, HTTPMessage M, object Tag)
        {
            if (M != null)
            {
                if (M.StatusCode != 200)
                {
                    // Renew Failed
                    EventLogger.Log(this, System.Diagnostics.EventLogEntryType.SuccessAudit, "Renewal [" + this.CurrentSID + "] Error:" + M.StatusCode.ToString() + " <" + DateTime.Now.ToLongTimeString() + ">");

                    SubscribeCycle.Remove(this.GetHashCode());
                    this.SubscribeCounter = 0;
                    PeriodicRenewFailedEvent.Fire(this);
                }
                else
                {
                    EventLogger.Log(this, System.Diagnostics.EventLogEntryType.SuccessAudit, "Renewal [" + this.CurrentSID + "] OK <" + DateTime.Now.ToLongTimeString() + ">");
                    SubscribeCycle.Add(this.GetHashCode(), CurrentTimeout / 2);
                }
            }
            else
            {
                // Renew Failed
                EventLogger.Log(this, System.Diagnostics.EventLogEntryType.SuccessAudit, "Renewal [" + this.CurrentSID + "] DeviceError <" + DateTime.Now.ToLongTimeString() + ">");

                SubscribeCycle.Remove(this.GetHashCode());
                this.SubscribeCounter = 0;
                PeriodicRenewFailedEvent.Fire(this);
            }
            this.SendEventTable.Remove(sender);
            sender.Dispose();
        }

        private String GetNewSID()
        {
            String TheSID = "";
            lock (SIDLock)
            {
                ++EventSID;
                TheSID = "uuid:" + ParentDevice.UniqueDeviceName + "-" + ServiceID + "-" + EventSID.ToString();
            }

            return (TheSID);
        }
        internal void SetVersion(string v)
        {
            DText p = new DText();
            if (v.IndexOf("-") == -1)
            {
                p.ATTRMARK = ".";
            }
            else
            {
                p.ATTRMARK = "-";
            }
            p[0] = v;

            string mj = p[1];
            string mn = p[2];

            if (mj == "")
            {
                this.Major = 0;
            }
            else
            {
                this.Major = int.Parse(mj);
            }

            if (mn == "")
            {
                this.Minor = 0;
            }
            else
            {
                this.Minor = int.Parse(mn);
            }
        }
        static internal UPnPService Parse(String XML, int startLine)
        {
            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);
            UPnPService RetVal = new UPnPService(1);

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "service")
                {
                    if (XMLDoc.AttributeCount > 0)
                    {
                        for (int ax = 0; ax < XMLDoc.AttributeCount; ++ax)
                        {
                            XMLDoc.MoveToAttribute(ax);
                            if (XMLDoc.LocalName == "MaxVersion")
                            {
                                RetVal.SetVersion(XMLDoc.Value);
                            }
                        }
                        XMLDoc.MoveToContent();
                        XMLDoc.Read();
                    }
                    else
                    {
                        XMLDoc.Read();
                        XMLDoc.MoveToContent();
                    }
                    while (XMLDoc.LocalName != "service")
                    {
                        switch (XMLDoc.LocalName)
                        {
                            case "serviceType":
                                RetVal.ServiceURN = XMLDoc.ReadString();
                                break;
                            case "serviceId":
                                RetVal.ServiceID = XMLDoc.ReadString();
                                break;
                            case "SCPDURL":
                                RetVal.SCPDURL = XMLDoc.ReadString();
                                break;
                            case "controlURL":
                                RetVal.ControlURL = XMLDoc.ReadString();
                                break;
                            case "eventSubURL":
                                RetVal.EventURL = XMLDoc.ReadString();
                                break;
                            default:
                                break;
                        }
                        XMLDoc.Read();
                        XMLDoc.MoveToContent();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new XMLParsingException("Invalid service XML", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition, ex);
            }
            return (RetVal);
        }


        internal void _CancelEvent(String SID)
        {
            if (SubscriberTable.ContainsKey(SID))
            {
                SubscriberTable.Remove(SID);
                SubscriptionRemovedEvent.Fire(this);
            }
        }
        internal HTTPMessage _SubscribeEvent(out String SID, String CallbackURL, String Timeout)
        {
            SubscriberInfo sinfo = new SubscriberInfo();
            sinfo.SID = GetNewSID();
            sinfo.CallbackURL = CallbackURL;
            sinfo.SEQ = 1;

            int to = int.Parse(Timeout);

            if (to == 0)
            {
                sinfo.Expires = -1;
            }
            else
            {
                sinfo.Expires = DateTime.Now.AddSeconds(to).Ticks;
            }

            SubscriberTable[sinfo.SID] = sinfo;

            // Send an inital Event
            //SSDP.ParseURL(CallbackURL,out WebIP, out WebPort, out Tag);
            HTTPMessage Packet = new HTTPMessage();
            Packet.Directive = "NOTIFY";

            Packet.AddTag("Content-Type", "text/xml");
            Packet.AddTag("NT", "upnp:event");
            Packet.AddTag("NTS", "upnp:propchange");
            Packet.AddTag("SID", sinfo.SID);
            Packet.AddTag("SEQ", "0");
            Packet.AddTag("CONNECTION", "close");
            Packet.BodyBuffer = BuildEventXML();
            SID = sinfo.SID;
            SubscriptionAddedEvent.Fire(this); // Trigger event saying that a subscription was added
            return (Packet);
        }
        internal bool _RenewEvent(String SID, String Timeout)
        {
            SubscriberInfo sinfo;
            Object tmp = SubscriberTable[SID];
            if (tmp == null)
            {
                return (false);
            }
            else
            {
                sinfo = (SubscriberInfo)tmp;
            }

            int to = int.Parse(Timeout);
            if (to == 0)
            {
                sinfo.Expires = -1;
            }
            else
            {
                sinfo.Expires = DateTime.Now.AddSeconds(to).Ticks;
            }

            SubscriberTable[SID] = sinfo;
            return (true);
        }
        public static UPnPService FromSCPD(string SCPDXML)
        {
            UPnPService s = new UPnPService(1);
            s.ParseSCPD(SCPDXML, 0);
            return (s);
        }
        internal void ParseSCPD(String XML, int startLine)
        {
            bool loadSchema = false;
            string schemaUrn = "";

            if (XML == "")
            {
                return;
            }

            string evented = "no";
            string multicast = "no";
            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);

            XMLDoc.Read();
            XMLDoc.MoveToContent();

            if (XMLDoc.LocalName == "scpd")
            {
                try
                {
                    if (XMLDoc.HasAttributes)
                    {
                        // May be UPnP/1.1 SCPD
                        for (int i = 0; i < XMLDoc.AttributeCount; i++)
                        {
                            XMLDoc.MoveToAttribute(i);
                            if (XMLDoc.Prefix == "xmlns")
                            {
                                loadSchema = true;
                                schemaUrn = XMLDoc.Value;
                            }
                            // ToDo: Try to load the schema from the network first
                            //						if (XMLDoc.LocalName=="schemaLocation")
                            //						{
                            //							if (XMLDoc.Value=="http://www.vendor.org/Schemas/Sample.xsd")
                            //							{
                            //								schemaUrn = XMLDoc.LookupNamespace(XMLDoc.Prefix);
                            //							}
                            //						}
                        }
                        XMLDoc.MoveToElement();

                        if (loadSchema)
                        {
                            // Prompt Application for local Schema Location
                            System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();
                            fd.Multiselect = false;
                            fd.Title = schemaUrn;
                            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                FileStream fs = (FileStream)fd.OpenFile();
                                System.Text.UTF8Encoding U = new System.Text.UTF8Encoding();
                                byte[] buffer = new byte[(int)fs.Length];
                                fs.Read(buffer, 0, buffer.Length);
                                UPnPComplexType[] complexTypes = UPnPComplexType.Parse(U.GetString(buffer));
                                fs.Close();
                                foreach (UPnPComplexType complexType in complexTypes)
                                {
                                    this.AddComplexType(complexType);
                                }
                            }
                        }
                    }


                    XMLDoc.Read();
                    XMLDoc.MoveToContent();
                    while ((XMLDoc.LocalName != "scpd") && (XMLDoc.EOF == false))
                    {
                        if (XMLDoc.LocalName == "actionList" && !XMLDoc.IsEmptyElement)
                        {
                            XMLDoc.Read();
                            XMLDoc.MoveToContent();
                            while ((XMLDoc.LocalName != "actionList") && (XMLDoc.EOF == false))
                            {
                                if (XMLDoc.LocalName == "action")
                                {
                                    int embeddedLine = XMLDoc.LineNumber;
                                    //ParseActionXml("<action>\r\n" + XMLDoc.ReadInnerXml() + "</action>", embeddedLine);
                                    ParseActionXml(XMLDoc.ReadOuterXml(), embeddedLine-1);
                                }
                                if (!XMLDoc.IsStartElement())
                                {
                                    if (XMLDoc.LocalName != "actionList")
                                    {
                                        XMLDoc.Read();
                                        XMLDoc.MoveToContent();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (XMLDoc.LocalName == "serviceStateTable")
                            {
                                XMLDoc.Read();
                                XMLDoc.MoveToContent();

                                while ((XMLDoc.LocalName != "serviceStateTable") &&
                                    (XMLDoc.EOF == false))
                                {
                                    if (XMLDoc.LocalName == "stateVariable")
                                    {
                                        evented = "no";
                                        multicast = "no";

                                        XMLDoc.MoveToAttribute("sendEvents");
                                        if (XMLDoc.LocalName == "sendEvents")
                                        {
                                            evented = XMLDoc.GetAttribute("sendEvents");
                                        }
                                        XMLDoc.MoveToAttribute("multicast");
                                        if (XMLDoc.LocalName == "multicast")
                                        {
                                            multicast = XMLDoc.GetAttribute("multicast");
                                        }
                                        XMLDoc.MoveToContent();
                                        ParseStateVarXml(evented, multicast, XMLDoc, startLine);
                                    }
                                    if (!XMLDoc.IsStartElement())
                                    {
                                        if (XMLDoc.LocalName != "serviceStateTable")
                                        {
                                            XMLDoc.Read();
                                            XMLDoc.MoveToContent();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                XMLDoc.Skip();
                            }

                        }
                        if (!XMLDoc.IsStartElement())
                        {
                            XMLDoc.Read();
                            XMLDoc.MoveToContent();
                        }
                    }
                    // End of While

                }
                catch (XMLParsingException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw new XMLParsingException("Invalid SCPD XML", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition, ex);
                }


                // Add Associations
                foreach (UPnPAction A in this.Actions)
                {
                    foreach (UPnPArgument G in A.Arguments)
                    {
                        if (G.RelatedStateVar == null)
                        {
                            throw (new InvalidRelatedStateVariableException("Action: " + A.Name + " Arg: " + G.Name + " Contains invalid reference: " + G.StateVarName));
                        }
                        G.RelatedStateVar.AddAssociation(A.Name, G.Name);
                    }
                }
            }
            // End of If
        }
        private void ParseStateVarXml(String evented, String multicasted, XmlTextReader XMLDoc, int startLine)
        {
            //			if (XML=="")
            //			{
            //				return;
            //			}

            string ComplexType = "";
            string ComplexTypeNS = "";
            UPnPComplexType CT = null;
            DText P = new DText();
            P.ATTRMARK = ":";

            string minval = null;
            string maxval = null;
            string stepval = null;
            bool HasDef = false;

            string name = "";
            string DataType = "";
            string DefaultValue = null;
            ArrayList allowedValueList = new ArrayList();
            string LocalName = "";
            string lname2 = "";

            //			StringReader MyString = new StringReader(XML);
            //			XmlTextReader XMLDoc = new XmlTextReader(MyString);

            //			XMLDoc.Read();
            //			XMLDoc.MoveToContent();

            bool done = false;
            while (!done && XMLDoc.Read())
            {
                switch (XMLDoc.NodeType)
                {
                    case XmlNodeType.Element:
                        LocalName = XMLDoc.LocalName;
                        switch (XMLDoc.LocalName)
                        {
                            case "dataType":
                                if (XMLDoc.HasAttributes)
                                {
                                    for (int i = 0; i < XMLDoc.AttributeCount; i++)
                                    {
                                        XMLDoc.MoveToAttribute(i);
                                        if (XMLDoc.LocalName == "type")
                                        {
                                            P[0] = XMLDoc.Value;
                                            if (P.DCOUNT() == 1)
                                            {
                                                ComplexType = P[1];
                                            }
                                            else
                                            {
                                                ComplexType = P[2];
                                                ComplexTypeNS = P[1];
                                            }
                                            CT = (UPnPComplexType)ComplexTypeTable[ComplexType + ":" + XMLDoc.LookupNamespace(ComplexTypeNS)];
                                        }
                                    }
                                }
                                break;
                            case "allowedValueList":
                                bool done2 = false;
                                bool valueSet = false;
                                bool emptyValue = false;

                                while (!done2 && XMLDoc.Read())
                                {
                                    switch (XMLDoc.NodeType)
                                    {
                                        case XmlNodeType.Element:
                                            lname2 = XMLDoc.LocalName;
                                            if (lname2 == "allowedValue")
                                            {
                                                valueSet = false;
                                            }
                                            break;
                                        case XmlNodeType.EndElement:
                                            if (XMLDoc.LocalName == "allowedValue")
                                            {
                                                if (!valueSet && !emptyValue)
                                                {
                                                    emptyValue = true;
                                                    allowedValueList.Add("");
                                                }
                                            }
                                            if (XMLDoc.LocalName == "allowedValueList")
                                            {
                                                done2 = true;
                                            }
                                            break;
                                        case XmlNodeType.Text:
                                            if (lname2 == "allowedValue")
                                            {
                                                allowedValueList.Add(XMLDoc.Value);
                                                valueSet = true;
                                            }
                                            break;
                                    }
                                }
                                break;
                            case "allowedValueRange":
                                bool done3 = false;

                                while (!done3 && XMLDoc.Read())
                                {
                                    switch (XMLDoc.NodeType)
                                    {
                                        case XmlNodeType.Element:
                                            lname2 = XMLDoc.LocalName;
                                            break;
                                        case XmlNodeType.EndElement:
                                            if (XMLDoc.LocalName == "allowedValueRange")
                                            {
                                                done3 = true;
                                            }
                                            break;
                                        case XmlNodeType.Text:
                                            switch (lname2)
                                            {
                                                case "minimum":
                                                    minval = XMLDoc.Value;
                                                    break;
                                                case "maximum":
                                                    maxval = XMLDoc.Value;
                                                    break;
                                                case "step":
                                                    stepval = XMLDoc.Value;
                                                    break;
                                            }
                                            break;
                                    }
                                }
                                break;
                        }
                        break;
                    case XmlNodeType.EndElement:
                        if (XMLDoc.LocalName == "stateVariable")
                        {
                            done = true;
                        }
                        break;
                    case XmlNodeType.Text:
                        switch (LocalName)
                        {
                            case "name":
                                name = XMLDoc.Value.Trim();
                                break;
                            case "sendEventsAttribute": // Non-standard, but seems to be used widely
                                evented = XMLDoc.Value.Trim();
                                break;
                            case "dataType":
                                DataType = XMLDoc.Value.Trim();
                                break;
                            case "defaultValue":
                                DefaultValue = XMLDoc.Value;
                                HasDef = true;
                                break;
                        }
                        break;
                }
           }

            if (name == "")
            {
                throw new XMLParsingException("State Variable has no name", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition);
            }
            if (DataType == "")
            {
                throw new XMLParsingException("State Variable: \"" + name + "\" has no dataType", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition);
            }
  
            UPnPStateVariable var;
            if (CT == null)
            {
                var = new UPnPStateVariable(name);
            }
            else
            {
                var = new UPnPStateVariable(name, CT);
            }
            var.ParentService = this;
            if (evented == "yes")
            {
                var.SendEvent = true;
            }
            if (multicasted == "yes")
            {
                var.MulticastEvent = true;
            }
            var.VarType = DataType;
            if (allowedValueList.Count > 0)
            {
                var.AllowedStringValues = (string[])allowedValueList.ToArray(typeof(string));
            }

            if (HasDef)
            {
                var.DefaultValue = UPnPService.CreateObjectInstance(var.GetNetType(), DefaultValue);
            }
            if ((minval != null) && (maxval != null))
            {
                Object stepobj = null;
                if (stepval != null)
                {
                    try
                    {
                        stepobj = UPnPService.CreateObjectInstance(var.GetNetType(), stepval);
                    }
                    catch (Exception)
                    {
                        stepobj = null;
                    }
                }

                object MIN;
                object MAX;

                try
                {
                    MIN = UPnPService.CreateObjectInstance(var.GetNetType(), minval);
                }
                catch (Exception)
                {
                    FieldInfo mi = var.GetNetType().GetField("MinValue");
                    MIN = mi.GetValue(null);
                }

                try
                {
                    MAX = UPnPService.CreateObjectInstance(var.GetNetType(), maxval);
                }
                catch (Exception)
                {
                    FieldInfo mi = var.GetNetType().GetField("MaxValue");
                    MAX = mi.GetValue(null);
                }

                var.SetRange(MIN,
                    MAX,
                    stepobj);
            }

            //StateVariables.Add(name,var);
            StateVariables[name] = var; // TempPatch Only

            /*
            IDictionaryEnumerator en = RemoteMethods.GetEnumerator();
            UPnPAction a;
            while(en.MoveNext())
            {
                a = (UPnPAction)en.Value;
                for(int x =0;x<a.ArgumentList.Length;++x)
                {
                    if (a.ArgumentList[x].RelatedStateVar.Name == name)
                    {
                        a.ArgumentList[x].RelatedStateVar.VarType = DataType;
                        a.ArgumentList[x].RelatedStateVar.AllowedStringValues = var.AllowedStringValues;
                        a.ArgumentList[x].RelatedStateVar.DefaultValue = DefaultValue;
                    }
                }
            }*/
        }
        private void ParseActionXml(String XML, int startLine)
        {
            UPnPAction action = new UPnPAction();
            UPnPArgument arg;

            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();
                XMLDoc.Read();
                XMLDoc.MoveToContent();
                while (XMLDoc.LocalName != "action")
                {
                    switch (XMLDoc.LocalName)
                    {
                        case "name":
                            action.Name = XMLDoc.ReadString().Trim();
                            break;
                        case "argumentList":
                            if (XMLDoc.IsEmptyElement)
                            {
                                break;
                            }
                            XMLDoc.Read();
                            XMLDoc.MoveToContent();
                            while (XMLDoc.LocalName != "argumentList" && XMLDoc.EOF == false)
                            {
                                if (XMLDoc.LocalName == "argument")
                                {
                                    arg = new UPnPArgument();
                                    XMLDoc.Read();
                                    XMLDoc.MoveToContent();
                                    while (XMLDoc.LocalName != "argument")
                                    {
                                        switch (XMLDoc.LocalName)
                                        {
                                            case "name":
                                                arg.Name = XMLDoc.ReadString().Trim();
                                                break;
                                            case "retval":
                                                arg.IsReturnValue = true;
                                                break;
                                            case "direction":
                                                arg.Direction = XMLDoc.ReadString().Trim();
                                                break;
                                            case "relatedStateVariable":
                                                arg.StateVarName = XMLDoc.ReadString().Trim();
                                                break;
                                        }
                                        XMLDoc.Read();
                                        XMLDoc.MoveToContent();
                                    }
                                    action.AddArgument(arg);
                                    XMLDoc.Read();
                                    XMLDoc.MoveToContent();
                                }
                                else
                                {
                                    XMLDoc.Skip();
                                }
                            }
                            break;
                    }
                    // End of Switch
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();
                }
                // End of While

                AddAction(action);
            }
            catch (Exception ex)
            {
                throw new XMLParsingException("Invalid Action XML", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition, ex);
            }
        }
        public byte[] GetSCPDXml()
        {
            IDictionaryEnumerator varEnumerator = StateVariables.GetEnumerator();

            MemoryStream mstr = new MemoryStream();
            XmlTextWriter XDoc = new XmlTextWriter(mstr, System.Text.Encoding.UTF8);
            XDoc.Formatting = System.Xml.Formatting.Indented;
            XDoc.Indentation = 3;

            XDoc.WriteStartDocument();
            XDoc.WriteStartElement("scpd", "urn:schemas-upnp-org:service-1-0");

            IDictionaryEnumerator en = this.ComplexType_NamespacePrefix.GetEnumerator();
            while (en.MoveNext())
            {
                XDoc.WriteAttributeString("xmlns", en.Value.ToString(), null, en.Key.ToString());
                XDoc.WriteAttributeString(en.Value.ToString(), "schemaLocation", null, SchemaURLS[en.Key.ToString()].ToString());
            }

            XDoc.WriteStartElement("specVersion");
            DText PP = new DText();
            PP.ATTRMARK = ".";
            if (ParentDevice == null)
            {
                if (this.ComplexTypeTable.Count == 0)
                {
                    PP[0] = "1.0";
                }
                else
                {
                    PP[0] = "1.1";
                }
            }
            else
            {
                PP[0] = ParentDevice.ArchitectureVersion;
            }
            XDoc.WriteElementString("major", PP[1]);
            XDoc.WriteElementString("minor", PP[2]);
            XDoc.WriteEndElement();


            XDoc.WriteStartElement("actionList");
            foreach (UPnPAction A in this.Actions)
            {
                A.GetXML(XDoc);
            }
            XDoc.WriteEndElement();

            XDoc.WriteStartElement("serviceStateTable");
            while (varEnumerator.MoveNext())
            {
                ((UPnPStateVariable)varEnumerator.Value).GetStateVariableXML(XDoc);
            }
            XDoc.WriteEndElement();
            XDoc.WriteEndElement();
            XDoc.WriteEndDocument();
            XDoc.Flush();

            byte[] RetVal = new byte[mstr.Length - 3];
            mstr.Seek(3, SeekOrigin.Begin);
            mstr.Read(RetVal, 0, RetVal.Length);
            XDoc.Close();
            return (RetVal);
        }

        /// <summary>
        /// Retreives the XML for this Service
        /// </summary>
        /// <returns></returns>
        public void GetServiceXML(XmlTextWriter XDoc)
        {
            XDoc.WriteStartElement("service");
            XDoc.WriteElementString("serviceType", ServiceURN);
            XDoc.WriteElementString("serviceId", ServiceID);
            XDoc.WriteElementString("SCPDURL", SCPDURL);
            XDoc.WriteElementString("controlURL", __controlurl);
            XDoc.WriteElementString("eventSubURL", __eventurl);
            XDoc.WriteEndElement();
        }
        public String Version
        {
            get
            {
                if (Minor == 0)
                {
                    return (Major.ToString());
                }
                else
                {
                    return (Major.ToString() + "-" + Minor.ToString());
                }
            }
        }

        private string RemoveIPv6Scope(string addr)
        {
            int i = addr.IndexOf('%');
            if (i >= 0) addr = addr.Substring(0, i);
            return addr;
        }

        /// <summary>
        /// Invokes a method on this service with specific callbacks
        /// </summary>
        /// <param name="MethodName">Name of Method</param>
        /// <param name="InVarArr">Array of UPnPArguments</param>
        /// <param name="InvokeCallback">Callback for Success</param>
        /// <param name="ErrorCallback">Callback for Failed</param>
        /// <returns>Unique Handle identifier</returns>
        public void InvokeAsync(String MethodName, UPnPArgument[] InVarArr,
            object Tag,
            UPnPService.UPnPServiceInvokeHandler InvokeCallback,
            UPnPService.UPnPServiceInvokeErrorHandler ErrorCallback)
        {
            HTTPMessage request = new HTTPMessage();
            if (InVarArr == null)
            {
                InVarArr = new UPnPArgument[0];
            }

            UPnPAction action = (UPnPAction)RemoteMethods[MethodName];
            if (action == null)
            {
                throw (new UPnPInvokeException(MethodName, InVarArr, MethodName + " is not currently defined in this object"));
            }
            else
            {
                action.ValidateArgs(InVarArr);
            }

            String WebIP;
            String sName;
            int WebPort;

            SSDP.ParseURL(__controlurl, out WebIP, out WebPort, out sName);
            IPEndPoint dest = new IPEndPoint(IPAddress.Parse(WebIP), WebPort);

            request.Directive = "POST";
            request.DirectiveObj = sName;
            request.AddTag("Host", WebIP + ":" + WebPort);
            request.AddTag("Content-Type", "text/xml; charset=\"utf-8\"");
            request.AddTag("SoapAction", "\"" + ServiceURN + "#" + MethodName + "\"");

            MemoryStream mstream = new MemoryStream(4096);
            XmlTextWriter W = new XmlTextWriter(mstream, System.Text.Encoding.UTF8);
            W.Formatting = Formatting.Indented;
            W.Indentation = 3;

            W.WriteStartDocument();
            String S = "http://schemas.xmlsoap.org/soap/envelope/";

            W.WriteStartElement("s", "Envelope", S);
            W.WriteAttributeString("s", "encodingStyle", S, "http://schemas.xmlsoap.org/soap/encoding/");
            W.WriteStartElement("s", "Body", S);
            W.WriteStartElement("u", MethodName, ServiceURN);
            for (int ID = 0; ID < InVarArr.Length; ++ID)
            {
                if (action.GetArg(InVarArr[ID].Name).Direction == "in")
                {
                    W.WriteElementString(InVarArr[ID].Name, UPnPService.SerializeObjectInstance(InVarArr[ID].DataValue));
                }
            }
            W.WriteEndElement();
            W.WriteEndElement();
            W.WriteEndElement();
            W.WriteEndDocument();
            W.Flush();

            byte[] wbuf = new Byte[mstream.Length - 3];
            mstream.Seek(3, SeekOrigin.Begin);
            mstream.Read(wbuf, 0, wbuf.Length);
            W.Close();


            request.BodyBuffer = wbuf;
            AsyncInvokeInfo state = new AsyncInvokeInfo();
            state.Args = InVarArr;
            state.MethodName = MethodName;
            state.Packet = request;
            state.Tag = Tag;
            state.InvokeCB = InvokeCallback;
            state.ErrorCB = ErrorCallback;

            InvocationPipeline.PipelineRequest(dest, request, state);
        }
        /// <summary>
        /// Invokes a Method on this Service
        /// </summary>
        /// <param name="MethodName">Name of Method</param>
        /// <param name="InVarArr">Array of UPnPArguments</param>
        /// <returns>Unique Handle Identifier</returns>
        public void InvokeAsync(String MethodName, UPnPArgument[] InVarArr)
        {
            InvokeAsync(MethodName, InVarArr, null, null, null);
        }

        private void HandleInvokeRequest(HTTPRequest sender, HTTPMessage response, object Tag)
        {
            AsyncInvokeInfo state = (AsyncInvokeInfo)Tag;

            if (response == null)
            {
                if (state.ErrorCB != null)
                {
                    state.ErrorCB(this, state.MethodName, state.Args, new UPnPInvokeException(state.MethodName, state.Args, "Could not connect to device"), state.Tag);
                }
                else
                {
                    if (OnInvokeError != null)
                    {
                        OnInvokeError(this, state.MethodName, state.Args, new UPnPInvokeException(state.MethodName, state.Args, "Could not connect to device"), state.Tag);
                    }
                }
                return;
            }

            if (response.StatusCode == 100)
            {
                // Ignore
                return;
            }

            UPnPAction action = (UPnPAction)RemoteMethods[state.MethodName];
            if (response.StatusCode != 200)
            {
                if ((OnInvokeError != null) || (state.ErrorCB != null))
                {
                    if ((response.StatusCode == 500) && (response.BodyBuffer.Length > 0))
                    {
                        // Try to parse SOAP Encoded Error
                        UPnPCustomException ce = null;
                        try
                        {
                            ce = ParseErrorBody(response.StringBuffer,0);
                            OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.Error,
                                "UPnP Action <" + state.MethodName + "> Error [" + ce.ErrorCode.ToString() + "] " + ce.ErrorDescription);
                        }
                        catch (Exception ex)
                        {
                            ce = null;
                            OpenSource.Utilities.EventLogger.Log(ex,"HTTP Fault invoking " + state.MethodName + " : " + response.StatusData);

                        }
                        if (state.ErrorCB != null)
                        {
                            state.ErrorCB(this, action.Name, state.Args, new UPnPInvokeException(state.MethodName, state.Args, response.StatusData, ce), state.Tag);
                        }
                        else
                        {
                            if (OnInvokeError != null)
                            {
                                OnInvokeError(this, action.Name, state.Args, new UPnPInvokeException(state.MethodName, state.Args, response.StatusData, ce), state.Tag);
                            }
                        }
                    }
                    else
                    {
                        if (state.ErrorCB != null)
                        {
                            state.ErrorCB(this, action.Name, state.Args, new UPnPInvokeException(state.MethodName, state.Args, response.StatusData), state.Tag);
                        }
                        else
                        {
                            if (OnInvokeError != null)
                            {
                                OnInvokeError(this, action.Name, state.Args, new UPnPInvokeException(state.MethodName, state.Args, response.StatusData), state.Tag);
                            }
                        }
                    }
                }
                return;
            }


            //Lets Parse the data we got back
            //Now we can parse the XML Body

            StringReader MyString = new StringReader(response.StringBuffer.Trim());
            XmlTextReader XMLDoc = new XmlTextReader(MyString);

            String MethodTag = "";
            UPnPArgument VarArg;
            ArrayList VarList = new ArrayList();

            XMLDoc.Read();
            XMLDoc.MoveToContent();
            if (XMLDoc.LocalName == "Envelope")
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "Body")
                {
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();

                    MethodTag = XMLDoc.LocalName;

                    XMLDoc.Read();
                    XMLDoc.MoveToContent();
                    if (XMLDoc.LocalName != "Body")
                    {
                        while ((XMLDoc.LocalName != MethodTag) && (XMLDoc.EOF == false))
                        {
                            //							VarArg = new UPnPArgument(XMLDoc.Name,UPnPStringFormatter.UnEscapeString(XMLDoc.ReadInnerXml()));
                            VarArg = new UPnPArgument(XMLDoc.Name, XMLDoc.ReadString());

                            VarList.Add(VarArg);

                            if ((!XMLDoc.IsStartElement() && XMLDoc.LocalName != MethodTag) || XMLDoc.IsEmptyElement)
                            {
                                XMLDoc.Read();
                                XMLDoc.MoveToContent();
                            }
                        }
                    }
                }
            }

            //Finished processing the SOAP, lets build the return list
            Object RetVal = null;
            UPnPArgument[] InVarArr = state.Args;
            UPnPArgument ThisArg;
            Object[] temp = new Object[1];
            Type[] TypeParm = new Type[1];
            TypeParm[0] = Type.GetType("System.String");
            int StartIDX = 0;
            if (((UPnPAction)RemoteMethods[state.MethodName]).HasReturnValue == true)
            {
                UPnPArgument RetArg = action.GetArg(((UPnPArgument)VarList[0]).Name);
                Type RetType = RetArg.RelatedStateVar.GetNetType();
                RetVal = CreateObjectInstance(RetType,
                    (string)((UPnPArgument)VarList[0]).DataValue);
                StartIDX = 1;
            }

            for (int ID = StartIDX; ID < VarList.Count; ++ID)
            {
                for (int ix = 0; ix < InVarArr.Length; ++ix)
                {
                    if (InVarArr[ix].Name == ((UPnPArgument)VarList[ID]).Name)
                    {
                        ThisArg = action.GetArg(InVarArr[ix].Name);
                        if ((ThisArg.RelatedStateVar.GetNetType().FullName != "System.String") ||
                            (ThisArg.RelatedStateVar.GetNetType().FullName != "System.Object"))
                        {
                            InVarArr[ix].DataValue = CreateObjectInstance(ThisArg.RelatedStateVar.GetNetType(), (string)((UPnPArgument)VarList[ID]).DataValue);
                        }
                        else
                        {
                            InVarArr[ix].DataValue = ((UPnPArgument)VarList[ID]).DataValue;
                        }
                        InVarArr[ix].DataType = ThisArg.RelatedStateVar.GetNetType().FullName;
                        break;
                    }
                }
            }
            if (state.InvokeCB != null)
            {
                state.InvokeCB(this, state.MethodName, InVarArr, RetVal, state.Tag);
            }
            else
            {
                if (OnInvokeResponse != null)
                {
                    OnInvokeResponse(this, state.MethodName, InVarArr, RetVal, state.Tag);
                }
            }
        }

        public IList Actions
        {
            get
            {
                ArrayList TempList = new ArrayList();
                IEnumerator en = RemoteMethods.Values.GetEnumerator();
                while (en.MoveNext()) TempList.Add(en.Current);
                return ((IList)TempList);
            }
        }

        /// <summary>
        /// Remotely Invokes a method exposed by this Service, in a blocking fashion.
        /// </summary>
        /// <param name="MethodName">The name of the Method to Remotely Invoke</param>
        /// <param name="InVarArr">An array of UPnPArguments containing the input parameters</param>
        /// <returns>The return value</returns>
        public object InvokeSync(String MethodName, UPnPArgument[] InVarArr)
        {
            SyncInvokeAdapter adapter = new SyncInvokeAdapter();
            InvokeAsync(MethodName, InVarArr, null, adapter.InvokeHandler, adapter.InvokeErrorHandler);
            adapter.Result.WaitOne();
            if (adapter.InvokeException != null)
            {
                throw (adapter.InvokeException);
            }
            else
            {
                for (int i = 0; i < InVarArr.Length; ++i)
                {
                    InVarArr[i] = adapter.Arguments[i];
                }
                return (adapter.ReturnValue);
            }

        }

        /// <summary>
        /// Serializes an Object Instance to the UPnP Way ;)
        /// </summary>
        /// <param name="data">Object to be serialized</param>
        /// <returns>The serialized data, in string form</returns>
        public static string SerializeObjectInstance(Object data)
        {
            if (data == null) return "";

            string ObjectType = data.GetType().FullName;
            string RetVal = "";

            switch (ObjectType)
            {
                case "System.Byte[]":
                    RetVal = Base64.Encode((Byte[])data);
                    break;
                case "System.Uri":
                    RetVal = ((Uri)data).AbsoluteUri;
                    break;
                case "System.Boolean":
                    if ((bool)data == true)
                    {
                        RetVal = "1";
                    }
                    else
                    {
                        RetVal = "0";
                    }
                    break;
                case "System.DateTime":
                    System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.DateTimeFormatInfo();
                    RetVal = ((DateTime)data).ToString(dtfi.SortableDateTimePattern);
                    break;
                default:
                    RetVal = data.ToString();
                    break;
            }
            return (RetVal);
        }

        /// <summary>
        /// Create a CLR Object Instance
        /// <para>
        /// if null, will return default instance value.
        /// </para>
        /// </summary>
        /// <param name="ObjectType">Type</param>
        /// <param name="data">Serialized Data</param>
        /// <returns>CLR Object</returns>
        public static object CreateObjectInstance(Type ObjectType, string data)
        {
            object RetObj = null;
            MethodInfo mi;
            ConstructorInfo ci;
            Type[] TypeParm = new Type[1];
            TypeParm[0] = Type.GetType("System.String");
            Object[] Arg = new Object[1];

            if (data == null)
            {
                switch (ObjectType.FullName)
                {
                    case "System.Object":
                        RetObj = "";
                        break;
                    case "System.String":
                        RetObj = "";
                        break;
                    case "System.Boolean":
                        RetObj = false;
                        break;
                    case "System.Byte[]":
                        RetObj = new Byte[0];
                        break;
                    case "System.Char":
                        RetObj = (char)0;
                        break;
                    case "System.UInt16":
                        RetObj = (UInt16)0;
                        break;
                    case "System.UInt32":
                        RetObj = (UInt32)0;
                        break;
                    case "System.Int32":
                        RetObj = (Int32)0;
                        break;
                    case "System.Int16":
                        RetObj = (Int16)0;
                        break;
                    case "System.Long":
                        RetObj = (long)0;
                        break;
                    case "System.Double":
                        RetObj = (double)0;
                        break;
                    case "System.Single":
                        RetObj = (Single)0;
                        break;
                    case "System.Byte":
                        RetObj = (Byte)0;
                        break;
                    case "System.SByte":
                        RetObj = (SByte)0;
                        break;
                    case "System.DateTime":
                        RetObj = DateTime.Now;
                        break;
                    case "System.Uri":
                        RetObj = new System.Uri("http://127.0.0.1/");
                        break;
                    default:
                        mi = ObjectType.GetMethod("Parse", TypeParm);
                        if (mi != null)
                        {
                            RetObj = mi.Invoke(null, Arg);
                        }
                        else
                        {
                            ci = ObjectType.GetConstructor(TypeParm);
                            if (ci != null)
                            {
                                Arg[0] = "";
                                try
                                {
                                    RetObj = ci.Invoke(Arg);
                                }
                                catch (Exception)
                                {
                                    throw (new Exception("Could not instantiate " + ObjectType.FullName));
                                }
                            }
                            else
                            {
                                ci = ObjectType.GetConstructor(Type.EmptyTypes);
                                if (ci != null)
                                {
                                    try
                                    {
                                        RetObj = ci.Invoke(new object[0]);
                                    }
                                    catch (Exception)
                                    {
                                        throw (new Exception("Could not instantiate " + ObjectType.FullName));
                                    }
                                }
                            }
                        }
                        break;

                }
                return (RetObj);
            }


            switch (ObjectType.FullName)
            {
                case "System.Byte[]":
                    RetObj = Base64.Decode(data);
                    break;
                case "System.Uri":
                    if (data == "")
                    {
                        RetObj = null;
                    }
                    else
                    {
                        //try
                        //{
                        RetObj = new Uri(data);
                        //}
                        //catch(UriFormatException)
                        //{
                        //throw(new UPnPCustomException(799,"(Broken TestTool Fix) Invalid Args -  " + data + " is not a Uri"));
                        //}
                    }
                    break;
                case "System.String":
                    RetObj = data;
                    break;
                case "System.Object":
                    RetObj = data;
                    break;
                case "System.Boolean":
                    if ((data == "True") || (data == "true") || (data == "1"))
                    {
                        RetObj = true;
                    }
                    else
                    {
                        if ((data == "False") || (data == "false") || (data == "0"))
                        {
                            RetObj = false;
                        }
                        else
                        {
                            throw (new UPnPCustomException(402, data + " is not a valid Boolean"));
                        }
                    }
                    break;
                default:
                    Arg[0] = data;

                    mi = ObjectType.GetMethod("Parse", TypeParm);
                    if (mi != null)
                    {
                        try
                        {
                            RetObj = mi.Invoke(null, Arg);
                        }
                        catch (Exception)
                        {
                            throw (new UPnPTypeMismatchException("Invalid value: " + data));
                        }
                    }
                    else
                    {
                        ci = ObjectType.GetConstructor(TypeParm);
                        if (ci != null)
                        {
                            try
                            {
                                RetObj = ci.Invoke(Arg);
                            }
                            catch (Exception)
                            {
                                throw (new UPnPTypeMismatchException("Invalid value: " + data));
                            }
                        }
                        else
                        {
                            throw (new UPnPTypeMismatchException("Cannot instantiate " + ObjectType.FullName));
                        }
                    }

                    break;
            }
            return RetObj;
        }


        internal object InvokeLocal(String MethodName, ref ArrayList VarList)
        {
            if (MethodName == "QueryStateVariable")
            {
                UPnPArgument SArg = (UPnPArgument)VarList[0];
                UPnPStateVariable SV = (UPnPStateVariable)StateVariables[SArg.DataValue];
                if (SV != null)
                {
                    return (SV.Value);
                }
                else
                {
                    throw (new UPnPCustomException(402, "Invalid Args: " + SArg.DataValue));
                }
            }

            foreach (UPnPArgument _ARG in VarList)
            {
                UPnPStateVariable V = this.GetStateVariableObject(MethodName, _ARG.Name);
                try
                {
                    V.Validate(UPnPService.CreateObjectInstance(V.GetNetType(), (string)_ARG.DataValue));
                }
                catch (Exception ex)
                {
                    throw (new UPnPCustomException(402, "Argument [" + _ARG.Name + "] : " + ex.Message));
                }
            }

            UPnPAction A = this.GetAction(MethodName);
            if (A == null)
            {
                throw (new UPnPCustomException(401, "Invalid Action: " + MethodName));
            }

            if (A.SpecialCase != null)
            {
                UPnPArgument[] OutArgs;
                object _RetArg;
                foreach (UPnPArgument _arg in VarList)
                {
                    _arg.DataValue = UPnPService.CreateObjectInstance(A.GetArg(_arg.Name).RelatedStateVar.GetNetType(), (string)_arg.DataValue);
                }
                A.SpecialCase(A, (UPnPArgument[])VarList.ToArray(typeof(UPnPArgument)), out _RetArg, out OutArgs);
                VarList.Clear();
                foreach (UPnPArgument _A in OutArgs)
                {
                    VarList.Add(_A);
                }
                if (A.HasReturnValue)
                {
                    UPnPArgument retArg = new UPnPArgument(A.GetRetArg().Name, _RetArg);
                    retArg.IsReturnValue = true;
                    _RetArg = retArg;
                }
                return (_RetArg);
            }


            MethodInfo x = A.MethodPointer;

            ParameterInfo[] pInfo = x.GetParameters();
            Object[] InVarArr = new Object[pInfo.Length];
            Type TheType;
            String TypeString;

            for (int init = 0; init < InVarArr.Length; ++init)
            {
                if ((pInfo[init].ParameterType.ToString() != "System.String") &&
                    (pInfo[init].ParameterType.ToString() != "System.String&"))
                {
                    TypeString = pInfo[init].ParameterType.ToString();
                    if (TypeString.EndsWith("&") == true)
                    {
                        TypeString = TypeString.Substring(0, TypeString.Length - 1);
                    }

                    TheType = GetTypeFromUnknown(TypeString);
                    InVarArr[init] = CreateObjectInstance(TheType, null);
                }
                else
                {
                    InVarArr[init] = "";
                }
            }

            for (int id = 0; id < VarList.Count; ++id)
            {
                for (int id2 = 0; id2 < pInfo.Length; ++id2)
                {
                    if (pInfo[id2].Name == ((UPnPArgument)VarList[id]).Name)
                    {
                        if ((pInfo[id2].ParameterType.ToString() != "System.String") &&
                            (pInfo[id2].ParameterType.ToString() != "System.String&"))
                        {

                            InVarArr[id2] = CreateObjectInstance(pInfo[id2].ParameterType, (string)((UPnPArgument)VarList[id]).DataValue);
                        }
                        else
                        {
                            InVarArr[id2] = ((UPnPArgument)VarList[id]).DataValue;
                        }
                        break;
                    }
                }
            }

            Object RetVal = null;
            RetVal = x.Invoke(ServiceInstance, InVarArr);

            VarList.Clear();
            for (int id = 0; id < InVarArr.Length; ++id)
            {
                if (pInfo[id].Attributes == ParameterAttributes.Out)
                {
                    VarList.Add(new UPnPArgument(pInfo[id].Name, InVarArr[id]));
                }
            }
            if (A.HasReturnValue)
            {
                UPnPArgument retArg = new UPnPArgument(A.GetRetArg().Name, RetVal);
                retArg.IsReturnValue = true;
                RetVal = retArg;
            }
            return RetVal;
        }

        /// <summary>
        /// Adds a UPnPAction to this service
        /// </summary>
        /// <param name="action">Action to add</param>
        public void AddMethod(UPnPAction action)
        {
            if (action.Name == null || action.Name.Length == 0) throw new Exception("Invalid action name");
            action.ParentService = this;
            this.AddAction(action);
        }

        /// <summary>
        /// Remove a UPnPAction, by name
        /// </summary>
        /// <param name="MethodName">Name of Action</param>
        public void RemoveMethod(string MethodName)
        {
            UPnPAction A = new UPnPAction();
            A.Name = MethodName;
            RemoveMethod(A);
        }
        /// <summary>
        /// Remove a UPnPAction, using name of object
        /// </summary>
        /// <param name="action">Action to remove</param>
        public void RemoveMethod(UPnPAction action)
        {
            UPnPAction A = GetAction(action.Name);
            if (A == null) return;

            foreach (UPnPArgument arg in A.ArgumentList)
            {
                arg.RelatedStateVar.RemoveAssociation(A.Name, arg.Name);
                if (arg.RelatedStateVar.GetAssociations().Length == 0) RemoveStateVariable(arg.RelatedStateVar);
            }
            RemoteMethods.Remove(A.Name);
        }

        /// <summary>
        /// Add a method to expose in this service
        /// </summary>
        /// <param name="MethodName">The name of the method to expose</param>
        public void AddMethod(String MethodName)
        {
            string retname = "_ReturnValue";
            UPnPStateVariable[] ESV;
            bool DontCreate = false;
            ESV = this.GetStateVariables();
            UPnPStateVariable sv;
            MethodInfo minfo = ServiceInstance.GetType().GetMethod(MethodName);
            if (minfo == null)
            {
                throw (new Exception(MethodName + " does not exist in " + ServiceInstance.GetType().ToString()));
            }

            // Create Generic State Variables
            DontCreate = false;
            if (minfo.ReturnType.FullName != "System.Void")
            {
                if (minfo.GetCustomAttributes(true).Length > 0)
                {
                    foreach (Attribute a in minfo.GetCustomAttributes(true))
                    {
                        if (a.GetType() == typeof(OpenSource.UPnP.ReturnArgumentAttribute))
                        {
                            retname = ((ReturnArgumentAttribute)a).Name;
                            break;
                        }
                    }
                }

                // There is a return value
                sv = new UPnPStateVariable("A_ARG_TYPE_" + MethodName + "_RetType", minfo.ReturnType, false);
                sv.AddAssociation(MethodName, retname);

                foreach (UPnPStateVariable _ESV in ESV)
                {
                    foreach (UPnPStateVariable.AssociationNode AESV in _ESV.GetAssociations())
                    {
                        if ((AESV.ActionName == MethodName) && (AESV.ArgName == retname))
                        {
                            // Don't create state variable
                            DontCreate = true;
                        }
                    }
                }

                if (DontCreate == false) this.AddStateVariable(sv);
            }

            ParameterInfo[] pinfo = minfo.GetParameters();
            for (int x = 0; x < pinfo.Length; ++x)
            {
                sv = new UPnPStateVariable("A_ARG_TYPE_" + MethodName + "_" + pinfo[x].Name, pinfo[x].ParameterType, false);
                sv.AddAssociation(MethodName, pinfo[x].Name);
                DontCreate = false;
                foreach (UPnPStateVariable _ESV in ESV)
                {
                    foreach (UPnPStateVariable.AssociationNode AESV in _ESV.GetAssociations())
                    {
                        if ((AESV.ActionName == MethodName) && (AESV.ArgName == pinfo[x].Name))
                        {
                            // Don't create state variable
                            DontCreate = true;
                        }
                    }
                }
                if (DontCreate == false) this.AddStateVariable(sv);
            }
            UPnPAction NewAction = new UPnPAction();
            NewAction.Name = MethodName;
            NewAction.ParentService = this;
            NewAction.MethodPointer = minfo;
            UPnPArgument ARG;

            if (minfo.ReturnType.FullName != "System.Void")
            {
                ARG = new UPnPArgument(retname, "");
                ARG.DataType = UPnPStateVariable.ConvertToUPnPType(minfo.ReturnType);
                ARG.Direction = "out";
                ARG.IsReturnValue = true;
                ARG.ParentAction = NewAction;
                ARG.StateVarName = this.GetStateVariableObject(MethodName, retname).Name;
                NewAction.AddArgument(ARG);
            }
            foreach (ParameterInfo p in pinfo)
            {
                ARG = new UPnPArgument(p.Name, "");
                ARG.DataType = UPnPStateVariable.ConvertToUPnPType(p.ParameterType);

                ARG.Direction = p.Attributes == ParameterAttributes.Out ? "out" : "in";

                ARG.IsReturnValue = false;
                ARG.ParentAction = NewAction;
                ARG.StateVarName = this.GetStateVariableObject(MethodName, p.Name).Name;
                NewAction.AddArgument(ARG);
            }
            this.AddAction(NewAction);
        }
        /// <summary>
        /// Changes the state of a StateVariable, and sends events if neccessary.
        /// </summary>
        /// <param name="VarName">The name of the variable to set</param>
        /// <param name="VarValue">The new value</param>
        public void SetStateVariable(String VarName, Object VarValue)
        {
            // Check upon existence of variable, instead of null-reference exception
            if (StateVariables.ContainsKey(VarName))
            {
                ((UPnPStateVariable)StateVariables[VarName]).Value = VarValue;
            }
            else
            {
                throw (new Exception(string.Format("Service '{0}' does not contain variable '{1}'", this.ServiceID, VarName)));
            }
        }

        private UPnPStateVariable GetAssociatedVar(String ActionName, String Arg)
        {
            Hashtable ArgTable = (Hashtable)VarAssociation[ActionName];
            if (ArgTable == null) return null;
            return ((UPnPStateVariable)ArgTable[Arg]);
        }
        /// <summary>
        /// Add a UPnPStateVariable
        /// </summary>
        /// <param name="NewVar"></param>
        public void AddStateVariable(UPnPStateVariable NewVar)
        {
            NewVar.ParentService = this;
            StateVariables[NewVar.Name] = NewVar;
            UPnPStateVariable.AssociationNode[] Nodes;

            Hashtable ArgTable;

            Nodes = NewVar.GetAssociations();
            for (int x = 0; x < Nodes.Length; ++x)
            {
                ArgTable = (Hashtable)VarAssociation[Nodes[x].ActionName];
                if (ArgTable == null) ArgTable = new Hashtable();

                if (ArgTable.ContainsKey(Nodes[x].ArgName) == true)
                {
                    StateVariables.Remove(((UPnPStateVariable)ArgTable[Nodes[x].ArgName]).Name);
                }

                ArgTable[Nodes[x].ArgName] = NewVar;
                VarAssociation[Nodes[x].ActionName] = ArgTable;

                // THIS: Updated to throw meaningfull exception instead of null reference
                UPnPAction A = GetAction(Nodes[x].ActionName);
                if (A != null)
                {
                    UPnPArgument Argument = A.GetArg(Nodes[x].ArgName);
                    if (Argument == null)
                    {
                        throw (new Exception(string.Format("There is no parameter named {0} for the {1}.{2} method.", Nodes[x].ArgName, ServiceInstance.GetType().ToString(), Nodes[x].ActionName)));
                    }
                    Argument.StateVarName = NewVar.Name;
                }
                // THIS: End update
            }

        }

        /// <summary>
        /// Remove a UPnPStateVariable
        /// <para>
        /// An exception will be thrown if you try to remove a UPnPStateVariable that is associated with an action,
        /// or a UPnPStateVariable that is referenced by an Action.
        /// </para>
        /// </summary>
        /// <param name="stateVariable">UPnPStateVariable to remove</param>
        public void RemoveStateVariable(UPnPStateVariable stateVariable)
        {
            foreach (UPnPStateVariable.AssociationNode n in stateVariable.GetAssociations())
            {
                try
                {
                    if (GetAction(n.ActionName).GetArg(n.ArgName).StateVarName == stateVariable.Name)
                    {
                        throw (new UPnPStateVariable.CannotRemoveException("Associated with " + n.ActionName + ":" + n.ArgName));
                    }
                }
                catch (System.NullReferenceException)
                { }

            }

            StateVariables.Remove(stateVariable.Name);
        }

        /// <summary>
        /// Get a UPnPStateVariable associated with a given Action/Argument.
        /// </summary>
        /// <param name="MethodName">Action</param>
        /// <param name="ArgName">Argument</param>
        /// <returns></returns>
        public UPnPStateVariable GetStateVariableObject(String MethodName, String ArgName)
        {
            UPnPStateVariable[] x = this.GetStateVariables();
            UPnPStateVariable RetVal = null;
            foreach (UPnPStateVariable SV in x)
            {
                foreach (UPnPStateVariable.AssociationNode n in SV.GetAssociations())
                {
                    if ((n.ActionName == MethodName) && (n.ArgName == ArgName))
                    {
                        RetVal = SV;
                        break;
                    }
                }
                if (RetVal != null)
                {
                    break;
                }
            }
            return (RetVal);
        }
        /// <summary>
        /// Get UPnPStateVariable by name
        /// </summary>
        /// <param name="VarName">Name</param>
        /// <returns></returns>
        public UPnPStateVariable GetStateVariableObject(String VarName)
        {
            Object r = StateVariables[VarName];
            if (r == null)
            {
                return (null);
            }
            else
            {
                return ((UPnPStateVariable)r);
            }
        }

        /// <summary>
        /// Returns the current value of a StateVariable
        /// </summary>
        /// <param name="VarName">The name of the StateVariable</param>
        /// <returns>Current Value</returns>
        public Object GetStateVariable(String VarName)
        {
            Object SV = StateVariables[VarName];
            if (SV == null)
            {
                return (null);
            }
            else
            {
                return (((UPnPStateVariable)SV).Value);
            }
        }

        private byte[] BuildEventXML()
        {
            return (BuildEventXML(this.GetStateVariables()));
        }

        private byte[] BuildEventXML(UPnPStateVariable[] vars)
        {
            MemoryStream ms = new MemoryStream();
            XmlTextWriter XMLDoc = new XmlTextWriter(ms, System.Text.Encoding.UTF8);

            XMLDoc.Formatting = System.Xml.Formatting.Indented;
            XMLDoc.Indentation = 3;

            string prefix = "e";
            string ns = "urn:schemas-upnp-org:event-1-0";

            XMLDoc.WriteStartDocument();
            XMLDoc.WriteStartElement(prefix, "propertyset", ns);

            foreach (UPnPStateVariable SVar in vars)
            {
                SVar.BuildProperty(prefix, ns, XMLDoc);
            }
            XMLDoc.WriteEndElement();
            XMLDoc.WriteEndDocument();
            XMLDoc.Flush();

            byte[] RetVal = new byte[ms.Length - 3];
            ms.Seek(3, SeekOrigin.Begin);
            ms.Read(RetVal, 0, RetVal.Length);
            XMLDoc.Close();

            return (RetVal);
        }
        private void ParseEvents(String XML, int startLine)
        {
            StringReader MyString = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(MyString);
            Hashtable TheEvents = new Hashtable();

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();
                if (XMLDoc.LocalName == "propertyset")
                {
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();

                    while ((XMLDoc.LocalName != "propertyset") && (XMLDoc.EOF == false))
                    {
                        if (XMLDoc.LocalName == "property")
                        {
                            XMLDoc.Read();
                            XMLDoc.MoveToContent();
                        }

                        String name = XMLDoc.LocalName;
                        string val = XMLDoc.ReadString();
                        //string val = UPnPStringFormatter.PartialEscapeString(XMLDoc.ReadInnerXml());

                        if (StateVariables.ContainsKey(name) == true)
                        {
                            UPnPStateVariable var = (UPnPStateVariable)StateVariables[name];
                            try
                            {
                                Object RetVal = UPnPService.CreateObjectInstance(var.GetNetType(), val);
                                var.Value = RetVal;
                                OpenSource.Utilities.EventLogger.Log(this, System.Diagnostics.EventLogEntryType.SuccessAudit, RetVal.ToString());
                                StateVariables[name] = var;
                            }
                            catch (Exception eve)
                            {
                                OpenSource.Utilities.EventLogger.Log(eve);
                            }
                        }
                        XMLDoc.Read();
                        XMLDoc.MoveToContent();

                        if (XMLDoc.LocalName == "property" && !XMLDoc.IsStartElement())
                        {
                            XMLDoc.Read();
                            XMLDoc.MoveToContent();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new XMLParsingException("Invalid Event XML", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition, ex);
            }
            return;
        }

        private Uri[] ParseEventURL(String URLList)
        {
            DText parser = new DText();
            String temp;
            ArrayList TList = new ArrayList();

            parser.ATTRMARK = ">";
            parser[0] = URLList;

            int cnt = parser.DCOUNT();
            for (int x = 1; x <= cnt; ++x)
            {
                temp = parser[x];
                try
                {
                    temp = temp.Substring(temp.IndexOf("<") + 1);
                    TList.Add(new Uri(temp));
                }
                catch (Exception) { }
            }
            Uri[] RetVal = new Uri[TList.Count];
            for (int x = 0; x < RetVal.Length; ++x) RetVal[x] = (Uri)TList[x];
            return RetVal;
        }

        internal void SendEvents(UPnPStateVariable V)
        {
            //IDictionaryEnumerator en = SubscriberTable.GetEnumerator();
            SubscriberInfo sinfo;
            HTTPMessage Packet;
            Uri[] EventURLS;
            ICollection en = SubscriberTable.Keys;
            String[] KEYS = new String[en.Count];
            HTTPRequest R;
            en.CopyTo(KEYS, 0);

            for (int keyid = 0; keyid < KEYS.Length; ++keyid)
            {
                object _tobj = SubscriberTable[KEYS[keyid]];
                if (_tobj != null)
                {
                    sinfo = (SubscriberInfo)_tobj;
                    if ((sinfo.Expires > DateTime.Now.Ticks) || (sinfo.Expires == -1))
                    {
                        EventURLS = ParseEventURL(sinfo.CallbackURL);
                        for (int x = 0; x < EventURLS.Length; ++x)
                        {
                            try
                            {
                                IPAddress dest = IPAddress.Parse(EventURLS[x].Host);

                                Packet = new HTTPMessage();
                                Packet.Directive = "NOTIFY";
                                Packet.AddTag("Content-Type", "text/xml");
                                Packet.AddTag("NT", "upnp:event");
                                Packet.AddTag("NTS", "upnp:propchange");
                                Packet.BodyBuffer = BuildEventXML(new UPnPStateVariable[1] { V });
                                Packet.DirectiveObj = HTTPMessage.UnEscapeString(EventURLS[x].PathAndQuery);
                                if (dest.AddressFamily == AddressFamily.InterNetwork) Packet.AddTag("Host", EventURLS[x].Host + ":" + EventURLS[x].Port.ToString());
                                if (dest.AddressFamily == AddressFamily.InterNetworkV6) Packet.AddTag("Host", "[" + RemoveIPv6Scope(EventURLS[x].Host) + "]:" + EventURLS[x].Port.ToString());
                                Packet.AddTag("SID", sinfo.SID);
                                Packet.AddTag("SEQ", sinfo.SEQ.ToString());
                                Packet.AddTag("CONNECTION", "close");
                                ++sinfo.SEQ;
                                SubscriberTable[KEYS[keyid]] = sinfo;

                                //OpenSource.Utilities.EventLogger.Log(this,System.Diagnostics.EventLogEntryType.SuccessAudit,Packet.StringBuffer);

                                R = new HTTPRequest();
                                SendEventTable[R] = R;
                                R.OnResponse += new HTTPRequest.RequestHandler(HandleSendEvent);
                                R.PipelineRequest(new IPEndPoint(IPAddress.Parse(EventURLS[x].Host), EventURLS[x].Port), Packet, null);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else
                    {
                        SubscriberTable.Remove(sinfo.SID);
                    }
                }
            }

        }

        private void HandleSendEvent(HTTPRequest R, HTTPMessage M, object Tag)
        {
            R.Dispose();
            SendEventTable.Remove(R);
        }

        private Type GetTypeFromUnknown(String TypeName)
        {
            AppDomain ad = AppDomain.CurrentDomain;
            Assembly[] amy = ad.GetAssemblies();
            Module[] md;
            Type qqt = null;
            try
            {
                return (Type.GetType(TypeName, true));
            }
            catch
            { }

            for (int amyx = 0; amyx < amy.Length; ++amyx)
            {
                md = amy[amyx].GetModules();
                for (int mdx = 0; mdx < md.Length; ++mdx)
                {
                    qqt = md[mdx].GetType(TypeName);
                    if (qqt != null)
                    {
                        break;
                    }
                }
                if (qqt != null)
                {
                    break;
                }

            }
            if (qqt != null)
            {
                return (qqt);
            }
            else
            {
                throw (new Exception("Type: " + TypeName + " could not be loaded"));
            }
        }

        internal void _TriggerEvent(String SID, long SEQ, String XML, int startLine)
        {
            ParseEvents(XML,startLine);
            if (OnUPnPEvent != null) OnUPnPEvent(this, SEQ);
        }

        /// <summary>
        /// Gets the IPEndPoint of the ControlPoint which called
        /// </summary>
        /// <returns></returns>
        public IPEndPoint GetCaller()
        {
            UPnPDevice.InvokerInfoStruct iis = (UPnPDevice.InvokerInfoStruct)this.ParentDevice.InvokerInfo[Thread.CurrentThread.GetHashCode()];
            return (iis.WebSession.Remote);
        }

        private HTTPSession GetWebSession()
        {
            UPnPDevice.InvokerInfoStruct iis = (UPnPDevice.InvokerInfoStruct)this.ParentDevice.InvokerInfo[Thread.CurrentThread.GetHashCode()];
            return (iis.WebSession);
        }

        /// <summary>
        /// Gets the IPEndPoint of the Interface that recieved the invocation request
        /// </summary>
        /// <returns></returns>
        public IPEndPoint GetReceiver()
        {
            UPnPDevice.InvokerInfoStruct iis = (UPnPDevice.InvokerInfoStruct)this.ParentDevice.InvokerInfo[Thread.CurrentThread.GetHashCode()];
            return (iis.WebSession.Source);
        }

        /// <summary>
        /// This will allow you to process a request, in an Async fashion.
        /// <para>
        /// Call this method from the dispatch thread, and specify a unique ID. Use this ID to call
        /// DelayedInvokeResponse.
        /// </para>
        /// </summary>
        /// <param name="ID">Unique ID</param>
        /// <param name="OutArgs">Array of Args that you will need to assign in your callback</param>
        public void DelayInvokeRespose(long ID, out UPnPArgument[] OutArgs)
        {
            UPnPDevice.InvokerInfoStruct iis = (UPnPDevice.InvokerInfoStruct)ParentDevice.InvokerInfo[Thread.CurrentThread.GetHashCode()];
            OutArgs = iis.OutArgs;
            lock (DelayedResponseTable)
            {
                DelayedResponseTable[ID] = iis;
            }
        }
        /// <summary>
        /// This is used to complete an AsyncRequest in an AsyncFashion.
        /// <para>
        /// You must either assign all the Argument values, or return an exception with "e"
        /// </para>
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="RetArg">Return Argument</param>
        /// <param name="OutArgs">Out Arguments</param>
        /// <param name="e">Exception</param>
        public void DelayedInvokeResponse(long id, object RetArg, UPnPArgument[] OutArgs, UPnPCustomException e)
        {
            UPnPDevice.InvokerInfoStruct iis = (UPnPDevice.InvokerInfoStruct)DelayedResponseTable[id];
            HTTPMessage resp = new HTTPMessage();
            if (e != null)
            {
                resp.StatusCode = 500;
                resp.StatusData = "Internal";
                resp.StringBuffer = ParentDevice.BuildErrorBody(e);
            }
            else
            {
                resp = ParentDevice.ParseInvokeResponse(iis.MethodName, iis.SOAPAction, this.ServiceURN, RetArg, OutArgs);
            }
            iis.WebSession.Send(resp);
            iis.WebSession.StartReading();
            lock (DelayedResponseTable)
            {
                DelayedResponseTable.Remove(id);
            }
        }

        private UPnPCustomException ParseErrorBody(String XML, int startLine)
        {
            StringReader sr = new StringReader(XML);
            XmlTextReader XMLDoc = new XmlTextReader(sr);
            UPnPCustomException RetVal = null;

            int ec = 0;
            String ed = "";

            try
            {
                XMLDoc.Read();
                XMLDoc.MoveToContent();

                if (XMLDoc.LocalName == "Envelope")
                {
                    XMLDoc.Read();
                    XMLDoc.MoveToContent();
                    while ((XMLDoc.LocalName != "Envelope") && (XMLDoc.EOF == false))
                    {
                        switch (XMLDoc.LocalName)
                        {
                            case "Body":
                                XMLDoc.Read();
                                XMLDoc.MoveToContent();
                                while ((XMLDoc.LocalName != "Body") && (XMLDoc.EOF == false))
                                {
                                    if (XMLDoc.LocalName == "Fault")
                                    {
                                        XMLDoc.Read();
                                        XMLDoc.MoveToContent();
                                        while ((XMLDoc.LocalName != "Fault") && (XMLDoc.EOF == false))
                                        {
                                            switch (XMLDoc.LocalName)
                                            {
                                                case "detail":
                                                    XMLDoc.Read();
                                                    XMLDoc.MoveToContent();
                                                    while ((XMLDoc.LocalName != "detail") && (XMLDoc.EOF == false))
                                                    {
                                                        if (XMLDoc.LocalName == "UPnPError")
                                                        {
                                                            XMLDoc.Read();
                                                            XMLDoc.MoveToContent();
                                                            while ((XMLDoc.LocalName != "UPnPError") && (XMLDoc.EOF == false))
                                                            {
                                                                switch (XMLDoc.LocalName)
                                                                {
                                                                    case "errorCode":
                                                                        ec = int.Parse(XMLDoc.ReadString());
                                                                        break;
                                                                    case "errorDescription":
                                                                        ed = XMLDoc.ReadString();
                                                                        break;
                                                                }
                                                                XMLDoc.Read();
                                                                XMLDoc.MoveToContent();
                                                            }

                                                            RetVal = new UPnPCustomException(ec, ed);
                                                        }
                                                        else
                                                        {
                                                            XMLDoc.Skip();
                                                        }
                                                        XMLDoc.Read();
                                                        XMLDoc.MoveToContent();
                                                    }
                                                    break;
                                                default:
                                                    XMLDoc.Skip();
                                                    break;
                                            }
                                            XMLDoc.Read();
                                            XMLDoc.MoveToContent();
                                        }

                                    }
                                    else
                                    {
                                        XMLDoc.Skip();
                                    }
                                    XMLDoc.Read();
                                    XMLDoc.MoveToContent();
                                }
                                break;
                            default:
                                XMLDoc.Skip();
                                break;
                        }
                        XMLDoc.Read();
                        XMLDoc.MoveToContent();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new XMLParsingException("Invalid Error XML", startLine + XMLDoc.LineNumber, XMLDoc.LinePosition, ex);
            }
            return (RetVal);
        }
    }
}
