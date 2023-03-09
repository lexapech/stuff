import Port from "./Port.js";
import Element from "./Element.js";
export default class LogicSimCore {
    constructor(portSize,editorMode) {
        this.portSize = portSize;
        this.elements = []
        this.wires=[]
        this.editorMode = editorMode
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
        if(port.type==="output") return element.getOutputPortsPos()[port.id]
        return element.getInputPortsPos()[port.id]
    }

    getElementNearPos(pos) {
        return this.elements.slice().reverse().find(e=>{
            let size = e.getSize()
            let tl = {x:e.pos.x - size.width/2 - this.portSize,y:e.pos.y-size.height/2}
            let br = {x:e.pos.x + size.width/2 + this.portSize,y:e.pos.y+size.height/2}
            return (pos.x >= tl.x && pos.x <= br.x) && (pos.y >= tl.y && pos.y <= br.y)
        })
    }

    getPortAtPos(element,pos) {
        let inputs = element.getInputPortsPos()
        let outputs = element.getOutputPortsPos()
        for(let i=0;i<inputs.length;i++){
            if (Math.abs(inputs[i].x-pos.x)<=this.portSize && Math.abs(inputs[i].y-pos.y)<=this.portSize){
                return new Port(element,"input",i)
            }
        }
        for(let i=0;i<outputs.length;i++){
            if (Math.abs(outputs[i].x-pos.x)<=this.portSize && Math.abs(outputs[i].y-pos.y)<=this.portSize){
                return new Port(element,"output",i)
            }
        }
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
        if(this.editorMode) return true
        if(!port.element.draggable && port.element.type!=='bar') return false
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