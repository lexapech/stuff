package org.lexapech;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.channels.FileChannel;
import java.util.ArrayList;
import java.util.Arrays;

import static org.lexapech.Constants.REGION_SIZE;

public class MCAHeader {
    public static final int LOCATIONS_LENGTH = 4096;
    public static final int TIMESTAMPS_LENGTH = 4096;
    private final byte[] locations;
    private final byte[] timestamps;

    public ArrayList<Integer> getLocations() {
        ArrayList<Integer> offsets = new ArrayList<>();
        for(int i = 0; i < LOCATIONS_LENGTH / Integer.BYTES ; i++) {
            int offset = getChunkOffset(i);
            if (offset > 0) {
                offsets.add(offset);
            }
        }
        offsets.sort(null);
        return offsets;
    }

    public static int coordinatesToIndex(int x, int z) {
        return ((x & (REGION_SIZE-1)) + (z & (REGION_SIZE-1)) * 32) * 4;
    }

    public MCAHeader() {
        locations = new byte[LOCATIONS_LENGTH];
        timestamps = new byte[TIMESTAMPS_LENGTH];
    }
    public MCAHeader(byte[] data) {
        locations = Arrays.copyOfRange(data,0,LOCATIONS_LENGTH);
        timestamps = Arrays.copyOfRange(data,LOCATIONS_LENGTH,LOCATIONS_LENGTH + TIMESTAMPS_LENGTH);
    }
    public MCAHeader(InputStream stream) throws IOException {
        locations = stream.readNBytes(LOCATIONS_LENGTH);
        timestamps = stream.readNBytes(TIMESTAMPS_LENGTH);
    }
    public MCAHeader(FileChannel channel) throws IOException {
        ByteBuffer locBuffer = ByteBuffer.allocate(LOCATIONS_LENGTH);
        ByteBuffer timeBuffer = ByteBuffer.allocate(TIMESTAMPS_LENGTH);
        channel.read(locBuffer);
        channel.read(timeBuffer);
        locations = locBuffer.array();
        timestamps = timeBuffer.array();
    }
    public int getChunkOffset(int x, int z) {
        int chunkRecordOffset = coordinatesToIndex(x, z);
        return ((Byte.toUnsignedInt(locations[chunkRecordOffset]) << 16)) +
                (Byte.toUnsignedInt(locations[chunkRecordOffset + 1]) << 8) +
                Byte.toUnsignedInt(locations[chunkRecordOffset + 2]);
    }
    public int getChunkOffset(int index) {
        int chunkRecordOffset = index * 4;
        return ((Byte.toUnsignedInt(locations[chunkRecordOffset]) << 16)) +
                (Byte.toUnsignedInt(locations[chunkRecordOffset + 1]) << 8) +
                Byte.toUnsignedInt(locations[chunkRecordOffset + 2]);
    }
    public int getTimestamp(int x, int z) {
        int chunkRecordOffset = coordinatesToIndex(x, z);
        return  (Byte.toUnsignedInt(locations[chunkRecordOffset]) << 24) +
                (Byte.toUnsignedInt(locations[chunkRecordOffset + 1]) << 16) +
                (Byte.toUnsignedInt(locations[chunkRecordOffset + 2]) << 8 ) +
                Byte.toUnsignedInt(locations[chunkRecordOffset + 3]);
    }
}
