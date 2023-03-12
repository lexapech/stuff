import Port from "./Port.js";

export default class Element{
    constructor(type, name, pos, draggable, portSize) {
        this.name = name
        this.type = type
        this.pos = pos
        this.draggable = draggable
        this.ports = []
        this.portSize = portSize
    }

    getSize(){
        if(this.type==='not')
            return {width: 80, height: 60}
        else {
            return {width: 80, height: 60}
        }
    }

    canDrag(pos) {
        if(this.draggable !== true) return false;
        let size = this.getSize()
        let tl = {x: this.pos.x - size.width / 2, y: this.pos.y - size.height / 2}
        let br = {x: this.pos.x + size.width / 2, y: this.pos.y + size.height / 2}
        return (pos.x >= tl.x && pos.x < br.x) && (pos.y >= tl.y && pos.y < br.y)
    }

    getPortCount() {
        if(this.type==='not')
            return {input:1, output:1};
        else {
            return {input:2, output:1};
        }
    }

    getPorts(){
        if(this.ports.length>0) return this.ports
        if(this.type==='not'){
            this.ports = [
                new Port(this,'input',0,this.portSize),
                new Port(this,'output',0,this.portSize)
            ]
        }
        else{
            this.ports = [
                new Port(this,'input',0,this.portSize),
                new Port(this,'input',1,this.portSize),
                new Port(this,'output',0,this.portSize)
            ]
        }
        return this.ports
    }

    isPosInside(pos) {
        let size = this.getSize()
        let tl = {x: this.pos.x - size.width/2 - this.portSize, y: this.pos.y - size.height/2}
        let br = {x: this.pos.x + size.width/2 + this.portSize, y: this.pos.y + size.height/2}
        return (pos.x >= tl.x && pos.x <= br.x) && (pos.y >= tl.y && pos.y <= br.y)
    }

}