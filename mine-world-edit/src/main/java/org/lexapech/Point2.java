package org.lexapech;

public record Point2(int x, int y) {
    public static Point2 add(Point2 a, Point2 b) {
        return new Point2(a.x+b.x,a.y+b.y);
    }
    public static Point2 subtract(Point2 a, Point2 b) {
        return new Point2(a.x-b.x,a.y-b.y);
    }
}
