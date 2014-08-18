using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace edkSerializer.Core {
  public class DeserializeProcess<T> {

    	private enum State { ReadType, ReadProperty, ReadValue, Finish } ;
    
		private Dictionary<short, object> _reverseRefTable;
    

    	public DeserializeProcess() {
    
      		_reverseRefTable = new Dictionary<short, object>();
      	}

    	public T Run(Stream stream) {
     		Object temp = null;

      		short version = VerifyStream(stream);
      
      		if (typeof(T).IsClass) {
        		temp = Activator.CreateInstance(typeof(T));
      		}

      		T res = (T)temp;

      		if (ConversionModule.ReadByte(stream) == (byte)TypeCode.Object) {
        		if (res.GetType().Name == ConversionModule.ReadString(stream)) {
          			short objID = ConversionModule.ReadShort(stream);

          			ReadObject(res, objID, stream);
        		}
      		}

      		return res;
      
    	}

    	private static short VerifyStream(Stream stream) {

      		byte[] currentByte = new byte[2];

      		// Read serialization algorithm type
      		stream.Read(currentByte, 0, 2);

      		if (currentByte[0] != TypeConstants.STREAM_MAGIC[0]) {
        		throw new Exception("Not a serializied strem");
      		}
	
    	    // Read version
      		stream.Read(currentByte, 0, 2);

		    return ConversionModule.ReadShort(currentByte);
    	}

    	private void ReadObject(object target, short id, Stream stream) {

      		State fsm = State.ReadType;

      		byte propType = (byte)TypeCode.Int16;
     	    String propeName = "";
      		bool terminate = false;
      		PropertyInfo[] propArray = target.GetType().GetProperties();

      		if (!_reverseRefTable.ContainsKey(id)) {
        		_reverseRefTable.Add(id, target);
      		}

			 
      		while (!terminate && stream.Position < stream.Length) {

        		switch (fsm) {
          			case State.ReadType:
	            		propType = ConversionModule.ReadByte(stream);
	            		fsm = State.ReadProperty;

	            		if (propType == TypeConstants.TC_ENDBLOCKDATA) {
	              			fsm = State.Finish;
	            		}

	            		break;
          			case State.ReadProperty:
			            propeName = ConversionModule.ReadString(stream);
			            fsm = State.ReadValue;
			            break;
		          	case State.ReadValue:
		            	PropertyInfo p = propArray.First(c => c.Name == propeName);

			            if (propType == (byte)TypeCode.String) {
			            	p.SetValue(target, ConversionModule.ReadString(stream), null);
			            } else if (propType == (byte)TypeCode.Object) {
							short objID = ConversionModule.ReadShort(stream);
			              	object obj = Activator.CreateInstance(p.PropertyType);
							ReadObject(obj, objID, stream);

			              	p.SetValue(target, obj, null);

			            } else if (propType == (byte)TypeConstants.TC_REFERENCE) {

			              	short objID = ConversionModule.ReadShort(stream);

			              	if (_reverseRefTable.ContainsKey(objID)) {
			                	object obj = _reverseRefTable[objID];
			                	p.SetValue(target, obj, null);
			              	}

			            } else {

			              	p.SetValue(target, ConversionModule.ReadValue((TypeCode)propType, stream), null);

			            }

		            	fsm = State.ReadType;
		            	break;
		          	case State.Finish:
		            	terminate = true;
		            	break;
		          	default:
		            	terminate = true;
		            	break;
		        	
				}

      		}
    	}
  	}
}
