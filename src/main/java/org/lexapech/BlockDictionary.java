package org.lexapech;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.ArrayList;
import java.util.HashMap;

public class BlockDictionary {
    private final HashMap<String,Integer> map;
    private final ArrayList<String> list;
    private static final BlockDictionary instance = new BlockDictionary();
    private BlockDictionary() {
        map = new HashMap<>();
        list = new ArrayList<>();
    }

    public static BlockDictionary getInstance() {
        return instance;
    }
    public void add(String blockName) {
        map.put(blockName,list.size());
        list.add(blockName);
    }
    public int get(String name) {
        Integer value = map.get(name);
        if (value == null) throw new NullPointerException(name);
        return value;
    }
    public String get(Integer id) {
        return list.get(id);
    }
    public int size() {
        return list.size();
    }
    public void loadFromFolder(String folder,String prefix) throws FileNotFoundException {
        File[] blockFiles = ResourceLoader.getResourceFolderFiles(folder);

        System.out.println("[BlockDictionary] "+blockFiles.length+" blocks found in resources");

        for (File blockFile : blockFiles) {
            String fileName = Utils.getNameWithoutExtension(blockFile.getName());
            this.add(prefix+":"+fileName);
        }
        System.out.println("[BlockDictionary] "+this.size() + " blocks added to dictionary");
    }
}
