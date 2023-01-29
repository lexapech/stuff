package org.lexapech;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.IntBuffer;
import java.nio.channels.FileChannel;
import java.nio.file.Path;
import java.nio.file.StandardOpenOption;
import java.util.ArrayList;
import java.util.zip.DeflaterOutputStream;

import static org.lexapech.Constants.*;
import static org.lexapech.MCAHeader.*;

public class MCAWriter {
    private final Path regionDirectory;
    public MCAWriter(Path regionDirectory) {

        this.regionDirectory = regionDirectory;
    }
    public void write(int x, int z, NBTTag root) throws IOException {
        int regionX = x >> 5;
        int regionZ = z >> 5;

        ByteArrayOutputStream compressed = new ByteArrayOutputStream();
        DeflaterOutputStream compress = new DeflaterOutputStream(compressed);
        NBTSerializer.serialize(compress,root);
        compress.close();
        System.out.println("[MCA Writer] compressing tag, resulting size " + compressed.size());
        ByteBuffer chunkData = ByteBuffer.allocate(5 + compressed.size());
        chunkData.putInt(compressed.size());
        chunkData.put((byte)2);
        chunkData.put(compressed.toByteArray());
        chunkData.position(0);
        int sectors = (chunkData.limit()/4096) + ((chunkData.limit() % 4096) != 0 ? 1 : 0);

        Path fileName = Path.of(String.format("%s/r.%d.%d.mca",regionDirectory.toString(),regionX,regionZ));
        try (FileChannel fileChannel = FileChannel.open(fileName,StandardOpenOption.READ,StandardOpenOption.WRITE,StandardOpenOption.CREATE)) {
            if(fileChannel.size() >= LOCATIONS_LENGTH + TIMESTAMPS_LENGTH) {
                ByteBuffer buff = ByteBuffer.allocate(LOCATIONS_LENGTH + TIMESTAMPS_LENGTH);
                fileChannel.read(buff);
                MCAHeader header = new MCAHeader(buff.array());
                int offset = header.getChunkOffset(x, z);
                int oldSize = 0;
                if (offset > 0) {
                    oldSize = getChunkLength(fileChannel,offset);
                }
                if (sectors > oldSize) {
                    int position = findPlace(fileChannel,header,sectors);
                    ByteBuffer newLocation = ByteBuffer.allocate(4);
                    IntBuffer newLocationInt = newLocation.asIntBuffer();
                    newLocationInt.put(position);
                    newLocation.position(1);
                    fileChannel.write(newLocation,MCAHeader.coordinatesToIndex(x, z));
                    fileChannel.write(chunkData,position * 4096L);
                    System.out.println("[MCA Writer] old location was "+offset+", writing "+sectors+" blocks to "+position);
                }
                else {
                        System.out.println("[MCA Writer] old size was "+oldSize+", writing "+sectors+" blocks to old position");
                        fileChannel.write(chunkData,offset * 4096L);
                }
            }
            else {
                ByteBuffer newHeader = ByteBuffer.allocate(LOCATIONS_LENGTH + TIMESTAMPS_LENGTH);
                IntBuffer newLocationInt = newHeader.asIntBuffer();
                newLocationInt.put(MCAHeader.coordinatesToIndex(x, z),2);
                fileChannel.write(newHeader);
                fileChannel.write(chunkData,2 * 4096L);

                System.out.println("[MCA Writer] created new file");
            }
        }
    }

    private int getChunkLength(FileChannel file, int index) throws IOException{
        ByteBuffer byteBuffer = ByteBuffer.allocate(4);
        IntBuffer intBuffer = byteBuffer.asIntBuffer();
        file.read(byteBuffer,(long)4096 * index);
        int byteLength = intBuffer.get();
        return (byteLength/4096) + ((byteLength % 4096) != 0 ? 1 : 0);
    }
    private int findPlace(FileChannel file,MCAHeader header,int neededSize) throws IOException{
        ArrayList<Integer> offsets = header.getLocations();
        if (offsets.size() == 0) return 2;
        for(int i = 1; i < offsets.size(); i++) {
            int previousChunkOffset = offsets.get(i-1);
            int space = offsets.get(i) - previousChunkOffset;
            if (space > neededSize) {
                int previousChuckLength = getChunkLength(file,previousChunkOffset);
                if (space - previousChuckLength >= neededSize) return previousChunkOffset + previousChuckLength;
            }
        }
        int last = offsets.get(offsets.size()-1);
        return last + getChunkLength(file,last);
    }
}
