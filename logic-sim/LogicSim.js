import LogicSimView from './LogicSimView.js'
import LogicSimCore from './LogicSimCore.js'
import Element from './Element.js'
import Wire from "./Wire.js";
import Toolbar from "./Toolbar.js";
import Port from "./Port.js";
import BarElement from "./BarElement.js";
import circuits from "./Circuits.js";

export default class LogicSim{

    constructor(id,testCallback,editorMode) {
        console.log("start")
        this.testCallback = testCallback
        let canvasParent = document.querySelector(id);
        this.canvas = document.createElement('canvas');
        canvasParent.appendChild(this.canvas);


        let toolBarHeight = 100
        this.portSize = 8;
        this.editorMode=!!editorMode

        this.circuit=circuits[0]
        this.testSetIndex = 0
        this.view = new LogicSimView(this);

        this.createButtons(toolBarHeight,canvasParent)
        this.toolbar = new Toolbar(this.view.width*0.7, this.view.height, toolBarHeight, this.portSize)
        this.toolbar.addElement('not')
        this.toolbar.addElement('and')
        this.toolbar.addElement('or')

        this.core = new LogicSimCore(this.portSize,this.editorMode)
        this.initMouse()
        this.loadCircuit(this.circuit)

        //this.core.addElement(new Element('not','not1',{x:100,y:100},false))
        //this.core.addElement(new Element('and','and1',{x:400,y:100},false))

    }

    createButtons(toolBarHeight,parent){
        let buttons = document.createElement('div');
        parent.appendChild(buttons);
        buttons.style.position="absolute";
        buttons.style.padding="10px";
        buttons.style.display="flex";

        buttons.style.left="70%";
        buttons.innerHTML=`
            <button id="logic-sim-reset-button">CБРОС</button>
            <button id="logic-sim-previous-button">предыдущий входной набор</button>
            <button id="logic-sim-next-button">следующий входной набор</button>`
        if(this.editorMode===true){
            buttons.innerHTML+=`<button id="logic-sim-save-button">сохранить</button>`
        }
        buttons.style.top=`${this.view.height-toolBarHeight/2 - buttons.clientHeight/2}px`;
        document.querySelector('#logic-sim-reset-button').addEventListener('click',(e)=>this.resetButtonPressed(e))
        document.querySelector('#logic-sim-previous-button').addEventListener('click',(e)=>this.previousButtonPressed(e))
        document.querySelector('#logic-sim-next-button').addEventListener('click',(e)=>this.nextButtonPressed(e))
        if(this.editorMode===true) {
            document.querySelector('#logic-sim-save-button').addEventListener('click', (e) => this.saveButtonPressed(e))
        }
    }

    addBar(x,type,width,ports,portSize){
        let bar = new BarElement(type,
            {x:x,y:(this.view.height-this.toolbar.toolbarHeight)/2},
            width,
            this.view.height-this.toolbar.toolbarHeight,
            ports,
            portSize)
        this.core.addElement(bar)
    }

    loadCircuit(circuit){
        circuit.elements.forEach(element =>{
            this.core.addElement(new Element(element.type,
                element.name,
                {
                    x:element.pos.x * this.view.width,
                    y:element.pos.y * this.view.height
                },
                this.editorMode,
                this.portSize))
        })

        this.addBar(this.view.width-50/2,'input',50,circuit.ports,this.portSize*1.5)
        this.addBar(50/2,'output',50,circuit.ports,this.portSize*1.5)
        this.addBar(this.view.width/2,'common',5,circuit.ports,this.portSize)

        circuit.wires.forEach(wire => {
            let startPort = this.findPort(wire.startPort)
            let endPort = this.findPort(wire.endPort)
            let wire1 = new Wire(startPort,endPort)
            this.core.addWire(wire1)
        })
        this.core.setPortValues("input",this.testToArray(this.circuit.tests[this.testSetIndex]))
        this.update(true)
    }

    findPort(port){
        let element = this.core.elements.find(e => e.name === port.element)
        let ports = element.getPorts()
        return ports.find(p => p.type === port.type && p.id === port.id)
    }

    testToArray(value){
        let str = value.toString(2)
        return Array(this.circuit.ports - str.length).fill("0").concat([...str])
    }

    resetButtonPressed(){
        console.log("circuit cleared")
        this.core.clear()
        this.loadCircuit(this.circuit)
    }
    previousButtonPressed(){
        this.testSetIndex--
        if(this.testSetIndex<0) this.testSetIndex = this.circuit.tests.length-1
        this.core.setPortValues("input",this.testToArray(this.circuit.tests[this.testSetIndex]))
        this.update(true)
    }
    nextButtonPressed(){
        this.testSetIndex++
        if(this.testSetIndex===this.circuit.tests.length) this.testSetIndex = 0
        this.core.setPortValues("input",this.testToArray(this.circuit.tests[this.testSetIndex]))
        this.update(true)
    }
    saveButtonPressed(){
        console.log(JSON.stringify(this.core.elements.filter(e=>e.type!=="bar").map(element=>{
            return {
                name: element.name,
                type: element.type,
                pos: {
                    x: element.pos.x / this.view.width,
                    y: element.pos.y / this.view.height
                }
            }
        })))
        console.log(JSON.stringify(this.core.wires.map(wire => {
            return {
                startPort:{
                    element:wire.startPort.element.name,
                    type:wire.startPort.type,
                    id:wire.startPort.id
                },
                endPort:{
                    element:wire.endPort.element.name,
                    type:wire.endPort.type,
                    id:wire.endPort.id
                }
            }
        })))
    }

    initMouse(){
        this.mousePressed = false
        this.canvas.addEventListener("mousedown",(e)=>this.mouseDown(e))
        this.canvas.addEventListener("mousemove",(e)=>this.mouseMove(e))
        this.canvas.addEventListener("mouseup",(e)=>this.mouseUp(e))
    }
    getCanvasCoordinates(pos) {
        let canvasBox = this.canvas.getBoundingClientRect()
        return {
            x: pos.x-canvasBox.x,
            y: pos.y-canvasBox.y,
        }
    }

    grabConnectedWire(connected,pos){
        this.grabbedWire = connected
        let mousePort = new Port(undefined,'mouse',0)
        mousePort.pos = pos
        this.grabbedWire.endPort = mousePort
    }

    mousePressOnScheme(pos){
        let element = this.core.getElementNearPos(pos)
        if(element) {
            let port = this.core.getPortAtPos(element,pos)

            if(port) {
                if(port.element.draggable ||
                  (port.element.type==='bar' && (port.type === 'common' || ((this.editorMode && port.type === 'output') || (!this.editorMode && port.type === 'input'))))) {
                    let connected = this.core.getConnectedWire(port)
                    if (connected) {
                        this.grabConnectedWire(connected, pos)
                    } else {
                        let mousePort = new Port(undefined,'mouse',0)
                        mousePort.pos = pos
                        this.grabbedWire = this.core.addWire(new Wire(port, mousePort))
                        console.log('new wire')
                    }
                }
            }
            else if(element.canDrag(pos)) {
                this.grabbedElement = element
                this.dragStartPos = element.pos
            }
        }
    }

    mousePressOnToolbar(pos){
        let element = this.toolbar.getElementNearPos(pos)
        if(element)
            this.grabbedElement = this.core.addElementFromTemplate(element)
    }

    mouseDown(e){
        this.mousePressed = true
        let pos = this.getCanvasCoordinates(e)
        if(this.isOverToolbar(pos)) {
            this.mousePressOnToolbar(pos)
        }
        else {
            this.mousePressOnScheme(pos)
        }

    }

    mouseMove(e){
        let pos = this.getCanvasCoordinates(e)
        if(this.mousePressed){
            let dPos = {x:pos.x-this.prevPos.x,y:pos.y-this.prevPos.y}
            if(this.grabbedElement) {
                this.core.moveElement(this.grabbedElement,dPos)
                this.update()
            }
            if(this.grabbedWire) {
                this.core.moveWireEnd(this.grabbedWire,pos)
                this.update()
            }
        }
        this.prevPos = pos
    }

    releaseGrabbedElement(){
        this.core.elements = this.core.elements.filter(x => x !== this.grabbedElement)
        if(!this.isOverToolbar(this.grabbedElement.pos)) {
            if (!this.canPlaceHere(this.grabbedElement)) {
                if(this.dragStartPos) {
                    this.grabbedElement.pos = this.dragStartPos
                }
                else {
                    this.grabbedElement = undefined
                }
            }
            if(this.grabbedElement)
                this.core.elements.push(this.grabbedElement)
            this.update()
        }
        else {
            this.core.removeWires(this.grabbedElement)
            this.update(true)
        }
        this.dragStartPos = undefined
        this.grabbedElement = undefined
    }
    releaseGrabbedWire(){
        this.core.wires = this.core.wires.filter(x=>x!==this.grabbedWire)
        this.grabbedWire = undefined
    }
    connectGrabbedWire(port){
        this.grabbedWire.endPort = port
        if(this.grabbedWire.endPort.getType(this.editorMode) === "output"){
            let t = this.grabbedWire.endPort;
            this.grabbedWire.endPort = this.grabbedWire.startPort;
            this.grabbedWire.startPort = t;
            console.log('connected')
        }
        this.grabbedWire = undefined
    }
    mouseUp(e){
        let pos = this.getCanvasCoordinates(e)
        this.mousePressed = false
        if(this.grabbedElement) {
            this.releaseGrabbedElement(pos)

        }
        if(this.grabbedWire) {
            let element = this.core.getElementNearPos(pos)
            if(element) {
                let port = this.core.getPortAtPos(element,pos)
                if(port && this.core.canConnect(this.grabbedWire,port)){
                    this.connectGrabbedWire(port)
                }
                else {
                    this.releaseGrabbedWire()
                }
            }
            else {
                this.releaseGrabbedWire()
            }
            this.update(true)
        }
    }

    isOverToolbar(pos){
        return pos.y > this.toolbar.height - this.toolbar.toolbarHeight
    }
    canPlaceHere(element){
        let size = element.getSize()
        if(this.editorMode) {
            return element.pos.x + size.width/2 < this.view.width/2
        }
        else {
            return element.pos.x - size.width/2 >= this.view.width/2
        }
    }

    runAllTests(){
        let passed = 0;
        let failed = []
        for(let i=0;i<this.circuit.tests.length;i++) {
            let res = this.core.simulate(this.testToArray(this.circuit.tests[i]))
            passed+= res
            if(!res) failed.push(this.testToArray(this.circuit.tests[i]).join(''))
        }
        let passedFraction = passed / this.circuit.tests.length
        let elementsUsed = this.core.elements.filter(e=>e.draggable===true).length
        if(this.testCallback)
            this.testCallback({passedFraction:passedFraction,elementsUsed:elementsUsed,failed:failed})
    }

    update(simulate){
        if(simulate) {
            this.runAllTests()
        }

        this.view.clear()
        this.view.drawElements(this.core.elements,this.grabbedElement)
        this.view.drawToolbar(this.toolbar)
        this.view.drawWires(this.core.wires)
        this.view.drawPorts(this.core.elements,this.grabbedElement)
        if(this.grabbedElement) {
            this.view.drawElement(this.grabbedElement)
            let ports = this.grabbedElement.getPorts()
            ports.forEach(port => this.view.drawPort(port))
        }

    }
}

