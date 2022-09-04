package org.lexapech;

import java.util.HashMap;

public class World {
    HashMap<Point2,Chunk> chunks;

    public World() {
        chunks = new HashMap<>();
    }
    public Chunk getChunk(Point2 pos) {
        return chunks.get(pos);
    }
    public Chunk getChunkWithBlock(Point3 pos) {
        return getChunk(Utils.blockToChunk(pos));
    }
    public void addChunk(Chunk chunk) {
        chunks.put(new Point2(chunk.x, chunk.z),chunk);
    }
    public Integer getBlockId(Point3 pos) {
        Chunk chunk = getChunkWithBlock(pos);
        return chunk.getBlockId(pos);
    }
    public String getBlock(Point3 pos) {
        Chunk chunk = getChunkWithBlock(pos);
        return chunk.getBlock(pos);
    }
    public void setBlockId(Point3 pos,int id) {
        Chunk chunk = getChunkWithBlock(pos);
        chunk.setBlockId(pos,id);
    }
    public void setBlock(Point3 pos,String block) {
        Chunk chunk = getChunkWithBlock(pos);
        chunk.setBlock(pos,block);
    }
}
