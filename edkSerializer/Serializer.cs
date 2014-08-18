using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;
using edkSerializer.Core;

namespace edkSerializer.API {
  public class Serializer {

		public static void Serialize<T>(T o, Stream stream) {
			SerializeProcess<T> s = new SerializeProcess<T>();

			s.Run(o, stream);

    	}

    	public static T Deserialize<T>(Stream stream) {

			DeserializeProcess<T> d = new DeserializeProcess<T> ();

			return d.Run (stream);

		}

  }
}
