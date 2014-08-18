using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace edkSerializer {
  
  public static class TypeConstants {
  
    public static byte TC_STRING = 0x74;
    public static byte TC_LIST = 0x75;
    public static byte TC_REFERENCE = 0x71;
    public static byte TC_OBJECT = 0x73;
    public static byte TC_CLASSDESC = 0x72;
    public static byte TC_ENDBLOCKDATA = 0x78;
    public static byte[] STREAM_MAGIC = new byte[] { 0xAC, 0xED };
    public static byte[] STREAM_VERSION = new byte[] { 0x00, 0x05 };
    
  
  }

 

}
