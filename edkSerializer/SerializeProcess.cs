using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;

namespace edkSerializer.Core {
  public class SerializeProcess<T> {
    
    private Dictionary<object, short> _referenceTable;
    private short _objectID = 1;
    

    public SerializeProcess() {
      _referenceTable = new Dictionary<object, short>();
      
    }

    public void Run(T o,Stream stream) {

      WriteHeader(stream);

      if (o.GetType().IsValueType) {

      } else {

        ConversionModule.WriteRefTypeProperty(o.GetType().Name, _objectID, stream, (byte)TypeCode.Object);
        _referenceTable.Add(o, _objectID);
        _objectID += 1;
        CreateSerializ(o, stream);
      }

    }

    private void WriteHeader(Stream stream) {
      // Write serialization algorithm
      stream.Write(TypeConstants.STREAM_MAGIC, 0, 2);

      // Write version
      stream.Write(TypeConstants.STREAM_VERSION, 0, 2);

    }

    private void CreateSerializ(object o, Stream stream) {

      // Write class description
      foreach (PropertyInfo p in o.GetType().GetProperties()) {

        if (p.PropertyType == typeof(System.String)) {

          ConversionModule.WriteStringTypeProperty(p.Name, (string)(p.GetValue(o, null)), stream);

        } else if (p.PropertyType.IsValueType) {

          ConversionModule.WriteValueTypeProperty(p.Name,p.GetValue(o,null),p.PropertyType, stream);

        } else {

          object subProperty = p.GetValue(o, null);

          if (_referenceTable.ContainsKey(subProperty)) {
            short current_id = _referenceTable[subProperty];
            ConversionModule.WriteRefTypeProperty(p.Name, current_id, stream, TypeConstants.TC_REFERENCE);

          } else {

            ConversionModule.WriteRefTypeProperty(p.Name, _objectID, stream, (byte)TypeCode.Object);
            _referenceTable.Add(subProperty, _objectID);
            _objectID += 1;
            CreateSerializ(subProperty, stream);
          }



        }

      }

      // Write version
      stream.Write(new byte[] { TypeConstants.TC_ENDBLOCKDATA }, 0, 1);

    }
  }
}
