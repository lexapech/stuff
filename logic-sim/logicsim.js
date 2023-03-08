
function onload(){
    let sim = new LogicSim()
}

class ToolBar{
    constructor() {
        this.elements=[]
    }
    addElement(element,count){
        this.elements.push({name: element, count:count})
    }
    getElements(){
        return this.elements
    }
    takeElement(element){
        let e = this.elements.findIndex(x=>x.name === element)
        if(e!==-1 && this.elements[e].count!==0) {
            if(this.elements[e].count>0)
                this.elements[e].count--;
            return e;
        }
    }
}




class LogicSim{

    constructor() {

        console.log("start")
        this.initCanvas()
        this.initMouse()
        this.elements = []
        this.wires=[]
        this.addElement({x:100,y:100},'not1','not',true)
        this.addElement({x:300,y:150},'and1','and',true)
        this.update()
    }
    initCanvas(){
        let canvasParent = document.querySelector('#logic-sim');
        let canvas = document.createElement('canvas');
        canvasParent.appendChild(canvas);
        canvas.style.width="100%";
        canvas.style.height="100%";
        canvas.width = canvas.clientWidth
        canvas.height = canvas.clientHeight
        canvas.style.imageRendering="crisp-edges"
        this.canvas = canvas
        this.width = canvas.clientWidth
        this.height = canvas.clientHeight

        this.portSize = 8;
        this.wireWidth = 2;

        if(canvas.getContext('2d')) {
            this.ctx = canvas.getContext('2d');
            this.ctx.fillStyle="black";
            this.ctx.textAlign="center"
            this.ctx.textBaseline="middle"
            this.ctx.font = "24px serif";


        }

    }

    getCanvasCoordinates(pos) {
        let canvasBox = this.canvas.getBoundingClientRect()
        return {
            x: pos.x-canvasBox.x,
            y: pos.y-canvasBox.y,
        }
    }

    mouseDown(e){
        this.mousePressed = true
        let pos = this.getCanvasCoordinates(e)
        let element = this.getElementNearPos(pos)
        if(element ) {
            let port = this.getPortAtPos(element,pos)

            if(port) {
                let connected = this.getConnected(port)
                if(connected) {
                    this.grabbedWire = connected
                    this.grabbedWire.endPort.pos = pos
                }
                else {
                    this.grabbedWire = this.addWire(port, {...pos})
                    console.log("added wire")
                }
            }
            else if(this.canDrag(element,pos))    {
                this.grabbedElement = element
                console.log(element)
            }
        }
        console.log(pos)
    }
    mouseMove(e){
        let pos = this.getCanvasCoordinates(e)
        if(this.mousePressed){
            let dPos = {x:pos.x-this.prevPos.x,y:pos.y-this.prevPos.y}
            if(this.grabbedElement) {
                this.moveElement(this.grabbedElement,dPos)
            }
            if(this.grabbedWire) {
                console.log("wire")
                this.moveWireEnd(this.grabbedWire,pos)
            }
        }
        this.prevPos = pos
    }
    mouseUp(e){
        let pos = this.getCanvasCoordinates(e)
        this.mousePressed = false
        if(this.grabbedElement) {
            this.elements = this.elements.filter(x=>x!==this.grabbedElement)
            this.elements.push(Object.assign( {},this.grabbedElement))
            this.grabbedElement=undefined
        }
        if(this.grabbedWire) {
            let element = this.getElementNearPos(pos)
            if(element ) {
                let port = this.getPortAtPos(element,pos)
                if(port && this.canConnect(this.grabbedWire,port)){
                    this.grabbedWire.endPort=port
                    this.grabbedWire=undefined
                }
                else {
                    this.wires = this.wires.filter(x=>x!==this.grabbedWire)
                    this.grabbedWire=undefined
                }
            }
            else {
                this.wires = this.wires.filter(x=>x!==this.grabbedWire)
                this.grabbedWire=undefined
            }
            this.update()
        }
        console.log(pos)
    }

    initMouse(){
        this.mousePressed = false
        this.canvas.addEventListener("mousedown",(e)=>this.mouseDown(e))
        this.canvas.addEventListener("mousemove",(e)=>this.mouseMove(e))
        this.canvas.addEventListener("mouseup",(e)=>this.mouseUp(e))
    }

    addElement(pos,name,type,draggable){
        this.elements.push({name:name,type:type,pos:pos,draggable:draggable})
    }

    moveElement(element,dPos){
        element.pos = {x:element.pos.x+dPos.x,y:element.pos.y+dPos.y}
        this.update()
    }

    addWire(start,end){
        let wire = {startPort:start,endPort:end}
        this.wires.push(wire)
        return wire
    }

    moveWireEnd(wire,pos) {
        wire.endPort.pos=pos
        this.update()
    }

    update(){
        this.ctx.clearRect(0,0,this.width,this.height)
        this.drawAllElements()
        this.drawAllWires()
        this.drawToolbar()
    }

    drawAllElements(){
        this.elements.forEach(element=>{
            if(element!==this.grabbedElement)
            this.drawElement(element.pos,element.type)
        })
        if(this.grabbedElement)
            this.drawElement(this.grabbedElement.pos,this.grabbedElement.type)
    }

    drawAllWires(){
        this.wires.forEach(wire=> this.drawWire(this.getPortPos(wire.startPort),this.getPortPos(wire.endPort)))
    }

    getPortPos(port){
        if(port.pos) return port.pos
        let element = this.elements.find(x=>x.name===port.element.name)
        if (!element) {
            console.error("element not found")
            return {x:0,y:0}
        }
        let inputs = this.getElementInputPortsPos(element)
        let output = this.getElementOutputPortPos(element)
        if(port.type==="output") return output
        return inputs[port.id]
    }

    getElementNearPos(pos) {
        return this.elements.reverse().find(e=>{
            let size = this.getElementSize(e.type)
            let tl = {x:e.pos.x - size.width/2 - this.portSize,y:e.pos.y-size.height/2}
            let br = {x:e.pos.x + size.width/2 + this.portSize,y:e.pos.y+size.height/2}
            return (pos.x >= tl.x && pos.x <= br.x) && (pos.y >= tl.y && pos.y <= br.y)
        })
    }

    canDrag(element,pos) {
        if(element.draggable!==true) return false;
        let size = this.getElementSize(element.type)
        let tl = {x:element.pos.x - size.width/2,y:element.pos.y-size.height/2}
        let br = {x:element.pos.x + size.width/2,y:element.pos.y+size.height/2}
        return (pos.x >= tl.x && pos.x < br.x) && (pos.y >= tl.y && pos.y < br.y)
    }

    getConnected(port){
        return this.wires.find(w=>{
            return w.endPort.element.name === port.element.name &&
                w.endPort.id === port.id &&
                w.endPort.type === port.type
        })
    }

    canConnect(wire,port) {
        if(wire.startPort.type===port.type) return false
        if(port.type==="output") return true
        let connected = this.wires.find(w=>{
            if(wire===w) return false
            return w.endPort.element.name === port.element.name &&
                   w.endPort.id === port.id
        })
        return !connected
    }

    getPortAtPos(element,pos) {
        let inputs = this.getElementInputPortsPos(element)
        let output = this.getElementOutputPortPos(element)
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
            return {element:element,type:type,id:id}
        else
            return undefined
    }

    drawWire(start,end){
        this.ctx.beginPath();
        this.ctx.moveTo(start.x, start.y);
        this.ctx.lineTo(end.x, end.y);
        this.ctx.lineWidth=this.wireWidth
        this.ctx.strokeStyle="black"
        this.ctx.stroke();
    }


    drawToolbar(toolbar){
        let toolBarHeight = 100;
        this.ctx.fillStyle="gray";
        this.ctx.fillRect(0,this.height - toolBarHeight, this.width,toolBarHeight);
    }

    getElementSize(element){
        if(element!=='not')
            return {width: 160, height: 80}
        else {
            return {width: 160, height: 40}
        }
    }
    getElementInputPortCount(element) {
        if(element!=='not')
            return 2;
        else {
            return 1;
        }
    }
    getElementColor(element){
        return "lightgray"
    }

    getElementInputPortsPos(element){
        let iPorts = this.getElementInputPortCount(element.type)
        let elementSize = this.getElementSize(element.type)
        if(iPorts===1){
            return [{x: element.pos.x-elementSize.width/2,y: element.pos.y}]
        }
        else {
            return [
                {x: element.pos.x - elementSize.width/2, y: element.pos.y - elementSize.height/4},
                {x: element.pos.x - elementSize.width/2, y: element.pos.y + elementSize.height/4}
            ]
        }
    }
    getElementOutputPortPos(element){
        let elementSize = this.getElementSize(element)
        return {x: element.pos.x+elementSize.width/2,y: element.pos.y}
    }

    drawElement(pos,type) {
        this.ctx.fillStyle=this.getElementColor(type);
        let elementSize = this.getElementSize(type)
        this.ctx.fillRect(pos.x - elementSize.width/2,pos.y - elementSize.height/2,
            elementSize.width,elementSize.height)
        this.ctx.fillStyle="black"
        this.ctx.fillText(type.toString().toUpperCase(),pos.x,pos.y,elementSize.width)

        this.drawPort({x: pos.x+elementSize.width/2,y: pos.y},this.portSize)

        let iPorts = this.getElementInputPortCount(type)
        if(iPorts===1){
            this.drawPort({x: pos.x-elementSize.width/2,y: pos.y},this.portSize)
        }
        else {
            this.drawPort({x: pos.x - elementSize.width/2, y: pos.y + elementSize.height/4}, this.portSize)
            this.drawPort({x: pos.x - elementSize.width/2, y: pos.y - elementSize.height/4}, this.portSize)
        }

    }

    drawPort(pos,size){
        this.ctx.fillStyle="black"
        this.ctx.beginPath();
        this.ctx.arc(pos.x-1, pos.y, size, 0, 2 * Math.PI);
        this.ctx.fill();
    }
}