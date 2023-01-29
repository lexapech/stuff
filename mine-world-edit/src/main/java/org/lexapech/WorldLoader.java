package org.lexapech;

import java.io.IOException;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Map;
import java.util.Random;

public class WorldLoader {
    Path worldPath;
    public void load(Path world) throws IOException{
        worldPath = world;
        NBTTag level = NBTReader.read(worldPath.resolve("level.dat"));
        Chunk c = loadChunk(-1,0);
        saveChunk(c);
    }

    private Chunk loadChunk(int x, int z) throws IOException {
        MCAReader reader = new MCAReader(worldPath.resolve("region"));
        NBTTag chunk = reader.read(x,z);
        return new Chunk(x, z, loadSections(chunk));
    }

    private ArrayList<Chunk> loadChunks(ArrayList<Point2> chunks) throws IOException {
        MCAReader reader = new MCAReader(worldPath.resolve("region"));
        ArrayList<Map.Entry<Point2,NBTTag>> chunkTags = reader.readBatch(chunks);
        ArrayList<Chunk> loadedChunks = new ArrayList<>();
        for (Map.Entry<Point2,NBTTag> chunk : chunkTags) {
            Point2 pos = chunk.getKey();
            loadedChunks.add(new Chunk(pos.x(), pos.y(), loadSections(chunk.getValue())));
        }
        return loadedChunks;
    }


    private void saveChunk(Chunk chunk) throws IOException{
        MCAReader reader = new MCAReader(worldPath.resolve("region"));
        NBTTag chunkTag = reader.read(chunk.x, chunk.z);
        for (int y = 0; y < chunk.size(); y++) {
            saveSection(chunk.getSection(y),chunkTag);
        }
        MCAWriter writer = new MCAWriter(worldPath.resolve("region"));
        writer.write(chunk.x, chunk.z, chunkTag);
    }

    private void saveSection(Section section,NBTTag chunk) {
        NBTTag newState = BlockStatesConverter.pack(section);
        NBTTag sections = chunk.getNodeWithName("/sections");
        NBTTag sectionTag = sections.getNodes().get(section.y());
        sectionTag.deleteNodeWithName("block_states");
        sectionTag.addNode(newState);
    }


    private ArrayList<Section> loadSections(NBTTag chunk) {

        String air = "minecraft:air";
        BlockDictionary blockDictionary = BlockDictionary.getInstance();
        ArrayList<Section> sectionList = new ArrayList<>();
        NBTTag sections = chunk.getNodeWithName("/sections");

        for (int y = 0; y < sections.getNodes().size(); y++) {
            NBTTag section = sections.getNodes().get(y);
            System.out.println("[WorldLoader] loading section " + y);
            int[] blocks = BlockStatesConverter.unpack(section.getNodeWithName("block_states"));
            sectionList.add(new Section(y,blocks));
        }
        return sectionList;
    }

}
