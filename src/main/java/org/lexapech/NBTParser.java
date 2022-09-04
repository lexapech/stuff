package org.lexapech;

import java.io.IOException;
import java.io.InputStream;
import static org.lexapech.NBTTag.TagID;

public class NBTParser {
    private static int tagCounter;
    public static NBTTag parse(InputStream stream) throws IOException {
        tagCounter = 0;
        NBTTag root = new NBTTag(TagID.TAG_Compound, "root");
        parseRecursively(stream,root);
        System.out.println("[NBT Parser] "+tagCounter + " tags parsed");
        return root;

    }
    private static void parseRecursively(InputStream stream, NBTTag parent) throws IOException {
        while(true) {
            int id;
            String name = "";
            if (parent.getID() == TagID.TAG_List) {
                int[] parentData = (int[]) parent.getData();
                id = parentData[0];
                parentData[1]--;
                if (parentData[1] < 0) {
                    parent.setData(null);
                    return;
                }
            } else {
                id = stream.read();
                if (id == -1) return;
                if (TagID.valueOf(id) == TagID.TAG_End) return;
                name = (String)TagID.TAG_String.read(stream);
            }
            NBTTag tag = new NBTTag(TagID.valueOf(id), name);
            if (TagID.valueOf(id) == TagID.TAG_List) {
                int type = (int)TagID.TAG_Byte.read(stream);
                int length = (int)TagID.TAG_Int.read(stream);
                tag.setData(new int[]{type, length});
                parseRecursively(stream, tag);
            }
            else if (TagID.valueOf(id) == TagID.TAG_Compound) {
                parseRecursively(stream, tag);
            }
            else {
                tag.setData(TagID.valueOf(id).read(stream));
            }
            parent.addNode(tag);
            tagCounter++;
        }
    }
}
