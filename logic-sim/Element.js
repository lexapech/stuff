
export default class Element{
    constructor(type,name,pos,draggable) {
        this.name = name
        this.type = type
        this.pos = pos
        this.draggable = draggable
    }

    getSize(){
        if(this.type!=='not')
            return {width: 160, height: 80}
        else {
            return {width: 160, height: 40}
        }
    }

    canDrag(pos) {
        if(this.draggable!==true) return false;
        let size = this.getSize()
        let tl = {x:this.pos.x - size.width/2,y:this.pos.y-size.height/2}
        let br = {x:this.pos.x + size.width/2,y:this.pos.y+size.height/2}
        return (pos.x >= tl.x && pos.x < br.x) && (pos.y >= tl.y && pos.y < br.y)
    }

    getInputPortCount() {
        if(this.type!=='not')
            return 2;
        else {
            return 1;
        }
    }
    getInputPortsPos(){
        let iPorts = this.getInputPortCount()
        let elementSize = this.getSize()
        if(iPorts===1){
            return [{x: this.pos.x-elementSize.width/2,y: this.pos.y}]
        }
        else {
            return [
                {x: this.pos.x - elementSize.width/2, y: this.pos.y - elementSize.height/4},
                {x: this.pos.x - elementSize.width/2, y: this.pos.y + elementSize.height/4}
            ]
        }
    }

    getOutputPortPos(){
        let elementSize = this.getSize()
        return {x: this.pos.x+elementSize.width/2,y: this.pos.y}
    }
}