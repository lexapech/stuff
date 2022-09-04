package org.lexapech;


import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.math.BigInteger;
import java.nio.ByteBuffer;
import java.nio.IntBuffer;
import java.nio.channels.FileChannel;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;
import java.util.*;
import java.util.zip.Inflater;
import java.util.zip.InflaterInputStream;
import static java.nio.file.Files.newInputStream;

public class MCAReader {

    private final Path regionDirectory;

    public MCAReader(Path region){
       regionDirectory = region;
    }
    public NBTTag read(int x, int z) throws IOException {
        int regionX = x >> 5;
        int regionZ = z >> 5;
        String fileName = String.format("%s/r.%d.%d.mca",regionDirectory.toString(),regionX,regionZ);
        System.out.println("[MCA Reader] reading file " + fileName);
        try (InputStream fileStream = newInputStream(Path.of(fileName))) {
            MCAHeader header = new MCAHeader(fileStream);
            fileStream.skipNBytes((long)4096 * (header.getChunkOffset(x & 31, z & 31)-2));
            int length = new BigInteger(fileStream.readNBytes(4)).intValue();
            byte compression = (byte)fileStream.read();
            try (InflaterInputStream decompress = new InflaterInputStream(fileStream, new Inflater(),length))
            {
                System.out.println("[MCA Reader] parsing tag, size " + length);
                NBTTag tag = NBTParser.parse(decompress);
                System.out.println("[MCA Reader] tag parsed");
                return tag;
            }
        }
    }
    public ArrayList<Map.Entry<Point2,NBTTag>> readBatch(List<Point2> chunks) throws IOException {

        HashMap<Point2,ArrayList<Point2>> regions = new HashMap<>();
        for (Point2 chunk : chunks) {
            int regionX = chunk.x() >> 5;
            int regionZ = chunk.y() >> 5;
            ArrayList<Point2> region = regions.computeIfAbsent(new Point2(regionX, regionZ), k -> new ArrayList<>());
            region.add(chunk);
        }
        ArrayList<Map.Entry<Point2,NBTTag>> chunkTags = new ArrayList<>();
        for (Map.Entry<Point2,ArrayList<Point2>> region : regions.entrySet()) {
            String fileName = String.format("%s/r.%d.%d.mca",regionDirectory.toString(),region.getKey().x(),region.getKey().y());
            System.out.println("[MCA Reader] reading file " + fileName);

            try (FileChannel fileChannel = FileChannel.open(Path.of(fileName),StandardOpenOption.READ)) {
                MCAHeader header = new MCAHeader(fileChannel);

                for (Point2 pos : region.getValue()) {
                    ByteBuffer sizeBuf = ByteBuffer.allocate(4);
                    IntBuffer sizeBufInt = sizeBuf.asIntBuffer();
                    fileChannel.position((long) 4096 * (header.getChunkOffset(pos.x() & 31, pos.y() & 31)));
                    fileChannel.read(sizeBuf);
                    int length = sizeBufInt.get();
                    ByteBuffer compressionBuf = ByteBuffer.allocate(1);
                    fileChannel.read(compressionBuf);
                    ByteBuffer dataBuf = ByteBuffer.allocate(length);
                    fileChannel.read(dataBuf);
                    ByteArrayInputStream stream = new ByteArrayInputStream(dataBuf.array());
                    try (InflaterInputStream decompress = new InflaterInputStream(stream, new Inflater(), length)) {
                        System.out.println("[MCA Reader] parsing tag, size " + length);
                        NBTTag tag = NBTParser.parse(decompress);
                        System.out.println("[MCA Reader] tag parsed");
                        chunkTags.add(Map.entry(pos,tag));
                    }
                }
            }
        }
        return chunkTags;
    }
}
