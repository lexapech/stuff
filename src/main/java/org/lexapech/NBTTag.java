package org.lexapech;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.IntBuffer;
import java.nio.LongBuffer;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.Objects;
import java.util.Queue;

public class NBTTag {

    public enum TagID {
        TAG_End {
            @Override
            public int intValue() {
                return 0;
            }
        },
        TAG_Byte {
            @Override
            public Object read(InputStream stream) throws IOException {
                return stream.read();
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                stream.write((int) value);
            }

            @Override
            public int intValue() {
                return 1;
            }
        },
        TAG_Short {
            @Override
            public Object read(InputStream stream) throws IOException {
                return ByteBuffer.wrap(stream.readNBytes(Short.BYTES)).getShort();
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                stream.write(ByteBuffer.allocate(Short.BYTES).putShort((short) value).array());
            }

            @Override
            public int intValue() {
                return 2;
            }
        },
        TAG_Int {
            @Override
            public Object read(InputStream stream) throws IOException {
                return ByteBuffer.wrap(stream.readNBytes(Integer.BYTES)).getInt();
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                stream.write(ByteBuffer.allocate(Integer.BYTES).putInt((int) value).array());
            }

            @Override
            public int intValue() {
                return 3;
            }
        },
        TAG_Long {
            @Override
            public Object read(InputStream stream) throws IOException {
                return ByteBuffer.wrap(stream.readNBytes(Long.BYTES)).getLong();
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                stream.write(ByteBuffer.allocate(Long.BYTES).putLong((long) value).array());
            }

            @Override
            public int intValue() {
                return 4;
            }
        },
        TAG_Float {
            @Override
            public Object read(InputStream stream) throws IOException {
                return ByteBuffer.wrap(stream.readNBytes(Float.BYTES)).getFloat();
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                stream.write(ByteBuffer.allocate(Float.BYTES).putFloat((float) value).array());
            }

            @Override
            public int intValue() {
                return 5;
            }
        },
        TAG_Double {
            @Override
            public Object read(InputStream stream) throws IOException {
                return ByteBuffer.wrap(stream.readNBytes(Double.BYTES)).getDouble();
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                stream.write(ByteBuffer.allocate(Double.BYTES).putDouble((double) value).array());
            }

            @Override
            public int intValue() {
                return 6;
            }
        },
        TAG_Byte_Array {
            @Override
            public Object read(InputStream stream) throws IOException {
                int length = (int) TAG_Int.read(stream);
                return stream.readNBytes(length);
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                TAG_Int.write(stream, ((byte[]) value).length);
                stream.write((byte[]) value);
            }

            @Override
            public int intValue() {
                return 7;
            }
        },
        TAG_String {
            @Override
            public Object read(InputStream stream) throws IOException {
                int length = (int) ((short) TAG_Short.read(stream));
                return new String(stream.readNBytes(length), StandardCharsets.UTF_8);
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                byte[] bytes = ((String) value).getBytes(StandardCharsets.UTF_8);
                TAG_Short.write(stream, (short) bytes.length);
                stream.write(bytes);
            }

            @Override
            public int intValue() {
                return 8;
            }
        },
        TAG_List {
            @Override
            public int intValue() {
                return 9;
            }
        },
        TAG_Compound {
            @Override
            public int intValue() {
                return 10;
            }
        },
        TAG_Int_Array {
            @Override
            public Object read(InputStream stream) throws IOException {
                int length = (int) TAG_Int.read(stream);
                int[] array = new int[length];
                ByteBuffer buffer = ByteBuffer.wrap(stream.readNBytes(length * Integer.BYTES));
                int i = 0;
                while (buffer.hasRemaining()) {
                    array[i++] = buffer.getInt();
                }
                return array;
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                int[] array = (int[]) value;
                TAG_Int.write(stream, array.length);
                ByteBuffer byteBuffer = ByteBuffer.allocate(array.length * Integer.BYTES);
                IntBuffer intBuffer = byteBuffer.asIntBuffer();
                intBuffer.put(array);
                stream.write(byteBuffer.array());
            }

            @Override
            public int intValue() {
                return 11;
            }
        },
        TAG_Long_Array {
            @Override
            public Object read(InputStream stream) throws IOException {
                int length = (int) TAG_Int.read(stream);
                long[] array = new long[length];
                ByteBuffer buffer = ByteBuffer.wrap(stream.readNBytes(length * Long.BYTES));
                int i = 0;
                while (buffer.hasRemaining()) {
                    array[i++] = buffer.getLong();
                }
                return array;
            }

            @Override
            public void write(OutputStream stream, Object value) throws IOException {
                long[] array = (long[]) value;
                TAG_Int.write(stream, array.length);
                ByteBuffer byteBuffer = ByteBuffer.allocate(array.length * Long.BYTES);
                LongBuffer longBuffer = byteBuffer.asLongBuffer();
                longBuffer.put(array);
                stream.write(byteBuffer.array());
            }

            @Override
            public int intValue() {
                return 12;
            }
        };

        public static TagID valueOf(Integer value) {
            return switch (value) {
                case 0 -> TAG_End;
                case 1 -> TAG_Byte;
                case 2 -> TAG_Short;
                case 3 -> TAG_Int;
                case 4 -> TAG_Long;
                case 5 -> TAG_Float;
                case 6 -> TAG_Double;
                case 7 -> TAG_Byte_Array;
                case 8 -> TAG_String;
                case 9 -> TAG_List;
                case 10 -> TAG_Compound;
                case 11 -> TAG_Int_Array;
                case 12 -> TAG_Long_Array;
                default -> throw new IllegalArgumentException("Unknown tag id");
            };
        }

        public Object read(InputStream stream) throws IOException {
            return null;
        }
        public void write(OutputStream stream, Object value) throws IOException {
        }
        public abstract int intValue();
    }


    private final TagID tagID;
    private final String name;
    private Object data;
    private NBTTag parent;

    private final LinkedList<NBTTag> nodes;

    public NBTTag(TagID id, String name) {
        tagID = id;
        this.name = name;
        data = null;
        if (id == TagID.TAG_Compound || id == TagID.TAG_List) {
            nodes = new LinkedList<>();
        } else {
            nodes = null;
        }
        parent = null;
    }

    public NBTTag(TagID id, String name,Object data) {
        this(id,name);
        setData(data);
    }
    public LinkedList<NBTTag> getNodes() {
        return nodes;
    }

    public TagID getID() {
        return tagID;
    }

    public String getName() {
        return name;
    }

    public Object getData() {
        return data;
    }

    public void setData(Object data) {
        this.data = data;
    }

    private void setParent(NBTTag parent) {
        this.parent = parent;
    }

    public void addNode(NBTTag node) {
        if (nodes != null) {
            node.setParent(this);
            nodes.add(node);
        } else throw new UnsupportedOperationException("Tag type is not List or Compound");
    }

    public NBTTag getNodeWithName(String name) {
        if (nodes == null) throw new UnsupportedOperationException("Tag type is not List or Compound");
        String[] path = name.split("/");
        NBTTag current = this;
        for (String tag : path) {
            boolean found = false;
            if (current.tagID == TagID.TAG_List) {
                current = current.getNodes().get(Integer.parseInt(tag));
                if (current == null) throw new NullPointerException("");
                continue;
            }

            for (NBTTag node : current.getNodes()) {

                if (node.getName().equals(tag)) {
                    current = node;
                    break;
                }
            }
        }
        return current;
    }

    public NBTTag findNodeWithName(String name) {
        if (nodes == null) throw new UnsupportedOperationException("Tag type is not List or Compound");
        ArrayList<NBTTag> visited = new ArrayList<>();
        Queue<NBTTag> queue = new LinkedList<>();
        queue.add(this);
        visited.add(this);
        while (!queue.isEmpty()) {
            NBTTag node = queue.poll();
            if (node.getName().equals(name)) return node;
            if (node.getNodes() != null) {
                for (NBTTag child : node.getNodes()) {
                    if (!visited.contains(child)) {
                        queue.add(child);
                        visited.add(child);
                    }
                }
            }
        }
        return null;
    }
    public void delete() {
        parent.getNodes().remove(this);
    }
    public void deleteNodeWithName(String name) {
        NBTTag node = findNodeWithName(name);
        node.delete();
    }
}
