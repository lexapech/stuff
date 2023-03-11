
export default class LogicSimView {
    constructor(controller) {
        this.controller = controller
        this.wireWidth = 2;
        this.initCanvas(controller.canvas)
    }

    initCanvas(canvas){
       this.canvas=canvas
        canvas.style.width="100%";
        canvas.style.height="100%";
        canvas.width = canvas.clientWidth
        canvas.height = canvas.clientHeight
        canvas.style.imageRendering="crisp-edges"
        this.canvas = canvas
        this.width = canvas.clientWidth
        this.height = canvas.clientHeight

        if(canvas.getContext('2d')) {
            this.ctx = canvas.getContext('2d');
            this.ctx.fillStyle="black";
            this.ctx.textAlign="center"
            this.ctx.textBaseline="middle"
            this.ctx.font = "24px serif";

        }

    }

    clear(){
        this.ctx.clearRect(0,0,this.width,this.height)
    }
    drawElements(elements,grabbedElement){
        elements.forEach(element=>{
            if(element!==grabbedElement)
                this.drawElement(element)
        })
    }
    drawPorts(elements,grabbedElement){
        elements.forEach(element=>{
            if(element!==grabbedElement) {
                let ports = element.getPorts()
                ports.forEach(port => this.drawPort(port))
            }

        })
    }

    drawToolbar(toolbar){
        this.ctx.fillStyle="gray";
        this.ctx.fillRect(0,this.height - toolbar.toolbarHeight, this.width,toolbar.toolbarHeight);
        toolbar.elements.forEach(x=>{
            this.drawElement(x)
            let ports = x.getPorts()
            ports.forEach(port => this.drawPort(port))
        })

    }
    drawWires(wires){
        wires.forEach(wire=> {
            this.drawWire(wire.startPort.getPos(),
                          wire.endPort.getPos())

        })
    }

    drawWire(start,end){
        this.ctx.beginPath();
        this.ctx.moveTo(start.x, start.y);
        this.ctx.lineTo(end.x, end.y);
        this.ctx.lineWidth=this.wireWidth
        this.ctx.strokeStyle="black"
        this.ctx.stroke();
    }

    getElementColor(element) {
        return (element.draggable || element.name==='toolbar' || element.type==='bar')?"lightgray":"gray"
    }

    drawElement(element) {
        this.ctx.fillStyle = this.getElementColor(element)
        let elementSize = element.getSize()
        this.ctx.fillRect(element.pos.x - elementSize.width/2,element.pos.y - elementSize.height/2,
            elementSize.width,elementSize.height)
        this.ctx.fillStyle="black"
        if(element.type!=='bar')
            this.ctx.fillText(element.type.toString().toUpperCase(),element.pos.x,element.pos.y,elementSize.width)
    }

    drawPort(port){
        let pos = port.getPos()
        this.ctx.fillStyle="black"
        this.ctx.beginPath();
        this.ctx.arc(pos.x, pos.y, port.size, 0, 2 * Math.PI);
        this.ctx.fill();
        this.ctx.fillStyle="white"
        if(port.element.type==='bar' && port.type !=="common")
            this.ctx.fillText(port.element.portValues[port.id].toString(), pos.x, pos.y - pos.y%2 + 2, port.size)
    }

}