import Port from "./Port.js";

export default class BarElement{
    constructor(portType, pos, width, height, portCount, portSize) {
        this.name = portType+'_bar'
        this.type = 'bar'
        this.portType = portType
        this.pos = pos
        this.width = width
        this.height = height
        this.portCount = portCount
        this.draggable = false
        this.portSize = portSize
        this.ports=[]
        this.portValues=Array(portCount).fill(0)
        this.portValid=Array(portCount).fill(0)
    }

    getSize(){
        return {width: this.width, height: this.height}
    }

    canDrag() {
        return false
    }

    getPortCount() {
        if(this.portType==='input')
            return {input:this.portCount, output:0};
        else if (this.portType==='output'){
            return {input:0, output:this.portCount};
        }
        else {
            return {input:this.portCount, output:this.portCount};
        }
    }

    getPorts(){
        if(this.ports.length>0) return this.ports
        this.ports = Array(this.portCount).fill(0).map((x,i)=>{return new Port(this, this.portType, i, this.portSize)})
        return this.ports
    }

    isPosInside(pos) {
        let size = this.getSize()
        let tl = {x: this.pos.x - size.width/2 - this.portSize, y: this.pos.y - size.height/2}
        let br = {x: this.pos.x + size.width/2 + this.portSize, y: this.pos.y + size.height/2}
        return (pos.x >= tl.x && pos.x <= br.x) && (pos.y >= tl.y && pos.y <= br.y)
    }

}