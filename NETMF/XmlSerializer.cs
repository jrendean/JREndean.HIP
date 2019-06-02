//-----------------------------------------------------------------------
// <copyright file="XmlSerializer.cs" company="JR Endean">
//     Written by JR Endean, jrendean.com and on the GHI forums, http://www.tinyclr.com/user/6257
//     with help from dobova (http://www.tinyclr.com/user/6270)
//     Released on the GHI Codeshare (http://www.tinyclr.com/codeshare)
//     http://www.apache.org/licenses/LICENSE-2.0.txt
// </copyright>
//-----------------------------------------------------------------------

namespace System.Xml.Serialization
{
    using System;
    using System.Collections;
    using System.Ext.Xml;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// An implementation of System.Xml.Serialization.XmlSerializer from desktop .NET to work on NETMF.
    /// </summary>
    public class XmlSerializer
    {
        private readonly Type typeToSerialize;

        /// <summary>
        /// Initializes a new instance of the System.Xml.Serialization.XmlSerializer class that can serialize objects of the specified type into XML documents, and deserialize XML documents into objects of the specified type.
        /// </summary>
        /// <param name="type">The type of the object that this System.Xml.Serialization.XmlSerializer can serialize.</param>
        public XmlSerializer(Type type)
        {
            this.typeToSerialize = type;
        }

        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a file using the specified System.IO.Stream.
        /// </summary>
        /// <param name="stream">The System.IO.Stream used to write the XML document.</param>
        /// <param name="instance">The System.Object to serialize.</param>
        public void Serialize(Stream stream, object instance)
        {
            using (XmlWriter xmlWriter = XmlWriter.Create(stream))
            {
                xmlWriter.WriteRaw("<?xml version=\"1.0\"?>");

                if (this.typeToSerialize.IsArray)
                {
                    SerializeRootArray(instance, xmlWriter);
                }
                else
                {
                    xmlWriter.WriteStartElement(this.typeToSerialize.Name);

                    foreach (FieldInfo fieldInfo in this.typeToSerialize.GetFields())
                    {
                        SerializeObject(instance, xmlWriter, fieldInfo);
                    }

                    xmlWriter.WriteEndElement();
                }
            }
        }
       
        /// <summary>
        /// Deserializes the XML document contained by the specified System.IO.Stream.
        /// </summary>
        /// <param name="stream">The System.IO.Stream that contains the XML document to deserialize.</param>
        /// <returns>The System.Object being deserialized.</returns>
        public object Deserialize(Stream stream)
        {
            object instance = this.typeToSerialize.GetConstructor(new Type[0]).Invoke(null);

            XmlReaderSettings readerSettings = 
                new XmlReaderSettings() 
                { 
                    IgnoreComments = true, 
                    IgnoreProcessingInstructions = true, 
                    IgnoreWhitespace = true 
                };

            using (XmlReader xmlReader = XmlReader.Create(stream, readerSettings))
            {
                try
                {
                    while (!xmlReader.EOF && xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element)
                        {
                            if (xmlReader.Name.IndexOf("ArrayOf") == 1)
                            {
                                DeserializeRootArray(instance, xmlReader);
                            }
                            else
                            {
                                foreach (FieldInfo fieldInfo in this.typeToSerialize.GetFields())
                                {
                                    DeserializeObject(instance, xmlReader, fieldInfo);
                                }
                            }
                        }
                    }
                }
                catch (XmlException xex)
                {
                    if (xex.ErrorCode != XmlException.XmlExceptionErrorCode.UnexpectedEOF)
                    {
                        throw;
                    }
                }
            }

            return instance;
        }

        private static void SerializeRootArray(object instance, XmlWriter xmlWriter)
        {
            bool first = true;

            foreach (var item in (IEnumerable)instance)
            {
                Type itemType = item.GetType();

                if (first)
                {
                    xmlWriter.WriteStartElement(string.Concat("ArrayOf", itemType.Name));
                    first = false;
                }

                xmlWriter.WriteStartElement(itemType.Name);

                foreach (FieldInfo fieldInfo in itemType.GetFields())
                {
                    SerializeObject(item, xmlWriter, fieldInfo);
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
        }

        private static void SerializeObject(object instance, XmlWriter xmlWriter, FieldInfo fieldInfo)
        {
            Type fieldType = fieldInfo.FieldType;

            if (fieldType.IsEnum)
            {
                SerializeEnum(instance, xmlWriter, fieldType, fieldInfo);
                return;
            }

            if (fieldType.IsArray)
            {
                SerializeArray(instance, xmlWriter, fieldInfo);
                return;
            }

            if (fieldType.IsClass && fieldType.Name != "String")
            {
                xmlWriter.WriteStartElement(fieldType.Name);

                foreach (FieldInfo fi in fieldType.GetFields())
                {
                    SerializeObject(fieldInfo.GetValue(instance), xmlWriter, fi);
                }

                xmlWriter.WriteEndElement();
            
                return;
            }

            if (fieldType.IsValueType && fieldType.FullName.IndexOf("System.") == -1)
            {
                // TODO: cannot deserialize since you cannot call the parameterless ctor of a struct from reflection
                // in the desktop framework you could use Activator.CreateInstance

                //xmlWriter.WriteStartElement(fieldInfo.Name);

                //foreach (FieldInfo fi in fieldType.GetFields())
                //{
                //    SerializeObject(fieldInfo.GetValue(instance), xmlWriter, fi);
                //}

                //xmlWriter.WriteEndElement();

                return;
            }

            object fieldValue = fieldInfo.GetValue(instance);
            if (fieldValue != null)
            {
                switch (fieldType.Name)
                {
                    case "Boolean":
                        // these need to be lowercase
                        xmlWriter.WriteElementString(fieldInfo.Name, fieldValue.ToString().ToLower());
                        break;

                    case "Char":
                        xmlWriter.WriteElementString(fieldInfo.Name, Encoding.UTF8.GetBytes(fieldValue.ToString())[0].ToString());
                        break;

                    case "DateTime":
                        xmlWriter.WriteElementString(fieldInfo.Name, GetStringFromDateTime((DateTime)fieldValue));
                        break;

                    default:
                        // TODO: structs fall through to here
                        xmlWriter.WriteElementString(fieldInfo.Name, fieldValue.ToString());
                        break;
                }
            }
        }

        private static void SerializeEnum(object instance, XmlWriter xmlWriter, Type fieldType, FieldInfo fieldInfo)
        {
            // NOTE: desktop .NET serializes enums with their .ToString() value ("Two") in the case below
            //          NETMF does not have the ability to parse an enum and only serializes the base value (1) in the case below
            //enum Numbers
            //{
            //    One,   // 0
            //    Two,   // 1
            //    Three  // 2
            //}
            //Numbers n = Numbers.Two;

            // NETMF -- 1
            // Desktop -- Two

            // add ability to have the declaring type provide serialization and deserialization methods
            //var method = fieldType.DeclaringType.GetMethod("Serialize" + fieldType.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            // cannot deserialize so screw it
            //MethodInfo method = null;
            //foreach (var t in fieldType.Assembly.GetTypes())
            //{
            //    method = t.GetMethod("Serialize" + fieldType.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            //    if (method != null)
            //    {
            //        break;
            //    }
            //}

            //if (method != null)
            //{
            //    var r = (string)method.Invoke(null, new object[] { fieldInfo.GetValue(instance) });
            //    xmlWriter.WriteElementString(fieldInfo.Name, r);
            //}
        }

        private static void SerializeArray(object instance, XmlWriter xmlWriter, FieldInfo fieldInfo)
        {
            object i = fieldInfo.GetValue(instance);

            if (i != null)
            {
                xmlWriter.WriteStartElement(fieldInfo.Name);

                foreach (var item in (IEnumerable)i)
                {
                    Type itemType = item.GetType();

                    if (itemType.IsClass && itemType.Name != "String")
                    {
                        xmlWriter.WriteStartElement(itemType.Name);

                        foreach (FieldInfo fi in itemType.GetFields())
                        {
                            SerializeObject(item, xmlWriter, fi);
                        }

                        xmlWriter.WriteEndElement();
                    }
                    else
                    {
                        SerializeArrayPrimitive(item, xmlWriter);
                    }
                }

                xmlWriter.WriteEndElement();
            }
        }
        
        private static void SerializeArrayPrimitive(object instance, XmlWriter xmlWriter)
        {
            string typeName = instance.GetType().Name;

            switch (typeName)
            {
                case "Boolean":
                    typeName = "boolean";
                    break;

                case "Byte":
                    //// TODO: this is not an array but a base64 encoded string but have not figured out how to decode it
                    ////xmlWriter.WriteElementString(fieldInfo.Name, Convert.ToBase64String((byte[])array));
                    return;

                case "SByte":
                    typeName = "byte";
                    break;

                case "Char":
                    typeName = "char";
                    break;

                case "DateTime":
                    typeName = "dateTime";
                    break;

                case "Double":
                    typeName = "double";
                    break;

                case "Guid":
                    typeName = "guid";
                    break;

                case "Int16":
                    typeName = "short";
                    break;
                case "UInt16":
                    typeName = "unsignedShort";
                    break;

                case "Int32":
                    typeName = "int";
                    break;
                case "UInt32":
                    typeName = "unsignedInt";
                    break;

                case "Int64":
                    typeName = "long";
                    break;
                case "UInt64":
                    typeName = "unsignedLong";
                    break;

                case "Single":
                    typeName = "float";
                    break;

                case "String":
                    typeName = "string";
                    break;
            }

            switch (typeName)
            {
                case "boolean":
                    // these need to be lowercase
                    xmlWriter.WriteElementString(typeName, instance.ToString().ToLower());
                    break;

                case "char":
                    xmlWriter.WriteElementString(typeName, Encoding.UTF8.GetBytes(instance.ToString())[0].ToString());
                    break;

                case "dateTime":
                    xmlWriter.WriteElementString(typeName, GetStringFromDateTime((DateTime)instance));
                    break;

                default:
                    xmlWriter.WriteElementString(typeName, instance.ToString());
                    break;
            }
        }

        private static void DeserializeRootArray(object instance, XmlReader xmlReader)
        {
            // TODO:
        }

        private static void DeserializeObject(object instance, XmlReader xmlReader, FieldInfo fieldInfo)
        {
            if (xmlReader.Name == fieldInfo.Name)
            {
                Type fieldType = fieldInfo.FieldType;

                if (fieldType.IsEnum)
                {
                    DeserializeEnum(instance, xmlReader, fieldType, fieldInfo);
                    return;
                }

                if (fieldType.IsArray)
                {
                    DeserializeArray(instance, xmlReader, fieldInfo);
                    return;
                }

                if (fieldType.IsClass && fieldType.Name != "String")
                {
                    object classInstance = fieldType.GetConstructor(new Type[0]).Invoke(null);
                    
                    xmlReader.Read();
                    
                    foreach (FieldInfo fi in fieldType.GetFields())
                    {
                        DeserializeObject(classInstance, xmlReader, fi);
                    }

                    fieldInfo.SetValue(instance, classInstance);

                    return;
                }

                if (fieldType.IsValueType && fieldType.FullName.IndexOf("System.") == -1)
                {
                    // TODO: cannot deserialize since you cannot call the parameterless ctor of a struct from reflection
                    // in the desktop framework you could use Activator.CreateInstance

                    //object structInstance = fieldType.GetConstructor(new Type[0]).Invoke(null);
                    ////object structInstance = AppDomain.CurrentDomain.CreateInstanceAndUnwrap(fieldType.Assembly.FullName, fieldType.FullName);
                    
                    //xmlReader.Read();

                    //foreach (FieldInfo fi in fieldType.GetFields())
                    //{
                    //    DeserializeObject(structInstance, xmlReader, fi);
                    //}

                    //fieldInfo.SetValue(instance, structInstance);
                    return;
                }

                string tempValue = xmlReader.ReadElementString(fieldInfo.Name);

                switch (fieldType.Name)
                {
                    case "Boolean":
                        fieldInfo.SetValue(instance, tempValue == "true");
                        break;

                    case "Byte":
                        fieldInfo.SetValue(instance, Convert.ToByte(tempValue));
                        break;
                    case "SByte":
                        fieldInfo.SetValue(instance, Convert.ToSByte(tempValue));
                        break;

                    case "Char":
                        fieldInfo.SetValue(instance, Convert.ToChar(Convert.ToByte(tempValue)));
                        break;

                    case "DateTime":
                        fieldInfo.SetValue(instance, GetDateTimeFromString(tempValue));
                        break;

                    case "Double":
                        fieldInfo.SetValue(instance, Convert.ToDouble(tempValue));
                        break;

                    case "Guid":
                        fieldInfo.SetValue(instance, GetGuidFromString(tempValue));
                        break;

                    case "Int16":
                        fieldInfo.SetValue(instance, Convert.ToInt16(tempValue));
                        break;
                    case "UInt16":
                        fieldInfo.SetValue(instance, Convert.ToUInt16(tempValue));
                        break;

                    case "Int32":
                        fieldInfo.SetValue(instance, Convert.ToInt32(tempValue));
                        break;
                    case "UInt32":
                        fieldInfo.SetValue(instance, Convert.ToUInt32(tempValue));
                        break;

                    case "Int64":
                        fieldInfo.SetValue(instance, Convert.ToInt64(tempValue));
                        break;
                    case "UInt64":
                        fieldInfo.SetValue(instance, Convert.ToUInt64(tempValue));
                        break;

                    case "Single":
                        fieldInfo.SetValue(instance, (Single)Convert.ToDouble(tempValue));
                        break;

                    case "String":
                        fieldInfo.SetValue(instance, tempValue);
                        break;

                    default:
                        break;
                }
            }
        }
        
        private static void DeserializeEnum(object instance, XmlReader xmlReader, Type fieldType, FieldInfo fieldInfo)
        {
            // NOTE: desktop .NET serializes enums with their .ToString() value ("Two") in the case below
            //          NETMF does not have the ability to parse an enum and only serializes the base value (1) in the case below

            //enum Numbers
            //{
            //    One,   // 0
            //    Two,   // 1
            //    Three  // 2
            //}
            //Numbers n = Numbers.Two;

            // NETMF -- 1
            // Desktop -- Two

            // add ability to have the declaring type provide serialization and deserialization methods
            //var method = fieldType.DeclaringType.GetMethod("Deserialize" + fieldType.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);


            var a = xmlReader.ReadElementString();

            //MethodInfo method = null;
            //foreach (var t in fieldType.Assembly.GetTypes())
            //{
            //    method = t.GetMethod("Deserialize" + fieldType.Name, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            //    if (method != null)
            //    {
            //        break;
            //    }
            //}

            //if (method != null)
            //{
            //    var r = method.Invoke(null, new object[] { xmlReader.ReadElementString() });
            //    // TODO: still get a CLR_E_WRONGTYPE
            //    // ggggggrrrrrrrrrrrrrrrrr
            //    //fieldInfo.SetValue(instance, r));
            //}
        }
        
        private static void DeserializeArray(object instance, XmlReader xmlReader, FieldInfo fieldInfo)
        {
            string startingElementName = xmlReader.Name;
            xmlReader.Read();
            string arrayTypeName = xmlReader.Name;

            // try to find the type of the array
            ArrayList possibleTypes = new ArrayList();
            foreach (var t in fieldInfo.DeclaringType.Assembly.GetTypes())
            {
                if (t.FullName.LastIndexOf(string.Concat(".", arrayTypeName)) > -1)
                {
                    possibleTypes.Add(t);
                }
            }

            if (possibleTypes.Count == 0)
            {
                // this should be for string, int, long, <primitive type>
                fieldInfo.SetValue(instance, DeserializeArrayPrimitive(startingElementName, xmlReader));
            }
            else
            {
                Type arrayType = (Type)possibleTypes[0];

                if (possibleTypes.Count > 1)
                {
                    // TODO: check which one is "closer" to the class we are in?
                }

                ArrayList array = new ArrayList();
                while (xmlReader.Name != startingElementName)
                {
                    object classInstance = arrayType.GetConstructor(new Type[0]).Invoke(null);
                    array.Add(classInstance);
                    xmlReader.Read();

                    foreach (FieldInfo fi in arrayType.GetFields())
                    {
                        DeserializeObject(classInstance, xmlReader, fi);
                    }

                    xmlReader.Read();
                }

                fieldInfo.SetValue(instance, array.ToArray(arrayType));
            }

            xmlReader.Read();
        }
        
        private static object DeserializeArrayPrimitive(string startingElementName, XmlReader xmlReader)
        {
            string arrayType = xmlReader.Name;

            ArrayList array = new ArrayList();
            Type returnType = null;

            while (xmlReader.Name != startingElementName)
            {
                string tempValue = xmlReader.ReadElementString();

                switch (arrayType)
                {
                    case "boolean":
                        returnType = returnType ?? typeof(bool);
                        array.Add(tempValue == "true");
                        break;

                    //// TODO: this is not an array but a base64 encoded string
                    ////case "byte":
                    ////    //returnType = returnType ?? typeof(bool);
                    ////    //return Convert.FromBase64String(tempValue);
                    ////    //throw new Exception("Should never reach this");
                    ////    break;

                    case "byte":
                        returnType = returnType ?? typeof(sbyte);
                        array.Add(Convert.ToSByte(tempValue));
                        break;

                    case "char":
                        returnType = returnType ?? typeof(char);
                        array.Add(Convert.ToChar(Convert.ToByte(tempValue)));
                        break;

                    case "dateTime":
                        returnType = returnType ?? typeof(DateTime);
                        array.Add(GetDateTimeFromString(tempValue));
                        break;

                    case "double":
                        returnType = returnType ?? typeof(double);
                        array.Add(Convert.ToDouble(tempValue));
                        break;

                    case "guid":
                        returnType = returnType ?? typeof(Guid);
                        array.Add(GetGuidFromString(tempValue));
                        break;

                    case "short":
                        returnType = returnType ?? typeof(short);
                        array.Add(Convert.ToInt16(tempValue));
                        break;
                    case "unsignedShort":
                        returnType = returnType ?? typeof(ushort);
                        array.Add(Convert.ToUInt16(tempValue));
                        break;

                    case "int":
                        returnType = returnType ?? typeof(int);
                        array.Add(Convert.ToInt32(tempValue));
                        break;
                    case "unsignedInt":
                        returnType = returnType ?? typeof(uint);
                        array.Add(Convert.ToUInt32(tempValue));
                        break;

                    case "long":
                        returnType = returnType ?? typeof(long);
                        array.Add(Convert.ToInt64(tempValue));
                        break;
                    case "unsignedLong":
                        returnType = returnType ?? typeof(ulong);
                        array.Add(Convert.ToUInt64(tempValue));
                        break;

                    case "float":
                        returnType = returnType ?? typeof(float);
                        array.Add((Single)Convert.ToDouble(tempValue));
                        break;

                    case "string":
                        returnType = returnType ?? typeof(string);
                        array.Add(tempValue);
                        break;

                    default:
                        returnType = returnType ?? typeof(object);
                        break;
                }
            }

            return array.ToArray(returnType);
        }

        private static object GetGuidFromString(string tempValue)
        {
            byte[] guidBytes = new byte[16];
            string[] split = tempValue.Split('-');
            int location = 0;

            for (int i = 0; i < split.Length; i++)
            {
                byte[] tempArray = HexToBytes(split[i]);

                //// TODO: is this needed or will it always need to be reversed
                ////bool temp = Microsoft.SPOT.Hardware.SystemInfo.IsBigEndian;

                if (i < 3)
                {
                    int end = tempArray.Length - 1;
                    for (int start = 0; start < end; start++)
                    {
                        byte b = tempArray[start];
                        tempArray[start] = tempArray[end];
                        tempArray[end] = b;
                        end--;
                    }
                }

                Array.Copy(tempArray, 0, guidBytes, location, tempArray.Length);
                location += split[i].Length / 2;
            }
            
            return new Guid(guidBytes);
        }

        private static byte[] HexToBytes(string hexString)
        {
            // Based on http://stackoverflow.com/a/3974535
            if (hexString.Length == 0 || hexString.Length % 2 != 0)
            {
                return new byte[0];
            }

            byte[] buffer = new byte[hexString.Length / 2];
            char c;
            for (int bx = 0, sx = 0; bx < buffer.Length; ++bx, ++sx)
            {
                // Convert first half of byte 
                c = hexString[sx];
                byte b = (byte)((c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0')) << 4);

                // Convert second half of byte 
                c = hexString[++sx];
                b |= (byte)(c > '9' ? (c > 'Z' ? (c - 'a' + 10) : (c - 'A' + 10)) : (c - '0'));
                buffer[bx] = b;
            }

            return buffer;
        }

        private static string GetStringFromDateTime(DateTime tempValue)
        {
            //// NETMF serializes out to this
            ////2012-06-27T20:02:40 -- ("s" format)
            //// .NET does this
            ////2012-06-27T20:36:57.995-07:00 -- ("o" format)
            ////                     ^ ^^^^^^
            ////                     |     |
            ////           milliseconds   timezone (-7 from GMT) (sometimes)

            // TODO: 
            //TimeSpan t = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);

            return tempValue.ToString("s");
        }

        private static DateTime GetDateTimeFromString(string tempValue)
        {
            int year = int.Parse(tempValue.Substring(0, 4));
            int month = int.Parse(tempValue.Substring(5, 2));
            int day = int.Parse(tempValue.Substring(8, 2));
            int hour = int.Parse(tempValue.Substring(11, 2));
            int minute = int.Parse(tempValue.Substring(14, 2));
            int second = int.Parse(tempValue.Substring(17, 2));

            //// NETMF serializes out to this
            ////2012-06-27T20:02:40 -- ("s" format)
            //// .NET does this
            ////2012-06-27T20:36:57.995-07:00 -- ("o" format)
            ////                     ^ ^^^^^^
            ////                     |     |
            ////           milliseconds   timezone (-7 from GMT) (sometimes)
            int millisecond = tempValue.Length == 19 ? 0 : int.Parse(tempValue.Substring(20, 2));
            
            var dateTime = new DateTime(year, month, day, hour, minute, second, millisecond);
            
            // TODO: 
            //TimeSpan t = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);

            return dateTime;
        }
    }
}
