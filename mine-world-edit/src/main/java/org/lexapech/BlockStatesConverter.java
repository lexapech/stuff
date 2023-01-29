package org.lexapech;

import java.util.*;

public class BlockStatesConverter {

    public static int[] unpack(NBTTag blockStates) {

        ArrayList<String> paletteBlocks = new ArrayList<>();
        int[] blocks = new int[Constants.SECTION_BLOCKS];
        BlockDictionary blockDictionary = BlockDictionary.getInstance();
        NBTTag palette = blockStates.getNodeWithName("palette");
        for (NBTTag block : palette.getNodes()) {
            NBTTag name = block.findNodeWithName("Name");
            paletteBlocks.add((String) name.getData());
        }
        int paletteSize = palette.getNodes().size();
        System.out.println("[BlockStates Unpack] blocks in palette: " + paletteSize);
        if (paletteSize==1) {
            Arrays.fill(blocks,blockDictionary.get(paletteBlocks.get(0)));
            return blocks;
        }
        NBTTag data = blockStates.getNodeWithName("data");
        long[] dataArray = (long[]) data.getData();

        int minimumBits = Math.max(4, (int) Math.ceil(Math.log(paletteSize) / Math.log(2)));
        System.out.println("[BlockStates Unpack] bit/block: " + minimumBits);
        int blockMask = (1 << minimumBits) - 1;
        int blocksPerLong = 64 / minimumBits;

        for (int blockIndex = 0, longIndex = 0; blockIndex < Constants.SECTION_BLOCKS; longIndex++) {
            long longValue = dataArray[longIndex];
            for (int i = 0; i < blocksPerLong && blockIndex < Constants.SECTION_BLOCKS; i++) {
                blocks[blockIndex] = (int) (longValue >> (i * minimumBits)) & blockMask;
                blockIndex++;
            }
        }

        for (int i = 0; i < Constants.SECTION_BLOCKS; i++) {
            String name = paletteBlocks.get(blocks[i]);
            blocks[i] = blockDictionary.get(name);
        }
        return blocks;
    }
    public static NBTTag pack(Section section) {
        HashSet<Integer> palette = new HashSet<>();
        for(Integer id : section.data()) {
            palette.add(id);
        }
        System.out.println("[BlockStates Pack] palette size: " + palette.size());
        BlockDictionary blockDictionary = BlockDictionary.getInstance();
        ArrayList<String> paletteBlocks = new ArrayList<>();
        HashMap<Integer,Integer> map = new HashMap<>();
        int newId=0;
        for (Integer id : palette) {
            map.put(id,newId);
            paletteBlocks.add(blockDictionary.get(id));
            newId++;
        }
        long[] longData=null;
        if (palette.size()!=1) {

            int minimumBits = Math.max(4, (int) Math.ceil(Math.log(paletteBlocks.size()) / Math.log(2)));
            System.out.println("[BlockStates Pack] bits/block: " + minimumBits);
            //int blockMask = (1 << minimumBits) - 1;
            int blocksPerLong = Long.SIZE / minimumBits;

            int[] data = Arrays.stream(section.data()).map(map::get).toArray();
            int longArrayLength = (int) Math.ceil((double) Constants.SECTION_BLOCKS / blocksPerLong);
            longData = new long[longArrayLength];
            for (int blockIndex = 0, longIndex = 0; blockIndex < Constants.SECTION_BLOCKS; longIndex++) {
                long longValue = 0;
                for (int i = 0; i < blocksPerLong && blockIndex < Constants.SECTION_BLOCKS; i++) {
                    longValue |= (long) data[blockIndex] << (i * minimumBits);
                    blockIndex++;
                }
                longData[longIndex] = longValue;
            }
        }
        NBTTag blockStates = new NBTTag(NBTTag.TagID.TAG_Compound,"block_states");

        NBTTag paletteTag = new NBTTag(NBTTag.TagID.TAG_List,"palette");
        for (String block : paletteBlocks) {
            NBTTag paletteEntry = new NBTTag(NBTTag.TagID.TAG_Compound,"");
            paletteEntry.addNode(new NBTTag(NBTTag.TagID.TAG_String,"Name",block));
            paletteTag.addNode(paletteEntry);
        }
        if (palette.size()!=1) blockStates.addNode(new NBTTag(NBTTag.TagID.TAG_Long_Array,"data",longData));
        blockStates.addNode(paletteTag);
        System.out.println("[BlockStates Pack] tag created");
        return blockStates;
    }
}
