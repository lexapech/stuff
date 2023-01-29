package org.lexapech;


import com.google.gson.Gson;
import com.google.gson.internal.LinkedTreeMap;

import java.io.*;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.logging.Logger;

import static java.nio.file.Files.newInputStream;

public class Main {

    public static void main(String[] args) {
        try {

            BlockDictionary blockDictionary = BlockDictionary.getInstance();
            blockDictionary.loadFromFolder("blockstates","minecraft");

            WorldLoader worldLoader = new WorldLoader();
            worldLoader.load(Path.of("D:/123/testworld"));

        }
        catch(IOException e) {
            e.printStackTrace();
        }

    }
}