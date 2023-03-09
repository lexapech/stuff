import Port from "./Port.js";
import Element from "./Element.js";
export default class LogicSimCore {
    constructor(portSize) {
        this.portSize = portSize;
        this.elements = []
        this.wires=[]
    }

    addElement(element){
        this.elements.push(element)
    }

    addElementFromTemplate(template){
        let name = ""
        let i=0
        while(true) {
            name=template.type+'_'+i
            if(this.elements.find(e=>e.name===name)===undefined) break;
            i++;
        }
        let newElement = new Element(template.type,name,template.pos,true)
        this.addElement(newElement)
        return newElement
    }

    removeWires(element){
        this.wires = this.wires.filter(wire=>{
            return wire.startPort.element.name !== element.name &&
                   wire.endPort.element.name !== element.name
        })
    }

    moveElement(element,dPos){
        element.pos = {x:element.pos.x+dPos.x,y:element.pos.y+dPos.y}
    }

    addWire(wire){
        this.wires.push(wire)
        return wire
    }

    moveWireEnd(wire,pos) {
        wire.endPort.pos=pos
    }

    getPortPos(port){
        if(port.pos) return port.pos
        let element = this.elements.find(x=>x.name===port.element.name)
        if (!element) {
            console.error("element not found")
            return {x:0,y:0}
        }
        let inputs = element.getInputPortsPos()
        let output = element.getOutputPortPos()
        if(port.type==="output") return output
        return inputs[port.id]
    }

    getElementNearPos(pos) {
        return this.elements.reverse().find(e=>{
            let size = e.getSize()
            let tl = {x:e.pos.x - size.width/2 - this.portSize,y:e.pos.y-size.height/2}
            let br = {x:e.pos.x + size.width/2 + this.portSize,y:e.pos.y+size.height/2}
            return (pos.x >= tl.x && pos.x <= br.x) && (pos.y >= tl.y && pos.y <= br.y)
        })
    }

    getPortAtPos(element,pos) {
        let inputs = element.getInputPortsPos()
        let output = element.getOutputPortPos()
        let type = undefined
        let id = undefined
        if (Math.abs(inputs[0].x-pos.x)<=this.portSize && Math.abs(inputs[0].y-pos.y)<=this.portSize){
            type="input"
            id=0
        }
        else if(inputs[1] && Math.abs(inputs[1].x-pos.x)<=this.portSize && Math.abs(inputs[1].y-pos.y)<=this.portSize){
            type="input"
            id=1
        }
        else if (Math.abs(output.x-pos.x)<=this.portSize && Math.abs(output.y-pos.y)<=this.portSize){
            type="output"
            id=0
        }
        if(type)
            return new Port(element,type,id)
        else
            return undefined
    }

    getConnectedWire(port){
        return this.wires.find(w=>{
            return w.endPort.element.name === port.element.name &&
                w.endPort.id === port.id &&
                w.endPort.type === port.type
        })
    }

    canConnect(wire,port) {
        if(!port.element.draggable) return false
        if(wire.startPort.type===port.type) return false
        if(port.type==="output") return true
        let connected = this.wires.find(w=>{
            if(wire===w) return false
            return w.endPort.element.name === port.element.name &&
                w.endPort.id === port.id
        })
        return !connected
    }
}