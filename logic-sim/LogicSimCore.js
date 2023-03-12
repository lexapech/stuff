import Element from "./Element.js";
import "./elogs_browser.min.js"
export default class LogicSimCore {

    Circuit = ELoGS.circuit;
    Const = ELoGS.constants;
    elementTypeMap = {
        "not": this.Const.MachineTruthTable.NOT,
        "and": this.Const.MachineTruthTable.AND,
        "or": this.Const.MachineTruthTable.OR,
        "0": this.Const.MachineTruthTable.OFF,
        "1": this.Const.MachineTruthTable.ON,
        'buf': {
            "0": [this.Const.Value.LOW],
            "1": [this.Const.Value.HIGH],
        }
    }

    constructor(portSize, editorMode) {
        this.portSize = portSize;
        this.elements = []
        this.wires = []
        this.editorMode = editorMode

    }

    clear(){
        this.elements = []
        this.wires = []
    }

    addElement(element){
        this.elements.push(element)
    }

    addElementFromTemplate(template){
        let name = ""
        let i = 0
        while(true) {
            name = template.type+'_'+i
            if(this.elements.find(e => e.name === name) === undefined) break;
            i++;
        }
        let newElement = new Element(template.type, name, template.pos,true, template.portSize)
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
        element.pos = {x: element.pos.x + dPos.x,
                       y: element.pos.y + dPos.y}
    }

    addWire(wire){
        this.wires.push(wire)
        return wire
    }

    moveWireEnd(wire,pos) {
        wire.endPort.pos = pos
    }

    getElementNearPos(pos) {
        return this.elements.reverse().find(e=>e.isPosInside(pos))
    }

    getPortAtPos(element,pos) {
        let ports = element.getPorts()

        for(let i = 0; i < ports.length; i++){
            let portPos = ports[i].getPos()
            if (Math.abs(portPos.x-pos.x) <= ports[i].size && Math.abs(portPos.y - pos.y) <= ports[i].size){
                return ports[i]
            }
        }
        return undefined
    }


    getConnectedWire(port){
        return this.wires.find(w=>{
            return w.endPort.element.name === port.element.name &&
                w.endPort.id === port.id &&
                w.endPort.getType(this.editorMode) === port.getType(this.editorMode) &&
                !(!this.editorMode && w.endPort.type==="common")
        })
    }

    canConnect(wire,port) {
        if(wire.startPort.element === port.element) return false;
        if(port.element.type==='bar' && !this.editorMode && port.type==="output" ) return false;
        if(!port.element.draggable && port.element.type!=='bar') return false
        if(wire.startPort.getType(this.editorMode)===port.getType(this.editorMode)) return false
        if(port.getType(this.editorMode)==="output") return true
        let connected = this.wires.find(w=>{
            if(wire===w) return false
            return w.endPort.element.name === port.element.name &&
                w.endPort.id === port.id
        })
        return !connected
    }

    setPortValues(port,value){
        let bar
        if(port === 'input'){
            bar = this.elements.find(bar => bar.name==="output_bar")
        }
        else {
            bar = this.elements.find(bar => bar.name==="input_bar")
        }
        bar.portValues = value
    }

    generateCircuit(inputValues){

        let circuit = new this.Circuit();
        this.elements.filter(element=>element.type!=="bar").forEach(element=>{
            circuit.addMachine(element.name,this.elementTypeMap[element.type])
        })
        let inputs = this.elements.find(element => element.name === "output_bar")
        inputs.getPorts().map(port=>{
            circuit.addMachine(`I${port.id}`,this.elementTypeMap[inputValues[port.id]])
        })
        let outputs = this.elements.find(element => element.name === "input_bar")
        outputs.getPorts().map(port=>{
            circuit.addMachine(`O${port.id}`,this.elementTypeMap["buf"])
        })
        let common = this.elements.find(element => element.name === "common_bar")
        common.getPorts().map(port=>{
            circuit.addMachine(`C${port.id}`,this.elementTypeMap["buf"])
        })
        this.wires.forEach(wire=>{
            let outName = wire.startPort.element.name
            let outId = wire.startPort.id
            if(outName==="output_bar"){
                outName = `I${wire.startPort.id}`
                outId = 0
            }
            else if(outName==="common_bar"){
                outName = `C${wire.startPort.id}`
                outId = 0
            }
            let inName = wire.endPort.element.name
            let inId = wire.endPort.id
            if(inName==="input_bar"){
                inName = `O${wire.endPort.id}`
                inId = 0
            }
            else if(inName==="common_bar"){
                inName = `C${wire.endPort.id}`
                inId = 0
            }

            circuit.addConnection(outName,outId,inName,inId)
        })
        return circuit;
    }

    simulate(inputValues){
        let display = false
        let currentInputValues = this.elements.find(element => element.name === "output_bar").portValues
        if(inputValues.every((v,i)=> v === currentInputValues[i])) {
            display = true
        }
        let circuit = this.generateCircuit(inputValues)
        try {
            const simulationResult = circuit.simulate();
            let outputValues = Array(inputValues.length).fill('0').map((x, i) => {
                return simulationResult.state[`O${i}`][0]
            })
            if (display) {
                let outputs = this.elements.find(element => element.name === "input_bar")
                outputs.portValues = outputValues.map(v => (+(!!v)).toString())
            }
            outputValues = outputValues.map(v => v !== undefined ? (+v).toString() : "undefined")
            return outputValues.every((v, i) => v === inputValues[i])
        }
        catch (e) {

            return false
        }

    }

}