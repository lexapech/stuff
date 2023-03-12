
export default class LogicSimView {
    constructor(controller) {
        this.controller = controller
        this.wireWidth = 6;
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
            this.ctx.fillStyle="white";
            this.ctx.font = "bold 20px Rubik, sans-serif";

        }

    }

    clear(){
        this.ctx.fillStyle="#8FD9DE";
        this.ctx.fillRect(0,0,this.width,this.height);
        this.ctx.fillStyle = "#ACE9ED";
        this.ctx.fillRect(this.width/2, 0, this.width, this.height);

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
        this.ctx.fillStyle="#68BAC6";
        this.ctx.fillRect(0,this.height - toolbar.toolbarHeight, this.width, toolbar.toolbarHeight);
        toolbar.elements.forEach(x=>{
            this.drawElement(x)
            let ports = x.getPorts()
            ports.forEach(port => this.drawPort(port))
        })

    }
    drawWires(wires){
        wires.forEach(wire=>this.drawWire(wire))
    }

    drawWire(wire){
        let start = wire.startPort.getPos();
        let end =  wire.endPort.getPos()
        this.ctx.beginPath();
        this.ctx.moveTo(start.x, start.y);
        this.ctx.lineTo(end.x, end.y);
        this.ctx.lineWidth=this.wireWidth;
        if(wire.active) this.ctx.strokeStyle="yellow"
        else {
            if (wire.startPort.element.draggable || wire.startPort.type==="common" || wire.startPort.element.name==="input_bar") this.ctx.strokeStyle = "white";
            else this.ctx.strokeStyle = "#1A768A"
        }
        this.ctx.stroke();
    }

    getElementColor(element) {
        if (element.draggable) return "#C199D3";
        if(element.name === "output_bar") {
            return "#8FD9DE"
        }
        else if(element.name === "input_bar"){
            return "#ACE9ED"
        }
        else if(element.name === "common_bar"){
            return "#8FD9DE"
        }
        else {
            return "#9772AB"
        }
    }

    drawElement(element) {
        this.ctx.fillStyle = this.getElementColor(element)
        let elementSize = element.getSize()
        this.ctx.fillRect(element.pos.x - elementSize.width/2,element.pos.y - elementSize.height/2,
            elementSize.width,elementSize.height);
        this.ctx.fillStyle="white";
        if(element.type!=='bar'){
            this.ctx.fillText(element.type.toString().toUpperCase(),element.pos.x,element.pos.y,elementSize.width)
        }
    }

    drawPort(port){
        let pos = port.getPos()
        if (port.element.type === 'bar') {
            this.ctx.fillStyle = "#1A768A";
            if(port.type==="input" && !port.element.portValid[port.id])
                this.ctx.fillStyle = "#FF768A";
        }
        else if (port.element.draggable) this.ctx.fillStyle = "#A04899";
        else this.ctx.fillStyle="#71416C";
        this.ctx.beginPath();
        this.ctx.arc(pos.x, pos.y, port.size, 0, 2 * Math.PI);
        this.ctx.fill();
        this.ctx.fillStyle="white";
        this.ctx.border = "5px solid red";
        if(port.element.type==='bar'){
            this.ctx.fillText(port.element.portValues[port.id].toString(), pos.x, pos.y - pos.y%2 + 2, port.size)
        }
    }

}