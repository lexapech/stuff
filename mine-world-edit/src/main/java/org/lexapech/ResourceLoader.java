package org.lexapech;

import java.io.File;
import java.io.FileNotFoundException;
import java.net.URL;

public class ResourceLoader {

    public static File[] getResourceFolderFiles (String folder) throws FileNotFoundException {
        ClassLoader loader = Thread.currentThread().getContextClassLoader();
        URL url = loader.getResource(folder);
        if (url==null) throw new FileNotFoundException();
        String path = url.getPath();
        return new File(path).listFiles();
    }

}
