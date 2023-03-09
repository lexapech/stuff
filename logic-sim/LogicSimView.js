
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

    drawToolbar(toolbar){
        this.ctx.fillStyle="gray";
        this.ctx.fillRect(0,this.height - toolbar.toolbarHeight, this.width,toolbar.toolbarHeight);
        toolbar.elements.forEach(x=>{
            this.drawElement(x)
        })

    }
    drawWires(wires){
        wires.forEach(wire=> {
            this.drawWire(this.controller.core.getPortPos(wire.startPort),
                          this.controller.core.getPortPos(wire.endPort))

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

    drawElement(element) {
        this.ctx.fillStyle="lightgray"
        let elementSize = element.getSize()
        this.ctx.fillRect(element.pos.x - elementSize.width/2,element.pos.y - elementSize.height/2,
            elementSize.width,elementSize.height)
        this.ctx.fillStyle="black"
        this.ctx.fillText(element.type.toString().toUpperCase(),element.pos.x,element.pos.y,elementSize.width)

        this.drawPort(element.getOutputPortPos(),this.controller.portSize)

        let ports = element.getInputPortsPos()
        ports.forEach(port=>{
            this.drawPort(port,this.controller.portSize)
        })
    }

    drawPort(pos,size){
        this.ctx.fillStyle="black"
        this.ctx.beginPath();
        this.ctx.arc(pos.x-1, pos.y, size, 0, 2 * Math.PI);
        this.ctx.fill();
    }

}