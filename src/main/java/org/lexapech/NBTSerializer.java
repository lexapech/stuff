package org.lexapech;

import java.io.IOException;
import java.io.OutputStream;
import static org.lexapech.NBTTag.TagID;

public class NBTSerializer {

    private static int tagCounter;
    public static void serialize(OutputStream stream,NBTTag root) throws IOException {
        tagCounter = 0;
        serializeRecursively(stream,root);
        System.out.println("[NBT Serializer] " + tagCounter +" tags serialized");
    }
    private static  void serializeRecursively(OutputStream stream,NBTTag parent) throws IOException {
      for(NBTTag tag : parent.getNodes()) {
          if (parent.getID() != TagID.TAG_List) {
              TagID.TAG_Byte.write(stream,tag.getID().intValue());
              TagID.TAG_String.write(stream,tag.getName());
          }
          if (tag.getID() == TagID.TAG_List) {
              int type = 0;
              if (tag.getNodes().size() > 0) {
                  type = tag.getNodes().get(0).getID().intValue();
              }
              TagID.TAG_Byte.write(stream,type);
              TagID.TAG_Int.write(stream,tag.getNodes().size());
              serializeRecursively(stream,tag);
          }
          else if (tag.getID() == TagID.TAG_Compound) {
              serializeRecursively(stream,tag);
              TagID.TAG_Byte.write(stream,0);
          }
          else {
              tag.getID().write(stream,tag.getData());
          }
          tagCounter++;
      }
    }
}
