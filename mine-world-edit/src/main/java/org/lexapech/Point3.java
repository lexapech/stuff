package org.lexapech;

public record Point3(int x, int y, int z) {
    public static Point3 add(Point3 a, Point3 b) {
        return new Point3(a.x+b.x,a.y+b.y,a.z+b.z);
    }
    public static Point3 subtract(Point3 a, Point3 b) {
        return new Point3(a.x-b.x,a.y-b.y,a.z-b.z);
    }
}
