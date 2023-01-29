package org.lexapech;

public class Utils {

    public static String getNameWithoutExtension(String fileName) {
        if(fileName.indexOf(".")>0) {
            return fileName.substring(0, fileName.lastIndexOf("."));
        }
        else {
            return fileName;
        }
    }
    public static int pointToIndex(Point3 pos) {
        return pos.y()*16*16 + pos.z()*16 + pos.x();
    }
    public static Point2 blockToChunk(Point3 pos) {
        return new Point2(pos.x()/16,pos.z()/16);
    }
    public static Point3 chunkToBlock(Point2 pos) {
        return new Point3(pos.x()*16,0,pos.y()*16);
    }
}
