using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using edkSerializer;
using edkSerializer.API;

namespace edkSerializer {

	class Program {

        static void Main(string[] args) {

			var fileToSave = "saved.binary";

			var zayn = new Person {
				Name = new FullName("Zayn", "Malik"),
                Age = 21,
                Address = new Address {

                    Street = "OneDirection Rd",

                    City = "London",

                    ZipCode = "777111",

                    PhoneNumber = "+44-7065892323"

                }

            };



            var harry = new Person {

				Name = new FullName("Harry", "Styles"),

                Age = 20,

				Address = zayn.Address,

				Friend = zayn

            };

			zayn.Friend = harry;


            TextWriterTraceListener tr1 = new TextWriterTraceListener(System.Console.Out);
            
            Trace.Listeners.Add(tr1);
            
            
            Trace.WriteLine("Start Serilaization");

            using (var buffer = new MemoryStream()) {
               
				Serializer.Serialize(zayn, buffer);
                

				var fileStream = File.Create(fileToSave);

				buffer.Seek(0, SeekOrigin.Begin);
				buffer.WriteTo(fileStream);

				fileStream.Close();
				        
                buffer.Seek(0, SeekOrigin.Begin);

				var loadedZayn = Serializer.Deserialize<Person>(buffer);


				Trace.Assert(zayn == loadedZayn, "Fail, object are not logicaly the same");

				Trace.Assert(loadedZayn != null && (loadedZayn.Friend != null || loadedZayn.Friend.Address != null),
                  "Couldn't recreate refrences");

				Trace.Assert(loadedZayn != null && loadedZayn.Friend != null 
					&& loadedZayn.Friend.Address != null && loadedZayn.Address != null 
					&& ReferenceEquals(loadedZayn.Address, loadedZayn.Friend.Address), "Address is not the same object");
			

			}

            Trace.Flush();

            

        }

    }
}