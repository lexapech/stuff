import LogicSimView from './LogicSimView.js'
import LogicSimCore from './LogicSimCore.js'
import Element from './Element.js'
import Wire from "./Wire.js";
import Toolbar from "./Toolbar.js";

class LogicSim{

    constructor(id) {
        console.log("start")
        let canvasParent = document.querySelector(id);
        this.canvas = document.createElement('canvas');
        canvasParent.appendChild(this.canvas);


        let toolBarHeight = 100
        this.portSize = 8;
        this.editorMode=true

        this.circuit=undefined

        this.view = new LogicSimView(this);

        this.resetButton = this.createButtons(toolBarHeight,canvasParent)
        this.toolbar = new Toolbar(this.view.width*0.7,this.view.height,toolBarHeight,this.portSize)
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
        buttons.style.top=`${this.view.height-toolBarHeight/2 - buttons.clientHeight/2}px`;
        document.querySelector('#logic-sim-reset-button').addEventListener('click',(e)=>this.resetButtonPressed(e))
        document.querySelector('#logic-sim-previous-button').addEventListener('click',(e)=>this.previousButtonPressed(e))
        document.querySelector('#logic-sim-next-button').addEventListener('click',(e)=>this.nextButtonPressed(e))
        return buttons
    }

    addBar(x,type,width,ports){
        let bar = new Element('bar','',{x:x,y:(this.view.height-this.toolbar.toolbarHeight)/2},false)
        bar.initPortBar(type,width,this.view.height-this.toolbar.toolbarHeight,ports)
        this.core.addElement(bar)
    }

    loadCircuit(circuit){
        this.addBar(this.view.width-50/2,'input',50,3)
        this.addBar(50/2,'output',50,3)
        this.addBar(this.view.width/2,'common',5,3)
        this.update()
    }

    resetButtonPressed(e){
        console.log("circuit resetted")
        this.core.clear()
        this.loadCircuit(this.circuit)
    }
    previousButtonPressed(e){

    }
    nextButtonPressed(e){

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
        this.grabbedWire.endPort.pos = pos
    }

    mousePressOnScheme(pos){
        let element = this.core.getElementNearPos(pos)
        if(element) {
            let port = this.core.getPortAtPos(element,pos)

            if(port) {
                if(port.element.draggable ||
                  (port.element.type==='bar' &&
                  (this.editorMode || port.element.portType==='input'))) {
                    let connected = this.core.getConnectedWire(port)
                    if (connected) {
                        this.grabConnectedWire(connected, pos)
                    } else {
                        this.grabbedWire = this.core.addWire(new Wire(port, {type: 'mouse', pos: pos}))
                        console.log('new wire')
                    }
                }
            }
            else if(element.canDrag(pos)) {
                this.grabbedElement = element
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

    releaseGrabbedElement(pos){
        this.core.elements = this.core.elements.filter(x => x !== this.grabbedElement)
        //this.core.elements.push(Object.assign( {},this.grabbedElement))
        if(!this.isOverToolbar(this.grabbedElement.pos))
            this.core.elements.push(this.grabbedElement)
        else {
            this.core.removeWires(this.grabbedElement)
        }
        this.grabbedElement = undefined
    }
    releaseGrabbedWire(){
        this.core.wires = this.core.wires.filter(x=>x!==this.grabbedWire)
        this.grabbedWire = undefined
    }
    connectGrabbedWire(port){
        this.grabbedWire.endPort = port
        if(this.grabbedWire.endPort.type === "output"){
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
            this.update()
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
            this.update()
        }
    }

    isOverToolbar(pos){
        return pos.y > this.toolbar.height - this.toolbar.toolbarHeight
    }

    update(){
        this.view.clear()
        this.view.drawElements(this.core.elements,this.grabbedElement)
        this.view.drawToolbar(this.toolbar)
        this.view.drawWires(this.core.wires)
        if(this.grabbedElement)
            this.view.drawElement(this.grabbedElement)
    }
}

new LogicSim('#logic-sim-container')