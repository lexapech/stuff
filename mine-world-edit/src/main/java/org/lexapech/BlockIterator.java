package org.lexapech;

import java.util.ListIterator;

public class BlockIterator implements ListIterator<Integer> {
    private final int step;
    private final Chunk chunk;
    private int currentIndex;
    private Point3 currentPos;
    private Section currentSection;
    private final Point3 delta;
    public BlockIterator(Chunk chunk, Point3 delta, Point3 pos) {
        this.step = Utils.pointToIndex(delta);
        currentIndex = Utils.pointToIndex(pos);
        this.chunk = chunk;
        this.delta=delta;
        currentPos=pos;
        currentSection = chunk.getSection((currentPos.y()+64) / 16);
    }
    @Override
    public boolean hasNext() {
        Point3 nextPos = Point3.add(currentPos,delta);
        return     (nextPos.x()>=0 && nextPos.x()<16 &&
                    nextPos.y()>=-64 && nextPos.y()<320 &&
                    nextPos.z()>=0 && nextPos.z()<16);
    }

    @Override
    public Integer next() {
        currentPos = Point3.add(currentPos,delta);
        currentIndex += step;
        boolean changeSection=false;
        while (currentIndex<0) {
            currentIndex+=4096;
            changeSection=true;
        }
        while (currentIndex>4095) {
            currentIndex-=4096;
            changeSection=true;
        }
        if (changeSection) {
            currentSection = chunk.getSection((currentPos.y()+64) / 16);
        }
        return  currentSection.data()[currentIndex];
    }

    @Override
    public boolean hasPrevious() {
        Point3 nextPos = Point3.subtract(currentPos,delta);
        return     (nextPos.x()>=0 && nextPos.x()<16 &&
                nextPos.y()>=-64 && nextPos.y()<320 &&
                nextPos.z()>=0 && nextPos.z()<16);
    }

    public Integer current() {
        return  currentSection.data()[currentIndex];
    }

    @Override
    public Integer previous() {
        currentPos = Point3.subtract(currentPos,delta);
        currentIndex -= step;
        boolean changeSection=false;
        while (currentIndex<0) {
            currentIndex+=4096;
            changeSection=true;
        }
        while (currentIndex>4095) {
            currentIndex-=4096;
            changeSection=true;
        }
        if (changeSection) {
            currentSection = chunk.getSection((currentPos.y()+64) / 16);
        }
        return  currentSection.data()[currentIndex];
    }

    @Override
    public int nextIndex() {
        return 0;
    }

    @Override
    public int previousIndex() {
        return 0;
    }

    @Override
    public void remove() {

    }

    @Override
    public void set(Integer integer) {
        currentSection.data()[currentIndex] = integer;
        chunk.markChanged();
    }

    @Override
    public void add(Integer integer) {

    }
}
