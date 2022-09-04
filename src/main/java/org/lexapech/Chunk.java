package org.lexapech;

import java.util.ArrayList;

public class Chunk {

    private final ArrayList<Section> sections;
    int x,z;

    boolean changed;
    public Chunk(int x,int z,ArrayList<Section> sections) {
       this.sections = sections;
       this.x = x;
       this.z = z;
       changed=false;
    }

    public int size() {
        return sections.size();
    }

    public int getBlockId(Point3 pos) {

        Section section = sections.get((pos.y()-Constants.WORLD_BOTTOM) / 16);
        int sectionY = pos.y() & 15;
        int sectionX = pos.x() & 15;
        int sectionZ = pos.z() & 15;
        int blockPos = sectionY*256 + sectionZ*16 + sectionX;
        return section.data()[blockPos];
    }
    public String getBlock(Point3 pos) {
        BlockDictionary blockDictionary = BlockDictionary.getInstance();
        return blockDictionary.get(getBlockId(pos));
    }
    public void setBlockId(Point3 pos, int id) {

        Section section = sections.get((pos.y()-Constants.WORLD_BOTTOM) / 16);
        int sectionY = pos.y() & 15;
        int sectionX = pos.x() & 15;
        int sectionZ = pos.z() & 15;
        int blockPos = sectionY*256 + sectionZ*16 + sectionX;
        section.data()[blockPos]=id;
        changed=true;
    }
    public void setBlock(Point3 pos, String block) {
        BlockDictionary blockDictionary = BlockDictionary.getInstance();
        setBlockId(pos,blockDictionary.get(block));
    }
    public void markChanged() {
        changed = true;
    }

    public BlockIterator getXInterator(Point3 pos) {
        return new BlockIterator(this,new Point3(1,0,0),pos);
    }

    public BlockIterator getYInterator(Point3 pos) {
        return new BlockIterator(this,new Point3(0,1,0),pos);
    }
    public BlockIterator getZInterator(Point3 pos) {
        return new BlockIterator(this,new Point3(0,0,1),pos);
    }
    public Section getSection(int index) {
        return sections.get(index);
    }
}
