package org.lexapech;

import java.io.IOException;
import java.io.InputStream;
import java.math.BigInteger;
import java.nio.file.Path;
import java.util.zip.GZIPInputStream;
import java.util.zip.Inflater;
import java.util.zip.InflaterInputStream;

import static java.nio.file.Files.newInputStream;

public class NBTReader {

    public static NBTTag read(Path file) throws IOException {
        try (InputStream fileStream = newInputStream(file)) {
                try (GZIPInputStream decompress = new GZIPInputStream(fileStream)) {
                    NBTTag tag = NBTParser.parse(decompress);
                    System.out.println("[NBT Reader] tag parsed");
                    return tag;
                }
            }
        }
    }
